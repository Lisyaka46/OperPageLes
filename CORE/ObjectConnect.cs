using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static OperPage_les.App;

namespace OperPage_les.CORE
{
    internal class ObjectConnect
    {
        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        public bool ConnectInternet { get; private set; }

        /// <summary>
        /// Прошлое состояние подключения к интернету
        /// </summary>
        public bool OLD_ConnectInternet { get; private set; }

        /// <summary>
        /// Количество миллисекунд потраченное на обновление подключения
        /// </summary>
        internal ushort MillisecondUpdateTime { get; private set; }

        /// <summary>
        /// Инициализировать стартовый объект подключения к интернету
        /// </summary>
        public ObjectConnect()
        {
            ConnectInternet = false;
            MillisecondUpdateTime = 0;
        }

        /// <summary>
        /// Проверка подключения интернета
        /// </summary>
        internal async Task UpdateInternetConnection()
        {
            try
            {
                PingReply reply = await new Ping().SendPingAsync("yandex.ru", 3000);
                OLD_ConnectInternet = ConnectInternet;
                ConnectInternet = reply.Status == IPStatus.Success;
                MillisecondUpdateTime = (ushort)reply.RoundtripTime;
            }
            catch
            {
                OLD_ConnectInternet = ConnectInternet;
                ConnectInternet = false;
            }
        }
    }
}
