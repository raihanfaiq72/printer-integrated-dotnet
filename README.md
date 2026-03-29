# Cetak Marker Crystal - Aplikasi Desktop .NET

## 📁 Struktur Project

```
printer-integrated-dotnet/
├── Interfaces/           # Interface definitions
│   └── IServices.cs     # Service interfaces (IPrinterService, IApiService, IDebugService)
├── Models/              # Data models
│   └── ApiService.cs    # API response/request models
├── Services/            # Business logic implementations
│   ├── ApiService.cs    # API communication service
│   ├── PrinterService.cs # ESC/POS printer integration
│   └── DebugService.cs  # Debug mode functionality
├── ViewModels/          # MVVM pattern
│   └── MainWindowViewModel.cs # Main window business logic
├── MainWindow.xaml      # UI definition
├── MainWindow.xaml.cs   # UI code-behind (clean, minimal)
├── App.xaml            # Application resources
├── App.xaml.cs         # Application startup
├── marker-dotnet.csproj # Project configuration
└── .gitignore          # Git ignore rules
```

## 🏗️ Architecture Pattern

### **MVVM (Model-View-ViewModel)**
- **View**: MainWindow.xaml + MainWindow.xaml.cs (UI only)
- **ViewModel**: MainWindowViewModel.cs (business logic)
- **Model**: Models/ + Services/ (data and operations)

### **Dependency Injection Ready**
- Interface-based services
- Loose coupling between components
- Easy to mock for testing

### **Clean Code Principles**
- Single Responsibility Principle
- No inline comments
- Self-documenting code
- Separation of concerns

## 🔧 Key Features

### **Core Functionality**
- API integration with Crystal Clean server
- ESC/POS printer support
- Real-time status updates
- Error handling and validation

### **Debug Mode**
- Secret code: `7272` to enable, `2727` to disable
- API-only testing (no printer required)
- Visual indicators (blue background, debug title)

### **UI Features**
- Calculator-style numeric input
- Dynamic alert system
- Status and invoice display
- Responsive design

## 📦 Dependencies

- **Newtonsoft.Json**: JSON serialization
- **System.Net.Http**: HTTP client
- **System.Drawing.Printing**: Printer management
- **WPF**: UI framework

## 🚀 Getting Started

1. Install printer named "printer_label" (for production)
2. Configure API endpoints in `ApiService.cs`
3. Run with `dotnet run`
4. Use debug mode `7272` for testing without printer

## 🔄 Workflow

1. **Input**: User enters marker quantity
2. **Validation**: Check input and printer availability
3. **API Call**: Send request to Crystal Clean API
4. **Print**: Generate and print markers (skip in debug mode)
5. **Status Update**: Notify server of completion/failure

## 🛠️ Configuration

- **Printer Name**: `printer_label` (configurable in `PrinterService.cs`)
- **API URLs**: Crystal Clean endpoints
- **Debug Codes**: `7272` / `2727` (configurable in `DebugService.cs`)

## 📝 Code Standards

- No inline comments
- Self-documenting method/variable names
- Interface-based design
- Separation of UI and business logic
- Async/await for network operations
