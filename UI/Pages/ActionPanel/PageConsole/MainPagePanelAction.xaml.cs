using OPLAPI.OIEL.UserElementsControl;
using System.Windows.Controls;
using OperPageLes.CORE.Enums.Theme;

namespace OperPageLes.UI.Pages.ActionPanel.PageConsole
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

            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.VioletRed].ConnectPalleteFromIELElement(IELButtonDeleteCommandViewer);
            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonDeleteAllCommandViewers);
            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.Purple].ConnectPalleteFromIELElement(IELButtonCommandBuffer);

            IELButtonDeleteCommandViewer.IsEnabled = false;
        }
    }
}
