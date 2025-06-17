#region Link
using IEL;
using IEL.CORE.Classes;
using IEL.CORE.Classes.Browser;
using IEL.CORE.Enums;
using OperPage_les.CORE;
using OperPage_les.CORE.Settings;
using OperPage_les.UI.Dialogs;
using OperPage_les.UI.Pages.ActionPanel;
using OperPage_les.UI.Pages.Browser;
using OperPage_les.UI.Pages.PanelButtonInformation.MainWindow;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfAnimatedGif;
#endregion

namespace OperPage_les.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region PanelAction
        #region Source
        /// <summary>
        /// Страница взаимодействия с вкладками браузера страниц
        /// </summary>
        private static readonly PageInlayPanelAction PageInlay = new();
        #endregion
        /// <summary>
        /// Настройки панели действий для браузера страниц
        /// </summary>
        private readonly PanelActionSettingVisual PanelActionSettingsInlay;

        /// <summary>
        /// Страница панели действий взаимодействия с вкладками браузера страниц
        /// </summary>
        private readonly PagePanelAction PanelActionPageInlay;
        #endregion

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

        //private MMDeviceEnumerator Device = new();

        /// <summary>
        /// Состояние воспроизведения приветственной анимации
        /// </summary>
        private bool HiAnimation = false;

        private int ActualIndexActivatePageDownToolButtons;

        /// <summary>
        /// Страницы информации нижней панели
        /// </summary>
        private readonly Page[] PagesButtonsInformation;

        public MainWindow()
        {
            InitializeComponent();
            Icon = App.LoadImage(Properties.Resources.IconMainApplication);
            ImageLogoApplication.Imaging = App.LoadImage(Properties.Resources.IconMainApplication);
            IELImageButtonHelp.Imaging = App.LoadImage(Properties.Resources.LightBulb);
            IELButtonSettings.Imaging = App.LoadImage(Properties.Resources.IconMainSettings);
            IELBrowserPageMain.IELButtonAddInlay.Imaging = App.LoadImage(Properties.Resources.Plus);
            IELImageButtonMenu.Imaging = App.LoadImage(Properties.Resources.Menu);
            ImageBehavior.SetAnimatedSource(ImageIndicator, new BitmapImage(new Uri(App.DirectoryImageLoading)));
            PanelActionPageInlay = new(PageInlay);
            PagesButtonsInformation =
            [
                new MainPageButtonInfo(), new Page2()
            ];
            #region Command
            //#if DEBUG
            //App.DataConsoleCommand.AddRange([
            //    #region anim
            //    new ConsoleCommand("anim", [new Parameter("Value", typeof(bool))],
            //    "Отключает или включает анимацию у окна ярлыков",
            //    (Command, param) =>
            //    {
            //        PageLabels? Page = IELBrowserPageMain.SearchPageType<PageLabels>();
            //        if (Page == null)
            //            return Task.FromResult(CommandStateResult.Failed(Command.Name,
            //                $"Страница \"{nameof(PageLabels)}\" в браузере не инициализирована!"));
            //        if ((bool)param[0]) Page.AnimationLoadingStart();
            //        else Page.AnimationLoadingStop();
            //        return Task.FromResult(CommandStateResult.Completed(Command.Name));
            //    }),
            //    #endregion
            //]);
            //#endif
            #endregion

            #region PanelAction

            #region PageInlay
            PageInlay.IELButtonPageOpenInlay.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (PageInlay.ActivateManipulateInlay != null)
                    IELBrowserPageMain.ActivateInlayInBrowserPage(PageInlay.ActivateManipulateInlay.PageElement);
            };
            PageInlay.IELButtonPageDeleteInlay.OnActivateMouseLeft += (sender, e, Key) =>
            {
                IELActionPanelMain.ClosePanelAction();
                if (PageInlay.ActivateManipulateInlay != null)
                    IELBrowserPageMain.DeleteInlayPage(PageInlay.ActivateManipulateInlay);
            };
            #endregion
            PanelActionPageInlay.IsKeyboardModeChanged += (Source, NewValue) =>
            {
                PageInlay.IELButtonPageOpenInlay.CharKeyboardActivate = NewValue;
                PageInlay.IELButtonPageDeleteInlay.CharKeyboardActivate = NewValue;
            };
            PanelActionSettingsInlay = new(IELBrowserPageMain, PanelActionPageInlay, new(200d, 240d));
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
            ActualIndexActivatePageDownToolButtons = -1;
            IELPageControllerButtons.LeftAnimateSwitch = new(-5, 0, 0, 0);
            IELPageControllerButtons.RightAnimateSwitch = new(5, 0, 0, 0);

            VisualRectangleDateTimeBackground.Opacity = 0d;
            IELMessageMain.Opacity = 0d;
            IELActionPanelMain.Opacity = 0d;
            byte[,] ColorBytes = new byte[4, 4]
            {
                { 255, 55, 101, 144, },
                { 255, 103, 120, 121, },
                { 255, 45, 113, 95, },
                { 255, 41, 91, 122, }
            };
            IELBrowserPageMain.QDataDefaultInlayBackground = new(new byte[4, 4]
            {
                { 255, 141, 195, 223, },
                { 255, 199, 223, 224, },
                { 255, 130, 224, 199, },
                { 255, 230, 188, 224, }
            });
            IELBrowserPageMain.QDataDefaultInlayBorderBrush = new(ColorBytes);
            IELBrowserPageMain.QDataDefaultInlayForeground = new(ColorBytes);

            Canvas.SetZIndex(IELMessageMain, -2);
            Canvas.SetZIndex(IELActionPanelMain, -2);
            #endregion

            SizeChanged += (sender, e) => IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);

            #region Settings
            UpdateImageMenu(App.CurrentApp.SettingMainApplication.PathMenuImage);
            ChangeBlurImageInDataTime(App.CurrentApp.SettingMainApplication.BlurBackgroundDataTime);
            #endregion
            //Closing += (sender, e) => App.Current.Shutdown(0);

            #region UpToolButtons
            #region IELImageButtonHelp
            IELImageButtonHelp.OnActivateMouseLeft += (sender, e, Key) => App.CurrentApp.UsingDiscriptionCommand();
            IELImageButtonHelp.IELSettingObject.MouseHover += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELImageButtonHelp,
                    "Быстрое открытие описания команд",
                    OrientationBorderPosition.LeftDown);
            };
            IELImageButtonHelp.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region IELButtonSettings
            IELButtonSettings.IELSettingObject.MouseHover += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELButtonSettings,
                    "Настройки программы",
                    OrientationBorderPosition.LeftDown);
            };
            IELButtonSettings.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            IELButtonSettings.OnActivateMouseLeft += (sender, e, Key) =>
            {
                new WindowSetting().ShowDialog();
            };
            #endregion
            #endregion

            IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                BrowserPage? Page = IELBrowserPageMain.ActualInlay?.PageElement;
                if (Page == null) return;
                switch(Page.GetType().Name)
                {
                    case "PageConsole":
                        ((PageConsole)Page.PageContent).TextBoxCommandInput.Focus();
                        break;
                    default: return;
                }
            };
            #region Down Tool Buttons Information
            ActualIndexActivatePageDownToolButtons = 0;
            IELPageControllerButtons.NextPage(PagesButtonsInformation[0], false);
            IELImageButtonNextButtons.OnActivateMouseLeft += (sender, e, Key) => NextPageDownToolButtons();
            IELImageButtonBackButtons.OnActivateMouseLeft += (sender, e, Key) => NextPageDownToolButtons(false);

            IELImageButtonNextButtons.MouseEnter += (sender, e) => IELPageControllerButtons.MoveActualPage(new(-3, 0, 0, 0), 400u);
            IELImageButtonNextButtons.MouseLeave += (sender, e) => IELPageControllerButtons.MoveActualPage(new(0), 400u);
            IELImageButtonBackButtons.MouseEnter += (sender, e) => IELPageControllerButtons.MoveActualPage(new(3, 0, 0, 0), 400u);
            IELImageButtonBackButtons.MouseLeave += (sender, e) => IELPageControllerButtons.MoveActualPage(new(0), 400u);

            #region IELBrowserPage
            IELBrowserPageMain.EventCloseBrowser += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            IELBrowserPageMain.EventChangeActiveInlay += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            IELBrowserPageMain.EventCloseInlay += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            IELBrowserPageMain.IELButtonAddInlay.OnActivateMouseLeft += (sender, e, Key) =>
            {
                IELActionPanelMain.ClosePanelAction();
                IELBrowserPageMain.AddInlayPage(new WindowBrowserPagesManager().AddNewPageInBrowser(IELBrowserPageMain));
            };
            IELBrowserPageMain.EventActiveActionInInlay += (Inlay) =>
            {
                PageInlay.ActivateManipulateInlay = Inlay;
                IELActionPanelMain.UsingPanelAction(PanelActionSettingsInlay);
                //DialogManagerPage.ShowDialog();
            };
            IELBrowserPageMain.EventOnDescriptionInlay += (Element, Text) =>
            {
                IELMessageMain.UsingBorderInformation(Element, Text, OrientationBorderPosition.Auto);
            };
            IELBrowserPageMain.EventOffDescriptionInlay += IELMessageMain.CloseBorderInformation;

#if DEBUG
            IELBrowserPageMain.EventChangeActiveInlay += () =>
            {
                //AudioPlayerControl.PlayMP3(Properties.Resources.B5);
            };
#endif
            #endregion

            
            #endregion

            //ImageLogoApplication.MouseEnter += (sender, e) =>
            //{
            //    App.AnimateDoubleEffect(ImageLogoApplication, OpacityProperty, 0.6d);
            //};

            //ImageLogoApplication.MouseLeave += (sender, e) =>
            //{
            //    App.AnimateDoubleEffect(ImageLogoApplication, OpacityProperty, 1d);
            //};

            //ImageLogoApplication.MouseDown += (sender, e) =>
            //{
            //    App.AnimateDoubleEffect(ImageLogoApplication, OpacityProperty, 0.4d);
            //};

            ImageLogoApplication.OnActivateMouseLeft += (sender, e, Key) =>
            {
                LicenseWindow License = new();
                License.ShowDialog();
            };
            KeyDown += (sender, e) =>
            {
                if (e.Key == Key.CapsLock) App.Flags.FlagRegisterState.Value = Console.CapsLock;
            };
            Activated += (sender, e) =>
            {
                if (!HiAnimation)
                {
                    HiAnimation = true;

                    #region Anim Start
                    #region 1
                    App.AnimateThicknessEffect(ImageLogoApplication, MarginProperty, new(8), BorderImageInformation.Margin, TimeSpan.FromMilliseconds(1400d));

                    TimeDataColumnDefinition.MaxWidth = 0d;

                    DoubleAnimation animation = App.GetDoubleAnimate();
                    animation.Duration = TimeSpan.FromMilliseconds(1200d);
                    animation.To = 124d;
                    Storyboard storyboard = new();
                    storyboard.Children.Add(animation);
                    Storyboard.SetTarget(animation, TimeDataColumnDefinition);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(ColumnDefinition.MaxWidth)"));
                    storyboard.Begin();
                    animation.Duration = TimeSpan.FromMilliseconds(250d);

                    App.AnimateThicknessEffect(BorderDateTime, MarginProperty, new(8), BorderDateTime.Margin, TimeSpan.FromMilliseconds(1400d));

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

            UpdateBackgroundDataThis.Start();
            UpdateBackgroundDataRunTime.Start();
        }

        //
        private void NextPageDownToolButtons(bool UpIndex = true)
        {
            if (UpIndex)
            {
                if (ActualIndexActivatePageDownToolButtons == PagesButtonsInformation.Length - 1)
                    ActualIndexActivatePageDownToolButtons = 0;
                else ActualIndexActivatePageDownToolButtons++;
            }
            else
            {
                if (ActualIndexActivatePageDownToolButtons == 0)
                    ActualIndexActivatePageDownToolButtons = PagesButtonsInformation.Length - 1;
                else ActualIndexActivatePageDownToolButtons--;
            }
            IELPageControllerButtons.NextPage(PagesButtonsInformation[ActualIndexActivatePageDownToolButtons], UpIndex);
        }

        /// <summary>
        /// Функция обновления визуальной информации в данном окне 100
        /// </summary>
        private void BackgroundUpdateVisualData()
        {
            TextBlockTime.Text = App.RealTime.ToShortTimeString();
            TextBlockData.Text = App.RealTime.ToShortDateString();
        }

        /// <summary>
        /// Функция обновления визуальной информации в данном окне 60
        /// </summary>
        private void BackgroundUpdateVisualDataRunTime()
        {
            
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
        internal void UpdateImageMenu(string Path)
        {
            ActivateLoadingIndicator();
            if (Path.Length > 0)
            {
                BitmapImage BitmapImageMenu = new(new Uri(Path));
                if (File.Exists(Path))
                {
                    ComplitedInstallImageMenu(BitmapImageMenu);
                }
                else
                {
                    BitmapImageMenu.DownloadCompleted += (sender, e) =>
                    {
                        ComplitedInstallImageMenu(BitmapImageMenu);
                    };
                    BitmapImageMenu.DownloadFailed += (sender, e) => FailedInstallImageMenu();
                    BitmapImageMenu.DecodeFailed += (sender, e) => FailedInstallImageMenu();
                }
            }
            else
            {
                App.AnimateDoubleEffect(ImageMenu, OpacityProperty, 0d, TimeSpan.FromMilliseconds(2300d));
            }
            DiactivateLoadingIndicator();
        }

        /// <summary>
        /// Успешная установка картинки фона
        /// </summary>
        /// <param name="bitmap">Карта изображения</param>
        private void ComplitedInstallImageMenu(BitmapImage bitmap)
        {
            ImageMenu.Source = bitmap;

            DiactivateLoadingIndicator();
            App.AnimateDoubleEffect(BlurEffectImageMenu, BlurEffect.RadiusProperty, 10d, 0d, TimeSpan.FromMilliseconds(2300d));
            App.AnimateThicknessEffect(ImageMenu, MarginProperty, new(-4), new(0), TimeSpan.FromMilliseconds(2300d));
            App.AnimateDoubleEffect(ImageMenu, OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(2300d));
        }

        /// <summary>
        /// Неудачное завершение установки картинки фона
        /// </summary>
        private void FailedInstallImageMenu()
        {
            DiactivateLoadingIndicator();
            System.Windows.MessageBox.Show("Не удалось загрузить фоновое изображение...", "Информация", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
        }

        internal void ChangeVisibilityMillisecondInternet(bool Value)
        {
            ((MainPageButtonInfo)PagesButtonsInformation[0]).VisibilityInternetMillisecond(Value);
        }
        #endregion

        #region Indicator
        /// <summary>
        /// Включить индикатор загрузки
        /// </summary>
        internal void ActivateLoadingIndicator()
        {
            ImageBehavior.SetAnimatedSource(ImageIndicator, new BitmapImage(new Uri(App.DirectoryImageLoading)));
            App.AnimateDoubleEffect(ImageIndicator, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
        }

        /// <summary>
        /// Выключить индикатор загрузки
        /// </summary>
        internal void DiactivateLoadingIndicator()
        {
            App.AnimateDoubleEffect(ImageIndicator, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1700d));
        }
        #endregion


        #region BlurBackgroundDataTime
        internal void ChangeBlurImageInDataTime(bool State)
        {
            App.AnimateDoubleEffect(VisualRectangleDateTimeBackground, OpacityProperty, State ? 0.5d : 0d, TimeSpan.FromMilliseconds(1300d));
        }
        #endregion
    }
}