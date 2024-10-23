

namespace AAC20.CORE.Flaging
{
    /// <summary>
    /// Перечисление состояний ожидаемого типа флага
    /// </summary>
    public enum WaitTypeFlag
    {
        Wait = -1,
        No = 0,
        Yes = 1,
    }

    /// <summary>
    /// Описание структуры объекта флага
    /// </summary>
    /// <remarks>
    /// Инициализировать объект флага
    /// </remarks>
    /// <param name="Value">Стартовое значение</param>
    public class Flag(bool Value)
    {

        /// <summary>
        /// Делегат события изменения состояния флага
        /// </summary>
        /// <param name="SetBool">Состояние изменённого флага</param>
        public delegate void EventChangeStateFlag(bool SetBool);

        /// <summary>
        /// Событие изменения состояния флага
        /// </summary>
        public event EventChangeStateFlag? ChangeStateFlag;

        /// <summary>
        /// Параметр состояния ожидания
        /// </summary>
        public bool Wait { get; internal set; } = false;

        /// <summary>
        /// Ресурсное значение флага
        /// </summary>
        private bool _Value = Value;

        /// <summary>
        /// Видимое значение флага
        /// </summary>
        public bool Value
        {
            get => _Value;
            set
            {
                if (Wait) throw new Exception("Состояние флага изменить невозможно в состоянии ожидания!");
                if (_Value == value) return;
                _Value = value;
                ChangeStateFlag?.Invoke(_Value);
            }
        }

        /// <summary>
        /// Преобразование флага в его значение
        /// </summary>
        /// <param name="f">Флаг</param>
        public static implicit operator bool(Flag f) => f._Value;
    }
}
