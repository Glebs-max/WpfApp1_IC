using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp1_IC.Services;

namespace WpfApp1_IC.Pages
{
    public partial class CameraBasicPage : Page
    {
        private InspectorController _controller;
        private bool _isRunning = false;

        public CameraBasicPage()
        {
            InitializeComponent();

            ShowImageCheck.IsChecked = true;
            ShowLogCheck.IsChecked = true;
        }

        //Старт / Стоп
        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if (!_isRunning)
            {
                StartController();
                _isRunning = true;

                // Иконка "Стоп"
                StartStopIcon.Data = Geometry.Parse("M 0 0 H 6 V 20 H 0 Z M 10 0 H 16 V 20 H 10 Z");
                StartStopIcon.Fill = Brushes.Red;

                button.ToolTip = "Стоп";
            }
            else
            {
                StopController();
                _isRunning = false;

                // Иконка "Старт"
                StartStopIcon.Data = Geometry.Parse("M 0 0 L 0 20 L 17 10 Z");
                StartStopIcon.Fill = Brushes.Green;

                button.ToolTip = "Старт";
            }
        }


        private void StartController()
        {
            try
            {
                var modbus = new ModbusService("192.168.0.127", 502);
                modbus.Connect();

                var camera = new CameraService();
                camera.InitCamera();

                _controller = new InspectorController(modbus, camera);

                _controller.DataMatrixRead += dm =>
                {
                    Dispatcher.Invoke(() => DmTextBox.Text = dm.Normalized);
                };

                _controller.FrameReceived += frame =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (frame != null)
                        {
                            CameraImage.Source = frame;
                            Log("Изображение обновлено");
                        }
                        else
                        {
                            Log("Изображение не получено");
                        }
                    });
                };

                _controller.ErrorOccurred += err =>
                {
                    Dispatcher.Invoke(() => Log("Ошибка: " + err));
                };

                _controller.SignalChanged += s =>
                {
                    Dispatcher.Invoke(() => Log($"Сигнал: {s}"));
                };

                _controller.Start();
                Log("Контроллер запущен");
            }
            catch (Exception ex)
            {
                Log("Ошибка при запуске: " + ex.Message);
            }
        }

        private void StopController()
        {
            if (_controller != null)
            {
                _controller.Stop();
                _controller.Dispose();
                _controller = null;
                Log("Контроллер остановлен");
            }
        }

        // Показать / скрыть изображение
        private void ShowImageCheck_Changed(object sender, RoutedEventArgs e)
        {
            UpdateLayoutVisibility();
        }

        // Показать / скрыть лог
        private void ShowLogCheck_Changed(object sender, RoutedEventArgs e)
        {
            UpdateLayoutVisibility();
        }

        // Логирование
        private void Log(string message)
        {
            LogTextBox.AppendText(message + Environment.NewLine);
            LogTextBox.ScrollToEnd();
        }

        private void UpdateLayoutVisibility()
        {
            bool showImage = ShowImageCheck.IsChecked == true;
            bool showLog = ShowLogCheck.IsChecked == true;

            // Скрываем/показываем содержимое
            CameraImage.Visibility = showImage ? Visibility.Visible : Visibility.Collapsed;
            LogTextBox.Visibility = showLog ? Visibility.Visible : Visibility.Collapsed;

            // --- ЛОГИКА РАСПРЕДЕЛЕНИЯ ПРОСТРАНСТВА ---

            if (showImage && showLog)
            {
                // Обычный режим
                ImageRow.Height = new GridLength(3, GridUnitType.Star);
                LogRow.Height = new GridLength(2, GridUnitType.Star);
            }
            else if (!showImage && showLog)
            {
                // Только лог
                ImageRow.Height = new GridLength(0);
                LogRow.Height = new GridLength(1, GridUnitType.Star);
            }
            else if (showImage && !showLog)
            {
                // Только изображение
                ImageRow.Height = new GridLength(1, GridUnitType.Star);
                LogRow.Height = new GridLength(0);
            }
            else
            {
                // Ни логов, ни изображения
                ImageRow.Height = new GridLength(0);
                LogRow.Height = new GridLength(0);

                // Центрируем DataMatrix
                DmTextBox.VerticalAlignment = VerticalAlignment.Center;
                //DmTextBox.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }
    }
}
