using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ApplicationOperPageLes.CORE.Network
{
    internal class TcpClientChat
    {
        /// <summary>
        /// Клиент передачи информации о сообщении
        /// </summary>
        internal TcpClient DeviceClientMessageObjInfo { get; private set; } = new();

        /// <summary>
        /// Клиент передачи строки сообщения
        /// </summary>
        internal TcpClient DeviceClientStringMessage { get; private set; } = new();

        /// <summary>
        /// Клиент передачи данных файлов
        /// </summary>
        internal TcpClient DeviceClientDataFile { get; private set; } = new();

        /// <summary>
        /// Имеется ли текущее подключение
        /// </summary>
        internal bool Connected => DeviceClientMessageObjInfo.Connected && DeviceClientStringMessage.Connected && DeviceClientDataFile.Connected;

        /// <summary>
        /// Подключиться асинхронно к хосту
        /// </summary>
        /// <param name="IP">Хост к которому подключается клиент</param>
        /// <returns></returns>
        internal async Task<bool> ConnectAsync(string IP)
        {
            try
            {
                await DeviceClientMessageObjInfo.ConnectAsync(IP, Chat.PortConnectionMessageObjInfo);
                DeviceClientMessageObjInfo.ReceiveBufferSize = 256;
                DeviceClientMessageObjInfo.SendBufferSize = 256;

                await DeviceClientStringMessage.ConnectAsync(IP, Chat.PortConnectionStringMessage);
                DeviceClientStringMessage.ReceiveBufferSize = 4096;
                DeviceClientStringMessage.SendBufferSize = 4096;

                await DeviceClientDataFile.ConnectAsync(IP, Chat.PortConnectionDataFile);
                DeviceClientDataFile.ReceiveBufferSize = 16384;
                DeviceClientDataFile.SendBufferSize = 16384;
                return true;
            }
            catch
            {
                if (DeviceClientMessageObjInfo.Connected)
                    DeviceClientMessageObjInfo.Close();

                if (DeviceClientStringMessage.Connected)
                    DeviceClientStringMessage.Close();

                if (DeviceClientDataFile.Connected)
                    DeviceClientDataFile.Close();
                return false;
            }
        }

        /// <summary>
        /// Принять подключаемые порты клиентов
        /// </summary>
        /// <param name="ClientMessageObjInfo">Клиент передачи информации о сообщении</param>
        /// <param name="ClientStringMessage">Клиент передачи строки сообщения</param>
        /// <param name="ClientDataFile">Клиент передачи данных файлов</param>
        /// <returns></returns>
        internal bool AcceptConnect(TcpClient ClientMessageObjInfo, TcpClient ClientStringMessage, TcpClient ClientDataFile)
        {
            if (ClientMessageObjInfo.Connected && ClientStringMessage.Connected && ClientDataFile.Connected)
            {
                DeviceClientMessageObjInfo = ClientMessageObjInfo;
                DeviceClientMessageObjInfo.ReceiveBufferSize = 256;
                DeviceClientMessageObjInfo.SendBufferSize = 256;

                DeviceClientStringMessage = ClientStringMessage;
                DeviceClientStringMessage.ReceiveBufferSize = 4096;
                DeviceClientStringMessage.SendBufferSize = 4096;

                DeviceClientDataFile = ClientDataFile;
                DeviceClientDataFile.ReceiveBufferSize = 16384;
                DeviceClientDataFile.SendBufferSize = 16384;
                return true;
            }
            return false;
        }
    }
}
