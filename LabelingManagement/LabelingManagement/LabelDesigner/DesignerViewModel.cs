using LabelDesigner.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Serialization;

namespace LabelDesigner
{
    public class DesignerViewModel : ObservableObject
    {
        private double _scale;
        private LabelAreaModel _labelArea;
        private FieldModel? _selectedField;
        private DesignerItemModel _propertiesTarget;
        private ToolboxItemType _selectedTool;

        public DesignerViewModel()
        {
            _scale = 4.0;
            _propertiesTarget = _labelArea = new();
            _selectedTool = ToolboxItemType.None;
        }

        public double Scale
        {
            get => _scale;
            set => Set(ref _scale, value);
        }
        [XmlIgnore]
        public DesignerItemModel PropertiesTarget
        {
            get => _propertiesTarget;
            set => Set(ref _propertiesTarget, value);
        }
        public LabelAreaModel LabelArea
        {
            get => _labelArea;
            set
            {
                PropertiesTarget = value;
                Set(ref _labelArea, value);
            }
        }
        [XmlIgnore]
        public FieldModel? SelectedField
        {
            get => _selectedField;
            set
            {
                if (_selectedField != null)
                    _selectedField.Adorner.Visibility = Visibility.Hidden;

                LabelArea.Adorner.Visibility = Visibility.Visible;

                if (value != null)
                {
                    value.Adorner.Visibility = Visibility.Visible;
                    LabelArea.Adorner.Visibility = Visibility.Hidden;
                }

                PropertiesTarget = value != null ? value : LabelArea;
                Set(ref _selectedField, value);
            }
        }
        [XmlIgnore]
        public ToolboxItemType SelectedTool
        {
            get => _selectedTool;
            set
            {
                SelectedField = null;
                Set(ref _selectedTool, value);
            }
        }
        public ObservableCollection<FieldModel> Fields { get; } = [];
    }
}