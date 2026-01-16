using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfApp1_IC.Pages;
using WpfApp1_IC.Services;

namespace WpfApp1_IC
{
    public partial class MainWindow : Window
    {
        public InspectorController CurrentController { get; set; }
        private class NavEntry
        {
            public Page Page { get; private set; }
            public string Path { get; private set; }
            


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

            MainFrame.Navigate(new Pages.HomePage());
            PathText.Text = "Главная";

            UpdateNavButtons();
        }

        // ЛЕВОЕ МЕНЮ
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            _isMenuExpanded = !_isMenuExpanded;

            if (_isMenuExpanded)
            {
                LeftColumn.Width = new GridLength(220);
                MenuItemsPanel.Visibility = Visibility.Visible;
                MenuText.Visibility = Visibility.Visible;
            }
            else
            {
                LeftColumn.Width = new GridLength(60);
                MenuItemsPanel.Visibility = Visibility.Collapsed;
                MenuText.Visibility = Visibility.Collapsed;
            }
        }

        // ПУНКТЫ МЕНЮ
        private void CameraBasic_Click(object sender, RoutedEventArgs e)
        {
            var controller = ((MainWindow)Application.Current.MainWindow).CurrentController;

            // Передаём контроллер, если он есть
            NavigateTo(new CameraBasicPage(controller), "Камера — базовый режим");
        }


        private void CameraAdvanced_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(new Pages.CameraAdvancedPage(), "Главная > Камера (расширенный режим)");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo(new Pages.SettingsPage(), "Главная > Настройки");
        }

        // КНОПКА ДОМОЙ
        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            _backStack.Clear();
            _forwardStack.Clear();

            MainFrame.Navigate(new Pages.HomePage());
            PathText.Text = "Главная";

            UpdateNavButtons();
        }

        // КНОПКА ПОМОЩИ
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел помощи пока не реализован.");
        }

        // НАЗАД
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_backStack.Count == 0)
            {
                MainFrame.Navigate(new Pages.HomePage());
                PathText.Text = "Главная";
                _forwardStack.Clear();
                UpdateNavButtons();
                return;
            }

            NavEntry entry = _backStack.Pop();

            if (MainFrame.Content is Page current)
            {
                _forwardStack.Push(new NavEntry(current, PathText.Text));
            }

            MainFrame.Navigate(entry.Page);
            PathText.Text = entry.Path;

            UpdateNavButtons();
        }

        // ВПЕРЁД
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_forwardStack.Count == 0)
                return;

            NavEntry entry = _forwardStack.Pop();

            if (MainFrame.Content is Page current)
            {
                _backStack.Push(new NavEntry(current, PathText.Text));
            }

            MainFrame.Navigate(entry.Page);
            PathText.Text = entry.Path;

            UpdateNavButtons();
        }

        // ГЛАВНАЯ НАВИГАЦИЯ
        public void NavigateTo(Page page, string path)
        {
            if (MainFrame.Content is Page current)
            {
                _backStack.Push(new NavEntry(current, PathText.Text));
            }

            _forwardStack.Clear();

            MainFrame.Navigate(page);
            PathText.Text = path;

            UpdateNavButtons();
        }

        // ОБНОВЛЕНИЕ КНОПОК
        private void UpdateNavButtons()
        {
            BackButton.IsEnabled = _backStack.Count > 0 || MainFrame.Content != null;
            ForwardButton.IsEnabled = _forwardStack.Count > 0;
        }
    }
}
