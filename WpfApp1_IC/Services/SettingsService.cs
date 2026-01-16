using System;
using System.IO;
using System.Text.Json;

namespace WpfApp1_IC.Services
{
    public class AppSettings
    {
        public bool UseCamera { get; set; } = true;
        public bool UseModbus { get; set; } = true;
        public bool UsePrinter { get; set; } = false;

        public string ModbusIp { get; set; } = "192.168.0.127";
        public int ModbusPort { get; set; } = 502;

        public string PrinterType { get; set; } = "Эмулятор (заглушка)";
        public string PrinterIp { get; set; } = "192.168.0.150";
        public int PrinterPort { get; set; } = 9100;

        public string IdmvsPath { get; set; } = ""; // ← ВОЗВРАЩАЕМ ЭТО

        public int RejectDelayMs { get; set; } = 500;
    }





    public static class SettingsService
    {
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static AppSettings Load()
        {
            if (!File.Exists(FilePath))
                return new AppSettings();

            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void Save(AppSettings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }
    }
}
