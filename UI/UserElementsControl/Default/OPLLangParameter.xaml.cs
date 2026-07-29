using IEL.UserElementsControl.Base;
using LibraryIEL.CORE.Themes.Palettes;

namespace OperPageLes.UI.UserElementsControl.Default
{
    /// <summary>
    /// Логика взаимодействия для OPLLangParameter.xaml
    /// </summary>
    public partial class OPLLangParameter : IELContainerBase
    {
        #region Palette
        /// <summary>
        /// Объект палитры
        /// </summary>
        public override PaletteData Palette
        {
            get => base.Palette;
            set
            {
                IELTextBoxLangValueTranslate.Palette = value;
                base.Palette = value;
            }
        }
        #endregion

        public OPLLangParameter()
        {
            InitializeComponent();
            TextBlockNotTraslationInfo.Foreground = SourceForeground.SourceBrush;
            TextBlockValueLangExample.Foreground = SourceForeground.SourceBrush;
            TextBlockLangKey.Foreground = SourceForeground.SourceBrush;
            IELTextBoxLangValueTranslate.Palette = Palette;
        }
    }
}
