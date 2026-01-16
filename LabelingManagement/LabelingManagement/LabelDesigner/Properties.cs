using LabelDesigner.Models;
using System.Windows.Controls;

namespace LabelDesigner
{
    public abstract class Properties : UserControl
    {
        private static bool _updating;
        private static DesignerItemModel? _item;

        protected static DesignerItemModel? Item
        {
            get => _item;
            set 
            {
                _item = value;
                if (value != null)
                    value.PropertyChanged += (s, e) => UpdateProperties();
            }
        }
        protected static Action? PropertiesUpdate { get; set; }

        public static void UpdateProperties()
        {
            _updating = true;
            PropertiesUpdate?.Invoke();
            _updating = false;
        }

        protected virtual void UpdateItem(Action itemUpdate)
        {
            if (!_updating)
                itemUpdate();
        }
    }
}
