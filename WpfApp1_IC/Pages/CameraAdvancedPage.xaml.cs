using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WpfApp1_IC.Services;
using Microsoft.Win32;

namespace WpfApp1_IC.Pages
{
    public partial class CameraAdvancedPage : Page
    {
        private readonly AppSettings _settings;

        public CameraAdvancedPage()
        {
            InitializeComponent();

            _settings = SettingsService.Load();

            RejectDelayBox.Text = _settings.RejectDelayMs.ToString();
            IdmvsPathBox.Text = _settings.IdmvsPath;
        }

        private void SaveRejectDelay_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(RejectDelayBox.Text, out int delayMs) || delayMs < 0)
            {
                MessageBox.Show("Введите корректное значение задержки в миллисекундах.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _settings.RejectDelayMs = delayMs;
            SettingsService.Save(_settings);

            MessageBox.Show($"Задержка отбраковки установлена: {delayMs} мс",
                            "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenIDMVS_Click(object sender, RoutedEventArgs e)
        {
            string path = IdmvsPathBox.Text;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть IDMVS.\nПуть: {path}\nОшибка: {ex.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void BrowseIDMVS_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите IDMVS.exe",
                Filter = "Исполняемые файлы (*.exe)|*.exe|Все файлы (*.*)|*.*",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                IdmvsPathBox.Text = dialog.FileName;

                _settings.IdmvsPath = dialog.FileName;
                SettingsService.Save(_settings);
            }
        }
    }
}
