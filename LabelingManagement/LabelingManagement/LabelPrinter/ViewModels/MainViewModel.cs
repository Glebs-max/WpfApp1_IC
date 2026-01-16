using LabelDesigner;
using LabelDesigner.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LabelPrinter.ViewModels
{
    public class MainViewModel(IServiceProvider provider) : ObservableObject
    {
        private ObservableObject? _currentViewModel;

        public ObservableObject? CurrentViewModel
        {
            get => _currentViewModel;
            set => Set(ref  _currentViewModel, value);
        }

       public T GetViewModel<T>() where T : ObservableObject => provider.GetRequiredService<T>();
    }
}
