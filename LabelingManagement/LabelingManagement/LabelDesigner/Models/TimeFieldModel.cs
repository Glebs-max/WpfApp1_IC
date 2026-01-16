using LabelDesigner.Services;

namespace LabelDesigner.Models
{
    public class TimeFieldModel : TextFieldModel
    {
        private DateTime _time = DateTime.Now;
        private string _timeFormat = $"HH$mm$ss";
        private char _separator = ':';

        public TimeFieldModel()
        {
            DataType = DataType.Current;
            DisplayTime();
            TimerService.OnTick += () => Time = DateTime.Now;
        }

        public override string FieldType => "Time";
        public DateTime Time
        {
            get => _time;
            set => Set(ref _time, value, DisplayTime);
        }
        public string TimeFormat
        {
            get => _timeFormat;
            set => Set(ref _timeFormat, value, DisplayTime);
        }
        public char Separator
        {
            get => _separator;
            set => Set(ref _separator, value, DisplayTime);
        }

        private void DisplayTime()
        {
            if (_separator == '\\')
                Text = TimeOnly.FromDateTime(_time).ToString(_timeFormat.Replace("$", "\\\\"));
            else
                Text = TimeOnly.FromDateTime(_time).ToString(_timeFormat.Replace('$', _separator));
        }
    }
}
