using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.UI.UserElementControl;
using IEL.CORE.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel.PageConsole
{
    /// <summary>
    /// Логика взаимодействия для PageBufferActionPanel.xaml
    /// </summary>
    public partial class PageBufferPanelAction : Page
    {
        /// <summary>
        /// Стиль отображения элементов в буфере
        /// </summary>
        private readonly QData QDataBackground = new(
                        [
                        [255, 243, 164, 207],
                        [255, 173, 97, 138],
                        [255, 243, 136, 194],
                        [255, 190, 166, 181],
                        ]);

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
        /// Буфер объектов команд
        /// </summary>
        internal Interpreter.Classes.Buffer BufferCommand;

        public PageBufferPanelAction()
        {
            InitializeComponent();
            BufferCommand = new(App.CurrentApp.SettingMainApplication.BufferSize);
            TextBlockCounterBuffer.Text = $"{(BufferCommand.Count < 10 ? "0" : string.Empty)}{BufferCommand.Count}/{BufferCommand.Length}";

            IELButtonBackMainMenu.Background = App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BG_PastelBlue);
            IELButtonBackMainMenu.BorderBrush = App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.BB_PastelBlue);
            IELButtonBackMainMenu.Foreground = App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(PaletteValuesEnum.FG_PastelBlue);

            IELButtonClearBuffer.OnActivateMouseLeft += (sender, e, Key) =>
            {
                TimeSpan BeginTimeOffset = TimeSpan.FromMilliseconds(50d);
                IELButtonClearBuffer.IsEnabled = false;
                ThicknessAnimationBuffer.To = new(0);
                ThicknessAnimationBuffer.Duration = TimeSpan.FromMilliseconds(160d);
                GridBuffer.BeginAnimation(MarginProperty, ThicknessAnimationBuffer);
                OpacityAnimationBuffer.To = 0d;
                TextBlockCounterBuffer.Text = $"00/{BufferCommand.Length}";
                for (int i = 0; i < BufferCommand.Count; i++)
                {
                    OPLButtonBufferCommand Button = (OPLButtonBufferCommand)GridBuffer.Children[i];
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

                ThicknessAnimation AnimationBuffer = new(new Thickness(0), TimeSpan.FromMilliseconds(160d))
                {
                    EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut, Amplitude = 0.6d }
                };
                Thickness ThicknessIndex = new(0);
                for (int i = index; i < BufferCommand.Count; i++)
                {
                    OPLButtonBufferCommand Button = (OPLButtonBufferCommand)GridBuffer.Children[i];
                    Button.Index--;
                    AnimationBuffer.To = new Thickness(0, (Button.Height + 2) * i, 0, 0);
                    AnimationBuffer.BeginTime = TimeSpan.FromMilliseconds((i - index) * 20d);
                    Button.BeginAnimation(MarginProperty, AnimationBuffer);
                }
            };

            BufferCommand.ClearBuffer += () =>
            {
                GridBuffer.Children.Clear();
            };
        }

        #region ManipulateBuffer
        /// <summary>
        /// Создать кнопку активации команды
        /// </summary>
        /// <param name="Name">Отображаемое имя</param>
        /// <param name="Command">Выполняющаяся команда</param>
        /// <returns>Кнопка выполняющая команду</returns>
        private OPLButtonBufferCommand CreateBufferButton(string Name, string Command)
        {
            OPLButtonBufferCommand Button = new(Name, Command, BufferCommand.Count)
            {
                Height = 40,
                Margin = new(0, (40 + 2) * BufferCommand.Count, 0, 0),
                Index = BufferCommand.Count,
                IELSettingObject = new()
                {
                    AnimationMillisecond = 200d,
                }
            };
            Button.Background = QDataBackground;
            return Button;
        }

        /// <summary>
        /// Добавить команду в буфер
        /// </summary>
        /// <param name="Name">Имя команды</param>
        /// <param name="Command">Строка команды</param>
        /// <param name="ActionActivateCommand">Событие которое происходит при активации команды в буфере</param>
        internal void InsertCommandFromBuffer(string Name, string Command, IEL.CORE.BaseUserControls.IELButtonBase.ActivateHandler ActionActivateCommand)
        {
            IELButtonClearBuffer.IsEnabled = true;
            if (BufferCommand.Count < BufferCommand.Length)
            {
                OPLButtonBufferCommand Button = CreateBufferButton(Name, Command);
                Button.OnActivateMouseLeft += ActionActivateCommand;
                Button.OnActivateMouseRight += (sender, e, Key) =>
                {
                    BufferCommand.Delete(Button.Index);
                    TextBlockCounterBuffer.Text =
                        $"{(BufferCommand.Count < 10 ? "0" : string.Empty)}{BufferCommand.Count}/{BufferCommand.Length}";
                    if (BufferCommand.Count == 0) IELButtonClearBuffer.IsEnabled = false;
                };
                BufferCommand.Add(Command);
                GridBuffer.Children.Add(Button);
            }
            else
            {
                BufferCommand.Add(Command);
                OPLButtonBufferCommand RealButton;
                for (int i = 0; i < GridBuffer.Children.Count - 1; i++)
                {
                    RealButton = (OPLButtonBufferCommand)GridBuffer.Children[i];
                    OPLButtonBufferCommand NextButton = (OPLButtonBufferCommand)GridBuffer.Children[i + 1];
                    RealButton.Text = NextButton.Text;
                    RealButton.TextCommand = NextButton.TextCommand;
                }
                RealButton = (OPLButtonBufferCommand)GridBuffer.Children[^1];
                RealButton.Text = Name;
                RealButton.TextCommand = Command;
            }
            TextBlockCounterBuffer.Text =
                $"{(BufferCommand.Count < 10 ? "0" : string.Empty)}{BufferCommand.Count}/{BufferCommand.Length}";
        }
        #endregion
    }
}
