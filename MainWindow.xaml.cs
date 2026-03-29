using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace marker_dotnet
{
    public partial class MainWindow : Window
    {
        private bool _isNewNumber = true;

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

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var quantity = DisplayTextBox.Text;
            var message = $"Cetak {quantity} marker Crystal";
            
            MessageBox.Show(message, "Konfirmasi Cetak", MessageBoxButton.OK, MessageBoxImage.Information);
            
            DisplayTextBox.Text = "0";
            _isNewNumber = true;
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
    }
}
