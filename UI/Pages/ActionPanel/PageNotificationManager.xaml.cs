using OperPageLes.CORE.Enums.Language;
using OperPageLes.CORE.Objects;
using OperPageLes.UI.UserElementsControl.Default;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.CORE.Language;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OperPageLes.UI.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageNotificationManager.xaml
    /// </summary>
    public partial class PageNotificationManager : Page, IOPLAnimate
    {
        private OPLAnimationManager? _ManagerAnimation;
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation
        {
            get => _ManagerAnimation;
            set
            {
                _ManagerAnimation = value;

            }
        }

        /// <summary>
        /// Объект визуализации объектов уведомления
        /// </summary>
        private StackPanel StackPanelNotifications;

        public PageNotificationManager()
        {
            InitializeComponent();
            StackPanelNotifications = new()
            {
                VerticalAlignment = VerticalAlignment.Top,
            };
            IELScrollNotification.Content = StackPanelNotifications;
            App.CurrentApp.AddNotification += AddNewNotification;
            Lang.LanguageUpdated += Lang_LanguageUpdated;
            Lang_LanguageUpdated(null, EventArgs.Empty);
            LoadingAllNotification();
        }

        /// <summary>
        /// Обработчик события изменения языкового перевода
        /// </summary>
        private void Lang_LanguageUpdated(object? sender, EventArgs e)
        {
            TextBlockNotificationManagerTitle.Text = Lang.GetValue(LangUITranslate.NotificationManager);
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
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(ViewNotification);
            ViewNotification.MouseRightButtonUp += (sender, e) =>
            {
                if (App.CurrentApp.ApplicationNotifications.Count > 0)
                    RemoveAtNotification((OPLNotification)sender);
            };

            StackPanelNotifications.Children.Add(ViewNotification);
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ViewNotification, MarginProperty,
                new Thickness(4d), TimeSpan.FromMilliseconds(600d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ViewNotification, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(600d));
        }

        /// <summary>
        /// Удалить элемент уведомления
        /// </summary>
        /// <param name="Index">Индекс удаляемого уведомления</param>
        private void RemoveAtNotification(OPLNotification Element)
        {
            int Index = StackPanelNotifications.Children.IndexOf(Element);
            App.CurrentApp.RemoveNotification(Element.CurrentNotification);
            Element.Height = Element.ActualHeight;
            if (ManagerAnimation != null)
            {
                DoubleAnimation Animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                Animation.From = Element.ActualHeight;
                Animation.To = 0d;
                Animation.FillBehavior = FillBehavior.Stop;
                Animation.Completed += (sender, e) =>
                    StackPanelNotifications.Children.Remove(Element);
                Element.BeginAnimation(HeightProperty, Animation);
            } else StackPanelNotifications.Children.Remove(Element);
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Element, MarginProperty,
                new Thickness(0d), TimeSpan.FromMilliseconds(500d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Element, OpacityProperty,
                0d, TimeSpan.FromMilliseconds(400d));
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
    }
}
