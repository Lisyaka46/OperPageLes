using System.Windows.Documents;
using System.Windows.Media;

namespace AAC20.Classes.Commands
{
    /// <summary>
    /// Конечные результаты выполнения команды
    /// </summary>
    public enum ResultState
    {
        /// <summary>
        /// Команда не выполнилась
        /// </summary>
        Failed = 0,

        /// <summary>
        /// Команда выполнилась успешно
        /// </summary>
        Complete = 1
    }

    /// <summary>
    /// Объект итогового состояния выполнения команды
    /// </summary>
    public class CommandStateResult
    {
        /// <summary>
        /// Итоговое состояние команды
        /// </summary>
        public readonly ResultState State;

        /// <summary>
        /// Сообщение в LOG
        /// </summary>
        public readonly string LOGMassage;

        /// <summary>
        /// Сообщение в консольную строку
        /// </summary>
        public readonly Paragraph? Massage;

        /// <summary>
        /// Успешный итог выполнения команды
        /// </summary>
        public static CommandStateResult Completed => new(ResultState.Complete, null, string.Empty);

        /// <summary>
        /// Ошибочный итог выполнения команды
        /// </summary>
        /// /// <param name="Message">Текст ошибки выводимый в консоль программу</param>
        /// <param name="Message_log">Текст который будет записан в лог программы</param>
        public static CommandStateResult Failed(string Message, string Message_log) => new(ResultState.Failed, new Paragraph(new Run(Message)), Message_log);

        /// <summary>
        /// Ошибочный итог выполнения команды с пользовательским объектом текста ошибки
        /// </summary>
        /// <param name="Message">Текст ошибки выводимый в консоль программу</param>
        /// <param name="Message_log">Текст который будет записан в лог программы</param>
        public static CommandStateResult Failed(Paragraph Message, string Message_log) => new(ResultState.Failed, Message, Message_log);

        /// <summary>
        /// Ошибочный итог выполнения команды из-за недостатка параметров
        /// </summary>
        /// <param name="NameCommand">Имя команды которая привела к ошибке</param>
        public static CommandStateResult FaledParameteres(string NameCommand)
        {
            Paragraph Massage = new();
            Massage.Inlines.Add(new Bold(new Run(">>> ")));
            Massage.Inlines.Add(new Run("The "));
            Massage.Inlines.Add(new Italic(new Run($"\"{NameCommand}\" ")) { Background = new SolidColorBrush(Colors.IndianRed) });
            Massage.Inlines.Add(new Run("command resulted in an error due to a lack of parameters to execute."));
            return new(ResultState.Failed, Massage, $"Команда \"{NameCommand}\" привела к ошибке из-за недостатка параметров");
        }

        /// <summary>
        /// Инициализировать объект итога выполнения команды
        /// </summary>
        /// <param name="ResultState">Итоговое состояние выполнения</param>
        /// <param name="Massage">Сообщение в консольную строку</param>
        /// <param name="Massage_log">Сообщение в LOG</param>
        private CommandStateResult(ResultState ResultState, Paragraph? Massage, string Massage_log)
        {
            State = ResultState;
            this.Massage = Massage;
            LOGMassage = Massage_log;
        }
    }
}
