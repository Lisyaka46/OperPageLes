using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OperPageLes.UI.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для PageConsoleSetting.xaml
    /// </summary>
    public partial class PageConsoleSetting : Page, IOPLAnimate
    {
        //
        private int OriginalConsoleScrollForce;

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        public PageConsoleSetting()
        {
            InitializeComponent();

            #region CheckBoxHitUse
            CheckBoxHitUse.IsChecked = App.CurrentApp.SettingMainApplication.HitUse;
            CheckBoxHitUse.Checked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.HitUse.Value = true;
            };
            CheckBoxHitUse.Unchecked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.HitUse.Value = false;
            };
            #endregion
            #region CheckBoxMovePageExecuteBufferCommand
            CheckBoxMovePageExecuteBufferCommand.IsChecked = App.CurrentApp.SettingMainApplication.MovePageExecuteBufferCommand;
            CheckBoxMovePageExecuteBufferCommand.Checked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.MovePageExecuteBufferCommand.Value = true;
            };
            CheckBoxMovePageExecuteBufferCommand.Unchecked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.MovePageExecuteBufferCommand.Value = false;
            };
            #endregion
            #region ConsoleScrollForce
            BorderSettingScrollForce.Opacity = 0d;
            BorderSettingScrollForce.Height = 0d;
            OriginalConsoleScrollForce = App.CurrentApp.SettingMainApplication.ConsoleScrollForce;
            SliderScrollForce.Value = OriginalConsoleScrollForce;
            TextBlockSliderScrollForce.Text = SliderScrollForce.Value.ToString();
            //BorderSettingBufferSize.Margin = new(BorderSettingBufferSize.Margin.Left, 0, BorderSettingBufferSize.Margin.Right, 35);
            SliderScrollForce.ValueChanged += (sender, e) =>
            {
                TextBlockSliderScrollForce.Text = e.NewValue.ToString();
                if ((e.NewValue != OriginalConsoleScrollForce && e.OldValue == OriginalConsoleScrollForce) ||
                    (e.NewValue == OriginalConsoleScrollForce && e.OldValue != OriginalConsoleScrollForce))
                {
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderSettingScrollForce, HeightProperty,
                        e.NewValue != OriginalConsoleScrollForce ? 50d : 6d, TimeSpan.FromMilliseconds(1200d));
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderSettingScrollForce, OpacityProperty,
                        e.NewValue != OriginalConsoleScrollForce ? 1d : 0d, TimeSpan.FromMilliseconds(1200d));
                }
            };
            IELButtonClearScrollForceValue.OnActivateMouseLeft += (sender, e) =>
            {
                SliderScrollForce.Value = OriginalConsoleScrollForce;
                App.CurrentApp.SettingMainApplication.ConsoleScrollForce.Value = OriginalConsoleScrollForce;
            };
            SliderScrollForce.MouseLeave += (sender, e) =>
            {
                if (SliderScrollForce.Value != OriginalConsoleScrollForce)
                    App.CurrentApp.SettingMainApplication.ConsoleScrollForce.Value = (int)SliderScrollForce.Value;
            };
            #endregion
        }
    }
}
