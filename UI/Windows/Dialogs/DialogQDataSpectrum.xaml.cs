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
        private int SelectIndex = -1;

        //
        private Border? BorderSelect;

        //
        private readonly QData[] QDataArray;

        public DialogQDataSpectrum()
        {
            QDataArray = [new(), new(), new()];
            InitializeComponent();
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Tilda));

            ColorPicker.ColorChanged += (sender, e) =>
            {
                if (SelectIndex == -1) return;
                WnColor color = ((ColorRoutedEventArgs)e).Color;
                QDataArray[SelectIndex].SetFromSpectrumColor(ChangeSpectrum, color);
                if (BorderSelect == null) return;
                ((SolidColorBrush)BorderSelect.Background).Color = color;
                ((SolidColorBrush)((TextBlock)BorderSelect.Child).Foreground).Color =
                    WnColor.FromRgb((byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
            };

            BorderColorBackground.MouseLeftButtonUp += (sender, e) =>
            {
                ActivateColorPicker(BorderColorBackground, 0);
            };

            BorderColorBorderBrush.MouseLeftButtonUp += (sender, e) =>
            {
                ActivateColorPicker(BorderColorBorderBrush, 1);
            };

            BorderColorForeground.MouseLeftButtonUp += (sender, e) =>
            {
                ActivateColorPicker(BorderColorForeground, 2);
            };
        }

        /// <summary>
        /// Отобразить окно манипуляции над спектом QData
        /// </summary>
        /// <param name="qdBG">Данные фона</param>
        /// <param name="qdBB">Данные границ</param>
        /// <param name="qdFG">Данные текста</param>
        /// <param name="spectrum">Изменяемый спектр</param>
        internal void ShowDialogChangeQData(QData qdBG, QData qdBB, QData qdFG, QData.EnumDataSpectrum spectrum)
        {
            QDataArray[0] = qdBG;
            QDataArray[1] = qdBB;
            QDataArray[2] = qdFG;
            ChangeSpectrum = spectrum;
            Title = $"Изменение спектра \"{spectrum}\"";
            NameSpectrum.Text = $"Спектр {spectrum}";

            BorderColorBackground.Background = new SolidColorBrush(QDataArray[0].GetFromSpectrumColor(spectrum));
            BorderColorBorderBrush.Background = new SolidColorBrush(QDataArray[1].GetFromSpectrumColor(spectrum));
            BorderColorForeground.Background = new SolidColorBrush(QDataArray[2].GetFromSpectrumColor(spectrum));

            WnColor SourceColor = QDataArray[0].GetFromSpectrumColor(spectrum);
            TextBlockBackgroundNaming.Foreground = new SolidColorBrush(
                WnColor.FromRgb((byte)(255 - SourceColor.R), (byte)(255 - SourceColor.G), (byte)(255 - SourceColor.B)));

            SourceColor = QDataArray[1].GetFromSpectrumColor(spectrum);
            TextBlockBorderBrushNaming.Foreground = new SolidColorBrush(
                WnColor.FromRgb((byte)(255 - SourceColor.R), (byte)(255 - SourceColor.G), (byte)(255 - SourceColor.B)));

            SourceColor = QDataArray[2].GetFromSpectrumColor(spectrum);
            TextBlockForegroundNaming.Foreground = new SolidColorBrush(
                WnColor.FromRgb((byte)(255 - SourceColor.R), (byte)(255 - SourceColor.G), (byte)(255 - SourceColor.B)));

            IELBlockExample50.SourceBackground.SetQData(QDataArray[0]);
            IELBlockExample50.SourceBorderBrush.SetQData(QDataArray[1]);
            IELBlockExample50.SourceForeground.SetQData(QDataArray[2]);
            IELBlockExample50.SetActiveSpecrum((StateSpectrum)spectrum + 1, false);

            IELBlockExampleB50.SourceBackground.SetQData(QDataArray[0]);
            IELBlockExampleB50.SourceBorderBrush.SetQData(QDataArray[1]);
            IELBlockExampleB50.SourceForeground.SetQData(QDataArray[2]);
            IELBlockExampleB50.SetActiveSpecrum((StateSpectrum)spectrum + 1, false);

            IELBlockExample80.SourceBackground.SetQData(QDataArray[0]);
            IELBlockExample80.SourceBorderBrush.SetQData(QDataArray[1]);
            IELBlockExample80.SourceForeground.SetQData(QDataArray[2]);
            IELBlockExample80.SetActiveSpecrum((StateSpectrum)spectrum + 1, false);

            IELBlockExampleB80.SourceBackground.SetQData(QDataArray[0]);
            IELBlockExampleB80.SourceBorderBrush.SetQData(QDataArray[1]);
            IELBlockExampleB80.SourceForeground.SetQData(QDataArray[2]);
            IELBlockExampleB80.SetActiveSpecrum((StateSpectrum)spectrum + 1, false);


            ActivateColorPicker(BorderColorBackground, 0);

            ShowDialog();
        }

        //
        private void ActivateColorPicker(Border border, int Index)
        {
            SelectIndex = Index;
            BorderSelect = border;
            WnColor color = QDataArray[Index].GetFromSpectrumColor(ChangeSpectrum);
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
