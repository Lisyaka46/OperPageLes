using AAC20.CORE;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using IEL.Interfaces.Core;
using IEL;
using IEL.Classes;
using DataScroll;

namespace AAC20.Windows.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageBufferActionPanel.xaml
    /// </summary>
    public partial class PageBufferActionPanel : Page, IPageKey
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageBufferActionPanel);

        /// <summary>
        /// Объект данных режима клавиатуры
        /// </summary>
        private bool _KeyboardMode = false;

        /// <summary>
        /// Режим клавиатуры
        /// </summary>
        public bool KeyboardMode
        {
            get => _KeyboardMode;
            set
            {
                _KeyboardMode = value;
                KeyboardModeChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Объект события изменения состояния Alt режима
        /// </summary>
        public IPageKey.Delegate_KeyboardModeChanged? KeyboardModeChanged { get; set; }

        /// <summary>
        /// Объект анимации позиции сколла буфера
        /// </summary>
        private readonly ThicknessAnimation ThicknessAnimationBuffer = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации прозрачности элементов буфера
        /// </summary>
        private readonly DoubleAnimation OpacityAnimationBuffer = new(0, TimeSpan.FromMilliseconds(90d))
        {
            EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Скролл-бар страницы визуализации буфера
        /// </summary>
        internal readonly CounterScrollBar ScrollBar;

        /// <summary>
        /// Константа размера Height для кнопок буфера
        /// </summary>
        [NotNull()]
        private readonly int H;

        public PageBufferActionPanel(int HeightButtonCommand)
        {
            InitializeComponent();
            H = HeightButtonCommand;
            ScrollBar = new(3);
            ScrollBar.ChangedValue += (Value) =>
            {
                ThicknessAnimationBuffer.To = new(0, 0 - (H + 2) * Value, 0, 0);
                GridBuffer.BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
            };
            TextBlockCounterBuffer.Text = $"{App.BufferCommand.Count}/{App.BufferCommand.Length}";
            KeyboardModeChanged = (Mode) =>
            {
                IELButtonBackMainMenu.CharKeyboardActivate = Mode;
                IELButtonClearBuffer.CharKeyboardActivate = Mode;
            };
            BorderBuffer.MouseWheel += (sender, e) =>
            {
                if (ScrollBar.MaxValue > 0 && App.BufferCommand.Count > 0)
                {
                    if (e.Delta > 0) ScrollBar.Up();
                    else if (e.Delta < 0) ScrollBar.Down();
                }
            };
            IELButtonClearBuffer.OnActivateMouseLeft += (Key) =>
            {
                TimeSpan BeginTimeOffset = TimeSpan.FromMilliseconds(ScrollBar.Value > 0 ? 50d : 0d);
                IELButtonClearBuffer.IsEnabled = false;
                ScrollBar.MaxClear();
                ThicknessAnimationBuffer.To = new(0);
                ThicknessAnimationBuffer.Duration = TimeSpan.FromMilliseconds(160d);
                GridBuffer.BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
                OpacityAnimationBuffer.To = 0d;
                TextBlockCounterBuffer.Text = $"0/{App.BufferCommand.Length}";
                for (int i = 0; i < App.BufferCommand.Count; i++)
                {
                    IELButtonCommand Button = (IELButtonCommand)GridBuffer.Children[i];
                    if (i == App.BufferCommand.Count - 1)
                    {
                        OpacityAnimationBuffer.FillBehavior = FillBehavior.Stop;
                        OpacityAnimationBuffer.Completed += (sender, e) => App.BufferCommand.DeleteAll();
                    }
                    ThicknessAnimationBuffer.To = new(-11, Button.Margin.Top + 11, 0, 0);
                    BeginTimeOffset.Add(TimeSpan.FromMilliseconds(60d));
                    OpacityAnimationBuffer.BeginTime = BeginTimeOffset;
                    ThicknessAnimationBuffer.BeginTime = BeginTimeOffset;
                    Button.BeginAnimation(OpacityProperty, OpacityAnimationBuffer);
                    Button.BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
                }
                OpacityAnimationBuffer.FillBehavior = FillBehavior.HoldEnd;
                OpacityAnimationBuffer.Completed -= (sender, e) => App.BufferCommand.DeleteAll();
                OpacityAnimationBuffer.BeginTime = TimeSpan.Zero;
                ThicknessAnimationBuffer.BeginTime = TimeSpan.Zero;
                ThicknessAnimationBuffer.Duration = TimeSpan.FromMilliseconds(300d);
            };
        }
    }
}
