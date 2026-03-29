using System;
using System.Drawing.Printing;
using marker_dotnet.Interfaces;
using marker_dotnet.Models;

namespace marker_dotnet.Services
{
    public class PrinterService : IPrinterService
    {
        private string _printerName;
        private readonly EpsonTMU220B _epsonPrinter;

        public PrinterService()
        {
            LoadConfig();
            _epsonPrinter = new EpsonTMU220B();
        }

        private void LoadConfig()
        {
            var config = ConfigService.GetConfig();
            _printerName = config.PrinterName;
        }

        public bool CetakMarker(string invoice, int totalMarkers)
        {
            try
            {
                if (!_epsonPrinter.Open())
                {
                    throw new Exception($"Tidak dapat membuka printer: {_printerName}");
                }

                _epsonPrinter.StartDocument();
                _epsonPrinter.Initialize();

                for (int i = 0; i <= totalMarkers; i++)
                {
                    var (markerCode, markerCount) = GenerateMarkerCode(invoice, i, totalMarkers);

                    PrintMarkerLabel(markerCode, markerCount);
                    
                    if (i < totalMarkers)
                    {
                        _epsonPrinter.FeedLines(3);
                    }
                }

                _epsonPrinter.PartialCut();
                _epsonPrinter.EndDocument();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Gagal mencetak: {ex.Message}");
            }
            finally
            {
                _epsonPrinter.Close();
            }
        }

        private void PrintMarkerLabel(string markerCode, string markerCount)
        {
            _epsonPrinter.SetAlignment(Alignment.Center);
            _epsonPrinter.SetCharacterSize(CharacterSize.DoubleHeightWidth);
            _epsonPrinter.SetFontStyle(true, false, true, true);
            
            _epsonPrinter.PrintLine(markerCode);
            _epsonPrinter.PrintLine(markerCount);
            
            _epsonPrinter.SetFontStyle(false, false, false, false);
            _epsonPrinter.SetCharacterSize(CharacterSize.Normal);
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
            try
            {
                foreach (string printerName in PrinterSettings.InstalledPrinters)
                {
                    if (printerName.Equals(_printerName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                // Handle case where printing is not supported
            }
            return false;
        }
    }
}
