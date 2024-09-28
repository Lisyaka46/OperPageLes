using System.Numerics;

namespace AAC20.Classes
{
    /// <summary>
    /// Класс объекта счётчика скролл-бара
    /// </summary>
    public class CounterScrollBar
    {
        /// <summary>
        /// Делегат события изменения значения счётчика
        /// </summary>
        /// <param name="Value">Присвоенное значение счётчику</param>
        public delegate void EventChangedValue(int Value);

        /// <summary>
        /// Объект события изменения значения счётчика
        /// </summary>
        public event EventChangedValue? ChangedValue;

        private int _Value;
        /// <summary>
        /// Текущее значение счётчика
        /// </summary>
        public int Value
        {
            get => _Value;
            set
            {
                if (value == _Value) return;
                else if (value > MaxValue) throw new ArgumentOutOfRangeException(nameof(value), "Аргумент при присвоении имеет число выше чем максимальный коэффициент");
                else
                {
                    _Value = value;
                    ChangedValue?.Invoke(_Value);
                }
            }
        }

        /// <summary>
        /// Максимальное значение счётчика
        /// </summary>
        public double MaxValue { get; private set; }

        /// <summary>
        /// Количество видимых элементов при старте
        /// </summary>
        public readonly int CountVisibleElements;

        /// <summary>
        /// Доля видимости объектов на позиции
        /// </summary>
        public readonly int TrafficShare;

        /// <summary>
        /// Минимальный порог максимального значения счётчика
        /// </summary>
        private readonly double Min_MaxValue;

        /// <summary>
        /// Инициализировать объект счётчика скролл-бара
        /// </summary>
        /// <param name="CountVisible">Количество видимых элементов при старте перед скроллом</param>
        /// <param name="TrafficShare">Доля видимости объектов на позиции скролла</param>
        public CounterScrollBar(int CountVisible, ushort TrafficShare = 1)
        {
            this.TrafficShare = TrafficShare;
            CountVisibleElements = CountVisible;
            MaxValue = -CountVisible / TrafficShare;
            Min_MaxValue = MaxValue;
            _Value = 0;
        }

        /// <summary>
        /// Изменить счётчик скролл-бара вверх (--)
        /// </summary>
        /// <returns>Итоговое число движения</returns>
        public int Up()
        {
            if (_Value > 0) ChangedValue?.Invoke(--_Value);
            return _Value;
        }

        /// <summary>
        /// Изменить счётчик скролл-бара вниз (++)
        /// </summary>
        /// <returns>Итоговое число движения</returns>
        public int Down()
        {
            if (MaxValue > 0d)
            {
                if ((double)_Value < MaxValue) ChangedValue?.Invoke(++_Value);
                return _Value;
            }
            else throw new ArgumentOutOfRangeException(nameof(Value), "Значение невозможно увеличить так как MaxValue < 0. " +
                $"(Value={Value} MaxValue={Value} MaxValue_%_TrafficShare={MaxValue % TrafficShare})");
        }

        /// <summary>
        /// Функция увеличения максимального значения
        /// </summary>
        /// <param name="value">Значение на сколько увеличивается максимальное значение</param>
        /// <returns>Увеличеное максимальное значение</returns>
        public double MaxUp(int value) => MaxValue += (double)value / TrafficShare;

        /// <summary>
        /// Функция уменьшения максимального значения
        /// </summary>
        /// <param name="value">Значение на сколько увеличивается максимальное значение</param>
        /// <returns>Уменьшенное максимальное значение</returns>
        public double MaxDown(int value)
        {
            if (MaxValue - (value / TrafficShare) >= Min_MaxValue)
            {
                MaxValue -= (double)value / TrafficShare;
                if (Value > 0 && Value > MaxValue) Value = (int)MaxValue;
                return MaxValue;
            }
            else throw new ArgumentOutOfRangeException(nameof(value), $"({nameof(MaxValue)} - {nameof(value)} < {Min_MaxValue}) невозможно уменьшить максимальное значение ({MaxValue - value} < {Min_MaxValue})");
        }

        /// <summary>
        /// Функция очистки максимального значения к нулевому
        /// </summary>
        /// <remarks>
        /// Очищается максимальное значение и значение счётчика
        /// </remarks>
        public void MaxClear()
        {
            Value = 0;
            MaxValue = Min_MaxValue;
        }
    }
}
