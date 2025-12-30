using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp1_IC.Services;

namespace WpfApp1_IC.Pages
{
    public partial class CameraBasicPage : Page
    {
        private InspectorController _controller;

        public CameraBasicPage()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var modbus = new ModbusService("192.168.0.127", 502);
                modbus.Connect();

                var camera = new CameraService();
                camera.InitCamera();

                _controller = new InspectorController(modbus, camera);

                // Подписки
                _controller.SignalChanged += OnSignalChanged;
                _controller.DataMatrixRead += OnDataMatrixRead;
                _controller.ErrorOccurred += OnErrorOccurred;

                // Обновление строки DM
                _controller.DataMatrixRead += dm =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        DmTextBox.Text = dm.Normalized;
                    });
                };

                // Обновление изображения
                _controller.FrameReceived += frame =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (frame != null)
                        {
                            CameraImage.Source = frame;
                            AppendLog("Кадр обновлён");
                        }
                        else
                        {
                            AppendLog("⚠ Изображение не получено");
                        }
                    });
                };

                _controller.Start();
                AppendLog("Контроллер запущен");
            }
            catch (Exception ex)
            {
                AppendLog("Ошибка при запуске: " + ex.Message);
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (_controller != null)
            {
                _controller.SignalChanged -= OnSignalChanged;
                _controller.DataMatrixRead -= OnDataMatrixRead;
                _controller.ErrorOccurred -= OnErrorOccurred;

                _controller.Stop();
                _controller.Dispose();
                _controller = null;

                AppendLog("Контроллер остановлен");
            }
        }

        private void OnSignalChanged(int signal)
        {
            Dispatcher.Invoke(() =>
            {
                AppendLog($"Сигнал изменился: {signal}");

            });
        }

        private void OnDataMatrixRead(DataMatrixResult result)
        {
            Dispatcher.Invoke(() =>
            {
                AppendLog("DataMatrix прочитан");
                AppendLog("RAW:");
                AppendLog(result.Raw);
                AppendLog("NORMALIZED:");
                AppendLog(result.Normalized);
            });
        }

        private void OnErrorOccurred(string error)
        {
            Dispatcher.Invoke(() =>
            {
                AppendLog("Ошибка: " + error);
            });
        }

        private void ToggleImage_Click(object sender, RoutedEventArgs e)
        {
            if (CameraImage.Visibility == Visibility.Visible)
            {
                CameraImage.Visibility = Visibility.Collapsed;
                ImageRow.Height = new GridLength(0); // скрыть строку
            }
            else
            {
                CameraImage.Visibility = Visibility.Visible;
                ImageRow.Height = new GridLength(300); // вернуть высоту
            }
        }


        private void AppendLog(string text)
        {
            LogTextBox.AppendText(text + Environment.NewLine);
            LogTextBox.ScrollToEnd();
        }

        private void SaveFrame(BitmapSource frame)
        {
            try
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(frame));

                string path = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "camera_frame.png"
                );

                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Create))
                {
                    encoder.Save(stream);
                }

                AppendLog("📁 Кадр сохранён: camera_frame.png");
            }
            catch (Exception ex)
            {
                AppendLog("❌ Ошибка сохранения кадра: " + ex.Message);
            }
        }
    }
}
