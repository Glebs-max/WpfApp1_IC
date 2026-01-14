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
        private bool _isMenuExpanded = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            _isMenuExpanded = !_isMenuExpanded; if (_isMenuExpanded)
            { 
                // Развернутое меню
                LeftColumn.Width = new GridLength(220); 
                // нормальная ширина
                MenuItemsPanel.Visibility = Visibility.Visible; 
                MenuText.Visibility = Visibility.Visible; 
            } 
            else 
            { 
                // Свернутое меню: только иконка
                LeftColumn.Width = new GridLength(60); 
                // узкая колонка под кнопку
                MenuItemsPanel.Visibility = Visibility.Collapsed; 
                MenuText.Visibility = Visibility.Collapsed;
            } 
        }

        private void CameraBasic_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.CameraBasicPage());
            PathText.Text = "Главная > Камера (базовый режим)";
        }

        private void CameraAdvanced_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new Pages.CameraAdvancedPage()); 
            //PathText.Text = "Главная > Камера (расширенный режим)";
        }

        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;

            PathText.Text = "Главная";
        }
    }
}