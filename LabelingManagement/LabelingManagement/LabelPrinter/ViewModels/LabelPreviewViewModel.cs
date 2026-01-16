using LabelDesigner;
using LabelPrinter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrinter.ViewModels
{
    public class LabelPreviewViewModel(MainViewModel mainViewModel) : ObservableObject
    {
        public DesignerViewModel? DesignerViewModel { get; set; }

        public void GetBack() => mainViewModel.CurrentViewModel = mainViewModel.GetViewModel<ProductsViewModel>();
    }
}
