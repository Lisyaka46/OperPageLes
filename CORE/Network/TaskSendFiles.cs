using OPLAPI.OIEL.UserElementsControl.Network;
using System.IO;
using System.Net.Sockets;

namespace OperPageLes.CORE.Network
{
    internal class TaskSendFiles()
    {
        /// <summary>
        /// Конастанта размера файла с привышением которого будет показываться текстовый прогресс
        /// </summary>
        private const long LengthBytesFromVisualTextProgress = 11274310L;

        /// <summary>
        /// Текущий поток отправки файлов
        /// </summary>
        private Task? SourceSendTask;

        /// <summary>
        /// Состояние активного потока отправки файлов
        /// </summary>
        internal bool Activate { get; private set; } = false;

        /// <summary>
        /// Количество файлов переданных в очередь
        /// </summary>
        internal uint CountQueueFiles { get; private set; } = 0u;

        /// <summary>
        /// Количество файлов загруженных из очереди
        /// </summary>
        internal int CountCompletedFiles { get; private set; } = 0;

        /// <summary>
        /// Процент загрузки текущего файла
        /// </summary>
        internal double LoadingCurrentFile { get; private set; } = 0d;

        /// <summary>
        /// Массив очереди зависимых объектов сообщений
        /// </summary>
        private volatile Queue<OPLNetworkMessage> UIMessages = [];

        /// <summary>
        /// Массив очереди передачи файлов
        /// </summary>
        private volatile Queue<string[]> InfoPathFiles = [];

        /// <summary>
        /// Добавить очередь отправки файлов
        /// </summary>
        /// <param name="FilesInfo">Информация о принимаемых</param>
        /// <param name="ClipFiles">Прикреплённые файлы к сообщению</param>
        internal void AddQueue(OPLNetworkMessage UIMessageElement, string[] PathFiles)
        {
            for (uint i = 0; i < UIMessageElement.StackPanelClip.Children.Count; i++)
                ((OPLVisualNetworkClipFile)UIMessageElement.StackPanelClip.Children[(int)i]).NumberIndex = CountQueueFiles + i;

            UIMessages.Enqueue(UIMessageElement);
            InfoPathFiles.Enqueue(PathFiles);

            CountQueueFiles += (uint)PathFiles.Length;
        }

        /// <summary>
        /// Принять все данные файлов
        /// </summary>
        /// <returns></returns>
        internal void SendProcess(Socket SocketSendFile)
        {
            if (Activate)
                throw new Exception("Невозможно запустить обработку очереди повторно!");
            SourceSendTask = new(() =>
            {
                Activate = true;
                while (UIMessages.Count > 0)
                    ExecuteSendFiles(SocketSendFile, UIMessages.Dequeue(), InfoPathFiles.Dequeue()).Wait();
                Activate = false;
                CountCompletedFiles = 0;
                CountQueueFiles = 0;
                LoadingCurrentFile = 0d;
            });
            SourceSendTask.Start();
        }

        /// <summary>
        /// Активировать передачу файлов
        /// </summary>
        private async Task ExecuteSendFiles(Socket SocketSendFile, OPLNetworkMessage UIMessageElement, string[] PathFiles)
        {
            OPLVisualNetworkClipFile ClipElement;
            byte[] Buffer;
            FileStream Reader;
            for (int i = 0; i < PathFiles.Length; i++)
            {
                ClipElement = UIMessageElement.Dispatcher.Invoke(() => (OPLVisualNetworkClipFile)UIMessageElement.StackPanelClip.Children[i]);
                ClipElement.Dispatcher.Invoke(() => ClipElement.IsVisibleIndex = false);

                if (!File.Exists(PathFiles[i]))
                {
                    ClipElement.IsEnabled = false;
                    return;
                }

                Reader = new(PathFiles[i], FileMode.Open, FileAccess.Read, FileShare.None);
                LoadingCurrentFile = 0d;
                if (Reader.Length > SocketSendFile.SendBufferSize)
                {
                    ClipElement.Dispatcher.Invoke(() => ClipElement.StartManipulate(Reader.Length >= LengthBytesFromVisualTextProgress));
                    Buffer = new byte[SocketSendFile.SendBufferSize];
                    while (SocketSendFile.SendBufferSize < Reader.Length - Reader.Position)
                    {
                        await Reader.ReadExactlyAsync(Buffer);
                        await SocketSendFile.SendAsync(Buffer);
                        LoadingCurrentFile = (double)Reader.Position / (double)Reader.Length;
                        ClipElement.Dispatcher.Invoke(() => ClipElement.SetValueManipulate(LoadingCurrentFile));
                    }
                    ClipElement.Dispatcher.Invoke(ClipElement.EndManipulate);
                }
                Buffer = new byte[Reader.Length - Reader.Position];
                await Reader.ReadExactlyAsync(Buffer);
                await SocketSendFile.SendAsync(Buffer);

                Reader.Close();
                Reader.Dispose();

                CountQueueFiles--;
                CountCompletedFiles++;
            }
        }
    }
}
