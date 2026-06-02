using OperPageLes.CORE.Settings.PaletteElements;
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
using OperPageLes.CORE.Enums;

namespace OperPageLes.UI.Pages.ActionPanel.PaletteWindow
{
    /// <summary>
    /// Логика взаимодействия для PageMainPalettePanelAction.xaml
    /// </summary>
    public partial class PageMainPalettePanelAction : Page
    {
        public PageMainPalettePanelAction()
        {
            InitializeComponent();
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELButtonSelectTheme);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Olive].ConnectPalleteFromIELElement(IELButtonExecuteTheme);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonDeleteTheme);
        }
    }
}
