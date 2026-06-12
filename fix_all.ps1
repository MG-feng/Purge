# 修复 Game1.cs 中的 SpriteFont.default 问题
$game1 = Get-Content Game1.cs -Raw
$game1 = $game1 -replace '_font = SpriteFont\.default;', '_font = null; // TODO: 加载字体文件'
$game1 = $game1 -replace '_smallfont = SpriteFont\.default;', '_smallfont = null; // TODO: 加载字体文件'
$game1 = $game1 -replace '_titlefont = SpriteFont\.default;', '_titlefont = null; // TODO: 加载字体文件'
Set-Content Game1.cs -Value $game1 -NoNewline
Write-Host "Fixed Game1.cs"

# 同样修复其他 UI 文件中的类似问题
$uiFiles = @(
    "game/script/ui/menumanager.cs",
    "game/script/ui/uimanager.cs",
    "game/script/ui/healthbar.cs",
    "game/script/ui/staminabar.cs"
)

foreach ($file in $uiFiles) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        $content = $content -replace 'SpriteFont\.default', 'null'
        $content = $content -replace '_font\s*=\s*null;', '_font = null; // TODO: 加载字体文件'
        Set-Content $file -Value $content -NoNewline
        Write-Host "Fixed: $file"
    }
}

Write-Host "Done! All font issues fixed."
