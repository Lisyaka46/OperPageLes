using System.Net;
using System.Net.Sockets;

namespace OperPageLes.CORE.Network
{
    internal class TcpListenerChat
    {
        /// <summary>
        /// Клиент передачи информации о сообщении
        /// </summary>
        private TcpListener ListenerMessageObjInfo = new(System.Net.IPAddress.Any, Chat.PortConnectionMessageObjInfo);

        /// <summary>
        /// Клиент передачи строки сообщения
        /// </summary>
        private TcpListener ListenerStringMessage = new(System.Net.IPAddress.Any, Chat.PortConnectionStringMessage);

        /// <summary>
        /// Клиент передачи данных файлов
        /// </summary>
        private TcpListener ListenerDataFile = new(System.Net.IPAddress.Any, Chat.PortConnectionDataFile);

        /// <summary>
        /// Асинхронный процесс обработки подключений к портам
        /// </summary>
        private Task? TaskPendingListener;

        /// <summary>
        /// Токен управляемый процессом обработки подключений к портам
        /// </summary>
        private CancellationTokenSource TokenSourcePendingListener = new();

        /// <summary>
        /// Событие принятия подключения чата
        /// </summary>
        internal event EventHandler<TcpClientChat>? ListenerPendingChat;

        /// <summary>
        /// Запустить прослушивание портов
        /// </summary>
        /// <returns></returns>
        internal void ListenerStart()
        {
            TokenSourcePendingListener.TryReset();
            TaskPendingListener = new(async () =>
            {
                TcpClientChat SourceClient;
                List<TcpClient> ClientsObjInfo = [];
                List<TcpClient> ClientsStringMessage = [];
                List<TcpClient> ClientsDataFile = [];
                int COI, CSM, CDF;
                try
                {
                    while (!TokenSourcePendingListener.IsCancellationRequested)
                    {
                        
                        if (ListenerMessageObjInfo.Pending())
                            ClientsObjInfo.Add(await ListenerMessageObjInfo.AcceptTcpClientAsync());
                        if (ListenerStringMessage.Pending())
                            ClientsStringMessage.Add(await ListenerStringMessage.AcceptTcpClientAsync());
                        if (ListenerDataFile.Pending())
                            ClientsDataFile.Add(await ListenerDataFile.AcceptTcpClientAsync());
                        if (ClientsObjInfo.Count > 0)
                        {
                            for (COI = 0; COI < ClientsObjInfo.Count; COI++)
                            {
                                for (CSM = 0; CSM < ClientsStringMessage.Count; CSM++)
                                {
                                    if (
                                    (ClientsObjInfo[COI].Client.RemoteEndPoint as IPEndPoint)?.Address.Equals(
                                    (ClientsStringMessage[CSM].Client.RemoteEndPoint as IPEndPoint)?.Address) ?? false)
                                        break;
                                }
                                if (CSM == ClientsStringMessage.Count) continue;
                                for (CDF = 0; CDF < ClientsDataFile.Count; CDF++)
                                {
                                    if (
                                    (ClientsObjInfo[COI].Client.RemoteEndPoint as IPEndPoint)?.Address.Equals(
                                    (ClientsDataFile[CDF].Client.RemoteEndPoint as IPEndPoint)?.Address) ?? false)
                                        break;
                                }
                                if (CDF == ClientsDataFile.Count) continue;
                                SourceClient = new();
                                if (SourceClient.AcceptConnect(ClientsObjInfo[COI], ClientsStringMessage[CSM], ClientsDataFile[CDF]))
                                {
                                    ClientsObjInfo.RemoveAt(COI);
                                    ClientsStringMessage.RemoveAt(CSM);
                                    ClientsDataFile.RemoveAt(CDF);
                                    ListenerPendingChat?.Invoke(this, SourceClient);
                                }
                            }
                        }
                        await Task.Delay(1000);
                    }
                }
                catch { }
                foreach (TcpClient client in ClientsObjInfo)
                {
                    client.Close();
                    client.Dispose();
                }
                foreach (TcpClient client in ClientsStringMessage)
                {
                    client.Close();
                    client.Dispose();
                }
                foreach (TcpClient client in ClientsDataFile)
                {
                    client.Close();
                    client.Dispose();
                }
                ListenerMessageObjInfo.Stop();
                ListenerStringMessage.Stop();
                ListenerDataFile.Stop();
            }, TokenSourcePendingListener.Token);

            ListenerMessageObjInfo.Start();
            ListenerStringMessage.Start();
            ListenerDataFile.Start();

            TaskPendingListener.Start();
        }

        /// <summary>
        /// Остановить прослушивание портов
        /// </summary>
        internal void ListenerStop()
        {
            TokenSourcePendingListener.Cancel();
        }
    }
}
