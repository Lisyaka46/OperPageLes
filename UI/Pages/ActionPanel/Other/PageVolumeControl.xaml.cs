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

namespace ApplicationOperPageLes.UI.Pages.ActionPanel.Other
{
    /// <summary>
    /// Логика взаимодействия для PageVolumeControl.xaml
    /// </summary>
    public partial class PageVolumeControl : Page
    {
        public PageVolumeControl()
        {
            InitializeComponent();
            SliderVolume.Value = App.CurrentApp.SettingMainApplication.Volume * 100;
            SliderVolume.MouseWheel += (sender, e) =>
            {
                if (SliderVolume.Value < SliderVolume.Maximum && SliderVolume.Value > SliderVolume.Minimum)
                    SliderVolume.Value += e.Delta < 0 ? -1f : 1f;
            };
            SliderVolume.ValueChanged += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.Volume.Value = (float)SliderVolume.Value / 100f;
            };
        }
    }
}
