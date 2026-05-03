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
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(ViewNotification);
            ViewNotification.MouseRightButtonUp += (sender, e) =>
            {
                if (App.CurrentApp.ApplicationNotifications.Count > 0)
                    RemoveAtNotification((OPLNotification)sender);
            };

            StackPanelNotifications.Children.Add(ViewNotification);
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(ViewNotification, MarginProperty, new(4d), TimeSpan.FromMilliseconds(600d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(ViewNotification, OpacityProperty, 1d, TimeSpan.FromMilliseconds(600d));
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
            DoubleAnimation Animation = App.ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
            Animation.From = Element.ActualHeight;
            Animation.To = 0d;
            Animation.FillBehavior = FillBehavior.Stop;
            Animation.Completed += (sender, e) =>
                StackPanelNotifications.Children.Remove(Element);
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
    }
}
