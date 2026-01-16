using LabelDesigner;
using LabelDesigner.Services;
using LabelPrinter.Data;
using LabelPrinter.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace LabelPrinter.ViewModels
{
    public class ProductsViewModel : ObservableObject
    {
        private readonly MainViewModel _mainViewModel;
        private readonly AppDbContext _db;

        public ProductsViewModel(MainViewModel mainViewModel, AppDbContext db)
        {
            _mainViewModel = mainViewModel;
            _db = db;
            _ = LoadProductsAsync();
        }

        public ObservableCollection<gtin> GTINs { get; } = [];

        public async Task LoadProductsAsync()
        {
            var list1 = await _db.gtins.AsNoTracking().ToListAsync();
            GTINs.Clear();
            foreach (var p in list1) GTINs.Add(p);
        }
        public void LoadLabel(string labelPath)
        {
            LabelPreviewViewModel model = _mainViewModel.GetViewModel<LabelPreviewViewModel>();
            model.DesignerViewModel = DesignerService.LoadLabel(labelPath);
            _mainViewModel.CurrentViewModel = model;
        }
        public void GetBack() => _mainViewModel.CurrentViewModel = null;
    }
}
