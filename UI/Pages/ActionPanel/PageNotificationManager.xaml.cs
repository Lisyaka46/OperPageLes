using OperPageLes.CORE.Objects;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.UserElementsControl.Default;
using OIEL.UserElementsControl;
using OIEL.UserElementsControl.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Pages.ActionPanel
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
            App.CurrentApp.AddNotification += AddNewNotification;
            Initialized += (sender, e) =>
            {
                LoadingAllNotification();
            };
        }

        /// <summary>
        /// Добавить отображение нового уведомления
        /// </summary>
        /// <param name="Sender">Объект вызываемый событие</param>
        /// <param name="SourceNotification">Объект данных уведомления</param>
        private void AddNewNotification(object? Sender, Notification SourceNotification)
        {
            OPLNotification ViewNotification = new(in SourceNotification)
            {
                Opacity = 0d,
                Margin = new(6d),
                CornerRadius = new(5d),
            };
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Cocoa].ConnectPalleteFromIELElement(ViewNotification);
            ViewNotification.MouseRightButtonUp += (sender, e) =>
            {
                RemoveAtNotification(StackPanelNotifications.Children.IndexOf((UIElement)sender));
            };

            StackPanelNotifications.Children.Add(ViewNotification);
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(ViewNotification, MarginProperty, new(4d), TimeSpan.FromMilliseconds(600d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(ViewNotification, OpacityProperty, 1d, TimeSpan.FromMilliseconds(600d));
        }

        /// <summary>
        /// Удалить элемент уведомления
        /// </summary>
        /// <param name="Index">Индекс удаляемого уведомления</param>
        private void RemoveAtNotification(int Index)
        {
            App.CurrentApp.RemoveAtNotification(Index);
            OPLNotification Element = (OPLNotification)StackPanelNotifications.Children[Index];
            Element.Height = Element.ActualHeight;
            DoubleAnimation Animation = App.ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
            Animation.From = Element.ActualHeight;
            Animation.To = 0d;
            Animation.FillBehavior = FillBehavior.Stop;
            Animation.Completed += (sender, e) =>
                StackPanelNotifications.Children.RemoveAt(Index);
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(Element, MarginProperty, new(0), TimeSpan.FromMilliseconds(500d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(Element, OpacityProperty, 0d, TimeSpan.FromMilliseconds(400d));
            Element.BeginAnimation(HeightProperty, Animation);
        }

        /// <summary>
        /// Загрузить и отобразить все элементы уведомлений в приложении
        /// </summary>
        private void LoadingAllNotification()
        {
            foreach (Notification Element in App.CurrentApp.ApplicationNotifications)
            {
                AddNewNotification(null, Element);
            }
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
