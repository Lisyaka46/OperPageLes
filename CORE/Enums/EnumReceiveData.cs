using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationOperPageLes.CORE.Enums
{
    public enum EnumReceiveData : byte
    {
        /// <summary>
        /// Файл
        /// </summary>
        OnlyFile = 0xF0,

        /// <summary>
        /// Только сообщение
        /// </summary>
        OnlyMessage = 0xFF,

        /// <summary>
        /// Сообщение и больше 1 файла
        /// </summary>
        MessageAndFiles = 0xAA,
    }
}
