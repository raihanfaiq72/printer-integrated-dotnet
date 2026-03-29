# Development Guide

## 🚀 Development Mode vs Production Mode

### **🔧 Development Mode (DEBUG)**
Saat development dengan `dotnet run`, aplikasi akan:
- **Skip setup window** langsung ke main window
- **Title**: `[DEBUG] Nama Program` 
- **Load config** dari database yang sudah ada
- **Bisa testing** main window tanpa setup ulang

```bash
# Development - langsung main window
dotnet run
```

### **🏭 Production Mode (RELEASE)**
Saat build untuk deployment:
- **Normal flow** → Setup window jika first run
- **Title**: `Nama Program` (tanpa [DEBUG])
- **Setup required** untuk fresh install

```bash
# Production - normal flow dengan setup
dotnet run --configuration Release
```

## 🛠️ Development Workflow

### **1. First Time Setup:**
```bash
# Jalankan sekali untuk setup database
dotnet run --configuration Release
# → Setup window → Isi konfigurasi → Save
```

### **2. Development Testing:**
```bash
# Development mode - langsung main window
dotnet run
# → Main window langsung terbuka dengan [DEBUG] title
```

### **3. Testing Production Flow:**
```bash
# Test production mode
dotnet run --configuration Release
# → Normal flow dengan setup check
```

## 📁 Database Location

**Config tersimpan di:**
```
%AppData%\MarkerCrystal\config.db
```

**Development vs Production menggunakan database yang sama!**

## 🎯 Tips Development

### **Reset Configuration (jika perlu):**
```bash
# Hapus database untuk fresh setup
Remove-Item "$env:APPDATA\MarkerCrystal\config.db" -Force
```

### **Debug Main Window:**
- **Development mode**: Langsung main window
- **Title**: `[DEBUG] Printer marker` 
- **Bisa testing** semua fitur tanpa setup

### **Testing Setup Flow:**
- **Production mode**: Normal setup flow
- **Title**: `Printer marker` (tanpa debug)
- **Fresh install behavior**

## 🔄 Switching Between Modes

### **Development (Default):**
```bash
dotnet run
# Skip setup → Main window dengan [DEBUG]
```

### **Production Testing:**
```bash
dotnet run --configuration Release
# Normal flow → Setup jika first run
```

### **Build for Distribution:**
```bash
dotnet publish -c Release -r win-x64 --self-contained
# Production build untuk deployment
```

## 🐛 Troubleshooting

### **Main Window Tidak Muncul:**
1. Cek console output untuk error
2. Pastikan database tidak corrupt
3. Reset config jika perlu

### **Setup Window Selalu Muncul:**
1. Cek `is_configured` di database
2. Pastikan save berhasil
3. Reset dan setup ulang

### **Database Issues:**
```bash
# Check database location
Get-ChildItem "$env:APPDATA\MarkerCrystal\"

# Reset database
Remove-Item "$env:APPDATA\MarkerCrystal\config.db" -Force
```

## 📝 Development Notes

- **Development mode** otomatis skip setup untuk efisiensi
- **Database sama** untuk development dan production
- **Title differences** untuk identifikasi mode
- **Configuration persist** antar run

Happy coding! 🚀
