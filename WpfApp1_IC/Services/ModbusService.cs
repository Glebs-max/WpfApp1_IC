using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyModbus;

namespace WpfApp1_IC.Services
{
    public class ModbusService : IDisposable
    {
        private readonly ModbusClient _client;

        public ModbusService(string ipAddress, int port)
        {
            _client = new ModbusClient(ipAddress, port);
        }

        public void Connect()
        {
            _client.Connect();
        }

        public int ReadSignal()
        {
            int[] values = _client.ReadHoldingRegisters(0, 1);
            return values[0];
        }

        public void ActivateRejector(int durationMs = 500)
        {
            _client.WriteSingleCoil(0, true);
            System.Threading.Thread.Sleep(durationMs);
            _client.WriteSingleCoil(0, false);
        }

        public void Dispose()
        {
            if (_client.Connected)
                _client.Disconnect();
        }
    }
}
