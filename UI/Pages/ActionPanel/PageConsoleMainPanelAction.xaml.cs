using System.Windows.Controls;

namespace OperPage_les.Windows.Frames
{
    /// <summary>
    /// Логика взаимодействия для PageMainConsolePanelAction.xaml
    /// </summary>
    public partial class PageMainConsolePanelAction : Page
    {
        public PageMainConsolePanelAction()
        {
            InitializeComponent();
            IELButtonCrearConsole.IELSettingObject.SettingMouseImage = App.ResourceDefaultMouseImageSetting;
            IELButtonCommandBuffer.IELSettingObject.SettingMouseImage = App.ResourceDefaultMouseImageSetting;
        }
    }
}
