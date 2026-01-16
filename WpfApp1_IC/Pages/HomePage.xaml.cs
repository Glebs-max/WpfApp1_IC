using System;
using System.Windows;
using System.Windows.Controls;
using WpfApp1_IC.Services;

namespace WpfApp1_IC.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SettingsPage());
        }

        private void StartInspection_Click(object sender, RoutedEventArgs e)
        {
            var settings = SettingsService.Load();

            ModbusService modbus = null;
            CameraService camera = null;

            try
            {
                if (settings.UseModbus)
                {
                    modbus = new ModbusService(settings.ModbusIp, settings.ModbusPort);
                    modbus.Connect();
                }

                if (settings.UseCamera)
                {
                    camera = new CameraService();
                    camera.Open(); // камера открывается только сейчас
                }

                var controller = new InspectorController(modbus, camera);
                controller.RejectDelayMs = settings.RejectDelayMs;
                ((MainWindow)Application.Current.MainWindow).CurrentController = controller;


                NavigationService.Navigate(new CameraBasicPage(controller));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при запуске устройств:\n" + ex.Message,
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                modbus?.Dispose();
                camera?.Dispose();
            }
        }
    }
}
