using IEL.CORE.Classes;
using IEL.CORE.Classes.Browser;
using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OperPage_les.CORE;
using OperPage_les.CORE.Flaging;
using OperPage_les.CORE.Settings;
using OperPage_les.CORE.Settings.Struct;
using OperPage_les.UI.Dialogs;
using OperPage_les.UI.Pages.Browser;
using OperPage_les.UI.UserElementControl;
using OperPage_les.Windows;
using OperPage_les.Windows.Pages.ActionPanel;
using OperPage_les.Windows.Pages.Browser;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace OperPage_les
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        /// <summary>
        /// Окно описания всех команд
        /// </summary>
        private WindowDiscriptionCommands? DiscriptionCommands;

        #region Application Flags
        /// <summary>
        /// Флаги данной формы
        /// </summary>
        internal readonly struct Flags
        {
            /// <summary>
            /// Состояние подключения к интернету
            /// </summary>
            internal static readonly Flag InternetPinging = new(false);

            /// <summary>
            /// Флаг состояния видимости объекта страниц
            /// </summary>
            internal static readonly Flag FlagFrameComponentVisible = new(true);

            /// <summary>
            /// Флаг состояния регистра
            /// </summary>
            internal static readonly Flag FlagRegisterState = new(Console.CapsLock);
        };
        #endregion

        #region AnimationObject

        #region ThicknessAnimation
        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut },
            From = null
        };

        /// <summary>
        /// Дать объект анимации
        /// </summary>
        /// <param name="NewDuration">Новое время анимации</param>
        /// <returns>Объект анимации</returns>
        internal static ThicknessAnimation GetThicknessAnimate(TimeSpan? NewDuration = null)
        {
            ThicknessAnimation Result = ThicknessAnimate.Clone();
            if (NewDuration.HasValue) Result.Duration = NewDuration.Value;
            return Result;
        }

        /// <summary>
        /// Анимировать эффект цвета объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="From">Значение от которого начинается анимация</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal static void AnimateThicknessEffect(IAnimatable Element, DependencyProperty Property, Thickness From, Thickness To, TimeSpan? Duration = null)
        {
            ThicknessAnimation animation = GetThicknessAnimate(Duration);
            animation.From = From;
            animation.To = To;
            Element.BeginAnimation(Property, animation);
        }

        /// <summary>
        /// Анимировать эффект цвета объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal static void AnimateThicknessEffect(IAnimatable Element, DependencyProperty Property, Thickness To, TimeSpan? Duration = null)
        {
            ThicknessAnimation animation = GetThicknessAnimate(Duration);
            animation.To = To;
            Element.BeginAnimation(Property, animation);
        }
        #endregion

        #region DoubleAnimation
        /// <summary>
        /// Объект анимации для управления double значением
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimate = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut },
            From = null
        };

        /// <summary>
        /// Дать объект анимации
        /// </summary>
        /// <param name="NewDuration">Новое время анимации</param>
        /// <returns>Объект анимации</returns>
        internal static DoubleAnimation GetDoubleAnimate(TimeSpan? NewDuration = null)
        {
            DoubleAnimation Result = DoubleAnimate.Clone();
            if (NewDuration.HasValue) Result.Duration = NewDuration.Value;
            return Result;
        }

        /// <summary>
        /// Анимировать числовой эффект объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="From">Значение от которого начинается анимация</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal static void AnimateDoubleEffect(IAnimatable Element, DependencyProperty Property, double From, double To, TimeSpan? Duration = null)
        {
            
            DoubleAnimation animation = GetDoubleAnimate(Duration);
            animation.From = From;
            animation.To = To;
            Element.BeginAnimation(Property, animation);
        }
        /// <summary>
        /// Анимировать числовой эффект объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal static void AnimateDoubleEffect(IAnimatable Element, DependencyProperty Property, double To, TimeSpan? Duration = null)
        {

            DoubleAnimation animation = GetDoubleAnimate(Duration);
            animation.To = To;
            Element.BeginAnimation(Property, animation);
        }
        #endregion

        #region ColorAnimation
        /// <summary>
        /// Объект анимации для управления Color значением
        /// </summary>
        private static readonly ColorAnimation ColorAnimate = new(Colors.Black, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut },
            From = null
        };

        /// <summary>
        /// Дать объект анимации
        /// </summary>
        /// <param name="NewDuration">Новое время анимации</param>
        /// <returns>Объект анимации</returns>
        internal static ColorAnimation GetColorAnimate(TimeSpan? NewDuration = null)
        {
            ColorAnimation Result = ColorAnimate.Clone();
            if (NewDuration.HasValue) Result.Duration = NewDuration.Value;
            return Result;
        }

        /// <summary>
        /// Анимировать эффект цвета объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal static void AnimateColorEffect(IAnimatable Element, DependencyProperty Property,
            System.Windows.Media.Color From, System.Windows.Media.Color To, TimeSpan? Duration = null)
        {
            ColorAnimation animation = GetColorAnimate(Duration);
            animation.From = From;
            animation.To = To;
            Element.BeginAnimation(Property, animation);
        }

        /// <summary>
        /// Анимировать эффект цвета объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="From">Значение от которого начинается анимация</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal static void AnimateColorEffect(IAnimatable Element, DependencyProperty Property, System.Windows.Media.Color To, TimeSpan? Duration = null)
        {
            ColorAnimation animation = GetColorAnimate(Duration);
            animation.To = To;
            Element.BeginAnimation(Property, animation);
        }
        #endregion

        #endregion

        /// <summary>
        /// Константа высоты размера кнопки буфера
        /// </summary>
        internal const int HeightButtonBuffer = 45;

        /// <summary>
        /// Массив сех имён команд и алиасов
        /// </summary>
        public string[] AllNamesCommand => [.. DataAliases.Select(x => x.Name).Concat(DataConsoleCommand.Select(x => x.Name))];

        #region Data
        /// <summary>
        /// Массив консольных команд
        /// </summary>
        internal readonly List<AliasCommand<ICommandOPER>> DataAliases = [];

        #region Console command
        /// <summary>
        /// Массив консольных команд
        /// </summary>
        internal static readonly List<ConsoleCommand> DataConsoleCommand =
        [
            #region reboot
            new ConsoleCommand("reboot", "Перезагружает программу", (Command, param) =>
            {
                RebootApplication();
                return Task.FromResult(CommandStateResult.Completed(Command.Name));
            }),
            #endregion

            #region close
            new ConsoleCommand("close", "Закрывает программу", (Command, param) =>
            {
                Current.Shutdown(0);
                return Task.FromResult(CommandStateResult.Completed(Command.Name));
            }),
            #endregion

            #region clear
            new ConsoleCommand("clear",
            "Очищает текстовый вывод главного меню программы",
            (Command, param) =>
            {
                App.MainWindowApplication.IELBrowserPageMain.SearchPageType<PageConsole>()?.ClearConsoleText();
                return Task.FromResult(CommandStateResult.Completed(Command.Name));
            }),
            #endregion

            #region print
            new ConsoleCommand("print", [new Parameter("Text", typeof(string))],
            "Выводит введённый параметр \"Text\" в консоль главного меню программы, игнорируя другие параметры",
            (Command, param) =>
            {
                return Task.FromResult(CommandStateResult.Completed(Command.Name, (string)param[0]));
            }),
            #endregion

            #region buffer
            new ConsoleCommand("buffer",
            "Отображает содержание буфера команд в консоль главного меню программы",
            (Command, param) =>
            {
                PageBufferPanelAction PageBuffer = PageConsole.BufferPage;
                return Task.FromResult(CommandStateResult.Completed(Command.Name,
                    $"%//{PageBuffer.BufferCommand.Count}/{PageBuffer.BufferCommand.Length}://" +
                    $"%**[**{string.Join(',', PageBuffer.BufferCommand.BufferElements.Where((i) =>
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
            new ConsoleCommand("open_link", [new Parameter("Link", typeof(string))],
            "Открывает в браузере заданную ссылку \"Link\"",
            (Command, param) =>
            {
                try
                {
                    string url = (string)param[0];
                    bool UsePageBroswer = CurrentApp.SettingMainApplication.UseOpenLinkInPageBrowser;
                    if (UsePageBroswer)
                    {
                        PageWebBrowser?[]? AllWebBrowsers = MainWindowApplication.IELBrowserPageMain.SearchAllPageType<PageWebBrowser>();
                        if (AllWebBrowsers != null)
                        {
                            PageWebBrowser? pageWebBrowser = AllWebBrowsers.FirstOrDefault();
                            pageWebBrowser?.WebViewGoUrl(url);
                            MainWindowApplication.IELBrowserPageMain.ActivateInlayIndex(Array.IndexOf(AllWebBrowsers, pageWebBrowser));
                        }
                        else
                        {
                            BrowserPage browser_page_element = new(new PageWebBrowser(), "Веб-браузер", null);
                            browser_page_element.Disposed += (sender) =>
                            {
                                ((PageWebBrowser)browser_page_element.PageContent).WebBrowserElement.Dispose();
                            };
                            MainWindowApplication.IELBrowserPageMain.AddInlayPage(browser_page_element);
                            ((PageWebBrowser)browser_page_element.PageContent).WebViewGoUrl(url);
                        }
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
            new ConsoleCommand("open_directory",
            [
                new Parameter("Directory", typeof(string), string.Empty)
            ],
            "Открывает заданную директорию в проводнике. При отсутствии параметра будет открывать главную страницу проводника\n" +
            "- Вписав \"*\" в параметры, откроет гравную директорию процесса приложения",
            (Command, param) =>
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
            new ConsoleCommand("open_file",
            [
                new Parameter("File", typeof(string))
            ],
            "Открывает файл по его заданной директории",
            (Command, param) =>
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
        ];
        #endregion

        /// <summary>
        /// Массив консольных команд
        /// </summary>
        internal readonly List<LabelAction> DataLabels = [];
        #endregion

        /// <summary>
        /// Главное окно програмы
        /// </summary>
        internal static UI.Windows.MainWindow MainWindowApplication => (UI.Windows.MainWindow)Current.MainWindow;

        /// <summary>
        /// Поток обновляемый данные интернета
        /// </summary>
        private readonly ThreadGenericProcess ThreadInternetCheckConnection;

        /// <summary>
        /// Экземпляр созданного приложения
        /// </summary>
        internal static App CurrentApp => (App)Current;

        /// <summary>
        /// Страница взаимодествия с ярлыками
        /// </summary>
        internal PageLabels? MainPageLabels;

        /// <summary>
        /// Массив ключей настроек <b>процесса</b>
        /// </summary>
        private SettingProcess SettingApplicationProcess;

        /// <summary>
        /// Массив ключей настроек <b>приложения</b>
        /// </summary>
        internal SettingApplication SettingMainApplication { get; private set; }

        /// <summary>
        /// Файл настроек <b>процесса</b>
        /// </summary>
        private readonly string PathSettingProcess = MainDirectoryApplication + "/CurrentSettings.json";

        /// <summary>
        /// Имя файла настроек <b>приложения</b>
        /// </summary>
        private readonly string PathSettingApplication = MainDirectoryApplication + "/ApplicationSettings.json";

        /// <summary>
        /// Строка вывода перед сообщением
        /// </summary>
        public const string ConsolePreMessage = "%**>>>**";

        /// <summary>
        /// Директория файла открытых настроек <b>приложения</b>
        /// </summary>
        private string ActivePathSettingApplication = string.Empty;

        #region DIRECTIRY RESOURCES
        /// <summary>
        /// Главная директория ресурсов проекта
        /// </summary>
        internal static readonly string MainDirectoryApplication = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/OperPage_les/";

        /// <summary>
        /// Главная директория файлов изображений
        /// </summary>
        internal static readonly string DirectoryImagesApplication = MainDirectoryApplication + @"/Images/";

        /// <summary>
        /// Главная директория ресурсов
        /// </summary>
        internal static readonly string DirectoryResourcesApplication = MainDirectoryApplication + @"/Resources/";

        /// <summary>
        /// Главная директория ресурсов
        /// </summary>
        internal static readonly string DirectoryDataLabels = DirectoryResourcesApplication + "Labels.json";

        /// <summary>
        /// Директория файла анимации загрузки
        /// </summary>
        internal static readonly string DirectoryImageLoading = DirectoryImagesApplication + "Loading.gif";
        #endregion

        /// <summary>
        /// Реальное время
        /// </summary>
        internal static DateTime RealTime => DateTime.Now;

        /// <summary>
        /// Количество миллисекунд ушедших на подключение
        /// </summary>
        internal static volatile object MillisecondInternetConnection = -1L;

        public App()
        {
            DataConsoleCommand.AddRange([
                #region alias
                new ConsoleCommand("alias",
                [
                    new Parameter("Name", typeof(string)),
                    new Parameter("Command", typeof(string)),
                    new Parameter("Description", typeof(string), string.Empty)
                ],
                "Создаёт алиас \"Name\" на команду \"Command\". С описанием \"Description\"", (Main, param) =>
                {
                    string[] NameAliases = [.. CurrentApp.DataAliases.Select(i => i.Name)];
                    string NameAlias = ((string)param[0]).ToLower();
                    if (NameAliases.Contains(NameAlias))
                    {
                        return Task.FromResult(CommandStateResult.Failed(Main.Name,
                            $"Aлиас \"%//{NameAlias}//\" невозможно создать, так как он уже создан\n%#EA5555//Для переопределения введите команду: %**alias_replace**//"));
                    }
                    CurrentApp.DataAliases.Add(new(NameAlias, (string)param[1], (string)param[2], ICommandOPER.ReadCommand([.. DataConsoleCommand], (string)param[1])));
                    if (DiscriptionCommands != null) DiscriptionCommands.IELButtonAlias.IsEnabled = CurrentApp.DataAliases.Count > 0;
                    return Task.FromResult(CommandStateResult.Completed(Main.Name,
                        $"Aлиас \"%//{NameAlias}//\" на команду \"%//{param[1]}//\" успешно %**создан**"));
                }),
                #endregion

                #region alias_replace
                new ConsoleCommand("alias_replace",
                [
                    new Parameter("Name", typeof(string)),
                    new Parameter("Command", typeof(string)),
                    new Parameter("Description", typeof(string), string.Empty)
                ],
                "Изменяет алиас \"Name\" на новую команду алиаса \"Command\". С необязательным изменением описания \"Description\"", (Main, param) =>
                {
                    string[] NameAliases = [.. CurrentApp.DataAliases.Select(i => i.Name)];
                    string NameAlias = ((string)param[0]).ToLower();
                    int index_replace = Array.IndexOf(NameAliases, NameAlias);
                    if (index_replace == -1)
                    {
                        return Task.FromResult(CommandStateResult.Failed(Main.Name,
                            $"Aлиас \"%//{NameAlias}//\" невозможно изменить, так как он не существует \n%#EA5555//Для создания алиаса введите команду: %**alias**//"));
                    }
                    CommandStateResult Result = CurrentApp.DataAliases[index_replace].ChangeSourceCommand(
                        [.. DataConsoleCommand], (string)param[1], ((string)param[2]).Length > 0 ? (string)param[2] : null);
                    return Task.FromResult(CommandStateResult.Completed(Main.Name,
                        $"Aлиас \"%//{NameAlias}//\" на команду \"%//{param[1]}//\" {(Result.State == ResultState.Complete ? "успешно %**изменён**" : "невозможно %**изменить**")}"));
                }),
                #endregion

                #region label
                new ConsoleCommand("label",
                [
                    new Parameter("Name", typeof(string)), new Parameter("Command", typeof(string)),
                    new Parameter("Description", typeof(string), string.Empty)
                ],
                "Создаёт ярлык с именем \"Name\" и командой \"Command\", можно создать описание не обязательным параметром \"Description\"\n",
                (Command, param) =>
                {
                    DataLabels.Add(new((string)param[0], (string)param[2], (string)param[1]));
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Ярлык \"%**{(string)param[0]}**\" успешно создан"));
                }),
                #endregion

                #region create_label
                new ConsoleCommand("create_label", "Открывает окно создания ярлыка",
                (Command, param) =>
                {
                    LabelAction? label = new WindowGenLabel().CreateLabel();
                    if (label != null) DataLabels.Add(label);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, label != null ? $"Ярлык \"%**{label?.Name}**\" успешно создан" : null));
                }),
                #endregion
            ]);
            InitializeComponent();
            Directory.CreateDirectory(MainDirectoryApplication);
            Directory.CreateDirectory(DirectoryImagesApplication);
            Directory.CreateDirectory(DirectoryResourcesApplication);
            Log("Инициализация параметров приложения");

            MillisecondInternetConnection = -1L;
            ThreadInternetCheckConnection = new(CheckInternetConnection, 5100);
            ThreadInternetCheckConnection.Start();

            #region Settings
            Log("Инициализация настроек");
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

            DataLabels = [];
            if (!File.Exists(DirectoryDataLabels))
            {
                string SettingApplicationJSON = JsonConvert.SerializeObject(DataLabels);
                File.WriteAllText(DirectoryDataLabels, SettingApplicationJSON);
            }
            else
            {
                LabelAction[]? Labels = JsonConvert.DeserializeObject<LabelAction[]>(File.ReadAllText(DirectoryDataLabels));
                if (Labels != null) DataLabels.AddRange(Labels);
            }

            SettingMainApplication.PathMenuImage.Changed += (Old, New) =>
            {
                MainWindowApplication.UpdateImageMenu(New);
            };
            SettingMainApplication.BlurBackgroundDataTime.Changed += (Old, New) =>
            {
                MainWindowApplication.ChangeBlurImageInDataTime(New);
            };
            SettingMainApplication.MillisecondInternetConnection.Changed += (Old, New) =>
            {
                MainWindowApplication.ChangeVisibilityMillisecondInternet(New);
            };

            if (!File.Exists(DirectoryImageLoading))
            {
                FileStream stream = File.Create(DirectoryImageLoading);
                stream.Position = 0;
                stream.Write(OperPage_les.Properties.Resources.Loading);
                stream.Close();
            }
            #endregion
        }

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="e">Объект события начала работы прораммы</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            Log("Подключение программной точки входа");
            Current.MainWindow = new UI.Windows.MainWindow();
            Current.MainWindow.Closed += (sender, e) =>
            {
                DiscriptionCommands?.Close();
            };
            Current.Exit += (sender, e) =>
            {
                ThreadInternetCheckConnection.Kill();
#if !DEBUG
                UpdateSettingApplication();
                UpdateFileDataLabel();
#endif
            };
            Log("Открытие главного окна");
            try
            {
                MainWindowApplication.Show();
            }
            catch (Exception ex)
            {
                Log($"{ex.Message}");
            }
        }

        /// <summary>
        /// Перезагрузить программу
        /// </summary>
        internal static void RebootApplication()
        {
            Process.Start(Process.GetCurrentProcess().ProcessName, Environment.GetCommandLineArgs());
            Current.Shutdown(0);
        }

        internal static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) throw new Exception("Неожиданное содержание нулевого массива байтов.");
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            //image.Freeze();
            return image;
        }

        internal static void Log(string log)
        {
            StreamWriter stream = File.AppendText(MainDirectoryApplication + @"/Access.log");
            stream.WriteLine($"{DateTime.Now.ToLocalTime()}: " + log);
            stream.Close();
        }

        /// <summary>
        /// Проверка подключения интернета
        /// </summary>
        private static void CheckInternetConnection()
        {
            Ping ObjPing = new();
            try
            {
                Flags.InternetPinging.Wait = true;
                PingReply reply = ObjPing.SendPingAsync("yandex.ru", 3000).Result;
                Flags.InternetPinging.Wait = false;
                Flags.InternetPinging.Value = reply.Status == IPStatus.Success;
                MillisecondInternetConnection = reply.RoundtripTime;
            }
            catch
            {
                Flags.InternetPinging.Wait = false;
                Flags.InternetPinging.Value = false;
            }
        }

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
            File.WriteAllText(DirectoryDataLabels, SettingApplicationJSON);
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

        /// <summary>
        /// Взаимодействовать с окном описания команд (Включает/Активирует)
        /// </summary>
        internal void UsingDiscriptionCommand()
        {
            if (DiscriptionCommands == null)
            {
                DiscriptionCommands = new();
                DiscriptionCommands.Closing += (sender, e) =>
                {
                    DiscriptionCommands = null;
                };
                DiscriptionCommands.Show();
            }
            else
            {
                DiscriptionCommands.WindowState = WindowState.Normal;
                DiscriptionCommands.Activate();
            }
        }

        #region CommandActivate
        /// <summary>
        /// Активировать команду
        /// </summary>
        /// <param name="CommandString">Строка команды</param>
        /// <param name="AppendBufferCommand">Состояние добавления команды в буфер</param>
        internal void ActivateActionCommand(PageConsole? Console, string CommandString, bool AppendBufferCommand = true)
        {
            if (CommandString.Length == 0) return;
            if (Console != null) Console.TextBoxCommandInput.Text = string.Empty;
            ConsoleCommand? Command = ICommandOPER.ReadCommand([.. DataConsoleCommand], CommandString);
            string Name = ICommandOPER.ReadNameCommand(CommandString);
            string[] Parameters = ICommandOPER.ReadParametersCommand(CommandString);

            if (AppendBufferCommand && Console != null)
            {
                PageConsole.BufferPage.InsertCommandFromBuffer(Name, CommandString,
                (sender, Key) =>
                {
                    ActivateActionCommand(Console, CommandString);
                });
            }

            CommandStateResult result = Command == null ? CommandStateResult.FaledCommand(Name) : Command.ExecuteCommand(Parameters);
            if (result.State == ResultState.InvalidCommand)
            {
                AliasCommand<ICommandOPER>? Alias = ICommandOPER.ReadCommand([.. App.CurrentApp.DataAliases], CommandString);
                result = Alias == null ? CommandStateResult.FaledCommand(Name) : Alias.ExecuteCommand(Parameters);
            }
            if (Console != null) SummarizeCommandStateResult(Console, result);
        }

        /// <summary>
        /// Активировать команду не добавляя в буфер
        /// </summary>
        /// <param name="CommandString">Строка команды</param>
        public void ActivateActionCommand(PageConsole? Console, string CommandString) => ActivateActionCommand(Console, CommandString, false);

        /// <summary>
        /// Создать действие над итогом выполнения команды
        /// </summary>
        /// <param name="Result">Объект итога выполнения команды</param>
        [MTAThread()]
        internal static void SummarizeCommandStateResult(PageConsole Console, CommandStateResult Result)
        {
            Console.AddTextInConsole(Result.Message);
        }
        #endregion
    }
}
