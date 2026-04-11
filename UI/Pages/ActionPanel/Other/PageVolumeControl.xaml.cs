using NAudio.CoreAudioApi;
using NAudio.Wave;
using OperPageLes.UI.UserElementsControl.Default;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using System.Xml.Linq;

namespace OperPageLes.UI.Pages.ActionPanel.Other
{
    /// <summary>
    /// Логика взаимодействия для PageVolumeControl.xaml
    /// </summary>
    public partial class PageAudioControl : Page
    {
        /// <summary>
        /// Объект управляемый перечислением аудио девайсов
        /// </summary>
        private MMDeviceEnumerator DeviceEnumerator;

        /// <summary>
        /// Отображение массива девайсов
        /// </summary>
        private StackPanel StackPanelAudioDevices;

        /// <summary>
        /// Активный индекс аудио вывода
        /// </summary>
        private int ActiveIndex;

        public PageAudioControl()
        {
            InitializeComponent();
            DeviceEnumerator = new();
            StackPanelAudioDevices = new()
            {
                VerticalAlignment = VerticalAlignment.Top,
            };
            ScrollDevices.Content = StackPanelAudioDevices;
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
            _ = DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            foreach (MMDevice Element in DeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                OPLCheckAudioDevice VisualDevice = GetNewVisualAudioDevice(in Element);
                StackPanelAudioDevices.Children.Add(VisualDevice);
            }
        }

        /// <summary>
        /// Создать новый объект отображение аудио девайса
        /// </summary>
        /// <param name="SourceDevice">Объект информации аудио девайса</param>
        /// <returns></returns>
        private OPLCheckAudioDevice GetNewVisualAudioDevice(in MMDevice? SourceDevice)
        {
            OPLCheckAudioDevice Result = new(SourceDevice)
            {
                Margin = new(3d),
                ManagerAnimation = App.ManagerAnimation,
            };
            Result.OnActivateMouseLeft += (sender, e) =>
            {
                ((OPLCheckAudioDevice)StackPanelAudioDevices.Children[ActiveIndex]).Activate = false;
                OPLCheckAudioDevice OPLDevice = (OPLCheckAudioDevice)sender;
                OPLDevice.Activate = true;
                ActiveIndex = StackPanelAudioDevices.Children.IndexOf(OPLDevice);
                App.CurrentApp.SoundChannelWaveOut.DeviceNumber = ActiveIndex;
            };
            return Result;
        }
    }
}
