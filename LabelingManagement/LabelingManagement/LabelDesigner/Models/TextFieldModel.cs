using LabelDesigner.DesignerVisuals;
using System.Windows;
using System.Windows.Media;

namespace LabelDesigner.Models
{
    public class TextFieldModel : FieldModel
    {
        public TextFieldModel()
        {
            DataType = DataType.Fixed;
        }

        public new TextVisual Visual => (TextVisual)base.Visual;

        public override string FieldType => "Text";
        public string Text
        {
            get => Visual.Content.Text;
            set
            {
                Set(() =>
                {
                    if (value == string.Empty)
                        Visual.Content.Text = " ";
                    else
                        Visual.Content.Text = value;

                    InitialSize = MeasureTextSize();
                    MeasureArrangeVisual();
                });
            }
        }
        public string TextFontFamily
        {
            get => Visual.Content.FontFamily.Source;
            set
            {
                Set(() =>
                {
                    Visual.Content.FontFamily = new(string.IsNullOrEmpty(value) ? "Arial" : value);
                    InitialSize = MeasureTextSize();
                    MeasureArrangeVisual();
                });
            }
        }
        public bool Bold
        {
            get => Visual.Content.FontWeight == FontWeights.Bold;
            set
            {
                Set(() =>
                {
                    Visual.Content.FontWeight = value ? FontWeights.Bold : FontWeights.Normal;
                    InitialSize = MeasureTextSize();
                    MeasureArrangeVisual();
                });
            }
        }

        protected override TextVisual InitializeVisual() => new("DefaultText");
        protected override void InvertField() => (Visual.Content.Background, Visual.Content.Foreground) = (Visual.Content.Foreground, Visual.Content.Background);
        private Size MeasureTextSize()
        {
            FormattedText ft = new(
                Visual.Content.Text.EndsWith(' ') ? Visual.Content.Text + '\0' : Visual.Content.Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(Visual.Content.FontFamily, Visual.Content.FontStyle, Visual.Content.FontWeight, Visual.Content.FontStretch),
                Visual.Content.FontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(Visual.Content).PixelsPerDip
            );

            return new(ft.Width, ft.Height);
        }
    }
}
