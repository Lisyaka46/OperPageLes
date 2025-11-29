using ApplicationOperPageLes.CORE.Enums;
using System.Windows.Controls;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel.PageConsole
{
    /// <summary>
    /// Логика взаимодействия для PageMainConsolePanelAction.xaml
    /// </summary>
    public partial class MainPagePanelAction : Page
    {
        public MainPagePanelAction()
        {
            InitializeComponent();

            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonCrearConsole);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonCommandBuffer);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELButtonDiscriptionCommand);
        }
    }
}
