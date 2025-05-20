using IEL.CORE.Enums;

namespace OperPage_les.Windows.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : System.Windows.Controls.Page
    {
        public PageDeveloper()
        {
            InitializeComponent();
            ComboBoxStateInlay.SelectionChanged += (sender, e) =>
            {
                StateSpectrum Spectrum = ComboBoxStateInlay.SelectedIndex switch
                {
                    0 => StateSpectrum.Default,
                    1 => StateSpectrum.NotEnabled,
                    2 => StateSpectrum.Select,
                    3 => StateSpectrum.Used,
                    _ => StateSpectrum.Default,
                };
                HeadInlay.IELSettingObject.BackgroundSetting.InvokeObjectUsedStateColor(Spectrum);
                HeadInlay.IELSettingObject.BorderBrushSetting.InvokeObjectUsedStateColor(Spectrum);
                HeadInlay.IELSettingObject.ForegroundSetting.InvokeObjectUsedStateColor(Spectrum);
            };
            ButtonPixelColorCopy.Click += (sender, e) =>
            {
                System.Windows.Clipboard.SetText(TextBoxColorCode.Text);
            };
        }   
    }
}
