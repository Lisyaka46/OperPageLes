using IEL.GUI;
using ApplicationOperPageLes.CORE.Settings;
using ApplicationOperPageLes.UI.Pages.ActionPanel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.ComponentModel;
using ApplicationOperPageLes.CORE;

namespace ApplicationOperPageLes.UI.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для PagePanelActionSetting.xaml
    /// </summary>
    public partial class PagePanelActionSetting : Page
    {
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
            IELButtonSetKeyKeboardMode.Text = OPLKeyConverter.ConvertKeyToString(App.CurrentApp.SettingMainApplication.KEY_KeyboardModePanelAction);
            IELButtonSetKeyKeboardMode.OnActivateMouseLeft += (sender, e, Key) =>
            {
                ActivateSelectButtonKey(IELButtonSetKeyKeboardMode);
            };
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
                IELButtonDemo1.CharKeyboardActivate = true;
                IELButtonDemo2.CharKeyboardActivate = true;
                IELButtonDemo3.CharKeyboardActivate = true;
                if (CheckBoxKeyboardRightClick.IsChecked ?? false)
                    App.DoubleAnimationType.AnimateEffect(ViewActivateRightClick, OpacityProperty, 0.7d, TimeSpan.FromMilliseconds(500d));
            };
            CheckBoxKeyboardMode.Unchecked += (sender, e) =>
            {
                IELButtonDemo1.CharKeyboardActivate = false;
                IELButtonDemo2.CharKeyboardActivate = false;
                IELButtonDemo3.CharKeyboardActivate = false;
                App.DoubleAnimationType.AnimateEffect(ViewActivateRightClick, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
            };
            #endregion
            #region CheckBoxKeyboardRightClick
            ViewActivateRightClick.Opacity = 0d;
            IELButtonSetKeyKeboardRightClick.Text = OPLKeyConverter.ConvertKeyToString(App.CurrentApp.SettingMainApplication.KEY_PanelActionRightClick);
            IELButtonSetKeyKeboardRightClick.OnActivateMouseLeft += (sender, e, Key) =>
            {
                ActivateSelectButtonKey(IELButtonSetKeyKeboardRightClick);
            };
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
                    App.DoubleAnimationType.AnimateEffect(ViewActivateRightClick, OpacityProperty, 0.7d, TimeSpan.FromMilliseconds(500d));
            };
            CheckBoxKeyboardRightClick.Unchecked += (sender, e) =>
            {
                if (CheckBoxKeyboardMode.IsChecked ?? false)
                    App.DoubleAnimationType.AnimateEffect(ViewActivateRightClick, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
            };
            #endregion
            #region CheckBoxKeyboardClosePanelAction
            IELButtonSetKeyKeboardClosePanelAction.Text = OPLKeyConverter.ConvertKeyToString(App.CurrentApp.SettingMainApplication.KEY_PanelActionClose);
            IELButtonSetKeyKeboardClosePanelAction.OnActivateMouseLeft += (sender, e, Key) =>
            {
                ActivateSelectButtonKey(IELButtonSetKeyKeboardClosePanelAction);
            };
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
                App.DoubleAnimationType.AnimateEffect(PanelActionDemo, WidthProperty, 0d, TimeSpan.FromMilliseconds(500d));
                App.DoubleAnimationType.AnimateEffect(PanelActionDemo, HeightProperty, 0d, TimeSpan.FromMilliseconds(500d));
            };
            CheckBoxKeyboardClosePanelAction.Unchecked += (sender, e) =>
            {
                App.DoubleAnimationType.AnimateEffect(PanelActionDemo, WidthProperty, 200d, TimeSpan.FromMilliseconds(500d));
                App.DoubleAnimationType.AnimateEffect(PanelActionDemo, HeightProperty, 150d, TimeSpan.FromMilliseconds(500d));
            };
            #endregion
        }

        /// <summary>
        /// Установить фокус на элемент изменения клавиши
        /// </summary>
        /// <param name="button"></param>
        private void ActivateSelectButtonKey(IELButtonText? button)
        {
            if (button == null) return;
            SelectSetKey?.Background.SetUsedState(false);
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
            SelectSetKey.Text = OPLKeyConverter.ConvertKeyToString(key);
            SelectSetKey.FontStyle = FontStyles.Normal;
            SelectSetKey?.Background.SetUsedState(false);
            SelectSetKey = null;
        }
    }
}
