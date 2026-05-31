#region Link
using IEL.CORE.Enums;
using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using OIEL.CORE.Browser;
using OIEL.UserElementsControl;
using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Objects;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel;
using OperPageLes.UI.Pages.ActionPanel.Other;
using OperPageLes.UI.Pages.Browser;
using OperPageLes.UI.Pages.Browser.BrowserPageNetwork;
using OperPageLes.UI.Windows.Base;
using OperPageLes.UI.Windows.Dialogs;
using OperPageLes.Windows;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Globalization;
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
        [LibraryImport("user32.dll", EntryPoint = "keybd_event")]
        private static partial void Keybd_event(byte CodeButton, byte CodeScan, uint CodeState, UIntPtr dwExtralnfo);

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

        /// <summary>
        /// Страница управления загрузочными процессами
        /// </summary>
        private PageNotificationManager PageNotificationApplication;

        /// <summary>
        /// Страница управления аудио устройствами
        /// </summary>
        private PageAudioControl PageAudioControlApplication;

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
            IELOpenDataFolder.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Folder));
            IELButtonTheme.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Brush));
            IELButtonSettings.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainSettings));
            IELButtonCollapse.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Collapse));
            IELButtonClose.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));

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

            TextBlockVersion.Text = App.Version;

            NotificationIndicator.Opacity = 0d;
            VisualLoadingElement.ManagerAnimation = App.ManagerAnimation;
            VisualLoadingElement.Opacity = 0d;
            TextBlockCountLoadingProcess.Opacity = 0d;

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
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PlumCrayola].ConnectPalleteFromIELElement(IELButtonBack);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELOpenDataFolder);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Chocolate].ConnectPalleteFromIELElement(IELButtonTheme);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonSettings);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.LightBlue].ConnectPalleteFromIELElement(IELButtonCollapse);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Red].ConnectPalleteFromIELElement(IELButtonClose);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELBlockInfoInternetConnection);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Saffron].ConnectPalleteFromIELElement(IELBlockInfoStateRegister);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Violet].ConnectPalleteFromIELElement(IELBlockInfoCurrentLanguage);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELBlockInfoVolume);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Cocoa].ConnectPalleteFromIELElement(IELActionPanelMain);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PlumCrayola].ConnectPalleteFromIELElement(IELButtonHomeBrowser);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.BlueGreenCrayola].ConnectPalleteFromIELElement(IELButtonAddLabel);
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

            PageNotificationApplication = new();
            App.CurrentApp.AddNotification += (sender, e) =>
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(NotificationIndicator, OpacityProperty, 1d, TimeSpan.FromMilliseconds(100d));
            };
            App.CurrentApp.ClearNotification += (sender, e) =>
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(NotificationIndicator, OpacityProperty, 0d, TimeSpan.FromMilliseconds(100d));
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
                if (App.CurrentApp.ThemeApp == null)
                {
                    App.CurrentApp.ThemeApp = new()
                    {
                        ManagerAnimation = App.ManagerAnimation,
                        SourcePanelAction = IELActionPanelMain
                    };
                    App.CurrentApp.ThemeApp.LoadingThemes();
                }
                ActivateCustomPageBrowser(App.CurrentApp.ThemeApp);
            };
            #endregion

            #region IELImageButtonCollapse
            IELButtonCollapse.OnActivateMouseLeft += (sender, e) =>
            {
                WindowState = WindowState.Minimized;
            };
            #endregion

            #region IELImageButtonClose
            IELButtonClose.OnActivateMouseLeft += (sender, e) =>
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
                App.CurrentApp.SettingApp ??= new();
                ActivateCustomPageBrowser(App.CurrentApp.SettingApp);
            };
            #endregion

            #region IELButtonBack
            IELButtonBack.IsEnabled = false;
            IELButtonBack.Margin = new(0, 5, 6, 0);
            IELButtonBack.Width = 0d;
            IELButtonBack.OnActivateMouseLeft += (sender, e) =>
            {
                if (IELActionPanelMain.PanelActionActivate)
                    IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                Disable_IELButtonBack();
                App.CurrentApp.MainBrowser.GoBack();
            };
            #endregion

            #region IELOpenDataFolder
            IELOpenDataFolder.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELOpenDataFolder,
                    "Главная директория данных программы",
                    OrientationPositionCursor.LeftDown);
            };
            IELOpenDataFolder.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            IELOpenDataFolder.OnActivateMouseLeft += (sender, e) =>
            {
                Process p = new();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/c start {StructDirectoryResources.MainDirectoryApplication}";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                if (IELActionPanelMain.PanelActionActivate)
                    IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            };
            #endregion

            #endregion

            #region DownToolButtons

            #region IELButtonAddLabel
            IELButtonAddLabel.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Plus));
            IELButtonAddLabel.OnActivateMouseLeft += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
                DialogGenLabel dialog = new();
                SourceLabelAction? Result = dialog.CreateLabel();
                if (Result == null) return;
                App.CurrentApp.AddNewLabel(Result);
            };
            IELButtonAddLabel.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELButtonAddLabel,
                    "Добавить ярлык на главную страницу",
                    OrientationPositionCursor.RightUp);
            };
            IELButtonAddLabel.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion

            #region IELButtonHomeBrowser
            IELButtonHomeBrowser.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Home));
            IELButtonHomeBrowser.OnActivateMouseLeft += (sender, e) =>
            {
                if (IELActionPanelMain.PanelActionActivate)
                    IELActionPanelMain.ClosePanelAction();
                if (IELButtonBack.IsEnabled)
                    Disable_IELButtonBack();
                App.CurrentApp.MainBrowser.OpenManagerAppPage();
            };
            #endregion

            #region IELBlockInfoInternetConnection
            IELBlockInfoInternetConnection.IsEnabled = false;
            IELBlockInfoInternetConnection.Padding = App.CurrentApp.SettingMainApplication.MillisecondInternetConnection ?
                new(0, 0, 0, 7) : new(0);
            IELBlockInfoInternetConnection.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Wifi));
            TextBlockInternetConnectionMillisecond.Opacity = 0d;
            IELBlockInfoInternetConnection.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELBlockInfoInternetConnection,
                    "Текущее подключение к интернету",
                    OrientationPositionCursor.RightUp);
            };
            IELBlockInfoInternetConnection.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion

            #region IELBlockInfoStateRegister
            IELBlockInfoStateRegister.Text = Console.CapsLock ? "а".ToUpper() : "a".ToLower();
            IELBlockInfoStateRegister.MouseUp += (sender, e) =>
            {
                Keybd_event(0x14, 0x45, 0x1, 0);
                Keybd_event(0x14, 0x45, 0x1 | 0x2, 0);
                IELBlockInfoStateRegister.Text = !Console.CapsLock ? "а".ToUpper() : "a".ToLower();
            };
            IELBlockInfoStateRegister.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELBlockInfoStateRegister,
                    "Регистр символов клавиатуры",
                    OrientationPositionCursor.RightUp);
            };
            IELBlockInfoStateRegister.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion

            #region IELBlockInfoCurrentLanguage
            IELBlockInfoCurrentLanguage.Text = InputLanguage.CurrentInputLanguage.Culture.NativeName[..3].ToUpper();
            InputLanguageManager.Current.InputLanguageChanged += (sender, e) =>
            {
                IELBlockInfoCurrentLanguage.Text = e.NewLanguage.NativeName[..3].ToUpper();
            };
            IELBlockInfoCurrentLanguage.MouseEnter += (sender, e) =>
            {
                IELMessageMain.UsingBorderInformation(IELBlockInfoCurrentLanguage,
                    "Текущая раскладка клавиатуры",
                    OrientationPositionCursor.RightUp);
            };
            IELBlockInfoCurrentLanguage.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            #endregion

            #region IELBlockInfoVolume
            PageAudioControlApplication = new();
            IELBlockInfoVolume.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Volume));
            TextBlockVolumeValue.Foreground = IELBlockInfoVolume.SourceForeground.SourceBrush;
            TextBlockVolumeValue.Text = ((int)(App.CurrentApp.SettingMainApplication.Volume * 100)).ToString();
            IELBlockInfoVolume.MouseHover += (sender, e) =>
            {
                if (IELActionPanelMain.PanelActionActivate && IELActionPanelMain.ActualVisualPage is PageAudioControl) return;
                IELMessageMain.UsingBorderInformation(IELBlockInfoVolume,
                    "Громкость звуков программы",
                    OrientationPositionCursor.RightUp);
            };
            IELBlockInfoVolume.MouseLeave += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
            };
            IELBlockInfoVolume.MouseRightButtonUp += (sender, e) =>
            {
                IELMessageMain.CloseBorderInformation();
                App.MainWindow.IELActionPanelMain.UsingPanelAction(IELBlockInfoVolume, PageAudioControlApplication,
                    Orientation: OrientationPositionCursor.RightUp,
                    DependencePointOnSize: false);
            };
            IELBlockInfoVolume.MouseLeftButtonUp += (sender, e) =>
            {
                StructDirectoryResources.Play(App.CurrentApp.SoundChannelWaveOut, nameof(OPRES.AudioPopUp));
            };
            #endregion

            #region BorderIndicator
            BorderIndicator.MouseEnter += (sender, e) =>
            {
                if (IELActionPanelMain.PanelActionActivate && IELActionPanelMain.ActualVisualPage is PageNotificationManager) return;
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
                App.MainWindow.IELActionPanelMain.UsingPanelAction(BorderIndicator, PageNotificationApplication,
                    Orientation: OrientationPositionCursor.LeftUp,
                    DependencePointOnSize: false);
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

            App.ConnectionPingChanged += (sender, e) =>
            {
                IELBlockInfoInternetConnection.Source =
                    StructDirectoryResources.GetResourceBitmap(e.Connect ? nameof(OPRES.WifiOn) : nameof(OPRES.WifiOff));
                TextBlockInternetConnectionMillisecond.Text = e.Connect ? $"{e.Ping}ms" : string.Empty;
                if (e.Connect != IELBlockInfoInternetConnection.IsEnabled)
                {
                    IELBlockInfoInternetConnection.IsEnabled = e.Connect;
                    App.ManagerAnimation.DoubleAnimationType.AnimateEffect(IELBlockInfoInternetConnection, OpacityProperty,
                        e.Connect ? 1d : 0d, TimeSpan.FromMilliseconds(400d));
                }
            };

            App.CurrentApp.SettingMainApplication.Volume.Changed += (Old, New) =>
            {
                TextBlockVolumeValue.Text = ((int)(New * 100)).ToString();
            };

            Activated += (sender, e) =>
            {
                if (App.CurrentApp.MainBrowser.ActivateManagerPage)
                {
                    App.CurrentApp.ManagerAppPage.Focus();
                }
            };
        }

        #region IELButtonBackControl
        /// <summary>
        /// Активировать/Показать кнопку возврата назад
        /// </summary>
        private void Enable_IELButtonBack()
        {
            IELButtonBack.IsEnabled = true;
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(IELButtonBack, WidthProperty,
                80, TimeSpan.FromMilliseconds(400d));
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(IELButtonBack, MarginProperty,
                new(5, 5, 6, 5), TimeSpan.FromMilliseconds(400d));
        }

        /// <summary>
        /// Диактивировать/Скрыть кнопку возврата назад
        /// </summary>
        private void Disable_IELButtonBack()
        {
            IELButtonBack.IsEnabled = false;
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(IELButtonBack, WidthProperty,
                0, TimeSpan.FromMilliseconds(400d));
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(IELButtonBack, MarginProperty,
                new(0, 5, 6, 0), TimeSpan.FromMilliseconds(400d));
        }
        #endregion

        /// <summary>
        /// Воспроизвести активацию собственной страницы в браузере страниц с логикой отображения
        /// </summary>
        /// <param name="SourcePage">Открываемая страница</param>
        /// <param name="RightAlign">Парвая ориентация появления</param>
        internal void ActivateCustomPageBrowser(PageBrowser SourcePage, bool RightAlign = true)
        {
            if (IELActionPanelMain.PanelActionActivate)
                IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            if (App.CurrentApp.MainBrowser.ActualPage != null)
                Enable_IELButtonBack();
            App.CurrentApp.MainBrowser.ActivateCustomPageBrowser(SourcePage, RightAlign);
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
            App.CurrentApp.TokenInternetConnection.ThrowIfCancellationRequested();
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
            Opacity = 0d;
            base.Show();

            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(TextBlockInternetConnectionMillisecond, OpacityProperty,
                        App.CurrentApp.SettingMainApplication.MillisecondInternetConnection ? 1d : 0d, TimeSpan.FromMilliseconds(800d));

            App.CurrentApp.MainBrowser.OpenManagerAppPage();

            #region AppPage
            App.CurrentApp.AddNewAppPage(typeof(PageConsole), "Консоль",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.LightBlue], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Command)));
            App.CurrentApp.AddNewAppPage(typeof(PageNetwork), "Сеть",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Chats)));
            App.CurrentApp.AddNewAppPage(typeof(PageWebBrowser), "Веб-браузер",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Aquamarine], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.World)));
            App.CurrentApp.AddNewAppPage(typeof(PageDeveloper), "Для разработчиков",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed]);
            #endregion
        }
        #endregion

        /// <summary>
        /// Начало визуализации загрузки
        /// </summary>
        /// <param name="ViewLoading">Элемент визуализации загрузочного процесса</param>
        internal void StartVisualizateLoadingProcess()
        {
            if (!IsLoadingProcess)
            {
                IsLoadingProcess = true;
                if (App.CurrentApp.SettingMainApplication.LoadingBorderVisualizate)
                {
                    BeginRotateBorder();
                    IsVisualLoagingProcessInBorder = true;
                }
                VisualLoadingElement.OpenLoading();
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(TextBlockCountLoadingProcess, OpacityProperty, 1d, TimeSpan.FromMilliseconds(400d));
            }
        } 

        /// <summary>
        /// Завершение визуализации загрузки
        /// </summary>
        /// <param name="ViewLoading">Элемент визуализации загрузочного процесса</param>
        internal void CompleteVisualizateLoadingProcess()
        {
            if (IsLoadingProcess)
            {
                IsLoadingProcess = false;
                if (IsVisualLoagingProcessInBorder)
                {
                    EndRotateBorder();
                    IsVisualLoagingProcessInBorder = false;
                }
                VisualLoadingElement.CloseLoading();
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(TextBlockCountLoadingProcess, OpacityProperty, 0d, TimeSpan.FromMilliseconds(400d));
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
                catch (FileNotFoundException)
                {
                    App.CurrentApp.AddNewNotification($"Файл картинки фонового изображения не был найден...",
                        EnumNotificationStyle.System,
                        StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Warning)),
                        "Ошибка установки фона");
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
            if (IELBlockInfoInternetConnection.IsEnabled)
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(TextBlockInternetConnectionMillisecond, OpacityProperty,
                    Value ? 1d : 0d, TimeSpan.FromMilliseconds(400d));
            }
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(IELBlockInfoInternetConnection, IELBlockInfoImage.PaddingProperty,
                    Value ? new(0, 0, 0, 7) : new(0), TimeSpan.FromMilliseconds(400d));
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