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
            IELButtonCrearConsole.IELSettingObject.SettingMouseImage = App.CurrentApp.ResourceDefaultMouseImageSetting;
            IELButtonCommandBuffer.IELSettingObject.SettingMouseImage = App.CurrentApp.ResourceDefaultMouseImageSetting;

            IELButtonCommandBuffer.QBackground.SetQData(App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BG_PastelBlue));
            IELButtonCommandBuffer.QBorderBrush.SetQData(App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BB_PastelBlue));
            IELButtonCommandBuffer.QForeground.SetQData(App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.FG_PastelBlue));
        }
    }
}
