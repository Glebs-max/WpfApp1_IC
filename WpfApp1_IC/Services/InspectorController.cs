using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp1_IC.Services
{
    public class InspectorController : IDisposable
    {
        private readonly ModbusService _modbus;
        private readonly CameraService _camera;

        private CancellationTokenSource _cts;

        // Фильтр дребезга
        private int _stableSignal = -1;
        private int _previousSignal = -1;
        private int _sameCount = 0;
        private const int FILTER_COUNT = 3;

        // Флаг, чтобы не триггерить камеру несколько раз подряд
        private bool _triggerInProgress = false;

        public event Action<int> SignalChanged;
        public event Action<DataMatrixResult> DataMatrixRead;
        public event Action<string> ErrorOccurred;
        public event Action<BitmapSource> FrameReceived;


        public InspectorController(ModbusService modbus, CameraService camera)
        {
            _modbus = modbus;
            _camera = camera;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(() => Loop(token), token);
        }

        private async Task Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    int rawSignal = _modbus.ReadSignal();

                    // === ФИЛЬТР ДРЕБЕЗГА ===
                    if (rawSignal == _stableSignal)
                    {
                        _sameCount++;
                    }
                    else
                    {
                        _sameCount = 0;
                        _stableSignal = rawSignal;
                    }

                    // Сигнал считается изменившимся только если он стабилен
                    if (_sameCount >= FILTER_COUNT)
                    {
                        if (_stableSignal != _previousSignal)
                        {
                            _previousSignal = _stableSignal;
                            SignalChanged?.Invoke(_stableSignal);

                            // === ЛОГИКА ТРИГГЕРА ===
                            if (_stableSignal == 1 && !_triggerInProgress)
                            {
                                _triggerInProgress = true;

                                var (dm, frame) = _camera.TriggerAndRead();

                                if(frame != null)
                                    FrameReceived?.Invoke(frame);
                                if (dm == null)
                                {
                                    ErrorOccurred?.Invoke("DataMatrix not read, activating rejector");
                                    _modbus.ActivateRejector();
                                }
                                else
                                {
                                    DataMatrixRead?.Invoke(dm);
                                }
                            }

                            // Когда сигнал вернулся в 0 — разрешаем следующий триггер
                            if (_stableSignal == 0)
                            {
                                _triggerInProgress = false;
                            }
                        }
                    }

                    await Task.Delay(50, token);
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(ex.Message);
                }
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        public void Dispose()
        {
            Stop();
            _modbus?.Dispose();
            _camera?.Dispose();
        }
    }
}
