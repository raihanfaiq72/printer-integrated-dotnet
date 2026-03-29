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
                if (ConfigService.IsFirstRun())
                {
                    var setupWindow = new SetupWindow();
                    if (setupWindow.ShowDialog() != true)
                    {
                        Current.Shutdown();
                        return;
                    }
                }

                var mainWindow = new MainWindow();
                var config = ConfigService.GetConfig();
                mainWindow.Title = config.ProgramName;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memulai aplikasi: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
    }
}
