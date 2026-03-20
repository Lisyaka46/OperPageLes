using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementsControl.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.CORE.Network
{
    internal class TaskReceiveFiles()
    {
        /// <summary>
        /// Текущий поток принятия файлов
        /// </summary>
        private Task? SourceReceiveTask = null;

        /// <summary>
        /// Состояние активного потока отправки файлов
        /// </summary>
        internal bool Activate { get; private set; } = false;

        /// <summary>
        /// Количество файлов переданных в очередь
        /// </summary>
        internal uint CountQueueFiles { get; private set; } = 0;

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
        private volatile Queue<ReadOnlyCollection<FileNetworkInfo>> InfoReceiveFiles = [];

        /// <summary>
        /// Массив очереди зависимых объектов сообщений
        /// </summary>
        private volatile Queue<UIElementCollection> CollectionsClipFiles = [];

        /// <summary>
        /// Добавить очередь отправки файлов
        /// </summary>
        /// <param name="FilesInfo">Информация о принимаемых</param>
        /// <param name="ClipFiles">Прикреплённые файлы к сообщению</param>
        internal void AddQueue(ReadOnlyCollection<FileNetworkInfo> FilesInfo, UIElementCollection ClipFiles)
        {
            OPLNetworkClipElement Clip;
            for (uint i = 0; i < ClipFiles.Count; i++)
            {
                Clip = (OPLNetworkClipElement)ClipFiles[(int)i];
                Clip.SetIndex(CountQueueFiles + i);
                Clip.SetExtractAssociatedIcon(String.Empty,
                        StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication)));
                //Clip.TextMessage = "В ожидании";
            }

            InfoReceiveFiles.Enqueue(FilesInfo);
            CollectionsClipFiles.Enqueue(ClipFiles);
            CountQueueFiles += (uint)FilesInfo.Count;
        }

        /// <summary>
        /// Принять все данные файлов
        /// </summary>
        /// <returns></returns>
        internal void ReceiveProcess(Socket SocketReceiveFile)
        {
            if (Activate)
                throw new Exception("Невозможно запустить обработку очереди повторно!");
            SourceReceiveTask = new(() =>
            {
                Activate = true;
                while (InfoReceiveFiles.Count > 0)
                    ExecuteReceiveFile(SocketReceiveFile, InfoReceiveFiles.Dequeue(), CollectionsClipFiles.Dequeue()).Wait();
                Activate = false;
                CountCompletedFiles = 0;
                CountQueueFiles = 0;
                LoadingCurrentFile = 0d;
            });
            SourceReceiveTask.Start();
        }

        /// <summary>
        /// Активировать принятие файлов
        /// </summary>
        private async Task ExecuteReceiveFile(Socket SocketReceiveFile,
            ReadOnlyCollection<FileNetworkInfo> CurrentFileInfo, UIElementCollection ClipElementCollection)
        {
            byte[] Buffer;
            string PathFile, ReNamePath;
            OPLNetworkClipElement ClipElement;
            FileStream Writer;
            for (int i = 0; i < CurrentFileInfo.Count; i++)
            {
                ClipElement = (OPLNetworkClipElement)ClipElementCollection[i];

                PathFile = $"{StructDirectoryResources.DirectoryDownloadApplication}{CurrentFileInfo[i].FileName}";

                ClipElement.Dispatcher.Invoke(() =>
                {
                    ClipElement.ClearIndex();
                    //ClipElement.TextMessage = string.Empty;
                    ClipElement.SetExtractAssociatedIcon(PathFile,
                        StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication)));
                });

                Writer = new($"{PathFile}.download", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                LoadingCurrentFile = 0d;
                await Task.Delay(SocketReceiveFile.ReceiveTimeout);
                if (CurrentFileInfo[i].LengthFileData > SocketReceiveFile.ReceiveBufferSize)
                {
                    ClipElement.Dispatcher.Invoke(ClipElement.StartManipulate);
                    Buffer = new byte[SocketReceiveFile.ReceiveBufferSize];
                    while (Writer.Position + SocketReceiveFile.ReceiveBufferSize < CurrentFileInfo[i].LengthFileData)
                    {
                        await SocketReceiveFile.ReceiveAsync(Buffer);
                        Writer.Write(Buffer);
                        await Task.Delay(SocketReceiveFile.ReceiveTimeout);
                        LoadingCurrentFile = (double)Writer.Position / (double)CurrentFileInfo[i].LengthFileData;
                        ClipElement.Dispatcher.Invoke(() => ClipElement.SetValueManipulate(LoadingCurrentFile));
                    }
                    ClipElement.Dispatcher.Invoke(ClipElement.EndManipulate);
                }
                Buffer = new byte[CurrentFileInfo[i].LengthFileData - Writer.Position];
                await SocketReceiveFile.ReceiveAsync(Buffer);
                Writer.Write(Buffer);
                await Task.Delay(SocketReceiveFile.ReceiveTimeout);

                Writer.Close();
                Writer.Dispose();

                ReNamePath = Path.ChangeExtension($"{PathFile}.download", $".{CurrentFileInfo[i].FileExtension}");
                if (File.Exists(ReNamePath)) File.Delete(ReNamePath);
                File.Move($"{PathFile}.download", Path.ChangeExtension($"{PathFile}.download", $".{CurrentFileInfo[i].FileExtension}"));
                ClipElement.Dispatcher.Invoke(() =>
                    ClipElement.SetExtractAssociatedIcon($"{PathFile}.{CurrentFileInfo[i].FileExtension}",
                        StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication))));

                CountQueueFiles--;
                CountCompletedFiles++;
            }
        }
    }
}
