import re
import os
import json
from pathlib import Path

def scan_all_strings(root_dir):
    """
    扫描所有 C# 文件中的字符串：
    - 双引号字符串 "xxx"
    - 逐字字符串 @"xxx"
    - 插值字符串 $"{xxx}"
    - 字符 'x' (单字符)
    """
    all_strings = set()
    strings_with_context = []

    # 正则模式 - 匹配各种字符串
    patterns = [
        # 普通字符串 "xxx"
        (r'"([^"\\]*(?:\\.[^"\\]*)*)"', 'double'),
        # 逐字字符串 @"xxx"
        (r'@("(?:[^"]|"")*")', 'verbatim'),
        # 插值字符串 $"{xxx}"
        (r'\$"([^"\\]*(?:\\.[^"\\]*)*)"', 'interpolated'),
        # 字符 'x'
        (r"'([^'\\]*(?:\\.[^'\\]*)*)'", 'char'),
    ]

    # 过滤模式：排除这些关键字/模式
    exclude_patterns = [
        r'^_loc$',           # _loc 本身
        r'^localization$',   # localization 本身
        r'^spritebatch$',    # spritebatch
        r'^graphicsdevice$', # graphicsdevice
        r'^content$',        # content
        r'^game1$',          # game1
        r'^player$',         # player
        r'^enemy$',          # enemy
        r'^bullet$',         # bullet
        r'^vector2$',        # vector2
        r'^color$',          # color
        r'^rectangle$',      # rectangle
        r'^keys\.',          # keys.w, keys.s 等
        r'^buttonstate\.',   # buttonstate.pressed
        r'^true$', r'^false$', r'^null$', r'^this$', r'^base$',
        r'^gettype$', r'^tostring$', r'^equals$', r'^gethashcode$',
        r'^\d+$',            # 纯数字
        r'^[a-z0-9_]+$',     # 纯英文变量名（不含中文）
    ]

    for root, dirs, files in os.walk(root_dir):
        # 跳过不需要的文件夹
        dirs[:] = [d for d in dirs if d not in ['bin', 'obj', 'Content', 'data', 'fonts', 'sound', 'lang', 'properties']]

        for file in files:
            if not file.endswith('.cs'):
                continue

            path = os.path.join(root, file)
            try:
                with open(path, 'r', encoding='utf-8') as f:
                    content = f.read()
                    lines = content.split('\n')

                    for pattern, ptype in patterns:
                        for match in re.finditer(pattern, content, re.DOTALL):
                            string_content = match.group(1)

                            # 清理字符串内容
                            if ptype == 'verbatim':
                                # 处理逐字字符串中的双引号转义
                                string_content = string_content.replace('""', '"')

                            # 跳过空字符串
                            if not string_content or string_content.strip() == '':
                                continue

                            # 跳过纯英文/数字/下划线（不含任何中文）
                            if re.match(r'^[a-zA-Z0-9_\.\-]+$', string_content):
                                # 检查是否包含中文 - 如果没有中文就跳过
                                if not re.search(r'[\u4e00-\u9fff\u3040-\u309f\u30a0-\u30ff]', string_content):
                                    continue

                            # 检查是否应该排除
                            should_exclude = False
                            for ex_pattern in exclude_patterns:
                                if re.match(ex_pattern, string_content, re.IGNORECASE):
                                    should_exclude = True
                                    break

                            if should_exclude:
                                continue

                            # 获取行号
                            line_num = content[:match.start()].count('\n') + 1

                            # 获取上下文
                            start_line = max(0, line_num - 2)
                            end_line = min(len(lines), line_num + 1)
                            context = '\n'.join(lines[start_line:end_line])

                            strings_with_context.append({
                                'text': string_content,
                                'path': path,
                                'line': line_num,
                                'context': context.strip(),
                                'type': ptype
                            })
                            all_strings.add(string_content)

            except Exception as e:
                print(f"Error reading {path}: {e}")

    return all_strings, strings_with_context

def is_comment_or_attribute(line):
    """检查是否是注释行或属性行"""
    stripped = line.strip()
    return (stripped.startswith('//') or
            stripped.startswith('/*') or
            stripped.startswith('*') or
            stripped.startswith('///') or
            stripped.startswith('[assembly') or
            stripped.startswith('using '))

def contains_chinese(text):
    """检查是否包含中文字符"""
    return bool(re.search(r'[\u4e00-\u9fff\u3040-\u309f\u30a0-\u30ff]', text))

def categorize_strings(strings_with_context):
    """分类字符串"""
    categories = {
        'ui_text': [],      # UI显示文本
        'log_messages': [], # 控制台/调试消息
        'file_paths': [],   # 文件路径
        'error_messages': [], # 错误消息
        'other': []         # 其他
    }

    ui_patterns = [
        r'spritebatch\.drawstring',
        r'DrawString',
        r'WriteLine',
        r'Console\.',
        r'Debug\.',
        r'MessageBox',
        r'Label',
        r'Text\s*=',
        r'Title\s*=',
        r'Description\s*=',
    ]

    log_patterns = [
        r'Console\.Write',
        r'Debug\.Write',
        r'System\.Diagnostics',
        r'print\(',
        r'Log\s*\.',
    ]

    path_patterns = [
        r'\.txt',
        r'\.json',
        r'\.dat',
        r'\.png',
        r'\.wav',
        r'\.mp3',
        r'\.ttf',
        r'Path\.Combine',
        r'Content/',
        r'game/',
    ]

    error_patterns = [
        r'Exception',
        r'Error',
        r'Failed',
        r'Warning',
        r'Invalid',
    ]

    for item in strings_with_context:
        text = item['text']
        context = item['context'].lower()

        # 跳过明显的代码标识符
        if re.match(r'^[a-z_][a-z0-9_]*$', text, re.IGNORECASE) and not contains_chinese(text):
            continue

        # 分类
        if any(re.search(p, context, re.IGNORECASE) for p in ui_patterns):
            categories['ui_text'].append(item)
        elif any(re.search(p, context, re.IGNORECASE) for p in log_patterns):
            categories['log_messages'].append(item)
        elif any(re.search(p, text, re.IGNORECASE) for p in path_patterns):
            categories['file_paths'].append(item)
        elif any(re.search(p, context, re.IGNORECASE) for p in error_patterns):
            categories['error_messages'].append(item)
        else:
            categories['other'].append(item)

    return categories

def generate_translation_json(all_strings, strings_with_context, lang_dir):
    """生成翻译 JSON 文件"""
    os.makedirs(lang_dir, exist_ok=True)

    # 创建翻译字典
    zh_dict = {}
    en_dict = {}

    # 加载现有翻译（如果存在）
    zh_path = os.path.join(lang_dir, 'zh.json')
    en_path = os.path.join(lang_dir, 'en.json')

    if os.path.exists(zh_path):
        with open(zh_path, 'r', encoding='utf-8') as f:
            zh_dict = json.load(f)
    if os.path.exists(en_path):
        with open(en_path, 'r', encoding='utf-8') as f:
            en_dict = json.load(f)

    # 添加新字符串
    for s in sorted(all_strings):
        if s not in zh_dict:
            zh_dict[s] = s  # 默认中文就是原文
        if s not in en_dict:
            # 英文翻译需要手动填写
            en_dict[s] = ''

    # 保存中文 JSON
    with open(zh_path, 'w', encoding='utf-8') as f:
        json.dump(zh_dict, f, ensure_ascii=False, indent=2, sort_keys=True)

    # 保存英文 JSON
    with open(en_path, 'w', encoding='utf-8') as f:
        json.dump(en_dict, f, ensure_ascii=False, indent=2, sort_keys=True)

    return zh_dict, en_dict

def generate_report(categories, lang_dir):
    """生成详细报告"""
    report_lines = []
    report_lines.append("=" * 60)
    report_lines.append("字符串扫描报告")
    report_lines.append("=" * 60)

    for category, items in categories.items():
        if items:
            report_lines.append(f"\n【{category.upper()}】({len(items)} 个)")
            for item in items[:20]:  # 最多显示20个
                rel_path = Path(item['path']).name
                text_preview = item['text'][:60] + ('...' if len(item['text']) > 60 else '')
                report_lines.append(f"  [{rel_path}:{item['line']}] {text_preview}")
            if len(items) > 20:
                report_lines.append(f"  ... 共 {len(items)} 个")

    report_lines.append(f"\n{'=' * 60}")
    report_lines.append(f"语言文件位置: {lang_dir}")
    report_lines.append(f"  - zh.json: 中文原文")
    report_lines.append(f"  - en.json: 需要翻译的英文")
    report_lines.append("=" * 60)

    return '\n'.join(report_lines)

def main():
    # 设置路径
    script_dir = Path(__file__).parent
    project_dir = script_dir / 'Purge_v0.4.0'

    if not project_dir.exists():
        # 尝试其他常见位置
        possible_dirs = [
            script_dir,
            script_dir.parent,
            Path.cwd() / 'Purge_v0.4.0',
            Path.cwd(),
        ]
        for d in possible_dirs:
            if (d / 'game1.cs').exists() or (d / 'game' / 'script').exists():
                project_dir = d
                break

    script_dir_path = project_dir / 'game' / 'script'
    lang_dir = project_dir / 'game' / 'lang'

    if not script_dir_path.exists():
        # 也可能是直接在项目根目录
        script_dir_path = project_dir
        lang_dir = project_dir / 'game' / 'lang'

    print(f"项目目录: {project_dir}")
    print(f"扫描目录: {script_dir_path}")
    print(f"语言目录: {lang_dir}")
    print()

    if not script_dir_path.exists():
        print(f"错误：找不到脚本目录")
        print("请确认目录结构，或将脚本放在项目根目录")
        return

    # 扫描所有字符串
    print("正在扫描 C# 文件...")
    all_strings, strings_with_context = scan_all_strings(str(script_dir_path))

    # 分类
    categories = categorize_strings(strings_with_context)

    # 生成翻译文件
    zh_dict, en_dict = generate_translation_json(all_strings, strings_with_context, str(lang_dir))

    # 生成报告
    report = generate_report(categories, lang_dir)
    print(report)

    # 保存报告到文件
    report_path = lang_dir / 'scan_report.txt'
    with open(report_path, 'w', encoding='utf-8') as f:
        f.write(report)

    print(f"\n详细报告已保存到: {report_path}")
    print(f"共发现 {len(all_strings)} 个唯一字符串")
    print(f"  - UI文本: {len(categories['ui_text'])}")
    print(f"  - 日志消息: {len(categories['log_messages'])}")
    print(f"  - 文件路径: {len(categories['file_paths'])}")
    print(f"  - 错误消息: {len(categories['error_messages'])}")
    print(f"  - 其他: {len(categories['other'])}")

if __name__ == "__main__":
    main()
