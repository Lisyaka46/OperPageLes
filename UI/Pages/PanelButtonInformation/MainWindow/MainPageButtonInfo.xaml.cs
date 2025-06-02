using IEL;
using OperPage_les.CORE;
using OperPage_les.CORE.Settings;
using IEL.CORE.Enums;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using static OperPage_les.App;
using WpfAnimatedGif;

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
        /// Сохранённое состояние флага подключения к интернету
        /// </summary>
        private bool SaveUpdatingInternetConnect = false;

        /// <summary>
        /// Объект управления фоновым обновлением информации в данном окне 1000
        /// </summary>
        private readonly UpdateBackgroundData UpdateBackgroundDataThis;

        /// <summary>
        /// Узнать открыт ли объект сообщения в данном объекте по имени
        /// </summary>
        /// <param name="NameObject">Имя объекта проверки</param>
        /// <returns>Состояние открыт ли объект или нет</returns>
        private static bool CheckOpenMessageInObject(string NameObject) => MainWindowApplication.IELMessageMain.FlagMessage &&
                    MainWindowApplication.IELMessageMain.CodeParentObject.Equals(NameObject);

        public MainPageButtonInfo()
        {
            bool VisualMillisecondConnectionEnabled = CurrentApp.SettingMainApplication.MillisecondInternetConnection;
            InitializeComponent();
            ImageBehavior.SetAnimatedSource(ImageIndicatorLoadingInternetConnection, new BitmapImage(new Uri(App.DirectoryImageLoading)));
            IELBlockInfoInternetConnection.Imaging = App.LoadImage(Properties.Resources.Wifi);
            //ImageBehavior.SetAnimatedSource(ImageIndicatorLoadingInternetConnection, App.LoadImage(Properties.Resources.Loading));

            UpdateBackgroundDataThis = new(1000d, (sender, e) => Dispatcher.BeginInvoke(UpdateInformation));
            IELBlockInfoStateRegister.Text = Flags.FlagRegisterState ? "A" : "a";
            ImageIndicatorLoadingInternetConnection.Opacity = 0d;
            TextBlockInternetConnectionMillisecond.Opacity = VisualMillisecondConnectionEnabled ? 1d : 0d;
            IELBlockInfoInternetConnection.ImageMargin = VisualMillisecondConnectionEnabled ? new Thickness(2, 0, 2, 8) : new Thickness(2, 0, 2, 4);
            #region BorderInternetConnection
            IELBlockInfoInternetConnection.MouseEnter += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoInternetConnection,
                    Flags.InternetPinging ? "Есть подключение к интернету" : "Нет подключения к интернету",
                    OrientationBorderPosition.RightUp);
            };
            IELBlockInfoInternetConnection.MouseLeave += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region BorderStateRegister
            IELBlockInfoStateRegister.MouseEnter += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoStateRegister,
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
                MainWindowApplication.IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region BorderCurrentLanguage
            IELBlockInfoCurrentLanguage.MouseEnter += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoCurrentLanguage,
                    "Текущий язык раскладки клавиатуры",
                    OrientationBorderPosition.RightUp);
            };
            IELBlockInfoCurrentLanguage.MouseLeave += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.CloseBorderInformation();
            };
            #endregion

            #region Event Flags
            Flags.FlagRegisterState.ChangeStateFlag += (NewValue) =>
            {
                IELBlockInfoStateRegister.Text = NewValue ? "A" : "a";
                AnimateBlurEffect((BlurEffect)IELBlockInfoStateRegister.Effect, 10u);
                if (CheckOpenMessageInObject(IELBlockInfoStateRegister.Name))
                        MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoStateRegister,
                            Flags.FlagRegisterState ? "Установлен большой регистр" : "Установлен малый регистр",
                            OrientationBorderPosition.RightUp);
            };
            #endregion
            UpdateBackgroundDataThis.Start();
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
            if (Flags.InternetPinging.Wait)
            {
                animation.To = 10d;
                ((BlurEffect)IELBlockInfoInternetConnection.Effect).BeginAnimation(BlurEffect.RadiusProperty, animation);

                if (VisualMillisecondConnectionEnabled)
                {
                    ((BlurEffect)TextBlockInternetConnectionMillisecond.Effect).BeginAnimation(BlurEffect.RadiusProperty, animation);
                }

                animation.To = 1d;
                ImageIndicatorLoadingInternetConnection.BeginAnimation(OpacityProperty, animation);

                if (VisualMillisecondConnectionEnabled && ImageIndicatorLoadingInternetConnection.Opacity < 1d)
                {
                    ImageIndicatorLoadingInternetConnection.BeginAnimation(OpacityProperty, animation);
                }
            }
            else
            {
                IELBlockInfoInternetConnection.Imaging = App.LoadImage(Flags.InternetPinging ? Properties.Resources.WifiOn : Properties.Resources.WifiOff);
                if (VisualMillisecondConnectionEnabled)
                {
                    TextBlockInternetConnectionMillisecond.Text = Flags.InternetPinging ? MillisecondInternetConnection.ToString() + "mc" : "???";
                }

                animation.To = 0d;
                ((BlurEffect)IELBlockInfoInternetConnection.Effect).BeginAnimation(BlurEffect.RadiusProperty, animation);
                if (VisualMillisecondConnectionEnabled)
                {
                    ((BlurEffect)TextBlockInternetConnectionMillisecond.Effect).BeginAnimation(BlurEffect.RadiusProperty, animation);
                }

                ImageIndicatorLoadingInternetConnection.BeginAnimation(OpacityProperty, animation);

                if (Flags.InternetPinging != SaveUpdatingInternetConnect && CheckOpenMessageInObject(IELBlockInfoInternetConnection.Name))
                {
                    MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoInternetConnection,
                        Flags.InternetPinging ? "Есть подключение к интернету" : "Нет подключения к интернету",
                        OrientationBorderPosition.RightUp);
                }
                SaveUpdatingInternetConnect = Flags.InternetPinging;
            }
        }

        internal void VisibilityInternetMillisecond(bool Value)
        {
            AnimateThicknessEffect(IELBlockInfoInternetConnection.MainFrontImage, System.Windows.Controls.Image.MarginProperty, Value ? new(2, 0, 2, 8) : new Thickness(2, 0, 2, 4),
                TimeSpan.FromMilliseconds(400d));
            AnimateDoubleEffect(TextBlockInternetConnectionMillisecond, TextBlock.OpacityProperty, Value ? 1d : 0d, TimeSpan.FromMilliseconds(500d));
        }
    }
}
