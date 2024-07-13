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
        /// Размер активной панели действий
        /// </summary>
        private static Size SizeActiveActionPanel => new(166, 176);

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
                    if (param.Length == 0) return Task.FromResult(CommandStateResult.FaledParameteres("Print"));
                    Paragraph Massage = new();
                    Massage.Inlines.Clear();
                    Massage.Inlines.Add(new Bold(new Run(">>> ")));
                    Massage.Inlines.Add(new Run(param[0]));
                    RichTextBoxMainMessage.Document.Blocks.Add(Massage);
                    return Task.FromResult(CommandStateResult.Completed);
                }),
            ]);

            UpdateBackgroundDataThis = new((sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualData));
            BackgroundUpdateVisualData();
            RichTextBoxMainMessage.Document = new();
            BorderActionPanel.Width = 0;
            BorderActionPanel.Height = 0;

            ButtonReboot.MouseUp += (sender, e) => App.RebootApplication();
            ButtonReturnCommand.MouseUp += (sender, e) =>
            {
                ActivateActionCommand(TextBoxCommandInput.Text);
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
                    else AnimationMoveActionPanel();
                }
            };

            UpdateBackgroundDataThis.TimerDataUpdate.Start();
        }

        /// <summary>
        /// Анимировать изменение состояния панель действий
        /// </summary>
        /// <param name="State">Состояние панели</param>
        private void AnimationActionPanel(bool State)
        {
            Flags.ActionPanelActivate.Value = State;
            AnimationMoveActionPanel();
            DoubleAnimateActionPanelWH.To = State ? 166d : 0d;
            BorderActionPanel.BeginAnimation(HeightProperty, DoubleAnimateActionPanelWH);
            DoubleAnimateActionPanelWH.To = State ? 176d : 0d;
            BorderActionPanel.BeginAnimation(WidthProperty, DoubleAnimateActionPanelWH);
        }

        /// <summary>
        /// Анимировать передвижение панели действий
        /// </summary>
        private void AnimationMoveActionPanel()
        {
            Point MousePoint = Mouse.GetPosition(RichTextBoxMainMessage);
            if (MousePoint.X + SizeActiveActionPanel.Width > RichTextBoxMainMessage.ActualWidth) MousePoint.X = RichTextBoxMainMessage.ActualWidth - SizeActiveActionPanel.Width - 1;
            if (MousePoint.Y + SizeActiveActionPanel.Height > RichTextBoxMainMessage.ActualHeight) MousePoint.Y = RichTextBoxMainMessage.ActualHeight - SizeActiveActionPanel.Height - 1;
            ThicknessAnimateActionPanel.To = new Thickness(MousePoint.X - 9, MousePoint.Y + 9, 0, 0);
            BorderActionPanel.BeginAnimation(MarginProperty, ThicknessAnimateActionPanel);
        }

        /// <summary>
        /// Активировать команду
        /// </summary>
        /// <param name="CommandString">Ктрока команды</param>
        private void ActivateActionCommand(string CommandString)
        {
            if (Flags.ActionPanelActivate.Value) AnimationActionPanel(false);
            if (CommandString.Length == 0) return;
            TextBoxCommandInput.Text = string.Empty;
            CommandStateResult Result = ConsoleCommand.ReadAndExecuteCommand([.. App.DataConsoleCommand], CommandString);
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