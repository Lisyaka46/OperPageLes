using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Label;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl;
using IEL.CORE.Classes;
using System.Windows.Controls;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel.PageLabel
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
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELButtonExecuteLabel);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonChangeLabel);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonRemoveLabel);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(IELBlockInfoTagLabel);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELButtonSetLabelTag);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELButtonActivateSelectMenu);
        }
    }
}
