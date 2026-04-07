using IEL.UserElementsControl;
using OIEL.UserElementsControl;
using System.Windows.Controls;

namespace OperPageLes.UI.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageActionInlay.xaml
    /// </summary>
    public partial class PageInlayPanelAction : Page
    {
        private OPLInlay? _ActivateManipulateInlay;
        /// <summary>
        /// Активная вкладка для взаимодействия с ней
        /// </summary>
        internal OPLInlay? ActivateManipulateInlay
        {
            get
            {
                return _ActivateManipulateInlay;
            }
            set
            {
                bool ActivateManipulate = value != null;
                IELButtonPageOpenInlay.IsEnabled = ActivateManipulate && (!value?.SourceBackground.GetUsedState() ?? false);
                IELButtonPageDeleteInlay.IsEnabled = ActivateManipulate;
                _ActivateManipulateInlay = value;
            }
        }

        public PageInlayPanelAction()
        {
            InitializeComponent();
            ActivateManipulateInlay = null;
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELButtonPageOpenInlay);
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonPageDeleteInlay);
        }
    }
}
