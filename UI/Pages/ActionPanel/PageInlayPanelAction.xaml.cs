using IEL.GUI;
using System.Windows.Controls;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageActionInlay.xaml
    /// </summary>
    public partial class PageInlayPanelAction : Page
    {
        private IELInlay? _ActivateManipulateInlay;
        /// <summary>
        /// Активная вкладка для взаимодействия с ней
        /// </summary>
        internal IELInlay? ActivateManipulateInlay
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
