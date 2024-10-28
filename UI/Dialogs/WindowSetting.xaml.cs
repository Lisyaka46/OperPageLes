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

namespace AAC20.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowSetting.xaml
    /// </summary>
    public partial class WindowSetting : Window
    {
        AAC20.UI. GeneralSetting;

        public WindowSetting()
        {
            InitializeComponent();
            GeneralSetting = new();
            GeneralSetting.EventChangeValue += (Name, Value) =>
            {
                App.CurrentApp.SettingApplication.GetSettingValue(CORE.Settings.EnumSettingApplication.PathMenuImage);
            };
            IELGeneralButton.OnActivateMouseLeft += () =>
            {
                IELFrameSetting.NextPage(GeneralSetting);
            };
        }
    }
}
