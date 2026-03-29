# EPSON TM-U220B Integration Guide

## 🖨️ Setup Instructions

### **1. Printer Installation**
1. Install EPSON TM-U220B driver
2. Connect printer via USB/Ethernet
3. Set printer name to: **"EPSON TM-U220B"**

### **2. Windows Configuration**
- Go to: `Control Panel > Devices and Printers`
- Right-click TM-U220B > `Printer properties`
- Set `Port` to correct connection (USB/Network)
- Test print to ensure connectivity

## 🔧 Library Features

### **Core Printing Functions**
- **Text Printing**: Center alignment, double size, bold
- **Document Management**: Start/end document control
- **Paper Handling**: Partial cut, line feeding
- **Font Control**: Size, style, emphasis

### **Advanced Features**
- **QR Code**: Print QR codes with custom size
- **Barcode**: Support for multiple barcode types
- **Bitmap**: Print images and logos
- **Density Control**: Adjust print darkness

### **ESC/POS Commands**
- Full ESC/POS command support
- Character encoding (Code Page 437)
- Printer initialization and reset
- Status checking

## 📋 Usage Example

```csharp
var printer = new EpsonTMU220B();

try
{
    printer.Open();
    printer.StartDocument();
    printer.Initialize();
    
    printer.SetAlignment(Alignment.Center);
    printer.SetCharacterSize(CharacterSize.DoubleHeightWidth);
    printer.SetFontStyle(true, false, true, true);
    
    printer.PrintLine("MARKER-001");
    printer.PrintLine("1/10");
    
    printer.PartialCut();
    printer.EndDocument();
}
finally
{
    printer.Close();
}
```

## 🚨 Troubleshooting

### **Common Issues**
1. **"Printer not found"**: Check printer name matches exactly
2. **"Cannot open printer"**: Verify printer is online and connected
3. **"Print quality poor"**: Adjust print density
4. **"Paper jam"**: Check paper path and restart printer

### **Debug Mode**
Use `7272` to enable debug mode for testing without printer

## 📱 Integration Status

✅ **Completed:**
- EPSON TM-U220B library implementation
- Enhanced printing with proper formatting
- Partial cut support
- Error handling improvements

✅ **Features:**
- Center alignment
- Double height & width text
- Bold emphasis
- Proper spacing between markers
- Automatic paper cutting

🔧 **Ready for Production**
