using IEL.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AAC20.Classes
{
    /// <summary>
    /// Класс конвертации объектов текста в содержимое элементов AAC20
    /// </summary>
    /// <remarks>
    /// <b>Синтаксис текста представляет собой вложенный вид "Начальный, конечный символ"</b>
    /// <code>\{X}([^{X}\*]|\*(\*|;|~|"|\$))+</code>
    /// Работает по принципу поиска регулярными выражениями
    /// </remarks>
    internal sealed class AACConverter
    {
        /// <summary>
        /// Создать регулярное выражение шаблона
        /// </summary>
        /// <param name="Start">Начальный символ</param>
        /// <param name="End">Конечный символ</param>
        /// <returns>Регулярное выражение шаблона</returns>
        private static Regex RegexingStartEndSumbol(char Start, char End) => new(@$"\{Start}([^{End}\*]|\*(\*|;|~|""|\$))+\{End}");

        /// <summary>
        /// Перевести текст в читаемый вид
        /// </summary>
        /// <returns></returns>
        private static string ReplacingText(string Text)
        {
            Text = Text.Replace("**", "*");
            Text = Text.Replace("*;", ";");
            Text = Text.Replace("**~", "~");
            Text = Text.Replace("*\"", "\"");
            Text = Text.Replace("*$", "$");
            return Text;
        }

        /// <summary>
        /// Конвертировать объект текста в массив объектов ярлыка
        /// </summary>
        /// <param name="Text">Текст Предоставляемый для обработки</param>
        /// <remarks>
        /// Пример синтаксиса одного элемента ярлыка: <c>$Name;Command$"Text"~</c>
        /// </remarks>
        /// <returns>Конвертированный массив объектов ярлыка</returns>
        public static LabelAction[] ConvertRegexToMassLabelAction(string Text)
        {
            MatchCollection LabelSQLTextElements = RegexingStartEndSumbol('$', '~').Matches(Text);
            List<LabelAction> labels = [];
            foreach (Match match in LabelSQLTextElements) labels.Add(ConvertRegexToLabelAction(match.Value));
            return [.. labels];
        }

        /// <summary>
        /// Конвертировать объект текста в объект ярлыка
        /// </summary>
        /// <param name="Text">Текст Предоставляемый для обработки</param>
        /// <remarks>
        /// Пример синтаксиса: <c>$Name;Command$"Text"~</c>
        /// </remarks>
        /// <returns>Конвертированный текст в объект ярлыка</returns>
        public static LabelAction ConvertRegexToLabelAction(string Text)
        {
            string Name, Command;
            Name = ReplacingText(RegexingStartEndSumbol('$', ';').Match(Text).Value[1..^1]);
            Command = ReplacingText(RegexingStartEndSumbol(';', '$').Match(Text).Value[1..^1]);
            Match DescriptionMatch = RegexingStartEndSumbol('"', '"').Match(Text);
            if (DescriptionMatch.Value.Length == 0) return new(Name, string.Empty, Command);
            return new(Name, ReplacingText(DescriptionMatch.Value[1..^1]), Command);
        }
    }
}
