using AAC20.CORE;
using AAC20.CORE.Flaging;
using AAC20.CORE.Settings;
using AAC20.Windows;
using AAC20.Windows.Frames;
using AAC20.Windows.Pages.ActionPanel;
using AAC20.Windows.Pages.Other;
using IEL;
using IEL.Classes;
using IEL.Interfaces.Core;
using Interpreter.Commands;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
        /// Объект управления фоновым обновлением информации в данном окне 60
        /// </summary>
        //private readonly UpdateBackgroundData UpdateBackgroundDataRunTime;

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
        /// Состояние воспроизведения приветственной анимации
        /// </summary>
        private bool HiAnimation = false;

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
                "Выводит все введёные параметры начиная с параметра \"Text\" в консоль главного меню программы",
                (Command, param) =>
                {
                    Paragraph Massage = new();
                    Massage.Inlines.Add(new Bold(new Run(">>> ")));
                    Massage.Inlines.Add(new Run(string.Join('\0', param)));
                    RichTextBoxMainMessage.Document.Blocks.Add(Massage);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region buffer
                new ConsoleCommand("buffer",
                "Отображает содержание буфера команд в консоль главного меню программы",
                (Command, param) =>
                {
                    Paragraph Massage = new();
                    Massage.Inlines.Add(new Bold(new Run(">>> ")));
                    Massage.Inlines.Add(new Run($"{Pages.PageBufferActPanel.BufferCommand.Count}/{Pages.PageBufferActPanel.BufferCommand.Length}:" +
                        $"[{string.Join(',', Pages.PageBufferActPanel.BufferCommand.BufferElements.Where((i) =>
                        {
                            if (i != null)
                            {
                                return i.Length > 0;
                            }
                            return false;
                        }))}]"));
                    RichTextBoxMainMessage.Document.Blocks.Add(Massage);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region label
                new ConsoleCommand("label",
                [
                    new Parameter("Name", typeof(string)), new Parameter("Command", typeof(string)),
                    new Parameter("Description", typeof(string), string.Empty)
                ],
                "Создаёт ярлык с именем \"Name\" и командой \"Command\", можно создать описание не обязательным параметром \"Description\"",
                (Command, param) =>
                {
                    PageLabels? Page = IELBrowserPageMain.SearchPageType<PageLabels>();
                    if (Page == null)
                        return Task.FromResult(CommandStateResult.Failed(Command.Name,
                            $">>> Страница \"{nameof(PageLabels)}\" в браузере не инициализирована!"));
                    Page.AddLabel(new((string)param[0], (string)param[2], (string)param[1]));
                    //CounterScrollBar g = Pages.PageObjLabelsAction.ScrollBar;
                    //Test.Text = $"Value:{g.Value} Max:{g.MaxValue}";
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region create_label
                new ConsoleCommand("create_label", "Открывает окно создания ярлыка",
                (Command, param) =>
                {
                    PageLabels? Page = IELBrowserPageMain.SearchPageType<PageLabels>();
                    if (Page == null)
                        return Task.FromResult(CommandStateResult.Failed(Command.Name,
                            $">>> Страница \"{nameof(PageLabels)}\" в браузере не инициализирована!"));
                    LabelAction? label = new Dialogs.WindowGenLabel().CreateLabel();
                    if (label != null) Page.AddLabel(label);
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region open_link
                new ConsoleCommand("open_link", [new Parameter("Link", typeof(string), true)],
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
                "Открывает заданную директорию в проводнике. При отсутствии параметра будет открывать гравную страницу проводника",
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
            ]);
            #endregion

            #region Event Flags
            Flags.FlagInternetConnection.ChangeStateFlag += (NewValue) =>
            {
                ImageInternetConnection.Source = new BitmapImage(new Uri($"{App.PathImageApplication}/Wifi{(NewValue ? "On" : "Off")}.png", UriKind.Relative));
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(800d);
                DoubleAnimateObj.From = 10d;
                DoubleAnimateObj.To = 0d;
                BlurEffectImageInternetConnection.BeginAnimation(BlurEffect.RadiusProperty, DoubleAnimateObj);
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(250d);
                DoubleAnimateObj.From = null;
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
            };
            Pages.PageDeveloperState.IELButtonCreateLabel.OnActivateMouseLeft += () =>
            {
                //Pages.PageObjLabelsAction.AddLabel(new("Test", "Test", "Test"));
                //Pages.PageDeveloperState.TextBlockLabelsCount.Text = $"={Pages.PageObjLabelsAction.CountLabel}";
            };
            #endregion

            UpdateBackgroundDataThis = new(1000d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualData));
            //UpdateBackgroundDataRunTime = new(0.1d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualDataRunTime));
            BackgroundUpdateVisualData();
            BrowserPageColumn.MaxWidth = 0d;
            IELMessageMain.Opacity = 0d;
            IELActionPanelMain.Opacity = 0d;
            RichTextBoxMainMessage.Document = new();
            SettingsMain = new(RichTextBoxMainMessage, Pages.PageMainActPanel, new(270d, 230d));

            Canvas.SetZIndex(IELMessageMain, -2);
            Canvas.SetZIndex(IELActionPanelMain, -2);

            ButtonReboot.OnActivateMouseLeft += () => App.RebootApplication();
            ButtonReturnCommand.OnActivateMouseLeft += () => ActivateActionCommand(TextBoxCommandInput.Text);
            SizeChanged += (sender, e) => IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);
            //Closing += (sender, e) => App.Current.Shutdown(0);

            string PathImage = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.PathMenuImage);
            if (PathImage.Length > 0)
            {
                ImageIndificator.Opacity = 1d;
                BitmapImage bitmap = new(new Uri(PathImage, UriKind.RelativeOrAbsolute));
                if (bitmap.PixelWidth > 0 && bitmap.PixelHeight > 0)
                {
                    DoubleAnimation animationDouble = DoubleAnimateObj.Clone();
                    bitmap.DownloadCompleted += (sender, e) =>
                    {
                        ImageMenu.Source = bitmap;
                        ThicknessAnimation animationThickness = ThicknessAnimate.Clone();

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
                    };
                    bitmap.DownloadFailed += (sender, e) =>
                    {
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
                    };
                    bitmap.DecodeFailed += (sender, e) =>
                    {
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
                    };
                }
            }

            App.BufferCommand.DelElement += (index) =>
            {
                Pages.PageBufferActPanel.GridBuffer.Children.RemoveAt(index);
                Pages.PageBufferActPanel.ScrollBar.MaxDown(1);

                ThicknessAnimation AnimationBuffer = new(new Thickness(0), TimeSpan.FromMilliseconds(160d))
                {
                    EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut, Amplitude = 0.6d }
                };
                Thickness ThicknessIndex = new(0);
                for (int i = index; i < App.BufferCommand.Count; i++)
                {
                    IELButtonCommand Button = (IELButtonCommand)Pages.PageBufferActPanel.GridBuffer.Children[i];
                    Button.Index--;
                    AnimationBuffer.To = new Thickness(0, (H + 2) * i, 0, 0);
                    AnimationBuffer.BeginTime = TimeSpan.FromMilliseconds((i - index) * 20d);
                    Button.BeginAnimation(FrameworkElement.MarginProperty, AnimationBuffer);
                }
            };

            App.BufferCommand.ClearBuffer += () =>
            {
                Pages.PageBufferActPanel.GridBuffer.Children.Clear();
                Pages.PageBufferActPanel.ScrollBar.MaxClear();
            };

            IELButtonLabel.OnActivateMouseLeft += () =>
            {
                if (!Flags.FlagFrameComponentVisible) UsingChangeStateFrameComponent();
                //FrameComponent.NextPage(Pages.PageObjLabelsAction);
                IELBrowserPageMain.AddInlayPage(new PageLabels(), "Ярлыки",
                        "Ярлыки которые предаставляются программой для быстрого взаимодействия");
            };

            IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                TextBoxCommandInput.Focus();
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

            TextBoxCommandInput.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(160, 245, 200), TimeSpan.FromMilliseconds(90d)));
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(255, 122, 84), TimeSpan.FromMilliseconds(90d)));
                        break;
                }
            };
            TextBoxCommandInput.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        ActivateActionCommand(TextBoxCommandInput.Text);
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Text = string.Empty;
                        break;
                    case Key.Apps:
                        IELActionPanelMain.UsingPanelAction(SettingsMain);
                        break;
                    default:
                        return;
                }
                TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(120, 204, 160), TimeSpan.FromMilliseconds(430d)));
            };

            BorderButtonsUp.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.RightCtrl:
                        break;
                }
            };

            BorderButtonsUp.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        break;
                }
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

            /*FrameComponent.OpenFrame += () =>
            {
                DoubleAnimation animation = DoubleAnimateObj.Clone();
                void SetZIndex(object? INsender, EventArgs INe)
                {
                    Canvas.SetZIndex(TextBlockNullFrameElement, -1);
                    TextBlockNullFrameElement.Opacity = 0d;
                    animation.Completed -= SetZIndex;
                    animation.FillBehavior = FillBehavior.HoldEnd;
                }
                animation.To = 0d;
                animation.Completed += SetZIndex;
                animation.FillBehavior = FillBehavior.Stop;
                TextBlockNullFrameElement.BeginAnimation(OpacityProperty, animation);
            };

            FrameComponent.ChangeElementPage += (Name) =>
            {
                /*if (Name.Equals(Pages.PageObjLabelsAction.GetType().Name))
                {
                    if (Flags.FlagInternetConnection && !Pages.PageObjLabelsAction.SQLCompleteSearch)
                    {
                        Pages.PageObjLabelsAction.StartLoadSQL();
                    }
                }*/
            /*};
            FrameComponent.ClosingFrame += () =>
            {
                Canvas.SetZIndex(TextBlockNullFrameElement, 1);
                DoubleAnimateObj.To = 1d;
                TextBlockNullFrameElement.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };*/

            /*SizeChanged += (sender, e) =>
            {
                if (Pages.PageObjLabelsAction.GridMain.ActualHeight == 0d) return;
                int ScrollCountVisible = (int)(BorderFrameComponent.ActualHeight / 79d) * Pages.PageObjLabelsAction.ScrollBar.TrafficShare;
                Pages.PageDeveloperState.ListBoxDeveloper.Items[0] = $"[0] CountVisible= {ScrollCountVisible} : {Pages.PageObjLabelsAction.ScrollBar.CountVisibleElements}";
                Pages.PageDeveloperState.ListBoxDeveloper.Items[1] = $"[1] ActualHeight={Pages.PageObjLabelsAction.GridMain.ActualHeight}";
                if (ScrollCountVisible != Pages.PageObjLabelsAction.ScrollBar.CountVisibleElements)
                {
                    int Value = Math.Abs(ScrollCountVisible - Pages.PageObjLabelsAction.ScrollBar.CountVisibleElements) / Pages.PageObjLabelsAction.ScrollBar.TrafficShare;
                    if (ScrollCountVisible > Pages.PageObjLabelsAction.ScrollBar.CountVisibleElements)
                    {
                        Pages.PageObjLabelsAction.ScrollBar.VisibleUp(Value);
                    }
                    else if (ScrollCountVisible < Pages.PageObjLabelsAction.ScrollBar.CountVisibleElements)
                    {
                        Pages.PageObjLabelsAction.ScrollBar.VisibleDown(Value);
                    }
                }
            };*/

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
            TextBoxCommandInput.Focus();
            //UpdateBackgroundDataRunTime.TimerDataUpdate.Start();
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
        /// <param name="CommandString">Ктрока команды</param>
        private void ActivateActionCommand(string CommandString)
        {
            IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);
            if (CommandString.Length == 0) return;
            TextBoxCommandInput.Text = string.Empty;
            ConsoleCommand? Command = ConsoleCommand.ReadCommand([.. App.DataConsoleCommand], CommandString);
            string Name = ConsoleCommand.ReadNameCommand(CommandString);
            string[] Parameters = ConsoleCommand.ReadParametersCommand(CommandString);
            Pages.PageBufferActPanel.IELButtonClearBuffer.IsEnabled = true;
            if (Pages.PageBufferActPanel.BufferCommand.Count < Pages.PageBufferActPanel.BufferCommand.Length)
            {
                IELButtonCommand Button = new(Name, CommandString, Pages.PageBufferActPanel.BufferCommand.Count)
                {
                    Height = H,
                    Margin = new(0, (H + 2) * Pages.PageBufferActPanel.BufferCommand.Count, 0, 0),
                    Index = Pages.PageBufferActPanel.BufferCommand.Count,
                };
                Button.OnActivateMouseLeft += () =>
                {
                    IELActionPanelMain.ClosePanelAction();
                    SummarizeCommandStateResult(
                        ConsoleCommand.ReadAndExecuteCommand(null, [.. App.DataConsoleCommand], App.BufferCommand[Button.Index]));
                };
                Button.OnActivateMouseRight += () =>
                {
                    Pages.PageBufferActPanel.BufferCommand.Delete(Button.Index);
                    Pages.PageBufferActPanel.TextBlockCounterBuffer.Text =
                        $"{Pages.PageBufferActPanel.BufferCommand.Count}/{Pages.PageBufferActPanel.BufferCommand.Length}";
                    if (Pages.PageBufferActPanel.BufferCommand.Count == 0) Pages.PageBufferActPanel.IELButtonClearBuffer.IsEnabled = false;
                };
                Pages.PageBufferActPanel.BufferCommand.Add(CommandString);
                Pages.PageBufferActPanel.GridBuffer.Children.Add(Button);
                Pages.PageBufferActPanel.ScrollBar.MaxUp(1);
            }
            else
            {
                Pages.PageBufferActPanel.BufferCommand.Add(CommandString);
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
                RealButton.TextCommand = CommandString;
            }
            Pages.PageBufferActPanel.TextBlockCounterBuffer.Text =
                $"{Pages.PageBufferActPanel.BufferCommand.Count}/{Pages.PageBufferActPanel.BufferCommand.Length}";
            SummarizeCommandStateResult(Command == null ? CommandStateResult.FaledCommand(Name) : Command.ExecuteCommand(Parameters));
        }

        [MTAThread()]
        internal void SummarizeCommandStateResult(CommandStateResult Result)
        {
            if (Result.State != ResultState.Complete && Result.Massage != null)
            {
                Paragraph P_Massage = new();
                foreach (Match Element in StringCommandError('"').Matches(Result.Massage))
                {
                    if ((Element.Value[0], Element.Value[^1]) == ('"', '"'))
                    {
                        P_Massage.Inlines.Add(new Italic(new Run(Element.Value)) { Background = new SolidColorBrush(Colors.IndianRed) });
                        continue;
                    }
                    P_Massage.Inlines.Add(new Run(Element.Value));
                }
                RichTextBoxMainMessage.Document.Blocks.Add(P_Massage);
            }
        }

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
            //int Volume = (int)(Device.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioMeterInformation.MasterPeakValue * 1900);
            //if (Math.Abs(RectangleTest.Width - 50 - Volume) >= 13 && Volume != 0) Volume /= 5;
            //byte rgbValue = (byte)(2.55d * Volume);
            //TextBlockTest.Foreground = new SolidColorBrush(Color.FromRgb(rgbValue, rgbValue, rgbValue));
            //TextBlockTest.Text = $"Volume: {Volume}";
            //ImageTest.Margin = new(428 - (Volume / 2), 204 - (Volume / 2), 0, 0);
            //ImageTest.Width = 10 + Volume;
            //ImageTest.Height = 10 + Volume;

            try
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
            catch { ImageMenu.Margin = new(0); }
            
        }

        /// <summary>
        /// Функция регулярного выражения выделения текста в ковычках "текст"
        /// </summary>
        private static Regex StringCommandError(char symbol) => new($"([^\\{symbol}]+|\\{symbol}[^\\{symbol}]+\\{symbol}?)");
    }
}