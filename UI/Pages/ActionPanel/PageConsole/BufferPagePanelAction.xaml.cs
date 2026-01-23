using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.UI.UserElementsControl;
using IEL.CORE.Classes;
using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ApplicationOperPageLes.UI.Pages.ActionPanel.PageConsole
{
    /// <summary>
    /// Логика взаимодействия для PageBufferActionPanel.xaml
    /// </summary>
    public partial class BufferPagePanelAction : Page
    {
        /// <summary>
        /// Буфер объектов команд
        /// </summary>
        internal Interpreter.Classes.Buffer BufferCommand;

        /// <summary>
        /// Объект анимации позиции при удалении одного элемента
        /// </summary>
        private static readonly ThicknessAnimation AnimationBuffer = new(new Thickness(0), TimeSpan.FromMilliseconds(160d))
        {
            EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut, Amplitude = 0.6d }
        };

        public BufferPagePanelAction()
        {
            InitializeComponent();
            GridBuffer.Opacity = 0d;
            BufferCommand = new(App.CurrentApp.SettingMainApplication.BufferSize);
            TextBlockCounterBuffer.Text = $"{(BufferCommand.Count < 10 ? "0" : string.Empty)}{BufferCommand.Count}/{BufferCommand.Length}";

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELButtonBackMainMenu);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonClearBuffer);

            IELButtonClearBuffer.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                IELButtonClearBuffer.IsEnabled = false;
                TextBlockCounterBuffer.Text = $"00/{BufferCommand.Length}";

                DoubleAnimation OpacityAnimationBuffer = new()
                {
                    From = null,
                    To = 0d,
                    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut },
                    Duration = TimeSpan.FromMilliseconds(300d),
                    FillBehavior = FillBehavior.Stop
                };
                OpacityAnimationBuffer.Completed += (sender, e) => BufferCommand.DeleteAll();
                GridBuffer.BeginAnimation(OpacityProperty, OpacityAnimationBuffer, HandoffBehavior.SnapshotAndReplace);
            };
            BufferCommand.DelElement += (index) =>
            {
                GridBuffer.Children.RemoveAt(index);
                for (int i = index; i < BufferCommand.Count; i++)
                {
                    IELButtonText Button = (IELButtonText)GridBuffer.Children[i];
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
        private IELButtonText CreateBufferButton(string Command)
        {
            IELButtonText Button = new()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                Height = 40,
                Margin = new(0, (40 + 2) * GridBuffer.Children.Count, 0, 0),
                MarginViewBox = new(0, 3, 0, 3),
                Text = Command,
                CornerRadius = new(8),

            };
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Chocolate].ConnectPalleteFromIELElement(Button);
            return Button;
        }

        /// <summary>
        /// Добавить команду в буфер
        /// </summary>
        /// <param name="Name">Имя команды</param>
        /// <param name="Command">Строка команды</param>
        /// <param name="ActionActivateCommand">Событие которое происходит при активации команды в буфере</param>
        internal void InsertCommandFromBuffer(string Command, Browser.PageConsole SourcePage)
        {
            if (!IELButtonClearBuffer.IsEnabled)
            {
                IELButtonClearBuffer.IsEnabled = true;
                App.DoubleAnimationType.AnimateEffect(GridBuffer, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
            }
            if (BufferCommand.Count < BufferCommand.Length)
            {
                IELButtonText Button = CreateBufferButton(Command);
                Button.OnActivateMouseLeft += async (sender, e) =>
                {
                    OPLCommandViewer Viewer = new();
                    SourcePage.GridConsole.RowDefinitions.Add(new() { Height = new(0d, GridUnitType.Auto) });
                    Grid.SetRow(Viewer, SourcePage.GridConsole.RowDefinitions.Count);
                    SourcePage.GridConsole.Children.Add(Viewer);
                    await App.CurrentApp.ActivateActionCommand(Viewer, Command);
                };
                Button.OnActivateMouseRight += (sender, e) =>
                {
                    BufferCommand.Delete(Button.Text);
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
                IELButtonText RealButton;
                for (int i = 0; i < GridBuffer.Children.Count - 1; i++)
                {
                    RealButton = (IELButtonText)GridBuffer.Children[i];
                    IELButtonText NextButton = (IELButtonText)GridBuffer.Children[i + 1];
                    RealButton.Text = NextButton.Text;
                }
                RealButton = (IELButtonText)GridBuffer.Children[^1];
                RealButton.Text = Name;
            }
            TextBlockCounterBuffer.Text =
                $"{(BufferCommand.Count < 10 ? "0" : string.Empty)}{BufferCommand.Count}/{BufferCommand.Length}";
        }
        #endregion
    }
}
