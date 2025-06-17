using OperPage_les.CORE.Label;
using OperPage_les.UI.UserElementControl;
using System.Windows.Controls;

namespace OperPage_les.Windows.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageLabelActionPanel.xaml
    /// </summary>
    public partial class PageLabelElementActionPanel : Page
    {
        public PageLabelElementActionPanel()
        {
            InitializeComponent();
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
