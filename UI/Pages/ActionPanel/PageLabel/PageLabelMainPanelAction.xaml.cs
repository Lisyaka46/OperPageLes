using OperPageLes.CORE.Enums;
using System.Windows.Controls;

namespace OperPageLes.UI.Pages.ActionPanel.PageLabel
{
    /// <summary>
    /// Логика взаимодействия для PageLabelMainActionPanel.xaml
    /// </summary>
    public partial class PageLabelMainActionPanel : Page
    {
        public PageLabelMainActionPanel()
        {
            InitializeComponent();

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELButtonCreateLabel);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELButtonSelectAllLabel);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonClearAllSelect);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELButtonManipulateTags);
        }
    }
}
