using IEL.CORE.Classes;
using IEL.CORE.Classes.Browser;
using IEL.CORE.Classes.ObjectSettings;
using IEL.GUI;
using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using InterpreterCommand.Classes;
using LibraryPackKey.CORE;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OperPageLes.CORE;
using OperPageLes.CORE.Label;
using OperPageLes.CORE.Settings.Struct;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel.PageConsole;
using OperPageLes.UI.Pages.Browser;
using OperPageLes.UI.UserElementControl;
using OperPageLes.UI.Windows;
using OperPageLes.UI.Windows.Dialogs;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace OperPageLes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
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
            if (Duration != null) ThicknessAnimate.Duration = Duration.Value;
            ThicknessAnimate.From = From;
            ThicknessAnimate.To = To;
            Element.BeginAnimation(Property, ThicknessAnimate, HandoffBehavior.SnapshotAndReplace);
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
            if (Duration != null) ThicknessAnimate.Duration = Duration.Value;
            ThicknessAnimate.From = null;
            ThicknessAnimate.To = To;
            Element.BeginAnimation(Property, ThicknessAnimate, HandoffBehavior.SnapshotAndReplace);
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
            if (Duration != null) DoubleAnimate.Duration = Duration.Value;
            DoubleAnimate.From = From;
            DoubleAnimate.To = To;
            Element.BeginAnimation(Property, DoubleAnimate, HandoffBehavior.SnapshotAndReplace);
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
            if (Duration != null) DoubleAnimate.Duration = Duration.Value;
            DoubleAnimate.From = null;
            DoubleAnimate.To = To;
            Element.BeginAnimation(Property, DoubleAnimate, HandoffBehavior.SnapshotAndReplace);
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
            if (Duration != null) ColorAnimate.Duration = Duration.Value;
            ColorAnimate.From = From;
            ColorAnimate.To = To;
            Element.BeginAnimation(Property, ColorAnimate, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// Анимировать эффект цвета объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="From">Значение от которого начинается анимация</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal static void AnimateColorEffect(IAnimatable Element, DependencyProperty Property,
            System.Windows.Media.Color To, TimeSpan? Duration = null)
        {
            if (Duration != null) ColorAnimate.Duration = Duration.Value;
            ColorAnimate.From = null;
            ColorAnimate.To = To;
            Element.BeginAnimation(Property, ColorAnimate, HandoffBehavior.SnapshotAndReplace);
        }
        #endregion

        #endregion

        #region Data
        /// <summary>
        /// Установленный ключ валидности для приложения
        /// </summary>
        internal PackKey InstallingKey { get; private set; }

        /// <summary>
        /// Интерпретатор команд
        /// </summary>
        internal readonly COMInterpreter Interpreter;

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
        internal readonly List<OPLViewerLoadingProcess> DataViewerLoadingProcess = [];
        #endregion

        #region Windows
        /// <summary>
        /// Главное окно програмы
        /// </summary>
        internal static new MainWindow MainWindow => (MainWindow)Current.MainWindow;

#if DEBUG
        /// <summary>
        /// Окно разработчика
        /// </summary>
        internal UI.Windows.DEV.WindowDeveloper Is_WindowDeveloper { get; private set; }
#endif

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
        internal List<Window> OpenedWindowsInApplication;
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
        /// Файл настроек <b>процесса</b>
        /// </summary>
        private readonly string PathSettingProcess = StructDirectoryResources.MainDirectoryApplication + "/CurrentSettings.json";

        /// <summary>
        /// Имя файла настроек <b>приложения</b>
        /// </summary>
        private readonly string PathSettingApplication = StructDirectoryResources.MainDirectoryApplication + "/ApplicationSettings.json";

        /// <summary>
        /// Строка вывода перед сообщением
        /// </summary>
        public const string ConsolePreMessage = "%**>>>**";

        /// <summary>
        /// Директория файла открытых настроек <b>приложения</b>
        /// </summary>
        private string ActivePathSettingApplication = string.Empty;

        /// <summary>
        /// Ресурс настроек для отображения клавиш мыши
        /// </summary>
        internal static IELMouseImageSetting ResourceDefaultMouseImageSetting { get; } = new()
        {
            NotEventImageMouse = LoadImage(OperPageLes.Properties.Resources.NotMouseButton),
            FullEventImageMouse = LoadImage(OperPageLes.Properties.Resources.DoubleMouseButton),
            OnlyRightEventImageMouse = LoadImage(OperPageLes.Properties.Resources.RightMouseButton),
            OnlyLeftEventImageMouse = LoadImage(OperPageLes.Properties.Resources.LeftMouseButton),
        };

        /// <summary>
        /// Реальное время
        /// </summary>
        internal static DateTime RealTime => DateTime.Now;

        /// <summary>
        /// Клиент для манипуляции в сети интернет
        /// </summary>
        internal static HttpClient UsedHttpClient { get; } = new();

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

        public App()
        {
            #region Resources
            OpenedWindowsInApplication = [];
            InstallingKey = PackKey.StaticKey;
            LogStreamWriter = StructDirectoryResources.CreateLogStreamWriter($"LOG_Access {DateTime.Now:dd.MM.yyyy}");
            LogWriteLine("---------- Старт нового экземпляра ----------");
            LogWriteLine("Инициализация свойств экземпляра");
#if DEBUG
            Is_WindowDeveloper = new();
#endif
            //Resources.Add("DefaultMouseImage", ResourceDefaultMouseImageSetting);
            #endregion

            #region Interpreter
            LogWriteLine("Настройка интерпретатора");
            Interpreter = new([
                #region alias
                new ConsoleCommand("alias",
                [
                    new Parameter("Name", typeof(string)),
                    new Parameter("Command", typeof(string)),
                    new Parameter("Description", typeof(string), string.Empty)
                ],
                "Создаёт алиас \"Name\" на команду \"Command\". С описанием \"Description\"", (Main, param) =>
                {
                    string NameAlias = ((string)param[0]).ToLower();
                    bool CompleteCreateAlias = Interpreter?.AddAliasCommand(NameAlias, (string)param[1], (string)param[2]) ?? false;
                    if (!CompleteCreateAlias)
                    {
                        return Task.FromResult(CommandStateResult.Failed(Main.Name,
                            $"Aлиас \"%//{NameAlias}//\" невозможно создать, так как он уже создан\n%#EA5555//Для переопределения введите команду: %**alias_replace**//"));
                    }
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
                    string NameAlias = ((string)param[0]).ToLower();
                    AliasCommand<ICommandOPER>? alias = Interpreter?.ReadAliasCommand(NameAlias);
                    if (alias == null)
                    {
                        return Task.FromResult(CommandStateResult.Failed(Main.Name,
                            $"Aлиас \"%//{NameAlias}//\" невозможно изменить, так как он не существует \n%#EA5555//Для создания алиаса введите команду: %**alias**//"));
                    }
                    ICommandOPER? Com = Interpreter?.ReadCommand((string)param[1]);
                    CommandStateResult Result = alias.ChangeSourceCommand(Com, (string)param[1], ((string)param[2]).Length > 0 ? (string)param[2] : null);
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
                    PageLabels? SourcePage = MainWindow.IELBrowserPageMain.SearchPageType<PageLabels>();
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
                new ConsoleCommand("create_label", "Открывает окно создания ярлыка",
                (Command, param) =>
                {
                    DialogGenLabel GenLabel = new();
                    ActiveDialog = GenLabel;
                    LabelAction? label = GenLabel.CreateLabel();
                    ActiveDialog = null;
                    if (label != null)
                    {
                        PageLabels? SourcePage = MainWindow.IELBrowserPageMain.SearchPageType<PageLabels>();
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
                new ConsoleCommand("reboot", "Перезагружает программу", (Command, param) =>
                {
                    RebootApplication();
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region close
                new ConsoleCommand("close", "Закрывает программу", (Command, param) =>
                {
                    MainWindow.Close();
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region clear
                new ConsoleCommand("clear",
                "Очищает текстовый вывод главного меню программы",
                (Command, param) =>
                {
                    MainWindow.IELBrowserPageMain.SearchPageType<PageConsole>()?.ClearConsoleText();
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
                new ConsoleCommand("open_directory",
                [
                    new Parameter("Directory", typeof(string), string.Empty)
                ],
                "Открывает заданную директорию в проводнике. При отсутствии параметра будет открывать главную страницу проводника\n" +
                "- Вписав \"*\" в параметры, откроет главную директорию процесса приложения",
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
                ]);
            #endregion
            LogWriteLine("Инициализация параметров приложения");

            #region Settings
            LogWriteLine("Инициализация настроек");
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

            DataLabelTags = 
                [..StructDirectoryResources.DeserializeObjectJson<string>(StructDirectoryResources.DirectoryDataLabelTags).Select(Tag => new LabelTag(Tag))];

            DataLabels = [..StructDirectoryResources.DeserializeObjectJson<LabelAction>(StructDirectoryResources.DirectoryDataLabels)];

            #region SettingRuntimeRealizeSettingChanges
            SettingMainApplication.PathMenuImage.Changed += (Old, New) =>
            {
                MainWindow.UpdateImageMenu(New);
            };
            SettingMainApplication.BlurBackgroundDataTime.Changed += (Old, New) =>
            {
                MainWindow.ChangeBlurImageInDataTime(New);
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
            #endregion

            #region MediaFiles
            LogWriteLine("Проверка ресурсов");
            StructDirectoryResources.CheckCreateAllResources();
            #endregion

            #endregion
        }

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="e">Объект события начала работы прораммы</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            LogWriteLine("Подключение программной точки входа");
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
            Current.Exit += (sender, e) =>
            {
                LogWriteLine("---------- Конец текущего экземпляра ----------");
                LogStreamWriter?.Close();
            };
            Current.MainWindow = new MainWindow();
            LogWriteLine("Открытие главного окна");
            try
            {
#if DEBUG
                Is_WindowDeveloper.Show();
#endif
                ((MainWindow)Current.MainWindow).Show();
            }
            catch (Exception ex)
            {
                LogWriteLine($"{ex.Message}");
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
#if DEBUG
                Is_WindowDeveloper = new();
#endif
                Current.MainWindow = new MainWindow();
#if DEBUG
                Is_WindowDeveloper.Show();
#endif
                ((MainWindow)Current.MainWindow).Show();
                LogWriteLine("/// Перезагрузка прошла успешно! ///");
            };
            RebootWindow.IsReboot = true;
            RebootWindow.Close();
        }

        /// <summary>
        /// Загрузчик изображений через данные байтов
        /// </summary>
        /// <param name="imageData">Массив данных картинки</param>
        /// <returns>Объект изображения</returns>
        /// <exception cref="Exception">Исключение при повреждённом или пустом изображении</exception>
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
        internal void LogWriteLine(string Text) => LogStreamWriter?.WriteLine($"{DateTime.Now:HH:mm:ss ff} " + Text);

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
        /// <param name="AppendBufferCommand">Состояние добавления команды в буфер</param>
        internal void ActivateActionCommand(PageConsole? Console, string CommandString, bool AppendBufferCommand = true)
        {
            if (CommandString.Length == 0) return;
            if (Console != null) Console.TextBoxCommandInput.Text = string.Empty;
            ConsoleCommand? Command = (ConsoleCommand?)Interpreter.ReadCommand(CommandString);
            string Name = COMInterpreter.ReadNameCommand(CommandString);
            string[] Parameters = COMInterpreter.ReadParametersCommand(CommandString);

            if (AppendBufferCommand && Console != null)
            {
                PageConsole.BufferPage.InsertCommandFromBuffer(Name, CommandString,
                (sender, e, Key) =>
                {
                    ActivateActionCommand(Console, CommandString);
                });
            }

            CommandStateResult result = Command == null ? CommandStateResult.FaledCommand(Name) : Command.ExecuteCommand(Parameters);
            if (result.State == ResultState.InvalidCommand)
            {
                AliasCommand<ICommandOPER>? Alias = Interpreter.ReadAliasCommand(CommandString);
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

        [GeneratedRegex(@"[^ ]+")]
        private static partial Regex RegexPackValidKey();
        #endregion
    }
}
