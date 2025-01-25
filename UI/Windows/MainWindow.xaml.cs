#region Link
using AAC20.CORE;
using AAC20.CORE.Flaging;
using AAC20.CORE.Settings;
using AAC20.UI.Dialogs;
using AAC20.UI.Pages.ActionPanel;
using AAC20.UI.Pages.Browser;
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
using System.Windows.Automation.Text;
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
        /// Настройки панели действий в браузере
        /// </summary>
        private readonly PanelActionSettingsFrameworkElement PASettingsBrowserManipulateInlay;

        //private MMDeviceEnumerator Device = new();

        /// <summary>
        /// Состояние воспроизведения приветственной анимации
        /// </summary>
        private bool HiAnimation = false;

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
            TextBlockRegister.Text = Flags.FlagRegisterState ? "A" : "a";

            VisualRectangleDateTimeBackground.Opacity = 0d;
            IELMessageMain.Opacity = 0d;
            IELActionPanelMain.Opacity = 0d;
            
            PASettingsBrowserManipulateInlay = new(IELBrowserPageMain, PageManipulateInlayPA, new(200d, 240d));

            Canvas.SetZIndex(IELMessageMain, -2);
            Canvas.SetZIndex(IELActionPanelMain, -2);
            #endregion

            SizeChanged += (sender, e) => IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);

            #region Settings
            UpdateImageMenu();
            ChangeBlurImageInDataTime(App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.BlurBackgroundDataTime).Equals("T"));
            #endregion
            //Closing += (sender, e) => App.Current.Shutdown(0);

            #region UpToolButtons

            IELButtonSettings.OnActivateMouseLeft += () =>
            {
                new WindowSetting().ShowDialog();
            };
            #endregion

            IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                IPageDefault? Page = IELBrowserPageMain.ActualInlay?.Page;
                if (Page == null) return;
                switch(Page.PageName)
                {
                    case nameof(PageConsole):
                        ((PageConsole)Page).TextBoxCommandInput.Focus();
                        break;
                    default: return;
                }
            };

            IELBrowserPageMain.EventCloseBrowser += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            IELBrowserPageMain.EventChangeActiveInlay += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };

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
            IELImageButtonHelp.OnActivateMouseLeft += App.UsingDiscriptionCommand;
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
        }

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
            //VisualRectangleDateTimeBackground.Visual.
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

            ImageIndificator.Source = new BitmapImage(new Uri($"{App.PathImageApplication}/Done.png", UriKind.RelativeOrAbsolute));
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
            ImageIndificator.Source = new BitmapImage(new Uri($"{App.PathImageApplication}/Warning.png", UriKind.RelativeOrAbsolute));
            //AddTextInConsole("Не удалось загрузить фоновое изображение...");

            animationDouble.From = 1d;
            animationDouble.To = 0d;
            animationDouble.Duration = TimeSpan.FromMilliseconds(1000d);
            ImageIndificator.BeginAnimation(OpacityProperty, animationDouble);
        }
        #endregion

        #region BlurBackgroundDataTime
        internal void ChangeBlurImageInDataTime(bool State)
        {
            DoubleAnimation animation = DoubleAnimateObj.Clone();
            animation.Duration = TimeSpan.FromMilliseconds(1300d);
            animation.To = State ? 0.5d : 0d;
            VisualRectangleDateTimeBackground.BeginAnimation(OpacityProperty, animation);
        }
        #endregion
    }
}