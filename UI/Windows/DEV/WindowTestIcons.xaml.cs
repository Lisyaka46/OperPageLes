using OperPageLes.CORE.Enums.Theme;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OperPageLes.UI.Windows.DEV
{
    /// <summary>
    /// Логика взаимодействия для WindowTestIcons.xaml
    /// </summary>
    public partial class WindowTestIcons : Window
    {
        private Array EnumThemeValues = Enum.GetValues<PaletteEnum>();

        public WindowTestIcons()
        {
            InitializeComponent();

            ComboBoxTheme.Items.Clear();

            ButtonSelectFileIcon.Click += (sender, e) =>
            {
                OpenFileDialog dialog = new()
                {
                    Multiselect = false,
                    Filter = "PNG files (*.png)|*.png",
                };
                dialog.ShowDialog();
                try
                {
                    BitmapImage bitmap = new(new Uri(dialog.FileName));
                    Icon64.Source = bitmap;
                    Icon45.Source = bitmap;
                    Icon90.Source = bitmap;
                    TextBlockFile.Text = dialog.FileName;
                }
                catch { }
            };

            ComboBoxTheme.SelectionChanged += (sender, e) =>
            {
                if (ComboBoxTheme.SelectedIndex == -1) return;
                PaletteEnum Spectrum = (PaletteEnum)(EnumThemeValues.GetValue(ComboBoxTheme.SelectedIndex) ?? throw new Exception());
                //App.CurrentApp.ActiveThemeApplication[Spectrum].ConnectPalleteFromIELElement(Icon64);
                //App.CurrentApp.ActiveThemeApplication[Spectrum].ConnectPalleteFromIELElement(Icon45);
                //App.CurrentApp.ActiveThemeApplication[Spectrum].ConnectPalleteFromIELElement(Icon90);
            };

            foreach (string item in Enum.GetNames<PaletteEnum>())
            {
                ComboBoxTheme.Items.Add(item);
            }



        }
    }
}
