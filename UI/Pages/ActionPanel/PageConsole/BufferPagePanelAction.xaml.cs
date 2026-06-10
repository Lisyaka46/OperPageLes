using IEL.CORE.Classes;
using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OPLAPI.OIEL.UserElementsControl;
using OperPageLes.CORE.Enums;
using OperPageLes.UI.Pages.Browser;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OperPageLes.UI.Pages.ActionPanel.PageConsole
{
    /// <summary>
    /// Логика взаимодействия для PageBufferActionPanel.xaml
    /// </summary>
    public partial class BufferPagePanelAction : Page, IOPLAnimate
    {
        /// <summary>
        /// Буфер объектов команд
        /// </summary>
        internal Interpreter.Classes.Buffer? BufferCommand;

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

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
            TextBlockCounterBuffer.Text = "NULL";

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELButtonBackMainMenu);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonClearBuffer);

            IELButtonClearBuffer.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                IELButtonClearBuffer.IsEnabled = false;
                TextBlockCounterBuffer.Text = BufferCommand == null ? "NULL" : $"00/{BufferCommand.Length}";

                DoubleAnimation OpacityAnimationBuffer = new()
                {
                    From = null,
                    To = 0d,
                    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut },
                    Duration = TimeSpan.FromMilliseconds(300d),
                    FillBehavior = FillBehavior.Stop
                };
                OpacityAnimationBuffer.Completed += (sender, e) => BufferCommand?.DeleteAll();
                GridBuffer.BeginAnimation(OpacityProperty, OpacityAnimationBuffer, HandoffBehavior.SnapshotAndReplace);
            };
            IELButtonClearBuffer.IsEnabled = false;
        }

        /// <summary>
        /// Подключить буфер команд к странице буфера
        /// </summary>
        /// <param name="SourceBuffer">Передаваемый буфер команд на подключение</param>
        internal void ConnectBuffer(Interpreter.Classes.Buffer SourceBuffer)
        {
            BufferCommand = SourceBuffer;
            TextBlockCounterBuffer.Text = $"{(BufferCommand.Count < 10 ? "0" : string.Empty)}{BufferCommand.Count}/{BufferCommand.Length}";
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
        internal void InsertCommandFromBuffer(string Command, Browser.InlayPages.PageConsole SourcePage)
        {
            if (BufferCommand == null) throw new Exception("Невозможно добавить команду в буфер которого нет!");
            if (!IELButtonClearBuffer.IsEnabled)
            {
                IELButtonClearBuffer.IsEnabled = true;
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, GridBuffer, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
            }
            if (BufferCommand.Count < BufferCommand.Length)
            {
                IELButtonText Button = CreateBufferButton(Command);
                Button.OnActivateMouseLeft += async (sender, e) =>
                {
                    if (App.CurrentApp.SettingMainApplication.MovePageExecuteBufferCommand)
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            if (App.CurrentApp.MainBrowser.ActualInlay?.Content?.GetType() != SourcePage.GetType())
                            {
                                OPLInlay? InlaySource = App.CurrentApp.MainBrowser.Inlays.FirstOrDefault(
                                    (i) => i.Content?.Equals(SourcePage) ?? false);
                                if (InlaySource != null)
                                {
                                    App.CurrentApp.MainBrowser.ActivateInlayIndex(App.CurrentApp.MainBrowser.Inlays.IndexOf(InlaySource));
                                }
                                else if (App.CurrentApp.MainBrowser.ActualInlay?.Content is Browser.InlayPages.PageConsole page)
                                {
                                    SourcePage = page;
                                }
                            }
                            Task.Delay(300);
                        });
                    }
                    OPLCommandViewer Viewer = SourcePage.CreateNewCommandViewer(Command);
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
