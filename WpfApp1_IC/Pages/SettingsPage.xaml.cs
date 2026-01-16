using Microsoft.Win32;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using WpfApp1_IC.Services;

namespace WpfApp1_IC.Pages
{
    public partial class SettingsPage : Page
    {
        private AppSettings _settings;

        public SettingsPage()
        {
            InitializeComponent();

            _settings = SettingsService.Load();

            // === КАМЕРА ===
            UseCameraCheck.IsChecked = _settings.UseCamera;
            RefreshCameraList();

            // === MODBUS ===
            UseModbusCheck.IsChecked = _settings.UseModbus;
            ModbusIpBox.Text = _settings.ModbusIp;
            ModbusPortBox.Text = _settings.ModbusPort.ToString();

            // === ПРИНТЕР ===
            UsePrinterCheck.IsChecked = _settings.UsePrinter;
            PrinterIpBox.Text = _settings.PrinterIp;
            PrinterPortBox.Text = _settings.PrinterPort.ToString();

            // Выбор типа принтера
            var item = PrinterTypeBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == _settings.PrinterType);

            if (item != null)
                PrinterTypeBox.SelectedItem = item;
            else
                PrinterTypeBox.SelectedIndex = 0;
        }

        // ============================
        // КАМЕРА
        // ============================

        private void RefreshCameraList()
        {
            CameraList.Items.Clear();

            var cameras = CameraDiscovery.FindCameras();

            foreach (var cam in cameras)
                CameraList.Items.Add(cam);

            if (cameras.Any())
                CameraList.SelectedIndex = 0;
        }

        private void RefreshCameras_Click(object sender, RoutedEventArgs e)
        {
            RefreshCameraList();
        }

        // ============================
        // MODBUS
        // ============================

        private void AutoDetectModbus_Click(object sender, RoutedEventArgs e)
        {
            string ip = AutoDetectModbusController();

            if (ip != null)
            {
                ModbusIpBox.Text = ip;
                MessageBox.Show("Контроллер найден: " + ip);
            }
            else
            {
                MessageBox.Show("Контроллер не найден");
            }
        }

        private string AutoDetectModbusController()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up)
                    continue;

                var props = ni.GetIPProperties();

                foreach (var ua in props.UnicastAddresses)
                {
                    if (ua.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    string subnet = ua.Address.ToString();
                    subnet = subnet.Substring(0, subnet.LastIndexOf('.') + 1);

                    for (int i = 1; i < 255; i++)
                    {
                        string testIp = subnet + i;

                        if (IsPortOpen(testIp, 502, 200))
                            return testIp;
                    }
                }
            }

            return null;
        }

        private bool IsPortOpen(string ip, int port, int timeoutMs)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(ip, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(timeoutMs);

                    if (!success)
                        return false;

                    client.EndConnect(result);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        // ============================
        // ПРИНТЕР
        // ============================

        private void TestPrinter_Click(object sender, RoutedEventArgs e)
        {
            string ip = PrinterIpBox.Text;
            int port = int.Parse(PrinterPortBox.Text);

            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(ip, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

                    if (!success)
                        throw new Exception("Таймаут подключения");

                    client.EndConnect(result);
                }

                MessageBox.Show("Принтер доступен", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения: " + ex.Message, "Ошибка");
            }
        }

        // ============================
        // СОХРАНЕНИЕ
        // ============================

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Камера
            _settings.UseCamera = UseCameraCheck.IsChecked == true;

            // Modbus
            _settings.UseModbus = UseModbusCheck.IsChecked == true;
            _settings.ModbusIp = ModbusIpBox.Text;
            _settings.ModbusPort = int.Parse(ModbusPortBox.Text);

            // Принтер
            _settings.UsePrinter = UsePrinterCheck.IsChecked == true;
            _settings.PrinterIp = PrinterIpBox.Text;
            _settings.PrinterPort = int.Parse(PrinterPortBox.Text);
            _settings.PrinterType = (PrinterTypeBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            SettingsService.Save(_settings);

            MessageBox.Show("Настройки сохранены");
        }
    }
}
