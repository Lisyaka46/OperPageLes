using IEL.CORE.Classes;
using IEL.UserElementsControl;
using OperPageLes.CORE;
using OperPageLes.CORE.Settings;
using OperPageLes.UI.Pages.ActionPanel;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OperPageLes.UI.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для PagePanelActionSetting.xaml
    /// </summary>
    public partial class PagePanelActionSetting : Page, IOPLAnimate
    {
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Объект кнопки на котором выделен фокус изменения назначенной клавиши
        /// </summary>
        private IELButtonText? SelectSetKey = null;

        /// <summary>
        /// Клавиши которые являются не присваиваемыми
        /// </summary>
        private readonly Key[] KeysNotValid =
            [
            Key.PrintScreen, Key.Scroll, Key.Pause,
            Key.Insert, Key.Home, Key.PageUp,
            Key.Delete, Key.End, Key.PageDown,
            Key.Apps, Key.CapsLock, Key.Tab, Key.Back, Key.LWin, Key.System,
            ];

        public PagePanelActionSetting()
        {
            InitializeComponent();
            CheckBoxKeyboardMode.IsChecked = false;
            CheckBoxKeyboardRightClick.IsChecked = false;
            CheckBoxKeyboardClosePanelAction.IsChecked = false;

            #region CheckBoxExitKeyboardMode
            CheckBoxExitKeyboardMode.IsChecked = App.CurrentApp.SettingMainApplication.ExitKeyboardModeInClosePanelAction;
            CheckBoxExitKeyboardMode.Checked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.ExitKeyboardModeInClosePanelAction.Value = true;
            };
            CheckBoxExitKeyboardMode.Unchecked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.ExitKeyboardModeInClosePanelAction.Value = false;
            };
            #endregion

            #region CheckBoxKeyboardMode
            IELButtonSetKeyKeboardMode.Text = IELKeyConverter.ConvertKeyToString(App.CurrentApp.SettingMainApplication.KEY_KeyboardModePanelAction);
            IELButtonSetKeyKeboardMode.OnActivateMouseLeft += UsingSelectKeyValueSet;
            IELButtonSetKeyKeboardMode.KeyDown += (sender, e) =>
            {
                if (!KeysNotValid.Any((i) => i == e.Key))
                {
                    VisualSetKey(e.Key);
                    App.CurrentApp.SettingMainApplication.KEY_KeyboardModePanelAction.Value = e.Key;
                }
                e.Handled = true;
            };
            CheckBoxKeyboardMode.Checked += (sender, e) =>
            {
                IELButtonDemo1.IsVisibleKeyActivate = true;
                IELButtonDemo2.IsVisibleKeyActivate = true;
                IELButtonDemo3.IsVisibleKeyActivate = true;
                if (CheckBoxKeyboardRightClick.IsChecked ?? false)
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ViewActivateRightClick, OpacityProperty,
                        0.7d, TimeSpan.FromMilliseconds(500d));
            };
            CheckBoxKeyboardMode.Unchecked += (sender, e) =>
            {
                IELButtonDemo1.IsVisibleKeyActivate = false;
                IELButtonDemo2.IsVisibleKeyActivate = false;
                IELButtonDemo3.IsVisibleKeyActivate = false;
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ViewActivateRightClick, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(500d));
            };
            #endregion

            #region CheckBoxKeyboardRightClick
            ViewActivateRightClick.Opacity = 0d;
            IELButtonSetKeyKeboardRightClick.Text = IELKeyConverter.ConvertKeyToString(App.CurrentApp.SettingMainApplication.KEY_PanelActionRightClick);
            IELButtonSetKeyKeboardRightClick.OnActivateMouseLeft += UsingSelectKeyValueSet;
            IELButtonSetKeyKeboardRightClick.KeyDown += (sender, e) =>
            {
                if (!KeysNotValid.Any((i) => i == e.Key))
                {
                    VisualSetKey(e.Key);
                    App.CurrentApp.SettingMainApplication.KEY_PanelActionRightClick.Value = e.Key;
                }
                e.Handled = true;
            };
            CheckBoxKeyboardRightClick.Checked += (sender, e) =>
            {
                if (CheckBoxKeyboardMode.IsChecked ?? false)
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ViewActivateRightClick, OpacityProperty,
                        0.7d, TimeSpan.FromMilliseconds(500d));
            };
            CheckBoxKeyboardRightClick.Unchecked += (sender, e) =>
            {
                if (CheckBoxKeyboardMode.IsChecked ?? false)
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ViewActivateRightClick, OpacityProperty,
                        0d, TimeSpan.FromMilliseconds(500d));
            };
            #endregion

            #region CheckBoxKeyboardClosePanelAction
            IELButtonSetKeyKeboardClosePanelAction.Text = IELKeyConverter.ConvertKeyToString(App.CurrentApp.SettingMainApplication.KEY_PanelActionClose);
            IELButtonSetKeyKeboardClosePanelAction.OnActivateMouseLeft += UsingSelectKeyValueSet;
            IELButtonSetKeyKeboardClosePanelAction.KeyDown += (sender, e) =>
            {
                if (!KeysNotValid.Any((i) => i == e.Key))
                {
                    VisualSetKey(e.Key);
                    App.CurrentApp.SettingMainApplication.KEY_PanelActionClose.Value = e.Key;
                }
                e.Handled = true;
            };
            CheckBoxKeyboardClosePanelAction.Checked += (sender, e) =>
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, PanelActionDemo, WidthProperty, 0d, TimeSpan.FromMilliseconds(500d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, PanelActionDemo, HeightProperty, 0d, TimeSpan.FromMilliseconds(500d));
            };
            CheckBoxKeyboardClosePanelAction.Unchecked += (sender, e) =>
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, PanelActionDemo, WidthProperty, 200d, TimeSpan.FromMilliseconds(500d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, PanelActionDemo, HeightProperty, 150d, TimeSpan.FromMilliseconds(500d));
            };
            #endregion
        }

        /// <summary>
        /// Использовать выделение для установки значения клавиши
        /// </summary>
        /// <param name="sender">Вызываемый объект</param>
        /// <param name="e">Управляемый объект событием</param>
        private void UsingSelectKeyValueSet(object sender, MouseButtonEventArgs e)
        {
            if (SelectSetKey != null)
            {
                SelectSetKey.SourceBackground.SetUsedState(false);
                SelectSetKey.FontStyle = FontStyles.Normal;
                bool EqualsSelectElement = SelectSetKey.Equals(sender);
                SelectSetKey = null;
                if (EqualsSelectElement) return;
            }
            if (sender is IELButtonText button)
            {
                button.SourceBackground.SetUsedState(true);
                ActivateSelectButtonKey(button);
            }
            e.Handled = true;
        }

        /// <summary>
        /// Установить фокус на элемент изменения клавиши
        /// </summary>
        /// <param name="button"></param>
        private void ActivateSelectButtonKey(IELButtonText? button)
        {
            if (button == null) return;
            SelectSetKey?.SourceBackground.SetUsedState(false);
            SelectSetKey = button;
            SelectSetKey.Focus();
            SelectSetKey.FontStyle = FontStyles.Italic;
        }

        /// <summary>
        /// Отобразить установленную клавишу
        /// </summary>
        /// <param name="key"></param>
        private void VisualSetKey(Key key)
        {
            if (SelectSetKey == null) return;
            SelectSetKey.Text = IELKeyConverter.ConvertKeyToString(key);
            SelectSetKey.FontStyle = FontStyles.Normal;
            SelectSetKey.SourceBackground.SetUsedState(false);
            SelectSetKey = null;
        }
    }
}
