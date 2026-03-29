using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using marker_dotnet.Interfaces;
using marker_dotnet.Services;

namespace marker_dotnet.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IPrinterService _printerService;
        private readonly IApiService _apiService;
        private readonly IDebugService _debugService;
        private readonly MainWindow _mainWindow;

        private bool _isNewNumber = true;
        private bool _isProcessing = false;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _printerService = new PrinterService();
            _apiService = new ApiService();
            _debugService = new DebugService();
        }

        public bool IsDebugMode => _debugService.IsDebugMode;

        public void Initialize()
        {
            _mainWindow.DisplayTextBox.Text = "0";
            CheckPrinterStatus();
        }

        public void ProcessDigitInput(string digit)
        {
            if (string.IsNullOrEmpty(digit) || digit.Length != 1) return;

            _debugService.CheckSecretCode(digit);

            if (_isNewNumber && digit == "0") return;

            if (_isNewNumber)
            {
                _mainWindow.DisplayTextBox.Text = digit;
                _isNewNumber = false;
            }
            else
            {
                _mainWindow.DisplayTextBox.Text += digit;
            }
        }

        public void ClearDisplay()
        {
            _mainWindow.DisplayTextBox.Text = "0";
            _isNewNumber = true;
        }

        public void ProcessBackspace()
        {
            if (!string.IsNullOrEmpty(_mainWindow.DisplayTextBox.Text) && _mainWindow.DisplayTextBox.Text.Length > 1)
            {
                _mainWindow.DisplayTextBox.Text = _mainWindow.DisplayTextBox.Text.Substring(0, _mainWindow.DisplayTextBox.Text.Length - 1);
            }
            else
            {
                ClearDisplay();
            }
        }

        public async Task ProcessPrintAsync()
        {
            if (_isProcessing)
            {
                MessageBox.Show("Sedang memproses pencetakan, mohon tunggu...", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!int.TryParse(_mainWindow.DisplayTextBox.Text, out int jumlahMarker) || jumlahMarker < 1)
            {
                MessageBox.Show("Masukkan jumlah marker yang valid (minimal 1)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_debugService.IsDebugMode && !_printerService.CheckPrinterAvailable())
            {
                var config = marker_dotnet.Models.ConfigService.GetConfig();
                ShowAlert("Printer Tidak Ditemukan", $"Printer '{config.PrinterName}' tidak terinstall. Pastikan printer terhubung dan nama benar.");
                return;
            }

            HideAlert();
            _isProcessing = true;
            UpdateUIState(true);

            try
            {
                _mainWindow.StatusTextBlock.Text = _debugService.IsDebugMode ? "Debug Mode - Menghubungi API..." : "Menghubungi API...";

                var apiResponse = await _apiService.KirimPermintaanCetakAsync(jumlahMarker, "desktop");

                if (apiResponse.Success)
                {
                    _mainWindow.StatusTextBlock.Text = _debugService.IsDebugMode ? $"Debug Mode - Invoice: {apiResponse.Invoice}" : $"Invoice: {apiResponse.Invoice} - Mencetak...";
                    _mainWindow.InvoiceTextBlock.Text = apiResponse.Invoice;

                    if (!_debugService.IsDebugMode)
                    {
                        _printerService.CetakMarker(apiResponse.Invoice, jumlahMarker);
                    }
                    else
                    {
                        await Task.Delay(1000);
                        _mainWindow.StatusTextBlock.Text = $"Debug Mode - Simulasi cetak {jumlahMarker} marker selesai";
                    }

                    await _apiService.UpdateStatusAsync(apiResponse.Invoice, "completed");

                    var successMessage = _debugService.IsDebugMode
                        ? $"Debug: Berhasil mengirim {jumlahMarker} marker ke API\nInvoice: {apiResponse.Invoice}\n(No actual printing)"
                        : $"Berhasil mencetak {jumlahMarker} marker\nInvoice: {apiResponse.Invoice}";

                    _mainWindow.StatusTextBlock.Text = _debugService.IsDebugMode ? $"Debug Mode - Berhasil mengirim {jumlahMarker} marker" : $"Berhasil mencetak {jumlahMarker} marker";
                    MessageBox.Show(successMessage, _debugService.IsDebugMode ? "Debug Success" : "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

                    ClearDisplay();
                    _mainWindow.InvoiceTextBlock.Text = "-";
                }
                else
                {
                    _mainWindow.StatusTextBlock.Text = "Gagal: " + apiResponse.Message;
                    MessageBox.Show(apiResponse.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                _mainWindow.StatusTextBlock.Text = "Error: " + ex.Message;
                MessageBox.Show($"Terjadi kesalahan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (ex.Message.ToLower().Contains("printer") || ex.Message.ToLower().Contains("cetak"))
                {
                    ShowAlert("Error Printer", ex.Message);
                }

                if (!string.IsNullOrEmpty(_mainWindow.InvoiceTextBlock.Text) && _mainWindow.InvoiceTextBlock.Text != "-")
                {
                    await _apiService.UpdateStatusAsync(_mainWindow.InvoiceTextBlock.Text, "failed", ex.Message);
                }
            }
            finally
            {
                _isProcessing = false;
                UpdateUIState(false);
                CheckPrinterStatus();
            }
        }

        private void CheckPrinterStatus()
        {
            if (_debugService.IsDebugMode)
            {
                HideAlert();
                _mainWindow.StatusTextBlock.Text = "Debug Mode - Ready";
                return;
            }

            if (!_printerService.CheckPrinterAvailable())
            {
                var config = marker_dotnet.Models.ConfigService.GetConfig();
                ShowAlert("Printer Tidak Ditemukan", $"Printer '{config.PrinterName}' tidak terinstall. Pastikan printer terhubung dan nama benar.");
            }
            else
            {
                HideAlert();
            }
        }

        public void EnableDebugMode()
        {
            _debugService.EnableDebugMode();
            ShowAlert("Debug Mode Aktif", "Mode debug diaktifkan. Printer tidak diperlukan untuk testing.");
            _mainWindow.Title = "Cetak Marker Crystal [DEBUG MODE]";
            _mainWindow.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
        }

        public void DisableDebugMode()
        {
            _debugService.DisableDebugMode();
            HideAlert();
            _mainWindow.Title = "Cetak Marker Crystal";
            _mainWindow.Background = new SolidColorBrush(Colors.White);
            CheckPrinterStatus();
        }

        private void ShowAlert(string title, string message)
        {
            _mainWindow.AlertTitleTextBlock.Text = title;
            _mainWindow.AlertMessageTextBlock.Text = message;
            _mainWindow.StatusAlertBorder.Visibility = Visibility.Visible;
            _mainWindow.StatusInfoBorder.Visibility = Visibility.Collapsed;
        }

        private void HideAlert()
        {
            _mainWindow.StatusAlertBorder.Visibility = Visibility.Collapsed;
            _mainWindow.StatusInfoBorder.Visibility = Visibility.Visible;
        }

        private void UpdateUIState(bool isProcessing)
        {
            foreach (UIElement element in _mainWindow.MainGrid.Children)
            {
                if (element is StackPanel stackPanel)
                {
                    foreach (var child in stackPanel.Children)
                    {
                        if (child is Button button)
                        {
                            button.IsEnabled = !isProcessing;
                        }
                    }
                }
            }

            var numberPad = FindChild<Grid>(_mainWindow, "NumberPadGrid");
            if (numberPad != null)
            {
                foreach (UIElement child in numberPad.Children)
                {
                    if (child is Button button)
                    {
                        button.IsEnabled = !isProcessing;
                    }
                }
            }
        }

        private static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T && (child as FrameworkElement)?.Name == childName)
                {
                    return (T)child;
                }

                var childOfChild = FindChild<T>(child, childName);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }
}
