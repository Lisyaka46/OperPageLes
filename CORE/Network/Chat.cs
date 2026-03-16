using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementsControl.Network;
using IEL.CORE.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.CORE.Network
{
    internal class Chat : IDisposable
    {
        /// <summary>
        /// Делегат события изменения занятости принятия или отправки данных объектом
        /// </summary>
        /// <param name="NewValue">Новое актуальное значение</param>
        internal delegate void IsBusyChangedEventHandler(bool NewValue);

        /// <summary>
        /// Клиент с которым осуществляется передача данных информации об передаваемом сообщении
        /// </summary>
        internal TcpClientChat DeviceClient { get; private set; }

        /// <summary>
        /// Объект информации о подключённом пользователе
        /// </summary>
        internal ChatUser User { get; private set; }

        /// <summary>
        /// Порт который является прослушиваемым для объектов сообщений
        /// </summary>
        internal static readonly int PortConnectionMessageObjInfo = 6019;

        /// <summary>
        /// Порт который является прослушиваемым для строки сообщения
        /// </summary>
        internal static readonly int PortConnectionStringMessage = 6020;

        /// <summary>
        /// Порт который является прослушиваемым для данных файлов
        /// </summary>
        internal static readonly int PortConnectionDataFile = 6021;

        /// <summary>
        /// Объект отображения чата
        /// </summary>
        internal readonly OPLNetworkChat UIChat;

        /// <summary>
        /// Объект отображения истории сообщений
        /// </summary>
        internal readonly StackPanel HistoryMessages;

        /// <summary>
        /// Объект отображения прикреплённых файлов к сообщению
        /// </summary>
        internal readonly StackPanel ClipFiles;

        /// <summary>
        /// Введённый текст в поле сообщения
        /// </summary>
        internal string EnteringMessage = string.Empty;

        /// <summary>
        /// Aсинхронный процесс отправки файлов
        /// </summary>
        private TaskSendFiles SendFiles;

        /// <summary>
        /// Aсинхронный процесс принятия файлов
        /// </summary>
        private TaskReceiveFiles ReceiveFiles;

        private bool _IsBusy = false;

        public Chat(ref StackPanel History, ref StackPanel Clips, ref OPLNetworkChat ChatElement)
        {
            DeviceClient = new();
            UIChat = ChatElement;
            HistoryMessages = History;
            ClipFiles = Clips;
            SendFiles = new();
            ReceiveFiles = new();
            User = new();
        }

        /// <summary>
        /// Состояние занятости
        /// </summary>
        internal bool IsBusy
        {
            get => _IsBusy;
            private set
            {
                _IsBusy = value;
                IsBusyChanged?.Invoke(IsBusy);
            }
        }

        /// <summary>
        /// Событие изменения свойства занятости объекта
        /// </summary>
        internal event IsBusyChangedEventHandler? IsBusyChanged;

        /// <summary>
        /// Принять подключение
        /// </summary>
        /// <param name="client">Подключённый клиент</param>
        /// <exception cref="InvalidOperationException"></exception>
        internal void AcceptConnect(TcpClientChat client)
        {
            if (!client.Connected)
                throw new InvalidOperationException("Невозможно принять объект подключения в его не подключённом состоянии");
            DeviceClient = client;
            UIChat.SenderTextPoint = User.NameUser;
            UIChat.IsEnabled = true;
        }

        /// <summary>
        /// Завершить исполнение и очистить все ресурсы объекта
        /// </summary>
        public void Dispose()
        {
            //if (DeviceClient.Connected)
            //    DeviceClient.(); // Функция отключения от хоста
            //DeviceClient.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Отправить данные сообщения и прикреплённых к сообщению файлов
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="PathFiles">Прикреплённые директории файлов</param>
        /// <returns></returns>
        internal void SendNetworkData(string Message, string[] PathFiles)
        {
            if (!DeviceClient.Connected)
                throw new InvalidOperationException("Невозможно передать данные не имея текущее подключение");
            OPLNetworkMessage NetworkMessage = new()
            {
                Message = Message,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                Margin = new(3),
                SenderTextPoint = "Вы",
            };
            DataNetworkInfo DataInfo = new(ref Message, ref PathFiles);
            UIChat.TextBlockHead.Text = $"{(ReceiveFiles.Activate ? "**" : string.Empty)}Вы:";
            UIChat.EndMessage = Message;
            UIChat.Icon = PathFiles.Length > 0 ? StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Save)) : null;
            UIChat.TextCount = DataInfo.CountFilesData;
            NetworkMessage.SetVisualFromNetworkInfo(DataInfo, ClipFiles.Children);
            HistoryMessages.Children.Add(NetworkMessage);
            try
            {
                DeviceClient.DeviceClientMessageObjInfo.Client.Send([.. DataInfo.SourceBytes]);
                if (DataInfo.LengthMessage > 0)
                    Task.Run(() => SendNetworkByte(Encoding.UTF8.GetBytes(Message)));
                if (DataInfo.CountFilesData > 0 && PathFiles.Length > 0 && DataInfo.FilesInfo != null)
                {
                    SendFiles.AddSendProcess(DeviceClient.DeviceClientDataFile, NetworkMessage, DataInfo.FilesInfo, PathFiles);
                }
            }
            catch
            {
                NetworkMessage.IsEnabled = false;
            }
        }

        /// <summary>
        /// Осуществить принятие данных по переданному шаблону информации о данных
        /// </summary>
        /// <returns></returns>
        internal async Task ReceiveNetworkData()
        {
            if (!DeviceClient.Connected)
                throw new InvalidOperationException("Невозможно принять данные не имея текущее подключение");
            List<byte> Data = [];
            byte[] SourceInfo = new byte[3];
            await DeviceClient.DeviceClientMessageObjInfo.Client.ReceiveAsync(SourceInfo);
            Data.AddRange(SourceInfo);
            if (Data[2] > 0)
            {
                SourceInfo = new byte[Data[2] * FileNetworkInfo.LengthDataOneObject];
                await DeviceClient.DeviceClientMessageObjInfo.Client.ReceiveAsync(SourceInfo);
                Data.AddRange(SourceInfo);
            }
            DataNetworkInfo DataInfo = new([..Data]);
            Data.Clear();
            GC.Collect();
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(UIChat.TextBlockHead,
                TextBlock.OpacityProperty, 0d, TimeSpan.FromMilliseconds(200d));
            OPLNetworkMessage NetworkMessage = new()
            {
                SenderTextPoint = "Неизвестный", // Тут должен быть переданный объект пользователя
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                Margin = new(3),
            };
            NetworkMessage.SetVisualFromNetworkInfo(DataInfo);
            HistoryMessages.Children.Add(NetworkMessage);
            UIChat.Icon = DataInfo.FilesInfo != null ? StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Save)) : null;
            UIChat.TextCount = DataInfo.CountFilesData;
            if (DataInfo.LengthMessage > 0)
            {
                NetworkMessage.Message = Encoding.UTF8.GetString(ReceiveNetworkByte(DeviceClient.DeviceClientStringMessage, DataInfo.LengthMessage));
            }
            UIChat.EndMessage = NetworkMessage.Message;
            UIChat.TextBlockHead.Text = $"{(ReceiveFiles.Activate ? "**" : string.Empty)}{NetworkMessage.SenderTextPoint}:";
            if (DataInfo.CountFilesData > 0 && DataInfo.FilesInfo != null)
            {
                ReceiveFiles.ReceiveProcess(DeviceClient.DeviceClientDataFile, DataInfo.FilesInfo, NetworkMessage.StackPanelClip.Children);
            }
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(UIChat.TextBlockHead,
                TextBlock.OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(600d));
        }

        #region Send
        /// <summary>
        /// Отправить подготовленные полные данные клиенту
        /// </summary>
        /// <param name="Data">Данные</param>
        /// <returns></returns>
        internal void SendNetworkByte(byte[] Data)
        {
            if (!DeviceClient.Connected)
                throw new InvalidOperationException("Невозможно отправить данные не имея подключения");
            int CountRepeat = Data.Length / DeviceClient.DeviceClientStringMessage.SendBufferSize;
            int EndRemainRepeat = Data.Length % DeviceClient.DeviceClientStringMessage.SendBufferSize;
            for (int i = 0; i < CountRepeat; i++)
            {
                DeviceClient.DeviceClientStringMessage.Client.Send(
                    new ArraySegment<byte>(Data, i * DeviceClient.DeviceClientStringMessage.SendBufferSize,
                    DeviceClient.DeviceClientStringMessage.SendBufferSize));
                Task.Delay(DeviceClient.DeviceClientStringMessage.SendTimeout);
            }
            if (EndRemainRepeat > 0)
            {
                DeviceClient.DeviceClientStringMessage.Client.Send(
                    new ArraySegment<byte>(Data, CountRepeat * DeviceClient.DeviceClientStringMessage.SendBufferSize,
                    EndRemainRepeat));
            }
            Task.Delay(DeviceClient.DeviceClientStringMessage.SendTimeout);
        }
        #endregion

        #region Receive
        /// <summary>
        /// Принять данные сообщения
        /// </summary>
        /// <param name="LengthReadBytes">Число отражающее количество читаемой информации (В байтах)</param>
        /// <returns></returns>
        internal static byte[] ReceiveNetworkByte(TcpClient DeviceClient, uint LengthReadBytes)
        {
            if (LengthReadBytes < DeviceClient.ReceiveBufferSize)
            {
                byte[] Buffer = new byte[LengthReadBytes];
                DeviceClient.Client.Receive(Buffer);
                return Buffer;
            }
            else
            {
                List<byte> Data = [];
                byte[] Buffer = new byte[DeviceClient.ReceiveBufferSize];
                while (Data.Count < LengthReadBytes)
                {
                    DeviceClient.Client.Receive(Buffer);
                    Data.AddRange(Buffer);
                    if (LengthReadBytes - Data.Count < DeviceClient.ReceiveBufferSize)
                    {
                        Buffer = new byte[LengthReadBytes - Data.Count];
                        DeviceClient.Client.Receive(Buffer);
                        Data.AddRange(Buffer);
                    }
                }
                return [.. Data];
            }
        }
        #endregion
    }
}
