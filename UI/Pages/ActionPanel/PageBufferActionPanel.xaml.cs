using OperPage_les.CORE;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using IEL.Interfaces.Core;
using IEL;
using IEL.Classes;
using DataScroll;
using IEL.Interfaces.Front;

namespace OperPage_les.Windows.Pages.ActionPanel
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
        /// Буфер объектов команд
        /// </summary>
        internal Interpreter.Classes.Buffer BufferCommand;

        /// <summary>
        /// Константа размера Height для кнопок буфера
        /// </summary>
        [NotNull()]
        private readonly int H;

        public PageBufferActionPanel(int HeightButtonCommand)
        {
            InitializeComponent();
            string StringSizeBuffer = App.CurrentApp.SettingApplication.GetSettingValue(CORE.Settings.EnumSettingApplication.BufferSize);
            int BufferLength = Convert.ToInt32(StringSizeBuffer);
            BufferCommand = new(BufferLength);
            H = HeightButtonCommand;
            ScrollBar = new(3);
            ScrollBar.ChangedValue += (Value) =>
            {
                ThicknessAnimationBuffer.To = new(0, 0 - (H + 2) * Value, 0, 0);
                GridBuffer.BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
            };
            TextBlockCounterBuffer.Text = $"{(BufferCommand.Count < 10 ? "0" : string.Empty)}{BufferCommand.Count} {BufferCommand.Length}";
            KeyboardModeChanged = (Mode) =>
            {
                IELButtonBackMainMenu.CharKeyboardActivate = Mode;
                IELButtonClearBuffer.CharKeyboardActivate = Mode;
            };
            BorderBuffer.MouseWheel += (sender, e) =>
            {
                if (ScrollBar.MaxValue > 0 && BufferCommand.Count > 0)
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
                TextBlockCounterBuffer.Text = $"00 {BufferCommand.Length}";
                for (int i = 0; i < BufferCommand.Count; i++)
                {
                    IELButtonCommand Button = (IELButtonCommand)GridBuffer.Children[i];
                    if (i == BufferCommand.Count - 1)
                    {
                        OpacityAnimationBuffer.FillBehavior = FillBehavior.Stop;
                        OpacityAnimationBuffer.Completed += (sender, e) => BufferCommand.DeleteAll();
                    }
                    ThicknessAnimationBuffer.To = new(-11, Button.Margin.Top + 11, 0, 0);
                    BeginTimeOffset.Add(TimeSpan.FromMilliseconds(60d));
                    OpacityAnimationBuffer.BeginTime = BeginTimeOffset;
                    ThicknessAnimationBuffer.BeginTime = BeginTimeOffset;
                    Button.BeginAnimation(OpacityProperty, OpacityAnimationBuffer);
                    Button.BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
                }
                OpacityAnimationBuffer.FillBehavior = FillBehavior.HoldEnd;
                OpacityAnimationBuffer.Completed -= (sender, e) => BufferCommand.DeleteAll();
                OpacityAnimationBuffer.BeginTime = TimeSpan.Zero;
                ThicknessAnimationBuffer.BeginTime = TimeSpan.Zero;
                ThicknessAnimationBuffer.Duration = TimeSpan.FromMilliseconds(300d);
            };
            BufferCommand.DelElement += (index) =>
            {
                GridBuffer.Children.RemoveAt(index);
                ScrollBar.MaxDown(1);

                ThicknessAnimation AnimationBuffer = new(new Thickness(0), TimeSpan.FromMilliseconds(160d))
                {
                    EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut, Amplitude = 0.6d }
                };
                Thickness ThicknessIndex = new(0);
                for (int i = index; i < BufferCommand.Count; i++)
                {
                    IELButtonCommand Button = (IELButtonCommand)GridBuffer.Children[i];
                    Button.Index--;
                    AnimationBuffer.To = new Thickness(0, (H + 2) * i, 0, 0);
                    AnimationBuffer.BeginTime = TimeSpan.FromMilliseconds((i - index) * 20d);
                    Button.BeginAnimation(MarginProperty, AnimationBuffer);
                }
            };

            BufferCommand.ClearBuffer += () =>
            {
                GridBuffer.Children.Clear();
                ScrollBar.MaxClear();
            };
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
            IELButtonCommand Button = new(Name, Command, BufferCommand.Count)
            {
                Height = H,
                Margin = new(0, (H + 2) * BufferCommand.Count, 0, 0),
                Index = BufferCommand.Count,
            };
            return Button;
        }

        /// <summary>
        /// Добавить команду в буфер
        /// </summary>
        /// <param name="Name">Имя команды</param>
        /// <param name="Command">Строка команды</param>
        /// <param name="ActionActivateCommand">Событие которое происходит при активации команды в буфере</param>
        internal void InsertCommandFromBuffer(string Name, string Command, IIELButtonDefault.Activate ActionActivateCommand)
        {
            IELButtonClearBuffer.IsEnabled = true;
            if (BufferCommand.Count < BufferCommand.Length)
            {
                IELButtonCommand Button = CreateBufferButton(Name, Command);
                Button.OnActivateMouseLeft += ActionActivateCommand;
                Button.OnActivateMouseRight += () =>
                {
                    BufferCommand.Delete(Button.Index);
                    TextBlockCounterBuffer.Text =
                        $"{(BufferCommand.Count < 10 ? "0" : string.Empty)}{BufferCommand.Count} {BufferCommand.Length}";
                    if (BufferCommand.Count == 0) IELButtonClearBuffer.IsEnabled = false;
                };
                BufferCommand.Add(Command);
                GridBuffer.Children.Add(Button);
                ScrollBar.MaxUp(1);
            }
            else
            {
                BufferCommand.Add(Command);
                IELButtonCommand RealButton;
                for (int i = 0; i < GridBuffer.Children.Count - 1; i++)
                {
                    RealButton = (IELButtonCommand)GridBuffer.Children[i];
                    IELButtonCommand NextButton = (IELButtonCommand)GridBuffer.Children[i + 1];
                    RealButton.Text = NextButton.Text;
                    RealButton.TextCommand = NextButton.TextCommand;
                }
                RealButton = (IELButtonCommand)GridBuffer.Children[^1];
                RealButton.Text = Name;
                RealButton.TextCommand = Command;
            }
            TextBlockCounterBuffer.Text =
                $"{(BufferCommand.Count < 10 ? "0" : string.Empty)}{BufferCommand.Count} {BufferCommand.Length}";
        }
        #endregion
    }
}
