using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationOperPageLes.CORE.Enums
{
    internal enum EnumTypeConnection : byte
    {
        /// <summary>
        /// Порт подключения передачи сообщений
        /// </summary>
        PortMessage = 0,

        /// <summary>
        /// Порт подключения передачи файлов
        /// </summary>
        PortFile = 1,

        /// <summary>
        /// Порт подключения передачи программных данных
        /// </summary>
        PortProgramm = 2,
    }
}
