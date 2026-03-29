using System;
using System.Runtime.InteropServices;
using System.Text;

namespace marker_dotnet.Services
{
    public class EpsonTMU220B
    {
        private const string PRINTER_NAME = "EPSON TM-U220B";

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int Level, ref DOCINFOA pDocInfo);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int nBytes, out int nWritten);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        private IntPtr hPrinter = IntPtr.Zero;

        public bool Open()
        {
            try
            {
                return OpenPrinter(PRINTER_NAME, out hPrinter, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                throw new Exception($"Gagal membuka printer EPSON TM-U220B: {ex.Message}");
            }
        }

        public void Close()
        {
            if (hPrinter != IntPtr.Zero)
            {
                ClosePrinter(hPrinter);
                hPrinter = IntPtr.Zero;
            }
        }

        public void Initialize()
        {
            ExecuteCommand(new byte[] { 0x1B, 0x40 });
        }

        public void SetAlignment(Alignment alignment)
        {
            byte[] command = { 0x1B, 0x61, (byte)alignment };
            ExecuteCommand(command);
        }

        public void SetFontStyle(bool bold, bool underline, bool doubleHeight, bool doubleWidth)
        {
            byte mode = 0;
            if (bold) mode |= 0x08;
            if (underline) mode |= 0x80;
            if (doubleHeight) mode |= 0x10;
            if (doubleWidth) mode |= 0x20;

            byte[] command = { 0x1B, 0x21, mode };
            ExecuteCommand(command);
        }

        public void SetCharacterSize(CharacterSize size)
        {
            byte[] command = { 0x1D, 0x21, (byte)size };
            ExecuteCommand(command);
        }

        public void PrintText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            byte[] bytes = Encoding.GetEncoding(437).GetBytes(text);
            WriteBytes(bytes);
        }

        public void PrintLine(string text = "")
        {
            PrintText(text + "\n");
        }

        public void FeedLines(int lines)
        {
            byte[] command = { 0x1B, 0x64, (byte)lines };
            ExecuteCommand(command);
        }

        public void CutPaper(CutType cutType)
        {
            byte[] command = { 0x1D, 0x56, (byte)cutType, 0x00 };
            ExecuteCommand(command);
        }

        public void PartialCut()
        {
            CutPaper(CutType.Partial);
        }

        public void FullCut()
        {
            CutPaper(CutType.Full);
        }

        public void PrintQRCode(string data, int model = 2, int size = 6)
        {
            if (string.IsNullOrEmpty(data)) return;

            SetAlignment(Alignment.Center);

            byte[] command = new byte[8 + data.Length];
            command[0] = 0x1D;
            command[1] = 0x28;
            command[2] = 0x6B;
            command[3] = 0x04;
            command[4] = 0x00;
            command[5] = 0x31;
            command[6] = (byte)size;
            command[7] = (byte)model;

            byte[] dataBytes = Encoding.GetEncoding(437).GetBytes(data);
            Buffer.BlockCopy(dataBytes, 0, command, 8, dataBytes.Length);

            ExecuteCommand(command);

            byte[] printCommand = { 0x1D, 0x28, 0x6B, 0x03, 0x00, 0x32, 0x43 };
            ExecuteCommand(printCommand);
        }

        public void PrintBarcode(string data, BarcodeType type, int width = 3, int height = 162)
        {
            if (string.IsNullOrEmpty(data)) return;

            SetAlignment(Alignment.Center);

            byte[] command = new byte[7 + data.Length];
            command[0] = 0x1D;
            command[1] = 0x6B;
            command[2] = (byte)type;
            command[3] = (byte)width;
            command[4] = (byte)(height / 8);
            command[5] = (byte)data.Length;

            byte[] dataBytes = Encoding.GetEncoding(437).GetBytes(data);
            Buffer.BlockCopy(dataBytes, 0, command, 6, dataBytes.Length);
            command[6 + data.Length] = 0x00;

            ExecuteCommand(command);
        }

        public void PrintBitmap(byte[] bitmapData, int width)
        {
            if (bitmapData == null || bitmapData.Length == 0) return;

            int height = bitmapData.Length / (width / 8);
            if (height > 255) height = 255;

            SetAlignment(Alignment.Center);

            byte[] header = { 0x1D, 0x76, 0x30, 0x00, (byte)(width / 8), (byte)height };
            ExecuteCommand(header);

            WriteBytes(bitmapData);
        }

        public void SetLineSpacing(int spacing)
        {
            byte[] command = { 0x1B, 0x33, (byte)spacing };
            ExecuteCommand(command);
        }

        public void ResetLineSpacing()
        {
            byte[] command = { 0x1B, 0x32 };
            ExecuteCommand(command);
        }

        public void SelectFont(int fontNumber)
        {
            byte[] command = { 0x1B, 0x4D, (byte)fontNumber };
            ExecuteCommand(command);
        }

        public void SetPrintDensity(int density)
        {
            if (density < 0) density = 0;
            if (density > 8) density = 8;

            byte[] command = { 0x1D, 0x7F, (byte)density };
            ExecuteCommand(command);
        }

        private void ExecuteCommand(byte[] command)
        {
            if (hPrinter == IntPtr.Zero)
                throw new Exception("Printer tidak terbuka");

            WriteBytes(command);
        }

        private void WriteBytes(byte[] bytes)
        {
            if (hPrinter == IntPtr.Zero)
                throw new Exception("Printer tidak terbuka");

            int nWritten;
            IntPtr pBytes = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, pBytes, bytes.Length);

            try
            {
                if (!WritePrinter(hPrinter, pBytes, bytes.Length, out nWritten))
                {
                    throw new Exception("Gagal menulis ke printer");
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(pBytes);
            }
        }

        public void StartDocument()
        {
            DOCINFOA docInfo = new DOCINFOA();
            docInfo.pDocName = "Marker Crystal Print";
            docInfo.pDataType = "RAW";

            if (!StartDocPrinter(hPrinter, 1, ref docInfo))
            {
                throw new Exception("Gagal memulai dokumen printer");
            }
        }

        public void EndDocument()
        {
            if (!EndDocPrinter(hPrinter))
            {
                throw new Exception("Gagal mengakhiri dokumen printer");
            }
        }
    }

    public enum Alignment
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    public enum CharacterSize
    {
        Normal = 0x00,
        DoubleHeight = 0x01,
        DoubleWidth = 0x10,
        DoubleHeightWidth = 0x11
    }

    public enum CutType
    {
        Full = 0x00,
        Partial = 0x01
    }

    public enum BarcodeType
    {
        UPC_A = 0,
        UPC_E = 1,
        EAN13 = 2,
        EAN8 = 3,
        CODE39 = 4,
        ITF = 5,
        CODABAR = 6
    }
}
