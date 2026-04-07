using OperPageLes.CORE.Network;
using OperPageLes.UI.UserElementsControl.Network;
using OperPageLes.UI.Windows.Dialogs;
using System.Windows;
using System.Windows.Controls;
using Orientation = System.Windows.Controls.Orientation;

namespace OperPageLes.UI.Pages.Browser.BrowserPageNetwork
{
    /// <summary>
    /// Логика взаимодействия для PageStackChats.xaml
    /// </summary>
    public partial class PageStackChats : Page
    {
        /// <summary>
        /// Объект прослушивания подключений по конкретному порту
        /// </summary>
        private static TcpListenerChat? Listener;

        /// <summary>
        /// Состояние активности прослушивания портов
        /// </summary>
        internal static bool IsActiveListener = false;

        /// <summary>
        /// Объект отображения чатов
        /// </summary>
        private static StackPanel StackChats = new()
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Top,
        };

        /// <summary>
        /// Массив подключённых чатов
        /// </summary>
        private static List<Chat> Chats = [];

        /// <summary>
        /// Асинхронный процесс обработки получения данных от клиента
        /// </summary>
        private Task? TaskReceiveClientData;

        /// <summary>
        /// Токен управляемый процессом получения данных от клиента
        /// </summary>
        private CancellationTokenSource TokenSourceReceiveClientData = new();

        /// <summary>
        /// Делегат события активирования чата
        /// </summary>
        /// <param name="chat">Активируемый чат</param>
        internal delegate void SelectChatDelegate(ref Chat chat);

        /// <summary>
        /// Событие выделение чата
        /// </summary>
        internal event SelectChatDelegate? SelectChat;

        public PageStackChats()
        {
            InitializeComponent();
            IELScrollViewerChats.Content = StackChats;

            TokenSourceReceiveClientData = new();

            IELButtonNewChat.OnActivateMouseLeft += (sender, e) =>
            {

                DialogConnect dialog = new();
                TcpClientChat? client = dialog.ConnectClient();
                if (client != null)
                    AddNewChat(client);
            };
        }

        /// <summary>
        /// Открыть прослушивание порта
        /// </summary>
        internal void OpenListener()
        {
            Listener = new();
            Listener.ListenerPendingChat += (sender, e) =>
            {
                Dispatcher.Invoke(() => AddNewChat(e));
            };
            Listener.ListenerStart();
            TokenSourceReceiveClientData.TryReset();
            TaskReceiveClientData = new(async () =>
            {
                int i;
                while (!TokenSourceReceiveClientData.IsCancellationRequested)
                {
                    for (i = 0; i < Chats.Count; i++)
                    {
                        if (Chats[i].DeviceClient.DeviceClientMessageObjInfo.Available > 0 && !Chats[i].IsBusy)
                            await Dispatcher.InvokeAsync(Chats[i].ReceiveNetworkData);
                        else await Task.Delay(100);
                    }
                }
            }, TokenSourceReceiveClientData.Token);

            TaskReceiveClientData.Start();
            IsActiveListener = true;
        }

        /// <summary>
        /// Создать новый чат
        /// </summary>
        /// <param name="client">Подключённый клиент</param>
        /// <exception cref="Exception"></exception>
        private void AddNewChat(TcpClientChat client)
        {
            if (!client.Connected)
                throw new Exception("Невозможно создать чат так как клиент не подключён");
            StackPanel HistoryMessages = new()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            StackPanel ClipFiles = new()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
            };
            OPLNetworkChat SourceNetworkChat = new()
            {
                Margin = new(3),
                FontSize = 16d,
                PaletteElement = App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Jade],
                IsEnabled = false,
            };

            Chat chat = new(ref HistoryMessages, ref ClipFiles, ref SourceNetworkChat);
            chat.AcceptConnect(client);
            Chats.Add(chat);

            SourceNetworkChat.MouseLeftButtonUp += (sender, e) =>
            {
                SelectChat?.Invoke(ref chat);
            };
            StackChats.Children.Add(SourceNetworkChat);
        }
    }
}
