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

        public MainWindow()
        {
            InitializeComponent();
            DisplayTextBox.Text = "0";
            this.KeyDown += MainWindow_KeyDown;
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                var digit = b.Content.ToString();
                
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

            // Check printer availability
            if (!PrinterService.CheckPrinterAvailable())
            {
                MessageBox.Show("Printer 'printer_label' tidak ditemukan. Pastikan printer terinstall dan nama printer benar.", "Error Printer", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _isProcessing = true;
            UpdateUIState(true);

            try
            {
                // Show loading message
                StatusTextBlock.Text = "Menghubungi API...";
                
                // Kirim permintaan ke API
                var apiResponse = await ApiService.KirimPermintaanCetakAsync(jumlahMarker, "desktop");
                
                if (apiResponse.Success)
                {
                    StatusTextBlock.Text = $"Invoice: {apiResponse.Invoice} - Mencetak...";
                    InvoiceTextBlock.Text = apiResponse.Invoice;
                    
                    // Cetak marker
                    PrinterService.CetakMarker(apiResponse.Invoice, jumlahMarker);
                    
                    // Update status ke completed
                    await ApiService.UpdateStatusAsync(apiResponse.Invoice, "completed");
                    
                    StatusTextBlock.Text = $"Berhasil mencetak {jumlahMarker} marker";
                    MessageBox.Show($"Berhasil mencetak {jumlahMarker} marker\nInvoice: {apiResponse.Invoice}", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    
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
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                var digit = (e.Key - Key.D0).ToString();
                
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
