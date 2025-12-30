using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1_IC
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

        private void CameraBasic_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.CameraBasicPage());
        }

        private void CameraAdvanced_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new Pages.CameraAdvancedPage());
        }
    }
}