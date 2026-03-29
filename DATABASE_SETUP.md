# Database Configuration System

## 🗄️ SQLite Database Setup

Aplikasi ini menggunakan database SQLite untuk menyimpan konfigurasi pengguna secara persisten.

### **📁 Database Location**
```
%AppData%\MarkerCrystal\config.db
```

### **🏗️ Database Schema**
```sql
CREATE TABLE config (
    id INTEGER PRIMARY KEY,
    program_name TEXT NOT NULL,
    printer_name TEXT NOT NULL,
    api_url TEXT NOT NULL,
    api_url_status TEXT NOT NULL,
    is_configured INTEGER NOT NULL DEFAULT 0,
    installed_date TEXT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

## 🔐 Security Features

### **Password Protection**
- **Required Password**: `081999967373`
- **Purpose**: Prevent unauthorized configuration changes
- **Validation**: Only during first-time setup

### **Configuration Data**
- **Program Name**: Customizable (default: "Printer marker")
- **Printer Name**: User-defined printer name
- **API URLs**: Loaded from .env file
- **Installation Date**: Automatically tracked

## 🚀 First Run Setup

### **Setup Process**
1. **Application Start** → Check database existence
2. **First Run Detection** → Show SetupWindow
3. **User Input** → Program name, printer name, password
4. **Validation** → Password verification
5. **Save Config** → Store in SQLite database
6. **Application Start** → Load main window with custom title

### **Setup Window Features**
- **Program Name**: "Printer marker [custom]"
- **Printer Name**: User's printer name
- **Password**: Required for authorization
- **Validation**: Real-time input validation

## 📝 Configuration Loading

### **Dynamic Configuration**
```csharp
// Load from database on startup
var config = ConfigService.GetConfig();
mainWindow.Title = config.ProgramName;
printerService.LoadConfig();
apiService.LoadConfig();
```

### **Environment Variables**
```
.env file:
API_URL=https://api-marker.crystalclean.co.id/api/get-print-batch-data
API_URL_STATUS=https://api-marker.crystalclean.co.id/api
```

## 🔄 Persistence Features

### **Data Persistence**
- **Survives Uninstall**: Database remains in AppData
- **Reinstall Support**: Configuration preserved
- **Portable Settings**: Move between machines

### **Configuration Reset**
- **Manual Reset**: `ConfigService.ResetConfig()`
- **First Run Again**: Triggers setup window
- **Fresh Install**: Clean configuration state

## 🛡️ Security Considerations

### **Password Security**
- **Hardcoded**: Only in source code
- **Required Setup**: Prevents unauthorized access
- **No Storage**: Password not saved in database

### **Data Protection**
- **Local Storage**: SQLite database only
- **No Cloud Sync**: Configuration stays local
- **User Control**: Full configuration control

## 📋 Usage Example

### **First Installation**
1. Run application → Setup window appears
2. Enter "Printer marker Store01" as program name
3. Enter "EPSON TM-U220B" as printer name  
4. Enter password "081999967373"
5. Click Save → Configuration stored
6. Main window opens with title "Printer marker Store01"

### **Reinstall Scenario**
1. Uninstall application
2. Reinstall application
3. Database detected → Skip setup
4. Main window opens with saved configuration

## 🔧 Maintenance

### **Database Operations**
- **Initialize**: Automatic on first run
- **Backup**: Copy config.db file
- **Reset**: Delete config.db file
- **Migrate**: Manual schema updates

### **Troubleshooting**
- **Setup Loop**: Check database permissions
- **Config Loss**: Verify AppData access
- **Password Issues**: Contact administrator
