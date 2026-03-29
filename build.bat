@echo off
echo 🚀 Starting Marker Crystal Build Process...

:: Step 1: Clean
echo 🧹 Cleaning previous build...
if exist publish rmdir /s /q publish
if exist *.zip del *.zip

:: Step 2: Build
echo 🔨 Building to executable...
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "./publish"

:: Step 3: Check results
echo 🧪 Checking build results...
if exist "publish\marker-dotnet.exe" (
    echo ✅ Build completed successfully!
    echo 📁 Output folder: .\publish
    echo 🎯 Ready for distribution!
    echo 📝 Run .\publish\marker-dotnet.exe to test
) else (
    echo ❌ Build failed - EXE file not found
)

echo 🎉 Build process completed!
pause
