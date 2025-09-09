using IEL.CORE.Enums;
using OperPage_les.CORE;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using static OperPage_les.App;

namespace OperPage_les.UI.Pages.PanelButtonInformation.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class MainPageButtonInfo : Page
    {
        [LibraryImport("user32.dll", EntryPoint = "keybd_event")]
        private static partial void Keybd_event(byte CodeButton, byte CodeScan, uint CodeState, UIntPtr dwExtralnfo);

        /// <summary>
        /// Объект управления фоновым обновлением информации в данном окне 1000
        /// </summary>
        private readonly UpdateBackgroundData UpdateBackgroundDataThis;

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
        internal ThreadGenericProcess ThreadInternetConnection;

        public MainPageButtonInfo()
        {
            bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;
            InitializeComponent();
            IndicatorLoadingInternetConnection.Source = new Uri(App.DirectoryFileLoadingInternet);
            IndicatorLoadingInternetConnection.MediaEnded += (sender, e) =>
            {
                IndicatorLoadingInternetConnection.Position = TimeSpan.FromMilliseconds(1);
            };
            IELBlockInfoInternetConnection.Imaging = App.LoadImage(Properties.Resources.Wifi);
            //ImageBehavior.SetAnimatedSource(ImageIndicatorLoadingInternetConnection, App.LoadImage(Properties.Resources.Loading));

            UpdateBackgroundDataThis = new(1000d, (sender, e) => Dispatcher.BeginInvoke(UpdateInformation));
            IELBlockInfoStateRegister.Text = Flags.FlagRegisterState ? "A" : "a";
            IndicatorLoadingInternetConnection.Opacity = 0d;
            ((BlurEffect)GridInfoInternetConnection.Effect).Radius = 0d;
            TextBlockInternetConnectionMillisecond.Opacity = VisualMillisecondConnectionEnabled ? 1d : 0d;
            IELBlockInfoInternetConnection.ImageMargin = VisualMillisecondConnectionEnabled ? new Thickness(2, 0, 2, 8) : new Thickness(2, 0, 2, 4);
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
                    Flags.FlagRegisterState ? "Установлен большой регистр" : "Установлен малый регистр",
                    OrientationBorderPosition.RightUp);
            };
            IELBlockInfoStateRegister.MouseUp += (sender, e) =>
            {
                Keybd_event(0x14, 0x45, 0x1, 0);
                Keybd_event(0x14, 0x45, 0x1 | 0x2, 0);
            };
            IELBlockInfoStateRegister.MouseLeave += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region BorderCurrentLanguage
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

            #region Event Flags
            Flags.FlagRegisterState.ChangeStateFlag += (NewValue) =>
            {
                IELBlockInfoStateRegister.Text = NewValue ? "A" : "a";
                AnimateBlurEffect((BlurEffect)IELBlockInfoStateRegister.Effect, 10u);
                if (CheckOpenMessageInObject(IELBlockInfoStateRegister.Name))
                    App.MainWindow.IELMessageMain.UsingBorderInformation(IELBlockInfoStateRegister,
                        Flags.FlagRegisterState ? "Установлен большой регистр" : "Установлен малый регистр",
                        OrientationBorderPosition.RightUp);
            };
            #endregion

            ThreadInternetConnection = new(async () =>
            {
                await InternetPinging.UpdateInternetConnection();
                if (InternetPinging.MillisecondUpdateTime > 100 || InternetPinging.OLD_ConnectInternet != InternetPinging.ConnectInternet)
                {
                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                    {
                        bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;

                        App.AnimateDoubleEffect((BlurEffect)GridInfoInternetConnection.Effect, BlurEffect.RadiusProperty, 10d, TimeSpan.FromMilliseconds(700d));

                        App.AnimateDoubleEffect(IndicatorLoadingInternetConnection, OpacityProperty, 1d, TimeSpan.FromMilliseconds(700d));
                    });
                    Thread.Sleep(InternetPinging.OLD_ConnectInternet != InternetPinging.ConnectInternet ? 2500 : InternetPinging.MillisecondUpdateTime);
                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                    {
                        bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;

                        IELBlockInfoInternetConnection.Imaging = App.LoadImage(InternetPinging.ConnectInternet ?
                                OperPage_les.Properties.Resources.WifiOn : OperPage_les.Properties.Resources.WifiOff);
                        if (VisualMillisecondConnectionEnabled)
                        {
                            TextBlockInternetConnectionMillisecond.Text = InternetPinging.ConnectInternet ? InternetPinging.MillisecondUpdateTime.ToString() + "mc" : "???";
                        }

                        App.AnimateDoubleEffect((BlurEffect)GridInfoInternetConnection.Effect, BlurEffect.RadiusProperty, 0d, TimeSpan.FromMilliseconds(700d));

                        App.AnimateDoubleEffect(IndicatorLoadingInternetConnection, OpacityProperty, 0d, TimeSpan.FromMilliseconds(700d));

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
                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                    {
                        IELBlockInfoInternetConnection.Imaging = App.LoadImage(InternetPinging.ConnectInternet ?
                                OperPage_les.Properties.Resources.WifiOn : OperPage_les.Properties.Resources.WifiOff);
                        if (VisualMillisecondConnectionEnabled)
                        {
                            TextBlockInternetConnectionMillisecond.Text = InternetPinging.ConnectInternet ? InternetPinging.MillisecondUpdateTime.ToString() + "mc" : "???";
                        }
                    });
                }
            }, 4000);

            UpdateBackgroundDataThis.Start();
            ThreadInternetConnection.Start();
        }

        /// <summary>
        /// Обновление информации
        /// </summary>
        private void UpdateInformation()
        {
            bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;
            DoubleAnimation animation = new()
            {
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut,
                },
                Duration = TimeSpan.FromMilliseconds(700),
            };
            string LangName = InputLanguage.CurrentInputLanguage.Culture.NativeName[0..3].ToUpper();
            if (!LangName.Equals(IELBlockInfoCurrentLanguage.Text))
            {
                IELBlockInfoCurrentLanguage.Text = LangName;
                AnimateBlurEffect((BlurEffect)IELBlockInfoCurrentLanguage.Effect, 5u);
            }
        }

        internal void VisibilityInternetMillisecond(bool Value)
        {
            AnimateThicknessEffect(IELBlockInfoInternetConnection.MainFrontImage, System.Windows.Controls.Image.MarginProperty, Value ? new(2, 0, 2, 8) : new Thickness(2, 0, 2, 4),
                TimeSpan.FromMilliseconds(400d));
            AnimateDoubleEffect(TextBlockInternetConnectionMillisecond, TextBlock.OpacityProperty, Value ? 1d : 0d, TimeSpan.FromMilliseconds(500d));
            TextBlockInternetConnectionMillisecond.Text = InternetPinging.ConnectInternet ? InternetPinging.MillisecondUpdateTime.ToString() + "mc" : "???";
        }
    }
}
