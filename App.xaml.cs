using CefSharp;
using CefSharp.Wpf;
using FluentFTP;
using IEL.UserElementsControl;
using Interpreter.Classes;
using Interpreter.Commands;
using InterpreterCommand.Classes;
using InterpreterCommand.Commands;
using LibraryPackKey.CORE;
using Newtonsoft.Json;
using OperPageLes.CORE;
using OperPageLes.CORE.Audio;
using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Enums.Language;
using OperPageLes.CORE.Enums.Theme;
using OperPageLes.CORE.Objects;
using OperPageLes.CORE.Settings.Struct;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel.PageConsole;
using OperPageLes.UI.Pages.Browser;
using OperPageLes.UI.Pages.Browser.BrowserPageNetwork;
using OperPageLes.UI.Pages.Browser.InlayPages;
using OperPageLes.UI.Windows;
using OperPageLes.UI.Windows.Dialogs;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Language;
using OPLAPI.CORE.Settings;
using OPLAPI.CORE.Settings.Base;
using OPLAPI.CORE.Settings.Parameters;
using OPLAPI.CORE.Themes;
using OPLAPI.OIEL.UserElementsControl;
using OPLAPI.OIEL.UserElementsControl.Base;
using OPLAPI.OIEL.UserElementsControl.Interfaces;
using Renci.SshNet;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Foundation;

namespace OperPageLes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        #region Data
        /// <summary>
        /// Реальное время
        /// </summary>
        internal static DateTime RealTime => DateTime.Now;

        /// <summary>
        /// Версия программы
        /// </summary>
        internal readonly string Version = "0.0.075";

        /// <summary>
        /// Запись в файл .log
        /// </summary>
        private static StreamWriter LogStreamWriter = StructDirectoryResources.CreateLogStreamWriter($"LOG_Access {DateTime.Now:dd.MM.yyyy}");

        /// <summary>
        /// Страница буфера объектов команд
        /// </summary>
        internal BufferPagePanelAction AppPageBuffer => _AppPageBuffer ?? throw new Exception("Невозможно получить страницу буфера!");
        private BufferPagePanelAction? _AppPageBuffer;

        #region GlobalUIElements
        #region GUIE_MainWindow
        /// <summary>
        /// Базовый элемент окна приложения
        /// </summary>
        private new OPLWindowBase MainWindow
        {
            get => (OPLWindowBase)base.MainWindow;
            set
            {
                base.MainWindow = value;
            }
        }
        /// <summary>
        /// Глобальный элемент главного окна приложения
        /// </summary>
        internal MainWindow GUIE_MainWindow
        {
            get => (MainWindow)MainWindow;
            private set
            {
                this.MainWindow = value;
            }
        }

        #region Inicialize
        /// <summary>
        /// Создать объект главного окна приложения
        /// </summary>
        private static MainWindow InicializeGUIE_MainWindow()
        {
            MainWindow Result = new()
            {
                ManagerAnimation = ManagerAnimation,
            };
            return Result;
        }
        #endregion
        #endregion

        /// <summary>
        /// Глобальный элемент менеджера анимаций под управлением приложения
        /// </summary>
        internal static readonly OPLAnimationManager ManagerAnimation = new();

        /// <summary>
        /// Глобальный элемент интерфейса панели действий
        /// </summary>
        internal static readonly IELPanelAction GUIE_PanelAction = new()
        {
            IsEnabledSettingQ = false,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            VerticalAlignment = System.Windows.VerticalAlignment.Top,
            CornerRadius = new(3d),
        };

        /// <summary>
        /// Глобальный элемент интерфейса для отображения сообщения
        /// </summary>
        internal static readonly IELBlockMessage GUIE_Message = new();

        #region GUIE_Browser
        /// <summary>
        /// Глобальный элемент интерфейса браузера страниц приложения
        /// </summary>
        internal static readonly OPLBrowserPage GUIE_Browser = InicializeGUIE_Browser();

        #region Inicialize
        /// <summary>
        /// Создать объект браузера страниц
        /// </summary>
        private static OPLBrowserPage InicializeGUIE_Browser()
        {
            OPLBrowserPage Result = new()
            {
                Margin = new(4d),
                ManagerAnimation = ManagerAnimation,
            };
            return Result;
        }
        #endregion
        #endregion
        #endregion

        #region PackKey
        /// <summary>
        /// Установленный ключ валидности для приложения
        /// </summary>
        internal PackKey InstallingKey { get; private set; } = PackKey.StaticKey(1L);
        #endregion

        #region Interpreter
        /// <summary>
        /// Интерпретатор команд
        /// </summary>
        internal readonly COMInterpreter<IOPERCommandViewer> Interpreter;
        #endregion

        #region Loading Manipulate
        /// <summary>
        /// Массив всех визуализационных объектов процессов
        /// </summary>
        private readonly List<IAsyncAction> DataLoadingProcess = [];

        /// <summary>
        /// Количество загрузочных потоков
        /// </summary>
        internal int CountLoadingProcess => DataLoadingProcess.Count;

        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки
        /// </summary>
        /// <typeparam name="T">Тип ожидаемого элемента</typeparam>
        /// <param name="Method">Асинхронный процесс получения значения</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        internal async Task<T> ExecuteVisualizateLoadingProcess<T>(Task<T> Method)
        {
            Dispatcher.Invoke(() =>
            {
                GUIE_MainWindow.TextBlockCountLoadingProcess.Text = (CountLoadingProcess + 1).ToString();
                GUIE_MainWindow.StartVisualizateLoadingProcess();
            });

            IAsyncAction AsyncActionMetod = Method.AsAsyncAction();
            DataLoadingProcess.Add(AsyncActionMetod);
            CancellationToken token = new(false);
            await Method.WaitAsync(token);

            DataLoadingProcess.Remove(AsyncActionMetod);
            AsyncActionMetod.Close();
            if (Method.IsCanceled) throw new OperationCanceledException();
            //GC.Collect(GC.GetGeneration(AsyncActionMetod));
            Dispatcher.Invoke(() =>
            {
                GUIE_MainWindow.TextBlockCountLoadingProcess.Text = CountLoadingProcess.ToString();
                if (CountLoadingProcess == 0) GUIE_MainWindow.CompleteVisualizateLoadingProcess();
            });
            return await Method;
        }

        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки без ожидаемого значения
        /// </summary>
        /// <typeparam name="T">Тип ожидаемого элемента</typeparam>
        /// <param name="Method">Асинхронный процесс получения значения</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        internal async Task ExecuteVisualizateLoadingProcess(Task Method)
        {
            Dispatcher.Invoke(() =>
            {
                GUIE_MainWindow.TextBlockCountLoadingProcess.Text = (CountLoadingProcess + 1).ToString();
                GUIE_MainWindow.StartVisualizateLoadingProcess();
            });

            IAsyncAction AsyncActionMetod = Method.AsAsyncAction();
            DataLoadingProcess.Add(AsyncActionMetod);
            CancellationToken token = new(false);
            await Method.WaitAsync(token);

            DataLoadingProcess.Remove(AsyncActionMetod);
            AsyncActionMetod.Close();
            if (Method.IsCanceled) throw new OperationCanceledException();
            //GC.Collect(GC.GetGeneration(AsyncActionMetod));
            Dispatcher.Invoke(() =>
            {
                GUIE_MainWindow.TextBlockCountLoadingProcess.Text = CountLoadingProcess.ToString();
                if (CountLoadingProcess == 0) GUIE_MainWindow.CompleteVisualizateLoadingProcess();
            });
        }
        #endregion

        #endregion

        #region Windows
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

        #region Notification
        /// <summary>
        /// Доступная коллекция для чтения всех уведомлений в приложении
        /// </summary>
        internal ReadOnlyCollection<Notification> ApplicationNotifications =>
            SourceApplicationNotifications.AsReadOnly();

        /// <summary>
        /// Уведомления приложения
        /// </summary>
        private readonly List<Notification> SourceApplicationNotifications;

        /// <summary>
        /// Событие добавления уведомления в приложение
        /// </summary>
        internal event EventHandler<Notification>? AddNotification;

        /// <summary>
        /// Событие очистки всех уведомлений в приложении
        /// </summary>
        internal event EventHandler? ClearNotification;

        /// <summary>
        /// Добавить новое уведомление в приложение
        /// </summary>
        /// <param name="SourceMessage">Сообщение уведомления</param>
        /// <param name="SourceStyle">Вид уведомления</param>
        /// <param name="SourceIcon">Иконка уведомления</param>
        /// <param name="Title">Заголовок уведомления (Если пустой то использует системный заголовок)</param>
        internal void AddNewNotification(string SourceMessage, EnumNotificationStyle SourceStyle, in ImageSource? SourceIcon = null)
        {
            Notification notification = new(SourceMessage, SourceStyle, SourceIcon);
            SourceApplicationNotifications.Add(notification);
            AddNotification?.Invoke(MainWindow, notification);
        }

        /// <summary>
        /// Удалить конкретное уведомление из приложения
        /// </summary>
        /// <param name="Source">Удаляемый элемент уведомления</param>
        internal void RemoveNotification(in Notification Source)
        {
            SourceApplicationNotifications.Remove(Source);
            if (SourceApplicationNotifications.Count == 0) ClearNotification?.Invoke(MainWindow, EventArgs.Empty);
        }

        /// <summary>
        /// Очистить все уведомления в приложении
        /// </summary>
        internal void ClearAllNotifications()
        {
            SourceApplicationNotifications.Clear();
            ClearNotification?.Invoke(MainWindow, EventArgs.Empty);
        }
        #endregion

        #region RecourceDialogPages
        /// <summary>
        /// Страница управления персанолизацией программы
        /// </summary>
        internal PageThemeController? ThemeApp { get; set; }

        #endregion

        #region Settings
        /// <summary>
        /// Словарь данных других категорий настроек
        /// </summary>
        private Dictionary<string, object[]> SourceDataAllCategorySetting = [];

        /// <summary>
        /// Словарь настроек других категорий
        /// </summary>
        internal ReadOnlyDictionary<string, object[]> DataOtherCategorySetting => SourceDataAllCategorySetting.AsReadOnly();

        /// <summary>
        /// Страница отображающая настройки приложения и компонентов
        /// </summary>
        internal readonly PageSetting PageSettingApplication;

        /// <summary>
        /// Категория настроек для общих параметров приложения
        /// </summary>
        internal readonly CategorySetting<EnumGeneralSettings> CategoryGeneralSetting;

        /// <summary>
        /// Массив ключей настроек <b>процесса</b>
        /// </summary>
        private SettingProcess SettingApplicationProcess;

        /// <summary>
        /// Имя файла настроек <b>приложения</b>
        /// </summary>
        private readonly string PathSettingApplication = StructDirectoryResources.MainDirectoryApplication + "/ApplicationSettings.json";
        #endregion

        #region AudioSettings
        /// <summary>
        /// Объект управления воспроизведением звуков
        /// </summary>
        internal readonly PlayControl SourcePlayControl;

        #endregion

        #region DialogManipulateAction
        /// <summary>
        /// Окно обработки данных
        /// </summary>
        private DialogManipulateActionWait DialogManipulateData;

        /// <summary>
        /// Массив этапов сохранения данных
        /// </summary>
        private readonly ActionManipulateData[] SaveDataActions;

        /// <summary>
        /// Массив этапов чтения данных
        /// </summary>
        private readonly ActionManipulateData[] ReadDataActions;
        #endregion

        /// <summary>
        /// Клиент для загрузки иконки сайта
        /// </summary>
        private readonly HttpClient ClientFavconLoading = new();

        #region Threads
        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        private ObjectConnect? InternetPinging;

        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        internal bool InternetConnectState => InternetPinging?.ConnectInternet ?? false;

        /// <summary>
        /// Поток обновляемый данные интернета
        /// </summary>
        private Task? TaskInternetConnection;

        /// <summary>
        /// Токен управления потоком проверки интернета
        /// </summary>
        internal CancellationToken TokenInternetConnection;

        /// <summary>
        /// Событие количества миллисекунд которое потребовалось на проверку интернета
        /// </summary>
        internal event EventHandler<ObjectConnectEventArgs>? ConnectionPingChanged;
        #endregion

#if DEBUG
        #region Testing
        /// <summary>
        /// Клиент SFTP для подключения к удалённому серверу
        /// </summary>
        SftpClient? SourceNetWorkClient = null;

        #endregion
#endif

        public App()
        {
            try
            {
                LogWriteLine("---------- Старт нового экземпляра ----------");

                LogWriteLine("Инициализация свойств экземпляра...");
                #region Resources
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                OpenedWindowsInApplication = [];
                SourceApplicationNotifications = [];
                Directory.CreateDirectory(StructDirectoryResources.DirectoryDownloadApplication);
                Canvas.SetZIndex(GUIE_PanelAction, 2);
                #endregion
                LogWriteLine("...Готово");

                LogWriteLine("Настройка интерпретатора...");
                #region Interpreter
                Interpreter = new([
                #region alias
                    new ConsoleCommand<IOPERCommandViewer>(CommandLevel.Basic, "alias",
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
                    bool CompleteCreateAlias = Interpreter?.AddAliasCommand(NameAlias, (string)param[1], (string)param[2], CommandLevel.LowLevel) ?? false;
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
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.Basic, "alias_replace",
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
                    CommandOPER<IOPERCommandViewer>? Com = Interpreter?.ReadCommand((string)param[1], CommandLevel.LowLevel);
                    CommandStateResult Result = alias.ChangeSourceCommand(Com, (string)param[1], ((string)param[2]).Length > 0 ? (string)param[2] : null);
                    return Task.FromResult(CommandStateResult.Completed(Main.Name,
                        $"Aлиас \"%//{NameAlias}//\" на команду \"%//{param[1]}//\" {(Result.State == ResultState.Complete ? "успешно %**изменён**" : "невозможно %**изменить**")}"));
                }),
                #endregion

                #region reboot
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "reboot", "Перезагружает программу", async (Command, param, CV) =>
                {
                    await RebootApplication();
                    return CommandStateResult.Completed(Command.Name);
                }),
                #endregion

                #region close
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "close", "Закрывает программу", (Command, param, CV) =>
                {
                    GUIE_MainWindow.Close();
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region clear
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.Basic, "clear",
                "Очищает текстовый вывод главного меню программы",
                (Command, param, CV) =>
                {
                    if (GUIE_Browser.ActualInlay?.Content is PageConsole page)
                    {
                        page.StackPanelConsole.Children.Clear();
                    }
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region print
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.Basic, "print", [new Parameter("Text", typeof(string))],
                "Выводит введённый параметр \"Text\" в консоль главного меню программы, игнорируя другие параметры",
                (Command, param, CV) =>
                {
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, (string)param[0]));
                }),
                #endregion

                #region buffer
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.Basic, "buffer",
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
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.Managed, "open_link", [new Parameter("Link", typeof(string))],
                "Открывает в браузере заданную ссылку \"Link\"",
                (Command, param, CV) =>
                {
#if !DEBUG
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Команда недоступна в режиме RELESE"));
#endif
                    try
                    {
                        string url = (string)param[0];
                        bool UsePageBroswer = false;//CurrentApp.SettingMainApplication.UseOpenLinkInPageBrowser;
                        if (UsePageBroswer)
                        {
                            //if (!CurrentApp.SettingMainApplication.UseOnlyCreatePageWebBrowser)
                            //{
                            //    if (MainWindow == null) return Task.FromResult(CommandStateResult.Failed(Command.Name, $"%**Главное окно не является активным объектом**"));
                            //    OPLInlay[] AllWebBrowsers = [..GUIE_MainWindow.IELBrowserPageMain.Inlays.Where(
                            //        (i) => i.Content?.GetType() == typeof(PageWebBrowser))];
                            //    if (AllWebBrowsers.Length > 1)
                            //    {

                            //    }
                            //    else if (AllWebBrowsers.Length == 1)
                            //    {
                            //        PageWebBrowser? PageBrowser = (PageWebBrowser?)AllWebBrowsers[0].Content;
                            //        if (PageBrowser == null)
                            //            return Task.FromResult(CommandStateResult.Failed(Command.Name, $"Не удалось открыть ссылку %#EA5555**\"{param[0]}\"**\n" +
                            //                $"%//Произошла критическая ошибка обнаружения браузера.//"));
                            //        else {
                            //        PageBrowser?.WebViewGoUrl(url);
                            //        GUIE_MainWindow.IELBrowserPageMain.ActivateInlayInBrowserPage(AllWebBrowsers[0].Content);
                            //        return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Открытие ссылки в странице браузера \"{url}\""));
                            //        }
                            //    }
                            //}
                            //PageBrowser browser_page_element = new(new PageWebBrowser(), "Веб-браузер", null);
                            //browser_page_element.Disposed += (sender) =>
                            //{
                            //    ((PageWebBrowser)browser_page_element.PageContent).WebBrowserElement.Dispose();
                            //};
                            //GUIE_MainWindow.IELBrowserPageMain.AddInlayPage(browser_page_element);
                            //((PageWebBrowser)browser_page_element.PageContent).WebViewGoUrl(url);
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
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.Managed, "open_directory",
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
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.Managed, "open_file",
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

                #region get_ip
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "get_ip",
                "Отправляет \"message\" через интернет к подключённому устройству",
                (Command, param, CV) =>
                {
                    CV?.AddString("Все сетевые IP:");
                    IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                    CV?.AddString(localIPs, (el) => el.AddressFamily.ToString().Equals("InterNetwork") ? el.ToString() : null);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, $"----"));
                }),
                #endregion

                #region notification
                new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "notification",
                [
                    new Parameter("Text", typeof(string)),
                ],
                "Создаёт уведомление с определённым \"Text\"",
                (Command, param, CV) =>
                {
                    AddNewNotification((string)param[0], EnumNotificationStyle.System);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                ]);
                #endregion
                LogWriteLine("...Готово");

                LogWriteLine("Инициализация настроек...");
                #region Settings
                PageSettingApplication = new()
                {
                    ManagerAnimation = ManagerAnimation
                };
                CategoryGeneralSetting = new("OPL");

                //LogWriteLine("> Установка значений на основе настроек");
                #region SettingRuntimeRealizeSettingChanges

                //SettingMainApplication.PathMenuImage.Changed += (Old, New) =>
                //{
                //    if (!Old.Equals(New)) GUIE_MainWindow.UpdateImageMenu(New);
                //};
                //SettingMainApplication.MillisecondInternetConnection.Changed += (Old, New) =>
                //{
                //    GUIE_MainWindow.ChangeVisibilityMillisecondInternet(New);
                //};
                //SettingMainApplication.ExitKeyboardModeInClosePanelAction.Changed += (Old, New) =>
                //{
                //    GUIE_MainWindow.IELActionPanelMain.IsKeyboardModeExit = New;
                //};
                //SettingMainApplication.KEY_KeyboardModePanelAction.Changed += (Old, New) =>
                //{
                //    GUIE_MainWindow.IELActionPanelMain.KeyActivateKeyboardMode = New;
                //};
                //SettingMainApplication.KEY_PanelActionRightClick.Changed += (Old, New) =>
                //{
                //    GUIE_MainWindow.IELActionPanelMain.KeyKeyboardModeActivateRightClick = New;
                //};
                //SettingMainApplication.KEY_PanelActionClose.Changed += (Old, New) =>
                //{
                //    GUIE_MainWindow.IELActionPanelMain.KeyCloseElement = New;
                //};
                #endregion
                //LogWriteLine("> ...Готово");

                #endregion
                LogWriteLine("...Готово");

                LogWriteLine($"Настройка \"{nameof(DialogManipulateData)}\"");
                #region DialogManipulateData
                DialogManipulateData = new()
                {
                    ManagerAnimation = ManagerAnimation,
                };
                SaveDataActions =
                [
                    #region DataSaveActions
                    new("Закрытие всех окон приложения", 300d)
                    {
                        #region Action
                        OriginAction = () =>
                        {
                            int count = OpenedWindowsInApplication.Count;
                            for (int i = 0; i < count; i++)
                            {
                                OpenedWindowsInApplication[0].Close();
                                Thread.Sleep(10);
                            }
                        },
                        #endregion
                    },
                    new("Сохранение настроек приложения", 300d)
                    {
                        #region Action
                        OriginAction = () =>
                        {
                            WriteSettingApplication(PathSettingApplication);
                        },
                        #endregion
                    },
                    new("Сохраняются все ярлыки", 300d)
                    {
                        #region Action
                        OriginAction = () =>
                        {
                            PageManagerAppPage AppPage = (PageManagerAppPage?)GUIE_Browser.SourceManagerAppPage ??
                                throw new Exception("Главная страница браузера не инициализирована!");
                            string SettingApplicationJSON = JsonConvert.SerializeObject(AppPage.Labels.Select((i) => i.Label));
                            File.WriteAllText(StructDirectoryResources.DirectoryDataLabels, SettingApplicationJSON);
                        },
                        #endregion
                    }
                    #endregion
                ];
                ReadDataActions =
                [
                    new("Инициализация системных страничных приложений", 1600d)
                    {
                        #region Action
                        OriginAction = async () =>
                        {
                            await GUIE_Browser.AddNewAppPage(typeof(PageConsole));
                            await GUIE_Browser.AddNewAppPage(typeof(PageNetwork));
                            await GUIE_Browser.AddNewAppPage(typeof(PageWebBrowser));
                            await GUIE_Browser.AddNewAppPage(typeof(PageDeveloper));
                        },
                        #endregion
                    }
                ];
                #endregion
                LogWriteLine("...Готово");

                LogWriteLine("Изучение звуковых данных...");
                SourcePlayControl = new();
                LogWriteLine("...Готово");

                LogWriteLine("Проверка ресурсов...");
                #region ResourcesInit
                StructDirectoryResources.CheckCreateAllResources();
                #endregion
                LogWriteLine("...Готово");

                #region WebBrowser
                CefSettings settings = new();
                settings.CefCommandLineArgs.Add("enable-media-stream", "1");
                settings.CefCommandLineArgs.Add("allow-running-insecure-content", "1");
                settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream", "1");
                settings.CefCommandLineArgs.Add("enable-speech-input", "1");
                settings.CefCommandLineArgs.Add("enable-usermedia-screen-capture", "1");
                settings.CefCommandLineArgs.Add("debug-plugin-loading", "1");
                settings.CefCommandLineArgs.Add("allow-outdated-plugins", "1");
                settings.CefCommandLineArgs.Add("always-authorize-plugins", "1");
                settings.CefCommandLineArgs.Add("disable-web-security", "1");
                settings.CefCommandLineArgs.Add("enable-npapi", "1");
                Cef.Initialize(settings);
                #endregion

                LogWriteLine("! Инициализация экземпляра успешна");
            }
            catch (Exception ex)
            {
                LogWriteLine($"/// ОШИБКА {ex.HResult}: {ex.Message} ///");
                LogWriteLine($"/// Трассировка стека: ///\n{ex.StackTrace}");
                System.Windows.MessageBox.Show("Программа проинициализирована не правильно!.\nПредоставлено логирование процесса...");
                LogStreamWriter.Close();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="e">Объект события начала работы прораммы</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            LogWriteLine(" ** ");
            try
            {
                #region OnStartup
                LogWriteLine($"Настройка \"{nameof(Theme)}\"");
                #region
                Theme.SetSelectEnumSpectrumType(Assembly.GetExecutingAssembly(), nameof(PaletteEnum));
                //PageManagerAppPage.PageLabelActionPanel.SetVisualTheme(in _ActiveThemeApplication);
                #endregion
                LogWriteLine("...Готово");

                LogWriteLine("Подключение связей страниц");
                _AppPageBuffer = new();
                AppPageBuffer.ConnectBuffer(new(50)); // SettingMainApplication.BufferSize
                AppPageBuffer.IELButtonBackMainMenu.OnActivateMouseLeft += (sender, e, Key) =>
                {
                    GUIE_PanelAction.NextPageInObject(PageConsole.PageConsoleActionPanelMain, RightAlgin: false);
                    //e.Handled = true;
                };
                LogWriteLine("...Готово");

                LogWriteLine("Обработка настроек");
                #region Settings
                Setting.AppendCategory += PageSettingApplication.HandlerAppendCategory;
                Setting.AddCategory(CategoryGeneralSetting);

                // Добавление параметров в общую категорию настроек приложения
                #region Setting Parameters
                ParameterSettingBase Parameter;
                #region VisualMillisecondConnect
                Parameter = new ParameterSetting<bool>(false);
                Parameter.ConnectLangParameters(LangParameterValue.Name,
                    LangSettingGeneralUITranslate.ParameterName_VisibleConnectInternetMillisecond);
                Parameter.ConnectLangParameters(LangParameterValue.Description,
                    LangSettingGeneralUITranslate.ParameterName_VisibleConnectInternetMillisecond);
                CategoryGeneralSetting.AddParameter(EnumGeneralSettings.VisualMillisecondConnect, Parameter);
                #endregion

                #region BufferLength
                Parameter = new LimitedParameterIntSetting(0, 100, 50);
                //Parameter.ConnectLangParameters(LangParameterValue.Name, LangSettingGeneralUITranslate.ParameterName_VisibleConnectInternetMillisecond);
                CategoryGeneralSetting.AddParameter(EnumGeneralSettings.BufferLength, Parameter);
                #endregion
                #endregion
                #endregion
                LogWriteLine("...Готово");

                #region DebugCommands
#if !DEBUG
                LogWriteLine("Пропустк добавления отладочных $ команд");
#endif
#if DEBUG
                LogWriteLine("Добавление отладочных $ команд");
                AddNewNotification(
                    "У вас есть доступ к отладочному режиму программы\n" +
                    "Все команды начинающиеся с '$' доступны,\n" +
                    "использование их может повлечь за собой непредсказуемое поведение",
                    EnumNotificationStyle.System);
                #region Console
                #region $server_connect
                Interpreter.AddCommand(new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "$server_connect",
                "Подключиться к удалённому серверу", (Main, param, CV) =>
                {
                    if (SourceNetWorkClient != null)
                    {
                        if (SourceNetWorkClient.IsConnected)
                            return Task.FromResult(CommandStateResult.Completed(Main.Name, "Уже имеется активное подключение к серверу"));
                        SourceNetWorkClient.Dispose();
                    }
                    FtpConfig config = new()
                    {
                        ConnectTimeout = 1000, // Тайм-аут подключения: 30 секунд
                    };
                    SourceNetWorkClient = new("193.42.125.40", 6000, "UserCocaga", "1234567890");
                    CancellationTokenSource TokenConnect = new();
                    TextBlock TextConnect = new()
                    {
                        Text = "Подключение к серверу...",
                        FontSize = 16d,
                        Foreground = new SolidColorBrush(Colors.Gold),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        Opacity = 0d,
                    };
                    CV?.AddNewUIElement(TextConnect);
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextConnect, TextBlock.OpacityProperty,
                        1d, TimeSpan.FromMilliseconds(500));
                    TextConnect.Text = "Происходит подключение к серверу...";
                    try
                    {
                        SourceNetWorkClient.Connect();
                        TextConnect.Text = "Подключение к серверу успешно!";
                    }
                    catch (Exception ex)
                    {
                        TextConnect.Foreground = new SolidColorBrush(Colors.Red);
                        TextConnect.Text = "Не удалось подключиться к серверу!";
                        CV?.AddString($"Ошибка подключения к серверу:\n{ex.Message}");
                    }
                    OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, TextConnect, TextBlock.MarginProperty,
                        new Thickness(14d, 0d, 0d, 0d), new Thickness(0d), TimeSpan.FromMilliseconds(1000));
                    return Task.FromResult(CommandStateResult.Completed(Main.Name));
                }));
                #endregion

                #region $server_disconnect
                Interpreter.AddCommand(new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "$server_disconnect",
                "Отключиться от удалённого сервера", (Main, param, CV) =>
                {
                    if (SourceNetWorkClient != null)
                    {
                        if (SourceNetWorkClient.IsConnected)
                            SourceNetWorkClient.Disconnect();
                        SourceNetWorkClient.Dispose();
                        SourceNetWorkClient = null;
                        return Task.FromResult(CommandStateResult.Completed(Main.Name, "Успешно отключено от сервера!"));
                    }
                    else
                        return Task.FromResult(CommandStateResult.Completed(Main.Name, "Нет активного подключения к серверу"));
                }));
                #endregion

                #region $category_rename
                Interpreter.AddCommand(new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "$category_rename",
                [new Parameter("Name", typeof(string))],
                "Переименовать общую категорию настроек",
                (Command, param, CV) =>
                {
                    string Name = (string)param[0];
                    CategoryGeneralSetting.NameCategory = Name;
                    return Task.FromResult(CommandStateResult.Failed(Command.Name, "Успешно"));
                }));
                #endregion

                #region $parameter_rename
                Interpreter.AddCommand(new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "$parameter_rename",
                [new Parameter("Name", typeof(string))],
                "Переименовать параметр в общей категории настроек",
                (Command, param, CV) =>
                {
                    string Name = (string)param[0];
                    ((ParameterSetting<bool>)CategoryGeneralSetting[EnumGeneralSettings.VisualMillisecondConnect]).ParameterName = Name;
                    return Task.FromResult(CommandStateResult.Failed(Command.Name, "Успешно"));
                }));
                #endregion

                #region $lang_title_rename
                Interpreter.AddCommand(new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "$lang_title_rename",
                [new Parameter("Name", typeof(string))],
                "Переименовать параметр заголовка главного окна, языкового перевода",
                (Command, param, CV) =>
                {
                    string Name = (string)param[0];
                    //Lang.UpdateDictionaryParameterLang(LangUITranslate.MainWindowTitle, Name);
                    return Task.FromResult(CommandStateResult.Failed(Command.Name, "Успешно"));
                }));
                #endregion

                #region $p_i
                Interpreter.AddCommand(new ConsoleCommand<IOPERCommandViewer>(CommandLevel.LowLevel, "$p_i",
                "Изменить значение параметра отображения отклика интернета на противоположное", (Main, param, CV) =>
                {
                    ParameterSetting<bool> Parameter =
                        (ParameterSetting<bool>)CategoryGeneralSetting[EnumGeneralSettings.VisualMillisecondConnect];
                    Parameter.Value = !Parameter.Value;
                    return Task.FromResult(CommandStateResult.Completed(Main.Name));
                }));
                #endregion
                #endregion
#endif
                LogWriteLine("...Готово");
                #endregion

                LogWriteLine("Запуск фоновых потоков");
                #region ThreadUptadeRuntime
                InternetPinging = new();
                TokenInternetConnection = new();
                TaskInternetConnection = new(() =>
                {
                    ObjectConnectEventArgs EventArgs;
                    while (!TokenInternetConnection.IsCancellationRequested)
                    {
                        EventArgs = InternetPinging.UpdateInternetConnection();
                        Dispatcher.Invoke(() => ConnectionPingChanged?.Invoke(null, EventArgs));
                        Thread.Sleep(4000);
                    }
                }, TokenInternetConnection);
                TaskInternetConnection.Start();
                #endregion
                LogWriteLine("...Готово");

                #region ConsolePage
                PageConsole.PageConsoleActionPanelMain.IELButtonCommandBuffer.OnActivateMouseLeft += (sender, e, Key) =>
                {
                    GUIE_PanelAction.NextPageInObject(AppPageBuffer);
                };
                PageConsole.PageConsoleActionPanelMain.IELButtonDeleteCommandViewer.OnActivateMouseLeft += (sender, e, Key) =>
                {
                    if (GUIE_Browser.ActualInlay?.Content is PageConsole page)
                    {
                        if (PageConsole.PageConsoleActionPanelMain.CommandViewerSelect != null)
                            page.DeleteCommandViewer(PageConsole.PageConsoleActionPanelMain.CommandViewerSelect);
                    }
                    GUIE_PanelAction.ClosePanelAction();
                };
                PageConsole.PageConsoleActionPanelMain.IELButtonDeleteAllCommandViewers.OnActivateMouseLeft += (sender, e, Key) =>
                {
                    if (GUIE_Browser.ActualInlay?.Content is PageConsole page)
                    {
                        page.StackPanelConsole.Children.Clear();
                    }
                    GUIE_PanelAction.ClosePanelAction();
                };
                #endregion

                Current.Exit += (sender, e) =>
                {
                    LogWriteLine("---------- Конец текущего экземпляра ----------");
                    LogStreamWriter.Close();
                };
                base.OnStartup(e);

                LogWriteLine("Установка встроенного языкового перевода");
                #region Language
                Lang.UpdateLang(Lang.GetLangFromLocate("Russian") ??
                    throw new Exception("Не удалось найти предустановленный языковой перевод \"Russian\""));
                #endregion
                LogWriteLine("...Готово");

                LogWriteLine("Проверка ключа входа");
                #region KeyValid
                if (File.Exists(StructDirectoryResources.DirectoryKeyValidFile))
                {
                    try
                    {
                        PackKey? SourceGenKey = PackKey.GenKey(File.ReadAllBytes(StructDirectoryResources.DirectoryKeyValidFile), GetID());
                        if (SourceGenKey != null)
                            InstallingKey = SourceGenKey;
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("Установленный валидный ключ не подходит", "Предупреждение",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (!InstallingKey.IsValid)
                {
                    LogWriteLine("Открытие диалога ввода ключа");
                    DialogInputProgramKey dialog = new();
                    LogWriteLine($"Менеджер анимаций: ({ManagerAnimation != null})");
                    PackKey? key = dialog.SetKeyValid(ManagerAnimation);
                    if (key != null)
                    {
                        InstallingKey = key;
                        File.WriteAllBytes(StructDirectoryResources.DirectoryKeyValidFile,
                            InstallingKey.GetHexDataKeyFromID(GetID()));
                    }
                }
                if (!InstallingKey.IsValid)
                {
                    Current.Shutdown();
                    return;
                }
                #endregion
                LogWriteLine("Ключ валиден!");

                LogWriteLine($"Настройка \"{nameof(GUIE_MainWindow)}\"");
                #region GUIE_MainWindow
                GUIE_MainWindow = InicializeGUIE_MainWindow();
                #endregion
                LogWriteLine("...Готово");

                LogWriteLine($"Настройка \"{nameof(GUIE_Browser)}\"");
                GUIE_Browser.GenerateNewMainManagerAppPage(typeof(PageManagerAppPage));
                GUIE_Browser.SourceManagerAppPage?.SourcePanelAction = GUIE_PanelAction;
                LogWriteLine("...Готово");

                LogWriteLine($"Настройка \"{nameof(Lang)}\"");
                #region Lang
                Lang.LanguageUpdated += Lang_LanguageUpdated;
                Lang_LanguageUpdated(null, EventArgs.Empty);
                #endregion
                LogWriteLine("...Готово");

                LogWriteLine("Обновление настроек от файла");
                #region SettingInstall
                if (File.Exists(SettingApplicationProcess.PathFileApplicationSetting))
                    SetSettingApplication(SettingApplicationProcess.PathFileApplicationSetting);
                else if (File.Exists(PathSettingApplication)) SetSettingApplication(PathSettingApplication);
                else
                {
                    var result = new Dictionary<string, object[]>
                    {
                        { CategoryGeneralSetting.KeyCategory, [..CategoryGeneralSetting.Parameters.Select((i) => i.Value.Value)] }
                    };

                    string json = JsonConvert.SerializeObject(result);
                    File.WriteAllText(PathSettingApplication, json);
                }
                #endregion
                LogWriteLine("...Готово");

                LogWriteLine("Открытие главного окна");
                #region MainWindowShow
                Dispatcher.BeginInvoke(async () =>
                {
                    DialogManipulateData.TitleHead = Lang.GetValue(LangManipulateDataTranslate.ReadImportantData);
                    await DialogManipulateData.ActivateVisualManipulate(ReadDataActions);
                    GUIE_MainWindow.Topmost = true;
                    await GUIE_MainWindow.Show();
                    GUIE_MainWindow.Topmost = false;
                });
                #endregion
                LogWriteLine("...Готово");
                #endregion
            }
            catch (Exception ex)
            {
                LogWriteLine($"/// ОШИБКА {ex.HResult}: {ex.Message} ///");
                LogWriteLine($"/// Трассировка стека: ///\n{ex.StackTrace}");
                LogStreamWriter.Close();
                System.Windows.MessageBox.Show("Программа открылась неправильно!.\nПредоставлено логирование процесса...");
                Environment.Exit(1);
            }
        }

        #region Handler reboot and close application
        /// <summary>
        /// Закрыть программу
        /// </summary>
        internal async Task CloseApplication(bool Exit = true)
        {
            if (Exit) LogWriteLine("/// Начало закрытия экземпляра! ///");
            #region Close
            GUIE_MainWindow.Close();
            DialogManipulateData.TitleHead = Lang.GetValue(LangManipulateDataTranslate.SaveImportantData);
            await DialogManipulateData.ActivateVisualManipulate(SaveDataActions);
            #endregion
            if (Exit)
            {
                LogWriteLine("/// Экземпляр закрыт! ///");
                LogStreamWriter.Close();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Перезагрузить программу
        /// </summary>
        internal async Task RebootApplication()
        {
            LogWriteLine("/// Начало перезагрузки экземпляра! ///");
            #region Reboot
            await CloseApplication(false);

            (GUIE_Message.Parent as System.Windows.Controls.Panel)?.Children.Remove(GUIE_Message);
            (GUIE_Browser.Parent as System.Windows.Controls.Panel)?.Children.Remove(GUIE_Browser);
            (GUIE_PanelAction.Parent as System.Windows.Controls.Panel)?.Children.Remove(GUIE_PanelAction);

            GUIE_MainWindow = InicializeGUIE_MainWindow();
            GUIE_MainWindow.Topmost = true;
            await GUIE_MainWindow.Show();
            GUIE_MainWindow.Topmost = false;
            #endregion
            LogWriteLine("/// Экземпляр перезагружен! ///");
        }
        #endregion

        #region LOG
        /// <summary>
        /// Записать сообщение в тектовый .log
        /// </summary>
        /// <param name="Text">Записываемый текст сообщения</param>
        /// <param name="Enclosure">Вложенность текста под отображение зависимости</param>
        internal static void LogWriteLine(string Text, int Enclosure = 1) =>
            LogStreamWriter.WriteLine($"{DateTime.Now:HH:mm:ss ff} {new string('>', Enclosure)} " + Text);
        #endregion

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
        /// Задать значение настроек приложения по директории json файла
        /// </summary>
        /// <param name="PathJsonFile">Директория файла настроек</param>
        private void SetSettingApplication(string PathJsonFile)
        {
            SourceDataAllCategorySetting =
                JsonConvert.DeserializeObject<Dictionary<string, object[]>>(File.ReadAllText(PathJsonFile)) ??
                throw new Exception("Неудалось десериализовать объект в список данных настроек для категорий");
            try
            {
                if (SourceDataAllCategorySetting.TryGetValue("OPL", out object[]? value))
                    Setting.SetAllParametersInCategory("OPL", value);
            }
            catch
            {
                MessageBoxResult Result = System.Windows.MessageBox.Show($"Файл настроек \"{PathJsonFile}\" Выдал критическую ошибку.\n" +
                    $"Все настройки будут изменены на значения по умолчанию.\n" +
                    $"Разрешить редактирование данного файла для записи настроек?\n" +
                    $"ПРИМЕЧАНИЕ: При отказе редактирования файла настроек программа будет закрыта!", "Ошибка чтения настроек",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                if (Result == MessageBoxResult.No) Environment.Exit(0);
                //Setting = new();
                //string SettingApplicationJSON = JsonConvert.SerializeObject(Setting);
                //File.WriteAllText(PathJsonFile, SettingApplicationJSON);
            }
            //SettingMainApplication = Setting;
        }

        /// <summary>
        /// Обновить файл настроек программы
        /// </summary>
        /// <param name="PathJsonFile">Директория файла настроек</param>
        internal void WriteSettingApplication(string PathJsonFile)
        {
            foreach (string Key in SourceDataAllCategorySetting.Keys)
            {
                CategorySettingBase Category = Setting.GetCategory(Key);
                for (uint i = 0u; i < SourceDataAllCategorySetting[Key].Length; i++)
                    SourceDataAllCategorySetting[Key][i] = Category.GetParameter(i).Value;
            }

            string SettingApplicationJSON = JsonConvert.SerializeObject(SourceDataAllCategorySetting);
            File.WriteAllText(PathJsonFile, SettingApplicationJSON);
        }

        #endregion

        /// <summary>
        /// Установка иконки хоста сайта через собственный клиент
        /// </summary>
        /// <param name="url">Ссылка хоста: Сама преобразуется в управляемый DNS сервер хоста</param>
        /// <returns>Картинка которая ссылается на иконку управляемого сайта</returns>
        internal async Task<BitmapImage> DownloadFavicon(Uri url)
        {
            string faviconurl = "http://" + url.DnsSafeHost + "/favicon.ico";
            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = await ClientFavconLoading.GetStreamAsync(faviconurl);
            bitmapImage.EndInit();
            return bitmapImage;
        }

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
            ConsoleCommand<IOPERCommandViewer>? Command = Interpreter.ReadCommand<ConsoleCommand<IOPERCommandViewer>>(CommandString, CommandLevel.LowLevel);
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
        internal void SummarizeCommandStateResult(IOPERCommandViewer CommandView, CommandStateResult Result)
        {
            if (Result.State != ResultState.Complete)
            {
                GUIE_MainWindow.BlurMainAnimateColor(Colors.Red);
            }
            CommandView.AddFormattedString(Result.Message);
        }
        #endregion

        #region Language
        /// <summary>
        /// Обработчик события обновления языкового перевода
        /// </summary>
        private void Lang_LanguageUpdated(object? sender, EventArgs e)
        {
            CategoryGeneralSetting.NameCategory = Lang.GetValue(LangSettingGeneralUITranslate.GeneralTitle);
        }
        #endregion
    }
}
