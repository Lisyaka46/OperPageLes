using IEL.CORE.Classes;
using IEL.UserElementsControl.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OperPageLes.UI.UserElementsControl.Default
{
    /// <summary>
    /// Логика взаимодействия для OPLLangParameter.xaml
    /// </summary>
    public partial class OPLLangParameter : IELContainerBase
    {
        #region PaletteElement
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty PaletteElementProperty =
            DependencyProperty.Register("PaletteElement", typeof(PaletteSpectrum), typeof(OPLLangParameter),
                new(new PaletteSpectrum(),
                    (sender, e) =>
                    {
                        PaletteSpectrum palette = (PaletteSpectrum)e.NewValue;
                        ((OPLLangParameter)sender).PaletteElement = palette;
                    }));

        /// <summary>
        /// Объект палитры
        /// </summary>
        public new PaletteSpectrum PaletteElement
        {
            get => (PaletteSpectrum)GetValue(PaletteElementProperty);
            set
            {
                IELTextBoxLangValueTranslate.PaletteElement = value;
                SetValue(PaletteElementProperty, value);
            }
        }
        #endregion

        public OPLLangParameter()
        {
            InitializeComponent();
            TextBlockNotTraslationInfo.Foreground = SourceForeground.SourceBrush;
            TextBlockValueLangExample.Foreground = SourceForeground.SourceBrush;
            TextBlockLangKey.Foreground = SourceForeground.SourceBrush;
            IELTextBoxLangValueTranslate.PaletteElement = PaletteElement;
        }
    }
}
