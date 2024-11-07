#region Link
using AAC20.CORE;
using AAC20.CORE.Flaging;
using AAC20.CORE.Settings;
using AAC20.UI.Dialogs;
using AAC20.Windows.Frames;
using AAC20.Windows.Pages.ActionPanel;
using AAC20.Windows.Pages.Other;
using IEL;
using IEL.Classes;
using IEL.Interfaces.Core;
using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace AAC20.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Флаги данной формы
        /// </summary>
        private readonly struct Flags
        {
            /// <summary>
            /// Флаг соеденения с интернетом
            /// </summary>
            internal static readonly Flag FlagInternetConnection = new(false);

            /// <summary>
            /// Флаг состояния видимости объекта страниц
            /// </summary>
            internal static readonly Flag FlagFrameComponentVisible = new(true);

            /// <summary>
            /// Флаг состояния регистра
            /// </summary>
            internal static readonly Flag FlagRegisterState = new(Console.CapsLock);

            /// <summary>
            /// Флаг обновления подсказок к командам
            /// </summary>
            internal static readonly Flag FlagHintRead = new(false);
        };

        /// <summary>
        /// Класс страниц данной формы
        /// </summary>
        private readonly struct Pages
        {
            /// <summary>
            /// Главная страница панели действий
            /// </summary>
            internal static readonly PageMainActionPanel PageMainActPanel = new();

            /// <summary>
            /// Страница буфера в панели действий
            /// </summary>
            internal static readonly PageBufferActionPanel PageBufferActPanel = new(H);

            #if DEBUG
            /// <summary>
            /// Страница разработчика
            /// </summary>
            internal static readonly PageDeveloper PageDeveloperState = new();
            #endif
        }

        /// <summary>
        /// Реальное время
        /// </summary>
        private static string RealTime => DateTime.Now.ToString("HH:mm:ss");

        /// <summary>
        /// Реальное время
        /// </summary>
        private static string RealData => DateTime.Now.ToString("dd.MM.yyyy");

        /// <summary>
        /// Объект управления фоновым обновлением информации в данном окне 1000
        /// </summary>
        private readonly UpdateBackgroundData UpdateBackgroundDataThis;

        /// <summary>
        /// Объект управления фоновым обновлением информации в данном окне 1
        /// </summary>
        private readonly UpdateBackgroundData UpdateBackgroundDataRunTime;

        /// <summary>
        /// Поток обновляемый совпадающие команды
        /// </summary>
        private readonly ThreadGenericProcess UpdateSearchHintCommand;

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления double значением
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimateObj = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления Color значением
        /// </summary>
        private static readonly ColorAnimation ColorAnimate = new(Colors.Black, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Константа размера Height для кнопок буфера
        /// </summary>
        [NotNull()]
        const int H = 41;

        private readonly SettingsPanelActionFrameworkElement SettingsMain;

        //private MMDeviceEnumerator Device = new();

        /// <summary>
        /// Страница ярлыков программы
        /// </summary>
        private PageLabels? PageLabelsApplication = null;

        /// <summary>
        /// Состояние воспроизведения приветственной анимации
        /// </summary>
        private bool HiAnimation = false;

        /// <summary>
        /// Массив всех отсортированных имён команд
        /// </summary>
        private string[] AllHintNames;

        /// <summary>
        /// Динамический массив всех имён команд
        /// </summary>
        private readonly List<string> EnumerateNameCommand = [];

        /// <summary>
        /// Константа высоты элемента подсказки к командам
        /// </summary>
        private const int HeightHintElement = 20;

        /// <summary>
        /// Состояние нажатия кнопки Shift при введении команды
        /// </summary>
        private bool ShiftDowningCommandTextInput = false;

        public MainWindow()
        {
            InitializeComponent();

            #region Command
            App.DataConsoleCommand.AddRange([
                #if DEBUG
                #region anim
                new ConsoleCommand("anim", [new Parameter("Value", typeof(bool))],
                "Отключает или включает анимацию у окна ярлыков",
                (Command, param) =>
                {
                    PageLabels? Page = IELBrowserPageMain.SearchPageType<PageLabels>();
                    if (Page == null)
                        return Task.FromResult(CommandStateResult.Failed(Command.Name,
                            $">>> Страница \"{nameof(PageLabels)}\" в браузере не инициализирована!"));
                    if ((bool)param[0]) Page.AnimationLoadingStart();
                    else Page.AnimationLoadingStop();
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region dev
                new ConsoleCommand("dev",
                "Открывает страницу разработчика",
                (Command, param) =>
                {
                    if (!Flags.FlagFrameComponentVisible) UsingChangeStateFrameComponent();
                    //FrameComponent.NextPage(Pages.PageDeveloperState);
                    IELBrowserPageMain.AddInlayPage(Pages.PageDeveloperState, "Страница разработчика",
                        "Для взаимодействия со страницей разработчика является рискованным, ДЕЛАЙТЕ ТОЛЬКО ЕСЛИ ЗНАЕТЕ ЧТО ДЕЛАЕТЕ !");
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion
                #endif

                #region clear
                new ConsoleCommand("clear",
                "Очищает текстовый вывод главного меню программы",
                (Command, param) =>
                {
                    RichTextBoxMainMessage.Document = new();
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region print
                new ConsoleCommand("print", [new Parameter("Text", typeof(string))],
                "Выводит введённый параметр \"Text\" в консоль главного меню программы, игнорируя другие параметры",
                (Command, param) =>
                {
                    AddTextInConsole((string)param[0]);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region buffer
                new ConsoleCommand("buffer",
                "Отображает содержание буфера команд в консоль главного меню программы",
                (Command, param) =>
                {
                    AddTextInConsole($"%//{Pages.PageBufferActPanel.BufferCommand.Count}/{Pages.PageBufferActPanel.BufferCommand.Length}://" +
                        $"%**[**{string.Join(',', Pages.PageBufferActPanel.BufferCommand.BufferElements.Where((i) =>
                        {
                            if (i != null)
                            {
                                return i.Length > 0;
                            }
                            return false;
                        }))}%**]**");
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
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
                    PageLabels? Page = IELBrowserPageMain.SearchPageType<PageLabels>();
                    if (Page == null)
                        return Task.FromResult(CommandStateResult.Failed(Command.Name,
                            $"Страница %#FF0000**\"{nameof(PageLabels)}\"** в браузере %__не инициализирована!__"));
                    Page.AddLabel(new((string)param[0], (string)param[2], (string)param[1]));
                    //CounterScrollBar g = Pages.PageObjLabelsAction.ScrollBar;
                    //Test.Text = $"Value:{g.Value} Max:{g.MaxValue}";
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region create_label
                new ConsoleCommand("create_label", "Открывает окно создания ярлыка\n" +
                "- Ярлык создастся только если открыта страница ярлыков в браузере",
                (Command, param) =>
                {
                    PageLabels? Page = IELBrowserPageMain.SearchPageType<PageLabels>();
                    if (Page == null)
                        return Task.FromResult(CommandStateResult.Failed(Command.Name,
                            $"Страница %#FF0000**\"{nameof(PageLabels)}\"** в браузере %__не инициализирована!__"));
                    LabelAction label = new WindowGenLabel().CreateLabel();
                    if (label != LabelAction.Empty) Page.AddLabel(label);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
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
                        Paragraph Message = new();
                        Run RuningText = new($"\"{param[0]}\"")
                        {
                            Background = new SolidColorBrush(Colors.Green),
                            Cursor = Cursors.Hand,
                        };
                        RuningText.MouseEnter += (sender, e) =>
                        {
                            ColorAnimate.To = Color.FromRgb(53, 161, 175);
                            RuningText.Background.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
                        };
                        RuningText.MouseLeave += (sender, e) =>
                        {
                            IELMessageMain.CloseBorderInformation();
                            ColorAnimate.To = Colors.Green;
                            RuningText.Background.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
                        };
                        RuningText.MouseLeftButtonUp += (sender, e) =>
                            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
                        Message.Inlines.Add(new Bold(new Run(">>> Открытие ссылки ")));
                        Message.Inlines.Add(RuningText);
                        RichTextBoxMainMessage.Document.Blocks.Add(Message);
                    }
                    catch
                    {
                        return Task.FromResult(CommandStateResult.Failed(Command.Name, $"Не удалось открыть ссылку \"{param[0]}\""));
                    }
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region open_directory
                new ConsoleCommand("open_directory", [new Parameter("Directory", typeof(string), string.Empty)],
                "Открывает заданную директорию в проводнике. При отсутствии параметра будет открывать главную страницу проводника\n" +
                "- Вписав \"*\" в параметры, откроет гравную директорию процесса приложения",
                (Command, param) =>
                {
                    Paragraph Message = new();
                    Run RuningText = new()
                    {
                        Background = new SolidColorBrush(Colors.Green),
                        Cursor = Cursors.Hand,
                    };
                    Message.Inlines.Add(new Bold(new Run(">>> Открытие директории ")));
                    switch ((string)param[0])
                    {
                        case "":
                            RuningText.Text = "\"MAIN\"";
                            RuningText.MouseLeftButtonUp += (sender, e) => Process.Start("explorer.exe");
                            Process.Start("explorer.exe");
                            break;
                        case "*":
                            RuningText.Text = "\"APPLICATION MAIN\"";
                            RuningText.MouseLeftButtonUp += (sender, e) => Process.Start("explorer.exe", Directory.GetCurrentDirectory());
                            Process.Start("explorer.exe", Directory.GetCurrentDirectory());
                            break;
                        default:
                            if (Directory.Exists((string)param[0]))
                            {
                                string Path = (string)param[0];
                                RuningText.Text = Path.Length >= 20 ? $"..\"{Path[(Path.Length - 20)..]}\"" : $"\"{Path}\"";
                                RuningText.MouseLeftButtonUp += (sender, e) => Process.Start("explorer.exe", (string)param[0]);
                                Process.Start("explorer.exe", (string)param[0]);
                                break;
                            }
                            return Task.FromResult(CommandStateResult.Failed(Command.Name, $"Директория \"{param[0]}\" не распознана"));
                    }
                    RuningText.MouseEnter += (sender, e) =>
                    {
                        ColorAnimate.To = Color.FromRgb(53, 161, 175);
                        RuningText.Background.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
                    };
                    RuningText.MouseLeave += (sender, e) =>
                    {
                        IELMessageMain.CloseBorderInformation();
                        ColorAnimate.To = Colors.Green;
                        RuningText.Background.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
                    };
                    Message.Inlines.Add(RuningText);
                    RichTextBoxMainMessage.Document.Blocks.Add(Message);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
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
                        Message.Inlines.Add(new Bold(new Run(">>> Открытие файла ")));
                        Run RuningText = new($"\"{Path.GetFileName(path)}\"")
                        {
                            Background = new SolidColorBrush(Colors.Green),
                            Cursor = Cursors.Hand,
                        };
                        RuningText.MouseEnter += (sender, e) =>
                        {
                            ColorAnimate.To = Color.FromRgb(53, 161, 175);
                            RuningText.Background.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
                        };
                        RuningText.MouseLeave += (sender, e) =>
                        {
                            IELMessageMain.CloseBorderInformation();
                            ColorAnimate.To = Colors.Green;
                            RuningText.Background.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
                        };

                        RuningText.MouseLeftButtonUp += (sender, e) => Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });

                        Message.Inlines.Add(RuningText);
                        RichTextBoxMainMessage.Document.Blocks.Add(Message);
                        return Task.FromResult(CommandStateResult.Completed(Command.Name));
                    }
                    else return Task.FromResult(
                        CommandStateResult.Failed(Command.Name, $"Файл \"{Path.GetFileName(path)}\" по данной директории не найден"));
                }),
                #endregion

                #region alias
                new ConsoleCommand("alias", [new Parameter("Name", typeof(string)), new Parameter("Command", typeof(string)),
                    new Parameter("Replace", typeof(bool), false)],
                "Создаёт алиас \"Name\" на команду \"Command\".\nВозможно изменение через параметр \"Replace\"\n" +
                "- Если \"Replace\" true то при нахождении уже созданного алиаса с таким именем, у него будет изменена команда", (Main, param) =>
                {
                    string[] NameAliases = [.. App.CurrentApp.DataAliases.Select(i => i.Name)];
                    Paragraph Message = new();
                    Message.Inlines.Add(new Bold(new Run(">>> ")));
                    string NameAlias = ((string)param[0]).ToLower();
                    if (NameAliases.Contains(NameAlias) && !(bool)param[2])
                    {
                        return Task.FromResult(CommandStateResult.Failed(Main.Name,
                            $"Aлиас \"{NameAlias}\" невозможно создать, так как он уже создан\nДля переопределения введите третий параметр: true"));
                    }
                    Message.Inlines.Add(new Run("Алиас "));
                    Run RuningText = new($"\"{NameAlias}\"")
                    {
                        Background = new SolidColorBrush(Colors.Green),
                        Cursor = Cursors.Hand,
                    };
                    RuningText.MouseEnter += (sender, e) =>
                    {
                        ColorAnimate.To = Color.FromRgb(53, 161, 175);
                        RuningText.Background.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
                    };
                    RuningText.MouseLeave += (sender, e) =>
                    {
                        IELMessageMain.CloseBorderInformation();
                        ColorAnimate.To = Colors.Green;
                        RuningText.Background.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
                    };
                    RuningText.MouseLeftButtonUp += (sender, e) => ActivateActionCommand(NameAlias);
                    Message.Inlines.Add(RuningText);
                    Message.Inlines.Add(new Run($" на команду \"{param[1]}\" успешно {((bool)param[2] ? "изменён" : "создан")}"));
                    if (!(bool)param[2])
                    {
                        EnumerateNameCommand.Add(NameAlias);
                        App.CurrentApp.DataAliases.Add(
                            new(NameAlias, (string)param[1], [.. App.DataConsoleCommand]));
                    }
                    else
                        App.CurrentApp.DataAliases[Array.IndexOf(NameAliases, NameAlias)].Command = (string)param[1];
                    RichTextBoxMainMessage.Document.Blocks.Add(Message);
                    return Task.FromResult(CommandStateResult.Completed(Main.Name));
                }),
                #endregion
            ]);
            #endregion

            #region Event Flags
            Flags.FlagInternetConnection.ChangeStateFlag += (NewValue) =>
            {
                ImageInternetConnection.Source = new BitmapImage(new Uri($"{App.PathImageApplication}/Wifi{(NewValue ? "On" : "Off")}.png", UriKind.Relative));
                AnimateBlurEffect(BlurEffectImageInternetConnection, 10u);
            };
            Flags.FlagRegisterState.ChangeStateFlag += (NewValue) =>
            {
                TextBlockRegister.Text = NewValue ? "A" : "a";
                AnimateBlurEffect(BlurEffectTextBlockRegister, 10u);
                if (IELMessageMain.FlagMessage && IELMessageMain.NameParentObject.Equals(BorderStateRegister.Name))
                    IELMessageMain.UsingBorderInformation(BorderStateRegister, BorderStateRegister.Name, Flags.FlagRegisterState ?
                        "Установлен большой регистр" : "Установлен малый регистр",
                        IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            #endregion

            #region Event Pages
            Pages.PageMainActPanel.IELButtonCrearConsole.OnActivateMouseLeft += (AltMode) =>
            {
                RichTextBoxMainMessage.Document = new();
                IELActionPanelMain.ClosePanelAction();
            };
            Pages.PageMainActPanel.IELButtonCrearConsole.OnActivateMouseRight += (AltMode) => RichTextBoxMainMessage.Document = new();

            Pages.PageMainActPanel.IELButtonCommandBuffer.OnActivateMouseLeft += (AltMode) =>
            {
                IELActionPanelMain.NextPage(Pages.PageBufferActPanel);
            };

            Pages.PageBufferActPanel.IELButtonBackMainMenu.OnActivateMouseLeft += (AltMode) =>
            {
                IELActionPanelMain.NextPage(Pages.PageMainActPanel, false);
            };

            Pages.PageMainActPanel.IELButtonDiscriptionCommand.OnActivateMouseLeft += (Key) =>
            {
                IELActionPanelMain.ClosePanelAction();
                UsingDiscriptionCommand();
            };
            Pages.PageDeveloperState.IELButtonCreateLabel.OnActivateMouseLeft += () =>
            {
                //Pages.PageObjLabelsAction.AddLabel(new("Test", "Test", "Test"));
                //Pages.PageDeveloperState.TextBlockLabelsCount.Text = $"={Pages.PageObjLabelsAction.CountLabel}";
            };
            #endregion

            #region BackgroundData
            UpdateBackgroundDataThis = new(1000d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualData));
            UpdateBackgroundDataRunTime = new(1d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualDataRunTime));
            UpdateSearchHintCommand = new(() =>
            {
            });
            BackgroundUpdateVisualData();
            #endregion

            #region SetParameteres
            AllHintNames = [];
            TextBlockRegister.Text = Flags.FlagRegisterState ? "A" : "a";
            BrowserPageColumn.MaxWidth = 0d;
            IELMessageMain.Opacity = 0d;
            IELActionPanelMain.Opacity = 0d;
            BorderHintCommand.Height = 0d;
            GridHintCommandParameter.Opacity = 0d;
            Canvas.SetZIndex(GridHintCommandParameter, -1);
            RichTextBoxMainMessage.Document = new();
            SettingsMain = new(RichTextBoxMainMessage, Pages.PageMainActPanel, new(270d, 230d));

            EnumerateNameCommand.AddRange(App.DataConsoleCommand.Select((i) => i.Name));
            EnumerateNameCommand.AddRange(App.CurrentApp.DataAliases.Select((i) => i.Name));

            Canvas.SetZIndex(IELMessageMain, -2);
            Canvas.SetZIndex(IELActionPanelMain, -2);
            #endregion

            ButtonReboot.OnActivateMouseLeft += App.RebootApplication;
            ButtonReturnCommand.OnActivateMouseLeft += () => ActivateActionCommand(TextBoxCommandInput.Text, true);
            SizeChanged += (sender, e) => IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);

            UpdateImageMenu();
            //Closing += (sender, e) => App.Current.Shutdown(0);

            #region UpToolButtons
            IELButtonLabel.OnActivateMouseLeft += () =>
            {
                if (!Flags.FlagFrameComponentVisible) UsingChangeStateFrameComponent();
                IELBrowserPageMain.AddInlayPage(PageLabelsApplication ??= new(), "Ярлыки",
                        "Ярлыки которые предаставляются программой для хранения важных команд");
            };

            IELButtonSettings.OnActivateMouseLeft += () =>
            {
                new WindowSetting().ShowDialog();
            };
            #endregion

            IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                TextBoxCommandInput.Focus();
            };

            IELBrowserPageMain.EventCloseBrowser += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            IELBrowserPageMain.EventChangeActiveInlay += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };

            #region IELButtonFrameComponentVisible
            IELButtonBrowserPageVisible.RenderTransform = new TransformGroup();
            ((TransformGroup)IELButtonBrowserPageVisible.RenderTransform).Children.Add(new RotateTransform(0d, 0d, 0d));

            IELButtonBrowserPageVisible.OnActivateMouseLeft += () => 
            {
                UsingChangeStateFrameComponent();
                IELMessageMain.CloseBorderInformation();
            };
            IELButtonBrowserPageVisible.OnActivateMouseRight += () =>
            {
                //FrameComponent.CloseFrame();
                IELMessageMain.CloseBorderInformation();
            };
            IELButtonBrowserPageVisible.MouseHover += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELButtonBrowserPageVisible, IELButtonBrowserPageVisible.Name,
                    (Flags.FlagFrameComponentVisible ? "Скрыть" : "Показать") + " глобальные страницы",
                    IELBlockMessage.OrientationBorderInfo.LeftUp);
            };
            IELButtonBrowserPageVisible.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            UsingChangeStateFrameComponent();
            #endregion

            TextBoxCommandInput.PreviewKeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Back:
                        if (TextBoxCommandInput.Text.Length > 0)
                        {
                            if (TextBoxCommandInput.Text[^1] == '*')
                            {
                                UsingAnimateBorderHintCommand(false);
                            }
                        }
                        return;
                }
            };
            TextBoxCommandInput.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(160, 245, 200), TimeSpan.FromMilliseconds(90d)));
                        UsingAnimateBorderCollectionHintCommand(false);
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(255, 122, 84), TimeSpan.FromMilliseconds(90d)));
                        UsingAnimateBorderCollectionHintCommand(false);
                        break;
                    case Key.LeftShift:
                    case Key.RightShift:
                        ShiftDowningCommandTextInput = true;
                        return;
                }
            };
            TextBoxCommandInput.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        ActivateActionCommand(TextBoxCommandInput.Text, true);
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Text = string.Empty;
                        break;
                    case Key.Apps:
                        IELActionPanelMain.UsingPanelAction(SettingsMain);
                        break;
                    case Key.LeftShift:
                    case Key.RightShift:
                        ShiftDowningCommandTextInput = false;
                        return;
                }
                TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(120, 204, 160), TimeSpan.FromMilliseconds(430d)));
                
                DoubleAnimation animation = DoubleAnimateObj.Clone();
                animation.Duration = TimeSpan.FromMilliseconds(300d);
                if (TextBoxCommandInput.Text.Length > 0)
                {
                    if (ShiftDowningCommandTextInput && e.Key == Key.D8)
                    {
                        if (GridHint.Children.Count == 1) UsingAnimateBorderHintCommand(true);
                        else UsingAnimateBorderCollectionHintCommand(false);
                        return;
                    }
                }
                if (!TextBoxCommandInput.Text.Contains('*')) UsingAnimateBorderCollectionHintCommand(TextBoxCommandInput.Text.Length > 0);
            };

            RichTextBoxMainMessage.MouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
                else if (e.ChangedButton == MouseButton.Right) IELActionPanelMain.UsingPanelAction(SettingsMain);
            };

            RichTextBoxMainMessage.TextChanged += (sender, e) =>
            {
                RichTextBoxMainMessage.ScrollToEnd();
            };

            #region BorderInternetConnection
            BorderInternetConnection.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(BorderInternetConnection, BorderInternetConnection.Name, Flags.FlagInternetConnection ?
                    "Есть подключение к интернету" : "Нет подключения к интернету",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            BorderInternetConnection.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region BorderStateRegister
            BorderStateRegister.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(BorderStateRegister, BorderStateRegister.Name, Flags.FlagRegisterState ?
                    "Установлен большой регистр" : "Установлен малый регистр",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            BorderStateRegister.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region BorderCurrentLanguage
            BorderCurrentLanguage.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(BorderCurrentLanguage, BorderCurrentLanguage.Name,
                    "Текущий язык раскладки клавиатуры",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            BorderCurrentLanguage.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region IELImageButtonHelp
            IELImageButtonHelp.OnActivateMouseLeft += UsingDiscriptionCommand;
            IELImageButtonHelp.MouseHover += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELImageButtonHelp, IELImageButtonHelp.Name,
                    "Быстрое открытие описания команд",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            IELImageButtonHelp.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion

            ImageLogoApplication.MouseEnter += (sender, e) =>
            {
                DoubleAnimateObj.To = 0.6d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };

            ImageLogoApplication.MouseLeave += (sender, e) =>
            {
                DoubleAnimateObj.To = 1d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };

            ImageLogoApplication.MouseDown += (sender, e) =>
            {
                DoubleAnimateObj.To = 0.4d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };

            ImageLogoApplication.MouseUp += (sender, e) =>
            {
                DoubleAnimateObj.To = 1d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
                Dialogs.LicenseWindow License = new();
                License.ShowDialog();
            };
            KeyDown += (sender, e) =>
            {
                if (e.Key == Key.CapsLock) Flags.FlagRegisterState.Value = Console.CapsLock;
            };
            Activated += (sender, e) =>
            {
                if (!HiAnimation)
                {
                    HiAnimation = true;

                    #region Anim Start
                    #region 1
                    ThicknessAnimate.Duration = TimeSpan.FromMilliseconds(1400d);

                    ThicknessAnimate.From = new(8);
                    ThicknessAnimate.To = BorderImageInformation.Margin;
                    BorderImageInformation.BeginAnimation(MarginProperty, ThicknessAnimate);

                    TimeDataColumnDefinition.MaxWidth = 0d;
                    DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(1200d);
                    DoubleAnimateObj.To = 124d;
                    Storyboard storyboard = new();
                    storyboard.Children.Add(DoubleAnimateObj);
                    Storyboard.SetTarget(DoubleAnimateObj, TimeDataColumnDefinition);
                    Storyboard.SetTargetProperty(DoubleAnimateObj, new PropertyPath("(ColumnDefinition.MaxWidth)"));
                    storyboard.Begin();
                    DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(250d);

                    ThicknessAnimate.From = new(8);
                    ThicknessAnimate.To = BorderDateTime.Margin;
                    BorderDateTime.BeginAnimation(MarginProperty, ThicknessAnimate);

                    ThicknessAnimate.From = null;
                    ThicknessAnimate.Duration = TimeSpan.FromMilliseconds(1400d);

                    #endregion
                    #endregion
                }
                //TextBoxCommandInput.Focus();
                /*GridMain.RenderTransform = new TransformGroup()
                {
                    Children = [
                        new RotateTransform(9d),
                        new ScaleTransform(0.3d, 0.3d)
                        ]
                };*/
                /*DoubleAnimateObj.To = 0d;
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(1200d);
                ((RotateTransform)((TransformGroup)GridMain.RenderTransform).Children[0]).BeginAnimation(RotateTransform.AngleProperty, DoubleAnimateObj);
                DoubleAnimateObj.To = 1d;
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(1200d);
                ((ScaleTransform)((TransformGroup)GridMain.RenderTransform).Children[1]).BeginAnimation(ScaleTransform.ScaleXProperty, DoubleAnimateObj);
                ((ScaleTransform)((TransformGroup)GridMain.RenderTransform).Children[1]).BeginAnimation(ScaleTransform.ScaleYProperty, DoubleAnimateObj);
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(300d);*/
            };

            UpdateBackgroundDataThis.TimerDataUpdate.Start();
            UpdateBackgroundDataRunTime.TimerDataUpdate.Start();
            TextBoxCommandInput.Focus();
        }

        /// <summary>
        /// Манипулировать анимацией борьера подсказок к командам через всю коллекцию
        /// </summary>
        /// <param name="Activate">Активировать или дизактивировать аинмацией</param>
        private void UsingAnimateBorderCollectionHintCommand(bool Activate)
        {
            DoubleAnimation animation = DoubleAnimateObj.Clone();
            animation.Duration = TimeSpan.FromMilliseconds(300d);
            GridHint.Children.Clear();
            if (Activate)
            {
                AllHintNames = [.. EnumerateNameCommand.Where((i) => { return i.Contains(TextBoxCommandInput.Text, StringComparison.CurrentCultureIgnoreCase); })];
                if (AllHintNames.Length == GridHint.Children.Count) return;
                Sorting.SortNames(ref AllHintNames);
                foreach (string Name in AllHintNames)
                {
                    TextBlock block = CreateHintBlock(Name, GridHint.Children.Count);
                    GridHint.Children.Add(block);
                }
            }
            if (Canvas.GetZIndex(GridHintCommandParameter) == 1 && !Activate) UsingAnimateBorderHintCommand(false);
            animation.To = Activate ? GridHint.Children.Count * HeightHintElement : 0d;
            BorderHintCommand.BeginAnimation(HeightProperty, animation);
        }

        /// <summary>
        /// Манипулировать анимацией борьера подсказок к командам
        /// </summary>
        /// <param name="Activate">Активировать или дизактивировать аинмацией</param>
        /// <param name="CommandTextActualHint">Константный текст поиска команды</param>
        private void UsingAnimateBorderHintCommand(bool Activate, string? CommandTextActualHint = null)
        {
            DoubleAnimation animation = DoubleAnimateObj.Clone();
            animation.Duration = TimeSpan.FromMilliseconds(300d);
            if (Activate)
            {
                ICommandAAC? CommandHint = null;
                string TextCommand = CommandTextActualHint ?? ((TextBlock)GridHint.Children[0]).Text;
                CommandHint ??= ICommandAAC.ReadCommand([.. App.DataConsoleCommand], TextCommand);
                CommandHint ??= ICommandAAC.ReadCommand([.. App.CurrentApp.DataAliases], TextCommand);
                if (CommandHint == null) return;
                Parameter[] Parameters = CommandHint.Parameters ?? [];
                TextBlockHintCommand.Text = $"{CommandHint.Name}* ";
                for (int i = 0; i < Parameters.Length; i++)
                {
                    TextBlockHintCommand.Text += $"{Parameters[i].Name}" +
                        $"{(Parameters[i].Absolutly ? string.Empty : '?')}" +
                        $"{(i < Parameters.Length - 1 ? ", " : string.Empty)}";
                }
                animation.To = HeightHintElement;
                BorderHintCommand.BeginAnimation(HeightProperty, animation);
            }
            animation.To = Activate ? 1d : 0d;
            Canvas.SetZIndex(GridHintCommandParameter, Activate ? 1 : -1);
            GridHintCommandParameter.BeginAnimation(OpacityProperty, animation);
            animation.To = Activate ? 300d : 142d;
            BorderHintCommand.BeginAnimation(WidthProperty, animation);
        }

        /// <summary>
        /// Создать объект подсказки к команде
        /// </summary>
        /// <param name="Name">Имя команды</param>
        /// <param name="Index">Индекс местоположения по оси Y</param>
        /// <returns>Объект подсказки к команде</returns>
        private TextBlock CreateHintBlock(string Name, int Index)
        {
            ColorAnimation color_animation = ColorAnimate.Clone();
            color_animation.Duration = TimeSpan.FromMilliseconds(120d);
            TextBlock Result = new()
            {
                Height = HeightHintElement,
                Text = Name,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new(0, HeightHintElement * Index, 0, 0),
                Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255)),
                Cursor = Cursors.Hand,
            };
            Result.MouseEnter += (sender, e) =>
            {
                color_animation.To = Color.FromArgb(100, 255, 255, 255);
                Result.Background.BeginAnimation(SolidColorBrush.ColorProperty, color_animation);
            };
            Result.MouseLeave += (sender, e) =>
            {
                color_animation.To = Color.FromArgb(0, 255, 255, 255);
                Result.Background.BeginAnimation(SolidColorBrush.ColorProperty, color_animation);
            };
            Result.MouseLeftButtonUp += (sender, e) =>
            {
                TextBoxCommandInput.Text = $"{Result.Text}*";
                UsingAnimateBorderHintCommand(true, Result.Text);
            };
            return Result;
        }

        /// <summary>
        /// Взаимодействовать с окном описания команд (Включает/Активирует)
        /// </summary>
        private void UsingDiscriptionCommand()
        {
            if (App.AppWindows.DiscriptionCommands == null)
            {
                App.AppWindows.DiscriptionCommands = new();
                App.AppWindows.DiscriptionCommands.Show();
            }
            else
            {
                App.AppWindows.DiscriptionCommands.WindowState = WindowState.Normal;
                App.AppWindows.DiscriptionCommands.Activate();
            }
        }

        /// <summary>
        /// Добавить и отформатировать текст в консоль
        /// </summary>
        /// <param name="Text">Текст добавляемый в консоль</param>
        /// <param name="Formatted">Форматировать или нет</param>
        internal void AddTextInConsole(string Text, bool Formatted = true)
        {
            Paragraph Message = new();
            Message.Inlines.Add(new Bold(new Run(">>> ")));
            if (Formatted)
            {
                List<Inline> Inlines = [];
                foreach (Match match in RegexFormattedText().Matches(Text))
                {
                    Inlines.Add(FormattedTextDetect(match.Value));
                }
                Message.Inlines.AddRange(Inlines);
            }
            else Message.Inlines.Add(new Run(Text));
            RichTextBoxMainMessage.Document.Blocks.Add(Message);
        }

        /// <summary>
        /// Изменить формативность текста с учётом первых знаков
        /// </summary>
        /// <remarks>
        /// <code>
        /// %#FFFFFF** <b>Italic</b> **
        /// </code>
        /// ** <b>Bold</b> **
        /// <code></code>
        /// // <i>Italic</i> //
        /// <code></code>
        /// __ <u>UnderLine</u> __
        /// <code></code>
        /// </remarks>
        /// <param name="Text">Текст форматирования</param>
        /// <returns>Форматированный текст</returns>
        private static Inline FormattedTextDetect(string Text)
        {
            if (Text.Length == 0 || (Text.Length == 1 && Text[0] == '%')) return new Run(Text);
            if (Text[0] == '%') Text = Text[1..]; // удаление "%"
            SolidColorBrush? color = null;
            if (Text[0] == '#')
            {
                color = new((Color)ColorConverter.ConvertFromString(
                    RegexFormattedTextColor().Match(Text).Value));
                Text = Text[7..];
            }
            Inline Result = $"{Text[0]}{Text[^1]}" switch
            {
                "**" => new Bold(new Run(Text[2..^2])),
                "//" => new Italic(new Run(Text[2..^2])),
                "__" => new Underline(new Run(Text[2..^2])),
                _ => new Run(Text),
            };
            if (color != null) Result.Background = color;
            return Result;
        }

        /// <summary>
        /// Анимировать еффект блюра - сигнализируя изменение
        /// </summary>
        /// <param name="Effect">Объект эффекта анимации</param>
        /// <param name="Power">Сила блюра при старте</param>
        private static void AnimateBlurEffect(BlurEffect Effect, uint Power)
        {
            DoubleAnimation animation = DoubleAnimateObj.Clone();
            animation.Duration = TimeSpan.FromMilliseconds(700d);
            animation.From = Power;
            animation.To = 0d;
            Effect.BeginAnimation(BlurEffect.RadiusProperty, animation);
        }

        /// <summary>
        /// Изменить состояние глобальных страниц на противоположное
        /// </summary>
        private void UsingChangeStateFrameComponent()
        {
            DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(560d);
            DoubleAnimateObj.To = Flags.FlagFrameComponentVisible ? 180d : 0d;
            ((RotateTransform)((TransformGroup)IELButtonBrowserPageVisible.RenderTransform).Children[0]).
                BeginAnimation(RotateTransform.AngleProperty, DoubleAnimateObj);

            DoubleAnimateObj.To = Flags.FlagFrameComponentVisible ? 0d : 420d;
            Storyboard storyboard = new();
            storyboard.Children.Add(DoubleAnimateObj);
            Storyboard.SetTarget(DoubleAnimateObj, BrowserPageColumn);
            Storyboard.SetTargetProperty(DoubleAnimateObj, new PropertyPath("(ColumnDefinition.MaxWidth)"));
            storyboard.Begin();
            DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(250d);
            Flags.FlagFrameComponentVisible.Value = !Flags.FlagFrameComponentVisible;
            IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);
        }

        /// <summary>
        /// Активировать команду
        /// </summary>
        /// <param name="CommandString">Строка команды</param>
        /// <param name="AppendBufferCommand">Состояние добавления команды в буфер</param>
        private void ActivateActionCommand(string CommandString, bool AppendBufferCommand = true)
        {
            IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);
            if (CommandString.Length == 0) return;
            TextBoxCommandInput.Text = string.Empty;
            ConsoleCommand? Command = ICommandAAC.ReadCommand([.. App.DataConsoleCommand], CommandString);
            string Name = ICommandAAC.ReadNameCommand(CommandString);
            string[] Parameters = ICommandAAC.ReadParametersCommand(CommandString);

            if (AppendBufferCommand) InsertCommandFromBuffer(Name, CommandString);

            CommandStateResult result = Command == null ? CommandStateResult.FaledCommand(Name) : Command.ExecuteCommand(Parameters);
            if (result.State == ResultState.InvalidCommand)
            {
                AliasCommand<ICommandAAC>? Alias = ICommandAAC.ReadCommand([.. App.CurrentApp.DataAliases], CommandString);
                result = Alias == null ? CommandStateResult.FaledCommand(Name) : Alias.ExecuteCommand();
            }
            SummarizeCommandStateResult(result);
        }

        /// <summary>
        /// Активировать команду не добавляя в буфер
        /// </summary>
        /// <param name="CommandString">Строка команды</param>
        internal void ActivateActionCommand(string CommandString) => ActivateActionCommand(CommandString, false);

        /// <summary>
        /// Создать действие над итогом выполнения команды
        /// </summary>
        /// <param name="Result">Объект итога выполнения команды</param>
        [MTAThread()]
        internal void SummarizeCommandStateResult(CommandStateResult Result)
        {
            if (Result.State != ResultState.Complete && Result.Massage != null)
            {
                AddTextInConsole(Result.Massage);
            }
        }

        #region ManipulateBuffer
        /// <summary>
        /// Создать кнопку активации команды
        /// </summary>
        /// <param name="Name">Отображаемое имя</param>
        /// <param name="Command">Выполняющаяся команда</param>
        /// <returns>Кнопка выполняющая команду</returns>
        private IELButtonCommand CreateBufferButton(string Name, string Command)
        {
            IELButtonCommand Button = new(Name, Command, Pages.PageBufferActPanel.BufferCommand.Count)
            {
                Height = H,
                Margin = new(0, (H + 2) * Pages.PageBufferActPanel.BufferCommand.Count, 0, 0),
                Index = Pages.PageBufferActPanel.BufferCommand.Count,
            };
            Button.OnActivateMouseLeft += () =>
            {
                IELActionPanelMain.ClosePanelAction();
                SummarizeCommandStateResult(
                    ICommandAAC.ReadAndExecuteCommand(null, [.. App.DataConsoleCommand], Pages.PageBufferActPanel.BufferCommand[Button.Index]));
            };
            Button.OnActivateMouseRight += () =>
            {
                Pages.PageBufferActPanel.BufferCommand.Delete(Button.Index);
                Pages.PageBufferActPanel.TextBlockCounterBuffer.Text =
                    $"{Pages.PageBufferActPanel.BufferCommand.Count}/{Pages.PageBufferActPanel.BufferCommand.Length}";
                if (Pages.PageBufferActPanel.BufferCommand.Count == 0) Pages.PageBufferActPanel.IELButtonClearBuffer.IsEnabled = false;
            };
            return Button;
        }

        /// <summary>
        /// Добавить команду в буфер
        /// </summary>
        /// <param name="Name">Имя команды</param>
        /// <param name="Command">Строка команды</param>
        private void InsertCommandFromBuffer(string Name, string Command)
        {
            Pages.PageBufferActPanel.IELButtonClearBuffer.IsEnabled = true;
            if (Pages.PageBufferActPanel.BufferCommand.Count < Pages.PageBufferActPanel.BufferCommand.Length)
            {
                IELButtonCommand Button = CreateBufferButton(Name, Command);
                Pages.PageBufferActPanel.BufferCommand.Add(Command);
                Pages.PageBufferActPanel.GridBuffer.Children.Add(Button);
                Pages.PageBufferActPanel.ScrollBar.MaxUp(1);
            }
            else
            {
                Pages.PageBufferActPanel.BufferCommand.Add(Command);
                IELButtonCommand RealButton;
                for (int i = 0; i < Pages.PageBufferActPanel.GridBuffer.Children.Count - 1; i++)
                {
                    RealButton = (IELButtonCommand)Pages.PageBufferActPanel.GridBuffer.Children[i];
                    IELButtonCommand NextButton = (IELButtonCommand)Pages.PageBufferActPanel.GridBuffer.Children[i + 1];
                    RealButton.Text = NextButton.Text;
                    RealButton.TextCommand = NextButton.TextCommand;
                }
                RealButton = (IELButtonCommand)Pages.PageBufferActPanel.GridBuffer.Children[^1];
                RealButton.Text = Name;
                RealButton.TextCommand = Command;
            }
            Pages.PageBufferActPanel.TextBlockCounterBuffer.Text =
                $"{Pages.PageBufferActPanel.BufferCommand.Count}/{Pages.PageBufferActPanel.BufferCommand.Length}";
        }
        #endregion

        /// <summary>
        /// Функция обновления визуальной информации в данном окне 100
        /// </summary>
        private void BackgroundUpdateVisualData()
        {
            TextBlockTime.Text = RealTime;
            TextBlockData.Text = RealData;
            if (!App.InternetPinging.Wait)
            {
                if (IELMessageMain.FlagMessage && Flags.FlagInternetConnection.Value != App.InternetPinging &&
                    IELMessageMain.NameParentObject.Equals(BorderInternetConnection.Name))
                {
                    IELMessageMain.Opacity = 0d;
                    IELMessageMain.UsingBorderInformation(BorderInternetConnection, BorderInternetConnection.Name, App.InternetPinging ?
                    "Есть подключение к интернету" : "Нет подключения к интернету",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
                }
                Flags.FlagInternetConnection.Value = App.InternetPinging;
            }
        }

        /// <summary>
        /// Функция обновления визуальной информации в данном окне 60
        /// </summary>
        private void BackgroundUpdateVisualDataRunTime()
        {
            string LangName = System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture.NativeName[0..3].ToUpper();
            if (!LangName.Equals(TextBlockLanguage.Text))
            {
                TextBlockLanguage.Text = LangName;
                AnimateBlurEffect(BlurEffectLanguage, 10u);
            }
            //int Volume = (int)(Device.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioMeterInformation.MasterPeakValue * 1900);
            //if (Math.Abs(RectangleTest.Width - 50 - Volume) >= 13 && Volume != 0) Volume /= 5;
            //byte rgbValue = (byte)(2.55d * Volume);
            //TextBlockTest.Foreground = new SolidColorBrush(Color.FromRgb(rgbValue, rgbValue, rgbValue));
            //TextBlockTest.Text = $"Volume: {Volume}";
            //ImageTest.Margin = new(428 - (Volume / 2), 204 - (Volume / 2), 0, 0);
            //ImageTest.Width = 10 + Volume;
            //ImageTest.Height = 10 + Volume;

            /*try
            {
                Point MousePoint = Mouse.GetPosition(this);
                Point PointScreen = PointToScreen(new(0, 0));
                //TextBlockTest.Text = $"Point: {PointScreen.X}:{PointScreen.Y} - {MousePoint.X}:{MousePoint.Y} - {ActualWidth}:{ActualHeight}";
                if (-MousePoint.X == PointScreen.X && -MousePoint.Y == PointScreen.Y)
                {
                    return;
                }
                else MousePoint = new(
                    (MousePoint.X - (ActualWidth / 2)) / 3,
                    (MousePoint.Y - (ActualHeight / 2)) / 3);
                ImageMenu.Margin = new(MousePoint.X, MousePoint.Y, 0, 0);
            }
            catch { ImageMenu.Margin = new(0); }*/

        }

        #region ImageMenu
        /// <summary>
        /// Обновить фотовое изображение
        /// </summary>
        internal void UpdateImageMenu()
        {
            ImageIndificator.Opacity = 1d;
            string Path = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.PathMenuImage);
            BitmapImage BitmapImageMenu = new(
                new Uri(Path, UriKind.RelativeOrAbsolute));
            if (File.Exists(Path))
            {
                ComplitedInstallImageMenu(BitmapImageMenu);
                return;
            };
            BitmapImageMenu.DownloadCompleted += (sender, e) =>
            {
                ComplitedInstallImageMenu(BitmapImageMenu);
            };
            BitmapImageMenu.DownloadFailed += (sender, e) => FailedInstallImageMenu();
            BitmapImageMenu.DecodeFailed += (sender, e) => FailedInstallImageMenu();
        }

        /// <summary>
        /// Успешная установка картинки фона
        /// </summary>
        /// <param name="bitmap">Карта изображения</param>
        private void ComplitedInstallImageMenu(BitmapImage bitmap)
        {
            ImageMenu.Source = bitmap;
            ThicknessAnimation animationThickness = ThicknessAnimate.Clone();
            DoubleAnimation animationDouble = DoubleAnimateObj.Clone();

            animationDouble.From = 10d;
            animationDouble.To = 0d;
            animationDouble.Duration = TimeSpan.FromMilliseconds(2300d);

            animationThickness.From = new(-4);
            animationThickness.To = new(0);
            animationThickness.Duration = TimeSpan.FromMilliseconds(2300d);

            BlurEffectImageMenu.BeginAnimation(BlurEffect.RadiusProperty, animationDouble);
            ImageMenu.BeginAnimation(MarginProperty, animationThickness);

            animationDouble.From = 0d;
            animationDouble.To = 1d;
            ImageMenu.BeginAnimation(OpacityProperty, animationDouble);

            animationDouble.From = 1d;
            animationDouble.To = 0d;
            animationDouble.Duration = TimeSpan.FromMilliseconds(700d);
            ImageIndificator.BeginAnimation(OpacityProperty, animationDouble);
        }

        /// <summary>
        /// Неудачное завершение установки картинки фона
        /// </summary>
        private void FailedInstallImageMenu()
        {
            DoubleAnimation animationDouble = DoubleAnimateObj.Clone();
            Paragraph Message = new();
            Message.Inlines.Add(new Bold(new Run(">>> ")));
            Message.Inlines.Add(new Run("Не удалось загрузить фоновое изображение...")
            {
                Background = new SolidColorBrush(Colors.IndianRed)
            });
            RichTextBoxMainMessage.Document.Blocks.Add(Message);

            animationDouble.From = 1d;
            animationDouble.To = 0d;
            animationDouble.Duration = TimeSpan.FromMilliseconds(700d);
            ImageIndificator.BeginAnimation(OpacityProperty, animationDouble);
        }
        #endregion

        /// <summary>
        /// Функция регулярного выражения выделения текста в ковычках "текст"
        /// </summary>
        private static Regex StringCommandError(char symbol) => new($"([^\\{symbol}]+|\\{symbol}[^\\{symbol}]+\\{symbol}?)");

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        // Текст который является %#00FF00FF__%**регистрационным**__ и %#FFFFFF**может** %~~даже так~~ %--постоянно-- %__форматироваться__
        [GeneratedRegex(@"([^%]+|(\%(#[0-9A-F]{6})?)(\*{2}[^\*]+\*{2}|_{2}[^_]+_{2}|\/{2}[^\/]+\/{2})|\%)")]
        private static partial Regex RegexFormattedText();

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        // %   #FFFFFF   //%**d**//
        [GeneratedRegex(@"#[0-9A-F]{6}")]
        private static partial Regex RegexFormattedTextColor();
    }
}