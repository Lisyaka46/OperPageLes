using IEL.CORE.Classes;
using OIEL.CORE.Browser;
using OperPageLes.CORE.Enums;
using OperPageLes.UI.Pages.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для PageSettingApp.xaml
    /// </summary>
    public partial class PageSettingApp : PageBrowser
    {
        /// <summary>
        /// Страница общих настроек программы
        /// </summary>
        private readonly PageGeneralSetting GeneralSetting;

        /// <summary>
        /// Страница настроек консоли
        /// </summary>
        private readonly PageConsoleSetting ConsoleSetting;

        /// <summary>
        /// Страница настроек панели действий
        /// </summary>
        private readonly PagePanelActionSetting PanelActionSetting;

        /// <summary>
        /// Активная настройка
        /// </summary>
        private QData? ActiveBackgroundSetting;

        /// <summary>
        /// Активный индекс столбца позиции складки настроек
        /// </summary>
        private int ActiveIndexColumn = -1;

        public PageSettingApp()
        {
            InitializeComponent();
            GeneralSetting = new();
            ConsoleSetting = new();
            PanelActionSetting = new();

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELGeneralButton);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELConsoleButton);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.LightBlue].ConnectPalleteFromIELElement(IELPanelActionButton);

            #region IELButtonsSetting
            #region IELGeneralButton
            IELGeneralButton.MouseEnter += (sender, e) =>
            {
                //App.ThicknessAnimationType.AnimateEffect(IELGeneralButton, MarginProperty,
                //    GetMarginAnimatePageButton(IELGeneralButton.Margin, true), TimeSpan.FromMilliseconds(IELGeneralButton.IELSettingObject.AnimationMillisecond));
            };
            IELGeneralButton.MouseLeave += (sender, e) =>
            {
                //App.ThicknessAnimationType.AnimateEffect(IELGeneralButton, MarginProperty,
                //    GetMarginAnimatePageButton(IELGeneralButton.Margin, false), TimeSpan.FromMilliseconds(IELGeneralButton.IELSettingObject.AnimationMillisecond));
            };
            IELGeneralButton.OnActivateMouseLeft += (sender, e) =>
            {
                int OriginIndex = Grid.GetColumn(IELGeneralButton);
                //ActiveBackgroundSetting?.SetUsedState(false);
                //ActiveBackgroundSetting = IELGeneralButton.SourceBackground.Sou;
                MainPageController.NextPage(GeneralSetting, ActiveIndexColumn < OriginIndex);
                ActiveIndexColumn = OriginIndex;
                //ActiveBackgroundSetting.SetUsedState(true);
            };
            #endregion

            #region IELConsoleButton
            IELConsoleButton.MouseEnter += (sender, e) =>
            {
                //App.ThicknessAnimationType.AnimateEffect(IELConsoleButton, MarginProperty,
                //    GetMarginAnimatePageButton(IELConsoleButton.Margin, true), TimeSpan.FromMilliseconds(IELConsoleButton.IELSettingObject.AnimationMillisecond));
            };
            IELConsoleButton.MouseLeave += (sender, e) =>
            {
                //App.ThicknessAnimationType.AnimateEffect(IELConsoleButton, MarginProperty,
                //    GetMarginAnimatePageButton(IELConsoleButton.Margin, false), TimeSpan.FromMilliseconds(IELConsoleButton.IELSettingObject.AnimationMillisecond));
            };
            IELConsoleButton.OnActivateMouseLeft += (sender, e) =>
            {
                int OriginIndex = Grid.GetColumn(IELConsoleButton);
                //ActiveBackgroundSetting?.SetUsedState(false);
                //ActiveBackgroundSetting = IELConsoleButton.SourceBackground;
                MainPageController.NextPage(ConsoleSetting, ActiveIndexColumn < OriginIndex);
                ActiveIndexColumn = OriginIndex;
                //ActiveBackgroundSetting.SetUsedState(true);
            };
            #endregion

            #region IELPanelActionButton
            IELPanelActionButton.MouseEnter += (sender, e) =>
            {
                //App.ThicknessAnimationType.AnimateEffect(IELPanelActionButton, MarginProperty,
                //    GetMarginAnimatePageButton(IELPanelActionButton.Margin, true), TimeSpan.FromMilliseconds(IELPanelActionButton.IELSettingObject.AnimationMillisecond));
            };
            IELPanelActionButton.MouseLeave += (sender, e) =>
            {
                //App.ThicknessAnimationType.AnimateEffect(IELPanelActionButton, MarginProperty,
                //    GetMarginAnimatePageButton(IELPanelActionButton.Margin, false), TimeSpan.FromMilliseconds(IELPanelActionButton.IELSettingObject.AnimationMillisecond));
            };
            IELPanelActionButton.OnActivateMouseLeft += (sender, e) =>
            {
                int OriginIndex = Grid.GetColumn(IELPanelActionButton);
                //ActiveBackgroundSetting?.SetUsedState(false);
                //ActiveBackgroundSetting = IELPanelActionButton.SourceBackground;
                MainPageController.NextPage(PanelActionSetting, ActiveIndexColumn < OriginIndex);
                ActiveIndexColumn = OriginIndex;
                //ActiveBackgroundSetting.SetUsedState(true);
            };
            #endregion
            #endregion
            #region This
            //Closed += (sender, e) =>
            //{
            //    App.CurrentApp.UpdateSettingApplication();
            //};
            #endregion
        }

        /// <summary>
        /// Узнать Thickness при наведении на кнопку страницы настроек
        /// </summary>
        /// <param name="Source">Текущий Thickness кнопки</param>
        /// <param name="Activate">Состояние активации наведения</param>
        /// <returns>Будущий Thickness для анимирования наведения</returns>
        private static Thickness GetMarginAnimatePageButton(Thickness Source, bool Activate) =>
            new(Source.Left, Source.Top, Source.Right, Activate ? 6 : 10);
    }
}
