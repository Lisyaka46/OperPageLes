using static OperPage_les.CORE.Settings.ISetting;

namespace OperPage_les.CORE.Settings
{
    internal class ObjSetting<T>(T DefaultValue)
    {
        /// <summary>
        /// Событие изменения значения объекта настроек
        /// </summary>
        internal event ChangeValue<T>? Changed;

        private T _Value = DefaultValue;
        /// <summary>
        /// Значение параметра настроек
        /// </summary>
        public T Value
        {
            get => _Value;
            set
            {
                Changed?.Invoke(_Value, value);
                _Value = value;
            }
        }

        public static implicit operator ObjSetting<T>(T value) => new(value);
        public static implicit operator T(ObjSetting<T> obj) => obj._Value;
    }
}
