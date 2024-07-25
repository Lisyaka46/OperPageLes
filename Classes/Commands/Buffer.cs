using static AAC20.Classes.Commands.Buffer;

namespace AAC20.Classes.Commands
{
    /// <summary>
    /// Буфер консольных команд
    /// </summary>
    /// <remarks>
    /// Инициализировать новый буфер команд
    /// </remarks>
    /// <param name="CountBuffer">Количество сохраняемых команд в буфер</param>
    public class Buffer(int CountBuffer = 50)
    {
        /// <summary>
        /// Массив элементов буфера
        /// </summary>
        private BufferCommand<ICommandAAC>[] BufferElements = new BufferCommand<ICommandAAC>[Math.Clamp(CountBuffer, 4, 80)];

        /// <summary>
        /// Количество добавленных команд
        /// </summary>
        public int Count { get; private set; } = 0;

        /// <summary>
        /// Общее количество мест в буфере
        /// </summary>
        public int Length => BufferElements.Length;

        /// <summary>
        /// Класс буферной команды
        /// </summary>
        /// <typeparam name="T">Тип команды для сохранения</typeparam>
        /// <param name="Command">Ссылка на команду</param>
        /// <param name="Name">Имя команды</param>
        /// <param name="Parameters">Параметры команды</param>
        public class BufferCommand<T>(ref T Command, string Name, string[] Parameters) where T : ICommandAAC
        {
            /// <summary>
            /// Ссылка на команду
            /// </summary>
            public T RefCommand = Command;

            /// <summary>
            /// Имя сохранённой команды
            /// </summary>
            public readonly string Name = Name;

            /// <summary>
            /// Параметры сохранённой команды
            /// </summary>
            private readonly string[] Parameters = Parameters;

            /// <summary>
            /// Создать выполнение сохранённой команды
            /// </summary>
            /// <returns>Итог выполнения команды</returns>
            public CommandStateResult ExecuteCommand() => RefCommand.ExecuteCommand(Parameters);
        }

        /// <summary>
        /// Индексатор буфера элементов
        /// </summary>
        /// <param name="key">Индекс читаемого элемента</param>
        /// <returns>Прочитанный текст элемента</returns>
        /// <exception cref="IndexOutOfRangeException">Исключение выхода индекса за границы буфера</exception>
        public BufferCommand<ICommandAAC> this[Index key]
        {
            get
            {
                if (key.Value < Length) return BufferElements[key];
                else throw new IndexOutOfRangeException($"Индекс ({key}) вышел за рамки буфера ({Length})");
            }
            private set
            {
                if (key.Value < Length) BufferElements[key] = value;
                else throw new IndexOutOfRangeException($"Индекс ({key}) вышел за рамки буфера ({Length})");
            }
        }

        /// <summary>
        /// Удалить <b>все</b> элементы буфера
        /// </summary>
        public void DeleteAll()
        {
            if (Count > 0)
            {
                BufferElements = new BufferCommand<ICommandAAC>[BufferElements.Length];
                Count = 0;
            }
        }

        /// <summary>
        /// Добавить элемент в буфер <b></b>
        /// </summary>
        /// <remarks>
        /// При переполнении самый первый элемент удаляется и добавляется текущий
        /// </remarks>
        /// <param name="Command">Элемент буфера</param>
        /// <param name="Name">Имя команды</param>
        /// <param name="Parameteres">Параметры выполняемой команды</param>
        public void Add(ICommandAAC Command, string Name, string[] Parameteres)
        {
            BufferCommand<ICommandAAC> BCom = new(ref Command, Name, Parameteres);
            if (Count < BufferElements.Length - 1) this[++Count] = BCom;
            else
            {
                BufferElements = [.. BufferElements.Skip(1)];
                this[^1] = BCom;
            }
        }
    }
}
