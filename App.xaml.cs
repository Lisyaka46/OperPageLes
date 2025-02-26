using AAC20.CORE;
using AAC20.CORE.Flaging;
using AAC20.CORE.Settings;
using AAC20.UI.Dialogs;
using AAC20.UI.Pages.Browser;
using AAC20.Windows;
using AAC20.Windows.Pages.ActionPanel;
using AAC20.Windows.Pages.Browser;
using IEL.Classes;
using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace AAC20
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Структура всех окон программы
        /// </summary>
        internal readonly struct AppWindows
        {
            /// <summary>
            /// Окно описания всех команд
            /// </summary>
            internal static WindowDiscriptionCommands? DiscriptionCommands = null;
        }

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

        /// <summary>
        /// Объект всех страниц программы
        /// </summary>
        internal readonly PagesApplication AllPages;

        /// <summary>
        /// Константа высоты размера кнопки буфера
        /// </summary>
        internal const int HeightButtonBuffer = 45;

        /// <summary>
        /// Массив сех имён команд и алиасов
        /// </summary>
        public string[] AllNamesCommand => [.. DataAliases.Select(x => x.Name).Concat(DataConsoleCommand.Select(x => x.Name))];

        /// <summary>
        /// Массив консольных команд
        /// </summary>
        internal readonly List<AliasCommand<ICommandAAC>> DataAliases = [];

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
                CurrentApp.AllPages.PageConsoleApplication.ClearConsoleText();
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
                PageBufferActionPanel PageBuffer = CurrentApp.AllPages.PageConsoleApplication.PageBufferPA;
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

            #region label
            new ConsoleCommand("label",
            [
                new Parameter("Name", typeof(string)), new Parameter("Command", typeof(string)),
                new Parameter("Description", typeof(string), string.Empty)
            ],
            "Создаёт ярлык с именем \"Name\" и командой \"Command\", можно создать описание не обязательным параметром \"Description\"\n" +
            "- Ярлык создастся только если открыта страница ярлыков в браузере",
            (Command, param) =>
            {
                PageLabels? Page = MainWindowApplication.IELBrowserPageMain.SearchPageType<PageLabels>();
                if (Page == null)
                    return Task.FromResult(CommandStateResult.Failed(Command.Name,
                        $"Страница %#EA5555**\"{nameof(PageLabels)}\"** в браузере %__не инициализирована!__"));
                Page.AddLabel(new((string)param[0], (string)param[2], (string)param[1]));
                return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Ярлык \"%**{(string)param[0]}**\" успешно создан"));
            }),
            #endregion

            #region create_label
            new ConsoleCommand("create_label", "Открывает окно создания ярлыка\n" +
            "- Ярлык создастся только если открыта страница ярлыков в браузере",
            (Command, param) =>
            {
                PageLabels? Page = MainWindowApplication.IELBrowserPageMain.SearchPageType<PageLabels>();
                if (Page == null)
                    return Task.FromResult(CommandStateResult.Failed(Command.Name,
                        $"Страница %#EA5555**\"{nameof(PageLabels)}\"** в браузере %__не инициализирована!__"));
                LabelAction label = new WindowGenLabel().CreateLabel();
                if (label != LabelAction.Empty) Page.AddLabel(label);
                return Task.FromResult(CommandStateResult.Completed(Command.Name, label != LabelAction.Empty ? $"Ярлык \"%**{label.Name}**\" успешно создан" : null));
            }),
            #endregion

            #region open_link
            new ConsoleCommand("open_link", [new Parameter("Link", typeof(string))],
            "Открывает в браузере заданную ссылку \"Link\"",
            (Command, param) =>
            {
                try
                {
                    string uri = (string)param[0];
                    Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
                    return Task.FromResult(CommandStateResult.Completed(Command.Name, $"Открытие ссылки \"{uri}\""));
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

            #region alias
            new ConsoleCommand("alias",
            [
                new Parameter("Name", typeof(string)),
                new Parameter("Command", typeof(string)),
                new Parameter("Replace", typeof(bool), false)
            ],
            "Создаёт алиас \"Name\" на команду \"Command\".\nВозможно изменение через параметр \"Replace\"\n" +
            "- Если \"Replace\" true то при нахождении уже созданного алиаса с таким именем, у него будет изменена команда", (Main, param) =>
            {
                string[] NameAliases = [.. CurrentApp.DataAliases.Select(i => i.Name)];
                string NameAlias = ((string)param[0]).ToLower();
                if (NameAliases.Contains(NameAlias) && !(bool)param[2])
                {
                    return Task.FromResult(CommandStateResult.Failed(Main.Name,
                        $"Aлиас \"{NameAlias}\" невозможно создать, так как он уже создан\nДля переопределения введите третий параметр: true"));
                }
                if (!(bool)param[2])
                {
                    CurrentApp.DataAliases.Add(
                        new(NameAlias, (string)param[1], [.. DataConsoleCommand]));
                }
                else
                    CurrentApp.DataAliases[Array.IndexOf(NameAliases, NameAlias)].Command = (string)param[1];
                return Task.FromResult(CommandStateResult.Completed(Main.Name,
                    $"Aлиас \"%//{NameAlias}//\" на команду \"%//{param[1]}//\" успешно {((bool)param[2] ? "изменён" : "создан")}"));
            }),
            #endregion
        ];

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
        /// Массив ключей настроек <b>процесса</b>
        /// </summary>
        private readonly Setting<EnumSettingProcess> SettingProcess;

        /// <summary>
        /// Массив ключей настроек <b>приложения</b>
        /// </summary>
        internal Setting<EnumSettingApplication> SettingApplication;

        /// <summary>
        /// Константа директории файла настроек <b>процесса</b>
        /// </summary>
        private const string PathSettingProcess = "CurrentSettings.so";

        /// <summary>
        /// Имя файла настроек <b>приложения</b>
        /// </summary>
        private const string NameFileApplicationSetting = "ApplicationSettings";

        /// <summary>
        /// Релятивная директория папки изображений приложения
        /// </summary>
        internal const string PathImageApplication = "/UI/Images";

        /// <summary>
        /// Строка вывода перед сообщением
        /// </summary>
        public const string ConsolePreMessage = "%**>>>**";

        public App()
        {
            InitializeComponent();
            ThreadInternetCheckConnection = new(CheckInternetConnection, 900);
            ThreadInternetCheckConnection.Start();
            SettingProcess = new(PathSettingProcess,
            [
                // SettingApplicationPath
                $"{NameFileApplicationSetting}.so",
            ]);
            SettingApplication = new(SettingProcess.GetSettingValue(EnumSettingProcess.SettingApplicationPath),
            [
                // PathMenuImage
                "!",
                // BufferSize
                "50",
                // BlurBackgroundDataTime
                "T"
            ]);
            AllPages = new();
        }

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="e">Объект события начала работы прораммы</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            Current.MainWindow = new UI.Windows.MainWindow();
            Current.Exit += (sender, e) =>
            {
                ThreadInternetCheckConnection.Kill();
            };
            MainWindowApplication.Show();
        }

        /// <summary>
        /// Перезагрузить программу
        /// </summary>
        internal static void RebootApplication()
        {
            Process.Start(Process.GetCurrentProcess().ProcessName, Environment.GetCommandLineArgs());
            Current.Shutdown(0);
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

        /// <summary>
        /// Анимировать числовой эффект объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal static void AnimateDoubleEffect(FrameworkElement Element, DependencyProperty Property, double To, double Duration = 700d)
        {
            DoubleAnimation animation = new()
            {
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut,
                },
                Duration = TimeSpan.FromMilliseconds(Duration),
                To = To,
            };
            Element.BeginAnimation(Property, animation);
        }

        /// <summary>
        /// Обновить файл настроек программы
        /// </summary>
        internal void UpdateSettingApplication() => 
            SettingApplication.UpdateFileSetting(SettingProcess.GetSettingValue(EnumSettingProcess.SettingApplicationPath));

        /// <summary>
        /// Взаимодействовать с окном описания команд (Включает/Активирует)
        /// </summary>
        internal static void UsingDiscriptionCommand()
        {
            if (AppWindows.DiscriptionCommands == null)
            {
                AppWindows.DiscriptionCommands = new();
                AppWindows.DiscriptionCommands.Show();
            }
            else
            {
                AppWindows.DiscriptionCommands.WindowState = WindowState.Normal;
                AppWindows.DiscriptionCommands.Activate();
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
            ConsoleCommand? Command = ICommandAAC.ReadCommand([.. DataConsoleCommand], CommandString);
            string Name = ICommandAAC.ReadNameCommand(CommandString);
            string[] Parameters = ICommandAAC.ReadParametersCommand(CommandString);

            if (AppendBufferCommand && Console != null)
            {
                Console.PageBufferPA.InsertCommandFromBuffer(Name, CommandString,
                () =>
                {
                    ActivateActionCommand(Console, CommandString);
                });
            }

            CommandStateResult result = Command == null ? CommandStateResult.FaledCommand(Name) : Command.ExecuteCommand(Parameters);
            if (result.State == ResultState.InvalidCommand)
            {
                AliasCommand<ICommandAAC>? Alias = ICommandAAC.ReadCommand([.. App.CurrentApp.DataAliases], CommandString);
                result = Alias == null ? CommandStateResult.FaledCommand(Name) : Alias.ExecuteCommand();
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
