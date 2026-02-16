using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.Windows.Dialogs;
using IEL.UserElementsControl.Base;
using Newtonsoft.Json.Linq;
using OIEL.CORE.Browser;
using OIEL.UserElementsControl;
using System.ComponentModel;
using System.Dynamic;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using OPRES = ApplicationOperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;

namespace ApplicationOperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageNetwork.xaml
    /// </summary>
    public partial class PageNetwork : PageBrowser, IDisposable
    {
        /// <summary>
        /// Объект анимации линии загрузки
        /// </summary>
        private readonly DoubleAnimation AnimationDoubleLine = new()
        {
            Duration = TimeSpan.FromSeconds(5d),
            EasingFunction = null,
            RepeatBehavior = RepeatBehavior.Forever,
            From = 0d,
            To = 40d,
        };

        /// <summary>
        /// Состояние активного открытия менеджера подключений
        /// </summary>
        private bool IsManagerConnectionActive = false;

        /// <summary>
        /// Объект истории сообщений текущего подключения
        /// </summary>
        private StackPanel StackPanelHistoryMessage;

        /// <summary>
        /// Объект отображения стека подключённых устройств
        /// </summary>
        private StackPanel StackPanelElementConnections;

        #region TCP
        /// <summary>
        /// Клиент подключения
        /// </summary>
        private TcpClient? SourceTCPClient;

        /// <summary>
        /// TCP сервер управления текущего устройства
        /// </summary>
        private TcpListener? TcpServer;

        /// <summary>
        /// Состояние активности сервера TCP
        /// </summary>
        public bool IsTcpServerEnabled { get; private set; } = false;

        /// <summary>
        /// Состояние активности прочтения сообщения
        /// </summary>
        public bool IsReceiveMessage { get; private set; } = false;

        /// <summary>
        /// Токен управления серверным асинхронным процессом прослушивания
        /// </summary>
        private CancellationTokenSource? CancellationTokenListenerServer;

        /// <summary>
        /// Серверный асинхронный процесс прослушивания
        /// </summary>
        private Task? TaskListenerServer;

        /// <summary>
        /// Токен управления асинхронным процессом проверки содержания байтов на прочтение
        /// </summary>
        private CancellationTokenSource? CancellationTokenCheckReadDataActiveSocket;

        /// <summary>
        /// Асинхронный процесс проверки содержания байтов на прочтение
        /// </summary>
        private Task? TaskCheckReadDataActiveSocket;

        /// <summary>
        /// Массив сокетов подключённых к серверу
        /// </summary>
        private List<TcpClient> ServerClientsConnected;

        /// <summary>
        /// Активное выделенное устройство между которым бкдет осуществляться передача данных
        /// </summary>
        private TcpClient? ActiveSelectClient;

        /// <summary>
        /// Активный объект интерфейса для активного выделенного устройства/сокета
        /// </summary>
        private OPLNetworkConnection? ActiveUISelectConnection;
        #endregion

        public PageNetwork()
        {
            ServerClientsConnected = [];
            InitializeComponent();
            LineTextConnection.Height = 0d;
            BorderManagerConnection.Width = 0d;
            LineTextConnection.Opacity = 0d;
            IELScrollHistoryMessage.Opacity = 0d;
            IELScrollHistoryMessage.IsEnabled = false;
            IELButtonGoSend.IsEnabled = false;
            IELButtonClip.IsEnabled = false;
            //IELTextBoxMessage.IsEnabled = false;
            IELTextBoxMessage.MaxLength = 65535;

            StackPanelHistoryMessage = new()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
            };
            IELScrollHistoryMessage.Content = StackPanelHistoryMessage;

            StackPanelElementConnections = new()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
            };
            IELScrollViewerElementConnections.Content = StackPanelElementConnections;

            #region Task
            CancellationTokenCheckReadDataActiveSocket = new();
            SolidColorBrushCheckDataReceive.Color = Colors.DarkGray;
            TaskCheckReadDataActiveSocket = new(async () =>
            {
                while (!CancellationTokenCheckReadDataActiveSocket.IsCancellationRequested)
                {
                    Dispatcher.Invoke(() => SolidColorBrushCheckDataReceive.Color = Colors.Gray);
                    await Task.Delay(100);
                    if (ActiveSelectClient != null)
                    {
                        if (ActiveSelectClient.Available > 0 && !IsReceiveMessage) // 0.01
                        {
                            try
                            {
                                byte[] MessageUTF8 = await ReceiveNetworkMessage(ActiveSelectClient);
                                await Dispatcher.InvokeAsync(() =>
                                {
                                    SolidColorBrushCheckDataReceive.Color = Colors.Green;
                                    OPLNetworkMessage NetworkMessage = new()
                                    {
                                        VerticalAlignment = VerticalAlignment.Stretch,
                                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                        Margin = new(-5, 1, 0, 1),
                                        Opacity = 0d,
                                        Message = Encoding.UTF8.GetString(MessageUTF8)
                                    };
                                    StackPanelHistoryMessage.Children.Add(NetworkMessage);
                                    App.ManagerAnimation.DoubleAnimationType.AnimateEffect(NetworkMessage, OpacityProperty,
                                        1d, TimeSpan.FromMilliseconds(400d));
                                    App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(NetworkMessage, MarginProperty,
                                        new(0, 1, 0, 1), TimeSpan.FromMilliseconds(400d));
                                });
                            }
                            catch (TimeoutException)
                            {
                                Dispatcher.Invoke(() => SolidColorBrushCheckDataReceive.Color = Colors.Red);
                                await Task.Delay(400);
                            }
                        }
                        Dispatcher.Invoke(() => SolidColorBrushCheckDataReceive.Color = Colors.Yellow);
                    }
                    else
                        Dispatcher.Invoke(() => SolidColorBrushCheckDataReceive.Color = Colors.Blue);
                    await Task.Delay(500);
                }
            }, CancellationTokenCheckReadDataActiveSocket.Token);
            #endregion

            IELButtonOpenNewConnection.OnActivateMouseLeft += async (sender, e) =>
            {
                DialogCreateTcpListener dialog = new()
                {
                    Title = "Подключение к серверу",
                    ManagerAnimation = App.ManagerAnimation,
                };
                TcpClient? tcp = dialog.OpenTcpClient();
                dialog.Dispose();
                if (tcp != null && tcp.Connected)
                {
                    SourceTCPClient = tcp;
                    OPLNetworkConnection connection = CreateConnectionUIElement(SourceTCPClient);
                    StackPanelElementConnections.Children.Add(connection);
                }
            };

            IELButtonGoSend.OnActivateMouseLeft += async (sender, e) =>
            {
                IELButtonGoSend.IsEnabled = false;
                IELButtonClip.IsEnabled = false;
                IELTextBoxMessage.IsEnabled = false;

                if (ActiveSelectClient != null)
                {
                    if (ActiveSelectClient.Connected) // 0.1
                    {
                        byte[] MessageUTF8 = Encoding.UTF8.GetBytes(IELTextBoxMessage.Text); // байты сообщения (65535 * 4)
                        byte[] BitCountSynbolsMessage = BitConverter.GetBytes((ushort)MessageUTF8.Length); // 2 байта на размер сообщения (255 * 255)
                        try
                        {
                            await ActiveSelectClient.Client.SendAsync(BitCountSynbolsMessage);
                            if (MessageUTF8.Length > ActiveSelectClient.SendBufferSize)
                            {
                                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(LineTextConnection, HeightProperty, 0d, 10d, TimeSpan.FromMilliseconds(500d));
                                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(LineTextConnection, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                                LineTextConnection.BeginAnimation(Line.StrokeDashOffsetProperty, AnimationDoubleLine);

                                int CountRepeat = MessageUTF8.Length / ActiveSelectClient.SendBufferSize;
                                int EndRemainRepeat = MessageUTF8.Length % ActiveSelectClient.SendBufferSize;
                                for (int i = 0; i < CountRepeat; i++)
                                {
                                    await ActiveSelectClient.Client.SendAsync(
                                        new ArraySegment<byte>(MessageUTF8, i * ActiveSelectClient.SendBufferSize, ActiveSelectClient.SendBufferSize));
                                    await Task.Delay(ActiveSelectClient.SendTimeout);
                                }
                                if (EndRemainRepeat > 0)
                                {
                                    await ActiveSelectClient.Client.SendAsync(
                                        new ArraySegment<byte>(MessageUTF8, CountRepeat * ActiveSelectClient.SendBufferSize, EndRemainRepeat));
                                }

                                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(LineTextConnection, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
                                LineTextConnection.BeginAnimation(Line.StrokeDashOffsetProperty, null);
                                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(LineTextConnection, Line.StrokeDashOffsetProperty,
                                    LineTextConnection.StrokeDashOffset, LineTextConnection.StrokeDashOffset - 5d, TimeSpan.FromMilliseconds(500d));
                            }
                            else
                                await ActiveSelectClient.Client.SendAsync(MessageUTF8);
                            OPLNetworkMessage NetworkMessage = new()
                            {
                                Message = IELTextBoxMessage.Text,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                                Margin = new(0, 1, -5, 1),
                                Opacity = 0d,
                                SenderTextPoint = "You",
                                //SendIndicatorSource = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Read)),
                            };
                            StackPanelHistoryMessage.Children.Add(NetworkMessage);
                            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(NetworkMessage, MarginProperty, new(0, 1, 0, 1), TimeSpan.FromMilliseconds(300d));
                            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(NetworkMessage, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                        }
                        catch (SocketException ex)
                        {
                            MessageInfo.Text = ex.Message;
                        }
                        IELButtonGoSend.IsEnabled = true;
                        IELButtonClip.IsEnabled = true;
                        IELTextBoxMessage.IsEnabled = true;
                        IELTextBoxMessage.Text = string.Empty;
                    }
                }
            };

            IELButtonManagerConnection.OnActivateMouseLeft += (sender, e) =>
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderManagerConnection, WidthProperty,
                    IsManagerConnectionActive ? 0d : 180d, TimeSpan.FromMilliseconds(500d));
                IELButtonManagerConnection.VisualGuide = IsManagerConnectionActive ?
                    IEL.CORE.Enums.StateVisualGuide.RightArrow : IEL.CORE.Enums.StateVisualGuide.LeftArrow;
                IELButtonManagerConnection.Text = IsManagerConnectionActive ? "Менеджер подключений" : "Назад";
                IsManagerConnectionActive = !IsManagerConnectionActive;

            };

            IELButtonManipulateLocalServer.OnActivateMouseLeft += (sender, e) =>
            {
                DialogCreateTcpListener dialog = new()
                {
                    Title = "Открытие сервера TCP",
                    ManagerAnimation = App.ManagerAnimation,
                };
                //App.CurrentApp.InicializeWindowInApplication(dialog);
                TcpListener? tcp = dialog.OpenTcpListener();
                if (tcp != null)
                {
                    ActivateListenerServer(ref tcp);

                    IELButtonManipulateLocalServer.IsEnabled = false;
                    IELButtonManipulateLocalServer.Text = "Сервер запущен";
                    IndicatorServer.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.ServerOn));
                }
            };

            Loaded += (sender, e) =>
            {
#warning Исправить ошибку передачи фокуса
                TaskCheckReadDataActiveSocket.Start();
            };
        }

        /// <summary>
        /// Активировать сервер TCP прослушивания устройств/подключений
        /// </summary>
        /// <param name="tcp">объект который соединяется с сервером</param>
        private void ActivateListenerServer(ref TcpListener tcp)
        {
            CancellationTokenListenerServer = new();
            TcpServer = tcp;
            SolidColorBrushCheckConnect.Color = Colors.DarkGray;
            SolidColorBrushCheckData.Color = Colors.DarkGray;
            SolidColorBrushCheckDataConnectSocket.Color = Colors.DarkGray;
            TaskListenerServer = new(async () =>
            {
                while (!CancellationTokenListenerServer.IsCancellationRequested)
                {
                    Dispatcher.Invoke(() => SolidColorBrushCheckData.Color = Colors.Gray);
                    Dispatcher.Invoke(() => SolidColorBrushCheckDataConnectSocket.Color = Colors.Gray);
                    Dispatcher.Invoke(() => SolidColorBrushCheckConnect.Color = Colors.Yellow);
                    await Task.Delay(100);
                    if (TcpServer.Pending())
                    {
                        Dispatcher.Invoke(() => SolidColorBrushCheckConnect.Color = Colors.Green);
                        TcpClient tcp = await TcpServer.AcceptTcpClientAsync();
                        tcp.ReceiveBufferSize = 10;
                        tcp.SendBufferSize = 10;
                        tcp.SendTimeout = 64;
                        if (tcp.Connected)
                        {
                            ServerClientsConnected.Add(tcp);
                            Dispatcher.Invoke(() =>
                            {
                                OPLNetworkConnection connection = CreateConnectionUIElement(tcp);
                                StackPanelElementConnections.Children.Add(connection);
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() => SolidColorBrushCheckConnect.Color = Colors.Red);
                            await Task.Delay(200);
                        }
                    }
                    Dispatcher.Invoke(() => SolidColorBrushCheckConnect.Color = Colors.Gray);
                    Dispatcher.Invoke(() => SolidColorBrushCheckDataConnectSocket.Color = Colors.Yellow);
                    Dispatcher.Invoke(() => SolidColorBrushCheckData.Color = Colors.Blue);
                    foreach (TcpClient TCPClient in ServerClientsConnected)
                    {
                        if (TCPClient.Client.Poll(10000, SelectMode.SelectError)) // 0.01
                        {
                            Dispatcher.Invoke(() => SolidColorBrushCheckData.Color = Colors.Red);
                            SolidColorBrushCheckDataConnectSocket.Color = TCPClient.Connected ? Colors.Blue : Colors.Yellow;
                            await TCPClient.Client.DisconnectAsync(TCPClient.Connected);
                            if (!TCPClient.Connected) ServerClientsConnected.Remove(TCPClient);
                            Dispatcher.Invoke(() => SolidColorBrushCheckDataConnectSocket.Color = Colors.Yellow);
                            await Task.Delay(100);
                        }
                        else if (TCPClient.Client.Poll(10000, SelectMode.SelectRead)) // 0.01
                        {
                            if (TCPClient.Connected)
                                Dispatcher.Invoke(() => SolidColorBrushCheckData.Color = Colors.Green);
                            else
                            {
                                await TCPClient.Client.DisconnectAsync(false);
                                ServerClientsConnected.Remove(TCPClient);
                                Dispatcher.Invoke(() => SolidColorBrushCheckData.Color = Colors.Red);
                                Dispatcher.Invoke(() => SolidColorBrushCheckDataConnectSocket.Color = Colors.Red);
                                await Task.Delay(100);
                                Dispatcher.Invoke(() => SolidColorBrushCheckDataConnectSocket.Color = Colors.Yellow);
                                await Task.Delay(100);
                            }
                        }
                    }
                    Dispatcher.Invoke(() => SolidColorBrushCheckDataConnectSocket.Color = Colors.Gray);
                    await Task.Delay(200);
                }
            }, CancellationTokenListenerServer.Token);
            TcpServer.Start();
            TaskListenerServer.Start();
            IsTcpServerEnabled = true;
        }

        /// <summary>
        /// Создать объект визуализации подключения
        /// </summary>
        /// <param name="tcp">Клиент который подключается</param>
        /// <returns></returns>
        private OPLNetworkConnection CreateConnectionUIElement(TcpClient tcp)
        {
            OPLNetworkConnection connection = new()
            {
                SenderTextPoint = tcp.Client.RemoteEndPoint?.ToString() ?? "???",
                Margin = new(2),
                BorderThicknessTop = 2,
            };
            connection.MouseLeftButtonUp += (sender, e) =>
            {
                if (((IELObjectBase)sender).SourceBackground.GetUsedState())
                {
                    ActiveUISelectConnection?.SourceBackground.SetUsedState(false);
                    ActiveSelectClient = null;
                    ActiveUISelectConnection = null;
                }
                else
                {
                    ActiveUISelectConnection?.SourceBackground.SetUsedState(false);
                    ActiveSelectClient = tcp;
                    ActiveUISelectConnection = connection;
                    ActiveUISelectConnection.SourceBackground.SetUsedState(true);
                }

                IELButtonGoSend.IsEnabled = ActiveUISelectConnection != null;
                IELButtonClip.IsEnabled = ActiveUISelectConnection != null;
                IELTextBoxMessage.IsEnabled = ActiveUISelectConnection != null;
                IELScrollHistoryMessage.IsEnabled = ActiveUISelectConnection != null;
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(TextBlockNoConnection, OpacityProperty,
                    ActiveUISelectConnection != null ? 0d : 0.4d, TimeSpan.FromMilliseconds(400d));
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(IELScrollHistoryMessage, OpacityProperty,
                    ActiveUISelectConnection != null ? 1d : 0d, TimeSpan.FromMilliseconds(400d));
            };
            return connection;
        }

        /// <summary>
        /// Освободить/Закрыть ресурсы объекта
        /// </summary>
        public void Dispose()
        {
            CancellationTokenListenerServer?.Cancel();
            CancellationTokenCheckReadDataActiveSocket?.Cancel();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Принять сообщение
        /// </summary>
        /// <param name="Source">Устройство отправитель</param>
        /// <returns></returns>
        private async Task<byte[]> ReceiveNetworkMessage(TcpClient Source)
        {
            IsReceiveMessage = true;
            List<byte> Data = [];
            byte[] Buffer = new byte[Source.ReceiveBufferSize];
            byte[] BufferCountSymbol = new byte[2];
            int CountReadBytes, Check;
            CountReadBytes = await Source.Client.ReceiveAsync(BufferCountSymbol);
            await Task.Delay(Source.ReceiveTimeout);

            if (!BitConverter.IsLittleEndian)
                BufferCountSymbol = [.. BufferCountSymbol.Reverse()];
            // Количество символов доступных для прочтения сообщения
            ushort CountSymbolsMessage = BitConverter.ToUInt16(BufferCountSymbol, 0);

            CountReadBytes = await Source.Client.ReceiveAsync(Buffer);
            if (CountSymbolsMessage < Source.ReceiveBufferSize)
            {
                Data.AddRange(new ArraySegment<byte>(Buffer, 0, CountSymbolsMessage));
            }
            else
            {

                do
                {
                    if (Data.Count + Buffer.Length <= CountSymbolsMessage)
                        Data.AddRange(Buffer);
                    else
                    {
                        Data.AddRange(new ArraySegment<byte>(Buffer, 0, CountSymbolsMessage - Data.Count));
                        break;
                        //Buffer = [.. Buffer.Skip(CountSymbolsMessage - Data.Count)];
                    }
                    Check = 0;
                    while (Source.Available == 0 && Check < 10 && Data.Count != CountSymbolsMessage)
                    {
                        Check++;
                        await Task.Delay(Source.ReceiveTimeout);
                    }
                    if (Source.Available == 0 && Check == 10 && Data.Count != CountSymbolsMessage)
                        throw new TimeoutException("Превышено время ожидания обработки сообщения. Сообщение потеряно.");
                    else if (Data.Count != CountSymbolsMessage)
                        CountReadBytes = await Source.Client.ReceiveAsync(Buffer);
                }
                while (CountReadBytes == Source.ReceiveBufferSize || Data.Count != CountSymbolsMessage);
            }
            IsReceiveMessage = false;
            return [.. Data];
        }
    }
}
