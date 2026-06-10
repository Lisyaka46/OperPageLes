using ColorPicker;
using ColorPicker.Models;
using IEL.CORE.Classes;
using IEL.CORE.Enums;
using OperPageLes.CORE.Struct;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
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
using OPRES = OperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для DialogQDataSpectrum.xaml
    /// </summary>
    public partial class DialogQDataSpectrum : Window, IOPLAnimate
    {
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        //
        private QData.EnumDataSpectrum ChangeSpectrum;

        //
        private Border? BorderSelect;

        /// <summary>
        /// Спектр палитры который представляет визуализацию изменений
        /// </summary>
        private PaletteSpectrum? PaletteSpectrumManipulate;

        /// <summary>
        /// Спектр палитры который является изменяемым
        /// </summary>
        private PaletteSpectrum? PaletteSpectrumSource;

        //
        private QData? QdataActiveChange;

        public DialogQDataSpectrum()
        {
            InitializeComponent();
            //Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Tilda));

            ColorPicker.ColorChanged += (sender, e) =>
            {
                if (QdataActiveChange == null) return;
                WnColor color = ((ColorRoutedEventArgs)e).Color;
                QdataActiveChange.SetFromSpectrumColor(ChangeSpectrum, color);
                if (BorderSelect == null) return;
                ((SolidColorBrush)BorderSelect.Background).Color = color;
            };

            BorderColorBackground.MouseLeftButtonUp += (sender, e) =>
            {
                if (PaletteSpectrumManipulate != null)
                    ActivateColorPicker(BorderColorBackground, PaletteSpectrumManipulate.BG);
            };

            BorderColorBorderBrush.MouseLeftButtonUp += (sender, e) =>
            {
                if (PaletteSpectrumManipulate != null)
                    ActivateColorPicker(BorderColorBorderBrush, PaletteSpectrumManipulate.BB);
            };

            BorderColorForeground.MouseLeftButtonUp += (sender, e) =>
            {
                if (PaletteSpectrumManipulate != null)
                    ActivateColorPicker(BorderColorForeground, PaletteSpectrumManipulate.FG);
            };

            IELButtonCancel.OnActivateMouseLeft += (sender, e) =>
            {
                Close();
            };

            IELButtonComplete.OnActivateMouseLeft += (sender, e) =>
            {
                if (PaletteSpectrumSource != null && PaletteSpectrumManipulate != null)
                {
                    PaletteSpectrumSource.BG.ChangeSourceQData(PaletteSpectrumManipulate.BG);
                    PaletteSpectrumSource.BB.ChangeSourceQData(PaletteSpectrumManipulate.BB);
                    PaletteSpectrumSource.FG.ChangeSourceQData(PaletteSpectrumManipulate.FG);
                }
                Close();
            };

            Closed += (sender, e) =>
            {
                PaletteSpectrumSource = null;
                PaletteSpectrumManipulate = null;
            };
        }

        /// <summary>
        /// Отобразить окно манипуляции над спектом QData
        /// </summary>
        /// <param name="SourceSpectrum">Объект манипуляции спектра палитры</param>
        /// <param name="spectrum">Изменяемый спектр</param>
        internal void ShowDialogChangeQData(PaletteSpectrum SourceSpectrum, QData.EnumDataSpectrum spectrum)
        {
            PaletteSpectrumSource = SourceSpectrum;
            PaletteSpectrumManipulate = (PaletteSpectrum)SourceSpectrum.Clone();

            ChangeSpectrum = spectrum;
            Title = $"Изменение спектра \"{spectrum}\"";
            NameSpectrum.Text = $"Спектр {spectrum}";

            BorderColorBackground.Background = new SolidColorBrush(PaletteSpectrumManipulate.BG.GetFromSpectrumColor(spectrum));
            BorderColorBorderBrush.Background = new SolidColorBrush(PaletteSpectrumManipulate.BB.GetFromSpectrumColor(spectrum));
            BorderColorForeground.Background = new SolidColorBrush(PaletteSpectrumManipulate.FG.GetFromSpectrumColor(spectrum));

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
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ColorPicker, StandardColorPicker.OpacityProperty,
                1d, TimeSpan.FromMilliseconds(400d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockSelectIndicator, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(400d));

            // Смещение позиции области относительно внешнего элемента
            System.Windows.Point OffsetPosElement = border.TransformToAncestor(
                GridColors).Transform(new System.Windows.Point(0, 0));

            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockSelectIndicator, MarginProperty,
                new Thickness(TextBlockSelectIndicator.Margin.Left, OffsetPosElement.Y - 4, TextBlockSelectIndicator.Margin.Right, TextBlockSelectIndicator.Margin.Bottom),
                TimeSpan.FromMilliseconds(300d));
        }
    }
}
