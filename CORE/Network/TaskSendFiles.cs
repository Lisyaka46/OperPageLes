using ApplicationOperPageLes.UI.UserElementsControl.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ApplicationOperPageLes.CORE.Network
{
    internal class TaskSendFiles
    {
        /// <summary>
        /// Текущий поток отправки файлов
        /// </summary>
        private Task? SourceSendTask;

        /// <summary>
        /// Состояние активного потока отправки файлов
        /// </summary>
        internal bool Activate { get; private set; }

        /// <summary>
        /// Количество файлов переданных в очередь
        /// </summary>
        internal int CountQueueFiles { get; private set; } = 0;

        /// <summary>
        /// Количество файлов загруженных из очереди
        /// </summary>
        internal int CountCompletedFiles { get; private set; } = 0;

        /// <summary>
        /// Процент загрузки текущего файла
        /// </summary>
        internal double LoadingCurrentFile { get; private set; } = 0d;

        /// <summary>
        /// Массив очереди информации о передаваемых сообщениях
        /// </summary>
        private List<ReadOnlyCollection<FileNetworkInfo>> InfoSendFiles = [];

        /// <summary>
        /// Массив очереди зависимых объектов сообщений
        /// </summary>
        private List<OPLNetworkMessage> UIMessages = [];

        /// <summary>
        /// Массив очереди передачи файлов
        /// </summary>
        private List<string[]> InfoPathFiles = [];

        public TaskSendFiles()
        {
        }

        /// <summary>
        /// Отправить все данные файлов
        /// </summary>
        /// <param name="UIMessageElement">Объект сообщения который отображает прикреплённые файлы</param>
        /// <param name="FilesInfo">Данные о передаваемых файлах</param>
        /// <param name="PathFiles">Директории передаваемых файлов</param>
        /// <returns></returns>
        internal void AddSendProcess(TcpClient Client, OPLNetworkMessage UIMessageElement, ReadOnlyCollection<FileNetworkInfo> FilesInfo, string[] PathFiles)
        {
            InfoSendFiles.Add(FilesInfo);
            InfoPathFiles.Add(PathFiles);
            UIMessages.Add(UIMessageElement);
            CountQueueFiles += FilesInfo.Count;
            if (!Activate)
                SourceSendTask = Task.Run(async () => await ExecuteSendFiles(Client));
        }

        /// <summary>
        /// Активировать передачу файлов
        /// </summary>
        private async Task ExecuteSendFiles(TcpClient SourceTCPClient)
        {
            Stream StreamSendFile;
            int CountReadBytesInFile;
            long SendBytes = 0L;
            OPLNetworkClipElement ClipElement;
            byte[] Buffer;
            Activate = true;
            while (UIMessages.Count > 0)
            {
                for (int i = 0; i < InfoPathFiles[0].Length; i++)
                {
                    ClipElement = UIMessages[0].Dispatcher.Invoke(() => (OPLNetworkClipElement)UIMessages[0].StackPanelClip.Children[i]);
                    ClipElement.Dispatcher.Invoke(ClipElement.ClearIndex);
                    ClipElement.Dispatcher.Invoke(ClipElement.StartManipulate);
                    await SourceTCPClient.Client.SendAsync(new ArraySegment<byte>([.. InfoSendFiles[0][i].SourceBytes]));
                    StreamSendFile = File.OpenRead(InfoPathFiles[0][i]);
                    Buffer = new byte[SourceTCPClient.ReceiveBufferSize];
                    LoadingCurrentFile = 0d;
                    while (StreamSendFile.Position < InfoSendFiles[0][i].LengthFileData)
                    {
                        if (StreamSendFile.Position + SourceTCPClient.ReceiveBufferSize > InfoSendFiles[0][i].LengthFileData)
                        {
                            Buffer = new byte[InfoSendFiles[0][i].LengthFileData - StreamSendFile.Position];
                            CountReadBytesInFile = await StreamSendFile.ReadAsync(Buffer);
                            SendBytes += CountReadBytesInFile;
                            LoadingCurrentFile = 1d;
                            ClipElement.Dispatcher.Invoke(() => ClipElement.SetValueManipulate(1d));
                            await SourceTCPClient.Client.SendAsync(Buffer);
                            break;
                        }
                        CountReadBytesInFile = await StreamSendFile.ReadAsync(Buffer);
                        SendBytes += CountReadBytesInFile;
                        LoadingCurrentFile = (double)SendBytes / (double)InfoSendFiles[0][i].LengthFileData;
                        ClipElement.Dispatcher.Invoke(() => ClipElement.SetValueManipulate(LoadingCurrentFile));
                        await SourceTCPClient.Client.SendAsync(Buffer);
                    }
                    StreamSendFile.Close();
                    StreamSendFile.Dispose();
                    ClipElement.Dispatcher.Invoke(ClipElement.EndManipulate);
                }
                CountCompletedFiles++;
                UIMessages.RemoveAt(0);
                InfoPathFiles.RemoveAt(0);
                InfoSendFiles.RemoveAt(0);
                GC.Collect();
            }
            Activate = false;
            CountCompletedFiles = 0;
            CountQueueFiles = 0;
            LoadingCurrentFile = 0d;
        }
    }
}
