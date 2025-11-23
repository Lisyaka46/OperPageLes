using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl;
using ApplicationOperPageLes.UI.UserElementControl.Interfaces;
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

        /// <summary>
        /// Настройка отображения фонового цвета объекта
        /// </summary>
        private readonly QData QDtataBackground = new(
                        [
                        [255, 107, 187, 189],
                        [255, 43, 194, 245],
                        [255, 145, 128, 185],
                        [255, 189, 78, 78],
                        ]);

        /// <summary>
        /// Настройка отображения цвета границ объекта
        /// </summary>
        private readonly QData QDtataBorderBrush = new(
                        [
                        [255, 0, 0, 0],
                        [255, 13, 0, 55],
                        [255, 20, 43, 95],
                        [255, 90, 10, 15],
                        ]);

        /// <summary>
        /// Настройка отображнения цвета текста объекта
        /// </summary>
        private readonly QData QDtataForeground = new(
                        [
                        [255, 27, 67, 69],
                        [255, 13, 84, 155],
                        [255, 12, 68, 85],
                        [255, 189, 78, 78],
                        ]);

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
            OPLMediaViewer Result = CreateMediaView(uri ?? new(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingDefault))));
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
        private OPLMediaViewer CreateMediaView(Uri uri)
        {
            OPLMediaViewer Result = new()
            {
                Margin = new(4),
                CornerRadius = new(5),
                BorderThickness = new(3),
                SourceView = uri,//new(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingDefault))),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                FontSize = 11d,
                Opacity = 0d,
                Focusable = false,
                Background = QDtataBackground,
                BorderBrush = QDtataBorderBrush,
                Foreground = QDtataForeground
            };
            return Result;
        }

        /// <summary>
        /// Сгенерировать объект отображения Image
        /// </summary>
        /// <returns></returns>
        private OPLImageViewer CreateImageView(ImageSource source)
        {
            OPLImageViewer Result = new()
            {
                Margin = new(4),
                CornerRadius = new(5),
                BorderThickness = new(3),
                SourceView = source,//new(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingDefault))),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                FontSize = 11d,
                Opacity = 0d,
                Focusable = false,
                Background = QDtataBackground,
                BorderBrush = QDtataBorderBrush,
                Foreground = QDtataForeground
            };
            return Result;
        }
    }
}
