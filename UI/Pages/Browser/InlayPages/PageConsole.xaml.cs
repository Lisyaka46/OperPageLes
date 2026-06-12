using IEL.CORE.Enums;
using InterpreterCommand.Classes;
using OPLAPI.OIEL.UserElementsControl;
using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel.PageConsole;
using OperPageLes.UI.UserElementsControl.Default;
using OPLAPI.CORE.Animation;
using OPLAPI.OIEL.CORE.Browser;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Pages.Browser.InlayPages
{
    /// <summary>
    /// Логика взаимодействия для PageConsole.xaml
    /// </summary>
    public partial class PageConsole : PageBrowser
    {
        [LibraryImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetCursorPos(int X, int Y);

        #region PanelActionConsole
        internal static readonly MainPagePanelAction PageConsoleActionPanelMain = new();
        /// <summary>
        /// Страница буфера в панели действий
        /// </summary>
        internal BufferPagePanelAction? BufferPage;
        #endregion

        /// <summary>
        /// Подключение подсказок к командам
        /// </summary>
        private static bool HitUse => App.CurrentApp.SettingMainApplication.HitUse;

        /// <summary>
        /// Активный индекс команды в буфере для строки ввода
        /// </summary>
        private int ActiveIndexBufferInput;

        /// <summary>
        /// Сохранённая строка для обозначения введённого текста перед перечислением элементов (Вверх/Вниз)
        /// </summary>
        private string SaveStringPrintBuffer;

        /// <summary>
        /// Производилось ли вычисление события KeyDown
        /// </summary>
        private bool SaveKeyDown;

        /// <summary>
        /// Текущая навигация в текущей странице
        /// </summary>
        private SelectNavigationPageConsoleEnum SelectNavigation;

        /// <summary>
        /// Объект управляемых визуализаторов команд
        /// </summary>
        internal StackPanel StackPanelConsole { get; private set; }

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public override OPLAnimationManager? ManagerAnimation
        {
            get => base.ManagerAnimation;
            set
            {
                base.ManagerAnimation = value;
                HitCommandsInterpreter.ManagerAnimation = value;
            }
        }

        public PageConsole()
        {
            InitializeComponent();
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Command));
            SelectNavigation = SelectNavigationPageConsoleEnum.None;
            SaveKeyDown = false;
            ActiveIndexBufferInput = -1;
            SaveStringPrintBuffer = string.Empty;
            BufferPage = App.CurrentApp.AppPageBuffer;

            #region BorderConsole
            BorderConsole.MouseUp += (sender, e) =>
            {
                if (BorderCommandInformation.Width != BorderButtonPanelInformationController.Width) return;
                else if (e.ChangedButton == MouseButton.Left && App.MainWindow.IELActionPanelMain.PanelActionActivate)
                    App.MainWindow.IELActionPanelMain.ClosePanelAction();
                else if (e.ChangedButton == MouseButton.Right)
                {
                    UsingPanelActionFromConsolePage(null);
                }
            };

            #region IELScrollConsole
            StackPanelConsole = new()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
            };
            IELScrollConsole.ScrollForce = App.CurrentApp.SettingMainApplication.ConsoleScrollForce;
            IELScrollConsole.VerticalScrollAligment = VerticalScrollAlignment.Right;
            IELScrollConsole.AutoUpdateVisibleHorizontalScroll = false;
            IELScrollConsole.AutoUpdateVisibleVerticalScroll = true;
            IELScrollConsole.Content = StackPanelConsole;
            #endregion
            #endregion

            #region ButtonReturnCommand
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(ButtonReturnCommand);
            ButtonReturnCommand.OnActivateMouseLeft += async (sender, e) =>
            {
                if (TextBoxCommandInput.Text.Length == 0) return;
                else if (HitUse) HitCommandsInterpreter.ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);
                string Command = TextBoxCommandInput.Text;
                TextBoxCommandInput.Text = string.Empty;
                await ActivateCommand(Command);
            };
            #endregion

            #region HitCommandsInterpreter
            //HitCommandsInterpreter.ManagerAnimation = App.CurrentApp.ManagerAnimation;
            HitCommandsInterpreter.Connect(App.CurrentApp.Interpreter, in TextBoxCommandInput.TextBoxMain);
            App.CurrentApp.SettingMainApplication.HitUse.Changed += (Old, New) =>
            {
                if (!New && HitCommandsInterpreter.StateVisibleHit != OPLHitInterpreter.HitStateEnum.Hidden)
                {
                    HitCommandsInterpreter.ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);
                }
                else if (New && TextBoxCommandInput.Text.Length > 0)
                {
                    if (TextBoxCommandInput.Text.Contains('*')) HitCommandsInterpreter.UsingOneHitCommand(TextBoxCommandInput.Text);
                    else HitCommandsInterpreter.UsingAllHintCommand(TextBoxCommandInput.Text);
                }
            };
            #endregion

            #region PanelAction
            App.MainWindow.IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                if (Name == nameof(BorderConsole)) TextBoxCommandInput.Focus();
            };
            #endregion

            #region BorderButtonPanelInformationController
            BorderCommandInformation.Margin = new(0);
            BorderCommandInformation.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            BorderCommandInformation.Width = BorderButtonPanelInformationController.Width;
            BorderButtonPanelInformationController.MouseLeftButtonUp += (sender, e) =>
            {
                if (ManagerAnimation != null)
                {
                    DoubleAnimation animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                    animation.EasingFunction = new BackEase()
                    {
                        Amplitude = 0.15d,
                        EasingMode = EasingMode.EaseOut,
                    };
                    animation.Duration = TimeSpan.FromMilliseconds(400d);
                    if (BorderCommandInformation.HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
                    {
                        animation.FillBehavior = FillBehavior.Stop;
                        animation.To = BorderConsole.ActualWidth;
                        animation.Completed += (sender, e) =>
                        {
                            BorderCommandInformation.Width = double.NaN;
                            BorderCommandInformation.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        };
                    }
                    else
                    {
                        BorderCommandInformation.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        animation.From = BorderConsole.ActualWidth;
                        animation.To = BorderButtonPanelInformationController.Width;
                    }
                    BorderCommandInformation.BeginAnimation(WidthProperty, animation);
                }
                else if (BorderCommandInformation.HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
                {
                    BorderCommandInformation.Width = double.NaN;
                    BorderCommandInformation.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                }
                else
                {
                    BorderCommandInformation.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    BorderCommandInformation.Width = BorderButtonPanelInformationController.Width;
                }
            };
            #endregion

            #region TextBoxCommandInput
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Violet].ConnectPalleteFromIELElement(TextBoxCommandInput);
            TextBoxCommandInput.PreviewKeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Back:
                        if (!HitUse) return;
                        if (TextBoxCommandInput.Text.Length > 1)
                        {
                            if (TextBoxCommandInput.Text[^1] == '*'
                            && TextBoxCommandInput.Text.AsSpan().Count('*') == 1
                            && HitCommandsInterpreter.StateVisibleHit == OPLHitInterpreter.HitStateEnum.VisibleOneCommand)
                            {
                                HitCommandsInterpreter.UsingAllHintCommand(TextBoxCommandInput.Text);
                            }
                        }
                        else HitCommandsInterpreter.ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);
                        return;
                    case Key.Tab:
                        HitCommandsInterpreter.NextSelectHit(false);
                        e.Handled = true;
                        break;
                }
            };
            TextBoxCommandInput.KeyDown += (sender, e) =>
            {
                if (!SaveKeyDown)
                {
                    SaveKeyDown = true;
                    if (SelectNavigation == SelectNavigationPageConsoleEnum.BufferCommandTextBox)
                        SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                    if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Enter && e.Key != Key.Escape)
                    {
                        SaveStringPrintBuffer = string.Empty;
                    }
                }
            };
            TextBoxCommandInput.KeyUp += async (sender, e) =>
            {
                SaveKeyDown = false;
                switch (e.Key)
                {
                    case Key.Enter:
                        SaveStringPrintBuffer = string.Empty;
                        TextBoxCommandInput.SetActiveSpecrum(StateSpectrum.Used, true);
                        if (SelectNavigation == SelectNavigationPageConsoleEnum.HitCommands)
                        {
                            SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                            TextBoxCommandInput.Text += "* ";
                        }
                        else if (TextBoxCommandInput.Text.Length > 0)
                        {
                            if (HitCommandsInterpreter.StateVisibleHit != OPLHitInterpreter.HitStateEnum.Hidden)
                                HitCommandsInterpreter.ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);

                            string Command = TextBoxCommandInput.Text;
                            TextBoxCommandInput.Text = string.Empty;
                            await ActivateCommand(Command);
                        }
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Text = SaveStringPrintBuffer.Length > 0 ? SaveStringPrintBuffer : string.Empty;
                        SaveStringPrintBuffer = string.Empty;
                        if (TextBoxCommandInput.Text.Length > 0)
                        {
                            SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                        }
                        else if (HitCommandsInterpreter.StateVisibleHit != OPLHitInterpreter.HitStateEnum.Hidden)
                            HitCommandsInterpreter.ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);
                        break;
                    case Key.Apps:
                        //if (!App.MainWindow.IELActionPanelMain.PanelActionActivate)
                        //    App.MainWindow.IELActionPanelMain.OpenPanelAction(RichTextBoxMainMessage, ConsolePage,
                        //        PositionAnimActionPanel.CenterObject, OrientationPositionCursor.LeftUp);
                        //else
                        //    App.MainWindow.IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                        break;
                    case Key.Tab:
                    case Key.Down:
                    case Key.Up:
                        if (HitCommandsInterpreter.StateVisibleHit == OPLHitInterpreter.HitStateEnum.Hidden && BufferPage.BufferCommand != null)
                        {
                            if (e.Key == Key.Up)
                            {
                                if (BufferPage.BufferCommand.Count == 0) return;
                                if (ActiveIndexBufferInput == -1)
                                {
                                    SaveStringPrintBuffer = TextBoxCommandInput.Text;
                                    ActiveIndexBufferInput = BufferPage.BufferCommand.Count - 1;
                                }
                                else ActiveIndexBufferInput = ActiveIndexBufferInput > 0 ? ActiveIndexBufferInput - 1 : BufferPage.BufferCommand.Count - 1;
                                TextBoxCommandInput.Text = BufferPage.BufferCommand.BufferElements[ActiveIndexBufferInput];
                            }
                            else if (e.Key == Key.Down || e.Key == Key.Tab)
                            {
                                if (BufferPage.BufferCommand.Count == 0) return;
                                if (ActiveIndexBufferInput == -1)
                                {
                                    SaveStringPrintBuffer = TextBoxCommandInput.Text;
                                    ActiveIndexBufferInput = 0;
                                }
                                else ActiveIndexBufferInput = ActiveIndexBufferInput < BufferPage.BufferCommand.Count - 1 ? ActiveIndexBufferInput + 1 : 0;
                                TextBoxCommandInput.Text = BufferPage.BufferCommand.BufferElements[ActiveIndexBufferInput];
                            }
                        }
                        else return;
                        break;
                    default:
                        if (SelectNavigation == SelectNavigationPageConsoleEnum.HitCommands)
                            SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                        break;
                }
                if (HitUse && SelectNavigation != SelectNavigationPageConsoleEnum.HitCommands)
                {
                    if (TextBoxCommandInput.Text.Length > 0 && TextBoxCommandInput.Text.Contains('*') &&
                        HitCommandsInterpreter.StateVisibleHit != OPLHitInterpreter.HitStateEnum.VisibleOneCommand)
                    {
                        ActiveIndexBufferInput = -1;
                        HitCommandsInterpreter.UsingOneHitCommand(TextBoxCommandInput.Text);
                        return;
                    }
                    else if (TextBoxCommandInput.Text.Length == 0 && HitCommandsInterpreter.StateVisibleHit != OPLHitInterpreter.HitStateEnum.Hidden)
                        HitCommandsInterpreter.ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);
                    else if (!TextBoxCommandInput.Text.Contains('*') && TextBoxCommandInput.Text.Length > 0)
                    {
                        ActiveIndexBufferInput = -1;
                        HitCommandsInterpreter.UsingAllHintCommand(TextBoxCommandInput.Text);
                    }
                }
            };
            TextBoxCommandInput.Focus();
            #endregion

            #region IELImageButtonHelp
            //App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELImageButtonHelp);
            //IELImageButtonHelp.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.LightBulb));
            //IELImageButtonHelp.OnActivateMouseLeft += (sender, e) =>
            //{
            //    WindowDiscriptionCommands WindowDescription = new()
            //    {
            //        //ManagerAnimation = App.CurrentApp.ManagerAnimation,
            //    };
            //    App.CurrentApp.InicializeWindowInApplication(WindowDescription);
            //    WindowDescription.Show();
            //};
            //IELImageButtonHelp.MouseHover += (sender, e) =>
            //{
            //    App.MainWindow.IELMessageMain.UsingBorderInformation(IELImageButtonHelp,
            //        "Быстрое открытие описания команд",
            //        OrientationPositionCursor.LeftDown);
            //};
            //IELImageButtonHelp.MouseLeave += (sender, e) =>
            //{
            //    App.MainWindow.IELMessageMain.CloseBorderInformation();
            //};
            #endregion

        }

        #region PanelActionManipulate
        /// <summary>
        /// Исполнить использование панели действий под страницу консоли
        /// </summary>
        /// <param name="SelectViewer">Визуализатор команды который был выделен</param>
        private void UsingPanelActionFromConsolePage(OPLCommandViewer? SelectViewer)
        {
            PageConsoleActionPanelMain.CommandViewerSelect = SelectViewer;
            PageConsoleActionPanelMain.IELButtonCommandBuffer.IsEnabled = BufferPage != null;
            App.MainWindow.IELActionPanelMain.UsingPanelAction(IELScrollConsole, PageConsoleActionPanelMain,
                Orientation: OrientationPositionCursor.RightDown);
        }
        #endregion

        /// <summary>
        /// Установить состояние выделяемой навигации
        /// </summary>
        /// <param name="Value">Присваемое значение</param>
        private void SetSelectNavigation(SelectNavigationPageConsoleEnum Value)
        {
            SelectNavigation = Value;
        }

        #region Command
        /// <summary>
        /// Создать новый визуализационный объект контента выполнения консольной команды
        /// </summary>
        /// <returns></returns>
        public OPLCommandViewer CreateNewCommandViewer(string Command)
        {
            OPLCommandViewer Viewer = new()
            {
                Margin = new(0),
                FontSize = 16d,
                CornerRadius = new(6),
                BorderThickness = new(2),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Opacity = 0d,
                DeleteButtonSource = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross)),
                Text = Command,
                IsAnimatedSettingQ = false,
                IsEnabledSettingQ = false,
            };
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(Viewer);
            System.Windows.Data.Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["RussianRail G Pro"]
            };
            BindingOperations.SetBinding(Viewer, FontFamilyProperty, binding);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(Viewer);

            Viewer.MouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && App.MainWindow.IELActionPanelMain.PanelActionActivate)
                    App.MainWindow.IELActionPanelMain.ClosePanelAction();
                else if (e.ChangedButton == MouseButton.Right) UsingPanelActionFromConsolePage(sender as OPLCommandViewer);
                e.Handled = true;
            };

            Viewer.ButtonDelete_OnActivateMouseLeft += (sender, e) =>
            {
                if (Viewer.IsTokenAsyncLoadingEnabled || Viewer.IsTokenAsyncWhileEnabled)
                {
                    MessageBoxResult Result =
                        System.Windows.MessageBox.Show("Вы точно хотите принудительно завершить выполнение команды?", "Подтверждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (Result == MessageBoxResult.No)
                    {
                        e.Handled = true;
                        return;
                    }
                    else if (Viewer.IsTokenAsyncLoadingEnabled)
                        Viewer.CancelExecuteTaskCommand();
                    else if (Viewer.IsTokenAsyncWhileEnabled)
                        Viewer.ExitAsyncWhileOperation();
                }
                DeleteCommandViewer(Viewer);
                e.Handled = true;
            };

            Viewer.AddContentInViewer += (sender, e) =>
            {
                //if (sender is OPLCommandViewer viewer && GridConsole.Children.IndexOf(viewer) + 1 < GridConsole.Children.Count)
                //{
                //    double ChangeTop = viewer.Margin.Top + viewer.ActualHeight + 10d;
                //    FrameworkElement Element;
                //    for (int i = GridConsole.Children.IndexOf(viewer) + 1; i < GridConsole.Children.Count; i++)
                //    {
                //        Element = (FrameworkElement)GridConsole.Children[i];
                //        Element.BeginAnimation(MarginProperty, null);
                //        Element.Margin = new(5, ChangeTop, 5, 5);
                //    }
                //}
            };

            StackPanelConsole.Children.Add(Viewer);
            //IELScrollConsole.SourceViewer.ScrollToEnd();
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Viewer, MarginProperty,
                new Thickness(5), TimeSpan.FromMilliseconds(600d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Viewer, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(600d));
            return Viewer;
        }

        /// <summary>
        /// Удалить элемент визуализации команды из страницы консоли
        /// </summary>
        /// <param name="Element">Удаляемый визуализационный элемент</param>
        internal void DeleteCommandViewer(OPLCommandViewer Element)
        {
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Element, MarginProperty,
                new Thickness(0), TimeSpan.FromMilliseconds(300d));
            if (ManagerAnimation != null)
            {
                DoubleAnimation animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animation.Duration = TimeSpan.FromMilliseconds(400d);
                animation.From = Element.ActualHeight;
                animation.To = 0d;
                Element.BeginAnimation(HeightProperty, animation);
                animation.FillBehavior = FillBehavior.Stop;
                animation.Completed += (sender, e) =>
                {
                    //Element.BeginAnimation(OpacityProperty, null);
                    //Element.BeginAnimation(HeightProperty, null);
                    //Element.Height = 0d;
                    //Element.Opacity = 0d;
                    StackPanelConsole.Children.Remove(Element);
                };
                animation.From = 1d;
                Element.BeginAnimation(OpacityProperty, animation);
            } else StackPanelConsole.Children.Remove(Element);
        }

        /// <summary>
        /// Активировать команду под логикой страницы консоли<br/>
        /// Команда учитывается из текстового поля
        /// </summary>
        /// <param name="Command">Выполняемая команда</param>
        /// <returns></returns>
        public async Task ActivateCommand(string Command)
        {
            if (Command.Length == 0) return;
            BufferPage?.InsertCommandFromBuffer(Command, this);

            await App.CurrentApp.ActivateActionCommand(CreateNewCommandViewer(COMInterpreterBase.ReadNameCommand(Command)), Command);
        }
        #endregion
    }
}
