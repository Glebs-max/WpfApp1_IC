using LabelDesigner.DesignerVisuals;
using System.Windows;
using System.Xml.Serialization;

namespace LabelDesigner.Models
{
    [XmlInclude(typeof(FieldModel))]
    [XmlInclude(typeof(LabelAreaModel))]
    [XmlInclude(typeof(TextFieldModel))]
    [XmlInclude(typeof(ImageFieldModel))]
    [XmlInclude(typeof(DateFieldModel))]
    [XmlInclude(typeof(TimeFieldModel))]
    [XmlInclude(typeof(BarcodeFieldModel))]
    public abstract class DesignerItemModel : ObservableObject
    {
        public DesignerItemModel()
        {
            Visual = InitializeVisual();
            Adorner = InitializeAdorner();
        }

        public VisualElement Visual { get; }
        public DesignerItemAdorner Adorner { get; }

        public double MinWidth
        {
            get => Visual.MinWidth;
            set => Set(() => Visual.MinWidth = value);
        }
        public double MinHeight
        {
            get => Visual.MinHeight;
            set => Set(() => Visual.MinHeight = value);
        }

        protected abstract VisualElement InitializeVisual();
        protected abstract DesignerItemAdorner InitializeAdorner();
    }
}
