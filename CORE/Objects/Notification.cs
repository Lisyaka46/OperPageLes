using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Enums.Language;
using OPLAPI.CORE.Language;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace OperPageLes.CORE.Objects
{
    public class Notification : INotifyPropertyChanged
    {
        /// <summary>
        /// Иконка уведомления
        /// </summary>
        internal ImageSource? Icon { get; set; } = null;

        private string _Title;
        /// <summary>
        /// Краткий заголовок
        /// </summary>
        public string Title
        {
            get => _Title;
            private set
            {
                _Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        /// <summary>
        /// Сообщение уведомления
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Вид уведомления
        /// </summary>
        public readonly EnumNotificationStyle Style;

        #region PropertyChanged
        /// <summary>
        /// Событие изменения свойства параметра
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Запустить событие изменения свойства объекта
        /// </summary>
        /// <param name="Name">Имя изменяемого свойства</param>
        protected void OnPropertyChanged([CallerMemberName] string? Name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        #endregion

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
            _Title = GetTitleFromStyle(SourceStyle);
            Message = SourceMessage;
            Lang.LanguageUpdated += Lang_LanguageUpdated;
        }

        /// <summary>
        /// Узнать программный заголовок для уведомления
        /// </summary>
        /// <param name="Style">Вид уведомления</param>
        /// <returns>Строка заголовка для уведомления</returns>
        private static string GetTitleFromStyle(EnumNotificationStyle Style) =>
            Style switch
            {
                EnumNotificationStyle.System => Lang.GetValue(LangUITranslate.SystemNotification),
                EnumNotificationStyle.Warning => Lang.GetValue(LangUITranslate.WarningNotification),
                _ => Lang.GetValue(LangUITranslate.OtherNotification)
            };

        /// <summary>
        /// Обработчик события изменения языкового перевода
        /// </summary>
        private void Lang_LanguageUpdated(object? sender, EventArgs e)
        {
            Title = GetTitleFromStyle(Style);
        }
    }
}       
