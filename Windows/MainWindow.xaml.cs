using AAC20.Classes;
using AAC20.Classes.Flaging;
using AAC20.Windows;
using AAC20.Windows.Frames;
using AAC20.Windows.Pages.ActionPanel;
using AAC20.Windows.Pages.MainWindow;
using AAC20.Windows.Pages.Other;
using IEL;
using IEL.Classes;
using IEL.Interfaces.Core;
using Interpreter.Commands;
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

namespace AAC20
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
            /// Флаг активации правого нажатия с помощью кнопки CTRL в верхней панели кнопок
            /// </summary>
            internal static readonly Flag FlagCtrlActivateActionButtonUp = new(false);

            /// <summary>
            /// Флаг соеденения с интернетом
            /// </summary>
            internal static readonly Flag FlagInternetConnection = new(false);
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

            /// <summary>
            /// Страница кнопок верхней панели главного меню программы
            /// </summary>
            internal static readonly PageUpMainButtons PageMainButtonsUp = new();

            /// <summary>
            /// Страница всех ярлыков
            /// </summary>
            internal static readonly PageLabels PageObjLabelsAction = new();

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
                    if ((bool)param[0]) Pages.PageObjLabelsAction.AnimationLoadingStart();
                    else Pages.PageObjLabelsAction.AnimationLoadingStop();
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
                    Massage.Inlines.Add(new Run($"{App.BufferCommand.Count}/{App.BufferCommand.Length}:[{string.Join(',', App.BufferCommand.BufferElements)}]"));
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
                    Pages.PageObjLabelsAction.AddLabel(new((string)param[0], (string)param[2], (string)param[1]));
                    CounterScrollBar g = Pages.PageObjLabelsAction.ScrollBar;
                    Test.Text = $"Value:{g.Value} Max:{g.MaxValue}";
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion

                #region create_label
                new ConsoleCommand("create_label", "Открывает окно создания ярлыка",
                (Command, param) =>
                {
                    LabelAction? label = new WindowGenLabel().CreateLabel();
                    if (label != null) Pages.PageObjLabelsAction.AddLabel(label);
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
                        Process.Start(new System.Diagnostics.ProcessStartInfo(uri) { UseShellExecute = true });
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
                            Process.Start(new System.Diagnostics.ProcessStartInfo(uri) { UseShellExecute = true });
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
            ]);
            #endregion

            #region Label
            #endregion

            #region Event Flags
            /*Flags.FlagCtrlActivateActionButtonAltMode.ChangeStateFlag += (NewValue) =>
            {
                DoubleAnimateObj.To = NewValue ? 1d : 0d;
                //TextBlockRightButtonIndicatorKey.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };*/
            Flags.FlagCtrlActivateActionButtonUp.ChangeStateFlag += (NewValue) =>
            {
                DoubleAnimateObj.To = NewValue ? 1d : 0d;
                TextBlockRightButtonIndicatorKeyButtonsUp.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };

            Flags.FlagInternetConnection.ChangeStateFlag += (NewValue) =>
            {
                ImageInternetConnection.Source = new BitmapImage(new Uri($"/Windows/WindowsImages/Wifi{(NewValue ? "On" : "Off")}.png", UriKind.Relative));
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
            Pages.PageMainButtonsUp.IELButtonLabel.OnActivateMouseLeft += (key) =>
            {
                FrameComponent.NextPage(Pages.PageObjLabelsAction);
                
            };
            Pages.PageMainButtonsUp.IELButtonLabel.OnActivateMouseRight += (key) =>
            {
                FrameComponent.CloseFrame();

            };
            #endregion

            UpdateBackgroundDataThis = new(1000d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualData));
            //UpdateBackgroundDataRunTime = new(0.1d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualDataRunTime));
            ImageTest.Source = new BitmapImage(new Uri("C:/Users/killm/Рабочий стол/Main/Programm/С#/AAC20/Windows/WindowsImages/Logo02.png"));
            BackgroundUpdateVisualData();
            FrameButtonsUp.Navigate(Pages.PageMainButtonsUp);
            TextBlockRightButtonIndicatorKeyButtonsUp.Opacity = 0d;
            IELMessageMain.Opacity = 0d;
            IELActionPanelMain.Opacity = 0d;
            //FrameComponent.Opacity = 0d;
            RichTextBoxMainMessage.Document = new();
            SettingsMain = new(RichTextBoxMainMessage, Pages.PageMainActPanel, new(250d, 230d));

            Canvas.SetZIndex(IELMessageMain, -2);
            Canvas.SetZIndex(IELActionPanelMain, -2);

            ButtonReboot.OnActivateMouseLeft += () => App.RebootApplication();
            ButtonReturnCommand.OnActivateMouseLeft += () => ActivateActionCommand(TextBoxCommandInput.Text);
            SizeChanged += (sender, e) => IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);
            //Closing += (sender, e) => App.Current.Shutdown(0);

            if (Flags.FlagInternetConnection)
            {
                
                //Pages.PageObjLabelsAction.AddLabel(AACConverter.ConvertRegexToLabelAction("$Name;Command$\"Text\"~"));
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

            IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                TextBoxCommandInput.Focus();
            };

            /*TextBoxCommandInput.GotFocus += (sender, e) =>
            {
                if (Pages.PageMainButtonsUp.ModulePage.KeyboardMode) Pages.PageMainButtonsUp.ModulePage.KeyboardMode = false;
                if (Flags.ActionPanelActivate.Value)
                {
                    AnimationActionPanel(false, RichTextBoxMainMessage, SizeActiveActionPanel, PositionAnimActionPanel.CenterObject);
                    Flags.FlagCtrlActivateActionButtonAltMode.Value = false;
                }
            };*/

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
                    case Key.RightCtrl:
                        Pages.PageMainButtonsUp.ModulePage.KeyboardMode = true;
                        BorderButtonsUp.Focus();
                        return;
                    default:
                        return;
                }
                TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(120, 204, 160), TimeSpan.FromMilliseconds(430d)));
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
                ThicknessAnimate.To = new(3);
                DoubleAnimateObj.To = 0.6d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
                ImageLogoApplication.BeginAnimation(MarginProperty, ThicknessAnimate);
            };

            ImageLogoApplication.MouseLeave += (sender, e) =>
            {
                ThicknessAnimate.To = new(5);
                DoubleAnimateObj.To = 1d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
                ImageLogoApplication.BeginAnimation(MarginProperty, ThicknessAnimate);
            };

            ImageLogoApplication.MouseDown += (sender, e) =>
            {
                ThicknessAnimate.Duration = TimeSpan.FromMilliseconds(100d);
                ThicknessAnimate.To = new(6);
                DoubleAnimateObj.To = 0.4d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
                ImageLogoApplication.BeginAnimation(MarginProperty, ThicknessAnimate);
                ThicknessAnimate.Duration = TimeSpan.FromMilliseconds(300d);
            };

            ImageLogoApplication.MouseUp += (sender, e) =>
            {
                ThicknessAnimate.To = new(2);
                DoubleAnimateObj.To = 1d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
                ImageLogoApplication.BeginAnimation(MarginProperty, ThicknessAnimate);
                LicenseWindow License = new();
                License.ShowDialog();
            };

            FrameComponent.OpenFrame += () =>
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
            };
            FrameComponent.ClosingFrame += () =>
            {
                Canvas.SetZIndex(TextBlockNullFrameElement, 1);
                DoubleAnimateObj.To = 1d;
                TextBlockNullFrameElement.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };
            /*FrameComponent.ChangeElementPage += () =>
            {
                void SetZIndex(object? INsender, EventArgs INe)
                {
                    int Z = e.Content == null ? 1 : 0;
                    Canvas.SetZIndex(FrameComponent, e.Content == null ? 0 : 1);
                    Canvas.SetZIndex(TextBlockNullFrameElement, Z);
                    TextBlockNullFrameElement.Opacity = Z;
                    DoubleAnimateObj.Completed -= SetZIndex;
                    DoubleAnimateObj.FillBehavior = FillBehavior.HoldEnd;
                }
                bool NavigaitedPage = e.Content != null;
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(1300d);
                DoubleAnimateObj.To = NavigaitedPage ? 1d : 0d;
                FrameComponent.BeginAnimation(OpacityProperty, DoubleAnimateObj);
                if (NavigaitedPage)
                {
                    DoubleAnimateObj.To = 0d;
                    DoubleAnimateObj.FillBehavior = FillBehavior.Stop;
                    DoubleAnimateObj.Completed += SetZIndex;
                }
                else
                {
                    DoubleAnimateObj.To = 1d;
                }
                TextBlockNullFrameElement.BeginAnimation(OpacityProperty, DoubleAnimateObj);
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(250d);
                /*if (TextBlockNullFrameElement.Opacity < 1d && e.Content != null) return;
                if (e.Content != null)
                {
                    FrameComponent.Opacity = 0d;
                    DoubleAnimateObj.To = 1d;
                    FrameComponent.BeginAnimation(OpacityProperty, DoubleAnimateObj);
                }
                DoubleAnimateObj.To = e.Content == null ? 1d : 0d;
                DoubleAnimateObj.FillBehavior = FillBehavior.Stop;
                DoubleAnimateObj.Completed += SetZIndex;
                TextBlockNullFrameElement.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };*/

            Activated += (sender, e) =>
            {
                //TextBoxCommandInput.Focus();
                /*GridMain.RenderTransform = new TransformGroup()
                {
                    Children = [
                        new RotateTransform(9d),
                        new ScaleTransform(0.3d, 0.3d)
                        ]
                };
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
            if (App.BufferCommand.Count < App.BufferCommand.Length)
            {
                IELButtonCommand Button = new(Name, CommandString, App.BufferCommand.Count)
                {
                    Height = H,
                    Margin = new(0, (H + 2) * App.BufferCommand.Count, 0, 0),
                    Index = App.BufferCommand.Count,
                };
                Button.OnActivateMouseLeft += () =>
                {
                    IELActionPanelMain.ClosePanelAction();
                    App.MainWindowApplication.SummarizeCommandStateResult(
                        ConsoleCommand.ReadAndExecuteCommand(null, [.. App.DataConsoleCommand], App.BufferCommand[Button.Index]));
                };
                Button.OnActivateMouseRight += () =>
                {
                    App.BufferCommand.Delete(Button.Index);
                    Pages.PageBufferActPanel.TextBlockCounterBuffer.Text = $"{App.BufferCommand.Count}/{App.BufferCommand.Length}";
                    if (App.BufferCommand.Count == 0) Pages.PageBufferActPanel.IELButtonClearBuffer.IsEnabled = false;
                };
                App.BufferCommand.Add(CommandString);
                Pages.PageBufferActPanel.GridBuffer.Children.Add(Button);
                Pages.PageBufferActPanel.ScrollBar.MaxUp(1);
            }
            else
            {
                App.BufferCommand.Add(CommandString);
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
            Pages.PageBufferActPanel.TextBlockCounterBuffer.Text = $"{App.BufferCommand.Count}/{App.BufferCommand.Length}";
            SummarizeCommandStateResult(Command == null ? CommandStateResult.FaledCommand(Name) : Command.ExecuteCommand(Parameters));
        }

        [MTAThread()]
        internal void SummarizeCommandStateResult(CommandStateResult Result)
        {
            if (Result.State != ResultState.Complete && Result.Massage != null)
            {
                Paragraph P_Massage = new();
                foreach (Match Element in StringCommandError().Matches(Result.Massage))
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
                ImageTest.Margin = new(MousePoint.X, MousePoint.Y, 0, 0);
            }
            catch { ImageTest.Margin = new(0); }
            
        }

        [GeneratedRegex("([^\"]+|\"[^\"]+\"?)")]
        /// <summary>
        /// Функция регулярного выражения выделения текста в ковычках "текст"
        /// </summary>
        private static partial Regex StringCommandError();
    }
}