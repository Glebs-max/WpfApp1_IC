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

                _controller.SignalChanged += OnSignalChanged;
                _controller.DataMatrixRead += OnDataMatrixRead;
                _controller.ErrorOccurred += OnErrorOccurred;

                _controller.DataMatrixRead += dm =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        DmTextBox.Text = dm.Normalized;
                    });
                };

                _controller.FrameReceived += frame =>
                {
                    Dispatcher.Invoke(() =>
                    {

                        if (frame != null)
                        {
                            CameraImage.Source = frame;
                            AppendLog("✅ Изображение обновлено");
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
                _controller.FrameReceived -= null;

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
            CameraImage.Visibility =
                CameraImage.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void AppendLog(string text)
        {
            LogTextBox.AppendText(text + Environment.NewLine);
            LogTextBox.ScrollToEnd();
        }
    }
}
