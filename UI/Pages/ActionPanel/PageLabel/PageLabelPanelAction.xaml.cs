using IEL.CORE.Classes;
using ApplicationOperPageLes.CORE.Label;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl;
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

        /// <summary>
        /// Страница панели действий взаимодействия с ярлыком в разделе выделения элемента
        /// </summary>
        internal readonly PagePanelAction PanelActionPageSelectLabel;

        public PageLabelElementActionPanel()
        {
            InitializeComponent();
            //IELBlockInfoTagLabel.Imaging = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Tag));
            PageLabelSelectManipulate = new();
            PanelActionPageSelectLabel = new(PageLabelSelectManipulate);
        }
    }
}
