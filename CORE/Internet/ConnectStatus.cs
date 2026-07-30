using LibraryIEL.CORE.Themes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static OperPageLes.App;

namespace OperPageLes.CORE.Internet
{
    internal readonly ref struct ConnectStatus
    {
        /// <summary>
        /// Данные подключения
        /// </summary>
        private readonly byte[] Data;

        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        public readonly bool Connection => Data[4] != 0;

        /// <summary>
        /// Количество миллисекунд потраченное на обновление подключения
        /// </summary>
        public readonly ushort CurrentPing => BitConverter.ToUInt16(Data, 0);

        /// <summary>
        /// Максимальный предел ожидания ответа
        /// </summary>
        public readonly ushort MaxPing => BitConverter.ToUInt16(Data, 2);

        /// <summary>
        /// Количество байт хранимых для состояния подключения к сети
        /// </summary>
        public const byte CountBytes = 5;

        /// <summary>
        /// Инициализировать объект отображающий состояние подключения к интернету
        /// </summary>
        /// <param name="SourceDataBytes">Байты для чтения данных</param>
        private ConnectStatus(byte[] SourceDataBytes)
        {
            if (SourceDataBytes.Length != CountBytes)
                throw new ArgumentException($"Массив байт не соответствует с ожидаемым размером ({SourceDataBytes.Length} => {CountBytes})",
                nameof(SourceDataBytes));
            Data = SourceDataBytes;
        }

        /// <summary>
        /// Проверка подключения интернета
        /// </summary>
        internal static ConnectStatus GetInternetConnectionStatus(Ping PingData, string HostNameOrAddress, ushort MaxPing)
        {
            byte[] DataBytes = new byte[CountBytes];
            try
            {
                PingReply reply = PingData.Send(HostNameOrAddress, MaxPing);
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)reply.RoundtripTime), 0, DataBytes, 0, sizeof(ushort));
                DataBytes[4] = (byte)(reply.Status == IPStatus.Success ? 1 : 0);
            }
            catch
            {
                Buffer.BlockCopy(BitConverter.GetBytes(MaxPing), 0, DataBytes, 0, sizeof(ushort));
                DataBytes[4] = 0;
            }
            Buffer.BlockCopy(BitConverter.GetBytes(MaxPing), 0, DataBytes, 2, sizeof(ushort));
            return new(DataBytes);
        }
    }
}
