using AAC20.Classes;
using AAC20.Classes.Flaging;
using AAC20.GUI;
using AAC20.Interfaces;
using AAC20.Windows;
using AAC20.Windows.Frames;
using AAC20.Windows.Pages.ActionPanel;
using AAC20.Windows.Pages.MainWindow;
using Interpreter.Commands;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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
            /// Флаг состояния активности панели действий в главной консоли
            /// </summary>
            internal static readonly Flag ActionPanelActivate = new(false);

            /// <summary>
            /// Флаг состояния активации кнопки панели действий через клавишу клавиатуры
            /// </summary>
            internal static readonly Flag ActionPanelActivateButtonAltMode = new(false);

            /// <summary>
            /// Флаг активации правого нажатия с помощью кнопки CTRL в панели действий
            /// </summary>
            internal static readonly Flag FlagCtrlActivateActionButtonAltMode = new(false);

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
        }

        /// <summary>
        /// Перечисление вариаций вычисления позиций панели действий
        /// </summary>
        private enum PositionAnimActionPanel
        {
            /// <summary>
            /// Обычное вычисление по курсору
            /// </summary>
            Default = 0,

            /// <summary>
            /// Вычисление цента объекта
            /// </summary>
            CenterObject = 1,
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
        /// Объект анимации для управления размерами панели действий
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimateActionPanelWH = new(0, TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления прозрачностью
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimateObj = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Размер активной панели действий
        /// </summary>
        private readonly Size SizeActiveActionPanel;

        /// <summary>
        /// Вложенность панели действий для анимации
        /// </summary>
        private int PanelVerschachtelung = 0;

        /// <summary>
        /// Ссылка на активную страницу панели действий
        /// </summary>
        [NotNull()]
        private IPageModuleButtonKeyAAC RefPageActionPanel;

        /// <summary>
        /// Ссылка на страницу верхней панели главного меню
        /// </summary>
        private static IPageModuleButtonKeyAAC RefPageButtonsUp => Pages.PageMainButtonsUp;

        /// <summary>
        /// Константа размера Height для кнопок буфера
        /// </summary>
        [NotNull()]
        const int H = 33;

        //private MMDeviceEnumerator Device = new();

        public MainWindow()
        {
            InitializeComponent();

            #region Command
            App.DataConsoleCommand.AddRange([

                new ConsoleCommand("clear",
                "Очищает текстовый вывод главного меню программы",
                (Command, param) =>
                {
                    RichTextBoxMainMessage.Document = new();
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),

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

                new ConsoleCommand("label", [new Parameter("Value", typeof(bool))],
                "Изменяет состояние объекта по его \"Value\"",
                (Command, param) =>
                {
                    Test.IsEnabled = (bool)param[0];
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),

            ]);
            #endregion

            Flags.FlagCtrlActivateActionButtonAltMode.ChangeStateFlag += (NewValue) =>
            {
                DoubleAnimateObj.To = NewValue ? 1d : 0d;
                TextBlockRightButtonIndicatorKey.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };
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

            Pages.PageMainActPanel.IELButtonCrearConsole.OnActivateMouseLeft += (AltMode) =>
            {
                RichTextBoxMainMessage.Document = new();
                AnimationActionPanel(false);
            };
            Pages.PageMainActPanel.IELButtonCrearConsole.OnActivateMouseRight += (AltMode) =>
            {
                RichTextBoxMainMessage.Document = new();
            };

            Pages.PageMainActPanel.IELButtonCommandBuffer.OnActivateMouseLeft += (AltMode) =>
            {
                NextPageInActtionPanel(Pages.PageBufferActPanel, AltMode);
            };

            Pages.PageBufferActPanel.IELButtonBackMainMenu.OnActivateMouseLeft += (AltMode) =>
            {
                NextPageInActtionPanel(Pages.PageMainActPanel, AltMode, false);
            };

            Pages.PageMainActPanel.IELButtonDiscriptionCommand.OnActivateMouseLeft += (Key) =>
            {
                AnimationActionPanel(false);
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

            UpdateBackgroundDataThis = new(1000d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualData));
            //UpdateBackgroundDataRunTime = new(0.1d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualDataRunTime));
            //ImageTest.Source = new BitmapImage(new Uri("https://sun9-46.userapi.com/impg/euj8JteQPLq-XpWDbR03hU2Dlz3IhzwLs4W9DA/bYNM9VcaP-w.jpg?size=800x800&quality=95&sign=b761945cee478f88087602b209cff6f9&type=album"));
            ImageTest.Source = new BitmapImage(new Uri("C:/Users/killm/Рабочий стол/Main/Programm/С#/AAC20/Windows/WindowsImages/Logo02.png"));
            //ImageInternetConnection.Source = new BitmapImage(new Uri("/Windows/WindowsImages/WifiOn.png", UriKind.Relative));
            BackgroundUpdateVisualData();
            FrameActionPanelLeft.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameActionPanelRight.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            RefPageActionPanel = Pages.PageMainActPanel;
            FrameActionPanelLeft.Navigate(Pages.PageMainActPanel);
            FrameButtonsUp.Navigate(Pages.PageMainButtonsUp);
            TextBlockRightButtonIndicatorKey.Opacity = 0d;
            TextBlockRightButtonIndicatorKeyButtonsUp.Opacity = 0d;
            RichTextBoxMainMessage.Document = new();
            SizeActiveActionPanel = new(BorderActionPanel.Width, BorderActionPanel.Height);
            BorderActionPanel.Width = 0;
            BorderActionPanel.Height = 0;

            ButtonReboot.OnActivateMouseLeft += (key) => App.RebootApplication();
            ButtonReturnCommand.OnActivateMouseLeft += (key) => ActivateActionCommand(TextBoxCommandInput.Text);
            SizeChanged += (sender, e) => AnimationActionPanel(false, PositionAnimActionPanel.CenterObject);
            //Closing += (sender, e) => App.Current.Shutdown(0);

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
                    Button.TextBlockNumberCommand.Text = $"#{i + 1}";
                    Button.IndexElement--;
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

            BorderActionPanel.KeyDown += (sender, e) =>
            {
                if (RefPageActionPanel.AltMode && e.Key != Key.Z && e.Key != Key.RightCtrl && !Flags.ActionPanelActivateButtonAltMode.Value)
                {
                    RefPageActionPanel.BlinkActivateIELButtonTextInKey(e.Key,
                        Flags.FlagCtrlActivateActionButtonAltMode.Value ?
                        IPageModuleButtonKeyAAC.OrientationActivate.RightButton :
                        IPageModuleButtonKeyAAC.OrientationActivate.LeftButton);
                    Flags.ActionPanelActivateButtonAltMode.Value = true;
                }
                if (e.Key == Key.RightCtrl && RefPageActionPanel.AltMode) Flags.FlagCtrlActivateActionButtonAltMode.Value = true;
            };

            BorderActionPanel.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        if (!Flags.FlagCtrlActivateActionButtonAltMode)
                            AnimationActionPanel(false, PositionAnimActionPanel.CenterObject);
                        return;
                    case Key.Z:
                        if (!Flags.FlagCtrlActivateActionButtonAltMode)
                            RefPageActionPanel.AltMode = !RefPageActionPanel.AltMode;
                        return;
                    case Key.RightCtrl:
                        if (!Flags.ActionPanelActivateButtonAltMode.Value)
                            Flags.FlagCtrlActivateActionButtonAltMode.Value = false;
                        return;
                    default: break;
                }
                if (!Flags.ActionPanelActivateButtonAltMode.Value) return;
                if (RefPageActionPanel.AltMode)
                {
                    RefPageActionPanel.ActivateIELButtonTextInKey(e.Key,
                    Flags.FlagCtrlActivateActionButtonAltMode ?
                    IPageModuleButtonKeyAAC.OrientationActivate.RightButton : IPageModuleButtonKeyAAC.OrientationActivate.LeftButton);
                }
                Flags.FlagCtrlActivateActionButtonAltMode.Value = false;
                Flags.ActionPanelActivateButtonAltMode.Value = false;
            };

            TextBoxCommandInput.GotFocus += (sender, e) =>
            {
                if (RefPageButtonsUp.AltMode) RefPageButtonsUp.AltMode = false;
                if (Flags.ActionPanelActivate.Value)
                {
                    AnimationActionPanel(false, PositionAnimActionPanel.CenterObject);
                    Flags.FlagCtrlActivateActionButtonAltMode.Value = false;
                }
            };

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
                        AnimationActionPanel(true);
                        break;
                    case Key.RightCtrl:
                        RefPageButtonsUp.AltMode = true;
                        BorderButtonsUp.Focus();
                        return;
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
                        Flags.FlagCtrlActivateActionButtonUp.Value = true;
                        break;
                    default:
                        break;
                }
            };

            BorderButtonsUp.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        RefPageButtonsUp.AltMode = false;
                        TextBoxCommandInput.Focus();
                        break;
                    case Key.RightCtrl:
                        Flags.FlagCtrlActivateActionButtonUp.Value = false;
                        break;
                    default:
                        break;
                }
            };

            RichTextBoxMainMessage.MouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && Flags.ActionPanelActivate.Value) AnimationActionPanel(false);
                else if (e.ChangedButton == MouseButton.Right)
                {
                    if (!Flags.ActionPanelActivate.Value) AnimationActionPanel(true);
                    else AnimationMoveActionPanel(PositionAnimActionPanel.Default);
                }
            };

            RichTextBoxMainMessage.TextChanged += (sender, e) =>
            {
                RichTextBoxMainMessage.ScrollToEnd();
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

            Test.OnActivateMouseRight += (Key) =>
            {
                //Test.IsEnabled = false;
            };

            Activated += (sender, e) =>
            {
                TextBoxCommandInput.Focus();
            };

            UpdateBackgroundDataThis.TimerDataUpdate.Start();
            //UpdateBackgroundDataRunTime.TimerDataUpdate.Start();
        }

        /// <summary>
        /// Перенаправить страницу панели
        /// </summary>
        /// <param name="Content">Новая страница панели</param>
        /// <param name="RightAlign">Правая ориентация движения</param>
        private void NextPageInActtionPanel(
            [DoesNotReturnIf(false), NotNull()] IPageModuleButtonKeyAAC Content,
            bool AltMode, bool RightAlign = true)
        {
            Frame OldFrameAnim = PanelVerschachtelung % 2 == 0 ? FrameActionPanelLeft : FrameActionPanelRight;
            Frame NewFrameAnim = !(PanelVerschachtelung % 2 == 0) ? FrameActionPanelLeft : FrameActionPanelRight;
            NewFrameAnim.Opacity = 0;
            Canvas.SetZIndex(OldFrameAnim, 0);
            Canvas.SetZIndex(NewFrameAnim, 1);
            OldFrameAnim.IsEnabled = false;
            NewFrameAnim.IsEnabled = true;
            NewFrameAnim.BeginAnimation(MarginProperty, null);
            NewFrameAnim.Margin = !RightAlign ? new(-20, -20, 40, -3) : new(40, -10, -20, -3);
            Content.AltMode = AltMode;
            RefPageActionPanel = Content;
            NewFrameAnim.Navigate(Content);

            DoubleAnimateObj.To = 0;
            OldFrameAnim.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            ThicknessAnimate.To = !RightAlign ? new(40, -20, -20, -3) : new(-20, -20, 40, -3);
            OldFrameAnim.BeginAnimation(MarginProperty, ThicknessAnimate);

            DoubleAnimateObj.To = 1;
            NewFrameAnim.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            ThicknessAnimate.To = new(0);
            NewFrameAnim.BeginAnimation(MarginProperty, ThicknessAnimate);

            PanelVerschachtelung = (PanelVerschachtelung + 1) % 2;
        }

        /// <summary>
        /// Анимировать изменение состояния панель действий
        /// </summary>
        /// <param name="State">Состояние панели</param>
        /// <param name="StylePositionAnimate">Стиль анимации позиции</param>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private void AnimationActionPanel(bool State, PositionAnimActionPanel StylePositionAnimate = PositionAnimActionPanel.Default)
        {
            if (State == Flags.ActionPanelActivate.Value) return;
            if (State) BorderActionPanel.Focus();
            else
            {
                RefPageActionPanel.AltMode = false;
                TextBoxCommandInput.Focus();
            }
            Flags.ActionPanelActivate.Value = State;
            AnimationMoveActionPanel(StylePositionAnimate);
            DoubleAnimateActionPanelWH.To = State ? SizeActiveActionPanel.Width : 0d;
            BorderActionPanel.BeginAnimation(WidthProperty, DoubleAnimateActionPanelWH);
            DoubleAnimateActionPanelWH.To = State ? SizeActiveActionPanel.Height : 0d;
            BorderActionPanel.BeginAnimation(HeightProperty, DoubleAnimateActionPanelWH);
            DoubleAnimateObj.To = State ? 1d : 0d;
            BorderActionPanel.BeginAnimation(OpacityProperty, DoubleAnimateObj);
        }

        /// <summary>
        /// Анимировать передвижение панели действий константно
        /// </summary>
        /// <param name="StylePositionToAnimate">Вид вычисления позиции позиции анимации</param>
        private void AnimationMoveActionPanel(PositionAnimActionPanel StylePositionToAnimate)
        {
            if (StylePositionToAnimate == PositionAnimActionPanel.Default)
            {
                Point MousePoint = Mouse.GetPosition(RichTextBoxMainMessage);
                if (Flags.ActionPanelActivate.Value)
                {
                    if (MousePoint.X + SizeActiveActionPanel.Width > RichTextBoxMainMessage.ActualWidth - 9)
                        MousePoint.X = RichTextBoxMainMessage.ActualWidth - SizeActiveActionPanel.Width - 1;
                    if (MousePoint.Y + SizeActiveActionPanel.Height > RichTextBoxMainMessage.ActualHeight - 47)
                        MousePoint.Y = RichTextBoxMainMessage.ActualHeight - SizeActiveActionPanel.Height - 1;
                }
                ThicknessAnimate.To = new Thickness(MousePoint.X, MousePoint.Y, 0, 0);
            }
            else if (StylePositionToAnimate == PositionAnimActionPanel.CenterObject)
            {
                ThicknessAnimate.To =
                    new Thickness(
                        BorderActionPanel.Margin.Left + BorderActionPanel.Width / 2,
                        BorderActionPanel.Margin.Top + BorderActionPanel.Height / 2,
                        0, 0);
            }
            BorderActionPanel.BeginAnimation(MarginProperty, ThicknessAnimate);
        }

        /// <summary>
        /// Активировать команду
        /// </summary>
        /// <param name="CommandString">Ктрока команды</param>
        private void ActivateActionCommand(string CommandString)
        {
            if (Flags.ActionPanelActivate.Value) AnimationActionPanel(false, PositionAnimActionPanel.CenterObject);
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
                };
                Button.OnActivateRightButtonMouse += () =>
                {
                    App.BufferCommand.Delete(Button.IndexElement);
                    Pages.PageBufferActPanel.TextBlockCounterBuffer.Text = $"{App.BufferCommand.Count}/{App.BufferCommand.Length}";
                    if (App.BufferCommand.Count == 0) Pages.PageBufferActPanel.IELButtonClearBuffer.IsEnabled = false;
                };
                Button.TextBlockNumberCommand.Text = $"#{App.BufferCommand.Count + 1}";
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
                    RealButton.TextBlockButtonCommand.Text = NextButton.TextBlockButtonCommand.Text;
                }
                RealButton = (IELButtonCommand)Pages.PageBufferActPanel.GridBuffer.Children[^1];
                RealButton.Text = Name;
                RealButton.TextBlockButtonCommand.Text = CommandString;
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
            if (!App.InternetPinging.Wait) Flags.FlagInternetConnection.Value = App.InternetPinging.Value;
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