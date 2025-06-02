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
