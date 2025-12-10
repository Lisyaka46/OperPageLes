using ApplicationOperPageLes.CORE.Struct;
using ColorPicker;
using ColorPicker.Models;
using IEL.CORE.Classes;
using IEL.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using OPRES = ApplicationOperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;

namespace ApplicationOperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для DialogQDataSpectrum.xaml
    /// </summary>
    public partial class DialogQDataSpectrum : Window
    {
        //
        private QData.EnumDataSpectrum ChangeSpectrum;

        //
        private Border? BorderSelect;

        //
        private PaletteSpectrum PaletteSpectrumManipulate;

        //
        private QData? QdataActiveChange;

        public DialogQDataSpectrum()
        {
            PaletteSpectrumManipulate = new();
            InitializeComponent();
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Tilda));

            ColorPicker.ColorChanged += (sender, e) =>
            {
                if (QdataActiveChange == null) return;
                WnColor color = ((ColorRoutedEventArgs)e).Color;
                QdataActiveChange.SetFromSpectrumColor(ChangeSpectrum, color);
                if (BorderSelect == null) return;
                ((SolidColorBrush)BorderSelect.Background).Color = color;
                ((SolidColorBrush)((TextBlock)BorderSelect.Child).Foreground).Color =
                    WnColor.FromRgb((byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
            };

            //BorderColorBackground.MouseLeftButtonUp += (sender, e) =>
            //{
            //    ActivateColorPicker(BorderColorBackground, PaletteSpectrumManipulate.BG);
            //};

            //BorderColorBorderBrush.MouseLeftButtonUp += (sender, e) =>
            //{
            //    ActivateColorPicker(BorderColorBorderBrush, PaletteSpectrumManipulate.BB);
            //};

            //BorderColorForeground.MouseLeftButtonUp += (sender, e) =>
            //{
            //    ActivateColorPicker(BorderColorForeground, PaletteSpectrumManipulate.FG);
            //};
        }

        /// <summary>
        /// Отобразить окно манипуляции над спектом QData
        /// </summary>
        /// <param name="qdBG">Данные фона</param>
        /// <param name="qdBB">Данные границ</param>
        /// <param name="qdFG">Данные текста</param>
        /// <param name="spectrum">Изменяемый спектр</param>
        internal void ShowDialogChangeQData(PaletteSpectrum SourceSpectrum, QData.EnumDataSpectrum spectrum)
        {
            PaletteSpectrumManipulate = SourceSpectrum;
            ChangeSpectrum = spectrum;
            Title = $"Изменение спектра \"{spectrum}\"";
            NameSpectrum.Text = $"Спектр {spectrum}";

            BorderColorBackground.Background = new SolidColorBrush(PaletteSpectrumManipulate.BG.GetFromSpectrumColor(spectrum));
            BorderColorBorderBrush.Background = new SolidColorBrush(PaletteSpectrumManipulate.BB.GetFromSpectrumColor(spectrum));
            BorderColorForeground.Background = new SolidColorBrush(PaletteSpectrumManipulate.FG.GetFromSpectrumColor(spectrum));

            WnColor SourceColor = PaletteSpectrumManipulate.BG.GetFromSpectrumColor(spectrum);
            TextBlockBackgroundNaming.Foreground = new SolidColorBrush(
                WnColor.FromRgb((byte)(255 - SourceColor.R), (byte)(255 - SourceColor.G), (byte)(255 - SourceColor.B)));

            SourceColor = PaletteSpectrumManipulate.BB.GetFromSpectrumColor(spectrum);
            TextBlockBorderBrushNaming.Foreground = new SolidColorBrush(
                WnColor.FromRgb((byte)(255 - SourceColor.R), (byte)(255 - SourceColor.G), (byte)(255 - SourceColor.B)));

            SourceColor = PaletteSpectrumManipulate.FG.GetFromSpectrumColor(spectrum);
            TextBlockForegroundNaming.Foreground = new SolidColorBrush(
                WnColor.FromRgb((byte)(255 - SourceColor.R), (byte)(255 - SourceColor.G), (byte)(255 - SourceColor.B)));

            IELBlockExample50.PaletteElement = PaletteSpectrumManipulate;
            IELBlockExample50.SetActiveSpecrum((StateSpectrum)spectrum + 1, false);

            IELBlockExampleB50.PaletteElement = PaletteSpectrumManipulate;
            IELBlockExampleB50.SetActiveSpecrum((StateSpectrum)spectrum + 1, false);

            IELBlockExample80.PaletteElement = PaletteSpectrumManipulate;
            IELBlockExample80.SetActiveSpecrum((StateSpectrum)spectrum + 1, false);

            IELBlockExampleB80.PaletteElement = PaletteSpectrumManipulate;
            IELBlockExampleB80.SetActiveSpecrum((StateSpectrum)spectrum + 1, false);


            ActivateColorPicker(BorderColorBackground, PaletteSpectrumManipulate.BG);

            ShowDialog();
        }

        //
        private void ActivateColorPicker(Border border, QData Source)
        {
            QdataActiveChange = Source;
            BorderSelect = border;
            WnColor color = Source.GetFromSpectrumColor(ChangeSpectrum);
            ColorPicker.Color.RGB_R = color.R;
            ColorPicker.Color.RGB_G = color.G;
            ColorPicker.Color.RGB_B = color.B;
            ColorPicker.Color.A = color.A;
            App.DoubleAnimationType.AnimateEffect(ColorPicker, StandardColorPicker.OpacityProperty, 1d, TimeSpan.FromMilliseconds(400d));
            App.DoubleAnimationType.AnimateEffect(TextBlockSelectIndicator, OpacityProperty, 1d, TimeSpan.FromMilliseconds(400d));

            // Смещение позиции области относительно внешнего элемента
            System.Windows.Point OffsetPosElement = border.TransformToAncestor(
                GridColors).Transform(new System.Windows.Point(0, 0));

            App.ThicknessAnimationType.AnimateEffect(TextBlockSelectIndicator, MarginProperty,
                new(TextBlockSelectIndicator.Margin.Left, OffsetPosElement.Y + 10, TextBlockSelectIndicator.Margin.Right, TextBlockSelectIndicator.Margin.Bottom),
                TimeSpan.FromMilliseconds(400d));
        }
    }
}
