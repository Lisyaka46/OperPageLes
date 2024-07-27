using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Timer = System.Timers.Timer;
using System.Windows.Input;
using System.Windows.Media.Animation;
using AAC20.Classes.Commands;
using AAC20.Classes;
using AAC20.Windows.Frames;
using AAC20.Windows.Pages.ActionPanel;
using System.Windows.Navigation;
using System.Windows.Controls;
using AAC20.Interfaces;

namespace AAC20
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private sealed class UpdateBackgroundData
        {
            /// <summary>
            /// Объект управляющий фоновым обновлением визуальной информации
            /// </summary>
            public readonly Timer TimerDataUpdate;

            /// <summary>
            /// Инициализировать объект управления фоновым обновлением информации в данном окне
            /// </summary>
            public UpdateBackgroundData(ElapsedEventHandler Elapsed)
            {
                TimerDataUpdate = new(1000);
                TimerDataUpdate.Elapsed += Elapsed;
            }
        }

        /// <summary>
        /// Флаги данной формы
        /// </summary>
        private static class Flags
        {
            /// <summary>
            /// Флаг состояния активности панели действий в главной консоли
            /// </summary>
            public static Flag ActionPanelActivate = new(false);
        };

        /// <summary>
        /// Класс страниц данной формы
        /// </summary>
        private static class Pages
        {
            /// <summary>
            /// Главная страница панели действий
            /// </summary>
            internal static PageMainActionPanel PageMainActPanel = new();

            /// <summary>
            /// Страница буффера в панели действий
            /// </summary>
            internal static PageBufferActionPanel PageBufferActPanel = new();
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
        /// Объект анимации для управления позицией панели действий
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimateActionPanel = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления прозрачностью панели действий
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimateActionPanelOpacity = new(0, TimeSpan.FromMilliseconds(250d))
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
        /// ССылка на активную страницу панели действий
        /// </summary>
        private IPageActionPanelAAC RefPageActionPanel;

        public MainWindow()
        {
            InitializeComponent();

            App.DataConsoleCommand.AddRange([
                new ConsoleCommand("clear", "Очистка выводимых данных", (param) =>
                {
                    RichTextBoxMainMessage.Document = new();
                    return Task.FromResult(CommandStateResult.Completed);
                }),
                new ConsoleCommand("print", [new Parameter("Text")], "Вывод текста на экран", (param) =>
                {
                    Paragraph Massage = new();
                    Massage.Inlines.Clear();
                    Massage.Inlines.Add(new Bold(new Run(">>> ")));
                    Massage.Inlines.Add(new Run(param[0]));
                    RichTextBoxMainMessage.Document.Blocks.Add(Massage);
                    return Task.FromResult(CommandStateResult.Completed);
                }),
            ]);

            Pages.PageMainActPanel.IELButtonCrearConsole.OnActivate += (AltMode) =>
            {
                RichTextBoxMainMessage.Document = new();
                AnimationActionPanel(false);
            };

            Pages.PageMainActPanel.IELButtonCommandBuffer.OnActivate += (AltMode) =>
            {
                NextPageInActtionPanel(Pages.PageBufferActPanel);
            };

            Pages.PageBufferActPanel.IELButtonBackMainMenu.OnActivate += (AltMode) =>
            {
                NextPageInActtionPanel(Pages.PageMainActPanel, false);
            };

            UpdateBackgroundDataThis = new((sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualData));
            BackgroundUpdateVisualData();
            FrameActionPanelLeft.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameActionPanelRight.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            RefPageActionPanel = Pages.PageMainActPanel;
            FrameActionPanelLeft.Navigate(Pages.PageMainActPanel);
            RichTextBoxMainMessage.Document = new();
            SizeActiveActionPanel = new(BorderActionPanel.Width, BorderActionPanel.Height);
            BorderActionPanel.Width = 0;
            BorderActionPanel.Height = 0;

            ButtonReboot.OnActivate += (key) => App.RebootApplication();
            ButtonReturnCommand.OnActivate += (key) => ActivateActionCommand(TextBoxCommandInput.Text);
            SizeChanged += (sender, e) => AnimationActionPanel(false, PositionAnimActionPanel.CenterObject);

            BorderActionPanel.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        AnimationActionPanel(false, PositionAnimActionPanel.CenterObject);
                        break;
                    case Key.Z:
                        RefPageActionPanel.AltModeChanged.Invoke(!RefPageActionPanel.AltMode);
                        break;
                    case Key.W:
                        Pages.PageMainActPanel.IELButtonCrearConsole.BlinkAnimation();
                        break;
                    default:
                        if (RefPageActionPanel.AltMode) RefPageActionPanel.ActivateInKey(e.Key);
                        break;
                }
            };

            TextBoxCommandInput.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        TextBoxCommandInput.TextBackground.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(160, 245, 200), TimeSpan.FromMilliseconds(90d)));
                        break;
                }
            };
            TextBoxCommandInput.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        TextBoxCommandInput.TextBackground.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(120, 204, 160), TimeSpan.FromMilliseconds(430d)));
                        ActivateActionCommand(TextBoxCommandInput.Text);
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

            UpdateBackgroundDataThis.TimerDataUpdate.Start();
        }

        /// <summary>
        /// Перенаправить страницу панели
        /// </summary>
        /// <param name="Content">Новая страница панели</param>
        /// <param name="RightAlign">Правая ориентация движения</param>
        private void NextPageInActtionPanel(IPageActionPanelAAC Content, bool RightAlign = true)
        {
            Frame OldFrameAnim = PanelVerschachtelung % 2 == 0 ? FrameActionPanelLeft : FrameActionPanelRight;
            Frame NewFrameAnim = !(PanelVerschachtelung % 2 == 0) ? FrameActionPanelLeft : FrameActionPanelRight;
            int Offset = RightAlign ? -18 : 18;
            NewFrameAnim.Opacity = 0;
            Canvas.SetZIndex(OldFrameAnim, 0);
            Canvas.SetZIndex(NewFrameAnim, 1);
            OldFrameAnim.IsEnabled = false;
            NewFrameAnim.IsEnabled = true;
            NewFrameAnim.BeginAnimation(MarginProperty, null);
            NewFrameAnim.Margin = new(0 - Offset);
            Content.AltMode = RefPageActionPanel.AltMode;
            RefPageActionPanel = Content;
            NewFrameAnim.Navigate(Content);

            DoubleAnimateActionPanelOpacity.To = 0;
            OldFrameAnim.BeginAnimation(OpacityProperty, DoubleAnimateActionPanelOpacity);
            ThicknessAnimateActionPanel.To = new(Offset);
            OldFrameAnim.BeginAnimation(MarginProperty, ThicknessAnimateActionPanel);

            DoubleAnimateActionPanelOpacity.To = 1;
            NewFrameAnim.BeginAnimation(OpacityProperty, DoubleAnimateActionPanelOpacity);
            ThicknessAnimateActionPanel.To = new(0);
            NewFrameAnim.BeginAnimation(MarginProperty, ThicknessAnimateActionPanel);

            PanelVerschachtelung = (PanelVerschachtelung + 1) % 2;
        }

        /// <summary>
        /// Анимировать изменение состояния панель действий
        /// </summary>
        /// <param name="State">Состояние панели</param>
        /// <param name="StylePositionAnimate">Стиль анимации позиции</param>
        private void AnimationActionPanel(bool State, PositionAnimActionPanel StylePositionAnimate = PositionAnimActionPanel.Default)
        {
            if (State == Flags.ActionPanelActivate.Value) return;
            if (State) BorderActionPanel.Focus();
            else
            {
                RefPageActionPanel.AltModeChanged.Invoke(false);
                RichTextBoxMainMessage.Focus();
            }
            Flags.ActionPanelActivate.Value = State;
            AnimationMoveActionPanel(StylePositionAnimate);
            DoubleAnimateActionPanelWH.To = State ? SizeActiveActionPanel.Width : 0d;
            BorderActionPanel.BeginAnimation(WidthProperty, DoubleAnimateActionPanelWH);
            DoubleAnimateActionPanelWH.To = State ? SizeActiveActionPanel.Height : 0d;
            BorderActionPanel.BeginAnimation(HeightProperty, DoubleAnimateActionPanelWH);
            DoubleAnimateActionPanelOpacity.To = State ? 1d : 0d;
            BorderActionPanel.BeginAnimation(OpacityProperty, DoubleAnimateActionPanelOpacity);
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
                ThicknessAnimateActionPanel.To = new Thickness(MousePoint.X, MousePoint.Y, 0, 0);
            }
            else if (StylePositionToAnimate == PositionAnimActionPanel.CenterObject)
            {
                ThicknessAnimateActionPanel.To =
                    new Thickness(
                        BorderActionPanel.Margin.Left + BorderActionPanel.Width / 2,
                        BorderActionPanel.Margin.Top + BorderActionPanel.Height / 2,
                        0, 0);
            }
            BorderActionPanel.BeginAnimation(MarginProperty, ThicknessAnimateActionPanel);
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
            //Pages.PageBufferActPanel.GridBuffer.Background = new SolidColorBrush(Colors.Black);
            CommandStateResult Result =
                ConsoleCommand.ReadAndExecuteCommand
                (App.BufferCommand, [.. App.DataConsoleCommand], CommandString);
            if (Result.State == ResultState.Complete)
            {
                Pages.PageBufferActPanel.GridBuffer.Children.Add(App.BufferCommand[App.BufferCommand.Count]);
            }

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