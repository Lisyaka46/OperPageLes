using IEL.GUI;
using OperPageLes.CORE.Settings;
using OperPageLes.UI.Pages.ActionPanel;
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

namespace OperPageLes.UI.Pages.Settings
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

        public PagePanelActionSetting()
        {
            InitializeComponent();
            CheckBoxKeyboardMode.IsChecked = false;
            CheckBoxKeyboardRightClick.IsChecked = false;
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
            IELButtonSetKeyKeboardMode.Text = App.CurrentApp.SettingMainApplication.KEY_KeyboardModePanelAction.Value.ToString();
            IELButtonSetKeyKeboardMode.OnActivateMouseLeft += (sender, e, Key) =>
            {
                ActivateSelectButtonKey(IELButtonSetKeyKeboardMode);
            };
            IELButtonSetKeyKeboardMode.KeyUp += (sender, e) =>
            {
                VisualSetKey(e.Key);
                App.CurrentApp.SettingMainApplication.KEY_KeyboardModePanelAction.Value = e.Key;
            };
            CheckBoxKeyboardMode.Checked += (sender, e) =>
            {
                IELButtonDemo1.CharKeyboardActivate = true;
                IELButtonDemo2.CharKeyboardActivate = true;
                IELButtonDemo3.CharKeyboardActivate = true;
                if (CheckBoxKeyboardRightClick.IsChecked ?? false)
                    App.AnimateDoubleEffect(ViewActivateRightClick, OpacityProperty, 0.7d, TimeSpan.FromMilliseconds(500d));
            };
            CheckBoxKeyboardMode.Unchecked += (sender, e) =>
            {
                IELButtonDemo1.CharKeyboardActivate = false;
                IELButtonDemo2.CharKeyboardActivate = false;
                IELButtonDemo3.CharKeyboardActivate = false;
                App.AnimateDoubleEffect(ViewActivateRightClick, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
            };
            #endregion
            #region CheckBoxKeyboardRightClick
            ViewActivateRightClick.Opacity = 0d;
            IELButtonSetKeyKeboardRightClick.Text = App.CurrentApp.SettingMainApplication.KEY_PanelActionRightClick.Value.ToString();
            IELButtonSetKeyKeboardRightClick.OnActivateMouseLeft += (sender, e, Key) =>
            {
                ActivateSelectButtonKey(IELButtonSetKeyKeboardRightClick);
            };
            IELButtonSetKeyKeboardRightClick.KeyUp += (sender, e) =>
            {
                VisualSetKey(e.Key);
                App.CurrentApp.SettingMainApplication.KEY_PanelActionRightClick.Value = e.Key;
            };
            CheckBoxKeyboardRightClick.Checked += (sender, e) =>
            {
                if (CheckBoxKeyboardMode.IsChecked ?? false)
                    App.AnimateDoubleEffect(ViewActivateRightClick, OpacityProperty, 0.7d, TimeSpan.FromMilliseconds(500d));
            };
            CheckBoxKeyboardRightClick.Unchecked += (sender, e) =>
            {
                if (CheckBoxKeyboardMode.IsChecked ?? false)
                    App.AnimateDoubleEffect(ViewActivateRightClick, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
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
            SelectSetKey?.IELSettingObject.BackgroundSetting.SetUsedState(false);
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
            SelectSetKey.Text = $"{(char)KeyInterop.VirtualKeyFromKey(key)}";
            SelectSetKey.FontStyle = FontStyles.Normal;
            SelectSetKey?.IELSettingObject.BackgroundSetting.SetUsedState(false);
            SelectSetKey = null;
        }
    }
}
