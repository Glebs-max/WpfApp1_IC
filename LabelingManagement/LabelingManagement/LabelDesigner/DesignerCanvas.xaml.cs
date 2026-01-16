using LabelDesigner.Models;
using LabelDesigner.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LabelDesigner
{
    public partial class DesignerCanvas : UserControl
    {
        public DesignerCanvas()
        {
            InitializeComponent();

            MouseLeftButtonDown += (s, mouse) =>
            {
                if (VM.SelectedTool == ToolboxItemType.None)
                {
                    VM.SelectedField = null;
                    return;
                }

                FieldModel? field = VM.SelectedTool switch
                {
                    ToolboxItemType.Text => new TextFieldModel(),
                    ToolboxItemType.Image => new ImageFieldModel(),
                    ToolboxItemType.Date => new DateFieldModel(),
                    ToolboxItemType.Time => new TimeFieldModel(),
                    ToolboxItemType.Barcode => new BarcodeFieldModel(),
                    ToolboxItemType.None => null,
                    _ => null
                };

                if (field != null)
                {
                    field.Visual.Loaded += (s, e) =>
                    {
                        field.FieldName = $"Field.{VM.Fields.Count(f => f.FieldName.StartsWith("Field.")):D2}";
                        field.KeepAspectRatio = true;
                        field.X = mouse.GetPosition(this).X / VM.Scale;
                        field.Y = mouse.GetPosition(this).Y / VM.Scale;
                        field.H = VM.LabelArea.H * 0.5 / VM.Scale;
                    };

                    VM.Fields.Add(field);
                    VM.SelectedTool = ToolboxItemType.None;
                    Mouse.OverrideCursor = null;
                }
            };
            MouseRightButtonDown += (s, e) =>
            {
                VM.SelectedTool = ToolboxItemType.None;
                Mouse.OverrideCursor = null;
            };
            MouseEnter += (s, e) =>
            {
                Focus();
            };
            PreviewMouseWheel += (s, e) =>
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    VM.Scale = Math.Clamp(VM.Scale + (e.Delta > 0 ? 0.05 : -0.05), 0.1, 10);
                    e.Handled = true;
                }
            };
            KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case Key.Delete:
                        if (VM.SelectedField != null)
                        {
                            if (CanvasArea.Children.Contains(VM.SelectedField.Adorner))
                                CanvasArea.Children.Remove(VM.SelectedField.Adorner);

                            if (CanvasArea.Children.Contains(VM.SelectedField.Visual))
                                CanvasArea.Children.Remove(VM.SelectedField.Visual);

                            VM.Fields.Remove(VM.SelectedField);
                        }
                        break;
                }
            };
            DataContextChanged += (s, e) =>
            {
                if (DataContext == null)
                    return;

                CanvasArea.Children.Clear();
                VM.SelectedField = null;

                if (Preview)
                {
                    VM.LabelArea.Grid = false;
                    CanvasArea.Children.Add(VM.LabelArea.Visual);

                    foreach (FieldModel field in VM.Fields)
                    {
                        CanvasArea.Children.Add(field.Visual);
                    }
                }
                else
                {
                    CanvasArea.Children.Add(VM.LabelArea.Visual);
                    CanvasArea.Children.Add(VM.LabelArea.Adorner);

                    foreach (FieldModel field in VM.Fields)
                    {
                        field.Visual.MouseLeftButtonDown += (s, e) =>
                        {
                            if (VM.SelectedTool != ToolboxItemType.None)
                                return;

                            VM.SelectedField = field;
                            e.Handled = true;
                            Mouse.OverrideCursor = null;
                        };
                        field.Visual.MouseEnter += (s, e) =>
                        {
                            if (VM.SelectedTool == ToolboxItemType.None)
                                Mouse.OverrideCursor = Cursors.Hand;
                        };
                        field.Visual.MouseLeave += (s, e) =>
                        {
                            if (VM.SelectedTool == ToolboxItemType.None)
                                Mouse.OverrideCursor = null;
                        };

                        CanvasArea.Children.Add(field.Visual);
                        CanvasArea.Children.Add(field.Adorner);
                    }

                    SetupDataContext();
                }
            };

            Loaded += (s, e) => TimerService.StartTimer();
        }

        public bool Preview { get; set; }
        private DesignerViewModel VM => (DesignerViewModel)DataContext;

        public string ConvertToZpl(double dpi = 300)
        {
            VM.Scale = 1;

            foreach (DesignerItemAdorner adorner in CanvasArea.Children.OfType<DesignerItemAdorner>())
            {
                adorner.Visibility = Visibility.Collapsed;
            }

            return BitmapService.ConvertToZpl(BitmapService.Monochrome(BitmapService.GetBitmap(CanvasArea, dpi, new(VM.LabelArea.W, VM.LabelArea.H))));
        }

        private void SetupDataContext()
        {
            VM.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(VM.LabelArea):
                        CanvasArea.Children.RemoveRange(0, 2);
                        CanvasArea.Children.Insert(0, VM.LabelArea.Visual);
                        CanvasArea.Children.Insert(1, VM.LabelArea.Adorner);
                        break;
                }
            };
            VM.Fields.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (FieldModel field in e.NewItems)
                    {
                        field.Visual.MouseLeftButtonDown += (s, e) =>
                        {
                            if (VM.SelectedTool != ToolboxItemType.None)
                                return;

                            VM.SelectedField = field;
                            e.Handled = true;
                            Mouse.OverrideCursor = null;
                        };
                        field.Visual.MouseEnter += (s, e) =>
                        {
                            if (VM.SelectedTool == ToolboxItemType.None)
                                Mouse.OverrideCursor = Cursors.Hand;
                        };
                        field.Visual.MouseLeave += (s, e) =>
                        {
                            if (VM.SelectedTool == ToolboxItemType.None)
                                Mouse.OverrideCursor = null;
                        };
                        field.Visual.Loaded += (s, e) =>
                        {
                            VM.SelectedField = field;
                        };

                        CanvasArea.Children.Add(field.Visual);
                        CanvasArea.Children.Add(field.Adorner);
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (FieldModel field in e.OldItems)
                    {
                        field.Visual.MouseLeftButtonDown -= (s, e) =>
                        {
                            if (VM.SelectedTool != ToolboxItemType.None)
                                return;

                            VM.SelectedField = field;
                            e.Handled = true;
                            Mouse.OverrideCursor = null;
                        };
                        field.Visual.MouseEnter -= (s, e) =>
                        {
                            if (VM.SelectedTool == ToolboxItemType.None)
                                Mouse.OverrideCursor = Cursors.Hand;
                        };
                        field.Visual.MouseLeave -= (s, e) =>
                        {
                            if (VM.SelectedTool == ToolboxItemType.None)
                                Mouse.OverrideCursor = null;
                        };
                        CanvasArea.Children.Remove(field.Visual);
                        CanvasArea.Children.Remove(field.Adorner);
                    }
                }
            };
        }
    }
}
