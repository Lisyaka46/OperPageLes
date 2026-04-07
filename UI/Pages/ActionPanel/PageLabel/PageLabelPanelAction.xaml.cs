using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Struct;
using System.Windows.Controls;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Pages.ActionPanel.PageLabel
{
    /// <summary>
    /// Логика взаимодействия для PageLabelActionPanel.xaml
    /// </summary>
    public partial class PageLabelElementActionPanel : Page
    {
        /// <summary>
        /// Страница управления выделением элемента ярлыка
        /// </summary>
        internal readonly PageLabelSelectManipulatePanelAction PageLabelSelectManipulate;

        public PageLabelElementActionPanel()
        {
            InitializeComponent();
            IELBlockInfoTagLabel.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Tag));
            PageLabelSelectManipulate = new();
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELButtonExecuteLabel);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonChangeLabel);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonRemoveLabel);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(IELBlockInfoTagLabel);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELButtonSetLabelTag);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELButtonActivateSelectMenu);
        }
    }
}
