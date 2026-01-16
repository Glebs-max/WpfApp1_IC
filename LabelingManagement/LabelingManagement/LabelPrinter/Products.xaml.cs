using LabelPrinter.Models;
using LabelPrinter.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace LabelPrinter
{
    public partial class Products : UserControl
    {
        public Products()
        {
            InitializeComponent();

            ProductsTable.LoadingRow += (s, e) => e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            ProductsTable.MouseDoubleClick += (s, e) => LoadLabel();
            ProductsTable.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                    LoadLabel();
            };
        }

        private ProductsViewModel VM => (ProductsViewModel)DataContext;

        private void LoadLabel()
        {
            if (ProductsTable.SelectedItem is gtin product)
                VM.LoadLabel("C:\\Users\\dvornikov.ga\\Desktop\\label.xml");
            //VM.LoadLabel(product.TemplateLabel ?? "");
        }
        private void GetBack_Click(object sender, RoutedEventArgs e) => VM.GetBack();
    }
}
