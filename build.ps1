# build.ps1 - Build Automation Script for Marker Crystal

Write-Host "🚀 Starting Marker Crystal Build Process..." -ForegroundColor Green

# Step 1: Clean previous build
Write-Host "🧹 Cleaning previous build..." -ForegroundColor Yellow
Remove-Item "./publish" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "*.zip" -Force -ErrorAction SilentlyContinue

# Step 2: Restore dependencies
Write-Host "📦 Restoring dependencies..." -ForegroundColor Yellow
dotnet restore

# Step 3: Build to EXE
Write-Host "🔨 Building to executable..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o "./publish"

# Step 4: Create ZIP package
Write-Host "📦 Creating distribution package..." -ForegroundColor Yellow
$version = Get-Date -Format "yyyy-MM-dd_HH-mm"
$zipName = "marker-dotnet-v$version.zip"
Compress-Archive -Path "./publish/*" -DestinationPath $zipName -Force

# Step 5: Show results
Write-Host "✅ Build completed successfully!" -ForegroundColor Green
Write-Host "📁 Output folder: ./publish" -ForegroundColor Cyan
Write-Host "📦 Package: $zipName" -ForegroundColor Cyan

$exeSize = (Get-Item "./publish/marker-dotnet.exe").Length / 1MB
$zipSize = (Get-Item $zipName).Length / 1MB
Write-Host "📊 EXE Size: $([math]::Round($exeSize, 2)) MB" -ForegroundColor Cyan
Write-Host "📊 ZIP Size: $([math]::Round($zipSize, 2)) MB" -ForegroundColor Cyan

# Step 6: Test build
Write-Host "🧪 Testing build..." -ForegroundColor Yellow
if (Test-Path "./publish/marker-dotnet.exe") {
    Write-Host "✅ EXE file created successfully" -ForegroundColor Green
    Write-Host "🎯 Ready for distribution!" -ForegroundColor Green
} else {
    Write-Host "❌ EXE file not found" -ForegroundColor Red
}

Write-Host "🎉 Build process completed!" -ForegroundColor Green
Write-Host "📝 Run .\publish\marker-dotnet.exe to test" -ForegroundColor Gray
