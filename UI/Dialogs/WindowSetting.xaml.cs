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
        /// Константа анимации наведения на кнопку страницы настроек
        /// </summary>
        private const int ButtonChangeMatginBottomPage = 4;

        public WindowSetting()
        {
            InitializeComponent();
            GeneralSetting = new();
            #region IELGeneralButton
            IELGeneralButton.MouseEnter += (sender, e) =>
            {
                App.AnimateThicknessEffect(IELGeneralButton, MarginProperty,
                    GetMarginAnimatePageButton(IELGeneralButton.Margin, true), TimeSpan.FromMilliseconds(IELGeneralButton.IELSettingObject.AnimationMillisecond));
            };
            IELGeneralButton.MouseLeave += (sender, e) =>
            {
                App.AnimateThicknessEffect(IELGeneralButton, MarginProperty,
                    GetMarginAnimatePageButton(IELGeneralButton.Margin, false), TimeSpan.FromMilliseconds(IELGeneralButton.IELSettingObject.AnimationMillisecond));
            };
            IELGeneralButton.OnActivateMouseLeft += (Key) =>
            {
                MainPageController.NextPage(GeneralSetting);
                IELGeneralButton.IELSettingObject.BackgroundSetting.UsedState = true;
            };
            IELGeneralButton.OnActivateMouseRight += (Key) =>
            {
                if (!IELGeneralButton.IELSettingObject.BackgroundSetting.UsedState) return;
                MainPageController.ClosePage();
                IELGeneralButton.IELSettingObject.BackgroundSetting.UsedState = false;
            };
            #endregion
            #region This
            Closed += (sender, e) =>
            {
                App.CurrentApp.UpdateSettingApplication();
            };
            #endregion
        }

        /// <summary>
        /// Узнать Thickness при наведении на кнопку страницы настроек
        /// </summary>
        /// <param name="Source">Текущий Thickness кнопки</param>
        /// <param name="Activate">Состояние активации наведения</param>
        /// <returns>Будущий Thickness для анимирования наведения</returns>
        private static Thickness GetMarginAnimatePageButton(Thickness Source, bool Activate) => 
            new(Source.Left, Source.Top, Source.Right, Source.Bottom + (Activate ? -ButtonChangeMatginBottomPage : ButtonChangeMatginBottomPage));
    }
}
