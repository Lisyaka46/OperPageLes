using IEL.CORE.Classes;
using OperPage_les.UI.Pages.Settings;
using System.Windows;
using System.Windows.Controls;

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
        /// Страница настроек консоли
        /// </summary>
        private readonly PageConsoleSetting ConsoleSetting;

        /// <summary>
        /// Активная настройка
        /// </summary>
        private BrushSettingQ? ActiveBackgroundSetting;

        /// <summary>
        /// Активный индекс столбца позиции складки настроек
        /// </summary>
        private int ActiveIndexColumn = -1;

        public WindowSetting()
        {
            InitializeComponent();
            GeneralSetting = new();
            ConsoleSetting = new();
            #region IELButtonsSetting
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
            IELGeneralButton.OnActivateMouseLeft += (sender, e, Key) =>
            {
                int OriginIndex = Grid.GetColumn(IELGeneralButton);
                ActiveBackgroundSetting?.SetUsedState(false);
                ActiveBackgroundSetting = IELGeneralButton.IELSettingObject.BackgroundSetting;
                MainPageController.NextPage(GeneralSetting, ActiveIndexColumn < OriginIndex);
                ActiveIndexColumn = OriginIndex;
                ActiveBackgroundSetting.SetUsedState(true);
            };
            #endregion
            #region IELConsoleButton
            IELConsoleButton.MouseEnter += (sender, e) =>
            {
                App.AnimateThicknessEffect(IELConsoleButton, MarginProperty,
                    GetMarginAnimatePageButton(IELConsoleButton.Margin, true), TimeSpan.FromMilliseconds(IELConsoleButton.IELSettingObject.AnimationMillisecond));
            };
            IELConsoleButton.MouseLeave += (sender, e) =>
            {
                App.AnimateThicknessEffect(IELConsoleButton, MarginProperty,
                    GetMarginAnimatePageButton(IELConsoleButton.Margin, false), TimeSpan.FromMilliseconds(IELConsoleButton.IELSettingObject.AnimationMillisecond));
            };
            IELConsoleButton.OnActivateMouseLeft += (sender, e, Key) =>
            {
                int OriginIndex = Grid.GetColumn(IELConsoleButton);
                ActiveBackgroundSetting?.SetUsedState(false);
                ActiveBackgroundSetting = IELConsoleButton.IELSettingObject.BackgroundSetting;
                MainPageController.NextPage(ConsoleSetting, ActiveIndexColumn < OriginIndex);
                ActiveIndexColumn = OriginIndex;
                ActiveBackgroundSetting.SetUsedState(true);
            };
            #endregion
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
            new(Source.Left, Source.Top, Source.Right, Activate ? 6 : 10);
    }
}
