using ApplicationOperPageLes.CORE.Struct;
using OIEL.UserElementsControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OIEL.UserElementsControl.Interfaces;
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

        /// <summary>
        /// Объект визуализации объектов уведомления
        /// </summary>
        private StackPanel StackPanelNotifications;

        /// <summary>
        /// Количество объектов визуализации загрузки
        /// </summary>
        private int CountLoadingViewers = 0;

        public PageNotificationManager()
        {
            InitializeComponent();
            StackPanelNotifications = new()
            {
                VerticalAlignment = VerticalAlignment.Top,
            };
            IELScrollNotification.Content = StackPanelNotifications;
        }

        /// <summary>
        /// Установить в страницу объект визуализирующий медиа
        /// </summary>
        /// <returns>Визуализационный объект</returns>
        internal OPLMediaViewer SetViewMediaElement(Uri? uri = null)
        {
            OPLMediaViewer Result = CreateMediaView(uri ?? StructDirectoryResources.GetResourceUri(nameof(OPRES.MediaLoadingDefault)));
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Chocolate].ConnectPalleteFromIELElement(Result);
            StackPanelNotifications.Children.Add(Result);
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(Result, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            CountLoadingViewers++;
            return Result;
        }

        /// <summary>
        /// Установить в страницу объект визуализирующий картинку
        /// </summary>
        /// <returns>Визуализационный объект</returns>
        internal OPLImageViewer SetViewImageElement(ImageSource? source = null)
        {
            if (StackPanelNotifications.Children.Count == 0) CreatedNewOneOnlyViewerImage?.Invoke(this, EventArgs.Empty);
            OPLImageViewer Result = CreateImageView(source ?? StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Warning)));
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Chocolate].ConnectPalleteFromIELElement(Result);
            StackPanelNotifications.Children.Add(Result);
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(Result, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            return Result;
        }

        /// <summary>
        /// Убрать визуализацию загрузочного элемента
        /// </summary>
        /// <param name="Element">Загрузочный элемент визуализации</param>
        internal void DeleteViewMediaElement<T>(IOPLObjectViewer<T> Element) where T : IFormattable
        {
            int i = StackPanelNotifications.Children.IndexOf((UIElement)Element);
            if (i == -1) throw new Exception("Элемент не находится в контейнере менеджера");
            StackPanelNotifications.Children.RemoveAt(i);
            if (typeof(T) == typeof(ImageSource))
            {
                if (CountLoadingViewers == 1) ClearedAllViewersImage?.Invoke(this, EventArgs.Empty);
                CountLoadingViewers--;
            }
        }

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
                SourceElement = uri,
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
                SourceElement = source,
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
