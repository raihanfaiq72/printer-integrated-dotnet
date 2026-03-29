using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using marker_dotnet.Interfaces;

namespace marker_dotnet.Services
{
    public class PrinterService : IPrinterService
    {
        private const string PRINTER_NAME = "printer_label";

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int Level, ref DOCINFOA pDocInfo);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

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

        public bool CetakMarker(string invoice, int totalMarkers)
        {
            IntPtr hPrinter = IntPtr.Zero;
            DOCINFOA docInfo = new DOCINFOA();
            docInfo.pDocName = "Marker Crystal Print";
            docInfo.pDataType = "RAW";

            try
            {
                if (!OpenPrinter(PRINTER_NAME, out hPrinter, IntPtr.Zero))
                {
                    throw new Exception($"Tidak dapat membuka printer: {PRINTER_NAME}");
                }

                if (!StartDocPrinter(hPrinter, 1, ref docInfo))
                {
                    throw new Exception("Gagal memulai dokumen printer");
                }

                for (int i = 0; i <= totalMarkers; i++)
                {
                    var (markerCode, markerCount) = GenerateMarkerCode(invoice, i, totalMarkers);

                    if (!StartPagePrinter(hPrinter))
                    {
                        throw new Exception("Gagal memulai halaman printer");
                    }

                    byte[] centerCommand = { 0x1B, 0x61, 0x01 };
                    byte[] emphasizeCommand = { 0x1B, 0x45, 0x01 };
                    byte[] doubleHeightWidthCommand = { 0x1D, 0x21, 0x11 };
                    byte[] resetCommand = { 0x1B, 0x40 };
                    byte[] feedCommand = { 0x0A, 0x0A };
                    byte[] cutCommand = { 0x1D, 0x56, 0x00, 0x0A };

                    WriteRawBytes(hPrinter, centerCommand);
                    WriteRawBytes(hPrinter, emphasizeCommand);
                    WriteRawBytes(hPrinter, doubleHeightWidthCommand);

                    WriteText(hPrinter, markerCode + "\n");
                    WriteText(hPrinter, markerCount + "\n");

                    WriteRawBytes(hPrinter, resetCommand);
                    WriteRawBytes(hPrinter, feedCommand);

                    if (!EndPagePrinter(hPrinter))
                    {
                        throw new Exception("Gagal mengakhiri halaman printer");
                    }
                }

                if (!EndDocPrinter(hPrinter))
                {
                    throw new Exception("Gagal mengakhiri dokumen printer");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Gagal mencetak: {ex.Message}");
            }
            finally
            {
                if (hPrinter != IntPtr.Zero)
                {
                    ClosePrinter(hPrinter);
                }
            }
        }

        private static void WriteRawBytes(IntPtr hPrinter, byte[] bytes)
        {
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

        private static void WriteText(IntPtr hPrinter, string text)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(text);
            WriteRawBytes(hPrinter, bytes);
        }

        public (string markerCode, string markerCount) GenerateMarkerCode(string invoice, int index, int total)
        {
            string baseCode = invoice.Replace("-", "");

            return (
                $"{baseCode}-{index}",
                $"{index}/{total}"
            );
        }

        public bool CheckPrinterAvailable()
        {
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                if (printerName.Equals(PRINTER_NAME, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
