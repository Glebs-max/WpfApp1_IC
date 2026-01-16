using System.Windows.Threading;

namespace LabelDesigner.Services
{
    public static class TimerService
    {
        private static readonly DispatcherTimer timer = new()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        public static event Action? OnTick;

        public static void StartTimer()
        {
            timer.Start();
            timer.Tick += (s, e) => OnTick?.Invoke();
        }
        public static void StopTimer()
        {
            timer.Stop();
            timer.Tick -= (s, e) => OnTick?.Invoke();
        }
    }
}
