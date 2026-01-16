using EasyModbus;
using System;

namespace WpfApp1_IC.Services
{
    public class ModbusService : IDisposable
    {
        private ModbusClient _client;
        private readonly string _ip;
        private readonly int _port;

        public ModbusService(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        // Подключение
        public void Connect()
        {
            if (_client != null && _client.Connected)
                return;

            _client = new ModbusClient(_ip, _port);

            try
            {
                _client.Connect();
            }
            catch (Exception ex)
            {
                throw new Exception("Modbus connection failed: " + ex.Message);
            }
        }

        // Отключение
        public void Disconnect()
        {
            try
            {
                if (_client != null && _client.Connected)
                    _client.Disconnect();
            }
            catch { }

            _client = null;
        }

        // Чтение сигнала
        public int ReadSignal()
        {
            if (_client == null || !_client.Connected)
                throw new Exception("Modbus not connected");

            try
            {
                bool[] coils = _client.ReadCoils(0, 1);
                return coils[0] ? 1 : 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Modbus read failed: " + ex.Message);
            }
        }

        // Активация отбраковщика
        public void ActivateRejector()
        {
            if (_client == null || !_client.Connected)
                return;

            try
            {
                _client.WriteSingleCoil(0, true);
                System.Threading.Thread.Sleep(300);
                _client.WriteSingleCoil(0, false);
            }
            catch { }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
