# fix_usings.ps1
$files = Get-ChildItem -Recurse -Filter *.cs

# 需要添加的 using 语句
$usings = @'
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

'@

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw

    # 检查是否已经有 using 语句
    if ($content -notmatch 'using System;') {
        # 在文件开头添加 using（如果有 namespace，放在 namespace 之前）
        if ($content -match 'namespace') {
            $content = $usings + $content
        } else {
            $content = $usings + $content
        }
        Set-Content $file.FullName -Value $content -NoNewline
        Write-Host "Added usings to: $($file.Name)"
    } else {
        Write-Host "Already has usings: $($file.Name)"
    }
}

Write-Host "Done!"
