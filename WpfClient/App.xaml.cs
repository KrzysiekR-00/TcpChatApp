using System.Windows;
using WpfClient.ViewModels;
using WpfClient.Views;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;
            window.Show();

            Exit += (sender, args) =>
            {
                viewModel.Dispose();
            };
        }
    }
}
