using AAC20;
using AAC20.CORE;
using IEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static AAC20.App;

namespace OperPage_les.UI.Pages.PanelButtonInformation.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
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
                    MainWindowApplication.IELMessageMain.NameParentObject.Equals(NameObject);

        public Page1()
        {
            InitializeComponent();
            UpdateBackgroundDataThis = new(1000d, (sender, e) => Dispatcher.BeginInvoke(UpdateInformation));
            IELBlockInfoStateRegister.Text = Flags.FlagRegisterState ? "A" : "a";
            ImageIndicatorLoadingInternetConnection.Opacity = 0d;
            #region BorderInternetConnection
            IELBlockInfoInternetConnection.MouseEnter += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoInternetConnection, IELBlockInfoInternetConnection.Name,
                    Flags.InternetPinging ? "Есть подключение к интернету" : "Нет подключения к интернету",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            IELBlockInfoInternetConnection.MouseLeave += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region BorderStateRegister
            IELBlockInfoStateRegister.MouseEnter += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoStateRegister, IELBlockInfoStateRegister.Name,
                    Flags.FlagRegisterState ? "Установлен большой регистр" : "Установлен малый регистр",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            IELBlockInfoStateRegister.MouseLeave += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.CloseBorderInformation();
            };
            #endregion
            #region BorderCurrentLanguage
            IELBlockInfoCurrentLanguage.MouseEnter += (sender, e) =>
            {
                MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoCurrentLanguage, IELBlockInfoCurrentLanguage.Name,
                    "Текущий язык раскладки клавиатуры",
                    IELBlockMessage.OrientationBorderInfo.RightUp);
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
                        MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoStateRegister, IELBlockInfoStateRegister.Name,
                            Flags.FlagRegisterState ? "Установлен большой регистр" : "Установлен малый регистр",
                            IELBlockMessage.OrientationBorderInfo.RightUp);
            };
            #endregion
            UpdateBackgroundDataThis.TimerDataUpdate.Start();
        }

        /// <summary>
        /// Обновление информации
        /// </summary>
        private void UpdateInformation()
        {
            DoubleAnimation animation = new()
            {
                EasingFunction = new CubicEase()
                {
                    EasingMode = EasingMode.EaseOut,
                },
                Duration = TimeSpan.FromMilliseconds(700),
            };
            string LangName = System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture.NativeName[0..3].ToUpper();
            if (!LangName.Equals(IELBlockInfoCurrentLanguage.Text))
            {
                IELBlockInfoCurrentLanguage.Text = LangName;
                AnimateBlurEffect((BlurEffect)IELBlockInfoCurrentLanguage.Effect, 5u);
            }
            if (Flags.InternetPinging.Wait)
            {
                animation.To = 10d;
                ((BlurEffect)IELBlockInfoInternetConnection.Effect).BeginAnimation(BlurEffect.RadiusProperty, animation);

                animation.To = 1d;
                ImageIndicatorLoadingInternetConnection.BeginAnimation(OpacityProperty, animation);
            }
            else
            {
                IELBlockInfoInternetConnection.Imaging = new BitmapImage(new Uri($"{App.PathImageApplication}/Wifi{(Flags.InternetPinging ? "On" : "Off")}.png", UriKind.Relative));
                
                animation.To = 0d;
                ((BlurEffect)IELBlockInfoInternetConnection.Effect).BeginAnimation(BlurEffect.RadiusProperty, animation);

                animation.To = 0d;
                ImageIndicatorLoadingInternetConnection.BeginAnimation(OpacityProperty, animation);

                if (Flags.InternetPinging != SaveUpdatingInternetConnect && CheckOpenMessageInObject(IELBlockInfoInternetConnection.Name))
                {
                    MainWindowApplication.IELMessageMain.UsingBorderInformation(IELBlockInfoInternetConnection, IELBlockInfoInternetConnection.Name,
                        Flags.InternetPinging ? "Есть подключение к интернету" : "Нет подключения к интернету",
                        IELBlockMessage.OrientationBorderInfo.RightUp);
                }
                SaveUpdatingInternetConnect = Flags.InternetPinging;
            }
        }
    }
}
