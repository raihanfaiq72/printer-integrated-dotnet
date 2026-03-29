# watch-build.ps1 - Auto Build When Files Change

Write-Host "👀 Auto-Build Watcher Started" -ForegroundColor Green
Write-Host "🔄 Watching for changes in:" -ForegroundColor Cyan
Write-Host "   - *.cs files" -ForegroundColor White
Write-Host "   - *.xaml files" -ForegroundColor White
Write-Host "   - *.csproj files" -ForegroundColor White
Write-Host "📝 Press Ctrl+C to stop watching" -ForegroundColor Yellow
Write-Host ""

$lastBuild = Get-Date
$buildCount = 0

while ($true) {
    # Check for changes
    $changedFiles = Get-ChildItem -Recurse -Filter "*.cs" -ErrorAction SilentlyContinue | 
        Where-Object { $_.LastWriteTime -gt $lastBuild }
    
    $changedXaml = Get-ChildItem -Recurse -Filter "*.xaml" -ErrorAction SilentlyContinue | 
        Where-Object { $_.LastWriteTime -gt $lastBuild }
    
    $changedProj = Get-ChildItem -Filter "*.csproj" -ErrorAction SilentlyContinue | 
        Where-Object { $_.LastWriteTime -gt $lastBuild }
    
    if ($changedFiles -or $changedXaml -or $changedProj) {
        $buildCount++
        Write-Host "🔄 File changes detected! Auto-build #$buildCount..." -ForegroundColor Yellow
        $lastBuild = Get-Date
        
        try {
            # Auto build
            Write-Host "🔨 Building..." -ForegroundColor Gray
            dotnet publish -c Release -r win-x64 --self-contained `
                -p:PublishSingleFile=true `
                -p:IncludeNativeLibrariesForSelfExtract=true `
                -o "./publish" --no-restore
            
            if (Test-Path "./publish/marker-dotnet.exe") {
                $exeSize = (Get-Item "./publish/marker-dotnet.exe").Length / 1MB
                Write-Host "✅ Auto-build #$buildCount completed! ($([math]::Round($exeSize, 2)) MB)" -ForegroundColor Green
                Write-Host "📁 EXE ready at: ./publish/marker-dotnet.exe" -ForegroundColor Cyan
            } else {
                Write-Host "❌ Build failed - EXE not found" -ForegroundColor Red
            }
        } catch {
            Write-Host "❌ Build failed: $_" -ForegroundColor Red
        }
        
        Write-Host "👀 Continuing to watch..." -ForegroundColor Gray
        Write-Host ""
    }
    
    Start-Sleep -Seconds 3
}
