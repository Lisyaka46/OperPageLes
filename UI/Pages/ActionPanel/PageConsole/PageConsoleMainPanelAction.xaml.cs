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
            //IELButtonCrearConsole.IELSettingObject.SettingMouseImage = App.CurrentApp.ResourceDefaultMouseImageSetting;
            //IELButtonCommandBuffer.IELSettingObject.SettingMouseImage = App.CurrentApp.ResourceDefaultMouseImageSetting;

            IELButtonCommandBuffer.Background = App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BG_PastelBlue);
            IELButtonCommandBuffer.BorderBrush = App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BB_PastelBlue);
            IELButtonCommandBuffer.Foreground = App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.FG_PastelBlue);
        }
    }
}
