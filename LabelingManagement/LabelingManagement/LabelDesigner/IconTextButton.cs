using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LabelDesigner
{
    public class IconTextButton : Button
    {
        static IconTextButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconTextButton), new FrameworkPropertyMetadata(typeof(IconTextButton)));
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty
            .Register(nameof(Icon), typeof(ImageSource), typeof(IconTextButton), new PropertyMetadata(null));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty
            .Register(nameof(Text), typeof(string), typeof(IconTextButton), new PropertyMetadata(string.Empty));
    }

}
