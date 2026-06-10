using OperPageLes.UI.UserElementsControl.Default;
using System.Windows;
using System.Windows.Controls;

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
        internal readonly StackPanel StackPanelAudioDevices;

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
            SliderVolume.MouseWheel += (sender, e) =>
            {
                if (SliderVolume.Value < SliderVolume.Maximum && SliderVolume.Value > SliderVolume.Minimum)
                    SliderVolume.Value += e.Delta < 0 ? -1f : 1f;
            };
            SliderVolume.ValueChanged += (sender, e) =>
            {
                int NewValue = (int)e.NewValue;
                App.CurrentApp.SettingMainApplication.Volume.Value = NewValue;
                TextBlockValue.Text = NewValue.ToString();
            };
            SliderVolume.Value = App.CurrentApp.SettingMainApplication.Volume;
            TextBlockValue.Text = ((int)SliderVolume.Value).ToString();
            //_ = DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
        }

        /// <summary>
        /// Обработка установки нового активного звукового устройства
        /// </summary>
        internal void SetActiveDeviceOutput(object Source, System.Windows.Input.MouseButtonEventArgs eventArgs)
        {
            if (ActiveIndexElement != -1)
            {
                ((OPLCheckAudioDevice)StackPanelAudioDevices.Children[ActiveIndexElement]).Activate = false;
            }
            OPLCheckAudioDevice Element = (OPLCheckAudioDevice)Source;
            Element.Activate = true;
            ActiveIndexElement = StackPanelAudioDevices.Children.IndexOf(Element);

            App.CurrentApp.SourcePlayControl.ChangeDevice(Element.IndexCurrentDevice);
        }
    }
}
