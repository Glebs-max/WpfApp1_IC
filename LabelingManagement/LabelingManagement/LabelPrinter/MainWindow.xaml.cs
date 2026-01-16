using System.Windows;
using LabelPrinter.ViewModels;

namespace LabelPrinter
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}