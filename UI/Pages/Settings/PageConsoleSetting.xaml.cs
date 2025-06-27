using System.Windows.Controls;

namespace OperPage_les.UI.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для PageConsoleSetting.xaml
    /// </summary>
    public partial class PageConsoleSetting : Page
    {
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
        }
    }
}
