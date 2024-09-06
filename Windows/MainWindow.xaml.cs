using AAC20.Classes;
using AAC20.Classes.Commands;
using AAC20.Interfaces;
using AAC20.Windows;
using AAC20.Windows.Frames;
using AAC20.Windows.Pages.ActionPanel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            public static readonly Flag ActionPanelActivate = new(false);

            /// <summary>
            /// Флаг состояния активации кнопки панели действий через клавишу клавиатуры
            /// </summary>
            public static readonly Flag ActionPanelActivateButtonAltMode = new(false);
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
            /// Страница буффера в панели действий
            /// </summary>
            internal static readonly PageBufferActionPanel PageBufferActPanel = new();
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
        /// Объект управления фоновым обновлением информации в данном окне
        /// </summary>
        private readonly UpdateBackgroundData UpdateBackgroundDataThis;

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
        private static readonly DoubleAnimation DoubleAnimateOpacity = new(0, TimeSpan.FromMilliseconds(250d))
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
        private IPageActionPanelAAC RefPageActionPanel;

        public MainWindow()
        {
            InitializeComponent();

            App.DataConsoleCommand.AddRange([
                new ConsoleCommand("clear", "Очищает текстовый вывод главного меню программы", (param) =>
                {
                    RichTextBoxMainMessage.Document = new();
                    return Task.FromResult(CommandStateResult.Completed);
                }),
                new ConsoleCommand("print", [new Parameter("Text")], "Выводит все введёные параметры начиная с параметра \"Text\" в консоль главного меню программы", (param) =>
                {
                    Paragraph Massage = new();
                    Massage.Inlines.Clear();
                    Massage.Inlines.Add(new Bold(new Run(">>> ")));
                    Massage.Inlines.Add(new Run(string.Join('\0', param)));
                    RichTextBoxMainMessage.Document.Blocks.Add(Massage);
                    return Task.FromResult(CommandStateResult.Completed);
                }),
            ]);

            App.AppFlags.FlagCtrlActivateActionButtonAltMode.ChangeStateFlag += (NewValue) =>
            {
                DoubleAnimateOpacity.To = NewValue ? 1d : 0d;
                TextBlockRightButtonIndicatorKey.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
            };

            Pages.PageMainActPanel.IELButtonCrearConsole.OnActivateMouseLeft += (AltMode) =>
            {
                RichTextBoxMainMessage.Document = new();
                AnimationActionPanel(false);
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
            BackgroundUpdateVisualData();
            FrameActionPanelLeft.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameActionPanelRight.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            RefPageActionPanel = Pages.PageMainActPanel;
            FrameActionPanelLeft.Navigate(Pages.PageMainActPanel);
            TextBlockRightButtonIndicatorKey.Opacity = 0d;
            RichTextBoxMainMessage.Document = new();
            SizeActiveActionPanel = new(BorderActionPanel.Width, BorderActionPanel.Height);
            BorderActionPanel.Width = 0;
            BorderActionPanel.Height = 0;

            ButtonReboot.OnActivateMouseLeft += (key) => App.RebootApplication();
            ButtonReturnCommand.OnActivateMouseLeft += (key) => ActivateActionCommand(TextBoxCommandInput.Text);
            SizeChanged += (sender, e) => AnimationActionPanel(false, PositionAnimActionPanel.CenterObject);

            //Closing += (sender, e) => App.Current.Shutdown(0);

            BorderActionPanel.KeyDown += (sender, e) =>
            {
                if (RefPageActionPanel.AltMode && e.Key != Key.Z && !Flags.ActionPanelActivateButtonAltMode.Value)
                {
                    if (e.Key == Key.RightCtrl)
                    {
                        App.AppFlags.FlagCtrlActivateActionButtonAltMode.Value = true;
                        return;
                    }
                    Flags.ActionPanelActivateButtonAltMode.Value = true;
                    RefPageActionPanel.BlinkActivateIELButtonTextInKey(e.Key);
                }
            };

            BorderActionPanel.KeyUp += (sender, e) =>
            {
                Flags.ActionPanelActivateButtonAltMode.Value = false;
                switch (e.Key)
                {
                    case Key.Escape:
                        AnimationActionPanel(false, PositionAnimActionPanel.CenterObject);
                        break;
                    case Key.Z:
                        RefPageActionPanel.AltMode = !RefPageActionPanel.AltMode;
                        break;
                    default:
                        RefPageActionPanel.ActivateIELButtonTextInKey(e.Key,
                        App.AppFlags.FlagCtrlActivateActionButtonAltMode ?
                        IPageActionPanelAAC.OrientationActivate.RightButton : IPageActionPanelAAC.OrientationActivate.LeftButton);
                        break;
                }
                App.AppFlags.FlagCtrlActivateActionButtonAltMode.Value = false;
            };

            TextBoxCommandInput.GotFocus += (sender, e) =>
            {
                if (Flags.ActionPanelActivate.Value) AnimationActionPanel(false, PositionAnimActionPanel.CenterObject);
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
                    default:
                        return;
                }
                TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(120, 204, 160), TimeSpan.FromMilliseconds(430d)));
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
                ThicknessAnimate.To = new(0);
                DoubleAnimateOpacity.To = 0.6d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
                ImageLogoApplication.BeginAnimation(MarginProperty, ThicknessAnimate);
            };

            ImageLogoApplication.MouseLeave += (sender, e) =>
            {
                ThicknessAnimate.To = new(2);
                DoubleAnimateOpacity.To = 1d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
                ImageLogoApplication.BeginAnimation(MarginProperty, ThicknessAnimate);
            };

            ImageLogoApplication.MouseDown += (sender, e) =>
            {
                ThicknessAnimate.Duration = TimeSpan.FromMilliseconds(100d);
                ThicknessAnimate.To = new(4);
                DoubleAnimateOpacity.To = 0.4d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
                ImageLogoApplication.BeginAnimation(MarginProperty, ThicknessAnimate);
                ThicknessAnimate.Duration = TimeSpan.FromMilliseconds(300d);
            };

            ImageLogoApplication.MouseUp += (sender, e) =>
            {
                ThicknessAnimate.To = new(2);
                DoubleAnimateOpacity.To = 1d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
                ImageLogoApplication.BeginAnimation(MarginProperty, ThicknessAnimate);
                LicenseWindow License = new();
                License.ShowDialog();
            };

            Activated += (sender, e) =>
            {
                TextBoxCommandInput.Focus();
            };

            UpdateBackgroundDataThis.TimerDataUpdate.Start();
        }

        /// <summary>
        /// Перенаправить страницу панели
        /// </summary>
        /// <param name="Content">Новая страница панели</param>
        /// <param name="RightAlign">Правая ориентация движения</param>
        private void NextPageInActtionPanel(
            [DoesNotReturnIf(false), NotNull()] IPageActionPanelAAC Content,
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
            NewFrameAnim.Margin = !RightAlign ? new(-20, -20, 40, -10) : new(40, -10, -20, -10);
            Content.AltMode = AltMode;
            RefPageActionPanel = Content;
            NewFrameAnim.Navigate(Content);

            DoubleAnimateOpacity.To = 0;
            OldFrameAnim.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
            ThicknessAnimate.To = !RightAlign ? new(40, -20, -20, -20) : new(-20, -20, 40, -20);
            OldFrameAnim.BeginAnimation(MarginProperty, ThicknessAnimate);

            DoubleAnimateOpacity.To = 1;
            NewFrameAnim.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
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
            DoubleAnimateOpacity.To = State ? 1d : 0d;
            BorderActionPanel.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
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
            SummarizeCommandStateResult(
                ConsoleCommand.ReadAndExecuteCommand(App.BufferCommand, Pages.PageBufferActPanel,
                [.. App.DataConsoleCommand], CommandString)
                );
        }

        [MTAThread()]
        internal void SummarizeCommandStateResult(CommandStateResult Result)
        {
            if (Result.State == ResultState.Failed && Result.Massage != null)
            {
                RichTextBoxMainMessage.Document.Blocks.Add(Result.Massage);
            }
        }

        /// <summary>
        /// Функция обновления визуальной информации в данном окне
        /// </summary>
        private void BackgroundUpdateVisualData()
        {
            TextBlockTime.Text = RealTime;
            TextBlockData.Text = RealData;
        }
    }
}