#region Link
using IEL.CORE.Classes;
using IEL.CORE.Classes.Browser;
using IEL.CORE.Enums;
using IEL.GUI;
using NAudio.Wave;
using ApplicationOperPageLes.CORE;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.Pages.ActionPanel;
using ApplicationOperPageLes.UI.Pages.Browser;
using ApplicationOperPageLes.UI.Pages.PanelButtonInformation.MainWindow;
using ApplicationOperPageLes.UI.UserElementControl;
using ApplicationOperPageLes.UI.Windows.Dialogs;
using ApplicationOperPageLes.Windows;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WmColor = System.Windows.Media.Color;
using OPRES = ApplicationOperPageLes.Properties.Resources;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using ApplicationOperPageLes.UI.UserElementControl.Interfaces;
using ApplicationOperPageLes.CORE.Enums;
#endregion

namespace ApplicationOperPageLes.UI.Windows
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
        private readonly PageSettingVisual PanelActionSettingsInlay;
        #endregion

        /// <summary>
        /// Токен управляемой асинхронной операции обновления информации в главном окне
        /// </summary>
        private readonly CancellationToken TokenUpdateBackgroundData;

        //private MMDeviceEnumerator Device = new();

        /// <summary>
        /// Состояние воспроизведения приветственной анимации
        /// </summary>
        private bool HiAnimation = false;

        private int ActualIndexActivatePageDownToolButtons;

        /// <summary>
        /// Страницы информации нижней панели
        /// </summary>
        private Page[] PagesButtonsInformation = [];

        /// <summary>
        /// Страница управления загрузочными процессами
        /// </summary>
        private PageNotificationManager PageControllerLoadingApplication;

        /// <summary>
        /// Настройка для панели действий страницы управления загрузочными элементами
        /// </summary>
        private PageSettingVisual SettingVisualPageLoadingController;

        /// <summary>
        /// Состояние загрузки какого-либо процесса
        /// </summary>
        internal bool IsLoadingProcess { get; private set; }

        /// <summary>
        /// Событие закрытия главного окна перед его удалением
        /// </summary>
        public new event FormClosingEventHandler? Closing;

        /// <summary>
        /// Событие закрытия главного окна после его удаления
        /// </summary>
        public new event FormClosedEventHandler? Closed;

        /// <summary>
        /// Состояние перезагрузки
        /// </summary>
        internal bool IsReboot = false;

        /// <summary>
        /// Состояние закрытия окна
        /// </summary>
        private bool IsClosing = false;

        /// <summary>
        /// Страница выбора новой страницы браузера
        /// </summary>
        private PageManagerBrowser ManagerBrowserNewPage;

#if DEBUG
        private readonly TextBlock DEV_Time;
        private readonly TextBlock DEV_Data;
        private readonly TextBlock DEV_IsLoadingProcess;
#endif

        /// <summary>
        /// Объект манипуляции вращением фона главного окна
        /// </summary>
        private static RotateTransform RotateMainWindowBackground = new()
        {
            Angle = 0d,
            CenterX = 0.5d,
            CenterY = 0.5d,
        };

        public MainWindow()
        {
            InitializeComponent();

            #region DEV
#if DEBUG
            DEV_Time = App.CurrentApp.Is_WindowDeveloper.BlockInlays[0].AddNewTextElement();
            DEV_Data = App.CurrentApp.Is_WindowDeveloper.BlockInlays[0].AddNewTextElement();
            DEV_IsLoadingProcess = App.CurrentApp.Is_WindowDeveloper.BlockInlays[0].AddNewTextElement();
#endif
            #endregion

            #region SetParameteres
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            ImageLogoApplication.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            IELImageButtonClose.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
            IELImageButtonHelp.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.LightBulb));
            IELButtonSettings.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainSettings));
            IELImageButtonMenu.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Menu));

            TokenUpdateBackgroundData = new(false);
            ActualIndexActivatePageDownToolButtons = -1;
            IELPageControllerButtons.LeftAnimateSwitch = new(-5, 0, 0, 0);
            IELPageControllerButtons.RightAnimateSwitch = new(5, 0, 0, 0);

            BorderNotificationIndicator.Opacity = 0d;
            IndicatorLoading.Opacity = 0d;
            IndicatorLoading.Source = new Uri(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingDefault)));
            IndicatorLoading.MediaEnded += (sender, e) =>
            {
                IndicatorLoading.Position = TimeSpan.FromMilliseconds(1);
            };

            IELMessageMain.Opacity = 0d;
            IELActionPanelMain.Opacity = 0d;
            ImageMenu.Opacity = 0d;
            IELActionPanelMain.IsKeyboardModeExit = App.CurrentApp.SettingMainApplication.ExitKeyboardModeInClosePanelAction;
            IELActionPanelMain.KeyActivateKeyboardMode = App.CurrentApp.SettingMainApplication.KEY_KeyboardModePanelAction;
            IELActionPanelMain.KeyKeyboardModeActivateRightClick = App.CurrentApp.SettingMainApplication.KEY_PanelActionRightClick;
            IELActionPanelMain.KeyCloseElement = App.CurrentApp.SettingMainApplication.KEY_PanelActionClose;
            IELActionPanelMain.EventMovePanelAction += (sender, e) =>
            {
                StructDirectoryResources.Play(App.CurrentApp.SoundChannelWaveOut, nameof(OPRES.AudioMove));
            };
            byte[][] ColorBytes =
            [
                [255, 55, 101, 144],
                [255, 103, 120, 121],
                [255, 45, 113, 95],
                [255, 41, 91, 122]
            ];
            IELBrowserPageMain.QDataDefaultInlayBackground = new(
            [
                [255, 141, 195, 223],
                [255, 199, 223, 224],
                [255, 130, 224, 199],
                [255, 230, 188, 224]
            ]);
            IELBrowserPageMain.QDataDefaultInlayBorderBrush = new(ColorBytes);
            IELBrowserPageMain.QDataDefaultInlayForeground = new(ColorBytes);
            IELBrowserPageMain.SetSourceImageButtonAddInlay(StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Plus)));

            #region Palette
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(ImageLogoApplication);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Red].ConnectPalleteFromIELElement(IELImageButtonClose);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELImageButtonHelp);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonSettings);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELBrowserPageMain.GetButtonAddInlay());
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Cocoa].ConnectPalleteFromIELElement(IELActionPanelMain);

            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELImageButtonBackButtons);
            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELImageButtonNextButtons);

            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.Olive].ConnectPalleteFromIELElement(IELImageButtonHelp);

            App.CurrentApp.SettingPaletteApplication.SourcePalette[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELImageButtonMenu);
            #endregion

            LinearGradientBrush LinearGradientMainWindowBackground = new()
            {
                GradientStops = new(
                    [
                    new GradientStop(WmColor.FromRgb(23, 43, 32), 0d),
                    new GradientStop(WmColor.FromRgb(43, 56, 49), 0.168d),
                    new GradientStop(WmColor.FromRgb(48, 58, 66), 0.257d),
                    new GradientStop(WmColor.FromRgb(60, 70, 82), 0.432d),
                    new GradientStop(WmColor.FromRgb(86, 116, 140), 0.582d),
                    new GradientStop(WmColor.FromRgb(115, 109, 94), 0.764d),
                    new GradientStop(WmColor.FromRgb(72, 64, 41), 1d),
                    ]),
                RelativeTransform = RotateMainWindowBackground,
            };
            BorderWindowMain.Background = LinearGradientMainWindowBackground;

            Canvas.SetZIndex(IELMessageMain, -2);
            Canvas.SetZIndex(IELActionPanelMain, -2);
            #endregion

            #region IELPanelAction
            IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                BrowserPage? Page = IELBrowserPageMain.ActualInlay?.PageElement;
                if (Page == null) return;
                switch (Page.GetType().Name)
                {
                    case "PageConsole":
                        ((PageConsole)Page.PageContent).TextBoxCommandInput.Focus();
                        break;
                    default: return;
                }
            };

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
            PanelActionSettingsInlay = new(IELBrowserPageMain, PageInlay, new(200d, 240d));
            #endregion

            PageControllerLoadingApplication = new();
            SettingVisualPageLoadingController = new(GridMain, PageControllerLoadingApplication, new(210, 255));
            PageControllerLoadingApplication.CreatedNewOneOnlyViewerImage += (sender, e) =>
            {
                App.DoubleAnimationType.AnimateEffect(BorderNotificationIndicator, OpacityProperty, 1d, TimeSpan.FromMilliseconds(100d));
            };
            PageControllerLoadingApplication.ClearedAllViewersImage += (sender, e) =>
            {
                App.DoubleAnimationType.AnimateEffect(BorderNotificationIndicator, OpacityProperty, 0d, TimeSpan.FromMilliseconds(100d));
            };

            #endregion

            #region Settings
            UpdateImageMenu(App.CurrentApp.SettingMainApplication.PathMenuImage);
            #endregion

            #region UpToolButtons
            #region IELImageButtonClose
            IELImageButtonClose.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Close();
            };
            #endregion

            #region IELImageButtonHelp
            IELImageButtonHelp.OnActivateMouseLeft += (sender, e, Key) =>
            {
                WindowDiscriptionCommands j = new();
                App.CurrentApp.OpenedWindowsInApplication.Add(j);
                j.Show();
            };
            IELImageButtonHelp.MouseHover += (sender, e) =>
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
            IELButtonSettings.MouseHover += (sender, e) =>
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
                new DialogSetting().ShowDialog();
            };
            #endregion

            #endregion

            #region DownToolButtons
            ActualIndexActivatePageDownToolButtons = 0;
            IELImageButtonNextButtons.OnActivateMouseLeft += (sender, e, Key) => NextPageDownToolButtons();
            IELImageButtonBackButtons.OnActivateMouseLeft += (sender, e, Key) => NextPageDownToolButtons(false);

            IELImageButtonNextButtons.MouseEnter += (sender, e) => IELPageControllerButtons.MoveActualPage(new(-3, 0, 0, 0), 400u);
            IELImageButtonNextButtons.MouseLeave += (sender, e) => IELPageControllerButtons.MoveActualPage(new(0), 400u);
            IELImageButtonBackButtons.MouseEnter += (sender, e) => IELPageControllerButtons.MoveActualPage(new(3, 0, 0, 0), 400u);
            IELImageButtonBackButtons.MouseLeave += (sender, e) => IELPageControllerButtons.MoveActualPage(new(0), 400u);

            BorderIndicator.MouseRightButtonUp += (sender, e) =>
            {
                IELActionPanelMain.UsingPanelAction(SettingVisualPageLoadingController, OrientationPanelActionPosition.LeftDown);
            };
            #endregion

            #region IELBrowserPage
            ManagerBrowserNewPage = new();
            ManagerBrowserNewPage.BrowserPageSelect += (sender, e) =>
            {
                App.DoubleAnimationType.AnimateEffect(FrameNewInlayBrowser, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
                App.DoubleAnimationType.AnimateEffect(IELBrowserPageMain, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                if (App.CurrentApp.SettingMainApplication.PathMenuImage.Value.Length > 0)
                    App.DoubleAnimationType.AnimateEffect(ImageMenu, ImageBrush.OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                App.DoubleAnimationType.AnimateEffect(IELBrowserPageBlurEffect, BlurEffect.RadiusProperty, 0d, TimeSpan.FromMilliseconds(500d));
                Canvas.SetZIndex(FrameNewInlayBrowser, -1);
                FrameNewInlayBrowser.IsHitTestVisible = false;
                FrameNewInlayBrowser.IsEnabled = false;
                IELBrowserPageMain.IsEnabled = true;
                if (e == null) return;
                IELInlay? SourceInlay = IELBrowserPageMain.AddInlayPage(e);
                SourceInlay?.SetImageButtonCloseInlay(StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross)));
            };
            FrameNewInlayBrowser.IsHitTestVisible = false;
            FrameNewInlayBrowser.IsEnabled = false;
            FrameNewInlayBrowser.Content = ManagerBrowserNewPage;
            FrameNewInlayBrowser.Opacity = 0d;
            Canvas.SetZIndex(FrameNewInlayBrowser, -1);

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
            IELBrowserPageMain.EventAddInlay += () =>
            {
                FrameNewInlayBrowser.IsEnabled = true;
                FrameNewInlayBrowser.IsHitTestVisible = true;
                IELBrowserPageMain.IsEnabled = false;
                TimeSpan t = TimeSpan.FromMilliseconds(500d);
                IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                App.DoubleAnimationType.AnimateEffect(FrameNewInlayBrowser, OpacityProperty, 1d, t);
                App.DoubleAnimationType.AnimateEffect(IELBrowserPageMain, OpacityProperty, 0.9d, t);
                if (App.CurrentApp.SettingMainApplication.PathMenuImage.Value.Length > 0)
                    App.DoubleAnimationType.AnimateEffect(ImageMenu, ImageBrush.OpacityProperty, 0.3d, t);
                App.DoubleAnimationType.AnimateEffect(IELBrowserPageBlurEffect, BlurEffect.RadiusProperty, 20d, t);
                Canvas.SetZIndex(FrameNewInlayBrowser, 1);
                ManagerBrowserNewPage.Focus();
            };
            IELBrowserPageMain.EventActiveActionInInlay += (Inlay) =>
            {
                PageInlay.ActivateManipulateInlay = Inlay;
                IELActionPanelMain.UsingPanelAction(PanelActionSettingsInlay, OrientationPanelActionPosition.LeftUp);
            };
            IELBrowserPageMain.EventOnDescriptionInlay += (Element, Text) =>
            {
                IELMessageMain.UsingBorderInformation(Element, Text, OrientationBorderPosition.Auto);
            };
            IELBrowserPageMain.EventOffDescriptionInlay += IELMessageMain.CloseBorderInformation;
            #endregion

            ImageLogoApplication.OnActivateMouseLeft += (sender, e, Key) =>
            {
                DialogLicenseWindow License = new();
                License.Show();
            };

            #region EventsWindow
            BorderWindowMain.MouseLeftButtonDown += (sender, e) =>
            {
                if (IELActionPanelMain.PanelActionActivate)
                    IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                TimeSpan t = TimeSpan.FromMilliseconds(900d);
                App.ThicknessAnimationType.AnimateEffect(BorderWindowMainContent, MarginProperty, new(45), t);
                App.DoubleAnimationType.AnimateEffect(BorderWindowMainContent, OpacityProperty, 0.4d, t);
                DragMove();
                App.ThicknessAnimationType.AnimateEffect(BorderWindowMainContent, MarginProperty, new(20), t);
                App.DoubleAnimationType.AnimateEffect(BorderWindowMainContent, OpacityProperty, 1d, t);
            };
            SizeChanged += (sender, e) => IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            Activated += (sender, e) =>
            {
                if (!HiAnimation)
                {
                    HiAnimation = true;
                    PagesButtonsInformation =
                    [
                        new MainPageButtonInfo(), new Page2()
                    ];
                    IELPageControllerButtons.NextPage(PagesButtonsInformation[0], false);

                    #region Anim Start
                    #region 1
                    TimeSpan t1400 = TimeSpan.FromMilliseconds(1400d);
                    TimeSpan t2000 = TimeSpan.FromMilliseconds(2000d);
                    App.ThicknessAnimationType.AnimateEffect(ImageLogoApplication, MarginProperty, new(8), BorderImageInformation.Margin, t1400);

                    App.ThicknessAnimationType.AnimateEffect(BorderDateTime, MarginProperty, new(8), BorderDateTime.Margin, t1400);

                    App.ThicknessAnimationType.AnimateEffect(BorderWindowMain, MarginProperty, new(20), new(0), t2000);

                    App.DoubleAnimationType.AnimateEffect(this, OpacityProperty, 0d, 1d, t1400);

                    App.DoubleAnimationType.AnimateEffect(RotateMainWindowBackground, RotateTransform.AngleProperty, 0d, 360d, TimeSpan.FromMilliseconds(3200d));

                    #endregion
                    #endregion
                }
                if (App.ActiveDialog != null && HiAnimation)
                {
                    App.ActiveDialog.Activate();
                }
            };
            base.Closing += (sender, e) =>
            {
                if (!IsReboot && !IsClosing) Close();
            };
            #endregion
        }

        #region ManipulateWindow
        /// <summary>
        /// Закрыть главное окно приложения без перезагрузки
        /// </summary>
        public new void Close()
        {
            IsClosing = true;
            Hide();
#if DEBUG
            App.CurrentApp.Is_WindowDeveloper.Close();
#endif
            TokenUpdateBackgroundData.ThrowIfCancellationRequested();
            Closing?.Invoke(this, new(CloseReason.UserClosing, false));
            bool WindowSaveClose = false;
            DialogSaveWait windowSave = new();
            windowSave.Closed += (sender, e) =>
            {
                WindowSaveClose = true;
                Closed?.Invoke(windowSave, new(CloseReason.WindowsShutDown));
                base.Close();
            };
            windowSave.OpenOnToComplete();
            windowSave.Focus();

            Thread thread = new(async () =>
            {
                Dispatcher.Invoke(() => windowSave.SetVisualTextSaving("Закрываются все окна приложения", 0d));
                Dispatcher.Invoke(() =>
                {
                    int count = App.CurrentApp.OpenedWindowsInApplication.Count;
                    for (int i = 0; i < count; i++)
                    {
                        App.CurrentApp.OpenedWindowsInApplication[0].Close();
                        App.CurrentApp.OpenedWindowsInApplication.RemoveAt(0);
                        Thread.Sleep(10);
                    }
                    //App.CurrentApp.OpenedWindowsInApplication.Clear();
                });
                Thread.Sleep(500);

                Dispatcher.Invoke(() => windowSave.SetVisualTextSaving("Обновляются ваши настройки", 30d));
                App.CurrentApp.UpdateSettingApplication();
                Thread.Sleep(300);

                Dispatcher.Invoke(() => windowSave.SetVisualTextSaving("Сохраняются все ярлыки", 60d));
                App.CurrentApp.UpdateFileDataLabel();
                Thread.Sleep(600);
                Dispatcher.Invoke(() => windowSave.SetVisualTextSaving("Сохраняются все теги", 87d));
                App.CurrentApp.UpdateFileDataLabelTag();
                Thread.Sleep(700);

                Dispatcher.Invoke(() => windowSave.SetVisualTextSaving("Ожидайте завершения...", 100d));
                await windowSave.Complete();
            });
            thread.Start();

            Task.Run(() =>
            {
                while (!WindowSaveClose) ;
                thread.Join();
            });
        }

        /// <summary>
        /// Собственная функция отображения главного окна
        /// </summary>
        public new void Show()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await BackgroundUpdateVisualData();
                    Thread.Sleep(1000);
                }
            }, TokenUpdateBackgroundData);
            base.Show();
        }
        #endregion

        #region DownToolButtons
        //
        private void NextPageDownToolButtons(bool UpIndex = true)
        {
            StructDirectoryResources.Play(App.CurrentApp.SoundChannelWaveOut, nameof(OPRES.AudioMove));
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
        #endregion

        #region Loading Manipulate
        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки
        /// </summary>
        /// <typeparam name="T">Тип ожидаемого элемента</typeparam>
        /// <param name="NameProcess">Название загрузочного процесса</param>
        /// <param name="Method">Асинхронный процесс получения значения</param>
        /// <param name="IsCanceledManipulate">Можно ли отменить операцию</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        internal async Task<T> ExecuteVisualizateLoadingProcess<T>(string NameProcess, Task<T> Method, bool IsCanceledManipulate = false)
        {
            OPLMediaViewer ViewLoading = GenerateVisualizateMediaLoadingProcess(NameProcess);
            ViewLoading.IsCanceledManipulate = IsCanceledManipulate;
            ViewLoading.Dispatcher.Invoke(StartVisualizateLoadingProcess, ViewLoading);
            CancellationToken token = new(false);
            ViewLoading.OnActivateMouseRight += (sender, e, Key) =>
            {
                token.ThrowIfCancellationRequested();
                ViewLoading.Dispatcher.Invoke(CompleteVisualizateLoadingProcess, ViewLoading);
            };

            await Method.WaitAsync(token);

            if (Method.IsCanceled) throw new OperationCanceledException();
            ViewLoading.Dispatcher.Invoke(CompleteVisualizateLoadingProcess, ViewLoading);

            return await Method;
        }

        /// <summary>
        /// Создать объект визуализирующий изображение
        /// </summary>
        /// <param name="NameProcess">Название загрузочного процесса</param>
        /// <returns>Объект визуализации загрузочного процесса</returns>
        public OPLImageViewer GenerateVisualizateImage(string Name)
        {
            OPLImageViewer Result = PageControllerLoadingApplication.SetViewImageElement();
            Result.Text = Name;
            Result.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Result.VisualClose();
                PageControllerLoadingApplication.DeleteViewMediaElement(Result);
            };
            Result.OnActivateMouseRight += Result.OnActivateMouseLeft;
            return Result;
        }

        /// <summary>
        /// Создать объект визуализирующий медиа
        /// </summary>
        /// <param name="Name">Название загрузочного процесса</param>
        /// <returns>Объект визуализации загрузочного процесса</returns>
        public OPLMediaViewer GenerateVisualizateMedia(string Name)
        {
            OPLMediaViewer Result = PageControllerLoadingApplication.SetViewMediaElement();
            Result.Text = Name;
            Result.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Result.VisualClose();
                PageControllerLoadingApplication.DeleteViewMediaElement(Result);
            };
            Result.OnActivateMouseRight += Result.OnActivateMouseLeft;
            return Result;
        }

        /// <summary>
        /// Создать объект визуализирующий медиа (Спецификация на загрузочный процесс)
        /// </summary>
        /// <param name="Name">Название загрузочного процесса</param>
        /// <returns>Объект визуализации загрузочного процесса</returns>
        internal OPLMediaViewer GenerateVisualizateMediaLoadingProcess(string Name)
        {
            OPLMediaViewer Result = GenerateVisualizateMedia(Name);
            App.CurrentApp.DataViewerLoadingProcess.Add(Result);
            return Result;
        }

        /// <summary>
        /// Начало визуализации загрузки
        /// </summary>
        /// <param name="ViewLoading">Элемент визуализации загрузочного процесса</param>
        internal void StartVisualizateLoadingProcess(OPLMediaViewer ViewLoading)
        {
            if (!IsLoadingProcess)
            {
                IsLoadingProcess = true;
                DoubleAnimation animation = App.DoubleAnimationType.SourceAnimation.Clone();
                animation.From = 0d;
                animation.To = 3600d;
                animation.RepeatBehavior = RepeatBehavior.Forever;
                animation.EasingFunction = null;
                animation.Duration = TimeSpan.FromMilliseconds(30000d);
                RotateMainWindowBackground.BeginAnimation(RotateTransform.AngleProperty, animation);
#if DEBUG
                DEV_IsLoadingProcess.Text = $"ILoad_P: {IsLoadingProcess}";
#endif
                App.DoubleAnimationType.AnimateEffect(IndicatorLoading, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            }
            ViewLoading.VisualOpen();
        } 

        /// <summary>
        /// Завершение визуализации загрузки
        /// </summary>
        /// <param name="ViewLoading">Элемент визуализации загрузочного процесса</param>
        internal void CompleteVisualizateLoadingProcess(OPLMediaViewer ViewLoading)
        {
            ViewLoading.VisualClose();
            PageControllerLoadingApplication.DeleteViewMediaElement(ViewLoading);
            App.CurrentApp.DataViewerLoadingProcess.Remove(ViewLoading);
            if (App.CurrentApp.DataViewerLoadingProcess.Count == 0 && IsLoadingProcess)
            {
                IsLoadingProcess = false;
                DoubleAnimation animation = App.DoubleAnimationType.SourceAnimation.Clone();
                animation.From = RotateMainWindowBackground.Angle % 360;
                animation.To = 360d;
                animation.Duration = TimeSpan.FromMilliseconds(3200d);
                RotateMainWindowBackground.BeginAnimation(RotateTransform.AngleProperty, animation);
#if DEBUG
                DEV_IsLoadingProcess.Text = $"ILoad_P: {IsLoadingProcess}";
#endif
                App.DoubleAnimationType.AnimateEffect(IndicatorLoading, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1700d));
            }
        }
        #endregion

        /// <summary>
        /// Функция обновления визуальной информации в данном окне
        /// </summary>
        private async Task BackgroundUpdateVisualData()
        {
            await Dispatcher.BeginInvoke(() =>
            {
                TextBlockTime.Text = App.RealTime.ToShortTimeString();
                TextBlockData.Text = App.RealTime.ToShortDateString();
#if DEBUG
                DEV_Time.Text = $"T: {TextBlockTime.Text}";
                DEV_Data.Text = $"D: {TextBlockData.Text}";
                DEV_IsLoadingProcess.Text = $"ILoad_P: {IsLoadingProcess}";
#endif
            });
        }

        #region ImageMenu
        /// <summary>
        /// Обновить фотовое изображение
        /// </summary>
        internal void UpdateImageMenu(string Path)
        {
            if (Path.Length > 0)
            {
                try
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
                        BitmapImageMenu.DownloadFailed += (sender, e) => 
                            System.Windows.MessageBox.Show("Не удалось загрузить фоновое изображение...", "Информация",
                                MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                    }
                }
                catch (FileNotFoundException ex)
                {
                    OPLImageViewer Element = GenerateVisualizateImage($"Файл картинки фонового изображения не был найден...");
                    Element.MouseHover += (sender, e) =>
                    {
                        IELMessageMain.UsingBorderInformation(Element, ex.Message, OrientationBorderPosition.LeftUp);
                    };
                    Element.MouseLeave += (sender, e) =>
                    {
                        IELMessageMain.CloseBorderInformation();
                    };
                }
            }
            else
            {
                App.DoubleAnimationType.AnimateEffect(ImageMenu, ImageBrush.OpacityProperty, 0d, TimeSpan.FromMilliseconds(2300d));
            }
        }

        /// <summary>
        /// Успешная установка картинки фона
        /// </summary>
        /// <param name="bitmap">Карта изображения</param>
        private void ComplitedInstallImageMenu(BitmapImage bitmap)
        {
            ImageMenu.ImageSource = bitmap;

            App.RectAnimationType.AnimateEffect(ImageMenu, ImageBrush.ViewboxProperty, new(0.025, 0.025, 0.95, 0.95), new(0, 0, 1, 1), TimeSpan.FromMilliseconds(2300d));
            App.ThicknessAnimationType.AnimateEffect(ImageMenu, MarginProperty, new(-4), new(0), TimeSpan.FromMilliseconds(2300d));
            App.DoubleAnimationType.AnimateEffect(ImageMenu, ImageBrush.OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(1500d));
            App.DoubleAnimationType.AnimateEffect(ImageMenu, OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(2300d));
        }

        internal void ChangeVisibilityMillisecondInternet(bool Value)
        {
            ((MainPageButtonInfo)PagesButtonsInformation[0]).VisibilityInternetMillisecond(Value);
        }
        #endregion

        #region BlurBackground
        /// <summary>
        /// Анимировать цвет сигнала в главном Border окна
        /// </summary>
        /// <param name="Color">Цвет который будет отображён как сигнальный</param>
        internal void BlurMainAnimateColor(WmColor Color)
        {
            App.ColorAnimationType.AnimateEffect(BorderWindowMain.Background,
                SolidColorBrush.ColorProperty, Color, Colors.Black, TimeSpan.FromMilliseconds(1300d));
        }
        #endregion
    }
}