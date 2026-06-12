# fix_syntax.ps1
Get-ChildItem -Recurse -Filter *.cs | ForEach-Object {
    $content = Get-Content $_.FullName -Raw

    # 类型名称修复
    $content = $content -replace '\blist<', 'List<'
    $content = $content -replace '\bspritefont\b', 'SpriteFont'
    $content = $content -replace '\btexture2d\b', 'Texture2D'
    $content = $content -replace '\bspritebatch\b', 'SpriteBatch'
    $content = $content -replace '\bgraphicsdevicemanager\b', 'GraphicsDeviceManager'
    $content = $content -replace '\bgametime\b', 'GameTime'
    $content = $content -replace '\bvector2\b', 'Vector2'
    $content = $content -replace '\brectangle\b', 'Rectangle'
    $content = $content -replace '\bcolor\b', 'Color'
    $content = $content -replace '\bkeys\b', 'Keys'
    $content = $content -replace '\bbuttonstate\b', 'ButtonState'
    $content = $content -replace '\bmousestate\b', 'MouseState'
    $content = $content -replace '\bkeyboardstate\b', 'KeyboardState'
    $content = $content -replace '\bdatetime\b', 'DateTime'
    $content = $content -replace '\bhashset\b', 'HashSet'
    $content = $content -replace '\bdictionary\b', 'Dictionary'

    Set-Content $_.FullName -Value $content -NoNewline
    Write-Host "Fixed: $($_.Name)"
}
