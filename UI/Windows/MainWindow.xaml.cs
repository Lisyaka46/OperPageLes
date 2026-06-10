#region Link
using IEL.CORE.Enums;
using IEL.UserElementsControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.UserElementsControl;
using OperPageLes.CORE.Audio;
using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Objects;
using OperPageLes.CORE.Settings.PaletteElements;
using OperPageLes.CORE.Settings.Struct;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel;
using OperPageLes.UI.Pages.ActionPanel.Other;
using OperPageLes.UI.Pages.Browser;
using OperPageLes.UI.Pages.Browser.BrowserPageNetwork;
using OperPageLes.UI.Pages.Browser.InlayPages;
using OperPageLes.UI.Windows.Base;
using OperPageLes.UI.Windows.Dialogs;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        [LibraryImport("user32.dll", EntryPoint = "keybd_event")]
        private static partial void Keybd_event(byte CodeButton, byte CodeScan, uint CodeState, UIntPtr dwExtralnfo);

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

        /// <summary>
        /// 
        /// </summary>
        private readonly OPLBrowserPage MainBrowser;

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

        #region WindowPages
        private PageSettingApp? _SettingApp = null;
        /// <summary>
        /// Страница управления настройками программы
        /// </summary>
        internal PageSettingApp SettingApp => _SettingApp ??= new();
        #endregion

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public override OPLAnimationManager? ManagerAnimation
        {
            get => SourceManagerAnimation;
            set
            {
                base.ManagerAnimation = value;
                SettingApp.ManagerAnimation = value;
                App.CurrentApp.SourcePlayControl.UpdateVisualElementsFromStackPanel(PageAudioControlApplication.StackPanelAudioDevices,
                    ManagerAnimation, PageAudioControlApplication.SetActiveDeviceOutput);
            }
        }

        public MainWindow()
        {
            App.CurrentApp.LogWriteLine("Инициализация компонентов...");
            InitializeComponent();
            App.CurrentApp.LogWriteLine("...Готово");

            #region SetParameteres
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            ImageLogoApplication.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            IELOpenDataFolder.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Folder));
            IELButtonTheme.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Brush));
            IELButtonSettings.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainSettings));
            IELButtonCollapse.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Collapse));
            IELButtonClose.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));

            App.CurrentApp.LogWriteLine("Настройка логики главного браузера страниц в главном окне...");
            #region BrowserPage
            MainBrowser = App.CurrentApp.MainBrowser;
            MainBrowser.NewInicializedAppPage += MainBrowser_NewInicializedAppPage;
            MainBrowser.EventCloseInlay += (sender, e) =>
            {
                if (IELActionPanelMain.PanelActionActivate) IELActionPanelMain.ClosePanelAction();
            };
            GridContentFromBrowser.Children.Add(MainBrowser);
            #endregion
            App.CurrentApp.LogWriteLine("...Готово");

            TokenUpdateBackgroundData = new(false);

            TextBlockVersion.Text = App.CurrentApp.Version;

            App.CurrentApp.LogWriteLine("...1");
            NotificationIndicator.Opacity = 0d;
            VisualLoadingElement.ManagerAnimation = ManagerAnimation;
            VisualLoadingElement.Opacity = 0d;
            TextBlockCountLoadingProcess.Opacity = 0d;

            IELMessageMain.Opacity = 0d;
            IELActionPanelMain.Opacity = 0d;
            ImageMenu.Opacity = 0d;
            IELActionPanelMain.EventMovePanelAction += (sender, e) =>
            {
                App.CurrentApp.SourcePlayControl.Play(nameof(OPRES.AudioMove));
            };
            App.CurrentApp.LogWriteLine("...2");

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

            App.CurrentApp.LogWriteLine("...2-1");
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
            App.CurrentApp.LogWriteLine("...3");

            //#region PageInlay
            //PageInlay.IELButtonPageOpenInlay.OnActivateMouseLeft += (sender, e, Key) =>
            //{
            //    if (PageInlay.ActivateManipulateInlay?.Content != null)
            //        App.CurrentApp.MainBrowser.ActivateInlayInBrowserPage(PageInlay.ActivateManipulateInlay.Content);
            //};
            //PageInlay.IELButtonPageDeleteInlay.OnActivateMouseLeft += (sender, e, Key) =>
            //{
            //    IELActionPanelMain.ClosePanelAction();
            //    if (PageInlay.ActivateManipulateInlay != null)
            //        App.CurrentApp.MainBrowser.DeleteInlayPage(PageInlay.ActivateManipulateInlay);
            //};
            //#endregion

            PageNotificationApplication = new();
            //App.CurrentApp.AddNotification += (sender, e) =>
            //{
            //    App.CurrentApp.ManagerAnimation.DoubleAnimationType.AnimateEffect(NotificationIndicator, OpacityProperty, 1d, TimeSpan.FromMilliseconds(100d));
            //};
            //App.CurrentApp.ClearNotification += (sender, e) =>
            //{
            //    App.CurrentApp.ManagerAnimation.DoubleAnimationType.AnimateEffect(NotificationIndicator, OpacityProperty, 0d, TimeSpan.FromMilliseconds(100d));
            //};

            #endregion
            App.CurrentApp.LogWriteLine("...4");

            #region UpToolButtons

            #region ImageLogoApplication
            ImageLogoApplication.OnActivateMouseLeft += (sender, e) =>
            {
                DialogLicenseWindow License = new();
                License.Show();
            };
            #endregion
            App.CurrentApp.LogWriteLine("...4-1");

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
                        //ManagerAnimation = App.CurrentApp.ManagerAnimation,
                        SourcePanelAction = IELActionPanelMain
                    };
                    App.CurrentApp.ThemeApp.LoadingThemes();
                }
                ActivateCustomPageBrowser(App.CurrentApp.ThemeApp);
            };
            #endregion
            App.CurrentApp.LogWriteLine("...4-2");

            #region IELImageButtonCollapse
            IELButtonCollapse.OnActivateMouseLeft += (sender, e) =>
            {
                WindowState = WindowState.Minimized;
            };
            #endregion
            App.CurrentApp.LogWriteLine("...4-3");

            #region IELImageButtonClose
            IELButtonClose.OnActivateMouseLeft += (sender, e) =>
            {
                Close();
            };
            #endregion
            App.CurrentApp.LogWriteLine("...4-4");

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
                ActivateCustomPageBrowser(SettingApp);
            };
            #endregion
            App.CurrentApp.LogWriteLine("...4-5");

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
            App.CurrentApp.LogWriteLine("...4-6");

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
            App.CurrentApp.LogWriteLine("...4-7");

            #endregion
            App.CurrentApp.LogWriteLine("...5");

            #region DownToolButtons

            #region IELButtonAddLabel
            IELButtonAddLabel.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Plus));
            IELButtonAddLabel.OnActivateMouseLeft += (sender, e) =>
            {
                if (MainBrowser.SourceManagerAppPage == null) return;
                PageManagerAppPage AppPage = (PageManagerAppPage)MainBrowser.SourceManagerAppPage;
                IELMessageMain.CloseBorderInformation();
                DialogGenLabel dialog = new();
                SourceLabelAction? Result = dialog.CreateLabel();
                if (Result == null) return;
                AppPage.AddLabel(Result);
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
            App.CurrentApp.LogWriteLine("...5-1");

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
            App.CurrentApp.LogWriteLine("...5-2");

            #region IELBlockInfoInternetConnection
            IELBlockInfoInternetConnection.IsEnabled = false;
            IELBlockInfoInternetConnection.Padding = new(0);
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
            App.CurrentApp.LogWriteLine("...\\/");
            App.CurrentApp.ConnectionPingChanged += (sender, e) =>
            {
                TextBlockInternetConnectionMillisecond.Text = e.Connect ? $"{e.Ping}ms" : string.Empty;
                if (IELBlockInfoInternetConnection.IsEnabled != e.Connect)
                {
                    if (!e.Connect)
                    {
                        OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockInternetConnectionMillisecond, OpacityProperty,
                            0d, TimeSpan.FromMilliseconds(400d));
                        OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IELBlockInfoInternetConnection, IELBlockInfoImage.PaddingProperty,
                            new Thickness(0d), TimeSpan.FromMilliseconds(400d));
                    }
                    else if (App.CurrentApp.SettingMainApplication.MillisecondInternetConnection)
                    {
                        OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockInternetConnectionMillisecond, OpacityProperty,
                            1d, TimeSpan.FromMilliseconds(400d));
                        OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IELBlockInfoInternetConnection, IELBlockInfoImage.PaddingProperty,
                            new Thickness(0d, 0d, 0d, 7d), TimeSpan.FromMilliseconds(400d));
                    }
                    IELBlockInfoInternetConnection.IsEnabled = e.Connect;
                    IELBlockInfoInternetConnection.SourceBackground.SetActiveSpecrum(
                        e.Connect ? StateSpectrum.Default : StateSpectrum.NotEnabled, true);
                    IELBlockInfoInternetConnection.Source =
                        StructDirectoryResources.GetResourceBitmap(e.Connect ? nameof(OPRES.WifiOn) : nameof(OPRES.WifiOff));
                }
            };
            #endregion
            App.CurrentApp.LogWriteLine("...5-3");

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
            App.CurrentApp.LogWriteLine("...5-4");

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
            App.CurrentApp.LogWriteLine("...5-5");

            #region IELBlockInfoVolume
            PageAudioControlApplication = new();
            IELBlockInfoVolume.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Volume));
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
                App.CurrentApp.SourcePlayControl.Play(nameof(OPRES.AudioPopUp));
            };
            App.CurrentApp.SettingMainApplication.Volume.Changed += (Old, New) =>
            {
                EllipseLittle.Opacity = Math.Min(New / 33d, 1d);
                EllipseMiddle.Opacity = Math.Min((New - 33) / 33d, 1d);
                EllipseBig.Opacity = Math.Min((New - 66) / 33d, 1d);
            };
            EllipseLittle.Opacity = Math.Min(App.CurrentApp.SettingMainApplication.Volume / 33d, 1d);
            EllipseMiddle.Opacity = Math.Min((App.CurrentApp.SettingMainApplication.Volume - 33) / 33d, 1d);
            EllipseBig.Opacity = Math.Min((App.CurrentApp.SettingMainApplication.Volume - 66) / 33d, 1d);
            #endregion
            App.CurrentApp.LogWriteLine("...5-6");

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
            App.CurrentApp.LogWriteLine("...5-7");

            #endregion
            App.CurrentApp.LogWriteLine("...6");

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
            App.CurrentApp.LogWriteLine("! Инициализация успешна");
        }

        private void MainBrowser_NewInicializedAppPage(object? sender, OPLInlay e)
        {
            if (IELActionPanelMain.PanelActionActivate)
                IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);

            e.MouseHover += (sender, e) =>
            {
                if (sender == null) return;
                OPLInlay Source = (OPLInlay)sender;
                if (Source.Content.Description.Length > 0)
                    IELMessageMain.UsingBorderInformation(Source, Source.Content.Description,
                        OrientationPositionCursor.Auto);
            };
            e.MouseLeave += (sender, e) =>
            {
                if (IELMessageMain.FlagMessage)
                    IELMessageMain.CloseBorderInformation();
            };

            IELButtonImage ButtonClose = e.GetButtonCloseInlay();
            ButtonClose.MarginViewBox = new(0d);
            ButtonClose.PaletteElement = App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Red];
            ButtonClose.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
        }

        /// <summary>
        /// Присвоить кнопкам цвет в завимисости от темы
        /// </summary>
        /// <param name="SourceTheme">Тема</param>
        internal void SetPallete(in Theme SourceTheme)
        {
            SourceTheme[PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(ImageLogoApplication);
            SourceTheme[PaletteSpectrumEnum.PlumCrayola].ConnectPalleteFromIELElement(IELButtonBack);
            SourceTheme[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELOpenDataFolder);
            SourceTheme[PaletteSpectrumEnum.Chocolate].ConnectPalleteFromIELElement(IELButtonTheme);
            SourceTheme[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonSettings);
            SourceTheme[PaletteSpectrumEnum.LightBlue].ConnectPalleteFromIELElement(IELButtonCollapse);
            SourceTheme[PaletteSpectrumEnum.Red].ConnectPalleteFromIELElement(IELButtonClose);
            SourceTheme[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELBlockInfoInternetConnection);
            SourceTheme[PaletteSpectrumEnum.Saffron].ConnectPalleteFromIELElement(IELBlockInfoStateRegister);
            SourceTheme[PaletteSpectrumEnum.Violet].ConnectPalleteFromIELElement(IELBlockInfoCurrentLanguage);
            SourceTheme[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELBlockInfoVolume);
            SourceTheme[PaletteSpectrumEnum.Cocoa].ConnectPalleteFromIELElement(IELActionPanelMain);
            SourceTheme[PaletteSpectrumEnum.PlumCrayola].ConnectPalleteFromIELElement(IELButtonHomeBrowser);
            SourceTheme[PaletteSpectrumEnum.BlueGreenCrayola].ConnectPalleteFromIELElement(IELButtonAddLabel);
        }

        /// <summary>
        /// Установить настройки для данной формы
        /// </summary>
        /// <param name="Setting">Объект настроек</param>
        internal void ChangeFromSetting(in SettingApplication Setting)
        {
            IELActionPanelMain.IsKeyboardModeExit = Setting.ExitKeyboardModeInClosePanelAction;
            IELActionPanelMain.KeyActivateKeyboardMode = Setting.KEY_KeyboardModePanelAction;
            IELActionPanelMain.KeyKeyboardModeActivateRightClick = Setting.KEY_PanelActionRightClick;
            IELActionPanelMain.KeyCloseElement = Setting.KEY_PanelActionClose;
            Width = Setting.MainWindowWidth;
            Height = Setting.MainWindowHeight;
            UpdateImageMenu(Setting.PathMenuImage);

            Setting.LoadingBorderVisualizate.Changed += (Old, New) =>
            {
                if (!IsVisualLoagingProcessInBorder) EndRotateBorder(New ? 1 : -1);
            };
        }

        #region IELButtonBackControl
        /// <summary>
        /// Активировать/Показать кнопку возврата назад
        /// </summary>
        private void Enable_IELButtonBack()
        {
            IELButtonBack.IsEnabled = true;
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IELButtonBack, WidthProperty,
                80d, TimeSpan.FromMilliseconds(400d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IELButtonBack, MarginProperty,
                new Thickness(5d, 5d, 6d, 5d), TimeSpan.FromMilliseconds(400d));
        }

        /// <summary>
        /// Диактивировать/Скрыть кнопку возврата назад
        /// </summary>
        private void Disable_IELButtonBack()
        {
            IELButtonBack.IsEnabled = false;
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IELButtonBack, WidthProperty,
                0d, TimeSpan.FromMilliseconds(400d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IELButtonBack, MarginProperty,
                new Thickness(0d, 5d, 6d, 0d), TimeSpan.FromMilliseconds(400d));
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
            if (MainBrowser.ActualPage != null)
                Enable_IELButtonBack();
            MainBrowser.ActivateCustomPageBrowser(SourcePage, RightAlign);
        }

        #region ManipulateWindow
        /// <summary>
        /// Закрыть главное окно приложения без перезагрузки
        /// </summary>
        public new void Close()
        {
            IsClosing = true;
            Hide();
            App.CurrentApp.SettingMainApplication.MainWindowWidth.Value = Width;
            App.CurrentApp.SettingMainApplication.MainWindowHeight.Value = Height;
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
                PageManagerAppPage AppPage = (PageManagerAppPage?)MainBrowser.SourceManagerAppPage ??
                    throw new Exception("Главная страница браузера не инициализирована!");
                string SettingApplicationJSON = JsonConvert.SerializeObject(AppPage.Labels.Select((i) => i.Label));
                File.WriteAllText(StructDirectoryResources.DirectoryDataLabels, SettingApplicationJSON);
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
            //App.CurrentApp.LogWriteLine($"...-1 {App.ApplicationPageDeveloper != null}");
            //TextBlock StackUpdateData = Dispatcher.Invoke(() => App.ApplicationPageDeveloper.AddNewStackTextBlock("Task: Обновление данных"));
            App.CurrentApp.LogWriteLine("...-1-0");
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    //Dispatcher.Invoke(() => OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, (SolidColorBrush)StackUpdateData.Foreground,
                    //    SolidColorBrush.ColorProperty, Colors.LightGreen, Colors.Black, TimeSpan.FromMilliseconds(300d)));
                    await BackgroundUpdateVisualData();
                    Thread.Sleep(1000);
                }
            }, TokenUpdateBackgroundData);
            App.CurrentApp.LogWriteLine("...-1-1");
            Opacity = 0d;
            App.CurrentApp.LogWriteLine("...0");
            base.Show();
            App.CurrentApp.LogWriteLine("...1");
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockInternetConnectionMillisecond, OpacityProperty,
                App.CurrentApp.SettingMainApplication.MillisecondInternetConnection ? 1d : 0d, TimeSpan.FromMilliseconds(800d));
            App.CurrentApp.LogWriteLine("...2");
            #region MainBrowser
            MainBrowser.GenerateNewMainManagerAppPage(typeof(PageManagerAppPage));
            MainBrowser.OpenManagerAppPage();
            _ = ((PageManagerAppPage?)MainBrowser.SourceManagerAppPage)?.AddLabelsFromJSON(StructDirectoryResources.DirectoryDataLabels);

            #region AppPage
            MainBrowser.AddNewAppPage(typeof(PageConsole), "Консоль",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.LightBlue], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Command)));
            MainBrowser.AddNewAppPage(typeof(PageNetwork), "Сеть",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Chats)));
            MainBrowser.AddNewAppPage(typeof(PageWebBrowser), "Веб-браузер",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Aquamarine], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.World)));
            MainBrowser.AddNewAppPage(typeof(PageDeveloper), "Для разработчиков",
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelRed], StructDirectoryResources.GetResourceBitmap(nameof(OPRES.ValidKeyIcon)));
            #endregion

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
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockCountLoadingProcess,
                    OpacityProperty, 1d, TimeSpan.FromMilliseconds(400d));
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
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockCountLoadingProcess,
                    OpacityProperty, 0d, TimeSpan.FromMilliseconds(400d));
            }
        }

        #region VisualBorderLoading
        /// <summary>
        /// Начать анимацию поворота барьера
        /// </summary>
        private void BeginRotateBorder()
        {
            if (ManagerAnimation != null)
            {
                DoubleAnimation animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animation.From = 0d;
                animation.To = 3600d;
                animation.RepeatBehavior = RepeatBehavior.Forever;
                animation.EasingFunction = null;
                animation.Duration = TimeSpan.FromMilliseconds(30000d);
                RotateMainWindowBackground.BeginAnimation(RotateTransform.AngleProperty, animation);
            }
        }

        /// <summary>
        /// Закончить анимацию барьера
        /// </summary>
        /// <param name="FromValue">Стартовое значение анимирования</param>
        private void EndRotateBorder(int FullCountRotate = 1)
        {
            if (ManagerAnimation != null)
            {
                DoubleAnimation animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animation.From = RotateMainWindowBackground.Angle % 360;
                animation.To = 361d * FullCountRotate;
                animation.Duration = TimeSpan.FromMilliseconds(3200d);
                RotateMainWindowBackground.BeginAnimation(RotateTransform.AngleProperty, animation);
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
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ImageMenu,
                    ImageBrush.OpacityProperty, 0d, TimeSpan.FromMilliseconds(2300d));
            }
        }

        /// <summary>
        /// Успешная установка картинки фона
        /// </summary>
        /// <param name="bitmap">Карта изображения</param>
        private void ComplitedInstallImageMenu(BitmapImage bitmap)
        {
            ImageMenu.ImageSource = bitmap;

            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, ImageMenu, ImageBrush.ViewboxProperty,
                new Rect(0.025d, 0.025d, 0.95d, 0.95d), new Rect(0d, 0d, 1d, 1d), TimeSpan.FromMilliseconds(2300d));
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, ImageMenu, MarginProperty,
                new Thickness(-4d), new Thickness(0d), TimeSpan.FromMilliseconds(2300d));
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, ImageMenu, ImageBrush.OpacityProperty,
                0d, 1d, TimeSpan.FromMilliseconds(1500d));
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, ImageMenu, OpacityProperty,
                0d, 1d, TimeSpan.FromMilliseconds(2300d));
        }

        internal void ChangeVisibilityMillisecondInternet(bool Value)
        {
            if (IELBlockInfoInternetConnection.IsEnabled)
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockInternetConnectionMillisecond, OpacityProperty,
                    Value ? 1d : 0d, TimeSpan.FromMilliseconds(400d));
            }
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IELBlockInfoInternetConnection, IELBlockInfoImage.PaddingProperty,
                    Value ? new Thickness(0d, 0d, 0d, 7d) : new Thickness(0d), TimeSpan.FromMilliseconds(400d));
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