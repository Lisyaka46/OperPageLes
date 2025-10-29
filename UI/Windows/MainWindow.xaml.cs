#region Link
using IEL.CORE.Classes;
using IEL.CORE.Classes.Browser;
using IEL.CORE.Enums;
using IEL.GUI;
using NAudio.Wave;
using OperPageLes.CORE;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel;
using OperPageLes.UI.Pages.Browser;
using OperPageLes.UI.Pages.PanelButtonInformation.MainWindow;
using OperPageLes.UI.UserElementControl;
using OperPageLes.UI.Windows.Dialogs;
using OperPageLes.Windows;
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
#endregion

namespace OperPageLes.UI.Windows
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
        private PageControllerLoading PageControllerLoadingApplication;

        /// <summary>
        /// Настройка для панели действий страницы управления загрузочными элементами
        /// </summary>
        private PanelActionSettingVisual SettingVisualPageLoadingController;

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
        /// Канал воспроизведения звуков
        /// </summary>
        private WaveOut SoundChannelWaveOut { get; }

        /// <summary>
        /// Страница выбора новой страницы браузера
        /// </summary>
        private PageManagerBrowser ManagerBrowserNewPage;

#if DEBUG
        private readonly TextBlock DEV_Time;
        private readonly TextBlock DEV_Data;
        private readonly TextBlock DEV_IsLoadingProcess;
#endif
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

            #region Audio
            SoundChannelWaveOut = new()
            {
                Volume = App.CurrentApp.SettingMainApplication.Volume,
            };
            #endregion

            #region SettingParameters
            App.CurrentApp.SettingMainApplication.Volume.Changed += (Old, New) =>
            {
                SoundChannelWaveOut.Volume = New;
            };
            #endregion

            #region SetParameteres
            Icon = App.LoadImage(Properties.Resources.IconMainApplication);
            ImageLogoApplication.Imaging = App.LoadImage(Properties.Resources.IconMainApplication);
            IELImageButtonHelp.Imaging = App.LoadImage(Properties.Resources.LightBulb);
            IELButtonSettings.Imaging = App.LoadImage(Properties.Resources.IconMainSettings);
            IELImageButtonMenu.Imaging = App.LoadImage(Properties.Resources.Menu);

            TokenUpdateBackgroundData = new(false);
            ActualIndexActivatePageDownToolButtons = -1;
            IELPageControllerButtons.LeftAnimateSwitch = new(-5, 0, 0, 0);
            IELPageControllerButtons.RightAnimateSwitch = new(5, 0, 0, 0);

            IndicatorLoading.Opacity = 0d;
            IndicatorLoading.Source = new Uri(StructDirectoryResources.DirectoryFileLoadingDefault);
            IndicatorLoading.MediaEnded += (sender, e) =>
            {
                IndicatorLoading.Position = TimeSpan.FromMilliseconds(1);
            };

            VisualRectangleDateTimeBackground.Opacity = 0d;
            IELMessageMain.Opacity = 0d;
            IELActionPanelMain.Opacity = 0d;
            IELActionPanelMain.IsKeyboardModeExit = App.CurrentApp.SettingMainApplication.ExitKeyboardModeInClosePanelAction;
            IELActionPanelMain.KeyActivateKeyboardMode = App.CurrentApp.SettingMainApplication.KEY_KeyboardModePanelAction;
            IELActionPanelMain.KeyKeyboardModeActivateRightClick = App.CurrentApp.SettingMainApplication.KEY_PanelActionRightClick;
            IELActionPanelMain.KeyCloseElement = App.CurrentApp.SettingMainApplication.KEY_PanelActionClose;
            IELActionPanelMain.EventMovePanelAction += (sender, e) =>
            {
                StructDirectoryResources.Play(SoundChannelWaveOut, StructDirectoryResources.DirectoryFileAudioMove);
            };
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
            IELBrowserPageMain.IELButtonAddInlay.Imaging = App.LoadImage(Properties.Resources.Plus);

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
            PanelActionPageInlay = new(PageInlay);
            PanelActionSettingsInlay = new(IELBrowserPageMain, PanelActionPageInlay, new(200d, 240d));
            PanelActionPageInlay.IsKeyboardModeChanged += (Source, NewValue) =>
            {
                PageInlay.IELButtonPageOpenInlay.CharKeyboardActivate = NewValue;
                PageInlay.IELButtonPageDeleteInlay.CharKeyboardActivate = NewValue;
            };
            #endregion

            PageControllerLoadingApplication = new();
            SettingVisualPageLoadingController = new(GridMain, new(PageControllerLoadingApplication), new(210, 255));

            #endregion

            #region Settings
            UpdateImageMenu(App.CurrentApp.SettingMainApplication.PathMenuImage);
            ChangeBlurImageInDataTime(App.CurrentApp.SettingMainApplication.BlurBackgroundDataTime);
            #endregion

            #region UpToolButtons
            #region IELImageButtonHelp
            IELImageButtonHelp.OnActivateMouseLeft += (sender, e, Key) =>
            {
                WindowDiscriptionCommands j = new();
                App.CurrentApp.OpenedWindowsInApplication.Add(j);
                j.Show();
            };
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
                App.AnimateDoubleEffect(FrameNewInlayBrowser, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
                App.AnimateDoubleEffect(IELBrowserPageMain, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                App.AnimateDoubleEffect(IELBrowserPageBlurEffect, BlurEffect.RadiusProperty, 0d, TimeSpan.FromMilliseconds(500d));
                Canvas.SetZIndex(FrameNewInlayBrowser, -1);
                FrameNewInlayBrowser.IsHitTestVisible = false;
                FrameNewInlayBrowser.IsEnabled = false;
                IELBrowserPageMain.IsEnabled = true;
                if (e == null) return;
                IELInlay? SourceInlay = IELBrowserPageMain.AddInlayPage(e);
                if (SourceInlay != null)
                {
                    SourceInlay.SourceCloseButtonImage = App.LoadImage(Properties.Resources.Cross);
                }
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
            IELBrowserPageMain.IELButtonAddInlay.OnActivateMouseLeft += (sender, e, Key) =>
            {
                FrameNewInlayBrowser.IsEnabled = true;
                FrameNewInlayBrowser.IsHitTestVisible = true;
                IELBrowserPageMain.IsEnabled = false;
                App.AnimateDoubleEffect(FrameNewInlayBrowser, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                App.AnimateDoubleEffect(IELBrowserPageMain, OpacityProperty, 0.9d, TimeSpan.FromMilliseconds(500d));
                App.AnimateDoubleEffect(IELBrowserPageBlurEffect, BlurEffect.RadiusProperty, 20d, TimeSpan.FromMilliseconds(500d));
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
                    App.AnimateThicknessEffect(ImageLogoApplication, MarginProperty, new(8), BorderImageInformation.Margin, TimeSpan.FromMilliseconds(1400d));

                    App.AnimateThicknessEffect(BorderDateTime, MarginProperty, new(8), BorderDateTime.Margin, TimeSpan.FromMilliseconds(1400d));

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

        /// <summary>
        /// Воспроизвести звук по директории звукового файла
        /// </summary>
        /// <param name="Sound">Директория звукового файла</param>
        internal void Play(string Sound)
        {
            StructDirectoryResources.Play(SoundChannelWaveOut, Sound);
        }

        #region DownToolButtons
        //
        private void NextPageDownToolButtons(bool UpIndex = true)
        {
            StructDirectoryResources.Play(SoundChannelWaveOut, StructDirectoryResources.DirectoryFileAudioMove);
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
            OPLViewerLoadingProcess ViewLoading = GenerateVisualizateLoadingProcess(NameProcess);
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
        /// Создать объект визуализирующий загрузочный процесс
        /// </summary>
        /// <param name="NameProcess">Название загрузочного процесса</param>
        /// <returns>Объект визуализации загрузочного процесса</returns>
        internal OPLViewerLoadingProcess GenerateVisualizateLoadingProcess(string NameProcess)
        {
            OPLViewerLoadingProcess Result = PageControllerLoadingApplication.SetViewElementLoading();
            App.CurrentApp.DataViewerLoadingProcess.Add(Result);
            Result.Text = NameProcess;
            return Result;
        }

        /// <summary>
        /// Начало визуализации загрузки
        /// </summary>
        /// <param name="ViewLoading">Элемент визуализации загрузочного процесса</param>
        internal void StartVisualizateLoadingProcess(OPLViewerLoadingProcess ViewLoading)
        {
            if (!IsLoadingProcess)
            {
                IsLoadingProcess = true;
#if DEBUG
                DEV_IsLoadingProcess.Text = $"ILoad_P: {IsLoadingProcess}";
#endif
                App.AnimateDoubleEffect(IndicatorLoading, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            }
            ViewLoading.VisualOpenLoading();
        } 

        /// <summary>
        /// Завершение визуализации загрузки
        /// </summary>
        /// <param name="ViewLoading">Элемент визуализации загрузочного процесса</param>
        internal void CompleteVisualizateLoadingProcess(OPLViewerLoadingProcess ViewLoading)
        {
            ViewLoading.VisualCloseLoading();
            PageControllerLoadingApplication.DeleteViewElementLoading(ViewLoading);
            App.CurrentApp.DataViewerLoadingProcess.Remove(ViewLoading);
            if (App.CurrentApp.DataViewerLoadingProcess.Count == 0 && IsLoadingProcess)
            {
                IsLoadingProcess = false;
#if DEBUG
                DEV_IsLoadingProcess.Text = $"ILoad_P: {IsLoadingProcess}";
#endif
                App.AnimateDoubleEffect(IndicatorLoading, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1700d));
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
        }

        /// <summary>
        /// Успешная установка картинки фона
        /// </summary>
        /// <param name="bitmap">Карта изображения</param>
        private void ComplitedInstallImageMenu(BitmapImage bitmap)
        {
            ImageMenu.Source = bitmap;

            App.AnimateDoubleEffect(BlurEffectImageMenu, BlurEffect.RadiusProperty, 10d, 0d, TimeSpan.FromMilliseconds(2300d));
            App.AnimateThicknessEffect(ImageMenu, MarginProperty, new(-4), new(0), TimeSpan.FromMilliseconds(2300d));
            App.AnimateDoubleEffect(ImageMenu, OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(2300d));
        }

        /// <summary>
        /// Неудачное завершение установки картинки фона
        /// </summary>
        private void FailedInstallImageMenu()
        {
            System.Windows.MessageBox.Show("Не удалось загрузить фоновое изображение...", "Информация", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
        }

        internal void ChangeVisibilityMillisecondInternet(bool Value)
        {
            ((MainPageButtonInfo)PagesButtonsInformation[0]).VisibilityInternetMillisecond(Value);
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