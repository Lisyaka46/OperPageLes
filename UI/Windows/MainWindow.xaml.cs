#region Link
using AAC20.CORE;
using AAC20.CORE.Flaging;
using AAC20.CORE.Settings;
using AAC20.UI.Dialogs;
using AAC20.UI.Pages.ActionPanel;
using AAC20.Windows.Frames;
using AAC20.Windows.Pages.ActionPanel;
using AAC20.Windows.Pages.Browser;
using IEL;
using IEL.Classes;
using IEL.Interfaces.Core;
using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace AAC20.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Флаги данной формы
        /// </summary>
        private readonly struct Flags
        {
            /// <summary>
            /// Флаг соеденения с интернетом
            /// </summary>
            internal static readonly Flag FlagInternetConnection = new(false);

            /// <summary>
            /// Флаг состояния видимости объекта страниц
            /// </summary>
            internal static readonly Flag FlagFrameComponentVisible = new(true);

            /// <summary>
            /// Флаг состояния регистра
            /// </summary>
            internal static readonly Flag FlagRegisterState = new(Console.CapsLock);

            /// <summary>
            /// Флаг обновления подсказок к командам
            /// </summary>
            internal static readonly Flag FlagHintRead = new(false);
        };

        /// <summary>
        /// Главная страница панели действий в консоли
        /// </summary>
        private static readonly PageMainConsolePanelAction PageConsolePA = new();

        /// <summary>
        /// Страница взаимодействия с вкладками браузера страниц
        /// </summary>
        private static readonly PageActionInlay PageManipulateInlayPA = new();

        /// <summary>
        /// Реальное время
        /// </summary>
        private static string RealTime => DateTime.Now.ToString("HH:mm:ss");

        /// <summary>
        /// Реальное время
        /// </summary>
        private static string RealData => DateTime.Now.ToString("dd.MM.yyyy");

        /// <summary>
        /// Объект управления фоновым обновлением информации в данном окне 1000
        /// </summary>
        private readonly UpdateBackgroundData UpdateBackgroundDataThis;

        /// <summary>
        /// Объект управления фоновым обновлением информации в данном окне 1
        /// </summary>
        private readonly UpdateBackgroundData UpdateBackgroundDataRunTime;

        /// <summary>
        /// Поток обновляемый совпадающие команды
        /// </summary>
        private readonly ThreadGenericProcess UpdateSearchHintCommand;

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления double значением
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimateObj = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления Color значением
        /// </summary>
        private static readonly ColorAnimation ColorAnimate = new(Colors.Black, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Настройки панели действий в консоли
        /// </summary>
        private readonly PanelActionSettingsFrameworkElement PASettingsConsole;

        /// <summary>
        /// Настройки панели действий в браузере
        /// </summary>
        private readonly PanelActionSettingsFrameworkElement PASettingsBrowserManipulateInlay;

        //private MMDeviceEnumerator Device = new();

        /// <summary>
        /// Состояние воспроизведения приветственной анимации
        /// </summary>
        private bool HiAnimation = false;

        /// <summary>
        /// Массив всех отсортированных имён команд
        /// </summary>
        private string[] AllHintNames;

        /// <summary>
        /// Константа высоты элемента подсказки к командам
        /// </summary>
        private const int HeightHintElement = 20;

        /// <summary>
        /// Строка вывода перед сообщением
        /// </summary>
        public const string ConsolePreMessage = "%**>>>**";

        public MainWindow()
        {
            InitializeComponent();

            #region Command
            #if DEBUG
            App.DataConsoleCommand.AddRange([
                #region anim
                new ConsoleCommand("anim", [new Parameter("Value", typeof(bool))],
                "Отключает или включает анимацию у окна ярлыков",
                (Command, param) =>
                {
                    PageLabels? Page = IELBrowserPageMain.SearchPageType<PageLabels>();
                    if (Page == null)
                        return Task.FromResult(CommandStateResult.Failed(Command.Name,
                            $"Страница \"{nameof(PageLabels)}\" в браузере не инициализирована!"));
                    if ((bool)param[0]) Page.AnimationLoadingStart();
                    else Page.AnimationLoadingStop();
                    return Task.FromResult(CommandStateResult.Completed(Command.Name));
                }),
                #endregion
            ]);
            #endif
            #endregion

            #region Event Flags
            Flags.FlagInternetConnection.ChangeStateFlag += (NewValue) =>
            {
                ImageInternetConnection.Source = new BitmapImage(new Uri($"{App.PathImageApplication}/Wifi{(NewValue ? "On" : "Off")}.png", UriKind.Relative));
                App.AnimateBlurEffect(BlurEffectImageInternetConnection, 10u);
            };
            Flags.FlagRegisterState.ChangeStateFlag += (NewValue) =>
            {
                TextBlockRegister.Text = NewValue ? "A" : "a";
                App.AnimateBlurEffect(BlurEffectTextBlockRegister, 10u);
                if (IELMessageMain.FlagMessage && IELMessageMain.NameParentObject.Equals(BorderStateRegister.Name))
                    IELMessageMain.UsingBorderInformation(BorderStateRegister, BorderStateRegister.Name, Flags.FlagRegisterState ?
                        "Установлен большой регистр" : "Установлен малый регистр",
                        IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            #endregion

            #region Event Pages
            #region PageConsolePA
            PageConsolePA.IELButtonCrearConsole.OnActivateMouseLeft += (AltMode) =>
            {
                RichTextBoxMainMessage.Document = new();
                IELActionPanelMain.ClosePanelAction();
            };
            PageConsolePA.IELButtonCrearConsole.OnActivateMouseRight += (AltMode) => RichTextBoxMainMessage.Document = new();

            PageConsolePA.IELButtonCommandBuffer.OnActivateMouseLeft += (AltMode) =>
            {
                IELActionPanelMain.NextPage(App.CurrentApp.AllPages.PageBuffer);
            };

            PageConsolePA.IELButtonDiscriptionCommand.OnActivateMouseLeft += (AltMode) =>
            {
                IELActionPanelMain.ClosePanelAction();
                UsingDiscriptionCommand();
            };
            #endregion
            #region PageManipulateInlayPA
            PageManipulateInlayPA.IELButtonPageOpenInlay.OnActivateMouseLeft += (AltMode) =>
            {
                if (PageManipulateInlayPA.ActivateManipulateInlay != null)
                    IELBrowserPageMain.ActivateInInlay(PageManipulateInlayPA.ActivateManipulateInlay);
            };
            PageManipulateInlayPA.IELButtonPageDeleteInlay.OnActivateMouseLeft += (AltMode) =>
            {
                IELActionPanelMain.ClosePanelAction();
                if (PageManipulateInlayPA.ActivateManipulateInlay != null)
                    IELBrowserPageMain.DeleteInlayPage(PageManipulateInlayPA.ActivateManipulateInlay);
            };
            #endregion

            App.CurrentApp.AllPages.PageBuffer.IELButtonBackMainMenu.OnActivateMouseLeft += (AltMode) =>
            {
                IELActionPanelMain.NextPage(PageConsolePA, false);
            };
            #endregion

            #region BackgroundData
            UpdateBackgroundDataThis = new(1000d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualData));
            UpdateBackgroundDataRunTime = new(1d, (sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualDataRunTime));
            UpdateSearchHintCommand = new(() =>
            {
            });
            BackgroundUpdateVisualData();
            #endregion

            #region SetParameteres
            AllHintNames = [];
            TextBlockRegister.Text = Flags.FlagRegisterState ? "A" : "a";
            BrowserPageColumn.MaxWidth = 0d;
            IELMessageMain.Opacity = 0d;
            IELActionPanelMain.Opacity = 0d;
            BorderHintCommand.Height = 0d;
            GridHintCommandParameter.Opacity = 0d;
            Canvas.SetZIndex(GridHintCommandParameter, -1);
            RichTextBoxMainMessage.Document = new();
            PASettingsConsole = new(RichTextBoxMainMessage, PageConsolePA, new(270d, 230d));
            PASettingsBrowserManipulateInlay = new(IELBrowserPageMain, PageManipulateInlayPA, new(200d, 240d));

            Canvas.SetZIndex(IELMessageMain, -2);
            Canvas.SetZIndex(IELActionPanelMain, -2);
            #endregion

            ButtonReturnCommand.OnActivateMouseLeft += () => ActivateActionCommand(TextBoxCommandInput.Text, true);
            SizeChanged += (sender, e) => IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);

            UpdateImageMenu();
            //Closing += (sender, e) => App.Current.Shutdown(0);

            #region UpToolButtons

            IELButtonSettings.OnActivateMouseLeft += () =>
            {
                new WindowSetting().ShowDialog();
            };
            #endregion

            IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                TextBoxCommandInput.Focus();
            };

            IELBrowserPageMain.EventCloseBrowser += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            IELBrowserPageMain.EventChangeActiveInlay += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };

            #region IELButtonFrameComponentVisible
            IELButtonBrowserPageVisible.RenderTransform = new TransformGroup();
            ((TransformGroup)IELButtonBrowserPageVisible.RenderTransform).Children.Add(new RotateTransform(0d, 0d, 0d));

            IELButtonBrowserPageVisible.OnActivateMouseLeft += () => 
            {
                UsingChangeStateFrameComponent();
                IELMessageMain.CloseBorderInformation();
            };
            IELButtonBrowserPageVisible.OnActivateMouseRight += () =>
            {
                //FrameComponent.CloseFrame();
                IELMessageMain.CloseBorderInformation();
            };
            IELButtonBrowserPageVisible.MouseHover += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELButtonBrowserPageVisible, IELButtonBrowserPageVisible.Name,
                    (Flags.FlagFrameComponentVisible ? "Скрыть" : "Показать") + " глобальные страницы",
                    IELBlockMessage.OrientationBorderInfo.LeftUp);
            };
            IELButtonBrowserPageVisible.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            UsingChangeStateFrameComponent();
            #endregion

            #region IELBrowserPage
            IELBrowserPageMain.IELButtonAddInlay.OnActivateMouseLeft += () =>
            {
                IELActionPanelMain.ClosePanelAction();
                new WindowBrowserPagesManager().AddNewPageInBrowser(IELBrowserPageMain);
            };
            IELBrowserPageMain.EventActiveActionInInlay += (Inlay) =>
            {
                PageManipulateInlayPA.ActivateManipulateInlay = Inlay;
                IELActionPanelMain.UsingPanelAction(PASettingsBrowserManipulateInlay);
                //DialogManagerPage.ShowDialog();
            };
            #endregion

            #region TextBoxCommandInput
            TextBoxCommandInput.PreviewKeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Back:
                        if (TextBoxCommandInput.Text.Length > 0)
                        {
                            if (TextBoxCommandInput.Text[^1] == '*')
                            {
                                UsingAnimateBorderHintCommand(false);
                                UsingAnimateBorderCollectionHintCommand(true);
                            }
                        }
                        return;
                }
            };
            TextBoxCommandInput.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(160, 245, 200), TimeSpan.FromMilliseconds(90d)));
                        UsingAnimateBorderCollectionHintCommand(false);
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(255, 122, 84), TimeSpan.FromMilliseconds(90d)));
                        UsingAnimateBorderCollectionHintCommand(false);
                        break;
                }
            };
            TextBoxCommandInput.TextChanged += (sender, e) =>
            {
                if (TextBoxCommandInput.Text.Length > 0)
                {
                    if (TextBoxCommandInput.Text[^1] == '*')
                    {
                        if (GridHint.Children.Count > 0)
                        {
                            string CommandText = ICommandAAC.ReadNameCommand(TextBoxCommandInput.Text[..^1]);
                            if (((TextBlock)GridHint.Children[0]).Text.Equals(CommandText))
                            {
                                UsingAnimateBorderHintCommand(true);
                                return;
                            }
                        }
                        UsingAnimateBorderCollectionHintCommand(false);
                        return;
                    }
                }
            };
            TextBoxCommandInput.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        ActivateActionCommand(TextBoxCommandInput.Text, true);
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Text = string.Empty;
                        break;
                    case Key.Apps:
                        IELActionPanelMain.UsingPanelAction(PASettingsConsole);
                        break;
                }
                TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(120, 204, 160), TimeSpan.FromMilliseconds(430d)));
                
                DoubleAnimation animation = DoubleAnimateObj.Clone();
                animation.Duration = TimeSpan.FromMilliseconds(300d);
                if (!TextBoxCommandInput.Text.Contains('*')) UsingAnimateBorderCollectionHintCommand(TextBoxCommandInput.Text.Length > 0);
            };
            #endregion

            RichTextBoxMainMessage.MouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
                else if (e.ChangedButton == MouseButton.Right) IELActionPanelMain.UsingPanelAction(PASettingsConsole);
            };

            RichTextBoxMainMessage.TextChanged += (sender, e) =>
            {
                RichTextBoxMainMessage.ScrollToEnd();
            };

            #region BorderInternetConnection
            BorderInternetConnection.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(BorderInternetConnection, BorderInternetConnection.Name, Flags.FlagInternetConnection ?
                    "Есть подключение к интернету" : "Нет подключения к интернету",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            BorderInternetConnection.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region BorderStateRegister
            BorderStateRegister.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(BorderStateRegister, BorderStateRegister.Name, Flags.FlagRegisterState ?
                    "Установлен большой регистр" : "Установлен малый регистр",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            BorderStateRegister.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region BorderCurrentLanguage
            BorderCurrentLanguage.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(BorderCurrentLanguage, BorderCurrentLanguage.Name,
                    "Текущий язык раскладки клавиатуры",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            BorderCurrentLanguage.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region IELImageButtonHelp
            IELImageButtonHelp.OnActivateMouseLeft += UsingDiscriptionCommand;
            IELImageButtonHelp.MouseHover += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELImageButtonHelp, IELImageButtonHelp.Name,
                    "Быстрое открытие описания команд",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            IELImageButtonHelp.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion

            ImageLogoApplication.MouseEnter += (sender, e) =>
            {
                DoubleAnimateObj.To = 0.6d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };

            ImageLogoApplication.MouseLeave += (sender, e) =>
            {
                DoubleAnimateObj.To = 1d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };

            ImageLogoApplication.MouseDown += (sender, e) =>
            {
                DoubleAnimateObj.To = 0.4d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            };

            ImageLogoApplication.MouseUp += (sender, e) =>
            {
                DoubleAnimateObj.To = 1d;
                ImageLogoApplication.BeginAnimation(OpacityProperty, DoubleAnimateObj);
                Dialogs.LicenseWindow License = new();
                License.ShowDialog();
            };
            KeyDown += (sender, e) =>
            {
                if (e.Key == Key.CapsLock) Flags.FlagRegisterState.Value = Console.CapsLock;
            };
            Activated += (sender, e) =>
            {
                if (!HiAnimation)
                {
                    HiAnimation = true;

                    #region Anim Start
                    #region 1
                    ThicknessAnimate.Duration = TimeSpan.FromMilliseconds(1400d);

                    ThicknessAnimate.From = new(8);
                    ThicknessAnimate.To = BorderImageInformation.Margin;
                    BorderImageInformation.BeginAnimation(MarginProperty, ThicknessAnimate);

                    TimeDataColumnDefinition.MaxWidth = 0d;
                    DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(1200d);
                    DoubleAnimateObj.To = 124d;
                    Storyboard storyboard = new();
                    storyboard.Children.Add(DoubleAnimateObj);
                    Storyboard.SetTarget(DoubleAnimateObj, TimeDataColumnDefinition);
                    Storyboard.SetTargetProperty(DoubleAnimateObj, new PropertyPath("(ColumnDefinition.MaxWidth)"));
                    storyboard.Begin();
                    DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(250d);

                    ThicknessAnimate.From = new(8);
                    ThicknessAnimate.To = BorderDateTime.Margin;
                    BorderDateTime.BeginAnimation(MarginProperty, ThicknessAnimate);

                    ThicknessAnimate.From = null;
                    ThicknessAnimate.Duration = TimeSpan.FromMilliseconds(1400d);

                    #endregion
                    #endregion
                }
                //TextBoxCommandInput.Focus();
                /*GridMain.RenderTransform = new TransformGroup()
                {
                    Children = [
                        new RotateTransform(9d),
                        new ScaleTransform(0.3d, 0.3d)
                        ]
                };*/
                /*DoubleAnimateObj.To = 0d;
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(1200d);
                ((RotateTransform)((TransformGroup)GridMain.RenderTransform).Children[0]).BeginAnimation(RotateTransform.AngleProperty, DoubleAnimateObj);
                DoubleAnimateObj.To = 1d;
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(1200d);
                ((ScaleTransform)((TransformGroup)GridMain.RenderTransform).Children[1]).BeginAnimation(ScaleTransform.ScaleXProperty, DoubleAnimateObj);
                ((ScaleTransform)((TransformGroup)GridMain.RenderTransform).Children[1]).BeginAnimation(ScaleTransform.ScaleYProperty, DoubleAnimateObj);
                DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(300d);*/
            };

            UpdateBackgroundDataThis.TimerDataUpdate.Start();
            UpdateBackgroundDataRunTime.TimerDataUpdate.Start();
            TextBoxCommandInput.Focus();
        }

        #region HintCommandManipulate
        /// <summary>
        /// Манипулировать анимацией борьера подсказок к командам через всю коллекцию
        /// </summary>
        /// <param name="Activate">Активировать или дизактивировать аинмацией</param>
        private void UsingAnimateBorderCollectionHintCommand(bool Activate)
        {
            DoubleAnimation animation = DoubleAnimateObj.Clone();
            animation.Duration = TimeSpan.FromMilliseconds(300d);
            if (Activate)
            {
                string CommandText = ICommandAAC.ReadNameCommand(TextBoxCommandInput.Text);
                AllHintNames = [.. App.CurrentApp.AllNamesCommand.Where((i) => { return i.Contains(CommandText, StringComparison.CurrentCultureIgnoreCase); })];
                if (AllHintNames.Length == GridHint.Children.Count) return;
                GridHint.Children.Clear();
                if (AllHintNames.Length > 0)
                {
                    Sorting.SortNames(ref AllHintNames);
                    foreach (string Name in AllHintNames)
                    {
                        TextBlock block = CreateHintBlock(Name, GridHint.Children.Count);
                        GridHint.Children.Add(block);
                    }
                }
            }
            else GridHint.Children.Clear();
            if (Canvas.GetZIndex(GridHintCommandParameter) == 1 && !Activate) UsingAnimateBorderHintCommand(false);
            animation.To = Activate && GridHint.Children.Count > 0 ? 
                GridHint.Children.Count * HeightHintElement + BorderHintCommand.Padding.Top + BorderHintCommand.Padding.Bottom : 0d;
            BorderHintCommand.BeginAnimation(HeightProperty, animation);
        }

        /// <summary>
        /// Манипулировать анимацией борьера подсказок к командам
        /// </summary>
        /// <param name="Activate">Активировать или дизактивировать аинмацией</param>
        /// <param name="CommandTextActualHint">Константный текст поиска команды</param>
        private void UsingAnimateBorderHintCommand(bool Activate, string? CommandTextActualHint = null)
        {
            DoubleAnimation animation = DoubleAnimateObj.Clone();
            animation.Duration = TimeSpan.FromMilliseconds(300d);
            if (Activate)
            {
                ICommandAAC? CommandHint = null;
                string TextCommand = CommandTextActualHint ?? ((TextBlock)GridHint.Children[0]).Text;
                CommandHint ??= ICommandAAC.ReadCommand([.. App.DataConsoleCommand], TextCommand);
                CommandHint ??= ICommandAAC.ReadCommand([.. App.CurrentApp.DataAliases], TextCommand);
                if (CommandHint == null) return;
                Parameter[] Parameters = CommandHint.Parameters ?? [];
                TextBlockHintCommand.Text = $"{CommandHint.Name}* ";
                for (int i = 0; i < Parameters.Length; i++)
                {
                    TextBlockHintCommand.Text += $"{Parameters[i].Name}" +
                        $"{(Parameters[i].Absolutly ? string.Empty : '?')}" +
                        $"{(i < Parameters.Length - 1 ? ", " : string.Empty)}";
                }
                animation.To = HeightHintElement + 
                    BorderHintCommand.Padding.Top + BorderHintCommand.Padding.Bottom + 
                    GridHintCommandParameter.Margin.Top + GridHintCommandParameter.Margin.Bottom;
                BorderHintCommand.BeginAnimation(HeightProperty, animation);
            }
            animation.To = Activate ? 1d : 0d;
            Canvas.SetZIndex(GridHintCommandParameter, Activate ? 1 : -1);
            GridHintCommandParameter.BeginAnimation(OpacityProperty, animation);

            animation.To = Activate ? 0d : 1d;
            GridHint.BeginAnimation(OpacityProperty, animation);

            animation.To = Activate ? 300d : 142d;
            BorderHintCommand.BeginAnimation(WidthProperty, animation);
        }

        /// <summary>
        /// Создать объект подсказки к команде
        /// </summary>
        /// <param name="Name">Имя команды</param>
        /// <param name="Index">Индекс местоположения по оси Y</param>
        /// <returns>Объект подсказки к команде</returns>
        private TextBlock CreateHintBlock(string Name, int Index)
        {
            ColorAnimation color_animation = ColorAnimate.Clone();
            color_animation.Duration = TimeSpan.FromMilliseconds(120d);
            TextBlock Result = new()
            {
                Height = HeightHintElement,
                Text = Name,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new(0, HeightHintElement * Index, 0, 0),
                Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                Cursor = Cursors.Hand,
            };
            Result.MouseEnter += (sender, e) =>
            {
                color_animation.To = Color.FromRgb(255, 255, 255);
                Result.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, color_animation);
            };
            Result.MouseLeave += (sender, e) =>
            {
                color_animation.To = Color.FromRgb(0, 0, 0);
                Result.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, color_animation);
            };
            Result.MouseLeftButtonUp += (sender, e) =>
            {
                TextBoxCommandInput.Text = $"{Result.Text}*";
                UsingAnimateBorderHintCommand(true, Result.Text);
            };
            return Result;
        }
        #endregion

        /// <summary>
        /// Взаимодействовать с окном описания команд (Включает/Активирует)
        /// </summary>
        private void UsingDiscriptionCommand()
        {
            if (App.AppWindows.DiscriptionCommands == null)
            {
                App.AppWindows.DiscriptionCommands = new();
                App.AppWindows.DiscriptionCommands.Show();
            }
            else
            {
                App.AppWindows.DiscriptionCommands.WindowState = WindowState.Normal;
                App.AppWindows.DiscriptionCommands.Activate();
            }
        }

        #region ManipulateText
        /// <summary>
        /// Добавить и отформатировать текст в консоль
        /// </summary>
        /// <param name="Text">Текст добавляемый в консоль</param>
        /// <param name="Formatted">Форматировать или нет</param>
        internal void AddTextInConsole(string Text, bool Formatted = true)
        {
            if (Text.Length == 0) return;
            Text = $"{ConsolePreMessage} {Text}";
            Paragraph Message = new();
            if (Formatted)
            {
                List<Inline> Inlines = [];
                foreach (Match match in RegexFormattedText().Matches(Text))
                {
                    Inlines.Add(FormattedTextDetect(match.Value));
                }
                Message.Inlines.AddRange(Inlines);
            }
            else Message.Inlines.Add(new Run(Text));
            RichTextBoxMainMessage.Document.Blocks.Add(Message);
        }

        /// <summary>
        /// Очистить текст консоли
        /// </summary>
        internal void CleatConsoleText() => RichTextBoxMainMessage.Document = new();

        /// <summary>
        /// Изменить формативность текста с учётом первых знаков
        /// </summary>
        /// <remarks>
        /// <code>
        /// %#FFFFFF** <b>Italic</b> **
        /// </code>
        /// ** <b>Bold</b> **
        /// <code></code>
        /// // <i>Italic</i> //
        /// <code></code>
        /// __ <u>UnderLine</u> __
        /// <code></code>
        /// </remarks>
        /// <param name="Text">Текст форматирования</param>
        /// <returns>Форматированный текст</returns>
        private static Inline FormattedTextDetect(string Text)
        {
            if (Text.Length == 0 || (Text.Length == 1 && Text[0] == '%')) return new Run(Text);
            if (Text[0] == '%') Text = Text[1..]; // удаление "%"
            SolidColorBrush? color = null;
            if (Text[0] == '#')
            {
                color = new((Color)ColorConverter.ConvertFromString(
                    RegexFormattedTextColor().Match(Text).Value));
                Text = Text[7..];
            }
            Inline Result = $"{Text[0]}{Text[^1]}" switch
            {
                "**" => new Bold(new Run(Text[2..^2])),
                "//" => new Italic(new Run(Text[2..^2])),
                "__" => new Underline(new Run(Text[2..^2])),
                _ => new Run(Text),
            };
            if (color != null) Result.Background = color;
            return Result;
        }
        #endregion

        /// <summary>
        /// Изменить состояние глобальных страниц на противоположное
        /// </summary>
        private void UsingChangeStateFrameComponent()
        {
            DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(560d);
            DoubleAnimateObj.To = Flags.FlagFrameComponentVisible ? 180d : 0d;
            ((RotateTransform)((TransformGroup)IELButtonBrowserPageVisible.RenderTransform).Children[0]).
                BeginAnimation(RotateTransform.AngleProperty, DoubleAnimateObj);

            DoubleAnimateObj.To = Flags.FlagFrameComponentVisible ? 0d : 420d;
            Storyboard storyboard = new();
            storyboard.Children.Add(DoubleAnimateObj);
            Storyboard.SetTarget(DoubleAnimateObj, BrowserPageColumn);
            Storyboard.SetTargetProperty(DoubleAnimateObj, new PropertyPath("(ColumnDefinition.MaxWidth)"));
            storyboard.Begin();
            DoubleAnimateObj.Duration = TimeSpan.FromMilliseconds(250d);
            Flags.FlagFrameComponentVisible.Value = !Flags.FlagFrameComponentVisible;
            IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);
        }

        #region CommandActivate
        /// <summary>
        /// Активировать команду
        /// </summary>
        /// <param name="CommandString">Строка команды</param>
        /// <param name="AppendBufferCommand">Состояние добавления команды в буфер</param>
        private void ActivateActionCommand(string CommandString, bool AppendBufferCommand = true)
        {
            if (CommandString.Length == 0) return;
            TextBoxCommandInput.Text = string.Empty;
            ConsoleCommand? Command = ICommandAAC.ReadCommand([.. App.DataConsoleCommand], CommandString);
            string Name = ICommandAAC.ReadNameCommand(CommandString);
            string[] Parameters = ICommandAAC.ReadParametersCommand(CommandString);

            if (AppendBufferCommand)
            {
                App.CurrentApp.AllPages.PageBuffer.InsertCommandFromBuffer(Name, CommandString,
                () =>
                {
                    ActivateActionCommand(CommandString);
                });
            }

            CommandStateResult result = Command == null ? CommandStateResult.FaledCommand(Name) : Command.ExecuteCommand(Parameters);
            if (result.State == ResultState.InvalidCommand)
            {
                AliasCommand<ICommandAAC>? Alias = ICommandAAC.ReadCommand([.. App.CurrentApp.DataAliases], CommandString);
                result = Alias == null ? CommandStateResult.FaledCommand(Name) : Alias.ExecuteCommand();
            }
            SummarizeCommandStateResult(result);
        }

        /// <summary>
        /// Активировать команду не добавляя в буфер
        /// </summary>
        /// <param name="CommandString">Строка команды</param>
        internal void ActivateActionCommand(string CommandString) => ActivateActionCommand(CommandString, false);

        /// <summary>
        /// Создать действие над итогом выполнения команды
        /// </summary>
        /// <param name="Result">Объект итога выполнения команды</param>
        [MTAThread()]
        internal void SummarizeCommandStateResult(CommandStateResult Result)
        {
            AddTextInConsole(Result.Message);
        }
        #endregion

        /// <summary>
        /// Функция обновления визуальной информации в данном окне 100
        /// </summary>
        private void BackgroundUpdateVisualData()
        {
            TextBlockTime.Text = RealTime;
            TextBlockData.Text = RealData;
            if (!App.InternetPinging.Wait)
            {
                if (IELMessageMain.FlagMessage && Flags.FlagInternetConnection.Value != App.InternetPinging &&
                    IELMessageMain.NameParentObject.Equals(BorderInternetConnection.Name))
                {
                    IELMessageMain.Opacity = 0d;
                    IELMessageMain.UsingBorderInformation(BorderInternetConnection, BorderInternetConnection.Name, App.InternetPinging ?
                    "Есть подключение к интернету" : "Нет подключения к интернету",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
                }
                Flags.FlagInternetConnection.Value = App.InternetPinging;
            }
        }

        /// <summary>
        /// Функция обновления визуальной информации в данном окне 60
        /// </summary>
        private void BackgroundUpdateVisualDataRunTime()
        {
            string LangName = System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture.NativeName[0..3].ToUpper();
            if (!LangName.Equals(TextBlockLanguage.Text))
            {
                TextBlockLanguage.Text = LangName;
                App.AnimateBlurEffect(BlurEffectLanguage, 10u);
            }
            //int Volume = (int)(Device.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioMeterInformation.MasterPeakValue * 1900);
            //if (Math.Abs(RectangleTest.Width - 50 - Volume) >= 13 && Volume != 0) Volume /= 5;
            //byte rgbValue = (byte)(2.55d * Volume);
            //TextBlockTest.Foreground = new SolidColorBrush(Color.FromRgb(rgbValue, rgbValue, rgbValue));
            //TextBlockTest.Text = $"Volume: {Volume}";
            //ImageTest.Margin = new(428 - (Volume / 2), 204 - (Volume / 2), 0, 0);
            //ImageTest.Width = 10 + Volume;
            //ImageTest.Height = 10 + Volume;

            /*try
            {
                Point MousePoint = Mouse.GetPosition(this);
                Point PointScreen = PointToScreen(new(0, 0));
                //TextBlockTest.Text = $"Point: {PointScreen.X}:{PointScreen.Y} - {MousePoint.X}:{MousePoint.Y} - {ActualWidth}:{ActualHeight}";
                if (-MousePoint.X == PointScreen.X && -MousePoint.Y == PointScreen.Y)
                {
                    return;
                }
                else MousePoint = new(
                    (MousePoint.X - (ActualWidth / 2)) / 3,
                    (MousePoint.Y - (ActualHeight / 2)) / 3);
                ImageMenu.Margin = new(MousePoint.X, MousePoint.Y, 0, 0);
            }
            catch { ImageMenu.Margin = new(0); }*/

        }

        #region ImageMenu
        /// <summary>
        /// Обновить фотовое изображение
        /// </summary>
        internal void UpdateImageMenu()
        {
            ImageIndificator.Opacity = 1d;
            string Path = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.PathMenuImage);
            BitmapImage BitmapImageMenu = new(new Uri(Path));
            if (File.Exists(Path))
            {
                ComplitedInstallImageMenu(BitmapImageMenu);
                return;
            };
            BitmapImageMenu.DownloadCompleted += (sender, e) =>
            {
                ComplitedInstallImageMenu(BitmapImageMenu);
            };
            BitmapImageMenu.DownloadFailed += (sender, e) => FailedInstallImageMenu();
            BitmapImageMenu.DecodeFailed += (sender, e) => FailedInstallImageMenu();
        }

        /// <summary>
        /// Успешная установка картинки фона
        /// </summary>
        /// <param name="bitmap">Карта изображения</param>
        private void ComplitedInstallImageMenu(BitmapImage bitmap)
        {
            ImageMenu.Source = bitmap;
            ThicknessAnimation animationThickness = ThicknessAnimate.Clone();
            DoubleAnimation animationDouble = DoubleAnimateObj.Clone();

            animationDouble.From = 10d;
            animationDouble.To = 0d;
            animationDouble.Duration = TimeSpan.FromMilliseconds(2300d);

            animationThickness.From = new(-4);
            animationThickness.To = new(0);
            animationThickness.Duration = TimeSpan.FromMilliseconds(2300d);

            BlurEffectImageMenu.BeginAnimation(BlurEffect.RadiusProperty, animationDouble);
            ImageMenu.BeginAnimation(MarginProperty, animationThickness);

            animationDouble.From = 0d;
            animationDouble.To = 1d;
            ImageMenu.BeginAnimation(OpacityProperty, animationDouble);

            animationDouble.From = 1d;
            animationDouble.To = 0d;
            animationDouble.Duration = TimeSpan.FromMilliseconds(700d);
            ImageIndificator.BeginAnimation(OpacityProperty, animationDouble);
        }

        /// <summary>
        /// Неудачное завершение установки картинки фона
        /// </summary>
        private void FailedInstallImageMenu()
        {
            DoubleAnimation animationDouble = DoubleAnimateObj.Clone();
            AddTextInConsole("Не удалось загрузить фоновое изображение...");

            animationDouble.From = 1d;
            animationDouble.To = 0d;
            animationDouble.Duration = TimeSpan.FromMilliseconds(700d);
            ImageIndificator.BeginAnimation(OpacityProperty, animationDouble);
        }
        #endregion

        #region Regex
        /// <summary>
        /// Функция регулярного выражения выделения текста в ковычках "текст"
        /// </summary>
        private static Regex StringCommandError(char symbol) => new($"([^\\{symbol}]+|\\{symbol}[^\\{symbol}]+\\{symbol}?)");

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        // Текст который является %#00FF00FF__%**регистрационным**__ и %#FFFFFF**может** %~~даже так~~ %--постоянно-- %__форматироваться__
        [GeneratedRegex(@"([^%]+|(\%(#[0-9A-F]{6})?)(\*{2}[^\*]+\*{2}|_{2}[^_]+_{2}|\/{2}[^\/]+\/{2})|\%)")]
        private static partial Regex RegexFormattedText();

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        // %   #FFFFFF   //%**d**//
        [GeneratedRegex(@"#[0-9A-F]{6}")]
        private static partial Regex RegexFormattedTextColor();
        #endregion
    }
}