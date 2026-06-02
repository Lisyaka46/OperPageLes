using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static OperPageLes.App;

namespace OperPageLes.CORE
{
    internal class ObjectConnect
    {
        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        public bool ConnectInternet { get; private set; }

        /// <summary>
        /// Количество миллисекунд потраченное на обновление подключения
        /// </summary>
        internal ushort CurrentPing { get; private set; }

        /// <summary>
        /// Максимальный предел ожидания ответа
        /// </summary>
        internal readonly ushort MaxPing;

        /// <summary>
        /// Текущий объект проверки подключения
        /// </summary>
        private Ping PingObject = new();

        /// <summary>
        /// Инициализировать стартовый объект подключения к интернету
        /// </summary>
        public ObjectConnect(ushort SourceMaxPing = 3000)
        {
            ConnectInternet = false;
            CurrentPing = 0;
            MaxPing = SourceMaxPing;
        }

        /// <summary>
        /// Проверка подключения интернета
        /// </summary>
        internal ObjectConnectEventArgs UpdateInternetConnection()
        {
            try
            {
                PingReply reply = PingObject.Send("yandex.ru", MaxPing);
                ConnectInternet = reply.Status == IPStatus.Success;
                CurrentPing = (ushort)reply.RoundtripTime;
            }
            catch
            {
                ConnectInternet = false;
                CurrentPing = 3000;
            }
            return ObjectConnectEventArgs.GetObject(ConnectInternet, CurrentPing);
        }
    }
}
