using System;
using System.Windows;
using System.Windows.Controls;
using marker_dotnet.Models;

namespace marker_dotnet
{
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            var config = ConfigService.GetConfig();
            ProgramNameTextBox.Text = config.ProgramName;
            PrinterNameTextBox.Text = config.PrinterName;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var programName = ProgramNameTextBox.Text.Trim();
            var printerName = PrinterNameTextBox.Text.Trim();
            var password = PasswordBox.Password;

            HideErrorMessage();

            if (string.IsNullOrEmpty(programName))
            {
                ShowErrorMessage("Nama program tidak boleh kosong");
                return;
            }

            if (string.IsNullOrEmpty(printerName))
            {
                ShowErrorMessage("Nama printer tidak boleh kosong");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowErrorMessage("Password tidak boleh kosong");
                return;
            }

            if (!ConfigService.ValidatePassword(password))
            {
                ShowErrorMessage("Password salah. Hubungi administrator.");
                return;
            }

            try
            {
                var success = ConfigService.SaveConfig(programName, printerName, password);
                
                if (success)
                {
                    MessageBox.Show("Konfigurasi berhasil disimpan!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    ShowErrorMessage("Gagal menyimpan konfigurasi");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Terjadi kesalahan: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfigService.IsFirstRun())
            {
                var result = MessageBox.Show("Anda harus melakukan konfigurasi untuk menggunakan aplikasi ini. Apakah Anda ingin keluar?", 
                    "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    this.DialogResult = false;
                    this.Close();
                }
                // Jika No, tetap di setup window
            }
            else
            {
                // Bukan first run, cukup close saja
                this.DialogResult = false;
                this.Close();
            }
        }

        private void ShowErrorMessage(string message)
        {
            ErrorMessage.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        private void HideErrorMessage()
        {
            ErrorBorder.Visibility = Visibility.Collapsed;
        }
    }
}
