using ApplicationOperPageLes.CORE;
using ApplicationOperPageLes.CORE.Animation;
using ApplicationOperPageLes.CORE.Interfaces;
using ApplicationOperPageLes.CORE.Label;
using ApplicationOperPageLes.CORE.Settings.PaletteElements;
using ApplicationOperPageLes.CORE.Settings.Struct;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.Pages.ActionPanel.PageConsole;
using ApplicationOperPageLes.UI.Pages.Browser;
using ApplicationOperPageLes.UI.UserElementsControl;
using ApplicationOperPageLes.UI.Windows;
using ApplicationOperPageLes.UI.Windows.Dialogs;
using IEL.CORE.Classes;
using IEL.CORE.Classes.Browser;
using IEL.UserElementsControl;
using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using InterpreterCommand.Classes;
using InterpreterCommand.Commands;
using LibraryPackKey.CORE;
using Microsoft.Win32;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Windows.Media.Protection.PlayReady;
using Windows.Web.Http;

namespace ApplicationOperPageLes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        #region AnimationObject
        /// <summary>
        /// Объект анимации Thickness
        /// </summary>
        internal static OPLThicknessAnimationType<ThicknessAnimation> ThicknessAnimationType =
            new(new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
            {
                DecelerationRatio = 0.6d,
                EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut },
                From = null
            });

        /// <summary>
        /// Объект анимации Double
        /// </summary>
        internal static OPLDoubleAnimationType<DoubleAnimation> DoubleAnimationType =
            new(new(0, TimeSpan.FromMilliseconds(250d))
            {
                DecelerationRatio = 0.2d,
                EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut },
                From = null
            });

        /// <summary>
        /// Объект анимации Point
        /// </summary>
        internal static OPLPointAnimationType<PointAnimation> PointAnimationType =
            new(new(new System.Windows.Point(0, 0), TimeSpan.FromMilliseconds(250d))
            {
                DecelerationRatio = 0.2d,
                EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut },
                From = null
            });

        /// <summary>
        /// Объект анимации Color
        /// </summary>
        internal static OPLColorAnimationType<ColorAnimation> ColorAnimationType =
            new(new(Colors.Black, TimeSpan.FromMilliseconds(250d))
            {
                DecelerationRatio = 0.2d,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut },
                From = null
            });

        /// <summary>
        /// Объект анимации Rect
        /// </summary>
        internal static OPLRectAnimationType<RectAnimation> RectAnimationType =
            new(new(new Rect(), TimeSpan.FromMilliseconds(250d))
            {
                DecelerationRatio = 0.8d,
                EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut },
                From = null
            });
        #endregion

        #region Data
        /// <summary>
        /// Страница буфера объектов команд
        /// </summary>
        internal BufferPagePanelAction AppPageBuffer => _AppPageBuffer ?? throw new Exception("Невозможно получить страницу буфера!");
        private BufferPagePanelAction? _AppPageBuffer;

        /// <summary>
        /// Установленный ключ валидности для приложения
        /// </summary>
        internal PackKey InstallingKey { get; private set; }

        /// <summary>
        /// Интерпретатор команд
        /// </summary>
        internal readonly COMInterpreter<IOPERCommandViewer> Interpreter;

        /// <summary>
        /// Массив ярлыков
        /// </summary>
        internal readonly List<LabelAction> DataLabels = [];

        /// <summary>
        /// Массив тегов для ярлыков
        /// </summary>
        internal readonly List<LabelTag> DataLabelTags = [];

        /// <summary>
        /// Массив всех визуализационных объектов процессов
        /// </summary>
        internal readonly List<OPLMediaViewer> DataViewerLoadingProcess = [];

        /// <summary>
        /// Страница разработчика
        /// </summary>
        internal static readonly PageDeveloper ApplicationPageDeveloper = new();
        #endregion

        #region Windows
        /// <summary>
        /// Главное окно програмы
        /// </summary>
        internal static new MainWindow MainWindow => (MainWindow)Current.MainWindow;

        /// <summary>
        /// Экземпляр созданного приложения
        /// </summary>
        internal static App CurrentApp => Current as App ?? throw new Exception("Непредвиденный перевод объекта приложения в неожидаемый тип.");

        /// <summary>
        /// Активое окно которое является дочерним от основного
        /// </summary>
        internal static Window? ActiveDialog = null;

        /// <summary>
        /// Открытые окна в приложении
        /// </summary>
        internal readonly List<Window> OpenedWindowsInApplication;
        #endregion

        /// <summary>
        /// Массив ключей настроек <b>процесса</b>
        /// </summary>
        private SettingProcess SettingApplicationProcess;

        /// <summary>
        /// Массив ключей настроек <b>приложения</b>
        /// </summary>
        internal SettingApplication SettingMainApplication { get; private set; }

        /// <summary>
        /// Палитра приложения по умолчанию
        /// </summary>
        internal Palette? DefaultPalette { get; private set; }

        /// <summary>
        /// Активная тема приложения
        /// </summary>
        internal Theme ActiveThemeApplication => _ActiveThemeApplication ?? throw new Exception("Невозможно получить тему по умолчанию!");
        private Theme? _ActiveThemeApplication;

        /// <summary>
        /// Файл настроек <b>процесса</b>
        /// </summary>
        private readonly string PathSettingProcess = StructDirectoryResources.MainDirectoryApplication + "/CurrentSettings.json";

        /// <summary>
        /// Имя файла настроек <b>приложения</b>
        /// </summary>
        private readonly string PathSettingApplication = StructDirectoryResources.MainDirectoryApplication + "/ApplicationSettings.json";

        /// <summary>
        /// Директория файла открытых настроек <b>приложения</b>
        /// </summary>
        private string ActivePathSettingApplication = string.Empty;

        /// <summary>
        /// Реальное время
        /// </summary>
        internal static DateTime RealTime => DateTime.Now;

        /// <summary>
        /// Клиент для манипуляции в сети интернет
        /// </summary>
        internal static System.Net.Http.HttpClient UsedHttpClient { get; } = new();

        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        internal static readonly ObjectConnect InternetPinging = new();

        /// <summary>
        /// Версия программы
        /// </summary>
        internal static readonly string Version = "*";

        /// <summary>
        /// Запись в файл .log
        /// </summary>
        private StreamWriter? LogStreamWriter = null;

        /// <summary>
        /// Канал воспроизведения звуков
        /// </summary>
        internal WaveOut SoundChannelWaveOut { get; }

        /// <summary>
        /// TCP сервер управления текущего устройства
        /// </summary>
        private static TcpListener? DeviceServer = null;

        /// <summary>
        /// Массив TCP клиентов подключённых к серверу
        /// </summary>
        private List<TcpClient> ServerConnectedClients;

        /// <summary>
        /// TCP клиент заявок
        /// </summary>
        private static TcpClient? DeviceClient = null;

        /// <summary>
        /// Поток данных
        /// </summary>
        NetworkStream? SourceNetWorckStream = null;

        public App()
        {
            LogWriteLine("---------- Старт нового экземпляра ----------");
            LogWriteLine("Инициализация свойств экземпляра");
            #region Resources
            SoundChannelWaveOut = new();
            OpenedWindowsInApplication = [];
            ServerConnectedClients = [];
            InstallingKey = PackKey.StaticKey;
            LogStreamWriter = StructDirectoryResources.CreateLogStreamWriter($"LOG_Access {DateTime.Now:dd.MM.yyyy}");
            //Resources.Add("DefaultMouseImage", ResourceDefaultMouseImageSetting);
            #endregion

            #region Interpreter
            LogWriteLine("Настройка интерпретатора");
            Interpreter = new([
                #region alias
                new ConsoleCommand<IOPERCommandViewer>("alias",
                [
                    new Parameter("Name", typeof(string)),
                    new Parameter("Command", typeof(string)),
                    new Parameter("Description", typeof(string), string.Empty)
                ],
                "Создаёт алиас \"Name\" на команду \"Command\". С описанием \"Description\"", (Main, param, CV) =>
                {
                    string NameAlias = ((string)param[0]).ToLower();
                    if (Interpreter?.Commands.Any((i) => i.Key.Equals(NameAlias)) ?? true)
                    {
                        return Task.FromResult(CommandStateResult.Failed(Main.Name,
                            $"Aлиас \"%//{NameAlias}//\" невозможно создать, так как название совпадает с %**консольной** командой"));
                    }
                    bool CompleteCreateAlias = Interpreter?.AddAliasCommand(NameAlias, (string)param[1], (string)param[2]) ?? false;
                    if (!CompleteCreateAlias)
                    {
                        return Task.FromResult(CommandStateResult.Failed(Main.Name,
                            $"Aлиас \"%//{NameAlias}//\" невозможно создать, так как он уже создан\n%#EA5555//Для переопределения введите команду: " +
                            "%**[alias_replace]**//"));
                    }
                    return Task.FromResult(CommandStateResult.Completed(Main.Name,
                        $"Aлиас \"%//{NameAlias}//\" на команду \"%//{param[1]}//\" успешно %**создан**"));
                }),
                #endregion

                #region alias_replace
                new ConsoleCommand<IOPERCommandViewer>("alias_replace",
                [
                    new Parameter("Name", typeof(string)),
                    new Parameter("Command", typeof(string)),
                    new Parameter("Description", typeof(string), string.Empty)
                ],
                "Изменяет алиас \"Name\" на новую команду алиаса \"Command\". С необязательным изменением описания \"Description\"", (Main, param, CV) =>
                {
                    string NameAlias = ((string)param[0]).ToLower();
                    AliasCommand<CommandOPER<IOPERCommandViewer>, IOPERCommandViewer>? alias = Interpreter?.ReadAliasCommand(NameAlias);
                    if (alias == null)
                    {
                        return Task.FromResult(CommandStateResult.Failed(Main.Name,
                            $"Aлиас \"%//{NameAlias}//\" невозможно изменить, так как он не существует \n%#EA5555//Для создания алиаса введите команду: %**alias**//"));
                    }
                    CommandOPER<IOPERCommandViewer>? Com = Interpreter?.ReadCommand((string)param[1]);
                    CommandStateResult Result = alias.ChangeSourceCommand(Com, (string)param[1], ((string)param[2]).Length > 0 ? (string)param[2] : null);
                    return Task.FromResult(CommandStateResult.Completed(Main.Name,
                        $"Aлиас \"%//{NameAlias}//\" на команду \"%//{param[1]}//\" {(Result.State == ResultState.Complete ? "успешно %**изменён**" : "невозможно %**изменить**")}"));
                }),
                #endregion

                #region label
                new ConsoleCommand<IOPERCommandViewer>("label",
                [
                    new Parameter("Name", typeof(string)), new Parameter("Command", typeof(string)),
                    new Parameter("Description", typeof(string), string.Empty)
                ],
                "Создаёт ярлык с именем \"Name\" и командой \"Command\", можно создать описание не обязательным параметром \"Description\"\n",
                (Command, param, CV) =>
                {
                    PageLabels? SourcePage = MainWindow.IELBrowserPageMain.SearchAnyPageType<PageLabels>();
                    if (SourcePage != null)
                    {
                        if (SourcePage.SelectLabelsMode) return
                            Task.FromResult(CommandStateResult.Failed(Command.Name,
                            $"%#FF7C66**Невозможно** создать ярлык \"%//{param[0]}//\", так как включён режим выделения"));
                    }
                    DataLabels.Add(new((string)param[0], (string)param[2], (string)param[1]));
                    SourcePage?.AppendNewOPLLbel(DataLabels.Count - 1);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Ярлык %#006B3C**\"{(string)param[0]}\"** успешно создан"));
                }),
                #endregion

                #region create_label
                new ConsoleCommand<IOPERCommandViewer>("create_label", "Открывает окно создания ярлыка",
                (Command, param, CV) =>
                {
                    DialogGenLabel GenLabel = new();
                    ActiveDialog = GenLabel;
                    LabelAction? label = GenLabel.CreateLabel();
                    ActiveDialog = null;
                    if (label != null)
                    {
                        PageLabels? SourcePage = MainWindow.IELBrowserPageMain.SearchAnyPageType<PageLabels>();
                        if (SourcePage != null)
                        {
                            if (SourcePage.SelectLabelsMode) return
                                Task.FromResult(CommandStateResult.Failed(Command.Name,
                                $"%#FF7C66**Невозможно** создать ярлык \"%//{param[0]}//\", так как включён режим выделения"));
                        }
                        DataLabels.Add(label);
                        SourcePage?.AppendNewOPLLbel(DataLabels.Count - 1);
                    }
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, label != null ? $"Ярлык %#006B3C**\"{label?.Name}\"** успешно создан" : null));
                }),
                #endregion

                #region reboot
                new ConsoleCommand<IOPERCommandViewer>("reboot", "Перезагружает программу", (Command, param, CV) =>
                {
                    RebootApplication();
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region close
                new ConsoleCommand<IOPERCommandViewer>("close", "Закрывает программу", (Command, param, CV) =>
                {
                    MainWindow.Close();
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region clear
                new ConsoleCommand<IOPERCommandViewer>("clear",
                "Очищает текстовый вывод главного меню программы",
                (Command, param, CV) =>
                {
                    if (MainWindow.IELBrowserPageMain.ActualInlay?.PageElement?.PageContent is PageConsole page)
                    {
                        page.StackPanelConsole.Children.Clear();
                    }
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region print
                new ConsoleCommand<IOPERCommandViewer>("print", [new Parameter("Text", typeof(string))],
                "Выводит введённый параметр \"Text\" в консоль главного меню программы, игнорируя другие параметры",
                (Command, param, CV) =>
                {
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, (string)param[0]));
                }),
                #endregion

                #region buffer
                new ConsoleCommand<IOPERCommandViewer>("buffer",
                "Отображает содержание буфера команд в консоль главного меню программы",
                (Command, param, CV) =>
                {
                    if (AppPageBuffer?.BufferCommand == null)
                        return Task.FromResult(CommandStateResult.Failed(Command.Name, "Буфер команд не подключён!"));
                    return Task.FromResult(CommandStateResult.Completed(Command.Name,
                        $"%//{AppPageBuffer.BufferCommand.Count}/{AppPageBuffer.BufferCommand.Length}://" +
                        $"%**[**{string.Join(',', AppPageBuffer.BufferCommand.BufferElements.Where((i) =>
                        {
                            if (i != null)
                            {
                                return i.Length > 0;
                            }
                            return false;
                        }))}%**]**"));
                }),
                #endregion

                #region open_link
                new ConsoleCommand<IOPERCommandViewer>("open_link", [new Parameter("Link", typeof(string))],
                "Открывает в браузере заданную ссылку \"Link\"",
                (Command, param, CV) =>
                {
                    try
                    {
                        string url = (string)param[0];
                        bool UsePageBroswer = CurrentApp.SettingMainApplication.UseOpenLinkInPageBrowser;
                        if (UsePageBroswer)
                        {
                            if (!CurrentApp.SettingMainApplication.UseOnlyCreatePageWebBrowser)
                            {
                                if (MainWindow == null) return Task.FromResult(CommandStateResult.Failed(Command.Name, $"%**Главное окно не является активным объектом**"));
                                IELInlay[] AllWebBrowsers = [..MainWindow.IELBrowserPageMain.Inlays.Where(
                                    (i) => i.PageElement?.PageContent.GetType() == typeof(PageWebBrowser))];
                                if (AllWebBrowsers.Length > 1)
                                {

                                }
                                else if (AllWebBrowsers.Length == 1)
                                {
                                    PageWebBrowser? PageBrowser = (PageWebBrowser?)AllWebBrowsers[0].PageElement?.PageContent;
                                    if (PageBrowser == null)
                                        return Task.FromResult(CommandStateResult.Failed(Command.Name, $"Не удалось открыть ссылку %#EA5555**\"{param[0]}\"**\n" +
                                            $"%//Произошла критическая ошибка обнаружения браузера.//"));
                                    PageBrowser?.WebViewGoUrl(url);
                                    MainWindow.IELBrowserPageMain.ActivateInlayInBrowserPage(AllWebBrowsers[0].PageElement);
                                    return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Открытие ссылки в странице браузера \"{url}\""));
                                }
                            }
                            BrowserPage browser_page_element = new(new PageWebBrowser(), "Веб-браузер", null);
                            browser_page_element.Disposed += (sender) =>
                            {
                                ((PageWebBrowser)browser_page_element.PageContent).WebBrowserElement.Dispose();
                            };
                            MainWindow.IELBrowserPageMain.AddInlayPage(browser_page_element);
                            ((PageWebBrowser)browser_page_element.PageContent).WebViewGoUrl(url);
                        }
                        else Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                        return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Открытие ссылки \"{url}\""));
                    }
                    catch
                    {
                        return Task.FromResult(CommandStateResult.Failed(Command.Name, $"Не удалось открыть ссылку %#EA5555**\"{param[0]}\"**"));
                    }
                }),
                #endregion

                #region open_directory
                new ConsoleCommand<IOPERCommandViewer>("open_directory",
                [
                    new Parameter("Directory", typeof(string), string.Empty)
                ],
                "Открывает заданную директорию в проводнике. При отсутствии параметра будет открывать главную страницу проводника\n" +
                "- Вписав \"*\" в параметры, откроет главную директорию процесса приложения",
                (Command, param, CV) =>
                {
                    string Text = "Открытие директории ";
                    switch ((string)param[0])
                    {
                        case "":
                            Text += "\"MAIN\"";
                            Process.Start("explorer.exe");
                            break;
                        case "*":
                            Text += "\"APPLICATION MAIN\"";
                            Process.Start("explorer.exe", Directory.GetCurrentDirectory());
                            break;
                        default:
                            if (Directory.Exists((string)param[0]))
                            {
                                string Path = (string)param[0];
                                Text += Path.Length >= 20 ? $"..\"{Path[(Path.Length - 20)..]}\"" : $"\"{Path}\"";
                                Process.Start("explorer.exe", (string)param[0]);
                                break;
                            }
                            return Task.FromResult(CommandStateResult.Failed(Command.Name, $"Директория \"{param[0]}\" не распознана"));
                    }
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, Text));
                }),
                #endregion

                #region open_file
                new ConsoleCommand<IOPERCommandViewer>("open_file",
                [
                    new Parameter("File", typeof(string))
                ],
                "Открывает файл по его заданной директории",
                (Command, param, CV) =>
                {
                    string path = (string)param[0];
                    Paragraph Message = new();
                    if (File.Exists(path))
                    {
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                        return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Открытие файла \"{Path.GetFileName(path)}\""));
                    }
                    else return Task.FromResult(
                        CommandStateResult.Failed(Command.Name, $"Файл \"{Path.GetFileName(path)}\" по данной директории не найден"));
                }),
                #endregion

                #region server_start
                new ConsoleCommand<IOPERCommandViewer>("server_start",
                [
                    new Parameter("ip", typeof(string), string.Empty),
                ],
                "Запуск сервера по ip, если нем параметра то регистрирует сервер по текущему ip компьютера",
                async (Command, param, CV) =>
                {
                    if (DeviceServer != null)
                    {
                        return CommandStateResult.Failed(Command.Name, "%__Вы уже запустили сервер__");
                    }
                    IPAddress ip = IPAddress.Parse("127.1.1.1");
                    if (((string)param[0]).Length == 0)
                    {
                        IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                        foreach (IPAddress Element in localIPs)
                        {
                            if (Element.AddressFamily.ToString().Equals("InterNetwork"))
                            {
                                ip = Element;
                                break;
                            }
                        }
                    }
                    else
                        ip = IPAddress.Parse((string)param[0]);
                    CV?.AddString($"Регистрация сервера: \"{ip.MapToIPv4()}\".");

                    #region Button CopyIP
                    IELButtonText ButtonCopyIP = new()
                    {
                        MarginViewBox = new(3),
                        FontSize = 14d,
                        Text = "Копировать IP",
                        Width = 105,
                        CornerRadius = new(5),
                        BorderThickness = new(2),
                        Margin = new(3, 0, 0, 0),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    };
                    System.Windows.Data.Binding binding = new()
                    {
                        Mode = BindingMode.OneWay,
                        Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["RussianRail G Pro"]
                    };
                    BindingOperations.SetBinding(ButtonCopyIP, IELButtonText.FontFamilyProperty, binding);
                    App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.LightBlue].ConnectPalleteFromIELElement(ButtonCopyIP);
                    ButtonCopyIP.OnActivateMouseLeft += (sender, e) =>
                    {
                        System.Windows.Clipboard.SetText(ip.MapToIPv4().ToString());
                    };
                    CV?.AddNewUIElement(ButtonCopyIP);
                    #endregion

                    DeviceServer = new(IPAddress.Any, 1111);
                    DeviceServer.Start();
                    CV?.AddString($"Сервер запущен.");
                    CV?.AddString($"Ожидаю запроса на подключение...");
                    TcpClient? SourceClient = null;
                    //int count;
                    Action WhileOperation = new(() =>
                    {
                        if (DeviceServer.Pending())
                        {
                            CV?.AddString($"Найден ожидающий клиент, принято на обработку.");
                            SourceClient = DeviceServer.AcceptTcpClient();  // ожидаем подключение клиента
                            ServerConnectedClients.Add(SourceClient);
                            SourceNetWorckStream = SourceClient.GetStream(); // для получения и отправки сообщений
                            //Server.
                            CV?.AddString($"Клиент принят: \"{(SourceNetWorckStream.Socket.RemoteEndPoint?.ToString() ?? "???")}\"");
                            if (Command.IsAsyncTokenWhileProcessEnabled)
                                Command.CloseAsyncToken();
                            else if (CV is OPLCommandViewer OPL_CV)
                                if (OPL_CV.IsTokenAsyncWhileEnabled)
                                    OPL_CV.ExitAsyncWhileOperation();
                            //CV?.AddString($"Сообщение отправлено.");
                            //ns.Write(hello, 0, hello.Length);     // отправляем сообщение
                            //do
                            //{
                            //    byte[] msg = new byte[1024];     // готовим место для принятия сообщения
                            //    count = ns.Read(msg, 0, msg.Length);   // читаем сообщение от клиента
                            //    CV?.AddString(Encoding.Default.GetString(msg, 0, count)); // выводим на экран полученное сообщение в виде строки
                            //}
                            //while (client.Connected && count > 0);
                        }
                    });
                    if (CV is OPLCommandViewer OPL_CV)
                    {
                        try { await OPL_CV.WaitWhileTaskOperation(WhileOperation, false); }
                        catch { }
                    }
                    else
                    {
                        await Command.WaitAsyncToken(WhileOperation, true);
                    }

                    return CommandStateResult.Completed(Command.Name);
                }),
                #endregion

                #region client_start
                new ConsoleCommand<IOPERCommandViewer>("client_start",
                [
                    new Parameter("ip", typeof(string)),
                ],
                "Инициализирует пользователя подключающегося к серверу",
                async (Command, param, CV) =>
                {
                    DeviceClient = new();
                    Task ConnectTask = DeviceClient.ConnectAsync((string)param[0], 1111);
                    CV?.AddString($"Произвожу попытку подключения к \"{param[0]}\"");
                    if (CV is OPLCommandViewer OPL_CV)
                    {
                        try
                        {
                            await CV.ExecuteVisualizateTask(ConnectTask, false);
                        }
                        catch
                        {
                            CV?.AddString("Не удалось подключиться к устройсту...");
                        }
                    }
                    else
                    {
                        try
                        {
                            await ConnectTask.WaitAsync(new CancellationToken(false));
                        }
                        catch
                        {
                            return CommandStateResult.Failed(Command.Name, "Не удалось создать подключение к устройству");
                        }
                    }
                    if (DeviceClient.Connected)
                    {
                        CV?.AddString("Подключение успешно!");
                        SourceNetWorckStream = DeviceClient.GetStream();

                        //while (true)
                        //{
                        //    try
                        //    {
                        //        var buffer = new byte[100];
                        //        int received = await stream.ReadAsync(buffer);

                        //        var message = Encoding.UTF8.GetString(buffer, 0, received);
                        //        CV?.AddString($"Message received: \"{message}\"");
                        //        await Task.Delay(1000);
                        //    }
                        //    catch { break; }
                        //}
                    }
                    return CommandStateResult.Completed(Command.Name);
                }),
                #endregion

                #region get_ip
                new ConsoleCommand<IOPERCommandViewer>("get_ip",
                "Отправляет \"message\" через интернет к подключённому устройству",
                (Command, param, CV) =>
                {
                    CV?.AddString("Все сетевые IP:");
                    IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                    CV?.AddString(localIPs, (el) => el.AddressFamily.ToString().Equals("InterNetwork") ? el.ToString() : null);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, $"----"));
                }),
                #endregion

                #region go_file
                new ConsoleCommand<IOPERCommandViewer>("go_file",
                [
                    new Parameter("file", typeof(string), string.Empty),
                ],
                "Отправляет \"file\" через интернет к подключённому устройству",
                async (Command, param, CV) =>
                {
                    string PathFile = (string)param[0];
                    if (PathFile.Length == 0)
                    {
                        Microsoft.Win32.OpenFileDialog dialog = new()
                        {

                        };
                        dialog.ShowDialog();
                        PathFile = dialog.FileName;
                    }
                    if (SourceNetWorckStream == null) return CommandStateResult.Failed(Command.Name, "Невозможно передать файл не имея подключения.");
                    CV?.AddString($"Начинаю процесс передачи файла: \"{PathFile}\"");
                    CV?.AddString($"Читаю данные для передачи...");
                    byte[] BytesFile = await File.ReadAllBytesAsync(PathFile);
                    CV?.AddString($"Передаю данные...");
                    if (CV != null)
                    {
                        await CV.ExecuteVisualizateTask(
                                SourceNetWorckStream.Socket.SendFileAsync(PathFile,
                                new byte[1024], new byte[1024], TransmitFileOptions.UseDefaultWorkerThread).AsTask());
                        SourceNetWorckStream.Close();
                    }
                    CV?.AddString($"Готово!");
                    return CommandStateResult.Completed(Command.Name);
                }),
                #endregion

                #region receive_file
                new ConsoleCommand<IOPERCommandViewer>("receive_file",
                "Принимает \"file\" через интернет от подключённого устройства",
                async (Command, param, CV) =>
                {
                    if (SourceNetWorckStream == null) return CommandStateResult.Failed(Command.Name, "Невозможно принять файл не имея подключения.");
                    OpenFolderDialog dialog = new()
                    {

                    };
                    dialog.ShowDialog();
                    FileStream Stream = File.OpenWrite(dialog.FolderName + $"/Oper_File_{Path.GetRandomFileName()}.download");
                    CV?.AddString($"Начинаю процесс получения файла...");
                    TextBlock BlockElement = new()
                    {
                        Text = "..."
                    };
                    CV?.AddNewUIElement(BlockElement);
                    int CountReadBytes = 1;
                    long Count = 0;
                    byte[] Buffer = new byte[16386];
                    while (CountReadBytes > 0)
                    {
                        CountReadBytes = await SourceNetWorckStream.Socket.ReceiveAsync(Buffer);
                        Count += CountReadBytes;
                        BlockElement.Text = "Получено ";
                        if (Count > 1024 && Count < 1024 * 1024) BlockElement.Text += $"{Count / 1024} Кбайт.";
                        else if (Count > 1024 * 1024) BlockElement.Text += $"{(Count / 1024) / 1024} Мбайт.";
                        if (CountReadBytes > 0) await Stream.WriteAsync(Buffer, new(false));
                    }
                    Stream.Close();
                    Stream.Dispose();
                    
                    CV?.AddString($"Готово!");
                    return CommandStateResult.Completed(Command.Name);
                }),
                #endregion

                ]);
            #endregion

            LogWriteLine("Инициализация параметров приложения");

            #region Settings
            LogWriteLine("Инициализация настроек...");
            SetSettingProcess();

            if (File.Exists(SettingApplicationProcess.PathFileApplicationSetting)) SetSettingApplication(SettingApplicationProcess.PathFileApplicationSetting);
            else if (File.Exists(PathSettingApplication)) SetSettingApplication(PathSettingApplication);
            else
            {
                SettingMainApplication = new();
                string SettingApplicationJSON = JsonConvert.SerializeObject(SettingMainApplication);
                File.WriteAllText(PathSettingApplication, SettingApplicationJSON);
                ActivePathSettingApplication = PathSettingApplication;
            }

            LogWriteLine("Установка значении на основе настроек");
            DataLabelTags = 
                [..StructDirectoryResources.DeserializeObjectJson<string>(StructDirectoryResources.DirectoryDataLabelTags).Select(Tag => new LabelTag(Tag))];

            DataLabels = [..StructDirectoryResources.DeserializeObjectJson<LabelAction>(StructDirectoryResources.DirectoryDataLabels)];
            SoundChannelWaveOut.Volume = SettingMainApplication.Volume;

            #region SettingRuntimeRealizeSettingChanges
            SettingMainApplication.PathMenuImage.Changed += (Old, New) =>
            {
                if (!Old.Equals(New)) MainWindow.UpdateImageMenu(New);
            };
            SettingMainApplication.MillisecondInternetConnection.Changed += (Old, New) =>
            {
                MainWindow.ChangeVisibilityMillisecondInternet(New);
            };
            SettingMainApplication.ExitKeyboardModeInClosePanelAction.Changed += (Old, New) =>
            {
                MainWindow.IELActionPanelMain.IsKeyboardModeExit = New;
            };
            SettingMainApplication.KEY_KeyboardModePanelAction.Changed += (Old, New) =>
            {
                MainWindow.IELActionPanelMain.KeyActivateKeyboardMode = New;
            };
            SettingMainApplication.KEY_PanelActionRightClick.Changed += (Old, New) =>
            {
                MainWindow.IELActionPanelMain.KeyKeyboardModeActivateRightClick = New;
            };
            SettingMainApplication.KEY_PanelActionClose.Changed += (Old, New) =>
            {
                MainWindow.IELActionPanelMain.KeyCloseElement = New;
            };
            SettingMainApplication.Volume.Changed += (Old, New) =>
            {
                SoundChannelWaveOut.Volume = New;
            };
            #endregion

            #region ResourcesInit
            LogWriteLine("Проверка ресурсов");
            StructDirectoryResources.CheckCreateAllResources();
            #endregion

            LogWriteLine("Успешно!");
            #endregion
        }

        /// <summary>
        /// Инициализировать окно в приложении
        /// </summary>
        /// <typeparam name="T">Тип инициализируемого окна</typeparam>
        /// <param name="SourceObject">Объект окна подлежащий инициализации</param>
        internal void InicializeWindowInApplication<T>(T SourceObject) where T : Window
        {
            OpenedWindowsInApplication.Add(SourceObject);
            SourceObject.Closed += (sender, e) =>
            {
                if (sender != null)
                    OpenedWindowsInApplication.Remove((Window)sender);
            };
        }

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="e">Объект события начала работы прораммы</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            LogWriteLine("Проверка ключа входа");
            if (File.Exists(StructDirectoryResources.DirectoryKeyValidFile))
            {
                try
                {
                    string MainPackAndValidKey = Encoding.UTF8.GetString(Convert.FromHexString(File.ReadAllText(StructDirectoryResources.DirectoryKeyValidFile)));
                    string AppGUID = RegexPackValidKey().Match(MainPackAndValidKey).Value;

                    MainPackAndValidKey = MainPackAndValidKey[(AppGUID.Length + 1)..];
                    string Pack = RegexPackValidKey().Match(MainPackAndValidKey).Value;

                    MainPackAndValidKey = MainPackAndValidKey[(Pack.Length + 1)..];
                    string Code = RegexPackValidKey().Match(MainPackAndValidKey).Value;

                    string Key = MainPackAndValidKey[(Code.Length + 1)..];

                    if (!AppGUID.Equals(GetID())) throw new Exception();
                    InstallingKey = PackKey.GenKey(StructPack.GenPack(long.Parse(Code) + 1, Pack), Key);
                } catch { }
                if (!InstallingKey.IsValid) System.Windows.Forms.MessageBox.Show("Установленный валидный ключ не подходит", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!InstallingKey.IsValid)
            {
                PackKey? key = new DialogInputProgramKey().SetKeyValid();
                if (key != null) InstallingKey = key;
            }
            if (!InstallingKey.IsValid)
            {
                Current.Shutdown();
                return;
            }
            LogWriteLine("Ключ валиден!");

            LogWriteLine("Инициализация палитры");
            DefaultPalette = new(Resources.MergedDictionaries[1]);
            _ActiveThemeApplication = new();

            LogWriteLine("Подключение связей страниц");
            _AppPageBuffer = new();
            AppPageBuffer.ConnectBuffer(new(SettingMainApplication.BufferSize));
            AppPageBuffer.IELButtonBackMainMenu.OnActivateMouseLeft += (sender, e, Key) =>
            {
                MainWindow.IELActionPanelMain.NextPageInObject(PageConsole.PageConsoleActionPanelMain, RightAlgin: false);
                e.Handled = true;
            };

            #region ConsolePage
            PageConsole.PageConsoleActionPanelMain.IELButtonCommandBuffer.OnActivateMouseLeft += (sender, e, Key) =>
            {
                MainWindow.IELActionPanelMain.NextPageInObject(AppPageBuffer);
            };
            PageConsole.PageConsoleActionPanelMain.IELButtonDiscriptionCommand.OnActivateMouseLeft += (sender, e, Key) =>
            {
                MainWindow.IELActionPanelMain.ClosePanelAction();
                //App.CurrentApp.UsingDiscriptionCommand();
            };
            PageConsole.PageConsoleActionPanelMain.IELButtonDeleteCommandViewer.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (MainWindow.IELBrowserPageMain.ActualInlay?.PageElement?.PageContent is PageConsole page)
                {
                    if (PageConsole.PageConsoleActionPanelMain.CommandViewerSelect != null)
                        page.DeleteCommandViewer(PageConsole.PageConsoleActionPanelMain.CommandViewerSelect);
                }
                MainWindow.IELActionPanelMain.ClosePanelAction();
            };
            PageConsole.PageConsoleActionPanelMain.IELButtonDeleteAllCommandViewers.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (MainWindow.IELBrowserPageMain.ActualInlay?.PageElement?.PageContent is PageConsole page)
                {
                    page.StackPanelConsole.Children.Clear();
                }
                MainWindow.IELActionPanelMain.ClosePanelAction();
            };
            #endregion

            Current.Exit += (sender, e) =>
            {
                LogWriteLine("---------- Конец текущего экземпляра ----------");
                LogStreamWriter?.Close();
            };
            Current.MainWindow = new MainWindow();
            LogWriteLine("Приминение настройки палитры");
            if (SettingMainApplication.ThemeInstallName.Value.Length > 0)
            {
                string FileTheme = $"{StructDirectoryResources.DirectoryThemeApplication}{SettingMainApplication.ThemeInstallName.Value}.qd";
                if (File.Exists(FileTheme))
                {
                    byte[] bytes = File.ReadAllBytes(FileTheme);
                    ((Palette)ActiveThemeApplication).ChangePaletteFromBytes(ref bytes);
                }
            }
            LogWriteLine("Открытие главного окна");
            try
            {
                ((MainWindow)Current.MainWindow).Show();
            }
            catch (Exception ex)
            {
                LogWriteLine($"/// ОШИБКА {ex.HResult}: {ex.Message} ///");
                LogStreamWriter?.Close();
                System.Windows.MessageBox.Show("Программа открылась неправильно!.\nПредоставлено логирование процесса...");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Перезагрузить программу
        /// </summary>
        internal void RebootApplication()
        {
            LogWriteLine("/// Старт процесса перезагрузки! ///");
            MainWindow RebootWindow = (MainWindow)Current.MainWindow;
            RebootWindow.Closed += (sender, e) =>
            {
                Current.MainWindow = new MainWindow();
                ((MainWindow)Current.MainWindow).Show();
                LogWriteLine("/// Перезагрузка прошла успешно! ///");
            };
            RebootWindow.IsReboot = true;
            RebootWindow.Close();
        }

        ///// <summary>
        ///// Загрузчик изображений через данные байтов
        ///// </summary>
        ///// <param name="imageData">Массив данных картинки</param>
        ///// <returns>Объект изображения</returns>
        ///// <exception cref="Exception">Исключение при повреждённом или пустом изображении</exception>
        //internal static BitmapImage LoadImage(byte[] imageData)
        //{
        //    if (imageData == null || imageData.Length == 0) throw new Exception("Неожиданное содержание нулевого массива байтов.");
        //    var image = new BitmapImage();
        //    using (var mem = new MemoryStream(imageData))
        //    {
        //        mem.Position = 0;
        //        image.BeginInit();
        //        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
        //        image.CacheOption = BitmapCacheOption.OnLoad;
        //        image.UriSource = null;
        //        image.StreamSource = mem;
        //        image.EndInit();
        //    }
        //    //image.Freeze();
        //    return image;
        //}

        /// <summary>
        /// Установка иконки хоста сайта через собственный клиент
        /// </summary>
        /// <param name="url">Ссылка хоста: Сама преобразуется в управляемый DNS сервер хоста</param>
        /// <returns>Картинка которая ссылается на иконку управляемого сайта</returns>
        internal static async Task<BitmapImage> DownloadFavicon(Uri url)
        {
            string faviconurl = "http://" + url.DnsSafeHost + "/favicon.ico";
            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = await UsedHttpClient.GetStreamAsync(faviconurl);
            bitmapImage.EndInit();
            return bitmapImage;
        }

        /// <summary>
        /// Записать сообщение в тектовый .log
        /// </summary>
        /// <param name="Text">Записываемый текст сообщения</param>
        /// <param name="Enclosure">Вложенность текста под отображение зависимости</param>
        internal void LogWriteLine(string Text, int Enclosure = 1) =>
            LogStreamWriter?.WriteLine($"{DateTime.Now:HH:mm:ss ff} {new string('>', Enclosure)} " + Text);

        /// <summary>
        /// Анимировать эффект блюра - сигнализируя изменение
        /// </summary>
        /// <param name="Effect">Объект эффекта анимации</param>
        /// <param name="Power">Сила блюра при старте</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        /// <param name="EnterToOriginValue">Возвратиться к текущему значению</param>
        internal static void AnimateBlurEffect(BlurEffect Effect, uint Power, double Duration = 700d, bool EnterToOriginValue = true)
        {
            DoubleAnimation animation = new()
            {
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut,
                },
                Duration = TimeSpan.FromMilliseconds(Duration),
                From = EnterToOriginValue ? Power : Effect.Radius,
                To = EnterToOriginValue ? 0d : Power
            };
            Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);
        }

        /// <summary>
        /// Получить уникальный идентификатор устройства
        /// </summary>
        /// <returns>Строка уникального идентификатора</returns>
        internal static string GetID()
        {
            ManagementObjectSearcher searcher = new("root\\CIMV2",
                   "SELECT UUID FROM Win32_ComputerSystemProduct");
            string Result = string.Empty;
            foreach (ManagementObject queryObj in searcher.Get().Cast<ManagementObject>())
                Result = queryObj["UUID"].ToString() ?? string.Empty;
            return Result;
        }

        #region Setting Manipulate
        /// <summary>
        /// Задать значение настроек процесса
        /// </summary>
        private void SetSettingProcess()
        {
            if (!File.Exists(PathSettingProcess))
            {
                SettingApplicationProcess = new();
                string SettingProcessJSON = JsonConvert.SerializeObject(SettingApplicationProcess);
                File.WriteAllText(PathSettingProcess, SettingProcessJSON);
                return;
            }
            SettingProcess Setting = JsonConvert.DeserializeObject<SettingProcess>(File.ReadAllText(PathSettingProcess));
            SettingApplicationProcess = EqualsNullPropertyInObject(Setting) ? new() : Setting;
        }

        /// <summary>
        /// Задать значение настроек приложения по директории json файла
        /// </summary>
        /// <param name="PathJsonFile">Директория файла настроек</param>
        private void SetSettingApplication(string PathJsonFile)
        {
            SettingApplication Setting;
            try
            {
                Setting = new();
                object ObjSetting = Setting;
                JObject? ObjectSetting = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(PathJsonFile));
                PropertyInfo[] properties = Setting.GetType().GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    object? Value = ObjectSetting?.GetValue(properties[i].Name)?.ToObject(properties[i].PropertyType);
                    if (Value == null) continue;
                    properties[i].SetValue(ObjSetting, Value);
                }
                Setting = (SettingApplication)ObjSetting;
            }
            catch
            {
                MessageBoxResult Result = System.Windows.MessageBox.Show($"Файл настроек \"{PathJsonFile}\" Выдал критическую ошибку.\n" +
                    $"Все настройки будут изменены на значения по умолчанию.\n" +
                    $"Разрешить редактирование данного файла для записи настроек?\n" +
                    $"ПРИМЕЧАНИЕ: При отказе редактирования файла настроек программа будет закрыта!", "Ошибка чтения настроек",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                if (Result == MessageBoxResult.No) Environment.Exit(0);
                Setting = new();
                string SettingApplicationJSON = JsonConvert.SerializeObject(Setting);
                File.WriteAllText(PathJsonFile, SettingApplicationJSON);
            }
            SettingMainApplication = Setting;
            ActivePathSettingApplication = PathJsonFile;
        }

        /// <summary>
        /// Обновить файл настроек программы
        /// </summary>
        internal void UpdateSettingApplication()
        {
            string SettingApplicationJSON = JsonConvert.SerializeObject(SettingMainApplication);
            File.WriteAllText(ActivePathSettingApplication, SettingApplicationJSON);
        }

        #endregion

        #region Labels Manipulate
        /// <summary>
        /// Обновить файл данных ярлыков
        /// </summary>
        internal void UpdateFileDataLabel()
        {
            string SettingApplicationJSON = JsonConvert.SerializeObject(DataLabels);
            File.WriteAllText(StructDirectoryResources.DirectoryDataLabels, SettingApplicationJSON);
        }

        /// <summary>
        /// Обновить файл данных тегов ярлыков
        /// </summary>
        internal void UpdateFileDataLabelTag()
        {
            string SettingApplicationJSON = JsonConvert.SerializeObject(DataLabelTags.Select(i => i.ValueTag));
            File.WriteAllText(StructDirectoryResources.DirectoryDataLabelTags, SettingApplicationJSON);
        }
        #endregion

        #region SearchNullableProperty
        /// <summary>
        /// Проверить, содержатся ли пустые поля <b>Nullable</b> в объекте
        /// </summary>
        /// <param name="Element">Проверяемый объект</param>
        /// <returns>Значение отображающее наличие пустых полей в объекте</returns>
        internal static bool EqualsNullPropertyInObject(object? Element)
        {
            if (Element == null) return true;
            return GetPropertyInfoNullPropertyInObject(Element) != null;
        }

        /// <summary>
        /// Получить значение пустого поля <b>Nullable</b> в объекте
        /// </summary>
        /// <param name="Element">Проверяемый объект</param>
        /// <returns>Возможно пустое поле</returns>
        internal static PropertyInfo? GetPropertyInfoNullPropertyInObject(object Element)
        {
            if (Element == null) return null;
            PropertyInfo[] properties = Element.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetValue(Element, null) == null) return properties[i];
            }
            return null;
        }
        #endregion

        #region CommandActivate
        /// <summary>
        /// Активировать команду
        /// </summary>
        /// <param name="CommandString">Строка команды</param>
        internal async Task ActivateActionCommand(IOPERCommandViewer? CommandView, string CommandString)
        {
            if (CommandString.Length == 0) return;
            ConsoleCommand<IOPERCommandViewer>? Command = Interpreter.ReadCommand<ConsoleCommand<IOPERCommandViewer>>(CommandString);
            string Name = COMInterpreterBase.ReadNameCommand(CommandString);
            string[] Parameters = COMInterpreterBase.ReadParametersCommand(CommandString);

            CommandStateResult result = Command == null ? CommandStateResult.FaledCommand(Name) :
                await Command.ExecuteCommand(Parameters, CommandView);
            if (result.State == ResultState.InvalidCommand)
            {
                AliasCommand<CommandOPER<IOPERCommandViewer>, IOPERCommandViewer>? Alias = Interpreter.ReadAliasCommand(CommandString);
                result = Alias == null ? CommandStateResult.FaledCommand(Name) : await Alias.ExecuteCommand(Parameters, CommandView);
            }
            if (CommandView != null) SummarizeCommandStateResult(CommandView, result);
        }

        /// <summary>
        /// Создать действие над итогом выполнения команды
        /// </summary>
        /// <param name="Result">Объект итога выполнения команды</param>
        [MTAThread()]
        internal static void SummarizeCommandStateResult(IOPERCommandViewer CommandView, CommandStateResult Result)
        {
            if (Result.State != ResultState.Complete)
            {
                MainWindow.BlurMainAnimateColor(Colors.Red);
            }
            CommandView.AddFormattedString(Result.Message);
        }

        [GeneratedRegex(@"[^ ]+")]
        private static partial Regex RegexPackValidKey();
        #endregion
    }
}
