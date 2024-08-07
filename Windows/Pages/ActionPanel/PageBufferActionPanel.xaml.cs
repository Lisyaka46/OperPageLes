using AAC20.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace AAC20.Windows.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageBufferActionPanel.xaml
    /// </summary>
    public partial class PageBufferActionPanel : Page, IPageActionPanelAAC
    {
        private bool PAltMode = false;
        /// <summary>
        /// Alt режим для переключения кнопок с помощью клавиш клавиатуры
        /// </summary>
        public bool AltMode { get => PAltMode; set => AltModeChanged.Invoke(value); }

        /// <summary>
        /// Объект события изменения состояния Alt режима
        /// </summary>
        public IPageActionPanelAAC.Delegate_AltModeChanged AltModeChanged { get; private set; }

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

        public PageBufferActionPanel()
        {
            InitializeComponent();
            TextBlockCounterBuffer.Text = $"{App.BufferCommand.Count}/{App.BufferCommand.Length}";
            AltModeChanged = (Mode) =>
            {
                IELButtonBackMainMenu.CharKeyKeyboardActivate = Mode;
                IELButtonClearBuffer.CharKeyKeyboardActivate = Mode;
                PAltMode = Mode;
            };
            BorderBuffer.MouseWheel += (sender, e) =>
            {
                if (App.BufferCommand.CounterBuffer.MaxValue > 0 && App.BufferCommand.Count > 0)
                {
                    if (e.Delta > 0 && App.BufferCommand.CounterBuffer.Value > 0) App.BufferCommand.CounterBuffer.Up();
                    else if (e.Delta < 0 &&
                    App.BufferCommand.CounterBuffer.Value < App.BufferCommand.CounterBuffer.MaxValue) App.BufferCommand.CounterBuffer.Down();
                    ThicknessAnimationBuffer.To = new(0, 0 - (App.BufferCommand[0].Height + 2) * App.BufferCommand.CounterBuffer.Value, 0, 0);
                    GridBuffer.BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
                }
            };
            IELButtonClearBuffer.OnActivate += (Key) =>
            {
                IELButtonClearBuffer.IsEnabled = false;
                OpacityAnimationBuffer.Completed += EventClearBuffer;
                App.BufferCommand.CounterBuffer.Value = 0;
                App.BufferCommand.CounterBuffer.MaxClear();
                ThicknessAnimationBuffer.To = new(0);
                ThicknessAnimationBuffer.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                ThicknessAnimationBuffer.Duration = TimeSpan.FromMilliseconds(160d);
                GridBuffer.BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
                OpacityAnimationBuffer.To = 0.4d;
                TextBlockCounterBuffer.Text = $"0/{App.BufferCommand.Length}";
                for (int i = 0; i < App.BufferCommand.Count; i++)
                {
                    ThicknessAnimationBuffer.To = new(0, App.BufferCommand[i].Margin.Top - 12, 0, 0);
                    OpacityAnimationBuffer.BeginTime = TimeSpan.FromMilliseconds(120 + i * 30);
                    App.BufferCommand[i].BeginAnimation(OpacityProperty, OpacityAnimationBuffer);
                    App.BufferCommand[i].BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
                }
                OpacityAnimationBuffer.BeginTime = null;
                ThicknessAnimationBuffer.EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut };
                ThicknessAnimationBuffer.Duration = TimeSpan.FromMilliseconds(300d);
            };
        }

        private void EventClearBuffer(object? sender, EventArgs e)
        {
            App.BufferCommand.DeleteAll(GridBuffer);
        }

        /// <summary>
        /// Активировать кнопку по ключу
        /// </summary>
        /// <param name="key">Ключ активации</param>
        public void ActivateInKey(Key key) => App.ActivateButtonInKey(this, key);
    }
}
