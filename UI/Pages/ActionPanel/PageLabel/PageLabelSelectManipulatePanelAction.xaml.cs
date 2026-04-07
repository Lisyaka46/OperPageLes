using OperPageLes.CORE.Enums;
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

namespace OperPageLes.UI.Pages.ActionPanel.PageLabel
{
    /// <summary>
    /// Логика взаимодействия для PageLabelSelectManipulatePanelAction.xaml
    /// </summary>
    public partial class PageLabelSelectManipulatePanelAction : Page
    {
        public PageLabelSelectManipulatePanelAction()
        {
            InitializeComponent();
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELButtonBack);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Olive].ConnectPalleteFromIELElement(IELButtonExecuteSelect);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Cocoa].ConnectPalleteFromIELElement(IELButtonClearSelect);
        }
    }
}
