using ApplicationOperPageLes.CORE.Enums;
using System.Windows.Controls;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel.PageLabel
{
    /// <summary>
    /// Логика взаимодействия для PageLabelMainActionPanel.xaml
    /// </summary>
    public partial class PageLabelMainActionPanel : Page
    {
        public PageLabelMainActionPanel()
        {
            InitializeComponent();

            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELButtonCreateLabel);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELButtonSelectAllLabel);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonClearAllSelect);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELButtonManipulateTags);
        }
    }
}
