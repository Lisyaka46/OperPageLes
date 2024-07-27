namespace AAC20.Classes
{
    /// <summary>
    /// Класс объекта счётчика скролл-бара
    /// </summary>
    public class CounterScrollBar
    {
        /// <summary>
        /// Текущее значение счётчика
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Максимальное значение счётчика
        /// </summary>
        public int MaxValue { get; private set; }

        /// <summary>
        /// Количество видимых элементов при старте
        /// </summary>
        public readonly int CountVisibleElements;

        /// <summary>
        /// Доля движения по одному объекту
        /// </summary>
        public readonly int TrafficShare;

        /// <summary>
        /// Инициализировать объект счётчика скролл-бара
        /// </summary>
        /// <param name="Max">Максимальное значение счётчика</param>
        /// <param name="countVisible">Количество видимых элементов при старте</param>
        /// <param name="value">Начальное значение счётчика</param>
        /// <param name="TrafficShare">Доля движения одного скрола по объекту</param>
        public CounterScrollBar(int Max, int countVisible, int value = 0, ushort TrafficShare = 1)
        {
            this.TrafficShare = TrafficShare;
            CountVisibleElements = countVisible * TrafficShare;
            MaxValue = Math.Clamp(Max * TrafficShare - CountVisibleElements, 0, Max);
            Value = value;
        }

        /// <summary>
        /// Изменить счётчик скролл-бара вверх
        /// </summary>
        /// <returns>Итоговое число движения</returns>
        public int Up() => Value > 0 ? --Value : 0;

        /// <summary>
        /// Изменить счётчик скролл-бара вниз
        /// </summary>
        /// <returns>Итоговое число движения</returns>
        public int Down()
        {
            if (MaxValue > 0) return Value < MaxValue ? ++Value : MaxValue;
            else throw new ArgumentOutOfRangeException(nameof(Value), $"Значение невозможно увеличить так как MaxValue < 0. (Value={Value} MaxValue={Value})");
        }

        /// <summary>
        /// Функция увеличения максимального значения
        /// </summary>
        /// <param name="value">Значение на чколько увеличивается максимальное значение</param>
        /// <returns>Увеличеное максимальное значение</returns>
        public int MaxUp(int value) => MaxValue += value;

        /// <summary>
        /// Функция уменьшения максимального значения
        /// </summary>
        /// <param name="value">Значение на чколько увеличивается максимальное значение</param>
        /// <returns>Уменьшенное максимальное значение</returns>
        public int MaxDown(int value)
        {
            if (MaxValue - value >= 0)
            {
                MaxValue -= value;
                if (Value > MaxValue) Value = MaxValue;
                return MaxValue;
            }
            else throw new ArgumentOutOfRangeException(nameof(value), $"({nameof(MaxValue)} - {nameof(value)} < 0) невозможно уменьшить максимальное значение ({MaxValue - value} < 0)");
        }
    }
}
