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

            App.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonCrearConsole, PaletteSpectrumEnum.PastelRed);
            App.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonCommandBuffer, PaletteSpectrumEnum.Purple);
            App.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonDiscriptionCommand, PaletteSpectrumEnum.Jade);
        }
    }
}
