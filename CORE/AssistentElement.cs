using System.Collections.ObjectModel;
using System.Windows.Media;
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
        internal class AssistentElement(string nickName, string phrase, string message,
                Color? colorNickName = null, Color? colorPhrase = null, byte[]? DataImageAssistent = null)
        {
            /// <summary>
            /// Ник помошника
            /// </summary>
            public readonly string NickName = nickName;

            /// <summary>
            /// Отображаемая фраза пользователя
            /// </summary>
            public readonly string Phrase = phrase;

            /// <summary>
            /// Отображаемое сообщение благодарности
            /// </summary>
            public readonly string Message = message;

            /// <summary>
            /// Цвет текста никнейма
            /// </summary>
            public readonly Color ColorNickName = colorNickName ?? DefaultColorNick;

            /// <summary>
            /// Цвет текста фразы
            /// </summary>
            public readonly Color ColorPhrase = colorPhrase ?? DefaultColorPhrase;

            /// <summary>
            /// Карта изоражения
            /// </summary>
            internal readonly byte[]? ImageSource = DataImageAssistent;
        }

        /// <summary>
        /// Все пользователи учавствующие в разработке
        /// </summary>
        internal static readonly ReadOnlyCollection<AssistentElement> AllAssistents = new(
        [
            new("Lisyaka", "Не знаю...",
                "- За всю разработку.",
                DataImageAssistent: null),
            new("Minsi", "Спасибо что живая.",
                "- За помощь в разработке." +
                "\n- За проектирование программы." +
                "\n- За оценку качества программы.",
                Color.FromRgb(86, 255, 120), Color.FromRgb(195, 189, 222),
                DataImageAssistent: Properties.Resources.MINSI),
            new("Vector", "Разработчик это художник, а дизайнер это кисть.",
                "- За работу в дизайне." +
                "\n- За проектирование стиля." +
                "\n- За планировку вида.",
                Color.FromRgb(62, 180, 137), Color.FromRgb(80, 200, 120),
                DataImageAssistent: Properties.Resources.VECTOR),
        ]);
    }
}
