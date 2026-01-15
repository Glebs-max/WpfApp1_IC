using System.Collections.Generic;
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
        private class NavEntry { 
            public Page Page { get; set; } 
            public string Path { get; set; } 

            public NavEntry(Page page, string path)
            { 
                Page = page; 
                Path = path; 
            } 
        }
        private bool _isMenuExpanded = true;
        private readonly Stack<NavEntry> _backStack = new Stack<NavEntry>();
        private readonly Stack<NavEntry> _forwardStack = new Stack<NavEntry>();


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
            NavigateTo(new Pages.CameraBasicPage(), "Главная > Камера");
        }

        private void CameraAdvanced_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new Pages.CameraAdvancedPage()); 
            //PathText.Text = "Главная > Камера (расширенный режим)";
        }
        #region Кнопки верхней панели
        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            _backStack.Clear();
            _forwardStack.Clear();

            MainFrame.Content = null;
            PathText.Text = "Главная";

            UpdateNavButtons();
        }



        // Настройка на главном экране
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(new Pages.SettingsPage(), "Главная > Настройки");

        }

        // Инфо 
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // Кнопка вперёд
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_forwardStack.Count == 0)
                return;

            var entry = _forwardStack.Pop();

            if (MainFrame.Content is Page current)
            {
                string currentPath = PathText.Text;
                _backStack.Push(new NavEntry(current, currentPath));
            }

            MainFrame.Navigate(entry.Page);
            PathText.Text = entry.Path;

            UpdateNavButtons();
        }



        // Кнопка прощу всё)
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_backStack.Count == 0)
                return;

            var entry = _backStack.Pop();

            if (MainFrame.Content is Page current)
            {
                string currentPath = PathText.Text;
                _forwardStack.Push(new NavEntry(current, currentPath));
            }

            MainFrame.Navigate(entry.Page);
            PathText.Text = entry.Path;

            UpdateNavButtons();
        }

        #endregion

        // Метод навигации 
        public void NavigateTo(Page page, string path)
        {
            if (MainFrame.Content is Page currentPage)
            {
                string currentPath = PathText.Text;
                _backStack.Push(new NavEntry(currentPage, currentPath));
            }

            _forwardStack.Clear();

            MainFrame.Navigate(page);
            PathText.Text = path;

            UpdateNavButtons();
        }


        // Обновленеи кнопок
        private void UpdateNavButtons()
        {
            BackButton.IsEnabled = _backStack.Count > 0;
            ForwardButton.IsEnabled = _forwardStack.Count > 0;
        }



    }

}