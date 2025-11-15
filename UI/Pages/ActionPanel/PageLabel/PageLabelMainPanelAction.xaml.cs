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

            IELButtonManipulateTags.QBackground.SetQData(App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BG_PastelBlue));
            IELButtonManipulateTags.QBorderBrush.SetQData(App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BB_PastelBlue));
            IELButtonManipulateTags.QForeground.SetQData(App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.FG_PastelBlue));
        }
    }
}
