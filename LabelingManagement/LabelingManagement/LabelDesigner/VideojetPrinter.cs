using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace LabelDesigner
{
    public class VidejetPrinter(string ip = "192.168.10.2", int portTextComms = 9100, int portZplEmulation = 1000) : IDisposable
    {
        private TcpClient? _textComms, _zplEmulation;
        private NetworkStream? _streamTextComms, _streamZplEmulation;

        public bool TextCommsConnected => _textComms?.Connected ?? false;
        public bool ZplEmulationConnected => _zplEmulation?.Connected ?? false;
        public bool IsConnected => TextCommsConnected && ZplEmulationConnected;

        public async Task ConnectAsync()
        {
            if (!TextCommsConnected)
            {
                _textComms = new TcpClient();
                await _textComms.ConnectAsync(ip, portTextComms);
                _streamTextComms = _textComms.GetStream();
                _streamTextComms.WriteTimeout = 5000;
                _streamTextComms.ReadTimeout = 5000;
            }
            if (!ZplEmulationConnected)
            {
                _zplEmulation = new TcpClient();
                await _zplEmulation.ConnectAsync(ip, portZplEmulation);
                _streamZplEmulation = _zplEmulation.GetStream();
                _streamZplEmulation.WriteTimeout = 5000;
                _streamZplEmulation.ReadTimeout = 5000;
            }
        }
        public void Disconnect()
        {
            try
            {
                _streamTextComms?.Close();
                _streamZplEmulation?.Close();
                _textComms?.Close();
                _zplEmulation?.Close();
            }
            catch { }

            _streamTextComms = null;
            _streamZplEmulation = null;
            _textComms = null;
            _zplEmulation = null;
        }
        public async Task SendZplAsync(string zpl)
        {
            if (!IsConnected || _streamZplEmulation == null)
                throw new InvalidOperationException("Not connected to printer.");

            byte[] bytes = Encoding.ASCII.GetBytes(zpl);

            await _streamZplEmulation.WriteAsync(bytes);
            await _streamZplEmulation.FlushAsync();
        }
        public async Task ClearQueue()
        {
            if (!IsConnected || _streamTextComms == null)
                throw new InvalidOperationException("Not connected to printer.");

            StreamReader _reader = new(_streamTextComms, Encoding.ASCII);
            StreamWriter _writer = new(_streamTextComms, Encoding.ASCII) { AutoFlush = true };

            await _writer.WriteAsync("CQI\r");
        }
        /*public async Task<string> ReceiveAsync(int bufferSize = 4096)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to printer.");

            byte[] buffer = new byte[bufferSize];
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }*/

        public void Dispose()
        {
            Disconnect();
        }
    }
}
