using NAudio.CoreAudioApi;
using NAudio.Wave;
using OperPageLes.UI.UserElementsControl.Default;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Windows.Devices.Enumeration;

namespace OperPageLes.UI.Pages.ActionPanel.Other
{
    /// <summary>
    /// Логика взаимодействия для PageVolumeControl.xaml
    /// </summary>
    public partial class PageAudioControl : Page
    {
        /// <summary>
        /// Отображение массива девайсов
        /// </summary>
        private StackPanel StackPanelAudioDevices;

        /// <summary>
        /// Активный индекс аудио вывода
        /// </summary>
        private int ActiveIndexElement;

        public PageAudioControl()
        {
            InitializeComponent();
            StackPanelAudioDevices = new()
            {
                VerticalAlignment = VerticalAlignment.Top,
            };
            ScrollDevices.Content = StackPanelAudioDevices;
            SliderVolume.Value = App.CurrentApp.SettingMainApplication.Volume;
            SliderVolume.MouseWheel += (sender, e) =>
            {
                if (SliderVolume.Value < SliderVolume.Maximum && SliderVolume.Value > SliderVolume.Minimum)
                    SliderVolume.Value += e.Delta < 0 ? -1f : 1f;
            };
            SliderVolume.ValueChanged += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.Volume.Value = (int)SliderVolume.Value;
            };
            //_ = DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities Capabilities = WaveOut.GetCapabilities(i);

                OPLCheckAudioDevice VisualDevice = GetNewVisualAudioDevice(Capabilities);
                StackPanelAudioDevices.Children.Add(VisualDevice);
            }
        }

        /// <summary>
        /// Создать новый объект отображение аудио девайса
        /// </summary>
        /// <param name="SourceDevice">Объект информации аудио девайса</param>
        /// <returns></returns>
        private OPLCheckAudioDevice GetNewVisualAudioDevice(WaveOutCapabilities SourceDevice)
        {
            OPLCheckAudioDevice Result = new(SourceDevice)
            {
                Margin = new(3d),
                ManagerAnimation = App.ManagerAnimation,
            };
            Result.OnActivateMouseLeft += SetActiveDivaceOutput;
            return Result;
        }

        private void SetActiveDivaceOutput(object Source, System.Windows.Input.MouseButtonEventArgs eventArgs)
        {
            if (ActiveIndexElement != -1)
            {
                ((OPLCheckAudioDevice)StackPanelAudioDevices.Children[ActiveIndexElement]).Activate = false;
            }
            OPLCheckAudioDevice Element = (OPLCheckAudioDevice)Source;
            Element.Activate = true;
            ActiveIndexElement = StackPanelAudioDevices.Children.IndexOf(Element);

            App.CurrentApp.SourceWaveOut.DeviceNumber = ActiveIndexElement;
        }
    }
}
