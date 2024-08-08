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
        /// Минимальный порог максимального значения счётчика
        /// </summary>
        private readonly int Min_MaxValue;

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
            MaxValue = Max / TrafficShare - CountVisibleElements / TrafficShare;
            Min_MaxValue = MaxValue;
            _Value = value;
        }

        /// <summary>
        /// Изменить счётчик скролл-бара вверх (--)
        /// </summary>
        /// <returns>Итоговое число движения</returns>
        public int Up()
        {
            if (_Value == 0) return 0;
            _Value = _Value > 0 ? _Value - 1 : 0;
            ChangedValue?.Invoke(_Value);
            return _Value;
        }

        /// <summary>
        /// Изменить счётчик скролл-бара вниз (++)
        /// </summary>
        /// <returns>Итоговое число движения</returns>
        public int Down()
        {
            if (MaxValue > 0)
            {
                if (_Value == MaxValue) return MaxValue;
                _Value = _Value < MaxValue ? _Value + 1 : MaxValue;
                ChangedValue?.Invoke(_Value);
                return _Value;
            }
            else throw new ArgumentOutOfRangeException(nameof(Value), $"Значение невозможно увеличить так как MaxValue < 0. (Value={Value} MaxValue={Value})");
        }

        /// <summary>
        /// Функция увеличения максимального значения
        /// </summary>
        /// <param name="value">Значение на сколько увеличивается максимальное значение</param>
        /// <returns>Увеличеное максимальное значение</returns>
        public int MaxUp(int value) => MaxValue += value;

        /// <summary>
        /// Функция уменьшения максимального значения
        /// </summary>
        /// <param name="value">Значение на сколько увеличивается максимальное значение</param>
        /// <returns>Уменьшенное максимальное значение</returns>
        public int MaxDown(int value)
        {
            if (MaxValue - value >= Min_MaxValue)
            {
                MaxValue -= value;
                if (Value > 0 && Value > MaxValue) Value = MaxValue;
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
