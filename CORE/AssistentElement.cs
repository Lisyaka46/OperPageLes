using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.CORE
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
        /// Структура пользователя
        /// </summary>
        internal class AssistentElement(string nickName, string message,
                Color? colorNickName = null, string? NameImageAssistent = null)
        {
            /// <summary>
            /// Ник помошника
            /// </summary>
            public readonly string NickName = nickName;

            /// <summary>
            /// Отображаемое сообщение благодарности
            /// </summary>
            public readonly string Message = message;

            /// <summary>
            /// Цвет текста никнейма
            /// </summary>
            public readonly Color ColorNickName = colorNickName ?? DefaultColorNick;

            /// <summary>
            /// Карта изоражения
            /// </summary>
            internal readonly string? NameImageSource = NameImageAssistent;
        }

        /// <summary>
        /// Все пользователи учавствующие в разработке
        /// </summary>
        internal static readonly ReadOnlyCollection<AssistentElement> AllAssistents = new(
        [
            new("Lisyaka",
                "- За всю разработку.",
                NameImageAssistent: nameof(OPRES.IconMainApplication)),
            new("Minsi",
                "- За помощь в разработке." +
                "\n- За проектирование программы." +
                "\n- За оценку качества программы.",
                Color.FromRgb(86, 255, 120),
                nameof(OPRES.MINSI)),
            new("Vector",
                "- За работу в дизайне." +
                "\n- За проектирование стиля." +
                "\n- За планировку вида.",
                Color.FromRgb(62, 180, 137),
                nameof(OPRES.VECTOR)),
        ]);
    }
}
