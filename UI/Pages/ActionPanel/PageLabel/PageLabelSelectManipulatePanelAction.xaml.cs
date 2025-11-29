using ApplicationOperPageLes.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel.PageLabel
{
    /// <summary>
    /// Логика взаимодействия для PageLabelSelectManipulatePanelAction.xaml
    /// </summary>
    public partial class PageLabelSelectManipulatePanelAction : Page
    {
        public PageLabelSelectManipulatePanelAction()
        {
            InitializeComponent();
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELButtonBack);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Olive].ConnectPalleteFromIELElement(IELButtonExecuteSelect);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Cocoa].ConnectPalleteFromIELElement(IELButtonClearSelect);
        }
    }
}
