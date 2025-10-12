using IEL.CORE.Enums;
using IEL.GUI;
using OperPageLes.CORE;
using OperPageLes.CORE.Struct;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using static OperPageLes.App;

namespace OperPageLes.UI.Pages.PanelButtonInformation.MainWindow
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

        public MainPageButtonInfo()
        {
            bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;
            InitializeComponent();
            IndicatorLoadingInternetConnection.Source = new Uri(StructDirectoryResources.DirectoryFileLoadingInternet);
            IndicatorLoadingInternetConnection.MediaEnded += (sender, e) =>
            {
                IndicatorLoadingInternetConnection.Position = TimeSpan.FromMilliseconds(1);
            };
            IELBlockInfoInternetConnection.Imaging = App.LoadImage(Properties.Resources.Wifi);

            UpdateInformationInObject(IELBlockInfoCurrentLanguage, InputLanguage.CurrentInputLanguage.Culture.EnglishName[0..3].ToUpper());
            UpdateInformationInObject(IELBlockInfoStateRegister, Console.CapsLock ? "а".ToUpper() : "a".ToLower());
            IELBlockInfoStateRegister.Focusable = false;
            IndicatorLoadingInternetConnection.Opacity = 0d;
            ((BlurEffect)GridInfoInternetConnection.Effect).Radius = 0d;
            TextBlockInternetConnectionMillisecond.Opacity = VisualMillisecondConnectionEnabled ? 1d : 0d;
            IELBlockInfoInternetConnection.ImageMargin = VisualMillisecondConnectionEnabled ? new(2, 0, 2, 8) : new(2, 0, 2, 4);
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
                UpdateInformationInObject(IELBlockInfoCurrentLanguage, InputLanguage.CurrentInputLanguage.Culture.EnglishName[0..3].ToUpper());
            };
            //InputLanguage.CurrentInputLanguage.
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

            ThreadInternetConnection = new(async () =>
            {
                await InternetPinging.UpdateInternetConnection();
                //Console.Beep(1000, 300);
                if (InternetPinging.MillisecondUpdateTime > 100 || InternetPinging.OLD_ConnectInternet != InternetPinging.ConnectInternet)
                {
                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                    {
                        bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;

                        AnimateDoubleEffect((BlurEffect)GridInfoInternetConnection.Effect, BlurEffect.RadiusProperty, 10d, TimeSpan.FromMilliseconds(700d));

                        AnimateDoubleEffect(IndicatorLoadingInternetConnection, OpacityProperty, 1d, TimeSpan.FromMilliseconds(700d));
                    });
                    Thread.Sleep(InternetPinging.OLD_ConnectInternet != InternetPinging.ConnectInternet ? 2500 : InternetPinging.MillisecondUpdateTime);
                    await Dispatcher.BeginInvoke(() =>
                    {
                        bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;

                        IELBlockInfoInternetConnection.Imaging = App.LoadImage(InternetPinging.ConnectInternet ?
                                OperPageLes.Properties.Resources.WifiOn : Properties.Resources.WifiOff);
                        if (VisualMillisecondConnectionEnabled)
                        {
                            TextBlockInternetConnectionMillisecond.Text = InternetPinging.ConnectInternet ? InternetPinging.MillisecondUpdateTime.ToString() + "mc" : "???";
                        }

                        AnimateDoubleEffect((BlurEffect)GridInfoInternetConnection.Effect, BlurEffect.RadiusProperty, 0d, TimeSpan.FromMilliseconds(700d));

                        AnimateDoubleEffect(IndicatorLoadingInternetConnection, OpacityProperty, 0d, TimeSpan.FromMilliseconds(700d));

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
                        IELBlockInfoInternetConnection.Imaging = App.LoadImage(InternetPinging.ConnectInternet ?
                                OperPageLes.Properties.Resources.WifiOn : OperPageLes.Properties.Resources.WifiOff);
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
            AnimateThicknessEffect(IELBlockInfoInternetConnection.MainFrontImage, MarginProperty, Value ? new(2, 0, 2, 8) : new(2, 0, 2, 4),
                TimeSpan.FromMilliseconds(400d));
            AnimateDoubleEffect(TextBlockInternetConnectionMillisecond, OpacityProperty, Value ? 1d : 0d, TimeSpan.FromMilliseconds(500d));
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
