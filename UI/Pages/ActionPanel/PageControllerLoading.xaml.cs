using OperPageLes.CORE.Struct;
using OperPageLes.UI.UserElementControl;
using System.Windows.Controls;

namespace OperPageLes.UI.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageControllerLoading.xaml
    /// </summary>
    public partial class PageControllerLoading : Page
    {
        public PageControllerLoading()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Установить в страницу объект визуализирующий загрузочный процесс
        /// </summary>
        /// <returns>Визуализационный объект</returns>
        internal OPLViewerLoadingProcess SetViewElementLoading()
        {
            TextBlockNotInfo.Visibility = System.Windows.Visibility.Hidden;
            OPLViewerLoadingProcess Result = CreateLoadingView();
            Result.Margin = new(4, GridElementsLoading.Children.Count > 0 ? 55 * GridElementsLoading.Children.Count + 5 : 5, 4, 4);
            GridElementsLoading.Children.Add(Result);
            App.AnimateDoubleEffect(Result, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            return Result;
        }

        /// <summary>
        /// Убрать визуализацию загрузочного элемента
        /// </summary>
        /// <param name="Element">Загрузочный элемент визуализации</param>
        internal void DeleteViewElementLoading(OPLViewerLoadingProcess Element)
        {
            int i = GridElementsLoading.Children.IndexOf(Element);
            GridElementsLoading.Children.Remove(Element);
            for (; i < GridElementsLoading.Children.Count && i > -1; i++)
            {
                OPLViewerLoadingProcess element = (OPLViewerLoadingProcess)GridElementsLoading.Children[i];
                App.AnimateThicknessEffect(element, MarginProperty,
                    new(4, i > 0 ? 55 * GridElementsLoading.Children.Count + 5 : 5, 4, 4), TimeSpan.FromMilliseconds(300d));
            }
            if (GridElementsLoading.Children.Count == 0)
            {
                TextBlockNotInfo.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Сгенерировать объект отображения процесса загрузки
        /// </summary>
        /// <returns></returns>
        private static OPLViewerLoadingProcess CreateLoadingView()
        {
            OPLViewerLoadingProcess Result = new()
            {
                Margin = new(4),
                CornerRadius = new(5),
                BorderThicknessBlock = new(3),
                SourceMediaLoading = new(StructDirectoryResources.DirectoryFileLoadingDefault),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                FontSize = 11d,
                Opacity = 0d,
                Focusable = false,
                IELSettingObject = new()
                {
                    BackgroundSetting = new(new byte[,]
                        {
                        { 255, 107, 187, 189 },
                        { 255, 43, 194, 245 },
                        { 255, 145, 128, 185 },
                        { 255, 189, 78, 78 },
                        })
                },
            };
            return Result;
        }
    }
}
