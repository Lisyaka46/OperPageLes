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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IEL.Interfaces.Core;
using OperPage_les.UI.Pages.Settings;
using OperPage_les.CORE.Settings;
using System.Windows.Media.Animation;

namespace OperPage_les.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowSetting.xaml
    /// </summary>
    public partial class WindowSetting : Window
    {
        /// <summary>
        /// Страница общих настроек программы
        /// </summary>
        private readonly PageGeneralSetting GeneralSetting;

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Константа дизактивированной кнопки страницы настроек
        /// </summary>
        private const int DiactivateButtonMatginBottomPage = 6;

        /// <summary>
        /// Константа активированной кнопки страницы настроек
        /// </summary>
        private const int ActivateButtonMatginBottomPage = 2;

        public WindowSetting()
        {
            InitializeComponent();
            GeneralSetting = new();
            GeneralSetting.EventChangeValue += (Name, Value) =>
            {
                App.CurrentApp.SettingApplication.SetSettingValue(Name, Value);
                switch (Name)
                {
                    case EnumSettingApplication.PathMenuImage:
                        App.MainWindowApplication.UpdateImageMenu();
                        break;
                    case EnumSettingApplication.BufferSize:
                        // REBOOT
                        break;
                    case EnumSettingApplication.BlurBackgroundDataTime:
                        App.MainWindowApplication.ChangeBlurImageInDataTime(Value.Equals("T"));
                        break;
                    case EnumSettingApplication.MillisecondInternetConnection:
                        App.MainWindowApplication.ChangeVisibilityMillisecondInternet(Value.Equals("T"));
                        break;
                }
            };
            MouseWheel += (sender, e) =>
            {
                if (e.Delta > 0)
                {
                    ScrollBarElement.Value -= 40;
                }
                else if (e.Delta < 0)
                {
                    ScrollBarElement.Value += 40;
                }
            };
            #region IELGeneralButton
            IELGeneralButton.MouseEnter += (sender, e) =>
            {
                ThicknessAnimate.To = new(IELGeneralButton.Margin.Left, 0, IELGeneralButton.Margin.Right, ActivateButtonMatginBottomPage);
                IELGeneralButton.BeginAnimation(MarginProperty, ThicknessAnimate);
            };
            IELGeneralButton.MouseLeave += (sender, e) =>
            {
                ThicknessAnimate.To = new(IELGeneralButton.Margin.Left, 0, IELGeneralButton.Margin.Right, DiactivateButtonMatginBottomPage);
                IELGeneralButton.BeginAnimation(MarginProperty, ThicknessAnimate);
            };
            IELGeneralButton.OnActivateMouseLeft += () =>
            {
                MainPageController.NextPage(GeneralSetting);
                double HeightScrollBar = GeneralSetting.ActualHeight - BorderElement.ActualHeight;
                if (HeightScrollBar <= 0)
                {

                }
                else
                {
                    ScrollBarElement.Maximum = HeightScrollBar;
                    ScrollBarElement.Value = 0;
                    MainPageController.Margin = new(0);
                }
            };
            ScrollBarElement.ValueChanged += (sender, e) =>
            {
                App.AnimateThicknessEffect(MainPageController, MarginProperty, new(0, -e.NewValue, 0, 0), TimeSpan.FromMilliseconds(240d));
            };
            #endregion
            #region This
            Closed += (sender, e) =>
            {
                App.CurrentApp.UpdateSettingApplication();
            };
            #endregion
        }
    }
}
