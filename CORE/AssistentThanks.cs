using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace OperPage_les.CORE
{
    internal struct AssistentThanks(string nickname, string phrase, string message)
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
        /// Ник помошника
        /// </summary>
        public string NickName { get; } = nickname;

        /// <summary>
        /// Отображаемая фраза
        /// </summary>
        public string Phrase { get; } = phrase;

        /// <summary>
        /// Отображаемое сообщение благодарности
        /// </summary>
        public string Message { get; } = message;

        /// <summary>
        /// Цвет текста никнейма
        /// </summary>
        public Color ColorNickName { get; internal set; } = DefaultColorNick;

        /// <summary>
        /// Цвет текста фразы
        /// </summary>
        public Color ColorPhrase { get; internal set; } = DefaultColorPhrase;

        /// <summary>
        /// Ссылка на изображение
        /// </summary>
        public Uri? UriImage { get; internal set; } = null;

        /// <summary>
        /// Картинка на файловое изоражение
        /// </summary>
        public BitmapImage? MapImage { get; internal set; } = null;
    }
}
