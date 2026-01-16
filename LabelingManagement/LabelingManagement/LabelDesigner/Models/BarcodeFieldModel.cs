using DataMatrix.net;
using LabelDesigner.DesignerVisuals;
using LabelDesigner.Services;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LabelDesigner.Models
{
    public class BarcodeFieldModel : FieldModel
    {
        private string _barcodeData = "Default Data";
        private readonly DmtxImageEncoder _dataMatrix = new();
        private readonly DmtxImageEncoderOptions _options = new()
        {
            MarginSize = 0,
            ModuleSize = (int)UnitsService.FromMM(5),
            Scheme = DmtxScheme.DmtxSchemeAsciiGS1,
            SizeIdx = DmtxSymbolSize.DmtxSymbolSquareAuto
        };

        public BarcodeFieldModel()
        {
            DataType = DataType.Database;
            BarcodeData = _barcodeData;
        }

        public new ImageVisual Visual => (ImageVisual)base.Visual;

        public override string FieldType => "Barcode";
        public string BarcodeData
        {
            get => _barcodeData;
            set => Set(ref _barcodeData, value, () => Visual.Content.Source = DisplayBarcode());
        }

        protected override ImageVisual InitializeVisual() => new(DisplayBarcode());
        protected override void InvertField()
        {
            (_options.ForeColor, _options.BackColor) = (_options.BackColor, _options.ForeColor);
            DisplayBarcode();
        }
        private BitmapSource DisplayBarcode() => Imaging.CreateBitmapSourceFromHBitmap(_dataMatrix.EncodeImage(string.IsNullOrEmpty(BarcodeData) ? "Default Data" : BarcodeData, _options).GetHbitmap(), nint.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
}
