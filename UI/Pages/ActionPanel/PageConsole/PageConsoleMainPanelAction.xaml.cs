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
        }
    }
}
