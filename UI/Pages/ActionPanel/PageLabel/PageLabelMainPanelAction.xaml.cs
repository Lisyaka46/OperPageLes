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

            IELButtonManipulateTags.Background = App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BG_PastelBlue);
            IELButtonManipulateTags.BorderBrush = App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BB_PastelBlue);
            IELButtonManipulateTags.Foreground = App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.FG_PastelBlue);
        }
    }
}
