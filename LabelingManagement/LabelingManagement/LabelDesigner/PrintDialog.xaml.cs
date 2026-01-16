using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LabelDesigner
{
    public partial class PrintDialog : Window
    {
        private readonly DesignerCanvas _canvas;

        public PrintDialog(DesignerCanvas canvas)
        {
            InitializeComponent();
            _canvas = canvas;
        }

        private async void Print_Click(object sender, RoutedEventArgs e)
        {
            string ip = IpBox.Text.Trim();
            int port = PortBox.Value ?? 0;

            try
            {
                string zpl = _canvas.ConvertToZpl();

                VidejetPrinter printer = new(ip, port);
                await printer.ConnectAsync();

                if (ClearQueueCheck.IsChecked ?? false)
                    await printer.ClearQueue();

                for (int i = 0; i < CountBox.Value; i++)
                    await printer.SendZplAsync(_canvas.ConvertToZpl());

                MessageBox.Show("Label sent to printer.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                printer.Disconnect();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
