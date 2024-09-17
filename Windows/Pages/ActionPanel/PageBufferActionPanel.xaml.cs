using AAC20.Classes;
using AAC20.GUI;
using AAC20.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace AAC20.Windows.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageBufferActionPanel.xaml
    /// </summary>
    public partial class PageBufferActionPanel : Page, IPageActionPanelAAC
    {
        /// <summary>
        /// Объект данных Alt-режима
        /// </summary>
        private bool _AltMode;

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
            ScrollBar = new(0, 4);
            ScrollBar.ChangedValue += (Value) =>
            {
                ThicknessAnimationBuffer.To = new(0, 0 - (H + 2) * Value, 0, 0);
                GridBuffer.BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
            };
            TextBlockCounterBuffer.Text = $"{App.BufferCommand.Count}/{App.BufferCommand.Length}";
            AltModeChanged = (Mode) =>
            {
                IELButtonBackMainMenu.CharKeyKeyboardActivate = Mode;
                IELButtonClearBuffer.CharKeyKeyboardActivate = Mode;
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

        /// <summary>
        /// Узнать состояние Alt-режима
        /// </summary>
        /// <returns>Состояние</returns>
        public bool GetAltMode() => _AltMode;

        /// <summary>
        /// Изменить состояние Alt-режима
        /// </summary>
        /// <param name="value">Значение</param>
        public void SetAltMode(bool value) => _AltMode = value;

        /// <summary>
        /// Активировать кнопку в данном элементе типа "IELButtonText" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        /// <param name="Orientation">Ориентация нажатия на кнопку</param>
        public void ActivateIELButtonTextInKey(Key key, IPageActionPanelAAC.OrientationActivate Orientation) =>
            IIELObjectKey.ActivateButtonInKey(MainGrid, key, Orientation);

        /// <summary>
        /// Активировать мерцание кнопки в данном элементе типа "IELButtonText" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        public void BlinkActivateIELButtonTextInKey(Key key) =>
            IIELObjectKey.BlinkActivateInKey(MainGrid, key);
    }
}
