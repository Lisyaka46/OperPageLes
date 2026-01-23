using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Interfaces;
using ApplicationOperPageLes.UI.UserElementsControl;
using System.Windows.Controls;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel.PageConsole
{
    /// <summary>
    /// Логика взаимодействия для PageMainConsolePanelAction.xaml
    /// </summary>
    public partial class MainPagePanelAction : Page
    {
        private OPLCommandViewer? _CommandViewerSelect = null;
        /// <summary>
        /// Выделенный объект визуализатора в консоли
        /// </summary>
        internal OPLCommandViewer? CommandViewerSelect
        {
            get => _CommandViewerSelect;
            set
            {
                IELButtonDeleteCommandViewer.IsEnabled = value != null;
                _CommandViewerSelect = value;
            }
        }
        public MainPagePanelAction()
        {
            InitializeComponent();

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.VioletRed].ConnectPalleteFromIELElement(IELButtonDeleteCommandViewer);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonDeleteAllCommandViewers);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonCommandBuffer);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELButtonDiscriptionCommand);

            IELButtonDeleteCommandViewer.IsEnabled = false;
        }
    }
}
