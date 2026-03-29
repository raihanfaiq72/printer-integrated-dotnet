using System;
using System.Threading.Tasks;
using System.Windows;
using marker_dotnet.Models;
using marker_dotnet.ViewModels;

namespace marker_dotnet
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Development mode - skip setup untuk testing
                #if DEBUG
                var mainWindow = new MainWindow();
                var config = ConfigService.GetConfig();
                mainWindow.Title = $"[DEBUG] {config.ProgramName}";
                mainWindow.Show();
                return;
                #endif

                // Production mode - normal flow
                if (ConfigService.IsFirstRun())
                {
                    var setupWindow = new SetupWindow();
                    if (setupWindow.ShowDialog() != true)
                    {
                        Current.Shutdown();
                        return;
                    }
                }

                // Konfigurasi sudah ada, langsung buka main window
                var mainWin = new MainWindow();
                var appConfig = ConfigService.GetConfig();
                mainWin.Title = appConfig.ProgramName;
                mainWin.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memulai aplikasi: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
    }
}
