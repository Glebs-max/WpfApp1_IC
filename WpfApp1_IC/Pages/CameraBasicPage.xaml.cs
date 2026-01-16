using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1_IC.Services;

namespace WpfApp1_IC.Pages
{
    public partial class CameraBasicPage : Page
    {
        private InspectorController _controller;
        private bool _isRunning = false;

        public CameraBasicPage(InspectorController controller = null)
        {
            InitializeComponent();

            // Начальные состояния UI
            ShowImageCheck.IsChecked = true;
            ShowLogCheck.IsChecked = true;

            // Если контроллер передан — используем его
            if (controller != null)
            {
                _controller = controller;
                Log("Контроллер передан из HomePage.");
            }
            else
            {
                // Если контроллера нет — создаём локальный
                Log("Контроллер не передан. Создаю локальный контроллер...");

                var settings = SettingsService.Load();

                ModbusService modbus = null;
                CameraService camera = null;

                try
                {
                    if (settings.UseModbus)
                        modbus = new ModbusService(settings.ModbusIp, settings.ModbusPort);

                    if (settings.UseCamera)
                        camera = new CameraService();

                    _controller = new InspectorController(modbus, camera);
                    _controller.RejectDelayMs = settings.RejectDelayMs;

                    Log("Локальный контроллер создан. Можно запускать инспекцию.");
                }
                catch (Exception ex)
                {
                    Log("Ошибка при создании локального контроллера: " + ex.Message);
                }
            }

            // Подписываемся на события (только если контроллер создан)
            if (_controller != null)
            {
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
            }
            else
            {
                Log("Контроллер не создан. Инспекция недоступна.");
            }

            UpdateLayoutVisibility();
        }

        // ===== КНОПКА СТАРТ / СТОП =====
        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (_controller == null)
            {
                Log("Невозможно запустить инспекцию — контроллер отсутствует.");
                return;
            }

            var button = (Button)sender;
            _isRunning = !_isRunning;

            if (_isRunning)
            {
                try
                {
                    _controller.Start();

                    StartStopIcon.Data = Geometry.Parse("M 0 0 H 6 V 20 H 0 Z M 10 0 H 16 V 20 H 10 Z");
                    StartStopIcon.Fill = Brushes.Red;
                    button.ToolTip = "Стоп";

                    Log("Инспекция запущена");
                }
                catch (Exception ex)
                {
                    Log("Ошибка запуска инспекции: " + ex.Message);
                }
            }
            else
            {
                _controller.Stop();

                StartStopIcon.Data = Geometry.Parse("M 0 0 L 0 20 L 17 10 Z");
                StartStopIcon.Fill = Brushes.Green;
                button.ToolTip = "Старт";

                Log("Инспекция остановлена");
            }
        }

        // ===== ПОКАЗАТЬ / СКРЫТЬ ИЗОБРАЖЕНИЕ =====
        private void ShowImageCheck_Changed(object sender, RoutedEventArgs e)
        {
            UpdateLayoutVisibility();
        }

        // ===== ПОКАЗАТЬ / СКРЫТЬ ЛОГ =====
        private void ShowLogCheck_Changed(object sender, RoutedEventArgs e)
        {
            UpdateLayoutVisibility();
        }

        // ===== ОБНОВЛЕНИЕ РАСПОЛОЖЕНИЯ =====
        private void UpdateLayoutVisibility()
        {
            bool showImage = ShowImageCheck.IsChecked == true;
            bool showLog = ShowLogCheck.IsChecked == true;

            CameraImage.Visibility = showImage ? Visibility.Visible : Visibility.Collapsed;
            LogTextBox.Visibility = showLog ? Visibility.Visible : Visibility.Collapsed;

            if (showImage && showLog)
            {
                ImageRow.Height = new GridLength(3, GridUnitType.Star);
                LogRow.Height = new GridLength(2, GridUnitType.Star);
            }
            else if (showImage && !showLog)
            {
                ImageRow.Height = new GridLength(1, GridUnitType.Star);
                LogRow.Height = new GridLength(0);
            }
            else if (!showImage && showLog)
            {
                ImageRow.Height = new GridLength(0);
                LogRow.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                ImageRow.Height = new GridLength(0);
                LogRow.Height = new GridLength(0);
            }
        }

        // ===== ЛОГИРОВАНИЕ =====
        private void Log(string message)
        {
            LogTextBox.AppendText(message + Environment.NewLine);
            LogTextBox.ScrollToEnd();
        }
    }
}
