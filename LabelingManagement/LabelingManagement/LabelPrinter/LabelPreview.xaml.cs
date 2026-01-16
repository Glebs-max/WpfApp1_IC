using LabelDesigner;
using LabelPrinter.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace LabelPrinter
{
    public partial class LabelPreview : UserControl
    {
        public LabelPreview()
        {
            InitializeComponent();
            PreviewArea.Preview = true;
        }

        private LabelPreviewViewModel VM => (LabelPreviewViewModel)DataContext;

        private void SendPrint_Click(object sender, RoutedEventArgs e)
        {

        }
        private void GetBack_Click(object sender, RoutedEventArgs e) => VM.GetBack();
    }
}
