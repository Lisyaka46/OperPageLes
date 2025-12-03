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

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonCrearConsole);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonCommandBuffer);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELButtonDiscriptionCommand);
        }
    }
}
