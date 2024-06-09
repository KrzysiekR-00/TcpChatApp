using System.Windows;
using System.Windows.Controls;

namespace WpfClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ScrollParentToEnd(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            ((sender as FrameworkElement)?.Parent as ScrollViewer)?.ScrollToEnd();
        }
    }
}