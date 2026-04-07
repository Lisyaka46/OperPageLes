using OperPageLes.CORE.Objects;
using IEL.UserElementsControl.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OperPageLes.UI.UserElementsControl.Default
{
    /// <summary>
    /// Логика взаимодействия для OPLNotification.xaml
    /// </summary>
    public partial class OPLNotification : IELContainerBase
    {
        /// <summary>
        /// Объект данных уведомления
        /// </summary>
        private Notification CurrentNotification;

        /// <summary>
        /// Создать Объект визуализации уведомления
        /// </summary>
        /// <param name="SourceNotification">Отображаемое уведомление</param>
        public OPLNotification(in Notification SourceNotification)
        {
            InitializeComponent();
            CurrentNotification = SourceNotification;
            BackgroundNotification.ImageSource = CurrentNotification.Icon;
            TextBlockTitle.Text = CurrentNotification.Title;
            TextBlockMessage.Text = CurrentNotification.Message;
        }
    }
}
