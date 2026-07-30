using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Threading;

namespace OperPageLes.CORE.Internet
{
    internal static class Connection
    {
        /// <summary>
        /// Максимальный допустимый пинг при проверке интернета
        /// </summary>
        internal static ushort MaxPing { get; set; } = 3000;

        /// <summary>
        /// Хост использующийся для проверки интернета
        /// </summary>
        internal static string Host { get; set; } = "ya.ru";

        /// <summary>
        /// Текущее подключение к интернету
        /// </summary>
        internal static bool StateConnect { get; set; } = false;

        /// <summary>
        /// Событие изменения подключения к интернету
        /// </summary>
        internal static event EventHandler<bool>? ConnectionChanged;

        /// <summary>
        /// Событие изменения потраченных милликенунд на подключение к интернету
        /// </summary>
        internal static event EventHandler<ushort>? PingChanged;

        /// <summary>
        /// Запустить процесс проверки подключения к интернету
        /// </summary>
        /// <param name="Token">Токен для отмены операции проверки</param>
        internal static async Task StartRunTimeCheckInternetConnection(CancellationToken Token = default)
        {
            bool? OldValue = default;
            using Ping PingData = new();
            ConnectStatus SourceStatus;
            while (!Token.IsCancellationRequested)
            {
                try
                {
                    SourceStatus = ConnectStatus.GetInternetConnectionStatus(PingData, Host, MaxPing);
                    if (OldValue == null || OldValue != SourceStatus.Connection)
                    {
                        OldValue = SourceStatus.Connection;
                        StateConnect = SourceStatus.Connection;
                        ConnectionChanged?.Invoke(null, SourceStatus.Connection);
                    }
                    PingChanged?.Invoke(null, SourceStatus.CurrentPing);
                    await Task.Delay(4000, Token);
                }
                catch (OperationCanceledException) { break; }
                catch { throw; }
            }
        }
    }
}
