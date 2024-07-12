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
        public readonly string Massage;

        /// <summary>
        /// Успешный итог выполнения команды
        /// </summary>
        public static CommandStateResult Completed => new(ResultState.Complete, string.Empty, string.Empty);

        /// <summary>
        /// Ошибочный итог выполнения команды из-за недостатка параметров
        /// </summary>
        /// <param name="NameCommand">Имя команды которая привела к ошибке</param>
        public static CommandStateResult FaledParameteres(string NameCommand) =>
            new(ResultState.Failed,
                $"The \"{NameCommand}\" command resulted in an error due to a lack of parameters to execute",
                $"Команда \"{NameCommand}\" привела к ошибке из-за недостатка параметров");

        /// <summary>
        /// Инициализировать объект итога выполнения команды
        /// </summary>
        /// <param name="ResultState">Итоговое состояние выполнения</param>
        /// <param name="Massage">Сообщение в консольную строку</param>
        /// <param name="Massage_log">Сообщение в LOG</param>
        internal CommandStateResult(ResultState ResultState, string Massage, string Massage_log)
        {
            State = ResultState;
            this.Massage = Massage;
            LOGMassage = Massage_log;
        }
    }
}
