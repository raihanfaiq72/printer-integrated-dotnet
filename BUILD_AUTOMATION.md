# 🚀 Build Automation Guide

## **📋 Quick Build Commands**

### **🔥 Development Build (Cepat):**
```bash
# Development mode - langsung main window
dotnet run
```

### **🏭 Production Build (Setup Window):**
```bash
# Production mode - dengan setup window
dotnet run --configuration Release
```

### **📦 EXE Build (Distribusi):**
```bash
# Build ke executable standalone
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "./publish"
```

## **🔄 Build Workflow Otomatis**

### **📝 Script Build PowerShell:**

Buat file `build.ps1`:
```powershell
# build.ps1 - Build Automation Script

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
Write-Host "📊 Size: $((Get-Item $zipName).Length / 1MB) MB" -ForegroundColor Cyan

# Step 6: Test build (optional)
Write-Host "🧪 Testing build..." -ForegroundColor Yellow
if (Test-Path "./publish/marker-dotnet.exe") {
    Write-Host "✅ EXE file created successfully" -ForegroundColor Green
} else {
    Write-Host "❌ EXE file not found" -ForegroundColor Red
}

Write-Host "🎉 Build process completed!" -ForegroundColor Green
```

### **📝 Batch File (Windows):**

Buat file `build.bat`:
```batch
@echo off
echo 🚀 Starting Marker Crystal Build Process...

:: Step 1: Clean
echo 🧹 Cleaning previous build...
if exist publish rmdir /s /q publish
if exist *.zip del *.zip

:: Step 2: Build
echo 🔨 Building to executable...
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "./publish"

:: Step 3: Create package
echo 📦 Creating distribution package...
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%"
set "version=%YY%-%MM%-%DD%_%HH%-%Min%"
set "zipName=marker-dotnet-v%version%.zip"

powershell Compress-Archive -Path "./publish/*" -DestinationPath "%zipName%" -Force

:: Step 4: Show results
echo ✅ Build completed successfully!
echo 📁 Output folder: ./publish
echo 📦 Package: %zipName%

echo 🎉 Build process completed!
pause
```

## **⚡ One-Click Build**

### **🎯 Cara Pakai:**

#### **PowerShell (Recommended):**
```bash
# Jalankan script build
.\build.ps1
```

#### **Batch File:**
```bash
# Jalankan batch file
build.bat
```

## **🔄 Build Otomatis Setiap Perubahan**

### **📝 Watch Script (Auto-build):**

Buat file `watch-build.ps1`:
```powershell
# watch-build.ps1 - Auto build saat file berubah

Write-Host "👀 Watching for file changes..." -ForegroundColor Green
Write-Host "🔄 Auto-build enabled for:" -ForegroundColor Cyan
Write-Host "   - *.cs files" -ForegroundColor White
Write-Host "   - *.xaml files" -ForegroundColor White
Write-Host "   - *.csproj files" -ForegroundColor White
Write-Host "Press Ctrl+C to stop watching" -ForegroundColor Yellow

$lastBuild = Get-Date

while ($true) {
    # Check for changes
    $changedFiles = Get-ChildItem -Recurse -Filter "*.cs" | 
        Where-Object { $_.LastWriteTime -gt $lastBuild }
    
    $changedXaml = Get-ChildItem -Recurse -Filter "*.xaml" | 
        Where-Object { $_.LastWriteTime -gt $lastBuild }
    
    $changedProj = Get-ChildItem -Filter "*.csproj" | 
        Where-Object { $_.LastWriteTime -gt $lastBuild }
    
    if ($changedFiles -or $changedXaml -or $changedProj) {
        Write-Host "🔄 File changes detected! Auto-building..." -ForegroundColor Yellow
        $lastBuild = Get-Date
        
        try {
            # Auto build
            dotnet publish -c Release -r win-x64 --self-contained `
                -p:PublishSingleFile=true `
                -p:IncludeNativeLibrariesForSelfExtract=true `
                -o "./publish"
            
            Write-Host "✅ Auto-build completed!" -ForegroundColor Green
            Write-Host "📁 EXE ready at: ./publish/marker-dotnet.exe" -ForegroundColor Cyan
        } catch {
            Write-Host "❌ Build failed: $_" -ForegroundColor Red
        }
        
        Write-Host "👀 Continuing to watch..." -ForegroundColor Gray
    }
    
    Start-Sleep -Seconds 2
}
```

### **🎯 Cara Pakai Auto-build:**
```bash
# Start auto-build watcher
.\watch-build.ps1

# Edit file Anda, build otomatis terjadi
# Tekan Ctrl+C untuk stop
```

## **📋 Build Commands Reference**

### **🔧 Development Commands:**
```bash
# Quick test
dotnet run

# Production test
dotnet run --configuration Release

# Clean build
dotnet clean && dotnet build

# Restore packages
dotnet restore
```

### **📦 Distribution Commands:**
```bash
# Full build (recommended)
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "./publish"

# Smaller build (requires .NET runtime)
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -o "./publish-framework-dependent"

# Debug build
dotnet publish -c Debug -r win-x64 --self-contained -p:PublishSingleFile=true -o "./publish-debug"
```

## **🎯 Build Optimization**

### **⚡ Faster Builds:**
```bash
# Incremental build (hanya yang berubah)
dotnet publish -c Release --no-restore -r win-x64 --self-contained -p:PublishSingleFile=true -o "./publish"

# Parallel build
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=false -o "./publish"
```

### **📏 Size Optimization:**
```bash
# Trim unused code (smaller size)
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true -o "./publish-minimal"

# ReadyToRun (faster startup)
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:PublishReadyToRun=true -o "./publish-fast"
```

## **🔄 CI/CD Pipeline**

### **📝 GitHub Actions:**
```yaml
# .github/workflows/build.yml
name: Build and Publish

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 10.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
    
    - name: Upload artifact
      uses: actions/upload-artifact@v3
      with:
        name: marker-dotnet-exe
        path: ./publish/
```

## **🎉 Quick Start**

### **🚀 Build Pertama Kali:**
```bash
# 1. Jalankan script build
.\build.ps1

# 2. Test EXE
.\publish\marker-dotnet.exe

# 3. Distribusi
# Copy folder publish atau file ZIP
```

### **🔄 Setiap Perubahan Kode:**
```bash
# Opsi 1: Manual build
.\build.ps1

# Opsi 2: Auto-build
.\watch-build.ps1
# Edit file Anda → build otomatis

# Opsi 3: Quick build
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o "./publish"
```

Sekarang Anda punya build automation yang siap digunakan! 🚀
