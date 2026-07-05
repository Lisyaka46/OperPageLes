using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE.Enums
{
    /// <summary>
    /// Перечисление общих параметров настроек
    /// </summary>
    internal enum EnumGeneralSettings : uint
    {
        /// <summary>
        /// Отображать отклик интернета в миллисекундах
        /// </summary>
        VisualMillisecondConnect = 0u,

        /// <summary>
        /// Длинна буфера хранения вводимых команд
        /// </summary>
        BufferLength = 1u,
    }
}
