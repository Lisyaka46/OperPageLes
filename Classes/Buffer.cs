using AAC20.Classes.Commands;
using AAC20.GUI;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using static AAC20.Classes.Buffer;

namespace AAC20.Classes
{
    // Сделать объект визуализирующий буфер !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
        /// Удалить элемент буфера
        /// </summary>
        /// <param name="ChildrenElements">Сетка элементов сохранённых команд буфера</param>
        /// <param name="DeleteElement">Объект удаляемый из сетки буфера</param>
        public void Delete(Grid ChildrenElements, UIElement DeleteElement)
        {
            int i = ChildrenElements.Children.IndexOf(DeleteElement);
            if (i == -1) throw new IndexOutOfRangeException("Удаляемый элемент из буфера сохранённых команд не найден (-1)");
            else Delete(ChildrenElements, i);
        }

        /// <summary>
        /// Удалить элемент буфера
        /// </summary>
        /// <param name="ChildrenElements">Сетка элементов сохранённых команд буфера</param>
        /// <param name="index">Индекс удаляемого элемента</param>
        public void Delete(Grid ChildrenElements, int index)
        {
            if (Count > 0)
            {
                ref IELButtonCommand? Button = ref BufferElements[index];
                if (Button != null)
                {
                    DoubleAnimation AnimationDeleteElement = new(0, TimeSpan.FromMilliseconds(100d))
                    {
                        EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
                        FillBehavior = FillBehavior.Stop,
                    };
                    AnimationDeleteElement.Completed += (sender, e) => ChildrenElements.Children.RemoveAt(index);
                    Button.BeginAnimation(FrameworkElement.OpacityProperty, AnimationDeleteElement);
                }
                ReSort(index);
                Count--;
                CounterBuffer.MaxDown(1);
            }
        }

        /// <summary>
        /// Пересортировка исключая index
        /// </summary>
        /// <param name="index">Исключающий индекс элемента</param>
        /// <param name="AnimateAction">Анимировать сортировку или нет</param>
        private void ReSort(int index, bool AnimateAction = true)
        {
            ref IELButtonCommand? Button = ref BufferElements[index];
            if (Count > 1 && index < Count - 1)
            {
                ThicknessAnimation AnimationBuffer = new(new Thickness(0), TimeSpan.FromMilliseconds(160d))
                {
                    EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut, Amplitude = 0.6d }
                };
                Thickness ThicknessIndex = new(0);
                for (int i = index; i < Count - 1; i++)
                {
                    if (i != Count - 1) BufferElements[i] = BufferElements[i + 1];
                    Button = ref BufferElements[i];
                    if (Button == null) continue;
                    else
                    {
                        ThicknessIndex = new Thickness(0, 29 * i + 4, 0, 0);
                        Button.TextBlockNumberCommand.Text = $"#{i + 1}";
                        if (!AnimateAction) Button.Margin = ThicknessIndex;
                        else
                        {
                            AnimationBuffer.To = ThicknessIndex;
                            AnimationBuffer.BeginTime = TimeSpan.FromMilliseconds((i - index) * 20d);
                            Button.BeginAnimation(FrameworkElement.MarginProperty, AnimationBuffer);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Удалить <b>все</b> элементы буфера
        /// </summary>
        /// <param name="ChildrenElements">Сетка элементов сохранённых команд буфера</param>
        public void DeleteAll(Grid ChildrenElements)
        {
            if (Count > 0)
            {
                ChildrenElements.Children.Clear();
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
        /// <param name="ChildrenElements">Сетка элементов буфера</param>
        public IELButtonCommand Add(ICommandAAC? Command, ref Grid ChildrenElements, string Name, string[] Parameteres, string StringCommand)
        {
            IELButtonCommand BCom = new(new BufferCommand<ICommandAAC>(ref Command, Name, Parameteres, StringCommand), Count - (Count < BufferElements.Length ? 0 : 1));
            if (Count < BufferElements.Length)
            {
                this[Count++] = BCom;
                CounterBuffer.MaxUp(1);
            }
            else
            {
                ChildrenElements.Children.RemoveAt(0);
                ReSort(0, false);
                this[^1] = BCom;
            }
            return BCom;
        }
    }
}
