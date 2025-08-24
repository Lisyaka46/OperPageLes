using IEL.CORE.Classes;
using OperPage_les.CORE.Label;
using OperPage_les.UI.UserElementControl;
using System.Windows.Controls;

namespace OperPage_les.UI.Pages.ActionPanel.PageLabel
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
            IELBlockInfoTagLabel.Imaging = App.LoadImage(Properties.Resources.Tag);
            PageLabelSelectManipulate = new();
            PanelActionPageSelectLabel = new(PageLabelSelectManipulate);
        }
    }
}
