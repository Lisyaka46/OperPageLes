using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementsControl;
using ApplicationOperPageLes.UI.UserElementsControl.Interfaces;
using IEL.CORE.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageNotificationManager.xaml
    /// </summary>
    public partial class PageNotificationManager : Page
    {
        /// <summary>
        /// Событие оповещающее что все элементы визуализации картинок были очищены
        /// </summary>
        internal event EventHandler? ClearedAllViewersImage;

        /// <summary>
        /// Событие оповещающее что был создан первый элемент визуализации картинки
        /// </summary>
        internal event EventHandler? CreatedNewOneOnlyViewerImage;

        private int CountImageViewers = 0;

        public PageNotificationManager()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Установить в страницу объект визуализирующий медиа
        /// </summary>
        /// <returns>Визуализационный объект</returns>
        internal OPLMediaViewer SetViewMediaElement(Uri? uri = null)
        {
            OPLMediaViewer Result = CreateMediaView(uri ?? StructDirectoryResources.GetResourceUri(nameof(OPRES.MediaLoadingDefault)));
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Chocolate].ConnectPalleteFromIELElement(Result);
            Result.Margin = GetMarginFromIndex(GridElementsLoading.Children.Count);
            GridElementsLoading.Children.Add(Result);
            App.DoubleAnimationType.AnimateEffect(Result, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            return Result;
        }

        /// <summary>
        /// Установить в страницу объект визуализирующий картинку
        /// </summary>
        /// <returns>Визуализационный объект</returns>
        internal OPLImageViewer SetViewImageElement(ImageSource? source = null)
        {
            if (CountImageViewers == 0) CreatedNewOneOnlyViewerImage?.Invoke(this, EventArgs.Empty);
            CountImageViewers++;
            OPLImageViewer Result = CreateImageView(source ?? StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Warning)));
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Chocolate].ConnectPalleteFromIELElement(Result);
            Result.Margin = GetMarginFromIndex(GridElementsLoading.Children.Count);
            GridElementsLoading.Children.Add(Result);
            App.DoubleAnimationType.AnimateEffect(Result, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            return Result;
        }

        /// <summary>
        /// Убрать визуализацию загрузочного элемента
        /// </summary>
        /// <param name="Element">Загрузочный элемент визуализации</param>
        internal void DeleteViewMediaElement<T>(IOPLObjectViewer<T> Element) where T : IFormattable
        {
            int i = GridElementsLoading.Children.IndexOf((UIElement)Element);
            if (i == -1) throw new Exception("Элемент не находится в контейнере менеджера");
            GridElementsLoading.Children.RemoveAt(i);
            for (; i < GridElementsLoading.Children.Count; i++)
            {
                FrameworkElement element = (FrameworkElement)GridElementsLoading.Children[i];
                App.ThicknessAnimationType.AnimateEffect(element, MarginProperty,
                    GetMarginFromIndex(i), TimeSpan.FromMilliseconds(300d));
            }
            if (typeof(T) == typeof(ImageSource))
            {
                if (CountImageViewers == 1) ClearedAllViewersImage?.Invoke(this, EventArgs.Empty);
                CountImageViewers--;
            }
        }

        /// <summary>
        /// Узнать позицию элемента по его индексу
        /// </summary>
        /// <param name="index">Индекс позиции элемента</param>
        /// <returns></returns>
        private Thickness GetMarginFromIndex(int index) => new(4, index > 0 ? 55 * GridElementsLoading.Children.Count + 5 : 5, 4, 4);

        /// <summary>
        /// Сгенерировать объект отображения Media
        /// </summary>
        /// <param name="uri">Ссылка на используемое медиа</param>
        /// <returns></returns>
        private static OPLMediaViewer CreateMediaView(Uri uri)
        {
            OPLMediaViewer Result = new()
            {
                Margin = new(4),
                CornerRadius = new(5),
                BorderThickness = new(3),
                Source = uri,//new(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingDefault))),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                FontSize = 11d,
                Opacity = 0d,
                Focusable = false,
            };
            return Result;
        }

        /// <summary>
        /// Сгенерировать объект отображения Image
        /// </summary>
        /// <returns></returns>
        private static OPLImageViewer CreateImageView(ImageSource source)
        {
            OPLImageViewer Result = new()
            {
                Margin = new(4),
                CornerRadius = new(5),
                BorderThickness = new(3),
                Source = source,//new(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingDefault))),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                FontSize = 11d,
                Opacity = 0d,
                Focusable = false,
            };
            return Result;
        }
    }
}
