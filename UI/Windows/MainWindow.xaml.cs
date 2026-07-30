#region Link
using IEL.CORE.Enums;
using IEL.UserElementsControl;
using LibraryIEL.CORE.Themes.Palettes;
using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Enums.Language;
using OPLAPI.CORE.Internet;
using OperPageLes.CORE.Objects;
using OperPageLes.CORE.Settings.Struct;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel;
using OperPageLes.UI.Pages.ActionPanel.Other;
using OperPageLes.UI.Pages.Browser;
using OperPageLes.UI.Pages.Browser.InlayPages;
using OperPageLes.UI.Windows.Dialogs;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Language;
using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.UserElementsControl;
using OPLAPI.OIEL.UserElementsControl.Base;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        #region Loading
        /// <summary>
        /// Текущее состояние отображения загрузки процесса
        /// </summary>
        private bool IsVisualLoagingProcessInBorder;

        /// <summary>
        /// Состояние загрузки какого-либо процесса
        /// </summary>
        internal bool IsLoadingProcess { get; private set; }

        /// <summary>
        /// Количество загружаемых процессов
        /// </summary>
        private uint CountLoadingProcesses;

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
        #endregion

        #region WindowPages
        private PageSettingApp? _SettingApp = null;
        /// <summary>
        /// Страница управления настройками программы
        /// </summary>
        internal PageSettingApp SettingApp => _SettingApp ??= new();

        private PageLanguageGeneratorApp? _LanguageGeneratorApp = null;
        /// <summary>
        /// Страница управления файлами языковых переводов
        /// </summary>
        internal PageLanguageGeneratorApp LanguageGeneratorApp => _LanguageGeneratorApp ??= new();
        #endregion

        #region PanelActionPages
        /// <summary>
        /// Страница управления языковыпи переводами
        /// </summary>
        private PageLanguageController? SourcePageLanguageController = null;
        #endregion

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public override OPLAnimationManager? ManagerAnimation
        {
            get => base.ManagerAnimation;
            set
            {
                base.ManagerAnimation = value;
                SettingApp.ManagerAnimation = value;
                PageNotificationApplication.ManagerAnimation = value;
                VisualLoadingElement.ManagerAnimation = value;
                App.CurrentApp.SourcePlayControl.UpdateVisualElementsFromStackPanel(PageAudioControlApplication.StackPanelAudioDevices,
                    ManagerAnimation, PageAudioControlApplication.SetActiveDeviceOutput);
            }
        }

        /// <summary>
        /// Асинхронный процесс обновления визуальных данных каждую секунду
        /// </summary>
        private DispatcherTimer DispatcherTimerVisualData;

        public MainWindow()
        {
            App.LogWriteLine($"Инициализация \"{nameof(MainWindow)}\"...");
            #region Inicialize
            InitializeComponent();
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(DispatcherTimerVisualData)}\"...");
            #region DispatcherTimerVisualData
            DispatcherTimerVisualData = new()
            {
                Interval = TimeSpan.FromMilliseconds(1000d),
            };
            DispatcherTimerVisualData.Tick += HandlerUpdateVisualData;
            #endregion
            App.LogWriteLine("...Готово");

            #region SetParameteres
            Lang_LanguageUpdated(null, EventArgs.Empty);
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            BorderColorMain.BorderBrush = new SolidColorBrush(Colors.Black);

            App.LogWriteLine($"Настройка \"{nameof(App.GUIE_Browser)}\"...");
            #region GUIE_Browser
            App.GUIE_Browser.NewInicializedAppPage += MainBrowser_NewInicializedAppPage;
            App.GUIE_Browser.EventCloseInlay += (sender, e) =>
            {
                if (App.GUIE_PanelAction.PanelActionActivate) App.GUIE_PanelAction.ClosePanelAction();
            };
            GridContentFromBrowser.Children.Add(App.GUIE_Browser);
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(App.GUIE_PanelAction)}\"...");
            #region GUIE_PanelAction
            GridMain.Children.Add(App.GUIE_PanelAction);
            #endregion
            App.LogWriteLine("...Готово");

            TokenUpdateBackgroundData = new(false);

            TextBlockVersion.Text = App.CurrentApp.Version;

            App.LogWriteLine("...1");
            VisualLoadingElement.ManagerAnimation = ManagerAnimation;
            VisualLoadingElement.Opacity = 0d;
            TextBlockCountLoadingProcess.Opacity = 0d;

            ImageMenu.Opacity = 0d;
            App.LogWriteLine("...2");

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
            #endregion

            App.LogWriteLine($"Настройка \"{nameof(App.GUIE_Message)}\"...");
            #region GUIE_Message
            GridMain.Children.Add(App.GUIE_Message);
            App.GUIE_Message.Opacity = 0d;
            App.GUIE_Message.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            App.GUIE_Message.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            App.GUIE_Message.VerticalAlignment = VerticalAlignment.Top;
            App.GUIE_Message.VerticalContentAlignment = VerticalAlignment.Top;
            App.GUIE_Message.OffsetUpDown = 6u;
            App.GUIE_Message.OffsetLeftRight = 6u;
            App.GUIE_Message.DefaultBackground = WnColor.FromRgb(197, 251, 228);
            App.GUIE_Message.FontSize = 14d;
            App.GUIE_Message.RadiusDefault = 15u;
            App.GUIE_Message.RadiusMagnite = 2u;
            System.Windows.Data.Binding SourceBinding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["Deledda Open Regular"]
            };
            BindingOperations.SetBinding(App.GUIE_Message, IELBlockMessage.FontFamilyProperty, SourceBinding);
            Canvas.SetZIndex(App.GUIE_Message, -2);
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(App.GUIE_PanelAction)}\"...");
            #region IELPanelAction
            App.GUIE_PanelAction.Opacity = 0d;
            Canvas.SetZIndex(App.GUIE_PanelAction, -2);
            App.GUIE_PanelAction.EventMovePanelAction += (sender, e) =>
            {
                App.CurrentApp.SourcePlayControl.Play(nameof(OPRES.AudioMove));
            };
            App.GUIE_PanelAction.EventClosingPanelAction += (Name) =>
            {
                PageBrowser? Page = App.GUIE_Browser.ActualInlay?.Content;
                if (Page == null) return;
                switch (Page.GetType().Name)
                {
                    case "PageConsole":
                        ((PageConsole)Page).TextBoxCommandInput.Focus();
                        break;
                    default: return;
                }
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(PageNotificationApplication)}\"...");
            #region PageNotificationApplication
            PageNotificationApplication = new()
            {
                ManagerAnimation = ManagerAnimation,
            };
            NotificationIndicator.Opacity = App.CurrentApp.ApplicationNotifications.Count > 0 ? 1d : 0d;
            App.CurrentApp.AddNotification += (sender, e) =>
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, NotificationIndicator, OpacityProperty,
                    1d, TimeSpan.FromMilliseconds(400d));
            };
            App.CurrentApp.ClearNotification += (sender, e) =>
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, NotificationIndicator, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(400d));
            };
            #endregion
            App.LogWriteLine("...Готово");

            #region UpToolButtons
            App.LogWriteLine($"Настройка \"{nameof(ImageLogoApplication)}\"...");
            #region ImageLogoApplication
            ImageLogoApplication.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            ImageLogoApplication.OnActivateMouseLeft += (sender, e) =>
            {
                DialogLicenseWindow License = new();
                License.Show();
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELLangApplication)}\"...");
            #region IELLangApplication
            IELLangApplication.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.World));
            IELLangApplication.OnActivateMouseLeft += async (sender, e) =>
            {
                if (App.GUIE_PanelAction.PanelActionActivate && App.GUIE_PanelAction.ActualVisualPage?.GetType() == typeof(PageLanguageController))
                {
                    App.GUIE_PanelAction.AnimationMovePanelAction(PositionAnimActionPanel.Cursor, OrientationPositionCursor.RightDown, false);
                    return;
                }
                PageLanguageController Source = SourcePageLanguageController ?? new() { ManagerAnimation = ManagerAnimation };
                App.GUIE_Message.CloseBorderInformation();
                if (!App.GUIE_PanelAction.PanelActionActivate)
                    App.GUIE_PanelAction.OpenPanelAction(IELLangApplication, Source,
                    Orientation: OrientationPositionCursor.RightDown, DependencePointOnSize: false);
                else App.GUIE_PanelAction.MoveNextObjectPage(IELLangApplication, Source,
                    Orientation: OrientationPositionCursor.RightDown, DependencePointOnSize: false);
                if (SourcePageLanguageController == null)
                {
                    SourcePageLanguageController = Source;
                    StartVisualizateLoadingProcess();
                    await SourcePageLanguageController.UpdateListLanguages();
                    CompleteVisualizateLoadingProcess();
                }
            };
            IELLangApplication.MouseHover += (sender, e) =>
            {
                if (App.GUIE_PanelAction.ActualVisualPage?.GetType() != typeof(PageLanguageController))
                {
                    App.GUIE_Message.UsingBorderInformation(IELLangApplication, Lang.GetValue(LangUITranslate.LangTranslate),
                        OrientationPositionCursor.RightUp);
                }
            };
            IELLangApplication.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELButtonTheme)}\"...");
            #region IELButtonTheme
            IELButtonTheme.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Brush));
            IELButtonTheme.MouseHover += (sender, e) =>
            {
                App.GUIE_Message.UsingBorderInformation(IELButtonTheme,
                    Lang.GetValue(LangUITranslate.PersonalizationSetting),
                    OrientationPositionCursor.LeftDown);
            };
            IELButtonTheme.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            IELButtonTheme.OnActivateMouseLeft += (sender, e) =>
            {
                if (App.CurrentApp.ThemeApp == null)
                {
                    App.CurrentApp.ThemeApp = new()
                    {
                        ManagerAnimation = ManagerAnimation,
                        SourcePanelAction = App.GUIE_PanelAction
                    };
                    //App.CurrentApp.ThemeApp.LoadingThemes();
                }
                ActivateCustomPageBrowser(App.CurrentApp.ThemeApp);
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELButtonCollapse)}\"...");
            #region IELButtonCollapse
            IELButtonCollapse.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Collapse));
            IELButtonCollapse.OnActivateMouseLeft += (sender, e) =>
            {
                WindowState = WindowState.Minimized;
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELButtonClose)}\"...");
            #region IELButtonClose
            IELButtonClose.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
            IELButtonClose.OnActivateMouseLeft += async (sender, e) =>
            {
                await App.CurrentApp.CloseApplication();
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELButtonSettings)}\"...");
            #region IELButtonSettings
            IELButtonSettings.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainSettings));
            IELButtonSettings.MouseHover += (sender, e) =>
            {
                App.GUIE_Message.UsingBorderInformation(IELButtonSettings,
                    Lang.GetValue(LangUITranslate.ProgramSetting),
                    OrientationPositionCursor.LeftDown);
            };
            IELButtonSettings.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            IELButtonSettings.OnActivateMouseLeft += (sender, e) =>
            {
                ActivateCustomPageBrowser(App.CurrentApp.PageSettingApplication);
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELButtonBack)}\"...");
            #region IELButtonBack
            IELButtonBack.IsEnabled = false;
            IELButtonBack.Margin = new(0, 5, 6, 0);
            IELButtonBack.Width = 0d;
            IELButtonBack.OnActivateMouseLeft += (sender, e) =>
            {
                if (App.GUIE_PanelAction.PanelActionActivate)
                    App.GUIE_PanelAction.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                Disable_IELButtonBack();
                App.GUIE_Browser.GoBack();
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELOpenDataFolder)}\"...");
            #region IELOpenDataFolder
            IELOpenDataFolder.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Folder));
            IELOpenDataFolder.MouseEnter += (sender, e) =>
            {
                App.GUIE_Message.UsingBorderInformation(IELOpenDataFolder,
                    Lang.GetValue(LangUITranslate.MainDirectoryData),
                    OrientationPositionCursor.LeftDown);
            };
            IELOpenDataFolder.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            IELOpenDataFolder.OnActivateMouseLeft += (sender, e) =>
            {
                Process p = new();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/c start {StructDirectoryResources.MainDirectoryApplication}";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                if (App.GUIE_PanelAction.PanelActionActivate)
                    App.GUIE_PanelAction.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            };
            #endregion
            App.LogWriteLine("...Готово");
            #endregion

            #region DownToolButtons
            App.LogWriteLine($"Настройка \"{nameof(IELButtonInstallAppPage)}\"...");
            #region IELButtonInstallAppPage
            IELButtonInstallAppPage.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.File));
            IELButtonInstallAppPage.OnActivateMouseLeft += (sender, e) =>
            {
                OpenFileDialog dialog = new()
                {

                };
                dialog.ShowDialog();
                App.GUIE_Browser.AddNewAppPage(dialog.FileName);
            };
            IELButtonInstallAppPage.MouseEnter += (sender, e) =>
            {
                App.GUIE_Message.UsingBorderInformation(IELButtonInstallAppPage,
                    Lang.GetValue(LangUITranslate.InstallNewPageApp),
                    OrientationPositionCursor.RightUp);
            };
            IELButtonInstallAppPage.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELButtonAddLabel)}\"...");
            #region IELButtonAddLabel
            IELButtonAddLabel.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Plus));
            IELButtonAddLabel.OnActivateMouseLeft += (sender, e) =>
            {
                if (App.GUIE_Browser.SourceManagerAppPage == null) return;
                PageManagerAppPage AppPage = (PageManagerAppPage)App.GUIE_Browser.SourceManagerAppPage;
                App.GUIE_Message.CloseBorderInformation();
                DialogGenLabel dialog = new();
                SourceLabelAction? Result = dialog.CreateLabel();
                if (Result == null) return;
                AppPage.AddLabel(Result);
            };
            IELButtonAddLabel.MouseEnter += (sender, e) =>
            {
                App.GUIE_Message.UsingBorderInformation(IELButtonAddLabel,
                    Lang.GetValue(LangShortcutUITranslate.ShortcutCreateDescription),
                    OrientationPositionCursor.RightUp);
            };
            IELButtonAddLabel.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELButtonHomeBrowser)}\"...");
            #region IELButtonHomeBrowser
            IELButtonHomeBrowser.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Home));
            IELButtonHomeBrowser.OnActivateMouseLeft += (sender, e) =>
            {
                if (App.GUIE_PanelAction.PanelActionActivate)
                    App.GUIE_PanelAction.ClosePanelAction();
                if (IELButtonBack.IsEnabled)
                    Disable_IELButtonBack();
                App.GUIE_Browser.OpenManagerAppPage();
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELBlockInfoInternetConnection)}\"...");
            #region IELBlockInfoInternetConnection
            IELBlockInfoInternetConnection.IsEnabled = Connection.StateConnect;
            IELBlockInfoInternetConnection.Padding = new(0);
            IELBlockInfoInternetConnection.Source =
                StructDirectoryResources.GetResourceBitmap(Connection.StateConnect ? nameof(OPRES.WifiOn) : nameof(OPRES.WifiOff));
            IELBlockInfoInternetConnection.SourceBackground.SetActiveSpecrum(
                        Connection.StateConnect ? SpectrumColor.Default : SpectrumColor.NotEnabled, false);
            TextBlockInternetConnectionMillisecond.Opacity = 0d;
            IELBlockInfoInternetConnection.MouseEnter += (sender, e) =>
            {
                App.GUIE_Message.UsingBorderInformation(IELBlockInfoInternetConnection,
                    Lang.GetValue(LangUITranslate.CurrentInternetConnection),
                    OrientationPositionCursor.RightUp);
            };
            IELBlockInfoInternetConnection.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };

            App.LogWriteLine($"Настройка \"{nameof(IELBlockInfoInternetConnection)}.Connection\"...");
            #region Connection
            Connection.ConnectionChanged += (sender, e) =>
            {
                if (IELBlockInfoInternetConnection.IsEnabled != e)
                {
                    if (!e)
                    {
                        OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockInternetConnectionMillisecond, OpacityProperty,
                            0d, TimeSpan.FromMilliseconds(400d));
                        OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, IELBlockInfoInternetConnection, IELBlockInfoImage.PaddingProperty,
                            new Thickness(0d), TimeSpan.FromMilliseconds(400d));
                    }
                    IELBlockInfoInternetConnection.IsEnabled = e;
                    IELBlockInfoInternetConnection.SourceBackground.SetActiveSpecrum(
                        e ? SpectrumColor.Default : SpectrumColor.NotEnabled, true);
                    IELBlockInfoInternetConnection.Source =
                        StructDirectoryResources.GetResourceBitmap(e ? nameof(OPRES.WifiOn) : nameof(OPRES.WifiOff));
                }
            };
            Connection.PingChanged += (sender, e) =>
            {
                TextBlockInternetConnectionMillisecond.Text = $"{e}ms";
            };
            #endregion
            App.LogWriteLine("...Готово");

            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELBlockInfoStateRegister)}\"...");
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
                App.GUIE_Message.UsingBorderInformation(IELBlockInfoStateRegister,
                    Lang.GetValue(LangUITranslate.KeyboardCharacterCase),
                    OrientationPositionCursor.RightUp);
            };
            IELBlockInfoStateRegister.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELBlockInfoCurrentLanguage)}\"...");
            #region IELBlockInfoCurrentLanguage
            IELBlockInfoCurrentLanguage.Text = InputLanguage.CurrentInputLanguage.Culture.NativeName[..3].ToUpper();
            InputLanguageManager.Current.InputLanguageChanged += (sender, e) =>
            {
                IELBlockInfoCurrentLanguage.Text = e.NewLanguage.NativeName[..3].ToUpper();
            };
            IELBlockInfoCurrentLanguage.MouseEnter += (sender, e) =>
            {
                App.GUIE_Message.UsingBorderInformation(IELBlockInfoCurrentLanguage,
                    Lang.GetValue(LangUITranslate.CurrentKeyboardLayout),
                    OrientationPositionCursor.RightUp);
            };
            IELBlockInfoCurrentLanguage.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(IELBlockInfoVolume)}\"...");
            #region IELBlockInfoVolume
            PageAudioControlApplication = new();
            IELBlockInfoVolume.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Volume));
            IELBlockInfoVolume.MouseHover += (sender, e) =>
            {
                if (App.GUIE_PanelAction.PanelActionActivate && App.GUIE_PanelAction.ActualVisualPage is PageAudioControl) return;
                App.GUIE_Message.UsingBorderInformation(IELBlockInfoVolume,
                    Lang.GetValue(LangUITranslate.AudioСontrol),
                    OrientationPositionCursor.RightUp);
            };
            IELBlockInfoVolume.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            IELBlockInfoVolume.MouseRightButtonUp += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
                App.GUIE_PanelAction.UsingPanelAction(IELBlockInfoVolume, PageAudioControlApplication,
                    Orientation: OrientationPositionCursor.RightUp,
                    DependencePointOnSize: false);
            };
            IELBlockInfoVolume.MouseLeftButtonUp += (sender, e) =>
            {
                App.CurrentApp.SourcePlayControl.Play(nameof(OPRES.AudioPopUp));
            };
            //App.CurrentApp.SettingMainApplication.Volume.Changed += (Old, New) =>
            //{
            //    EllipseLittle.Opacity = Math.Min(New / 33d, 1d);
            //    EllipseMiddle.Opacity = Math.Min((New - 33) / 33d, 1d);
            //    EllipseBig.Opacity = Math.Min((New - 66) / 33d, 1d);
            //};
            //EllipseLittle.Opacity = Math.Min(App.CurrentApp.SettingMainApplication.Volume / 33d, 1d);
            //EllipseMiddle.Opacity = Math.Min((App.CurrentApp.SettingMainApplication.Volume - 33) / 33d, 1d);
            //EllipseBig.Opacity = Math.Min((App.CurrentApp.SettingMainApplication.Volume - 66) / 33d, 1d);
            #endregion
            App.LogWriteLine("...Готово");

            App.LogWriteLine($"Настройка \"{nameof(BorderIndicator)}\"...");
            #region BorderIndicator
            BorderIndicator.MouseEnter += (sender, e) =>
            {
                if (App.GUIE_PanelAction.PanelActionActivate && App.GUIE_PanelAction.ActualVisualPage is PageNotificationManager) return;
                App.GUIE_Message.UsingBorderInformation(BorderIndicator,
                    Lang.GetValue(LangUITranslate.NotificationManager),
                    OrientationPositionCursor.LeftUp);
            };
            BorderIndicator.MouseLeave += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
            };
            BorderIndicator.MouseRightButtonUp += (sender, e) =>
            {
                App.GUIE_Message.CloseBorderInformation();
                App.GUIE_PanelAction.UsingPanelAction(BorderIndicator, PageNotificationApplication,
                    Orientation: OrientationPositionCursor.LeftUp,
                    DependencePointOnSize: false);
            };
            #endregion
            App.LogWriteLine("...Готово");
            #endregion

            App.LogWriteLine($"Настройка \"EventsWindow\"...");
            #region EventsWindow
            SizeChanged += (sender, e) =>
            {
                if (App.GUIE_PanelAction.PanelActionActivate)
                    App.GUIE_PanelAction.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            };
            Closing += (sender, e) =>
            {
                //App.CurrentApp.SettingMainApplication.MainWindowWidth.Value = Width;
                //App.CurrentApp.SettingMainApplication.MainWindowHeight.Value = Height;
                TokenUpdateBackgroundData.ThrowIfCancellationRequested();
            };
            Lang.LanguageUpdated += Lang_LanguageUpdated;
            #endregion
            App.LogWriteLine("...Готово");

            #endregion
            App.LogWriteLine("! Инициализация успешна");
        }

        /// <summary>
        /// Обработчик собфтия изменения языкового перевода
        /// </summary>
        private void Lang_LanguageUpdated(object? sender, EventArgs e)
        {
            Title = Lang.GetValue(LangUITranslate.MainWindowTitle);
            IELButtonBack.Text = Lang.GetValue(LangUITranslate.Back);
        }

        #region BrowserPageControl
        private void MainBrowser_NewInicializedAppPage(object? sender, OPLInlay e)
        {
            if (App.GUIE_PanelAction.PanelActionActivate)
                App.GUIE_PanelAction.ClosePanelAction(PositionAnimActionPanel.CenterObject);

            e.MouseHover += (sender, e) =>
            {
                if (sender == null) return;
                OPLInlay Source = (OPLInlay)sender;
                if (Source.Content.Description.Length > 0)
                    App.GUIE_Message.UsingBorderInformation(Source, Source.Content.Description,
                        OrientationPositionCursor.Auto);
            };
            e.MouseLeave += (sender, e) =>
            {
                if (App.GUIE_Message.FlagMessage)
                    App.GUIE_Message.CloseBorderInformation();
            };

            IELButtonImage ButtonClose = e.GetButtonCloseInlay();
            ButtonClose.MarginViewBox = new(0d);
            //ButtonClose.Palette = App.CurrentApp.ActiveThemeApplication[PaletteEnum.Red];
            ButtonClose.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
        }
        #endregion

        ///// <summary>
        ///// Присвоить кнопкам цвет в завимисости от темы
        ///// </summary>
        ///// <param name="SourceTheme">Тема</param>
        //internal void SetPallete(in Theme SourceTheme)
        //{
        //    SourceTheme[PaletteEnum.Lime].ConnectPalleteFromIELElement(ImageLogoApplication);
        //    SourceTheme[PaletteEnum.PlumCrayola].ConnectPalleteFromIELElement(IELButtonBack);
        //    SourceTheme[PaletteEnum.Green].ConnectPalleteFromIELElement(IELOpenDataFolder);
        //    SourceTheme[PaletteEnum.Chocolate].ConnectPalleteFromIELElement(IELButtonTheme);
        //    SourceTheme[PaletteEnum.Purple].ConnectPalleteFromIELElement(IELButtonSettings);
        //    SourceTheme[PaletteEnum.LightBlue].ConnectPalleteFromIELElement(IELButtonCollapse);
        //    SourceTheme[PaletteEnum.Red].ConnectPalleteFromIELElement(IELButtonClose);
        //    SourceTheme[PaletteEnum.Green].ConnectPalleteFromIELElement(IELBlockInfoInternetConnection);
        //    SourceTheme[PaletteEnum.Saffron].ConnectPalleteFromIELElement(IELBlockInfoStateRegister);
        //    SourceTheme[PaletteEnum.Violet].ConnectPalleteFromIELElement(IELBlockInfoCurrentLanguage);
        //    SourceTheme[PaletteEnum.Jade].ConnectPalleteFromIELElement(IELBlockInfoVolume);
        //    SourceTheme[PaletteEnum.Cocoa].ConnectPalleteFromIELElement(App.GUIE_PanelAction);
        //    SourceTheme[PaletteEnum.PlumCrayola].ConnectPalleteFromIELElement(IELButtonHomeBrowser);
        //    SourceTheme[PaletteEnum.BlueGreenCrayola].ConnectPalleteFromIELElement(IELButtonAddLabel);
        //    SourceTheme[PaletteEnum.LightBlue].ConnectPalleteFromIELElement(IELButtonInstallAppPage);
        //}

        /// <summary>
        /// Установить настройки для данной формы
        /// </summary>
        /// <param name="Setting">Объект настроек</param>
        internal void ChangeFromSetting(in SettingApplication Setting)
        {
            App.GUIE_PanelAction.IsKeyboardModeExit = Setting.ExitKeyboardModeInClosePanelAction;
            App.GUIE_PanelAction.KeyActivateKeyboardMode = Setting.KEY_KeyboardModePanelAction;
            App.GUIE_PanelAction.KeyKeyboardModeActivateRightClick = Setting.KEY_PanelActionRightClick;
            App.GUIE_PanelAction.KeyCloseElement = Setting.KEY_PanelActionClose;
            Width = Setting.MainWindowWidth;
            Height = Setting.MainWindowHeight;
            UpdateImageMenu(Setting.PathMenuImage);

            Setting.LoadingBorderVisualizate.ValueChanged += (Old, New) =>
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
            if (App.GUIE_PanelAction.PanelActionActivate)
                App.GUIE_PanelAction.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            if (App.GUIE_Browser.ActualPage != null)
                Enable_IELButtonBack();
            App.GUIE_Browser.ActivateCustomPageBrowser(SourcePage, RightAlign);
        }

        #region ManipulateWindow

        /// <summary>
        /// Собственная функция отображения главного окна
        /// </summary>
        public new async Task Show()
        {
            DispatcherTimerVisualData.Start();
            Opacity = 0d;
            base.Show();
            App.GUIE_Browser.OpenManagerAppPage();
            await ((PageManagerAppPage?)App.GUIE_Browser.SourceManagerAppPage)?.AddLabelsFromJSON(StructDirectoryResources.DirectoryDataLabels);
        }

        /// <summary>
        /// Собственная функция закрытия главного окна
        /// </summary>
        public new void Close()
        {
            DispatcherTimerVisualData.Stop();
            base.Close();
        }
        #endregion

        #region LoadingManipulate
        /// <summary>
        /// Начало визуализации загрузки
        /// </summary>
        internal void StartVisualizateLoadingProcess()
        {
            if (!IsLoadingProcess)
            {
                IsLoadingProcess = true;
                //if (App.CurrentApp.SettingMainApplication.LoadingBorderVisualizate)
                //{
                //    BeginRotateBorder();
                //    IsVisualLoagingProcessInBorder = true;
                //}
                VisualLoadingElement.OpenLoading();
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockCountLoadingProcess,
                    OpacityProperty, 1d, TimeSpan.FromMilliseconds(400d));
            }
            CountLoadingProcesses++;
            TextBlockCountLoadingProcess.Text = CountLoadingProcesses.ToString();
        } 

        /// <summary>
        /// Завершение визуализации загрузки
        /// </summary>
        internal void CompleteVisualizateLoadingProcess()
        {
            if (IsLoadingProcess)
            {
                CountLoadingProcesses--;
                TextBlockCountLoadingProcess.Text = CountLoadingProcesses.ToString();
                if (CountLoadingProcesses > 0u) return;
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

        #endregion

        /// <summary>
        /// Обработчик обновления визуальной информации в данном окне
        /// </summary>
        private void HandlerUpdateVisualData(object? sender, EventArgs e)
        {
            TextBlockTime.Text = App.RealTime.ToShortTimeString();
            TextBlockData.Text = App.RealTime.ToShortDateString();
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
                        StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Warning)));
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
            ((SolidColorBrush)BorderColorMain.BorderBrush).Color = Color;
            BorderColorMain.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderColorMain.BorderBrush, SolidColorBrush.ColorProperty,
                Colors.Black, TimeSpan.FromMilliseconds(1300d));
        }
        #endregion
    }
}