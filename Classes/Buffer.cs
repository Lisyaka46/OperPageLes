using AAC20.Classes.Commands;
using AAC20.GUI;
using System.Windows;
using System.Windows.Controls;
using static AAC20.Classes.Buffer;

namespace AAC20.Classes
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
        private IELButtonCommand?[] BufferElements = new IELButtonCommand[Math.Clamp(CountBuffer, 4, 80)];

        /// <summary>
        /// Количество добавленных команд
        /// </summary>
        public int Count { get; private set; } = 0;

        /// <summary>
        /// Общее количество мест в буфере
        /// </summary>
        public int Length => BufferElements.Length;

        /// <summary>
        /// Счётчик прокрутки мыши буфера
        /// </summary>
        public CounterScrollBar CounterBuffer = new(0, 4);

        /// <summary>
        /// Класс буферной команды
        /// </summary>
        /// <typeparam name="T">Тип команды для сохранения</typeparam>
        /// <param name="Command">Ссылка на команду</param>
        /// <param name="Name">Имя команды</param>
        /// <param name="Parameters">Параметры команды</param>
        public class BufferCommand<T>(ref T? Command, string Name, string[] Parameters, string StringCommand) where T : ICommandAAC
        {
            /// <summary>
            /// Ссылка на команду
            /// </summary>
            public T? RefCommand = Command;

            /// <summary>
            /// Пропись сохранённой команды
            /// </summary>
            public readonly string TextCommand = StringCommand;

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
            public CommandStateResult ExecuteCommand() => RefCommand?.ExecuteCommand(Parameters) ?? CommandStateResult.FaledCommand(Name);
        }

        /// <summary>
        /// Индексатор буфера элементов
        /// </summary>
        /// <param name="key">Индекс читаемого элемента</param>
        /// <returns>Прочитанный текст элемента</returns>
        /// <exception cref="IndexOutOfRangeException">Исключение выхода индекса за границы буфера</exception>
        public IELButtonCommand this[Index key]
        {
            get
            {
                if (key.Value < Length) return BufferElements[key] ?? throw new Exception("Объект по индексу является нулевым. Данный тип не допускает пустых значений");
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
        /// <param name="ChildrenElements">Сетка элементов сохранённых команд буфера</param>
        public void DeleteAll(Grid? ChildrenElements)
        {
            if (Count > 0)
            {
                ChildrenElements?.Children.Clear();
                BufferElements = new IELButtonCommand[BufferElements.Length];
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
        /// <param name="StringCommand">Пропись команды</param>
        public IELButtonCommand Add(ICommandAAC? Command, string Name, string[] Parameteres, string StringCommand)
        {
            IELButtonCommand BCom = new(new BufferCommand<ICommandAAC>(ref Command, Name, Parameteres, StringCommand), Count);
            if (Count < BufferElements.Length - 1)
            {
                this[Count++] = BCom;
                CounterBuffer.MaxUp(1);
            }
            else
            {
                BufferElements = [.. BufferElements.Skip(1)];
                this[^1] = BCom;
            }
            return BCom;
        }
    }
}
