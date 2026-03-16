using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementsControl.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Controls;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.CORE.Network
{
    internal class TaskReceiveFiles
    {
        /// <summary>
        /// Текущий поток принятия файлов
        /// </summary>
        private Task? SourceReceiveTask = null;

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
        private List<ReadOnlyCollection<FileNetworkInfo>> InfoReceiveFiles = [];

        /// <summary>
        /// Массив очереди зависимых объектов сообщений
        /// </summary>
        private List<UIElementCollection> CollectionsClipFiles = [];

        public TaskReceiveFiles()
        {
        }

        /// <summary>
        /// Отправить все данные файлов
        /// </summary>
        /// <param name="UIMessageElement">Объект сообщения который отображает прикреплённые файлы</param>
        /// <param name="FilesInfo">Данные о передаваемых файлах</param>
        /// <param name="PathFiles">Директории передаваемых файлов</param>
        /// <returns></returns>
        internal void ReceiveProcess(TcpClient Client, ReadOnlyCollection<FileNetworkInfo> FilesInfo, UIElementCollection ClipFiles)
        {
            InfoReceiveFiles.Add(FilesInfo);
            CollectionsClipFiles.Add(ClipFiles);
            CountQueueFiles += FilesInfo.Count;
            if (!Activate)
            {
                SourceReceiveTask = new(async () =>
                {
                    Activate = true;
                    while (InfoReceiveFiles.Count > 0)
                    {
                        for (int i = 0; i < InfoReceiveFiles.Count; i++)
                        {
                            await ExecuteReceiveFile(Client, InfoReceiveFiles[0][i], (OPLNetworkClipElement)CollectionsClipFiles[0][i]);
                            CountCompletedFiles = i;
                        }
                        InfoReceiveFiles.RemoveAt(0);
                        CollectionsClipFiles.RemoveAt(0);
                    }
                    Activate = false;
                });
                SourceReceiveTask.Start();
            }
        }

        /// <summary>
        /// Активировать принятие файлов
        /// </summary>
        private async Task ExecuteReceiveFile(TcpClient SourceTCPClient, FileNetworkInfo CurrentFileInfo, OPLNetworkClipElement ClipElement)
        {
            byte[] ExpansionFileData, NameFileData, Buffer;
            string PathFile, NameFile;
            LoadingCurrentFile = 0d;

            ExpansionFileData = Chat.ReceiveNetworkByte(SourceTCPClient, CurrentFileInfo.LengthFileExpansion);
            NameFileData = Chat.ReceiveNetworkByte(SourceTCPClient, CurrentFileInfo.LengthFileName);
            NameFile = $"{Encoding.UTF8.GetString(NameFileData)}.{Encoding.UTF8.GetString(ExpansionFileData)}";
            PathFile = $"{StructDirectoryResources.DirectoryDownloadApplication}{NameFile}";

            ClipElement.Dispatcher.Invoke(() =>
            {
                ClipElement.MathSizeFile(CurrentFileInfo.LengthFileData);
                ClipElement.Text = NameFile;
            });

            Stream StreamDownloadFile = File.OpenWrite(PathFile);
            ClipElement.Dispatcher.Invoke(() =>
            {
                ClipElement.SetExtractAssociatedIcon(PathFile, StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication)));
                ClipElement.StartManipulate();
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(ClipElement,
                    OPLNetworkClipElement.OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(ClipElement,
                    OPLNetworkClipElement.MarginProperty, new(3), TimeSpan.FromMilliseconds(500d));
            });

            if (CurrentFileInfo.LengthFileData < SourceTCPClient.ReceiveBufferSize)
            {
                Buffer = new byte[CurrentFileInfo.LengthFileData];
                await SourceTCPClient.Client.ReceiveAsync(Buffer);
                await StreamDownloadFile.WriteAsync(Buffer);
                LoadingCurrentFile = 1d;
                ClipElement.SetValueManipulate(1d);
            }
            else
            {
                Buffer = new byte[SourceTCPClient.ReceiveBufferSize];
                while (StreamDownloadFile.Position < CurrentFileInfo.LengthFileData)
                {
                    await SourceTCPClient.Client.ReceiveAsync(Buffer);
                    StreamDownloadFile.Write(Buffer);
                    if (StreamDownloadFile.Position + SourceTCPClient.ReceiveBufferSize > CurrentFileInfo.LengthFileData)
                    {
                        Buffer = new byte[CurrentFileInfo.LengthFileData - StreamDownloadFile.Position];
                        await SourceTCPClient.Client.ReceiveAsync(Buffer);
                        StreamDownloadFile.Write(Buffer);
                    }
                    LoadingCurrentFile = (double)StreamDownloadFile.Position / (double)CurrentFileInfo.LengthFileData;
                    ClipElement.Dispatcher.Invoke(() => ClipElement.SetValueManipulate(LoadingCurrentFile));
                }
            }
            StreamDownloadFile.Close();
            StreamDownloadFile.Dispose();
            await Task.Delay(SourceTCPClient.ReceiveTimeout);
            ClipElement.Dispatcher.Invoke(ClipElement.EndManipulate);

            GC.Collect();
        }
    }
}
