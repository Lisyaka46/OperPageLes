using IEL.CORE.Classes;
using IEL.CORE.Enums;
using IEL.GUI;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.Pages.ActionPanel.Other;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using static ApplicationOperPageLes.App;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.Pages.PanelButtonInformation.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class MainPageButtonInfo : Page
    {
        [LibraryImport("user32.dll", EntryPoint = "keybd_event")]
        private static partial void Keybd_event(byte CodeButton, byte CodeScan, uint CodeState, UIntPtr dwExtralnfo);

        /// <summary>
        /// Узнать открыт ли объект сообщения в данном объекте по имени
        /// </summary>
        /// <param name="NameObject">Имя объекта проверки</param>
        /// <returns>Состояние открыт ли объект или нет</returns>
        private static bool CheckOpenMessageInObject(string NameObject)
        {
            try
            {
                if (App.MainWindow == null) return false;
                return App.MainWindow.IELMessageMain.FlagMessage &&
                App.MainWindow.IELMessageMain.CodeParentObject.Equals(NameObject);
            }
            catch { return false; }
        }

        /// <summary>
        /// Поток обновляемый данные интернета
        /// </summary>
        internal Thread ThreadInternetConnection;

        /// <summary>
        /// Настройка отображения страницы настройки громкости в панели действий
        /// </summary>
        private readonly PageSettingVisual SettingVisualVolume;

#if DEBUG
        private readonly TextBlock DEV_InternetMillisecond;
#endif

        public MainPageButtonInfo()
        {
            bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;
            InitializeComponent();
#if DEBUG
            DEV_InternetMillisecond = CurrentApp.Is_WindowDeveloper.BlockInlays[1].AddNewTextElement();
#endif
            #region Palette
            App.CurrentApp.SettingPaletteApplication.SourcePalette[CORE.Enums.PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELBlockInfoInternetConnection);

            App.CurrentApp.SettingPaletteApplication.SourcePalette[CORE.Enums.PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELBlockInfoStateRegister);

            App.CurrentApp.SettingPaletteApplication.SourcePalette[CORE.Enums.PaletteSpectrumEnum.Violet].ConnectPalleteFromIELElement(IELBlockInfoCurrentLanguage);

            App.CurrentApp.SettingPaletteApplication.SourcePalette[CORE.Enums.PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(IELBlockInfoVolume);
            TextBlockVolumeValue.Foreground = IELBlockInfoVolume.SourceForeground.InicializeConnectedSolidColorBrush();
            #endregion

            SettingVisualVolume = new(GridMain, new PageVolumeControl(), new System.Windows.Size(150, 36));
            TextBlockVolumeValue.Text = ((int)(App.CurrentApp.SettingMainApplication.Volume * 100)).ToString();
            App.CurrentApp.SettingMainApplication.Volume.Changed += (Old, New) =>
            {
                TextBlockVolumeValue.Text = ((int)(New * 100)).ToString();
            };

            IndicatorLoadingInternetConnection.Source = new Uri(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingInternet)));
            IndicatorLoadingInternetConnection.MediaEnded += (sender, e) =>
            {
                IndicatorLoadingInternetConnection.Position = TimeSpan.FromMilliseconds(1);
            };

            //UpdateInformationInObject(IELBlockInfoCurrentLanguage, InputLanguage.CurrentInputLanguage.Culture.EnglishName[0..3].ToUpper());
            UpdateInformationInObject(IELBlockInfoStateRegister, Console.CapsLock ? "а".ToUpper() : "a".ToLower());
            IELBlockInfoStateRegister.Focusable = false;
            IndicatorLoadingInternetConnection.Opacity = 0d;
            ((BlurEffect)GridInfoInternetConnection.Effect).Radius = 0d;
            TextBlockInternetConnectionMillisecond.Opacity = VisualMillisecondConnectionEnabled ? 1d : 0d;

            #region BorderInternetConnection
            IELBlockInfoInternetConnection.MouseEnter += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.UsingBorderInformation(IELBlockInfoInternetConnection,
                    InternetPinging.ConnectInternet ? "Есть подключение к интернету" : "Нет подключения к интернету",
                    OrientationBorderPosition.RightUp);
            };
            IELBlockInfoInternetConnection.MouseLeave += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.CloseBorderInformation();
            };
            #endregion

            #region BorderStateRegister
            IELBlockInfoStateRegister.MouseEnter += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.UsingBorderInformation(IELBlockInfoStateRegister,
                    "Установленный регистр клавиатуры", OrientationBorderPosition.RightUp);
            };
            IELBlockInfoStateRegister.MouseUp += (sender, e) =>
            {
                Keybd_event(0x14, 0x45, 0x1, 0);
                Keybd_event(0x14, 0x45, 0x1 | 0x2, 0);
                UpdateInformationInObject(IELBlockInfoStateRegister, !Console.CapsLock ? "а".ToUpper() : "a".ToLower());
            };
            IELBlockInfoStateRegister.MouseLeave += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.CloseBorderInformation();
            };
            #endregion

            #region BorderCurrentLanguage
            InputLanguageManager.Current.InputLanguageChanged += (sender, e) =>
            {
                //UpdateInformationInObject(IELBlockInfoCurrentLanguage, InputLanguage.CurrentInputLanguage.Culture.EnglishName[0..3].ToUpper());
            };
            IELBlockInfoCurrentLanguage.MouseEnter += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.UsingBorderInformation(IELBlockInfoCurrentLanguage,
                    "Текущий язык раскладки клавиатуры",
                    OrientationBorderPosition.RightUp);
            };
            IELBlockInfoCurrentLanguage.MouseLeave += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.CloseBorderInformation();
            };
            #endregion

            #region BorderVolume
            IELBlockInfoVolume.MouseRightButtonUp += (sender, e) =>
            {
                App.MainWindow.IELActionPanelMain.UsingPanelAction(SettingVisualVolume, OrientationPanelActionPosition.LeftCenter);
                App.MainWindow.IELMessageMain.CloseBorderInformation();
            };
            IELBlockInfoVolume.MouseLeftButtonUp += (sender, e) =>
            {
                StructDirectoryResources.Play(CurrentApp.SoundChannelWaveOut, nameof(OPRES.AudioNotification));
            };
            IELBlockInfoVolume.MouseHover += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.UsingBorderInformation(IELBlockInfoVolume,
                    "Громкость звуков главного окна",
                    OrientationBorderPosition.RightUp);
            };
            IELBlockInfoVolume.MouseLeave += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.CloseBorderInformation();
            };
            #endregion

            ThreadInternetConnection = new(async () =>
            {
                await InternetPinging.UpdateInternetConnection();
#if DEBUG
                await Dispatcher.InvokeAsync(() => DEV_InternetMillisecond.Text = $"IP_E: {InternetPinging.MillisecondUpdateTime}");
#endif
                if (InternetPinging.MillisecondUpdateTime > 100 || InternetPinging.OLD_ConnectInternet != InternetPinging.ConnectInternet)
                {
                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                    {
                        bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;

                        DoubleAnimationType.AnimateEffect((BlurEffect)GridInfoInternetConnection.Effect, BlurEffect.RadiusProperty, 10d, TimeSpan.FromMilliseconds(700d));

                        DoubleAnimationType.AnimateEffect(IndicatorLoadingInternetConnection, OpacityProperty, 1d, TimeSpan.FromMilliseconds(700d));
                    });
                    Thread.Sleep(InternetPinging.OLD_ConnectInternet != InternetPinging.ConnectInternet ? 2500 : InternetPinging.MillisecondUpdateTime);
                    await Dispatcher.BeginInvoke(() =>
                    {
                        bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;

                        IELBlockInfoInternetConnection.Source =
                            StructDirectoryResources.GetResourceBitmap(InternetPinging.ConnectInternet ? nameof(OPRES.WifiOn) : nameof(OPRES.WifiOff));
                        if (VisualMillisecondConnectionEnabled)
                        {
                            TextBlockInternetConnectionMillisecond.Text = InternetPinging.ConnectInternet ? InternetPinging.MillisecondUpdateTime.ToString() + "mc" : "???";
                        }

                        DoubleAnimationType.AnimateEffect((BlurEffect)GridInfoInternetConnection.Effect, BlurEffect.RadiusProperty, 0d, TimeSpan.FromMilliseconds(700d));

                        DoubleAnimationType.AnimateEffect(IndicatorLoadingInternetConnection, OpacityProperty, 0d, TimeSpan.FromMilliseconds(700d));

                        if (CheckOpenMessageInObject(IELBlockInfoInternetConnection.Name))
                        {
                            App.MainWindow.IELMessageMain.UsingBorderInformation(IELBlockInfoInternetConnection,
                                InternetPinging.ConnectInternet ? "Есть подключение к интернету" : "Нет подключения к интернету",
                                OrientationBorderPosition.RightUp);
                        }
                    });
                }
                else
                {
                    await Dispatcher.BeginInvoke(() =>
                    {
                        //IELBlockInfoInternetConnection.Imaging =
                        //    StructDirectoryResources.GetResourceBitmap(InternetPinging.ConnectInternet ? nameof(OPRES.WifiOn) : nameof(OPRES.WifiOff));
                        if (VisualMillisecondConnectionEnabled)
                        {
                            TextBlockInternetConnectionMillisecond.Text = InternetPinging.ConnectInternet ? InternetPinging.MillisecondUpdateTime.ToString() + "mc" : "???";
                        }
                    });
                }
            }, 4000);

            ThreadInternetConnection.Start();
        }

        internal void VisibilityInternetMillisecond(bool Value)
        {
            //ThicknessAnimationType.AnimateEffect(IELBlockInfoInternetConnection.MainFrontImage, MarginProperty, Value ? new(2, 0, 2, 8) : new(2, 0, 2, 4),
            //    TimeSpan.FromMilliseconds(400d));
            DoubleAnimationType.AnimateEffect(TextBlockInternetConnectionMillisecond, OpacityProperty, Value ? 1d : 0d, TimeSpan.FromMilliseconds(500d));
            TextBlockInternetConnectionMillisecond.Text = InternetPinging.ConnectInternet ? InternetPinging.MillisecondUpdateTime.ToString() + "mc" : "???";
        }

        /// <summary>
        /// Обновить текстовую информацию в объекте
        /// </summary>
        /// <param name="Element">Обновляемый элемент</param>
        /// <param name="Text">Обновляемый текст</param>
        private static void UpdateInformationInObject(IELBlockInfoText Element, string Text)
        {
            Element.Text = Text;
            AnimateBlurEffect((BlurEffect)Element.Effect, 10u);
        }
    }
}
