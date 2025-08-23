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
            PageLabelSelectManipulate = new();
            PanelActionPageSelectLabel = new(PageLabelSelectManipulate);

            GridMainVisibleTag.MouseWheel += (sender, e) =>
            {
                if (e.Delta > 0) ScrollViewerTags.LineLeft();
                else if (e.Delta < 0) ScrollViewerTags.LineRight();
            };
        }

        /// <summary>
        /// Обновить отображение тегов
        /// </summary>
        /// <param name="tags">Обновляемые теги</param>
        internal void UpdateVisibleTag(LabelTag[] tags)
        {
            GridMainVisibleTag.Children.Clear();
            GridMainVisibleTag.ColumnDefinitions.Clear();
            for (int i = 0; i < tags.Length; i++)
            {
                GridMainVisibleTag.ColumnDefinitions.Add(new()
                {
                    Width = new(0, System.Windows.GridUnitType.Auto),
                });
                GridMainVisibleTag.Children.Add(OPLLabelCommand.CreateVisualTag(tags[i]));
                Grid.SetColumn(GridMainVisibleTag.Children[i], i);
            }
        }
    }
}
