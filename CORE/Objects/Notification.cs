using OperPageLes.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace OperPageLes.CORE.Objects
{
    public class Notification
    {
        /// <summary>
        /// Иконка уведомления
        /// </summary>
        internal ImageSource? Icon { get; set; } = null;

        /// <summary>
        /// Краткий заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Сообщение уведомления
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Вид уведомления
        /// </summary>
        public readonly EnumNotificationStyle Style;

        /// <summary>
        /// Создать уведомление с системным заголовком
        /// </summary>
        /// <param name="SourceMessage">Сообщение уведомления</param>
        /// <param name="SourceStyle">Вид уведомления</param>
        /// <param name="SourceIcon">Иконка уведомления</param>
        internal Notification(string SourceMessage, EnumNotificationStyle SourceStyle, in ImageSource? SourceIcon = null)
        {
            Icon = SourceIcon;
            Style = SourceStyle;
            Title = GetTitleFromStyle(SourceStyle);
            Message = SourceMessage;
        }

        /// <summary>
        /// Создать уведомление с собственным заголовком
        /// </summary>
        /// <param name="SourceMessage">Сообщение уведомления</param>
        /// <param name="SourceTitle">Заголовок уведомления</param>
        /// <param name="SourceIcon">Иконка уведомления</param>
        /// <param name="SourceStyle">Вид уведомления</param>
        internal Notification(string SourceMessage, string SourceTitle, EnumNotificationStyle SourceStyle, in ImageSource? SourceIcon = null)
        {
            Icon = SourceIcon;
            Style = SourceStyle;
            Title = SourceTitle;
            Message = SourceMessage;
        }

        /// <summary>
        /// Узнать программный заголовок для уведомления
        /// </summary>
        /// <param name="Style">Вид уведомления</param>
        /// <returns>Строка заголовка для уведомления</returns>
        private static string GetTitleFromStyle(EnumNotificationStyle Style) =>
            Style switch
            {
                EnumNotificationStyle.System => "Системное уведомление",
                _ => "Иное уведомление"
            };
    }
}
