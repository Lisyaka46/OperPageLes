using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAC20.Classes
{
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
