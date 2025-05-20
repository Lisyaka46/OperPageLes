using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperPage_les.CORE.Settings
{
    internal interface ISetting
    {
        /// <summary>
        /// Делегат события изменения параметра настроек
        /// </summary>
        /// <param name="NewValue">Старое значение параметра</param>
        /// <param name="NewValue">Новое значение параметра</param>
        public delegate void ChangeValue<T>(T OldValue, T NewValue);
    }
}
