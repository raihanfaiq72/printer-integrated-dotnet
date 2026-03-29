using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using marker_dotnet.Models;
using marker_dotnet.Services;

namespace marker_dotnet
{
    public partial class MainWindow : Window
    {
        private bool _isNewNumber = true;
        private bool _isProcessing = false;
        private bool _debugMode = false;
        private string _secretCodeInput = "";

        public MainWindow()
        {
            InitializeComponent();
            DisplayTextBox.Text = "0";
            this.KeyDown += MainWindow_KeyDown;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CheckPrinterStatus();
        }

        private void CheckSecretCode()
        {
            if (_secretCodeInput.EndsWith("7272"))
            {
                EnableDebugMode();
                _secretCodeInput = "";
            }
            else if (_secretCodeInput.EndsWith("2727"))
            {
                DisableDebugMode();
                _secretCodeInput = "";
            }
            
            // Keep only last 8 characters to prevent memory issues
            if (_secretCodeInput.Length > 8)
            {
                _secretCodeInput = _secretCodeInput.Substring(_secretCodeInput.Length - 8);
            }
        }

        private void EnableDebugMode()
        {
            _debugMode = true;
            ShowAlert("Debug Mode Aktif", "Mode debug diaktifkan. Printer tidak diperlukan untuk testing.");
            
            // Change window title to indicate debug mode
            this.Title = "Cetak Marker Crystal [DEBUG MODE]";
            
            // Change window color slightly to indicate debug mode
            this.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)); // Alice Blue
        }

        private void DisableDebugMode()
        {
            _debugMode = false;
            HideAlert();
            
            // Restore normal window title
            this.Title = "Cetak Marker Crystal";
            
            // Restore normal background
            this.Background = new SolidColorBrush(Colors.White);
            
            // Re-check printer status
            CheckPrinterStatus();
        }

        private void CheckPrinterStatus()
        {
            if (_debugMode)
            {
                // In debug mode, don't show printer alerts
                HideAlert();
                StatusTextBlock.Text = "Debug Mode - Ready";
                return;
            }
            
            if (!PrinterService.CheckPrinterAvailable())
            {
                ShowAlert("Printer Tidak Ditemukan", "Printer 'printer_label' tidak terinstall. Pastikan printer terhubung dan nama benar.");
            }
            else
            {
                HideAlert();
            }
        }

        private void ShowAlert(string title, string message)
        {
            AlertTitleTextBlock.Text = title;
            AlertMessageTextBlock.Text = message;
            StatusAlertBorder.Visibility = Visibility.Visible;
            StatusInfoBorder.Visibility = Visibility.Collapsed;
        }

        private void HideAlert()
        {
            StatusAlertBorder.Visibility = Visibility.Collapsed;
            StatusInfoBorder.Visibility = Visibility.Visible;
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                var digit = b.Content.ToString();
                
                // Add to secret code input for debug mode detection
                _secretCodeInput += digit;
                CheckSecretCode();
                
                if (_isNewNumber && digit == "0")
                {
                    return;
                }
                
                if (_isNewNumber)
                {
                    DisplayTextBox.Text = digit;
                    _isNewNumber = false;
                }
                else
                {
                    DisplayTextBox.Text += digit;
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayTextBox.Text = "0";
            _isNewNumber = true;
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DisplayTextBox.Text) && DisplayTextBox.Text.Length > 1)
            {
                DisplayTextBox.Text = DisplayTextBox.Text.Substring(0, DisplayTextBox.Text.Length - 1);
            }
            else
            {
                DisplayTextBox.Text = "0";
                _isNewNumber = true;
            }
        }

        private async void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessing)
            {
                MessageBox.Show("Sedang memproses pencetakan, mohon tunggu...", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!int.TryParse(DisplayTextBox.Text, out int jumlahMarker) || jumlahMarker < 1)
            {
                MessageBox.Show("Masukkan jumlah marker yang valid (minimal 1)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check printer availability (skip in debug mode)
            if (!_debugMode && !PrinterService.CheckPrinterAvailable())
            {
                ShowAlert("Printer Tidak Ditemukan", "Printer 'printer_label' tidak terinstall. Pastikan printer terhubung dan nama benar.");
                return;
            }

            HideAlert(); // Hide alert when starting print process
            _isProcessing = true;
            UpdateUIState(true);

            try
            {
                // Show loading message
                StatusTextBlock.Text = _debugMode ? "Debug Mode - Menghubungi API..." : "Menghubungi API...";
                
                // Kirim permintaan ke API
                var apiResponse = await ApiService.KirimPermintaanCetakAsync(jumlahMarker, "desktop");
                
                if (apiResponse.Success)
                {
                    StatusTextBlock.Text = _debugMode ? $"Debug Mode - Invoice: {apiResponse.Invoice}" : $"Invoice: {apiResponse.Invoice} - Mencetak...";
                    InvoiceTextBlock.Text = apiResponse.Invoice;
                    
                    // Skip printing in debug mode
                    if (!_debugMode)
                    {
                        PrinterService.CetakMarker(apiResponse.Invoice, jumlahMarker);
                    }
                    else
                    {
                        // In debug mode, simulate printing delay
                        await Task.Delay(1000); // Simulate 1 second printing time
                        StatusTextBlock.Text = $"Debug Mode - Simulasi cetak {jumlahMarker} marker selesai";
                    }
                    
                    // Update status ke completed
                    await ApiService.UpdateStatusAsync(apiResponse.Invoice, "completed");
                    
                    var successMessage = _debugMode 
                        ? $"Debug: Berhasil mengirim {jumlahMarker} marker ke API\nInvoice: {apiResponse.Invoice}\n(No actual printing)"
                        : $"Berhasil mencetak {jumlahMarker} marker\nInvoice: {apiResponse.Invoice}";
                    
                    StatusTextBlock.Text = _debugMode ? $"Debug Mode - Berhasil mengirim {jumlahMarker} marker" : $"Berhasil mencetak {jumlahMarker} marker";
                    MessageBox.Show(successMessage, _debugMode ? "Debug Success" : "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Reset display
                    DisplayTextBox.Text = "0";
                    _isNewNumber = true;
                    InvoiceTextBlock.Text = "-";
                }
                else
                {
                    StatusTextBlock.Text = "Gagal: " + apiResponse.Message;
                    MessageBox.Show(apiResponse.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Error: " + ex.Message;
                MessageBox.Show($"Terjadi kesalahan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Check if it's printer related error and show alert
                if (ex.Message.ToLower().Contains("printer") || ex.Message.ToLower().Contains("cetak"))
                {
                    ShowAlert("Error Printer", ex.Message);
                }
                
                // Update status ke failed jika ada invoice
                if (!string.IsNullOrEmpty(InvoiceTextBlock.Text) && InvoiceTextBlock.Text != "-")
                {
                    await ApiService.UpdateStatusAsync(InvoiceTextBlock.Text, "failed", ex.Message);
                }
            }
            finally
            {
                _isProcessing = false;
                UpdateUIState(false);
                // Re-check printer status after operation
                CheckPrinterStatus();
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                var digit = (e.Key - Key.D0).ToString();
                
                // Add to secret code input for debug mode detection
                _secretCodeInput += digit;
                CheckSecretCode();
                
                if (_isNewNumber && digit == "0")
                {
                    e.Handled = true;
                    return;
                }
                
                if (_isNewNumber)
                {
                    DisplayTextBox.Text = digit;
                    _isNewNumber = false;
                }
                else
                {
                    DisplayTextBox.Text += digit;
                }
                e.Handled = true;
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                var digit = (e.Key - Key.NumPad0).ToString();
                
                // Add to secret code input for debug mode detection
                _secretCodeInput += digit;
                CheckSecretCode();
                
                if (_isNewNumber && digit == "0")
                {
                    e.Handled = true;
                    return;
                }
                
                if (_isNewNumber)
                {
                    DisplayTextBox.Text = digit;
                    _isNewNumber = false;
                }
                else
                {
                    DisplayTextBox.Text += digit;
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                PrintButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                BackspaceButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                ClearButton_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }
        private void UpdateUIState(bool isProcessing)
        {
            // Disable/enable controls during processing
            foreach (UIElement element in MainGrid.Children)
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
            
            // Enable/disable number pad
            var numberPad = FindChild<Grid>(this, "NumberPadGrid");
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
