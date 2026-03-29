using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using marker_dotnet.ViewModels;

namespace marker_dotnet
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel(this);
            this.KeyDown += MainWindow_KeyDown;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Initialize();
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                _viewModel.ProcessDigitInput(b.Content.ToString());
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearDisplay();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ProcessBackspace();
        }

        private async void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.ProcessPrintAsync();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                var digit = (e.Key - Key.D0).ToString();
                _viewModel.ProcessDigitInput(digit);
                e.Handled = true;
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                var digit = (e.Key - Key.NumPad0).ToString();
                _viewModel.ProcessDigitInput(digit);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                _ = _viewModel.ProcessPrintAsync();
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                _viewModel.ProcessBackspace();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                _viewModel.ClearDisplay();
                e.Handled = true;
            }
        }
    }
}
