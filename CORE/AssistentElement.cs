using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace OperPage_les.CORE
{
    /// <summary>
    /// Класс управления пользователями учавствовавших в разработке
    /// </summary>
    public static class Assistents
    {
        /// <summary>
        /// Цвет текста ника по умолчанию
        /// </summary>
        private static readonly Color DefaultColorNick = Color.FromRgb(245, 225, 101);

        /// <summary>
        /// Цвет текста фразы
        /// </summary>
        private static readonly Color DefaultColorPhrase = Color.FromRgb(219, 177, 205);

        /// <summary>
        /// Структура пользователя
        /// </summary>
        /// <param name="Nickname">Наименование</param>
        /// <param name="Phrase">Фраза пользователя</param>
        /// <param name="Message">Сообщение пользователя</param>
        public readonly struct AssistentElement
        {
            /// <summary>
            /// Ник помошника
            /// </summary>
            public readonly string NickName;

            /// <summary>
            /// Отображаемая фраза пользователя
            /// </summary>
            public readonly string Phrase;

            /// <summary>
            /// Отображаемое сообщение благодарности
            /// </summary>
            public readonly string Message;

            /// <summary>
            /// Цвет текста никнейма
            /// </summary>
            public readonly Color ColorNickName;

            /// <summary>
            /// Цвет текста фразы
            /// </summary>
            public readonly Color ColorPhrase;

            /// <summary>
            /// Ссылка на изображение
            /// </summary>
            public readonly Uri? UriImage;

            /// <summary>
            /// Картинка на файловое изоражение
            /// </summary>
            public readonly BitmapImage? MapImage;

            internal AssistentElement(string NickName, string Phrase, string Message,
                Color? ColorNickName = null, Color? ColorPhrase = null, Uri? UriImage = null, BitmapImage? MapImage = null)
            {
                this.NickName = NickName;
                this.Phrase = Phrase;
                this.Message = Message;
                this.ColorNickName = ColorNickName ?? DefaultColorNick;
                this.ColorPhrase = ColorPhrase ?? DefaultColorPhrase;
                this.UriImage = UriImage;
                this.MapImage = MapImage;
            }
        }

        public static readonly AssistentElement[] AllAssistents =
        [
            new("Lisyaka", "Не знаю...",
                "- За всю разработку.",
                UriImage: new Uri("https://sun9-46.userapi.com/impg/euj8JteQPLq-XpWDbR03hU2Dlz3IhzwLs4W9DA/bYNM9VcaP-w.jpg?size=800x800&quality=95&sign=b761945cee478f88087602b209cff6f9&type=album"),
                MapImage: null),
            new("Minsi", "Спасибо что живая.",
                "- За помощь в разработке." +
                "\n- За проектирование программы." +
                "\n- За оценку качества программы.",
                Color.FromRgb(86, 255, 120), Color.FromRgb(195, 189, 222),
                MapImage: App.LoadImage(Properties.Resources.MINSI)),
            new("Vector", "Разработчик это художник, а дизайнер это кисть.",
                "- За работу в дизайне." +
                "\n- За проектирование стиля." +
                "\n- За планировку вида.",
                Color.FromRgb(62, 180, 137), Color.FromRgb(80, 200, 120),
                MapImage: App.LoadImage(Properties.Resources.VECTOR)),
        ];
    }
}
