using IEL.CORE.Classes;
using IEL.CORE.Enums;
using IEL.UserElementsControl;
using Interpreter.Classes;
using Interpreter.Commands;
using InterpreterCommand.Classes;
using InterpreterCommand.Commands;
using LibraryPackKey.CORE;
using Microsoft.Win32;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OIEL.CORE.Browser;
using OIEL.UserElementsControl;
using OIEL.UserElementsControl.Base.LabelBase;
using OIEL.UserElementsControl.Interfaces;
using OperPageLes.CORE;
using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Objects;
using OperPageLes.CORE.Settings.PaletteElements;
using OperPageLes.CORE.Settings.Struct;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel.PageConsole;
using OperPageLes.UI.Pages.Browser;
using OperPageLes.UI.Pages.Browser.BrowserPageNetwork;
using OperPageLes.UI.Windows;
using OperPageLes.UI.Windows.Dialogs;
using OPLAnimation.CORE.Animation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace OperPageLes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        /// <summary>
        /// Менеджер анимаций под управлением приложения
        /// </summary>
        internal static readonly OPLAnimationManager ManagerAnimation = new();

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
        internal static readonly PageDeveloper ApplicationPageDeveloper = new()
        {
            Title = "Страница разработчика",
            Description = "Используйте только если знаете что делаете!"
        };
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
        /// Добавить новое уведомление в приложение
        /// </summary>
        /// <param name="SourceMessage">Сообщение уведомления</param>
        /// <param name="SourceStyle">Вид уведомления</param>
        /// <param name="SourceIcon">Иконка уведомления</param>
        /// <param name="Title">Заголовок уведомления (Если пустой то использует системный заголовок)</param>
        internal void AddNewNotification(string SourceMessage, EnumNotificationStyle SourceStyle, in ImageSource? SourceIcon = null, string? Title = null)
        {
            Notification notification = Title == null ?
                new(SourceMessage, SourceStyle, SourceIcon) : new(SourceMessage, Title, SourceStyle, SourceIcon);
            SourceApplicationNotifications.Add(notification);
            AddNotification?.Invoke(MainWindow, notification);
        }

        /// <summary>
        /// Удалить конкретное уведомление из приложения
        /// </summary>
        /// <param name="Source">Удаляемый элемент уведомления</param>
        internal void RemoveNotification(in Notification Source) =>
            SourceApplicationNotifications.Remove(Source);

        /// <summary>
        /// Очистить все уведомления в приложении
        /// </summary>
        internal void ClearAllNotifications() => SourceApplicationNotifications.Clear();
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
        /// Поток данных
        /// </summary>
        NetworkStream? SourceNetWorckStream = null;

        #region Threads
        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        private ObjectConnect? InternetPinging;

        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        internal bool InternetConnectState = false;

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
        internal static event EventHandler<ObjectConnectEventArgs>? ConnectionPingChanged;
        #endregion


        #region Browser
        /// <summary>
        /// Браузер страниц приложения
        /// </summary>
        internal readonly OPLBrowserPage MainBrowser;

        /// <summary>
        /// Страница выбора приложения страницы для усправления в браузере страниц
        /// </summary>
        private readonly PageManagerAppPage SourceManagerAppPage;

        /// <summary>
        /// Страница менеджера приложений страниц
        /// </summary>
        internal Page ManagerAppPage => SourceManagerAppPage;
        #endregion

        public App()
        {
            LogWriteLine("---------- Старт нового экземпляра ----------");
            LogWriteLine("Инициализация свойств экземпляра");
            #region Resources
            SourceManagerAppPage = new();
            MainBrowser = new(SourceManagerAppPage);
            SoundChannelWaveOut = new();
            OpenedWindowsInApplication = [];
            SourceApplicationNotifications = [];
            InstallingKey = PackKey.StaticKey;
            LogStreamWriter = StructDirectoryResources.CreateLogStreamWriter($"LOG_Access {DateTime.Now:dd.MM.yyyy}");
            //Resources.Add("DefaultMouseImage", ResourceDefaultMouseImageSetting);
            Directory.CreateDirectory(StructDirectoryResources.DirectoryDownloadApplication);
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
                    PageLabels? SourcePage = MainBrowser.SearchAnyPageType<PageLabels>();
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
                        PageLabels? SourcePage = MainBrowser.SearchAnyPageType<PageLabels>();
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
                    if (MainBrowser.ActualInlay?.Content is PageConsole page)
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
#if !DEBUG
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Открытие ссылки \"{url}\""));
#endif
                    try
                    {
                        string url = (string)param[0];
                        bool UsePageBroswer = CurrentApp.SettingMainApplication.UseOpenLinkInPageBrowser;
                        if (UsePageBroswer)
                        {
                            //if (!CurrentApp.SettingMainApplication.UseOnlyCreatePageWebBrowser)
                            //{
                            //    if (MainWindow == null) return Task.FromResult(CommandStateResult.Failed(Command.Name, $"%**Главное окно не является активным объектом**"));
                            //    OPLInlay[] AllWebBrowsers = [..MainWindow.IELBrowserPageMain.Inlays.Where(
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
                            //        MainWindow.IELBrowserPageMain.ActivateInlayInBrowserPage(AllWebBrowsers[0].Content);
                            //        return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Открытие ссылки в странице браузера \"{url}\""));
                            //        }
                            //    }
                            //}
                            //PageBrowser browser_page_element = new(new PageWebBrowser(), "Веб-браузер", null);
                            //browser_page_element.Disposed += (sender) =>
                            //{
                            //    ((PageWebBrowser)browser_page_element.PageContent).WebBrowserElement.Dispose();
                            //};
                            //MainWindow.IELBrowserPageMain.AddInlayPage(browser_page_element);
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

                #region notification
                new ConsoleCommand<IOPERCommandViewer>("notification",
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
                }
                catch { }
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
                //e.Handled = true;
            };

            LogWriteLine("Запуск фоновых потоков");
            InternetPinging = new();
            TokenInternetConnection = new();
            TaskInternetConnection = new(() =>
            {
                ObjectConnectEventArgs EventArgs;
                while (true)
                {
                    EventArgs = InternetPinging.UpdateInternetConnection();
                    Dispatcher.Invoke(() => ConnectionPingChanged?.Invoke(null, EventArgs));
                    Thread.Sleep(4000);
                }
            }, TokenInternetConnection);
            TaskInternetConnection.Start();

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
                if (MainBrowser.ActualInlay?.Content is PageConsole page)
                {
                    if (PageConsole.PageConsoleActionPanelMain.CommandViewerSelect != null)
                        page.DeleteCommandViewer(PageConsole.PageConsoleActionPanelMain.CommandViewerSelect);
                }
                MainWindow.IELActionPanelMain.ClosePanelAction();
            };
            PageConsole.PageConsoleActionPanelMain.IELButtonDeleteAllCommandViewers.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (MainBrowser.ActualInlay?.Content is PageConsole page)
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

        //
        internal void AddNewAppPage(Type TypeAppPage, string NameAppPage, PaletteSpectrum? Spectrum = null, ImageSource? Icon = null)
        {
            SourceManagerAppPage.AddNewAppPage(in MainBrowser, TypeAppPage, NameAppPage, Spectrum, Icon);
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
