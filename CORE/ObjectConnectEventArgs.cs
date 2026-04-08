using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE
{
    public readonly struct ObjectConnectEventArgs
    {
        /// <summary>
        /// Состояние текущего подключения
        /// </summary>
        internal readonly bool Connect;

        /// <summary>
        /// Количество миллисекунд потраченное на проверку подключения
        /// </summary>
        internal readonly ushort Ping;

        /// <summary>
        /// Создать структуру параметров
        /// </summary>
        /// <param name="SourceConnect">Текущее состояние подключения</param>
        /// <param name="SourcePing">Количество миллисекунд потраченное на подключение</param>
        private ObjectConnectEventArgs(bool SourceConnect, ushort SourcePing)
        {
            Connect = SourceConnect;
            Ping = SourcePing;
        }

        /// <summary>
        /// Получить объект учитывая его свойства
        /// </summary>
        /// <param name="SourceConnect">Текущее состояние подключения</param>
        /// <param name="SourcePing">Количество миллисекунд потраченное на подключение</param>
        /// <returns></returns>
        public static ObjectConnectEventArgs GetObject(bool SourceConnect, ushort SourcePing) =>
            new(SourceConnect, SourcePing);
    }
}
