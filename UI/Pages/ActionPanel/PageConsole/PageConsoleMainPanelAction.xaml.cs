using ApplicationOperPageLes.CORE.Enums;
using System.Windows.Controls;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel.PageConsole
{
    /// <summary>
    /// Логика взаимодействия для PageMainConsolePanelAction.xaml
    /// </summary>
    public partial class PageMainConsolePanelAction : Page
    {
        public PageMainConsolePanelAction()
        {
            InitializeComponent();

            App.CurrentApp.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonCrearConsole, PaletteSpectrumEnum.PastelRed);
            App.CurrentApp.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonCommandBuffer, PaletteSpectrumEnum.Purple);
            App.CurrentApp.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonDiscriptionCommand, PaletteSpectrumEnum.Jade);
        }
    }
}
