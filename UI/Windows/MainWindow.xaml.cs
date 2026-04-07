#region Link
using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel;
using OperPageLes.UI.Pages.Browser;
using OperPageLes.UI.Pages.Browser.BrowserPageNetwork;
using OperPageLes.UI.Pages.PanelButtonInformation.MainWindow;
using OperPageLes.UI.Windows.Base;
using OperPageLes.UI.Windows.Dialogs;
using OperPageLes.Windows;
using IEL.CORE.Enums;
using IEL.UserElementsControl;
using OIEL.CORE.Browser;
using OIEL.UserElementsControl;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OPRES = OperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;
#endregion

namespace OperPageLes.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : OPLWindowBase
    {
        #region PanelAction
        /// <summary>
        /// Страница взаимодействия с вкладками браузера страниц
        /// </summary>
        private static readonly PageInlayPanelAction PageInlay = new();
        #endregion

        /// <summary>
        /// Токен управляемой асинхронной операции обновления информации в главном окне
        /// </summary>
        private readonly CancellationToken TokenUpdateBackgroundData;

        //private MMDeviceEnumerator Device = new();

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
        /// Текущее состояние отображения загрузки процесса
        /// </summary>
        private bool IsVisualLoagingProcessInBorder;

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

        #region BorderMainWindowLoading
        /// <summary>
        /// 
        /// </summary>
        private LinearGradientBrush LinearGradientMainWindowBackground;

        /// <summary>
        /// Объект манипуляции вращением фона главного окна
        /// </summary>
        private static RotateTransform RotateMainWindowBackground = new()
        {
            Angle = 0d,
            CenterX = 0.5d,
            CenterY = 0.5d,
        };
        #endregion

        public MainWindow()
        {
            InitializeComponent();

			#region SetParameteres
            ManagerAnimation = App.ManagerAnimation;
			Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            ImageLogoApplication.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            IELButtonTheme.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Brush));
            IELButtonSettings.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainSettings));
            IELImageButtonCollapse.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Collapse));
            IELImageButtonClose.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));

            IELImageButtonMenu.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Menu));

            #region BrowserPage
            App.CurrentApp.MainBrowser.ManagerAnimation = App.ManagerAnimation;
            App.CurrentApp.MainBrowser.Margin = new(4d);

            App.CurrentApp.MainBrowser.EventCloseBrowser += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            App.CurrentApp.MainBrowser.EventChangeActiveInlay += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            App.CurrentApp.MainBrowser.EventCloseInlay += () =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            App.CurrentApp.MainBrowser.EventAddInlay += () =>
            {
                if (IELActionPanelMain.PanelActionActivate)
                    IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            };
            #endregion

            GridContentFomBrowser.Children.Add(App.CurrentApp.MainBrowser);
            TokenUpdateBackgroundData = new(false);
            ActualIndexActivatePageDownToolButtons = -1;
            IELPageControllerButtons.LeftAnimateSwitch = new(-5, 0, 0, 0);
            IELPageControllerButtons.RightAnimateSwitch = new(5, 0, 0, 0);

            BorderNotificationIndicator.Opacity = 0d;
            IndicatorLoading.Opacity = 0d;
            IndicatorLoading.Source = StructDirectoryResources.GetResourceUri(nameof(OPRES.MediaLoadingDefault));
            IndicatorLoading.MediaEnded += (sender, e) =>
            {
                IndicatorLoading.Position = TimeSpan.FromMilliseconds(5);
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

            #region Palette
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(ImageLogoApplication);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Chocolate].ConnectPalleteFromIELElement(IELButtonTheme);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonSettings);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.LightBlue].ConnectPalleteFromIELElement(IELImageButtonCollapse);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Red].ConnectPalleteFromIELElement(IELImageButtonClose);

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Cocoa].ConnectPalleteFromIELElement(IELActionPanelMain);

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELImageButtonBackButtons);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELImageButtonNextButtons);

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELImageButtonMenu);
            #endregion

            LinearGradientMainWindowBackground = new()
            {
                GradientStops = new(
                    [
                    new GradientStop(WnColor.FromRgb(23, 43, 32), 0d),
                    new GradientStop(WnColor.FromRgb(43, 56, 49), 0.168d),
                    new GradientStop(WnColor.FromRgb(48, 58, 66), 0.257d),
                    new GradientStop(WnColor.FromRgb(60, 70, 82), 0.432d),
                    new GradientStop(WnColor.FromRgb(86, 116, 140), 0.582d),
                    new GradientStop(WnColor.FromRgb(115, 109, 94), 0.764d),
                    new GradientStop(WnColor.FromRgb(72, 64, 41), 1d),
                    ]),
                RelativeTransform = RotateMainWindowBackground,
            };
            Background = LinearGradientMainWindowBackground;

            Canvas.SetZIndex(IELMessageMain, -2);
            Canvas.SetZIndex(IELActionPanelMain, -2);
            #endregion

            #region IELPanelAction
            IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                PageBrowser? Page = App.CurrentApp.MainBrowser.ActualInlay?.Content;
                if (Page == null) return;
                switch (Page.GetType().Name)
                {
                    case "PageConsole":
                        ((PageConsole)Page).TextBoxCommandInput.Focus();
                        break;
                    default: return;
                }
            };

            #region PageInlay
            PageInlay.IELButtonPageOpenInlay.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (PageInlay.ActivateManipulateInlay?.Content != null)
                    App.CurrentApp.MainBrowser.ActivateInlayInBrowserPage(PageInlay.ActivateManipulateInlay.Content);
            };
            PageInlay.IELButtonPageDeleteInlay.OnActivateMouseLeft += (sender, e, Key) =>
            {
                IELActionPanelMain.ClosePanelAction();
                if (PageInlay.ActivateManipulateInlay != null)
                    App.CurrentApp.MainBrowser.DeleteInlayPage(PageInlay.ActivateManipulateInlay);
            };
            #endregion

            PageControllerLoadingApplication = new();
            PageControllerLoadingApplication.CreatedNewOneOnlyViewerImage += (sender, e) =>
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderNotificationIndicator, OpacityProperty, 1d, TimeSpan.FromMilliseconds(100d));
            };
            PageControllerLoadingApplication.ClearedAllViewersImage += (sender, e) =>
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderNotificationIndicator, OpacityProperty, 0d, TimeSpan.FromMilliseconds(100d));
            };

            #endregion

            #region Settings
            UpdateImageMenu(App.CurrentApp.SettingMainApplication.PathMenuImage);

            App.CurrentApp.SettingMainApplication.LoadingBorderVisualizate.Changed += (Old, New) =>
            {
                if (!IsVisualLoagingProcessInBorder) EndRotateBorder(New ? 1 : -1);
            };
            #endregion

            #region UpToolButtons

            #region ImageLogoApplication
            ImageLogoApplication.OnActivateMouseLeft += (sender, e) =>
            {
                DialogLicenseWindow License = new();
                License.Show();
            };
            #endregion

            #region IELButtonTheme
            IELButtonTheme.MouseHover += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELButtonTheme,
                    "Управление персонализацией программы",
                    OrientationPositionCursor.LeftDown);
            };
            IELButtonTheme.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            IELButtonTheme.OnActivateMouseLeft += (sender, e) =>
            {
                WindowThemeController Window = new();
                App.CurrentApp.InicializeWindowInApplication(Window);
                if (!Path.Exists(StructDirectoryResources.DirectoryThemeApplication))
                    Directory.CreateDirectory(StructDirectoryResources.DirectoryThemeApplication);
                Window.Show();
                //new DialogSetting().ShowDialog();
            };
            #endregion

            #region IELImageButtonCollapse
            IELImageButtonCollapse.OnActivateMouseLeft += (sender, e) =>
            {
                WindowState = WindowState.Minimized;
            };
            #endregion

            #region IELImageButtonClose
            IELImageButtonClose.OnActivateMouseLeft += (sender, e) =>
            {
                Close();
            };
            #endregion

            #region IELButtonSettings
            IELButtonSettings.MouseHover += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELButtonSettings,
                    "Настройки программы",
                    OrientationPositionCursor.LeftDown);
            };
            IELButtonSettings.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            IELButtonSettings.OnActivateMouseLeft += (sender, e) =>
            {
                new DialogSetting().ShowDialog();
            };
            #endregion

            #endregion

            #region DownToolButtons
            ActualIndexActivatePageDownToolButtons = 0;
            IELImageButtonNextButtons.OnActivateMouseLeft += (sender, e) => NextPageDownToolButtons();
            IELImageButtonBackButtons.OnActivateMouseLeft += (sender, e) => NextPageDownToolButtons(false);

            IELImageButtonNextButtons.MouseEnter += (sender, e) => IELPageControllerButtons.MoveActualPage(new(-3, 0, 0, 0), 400u);
            IELImageButtonNextButtons.MouseLeave += (sender, e) => IELPageControllerButtons.MoveActualPage(new(0), 400u);
            IELImageButtonBackButtons.MouseEnter += (sender, e) => IELPageControllerButtons.MoveActualPage(new(3, 0, 0, 0), 400u);
            IELImageButtonBackButtons.MouseLeave += (sender, e) => IELPageControllerButtons.MoveActualPage(new(0), 400u);

            IELButtonHomeBrowser.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Home));
            IELButtonHomeBrowser.OnActivateMouseLeft += (sender, e) =>
            {
                App.CurrentApp.MainBrowser.OpenManagerAppPage();
            };

            #region BorderIndicator
            BorderIndicator.MouseEnter += (sender, e) =>
            {
                if (IELActionPanelMain.PanelActionActivate) return;
                IELMessageMain.UsingBorderInformation(BorderIndicator,
                    "Менеджер оповещений",
                    OrientationPositionCursor.LeftUp);
            };
            BorderIndicator.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            BorderIndicator.MouseRightButtonUp += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
                App.MainWindow.IELActionPanelMain.UsingPanelAction(GridMain, PageControllerLoadingApplication,
                    Orientation: OrientationPositionCursor.LeftUp);
            };
            #endregion
            #endregion

            #region EventsWindow
            SizeChanged += (sender, e) =>
            {
                if (IELActionPanelMain.PanelActionActivate)
                    IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            };
            base.Closing += (sender, e) =>
            {
                if (!IsReboot && !IsClosing) Close();
            };
            #endregion
        }

        /// <summary>
        /// Активировать панель действий над элементом вкладки
        /// </summary>
        /// <param name="sender">Объект представления вкладки</param>
        /// <param name="e">Объект упрвления событием</param>
        private void InlayPanelActionActivate(object sender, MouseButtonEventArgs e)
        {
            PageInlay.ActivateManipulateInlay = (OPLInlay)sender;
            IELActionPanelMain.UsingPanelAction(App.CurrentApp.MainBrowser, PageInlay,
                Orientation: OrientationPositionCursor.RightDown);
        }

        #region ManipulateWindow
        /// <summary>
        /// Закрыть главное окно приложения без перезагрузки
        /// </summary>
        public new void Close()
        {
            IsClosing = true;
            Hide();
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
                        Thread.Sleep(10);
                    }
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
            TextBlock StackUpdateData = App.ApplicationPageDeveloper.AddNewStackTextBlock("Task: Обновление данных");
            Task.Run(async () =>
            {
                while (true)
                {
                    Dispatcher.Invoke(() => App.ManagerAnimation.ColorAnimationType.AnimateEffect((SolidColorBrush)StackUpdateData.Foreground,
                        SolidColorBrush.ColorProperty, Colors.LightGreen, Colors.Black, TimeSpan.FromMilliseconds(300d)));
                    await BackgroundUpdateVisualData();
                    Thread.Sleep(1000);
                }
            }, TokenUpdateBackgroundData);
            PagesButtonsInformation =
                    [
                        new MainPageButtonInfo(), new Page2()
                    ];
            IELPageControllerButtons.NextPage(PagesButtonsInformation[0], false);
            Opacity = 0d;
            ((MainPageButtonInfo)PagesButtonsInformation[0]).ThreadInternetConnection.Start();
            base.Show();
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(ImageLogoApplication, MarginProperty,
                new(8), BorderImageInformation.Margin, TimeSpan.FromMilliseconds(1400d));

            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(RotateMainWindowBackground, RotateTransform.AngleProperty,
                0d, 360d, TimeSpan.FromMilliseconds(3200d));
            App.CurrentApp.MainBrowser.OpenManagerAppPage();

            #region AppPage
            App.CurrentApp.AddNewAppPage(typeof(PageConsole), "Консоль",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.LightBlue], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Command)));
            App.CurrentApp.AddNewAppPage(typeof(PageLabels), "Ярлыки",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Tangerine], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.PaperClip)));
            App.CurrentApp.AddNewAppPage(typeof(PageNetwork), "Сеть",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Chats)));
            App.CurrentApp.AddNewAppPage(typeof(PageWebBrowser), "Веб-браузер",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Aquamarine], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Link)));
            App.CurrentApp.AddNewAppPage(typeof(PageDeveloper), "Для разработчиков",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed]);
            #endregion
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
        /// <returns>Исполненный асинхронный процесс</returns>
        internal async Task<T> ExecuteVisualizateLoadingProcess<T>(string NameProcess, Task<T> Method)
        {
            OPLMediaViewer ViewLoading = GenerateVisualizateMediaLoadingProcess(NameProcess);
            ViewLoading.Dispatcher.Invoke(StartVisualizateLoadingProcess, ViewLoading);
            CancellationToken token = new(false);
            //ViewLoading.OnActivateMouseRight += (sender, e) =>
            //{
            //    token.ThrowIfCancellationRequested();
            //    ViewLoading.Dispatcher.Invoke(CompleteVisualizateLoadingProcess, ViewLoading);
            //};
            TextBlock Element = App.ApplicationPageDeveloper.AddNewStackTextBlock("Task: " + NameProcess);

            await Method.WaitAsync(token);

            App.ApplicationPageDeveloper.StackPanelElementsVisual.Children.Remove(Element);
            if (Method.IsCanceled) throw new OperationCanceledException();
            ViewLoading.Dispatcher.Invoke(CompleteVisualizateLoadingProcess, ViewLoading);

            return await Method;
        }

        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки без ожидаемого значения
        /// </summary>
        /// <typeparam name="T">Тип ожидаемого элемента</typeparam>
        /// <param name="NameProcess">Название загрузочного процесса</param>
        /// <param name="Method">Асинхронный процесс получения значения</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        internal async Task ExecuteVisualizateLoadingProcess(string NameProcess, Task Method)
        {
            OPLMediaViewer ViewLoading = GenerateVisualizateMediaLoadingProcess(NameProcess);
            ViewLoading.Dispatcher.Invoke(StartVisualizateLoadingProcess, ViewLoading);
            CancellationToken token = new(false);
            //ViewLoading.OnActivateMouseRight += (sender, e) =>
            //{
            //    token.ThrowIfCancellationRequested();
            //    ViewLoading.Dispatcher.Invoke(CompleteVisualizateLoadingProcess, ViewLoading);
            //};
            TextBlock Element = App.ApplicationPageDeveloper.AddNewStackTextBlock("Task: " + NameProcess);

            await Method.WaitAsync(token);

            App.ApplicationPageDeveloper.StackPanelElementsVisual.Children.Remove(Element);
            if (Method.IsCanceled) throw new OperationCanceledException();
            ViewLoading.Dispatcher.Invoke(CompleteVisualizateLoadingProcess, ViewLoading);
        }

        /// <summary>
        /// Создать объект визуализирующий изображение (уведомление)
        /// </summary>
        /// <param name="Name">Название сообщения</param>
        /// <param name="ImageNotification">Картинка которая будет отображаться в элементе</param>
        /// <returns>Объект визуализации картинки</returns>
        public OPLImageViewer GenerateVisualizateImage(string Name, ImageSource? ImageNotification = null)
        {
            OPLImageViewer Result = PageControllerLoadingApplication.SetViewImageElement(ImageNotification);
            //Result.Text = Name;
            //Result.OnActivateMouseRight += Result.OnActivateMouseLeft;
            return Result;
        }

        /// <summary>
        /// Создать объект визуализирующий медиа (уведомление)
        /// </summary>
        /// <param name="Name">Название загрузочного процесса</param>
        /// <returns>Объект визуализации загрузочного процесса</returns>
        public OPLMediaViewer GenerateVisualizateMedia(string Name)
        {
            OPLMediaViewer Result = PageControllerLoadingApplication.SetViewMediaElement();
            //Result.Text = Name;
            //Result.OnActivateMouseRight += Result.OnActivateMouseLeft;
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
                if (App.CurrentApp.SettingMainApplication.LoadingBorderVisualizate)
                {
                    BeginRotateBorder();
                    IsVisualLoagingProcessInBorder = true;
                }
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(IndicatorLoading, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            }
        } 

        /// <summary>
        /// Завершение визуализации загрузки
        /// </summary>
        /// <param name="ViewLoading">Элемент визуализации загрузочного процесса</param>
        internal void CompleteVisualizateLoadingProcess(OPLMediaViewer ViewLoading)
        {
            PageControllerLoadingApplication.DeleteViewMediaElement(ViewLoading);
            App.CurrentApp.DataViewerLoadingProcess.Remove(ViewLoading);
            if (App.CurrentApp.DataViewerLoadingProcess.Count == 0 && IsLoadingProcess)
            {
                IsLoadingProcess = false;
                if (IsVisualLoagingProcessInBorder)
                {
                    EndRotateBorder();
                    IsVisualLoagingProcessInBorder = false;
                }
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(IndicatorLoading, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1700d));
            }
        }

        #region VisualBorderLoading
        /// <summary>
        /// Начать анимацию поворота барьера
        /// </summary>
        private static void BeginRotateBorder()
        {
            DoubleAnimation animation = App.ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
            animation.From = 0d;
            animation.To = 3600d;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.EasingFunction = null;
            animation.Duration = TimeSpan.FromMilliseconds(30000d);
            RotateMainWindowBackground.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        /// <summary>
        /// Закончить анимацию барьера
        /// </summary>
        /// <param name="FromValue">Стартовое значение анимирования</param>
        private static void EndRotateBorder(int FullCountRotate = 1)
        {
            DoubleAnimation animation = App.ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
            animation.From = RotateMainWindowBackground.Angle % 360;
            animation.To = 361d * FullCountRotate;
            animation.Duration = TimeSpan.FromMilliseconds(3200d);
            RotateMainWindowBackground.BeginAnimation(RotateTransform.AngleProperty, animation);
        }
        #endregion
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
                        IELMessageMain.UsingBorderInformation(Element, ex.Message, OrientationPositionCursor.LeftUp);
                    };
                    Element.MouseLeave += (sender, e) =>
                    {
                        IELMessageMain.CloseBorderInformation();
                    };
                }
            }
            else
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageMenu, ImageBrush.OpacityProperty, 0d, TimeSpan.FromMilliseconds(2300d));
            }
        }

        /// <summary>
        /// Успешная установка картинки фона
        /// </summary>
        /// <param name="bitmap">Карта изображения</param>
        private void ComplitedInstallImageMenu(BitmapImage bitmap)
        {
            ImageMenu.ImageSource = bitmap;

            App.ManagerAnimation.RectAnimationType.AnimateEffect(ImageMenu, ImageBrush.ViewboxProperty, new(0.025, 0.025, 0.95, 0.95), new(0, 0, 1, 1), TimeSpan.FromMilliseconds(2300d));
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(ImageMenu, MarginProperty, new(-4), new(0), TimeSpan.FromMilliseconds(2300d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageMenu, ImageBrush.OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(1500d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageMenu, OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(2300d));
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
        internal void BlurMainAnimateColor(WnColor Color)
        {
            //App.ColorAnimationType.AnimateEffect(BorderWindowMain.Background,
            //    SolidColorBrush.ColorProperty, Color, Colors.Black, TimeSpan.FromMilliseconds(1300d));
        }
        #endregion
    }
}