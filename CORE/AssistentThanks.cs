using Color = System.Windows.Media.Color;

namespace OperPage_les.CORE
{
    internal class AssistentThanks(string nickname, string phrase, string message, Uri? link = null, Uri? pathbitmap = null)
    {
        /// <summary>
        /// Ник помошника
        /// </summary>
        public string NickName { get; } = nickname;

        /// <summary>
        /// Отображаемая фраза
        /// </summary>
        public readonly string Phrase = phrase;

        /// <summary>
        /// Отображаемое сообщение благодарности
        /// </summary>
        public readonly string Message = message;

        /// <summary>
        /// Цвет текста никнейма
        /// </summary>
        public Color ColorNickName { get; internal set; }

        /// <summary>
        /// Цвет текста фразы
        /// </summary>
        public Color ColorPhrase { get; internal set; }

        /// <summary>
        /// Ссылка на изображение
        /// </summary>
        public Uri? LinkImage { get; } = link;

        /// <summary>
        /// Картинка на файловое изоражение
        /// </summary>
        public Uri? PathImage { get; } = pathbitmap;
    }
}
