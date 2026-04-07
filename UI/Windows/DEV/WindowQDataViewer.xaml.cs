using OperPageLes.CORE.Enums;
using IEL.CORE.Classes;
using System.Windows;
using System.Windows.Media;
using static IEL.CORE.Classes.QData;
using WnColor = System.Windows.Media.Color;

namespace OperPageLes.UI.Windows.DEV
{
#if DEBUG
    /// <summary>
    /// Логика взаимодействия для WindowQDataViewer.xaml
    /// </summary>
    public partial class WindowQDataViewer : Window
    {
        private ColorDialog Dialog;
        private readonly PaletteSpectrum PaletteSpectrumSource;
        private readonly System.Windows.Media.Brush[][] ArrayBrushSource;
        private string SaveNamePalette = string.Empty;
        public WindowQDataViewer()
        {
            /*
             <IEL:BrushSettingQ x:Key="FG_PastelBlue" DurationBrushSettingQ="00:00:00.2000000" Default="Black" Select="Black" Used="Black" NotEnabled="Black"/>
             */
            Dialog = new();
            PaletteSpectrumSource = new();
            InitializeComponent();
            TextBlockCodeView.Text = String.Empty;
            CheckBoxIsEnabledController.IsChecked = IELSourceButton.IsEnabled;
            ButtonBack.IsEnabled = false;
            IELSourceButton.PaletteElement = PaletteSpectrumSource;
            ButtonBack.Click += (sender, e) =>
            {
                ButtonBack.IsEnabled = false;
                ControlUpdateModeSetBrushQ(PaletteSpectrumSource);
                IELSourceButton.PaletteElement = PaletteSpectrumSource;

                TextGeneratePalette.Text = SaveNamePalette;
                TextGeneratePalette.IsEnabled = true;
                UpdateCode();
            };
            CheckBoxIsEnabledController.Checked += (sender, e) =>
            {
                IELSourceButton.IsEnabled = true;
            };
            CheckBoxIsEnabledController.Unchecked += (sender, e) =>
            {
                IELSourceButton.IsEnabled = false;
            };
            
            ControlUpdateModeSetBrushQ(PaletteSpectrumSource);
            ArrayBrushSource =
                [
                [BorderDefaultSpectrum.Background, BorderSelectSpectrum.Background, BorderUsedSpectrum.Background, BorderNotEnabledSpectrum.Background],
                [BorderDefaultSpectrum.BorderBrush, BorderSelectSpectrum.BorderBrush, BorderUsedSpectrum.BorderBrush, BorderNotEnabledSpectrum.BorderBrush],
                [TextBlockDefault.Foreground, TextBlockSelect.Foreground, TextBlockUsed.Foreground, TextBlockNotEnabled.Foreground]
                ];

            ComboBoxSelectQData.SelectedIndex = 0;

            IELSourceButton.OnActivateMouseLeft += (sender, e) => { };
            IELSourceButton.OnActivateMouseRight += (sender, e) => { };

            ButtonCreateCode.Click += (sender, e) =>
            {
                UpdateCode();
            };

            ButtonCopyCode.Click += (sender, e) =>
            {
                System.Windows.Clipboard.SetText(TextBlockCodeView.Text);
            };

            InicializeQData.Click += (sender, e) =>
            {
                if (ComboBoxSelectInitQData.SelectedIndex < 0) return;
                IELSourceButton.PaletteElement =
                    App.CurrentApp.ActiveThemeApplication[(PaletteSpectrumEnum)ComboBoxSelectInitQData.SelectedIndex];
                SaveNamePalette = TextGeneratePalette.Text;
                TextGeneratePalette.Text = ((PaletteSpectrumEnum)ComboBoxSelectInitQData.SelectedIndex).ToString();
                TextGeneratePalette.IsEnabled = false;
                ControlUpdateModeSetBrushQ(IELSourceButton.PaletteElement);
                UpdateCode();
                ButtonBack.IsEnabled = true;
            };

            //WriteFileQData.Click += async (sender, e) =>
            //{
            //    FileStream stream = File.OpenWrite("C:/Users/killm/Рабочий стол/Новая папка/QData.qd");
            //    stream.Position = 0;
            //    foreach (PaletteSpectrumEnum Element in Enum.GetValues<PaletteSpectrumEnum>())
            //    {
            //        PaletteSpectrum spectrum = App.CurrentApp.ActiveThemeApplication[Element];
            //        await spectrum.BG.WriteQdata(stream);
            //        await spectrum.BB.WriteQdata(stream);
            //        await spectrum.FG.WriteQdata(stream);
            //    }
            //    stream.Close();
            //    stream.Dispose();
            //};

            BorderDefaultSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(EnumDataSpectrum.Default);
            BorderSelectSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(EnumDataSpectrum.Select);
            BorderUsedSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(EnumDataSpectrum.Used);
            BorderNotEnabledSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(EnumDataSpectrum.NotEnabled);
            CreateAllPaletteButtons(ComboBoxSelectInitQData);
        }

        private void ControlUpdateModeSetBrushQ(PaletteSpectrum ElementSpectrum)
        {
            BorderDefaultSpectrum.Background = new SolidColorBrush(ElementSpectrum.BG.GetFromSpectrumColor(EnumDataSpectrum.Default));
            BorderSelectSpectrum.Background = new SolidColorBrush(ElementSpectrum.BG.GetFromSpectrumColor(EnumDataSpectrum.Select));
            BorderUsedSpectrum.Background = new SolidColorBrush(ElementSpectrum.BG.GetFromSpectrumColor(EnumDataSpectrum.Used));
            BorderNotEnabledSpectrum.Background = new SolidColorBrush(ElementSpectrum.BG.GetFromSpectrumColor(EnumDataSpectrum.NotEnabled));

            BorderDefaultSpectrum.BorderBrush = new SolidColorBrush(ElementSpectrum.BB.GetFromSpectrumColor(EnumDataSpectrum.Default));
            BorderSelectSpectrum.BorderBrush = new SolidColorBrush(ElementSpectrum.BB.GetFromSpectrumColor(EnumDataSpectrum.Select));
            BorderUsedSpectrum.BorderBrush = new SolidColorBrush(ElementSpectrum.BB.GetFromSpectrumColor(EnumDataSpectrum.Used));
            BorderNotEnabledSpectrum.BorderBrush = new SolidColorBrush(ElementSpectrum.BB.GetFromSpectrumColor(EnumDataSpectrum.NotEnabled));

            TextBlockDefault.Foreground = new SolidColorBrush(ElementSpectrum.FG.GetFromSpectrumColor(EnumDataSpectrum.Default));
            TextBlockSelect.Foreground = new SolidColorBrush(ElementSpectrum.FG.GetFromSpectrumColor(EnumDataSpectrum.Select));
            TextBlockUsed.Foreground = new SolidColorBrush(ElementSpectrum.FG.GetFromSpectrumColor(EnumDataSpectrum.Used));
            TextBlockNotEnabled.Foreground = new SolidColorBrush(ElementSpectrum.FG.GetFromSpectrumColor(EnumDataSpectrum.NotEnabled));
        }

        private void SetNewColorFromDialog(EnumDataSpectrum DataStateChange)
        {
            QData qd = ComboBoxSelectQData.SelectedIndex switch
            {
                0 => IELSourceButton.Background,
                1 => IELSourceButton.BorderBrush,
                2 => IELSourceButton.Foreground,
                _ => throw new Exception("Недоступное значение выделенного индекса.")
            };
            WnColor SourceColor = qd.GetFromSpectrumColor(DataStateChange);
            Dialog.Color = System.Drawing.Color.FromArgb(SourceColor.A, SourceColor.R, SourceColor.G, SourceColor.B);
            DialogResult Result = Dialog.ShowDialog();
            if (Result == System.Windows.Forms.DialogResult.Cancel) return;
            WnColor ResultColor = WnColor.FromArgb(Dialog.Color.A, Dialog.Color.R, Dialog.Color.G, Dialog.Color.B);
            App.ManagerAnimation.ColorAnimationType.AnimateEffect(ArrayBrushSource[ComboBoxSelectQData.SelectedIndex][(int)DataStateChange],
                        SolidColorBrush.ColorProperty, ResultColor, TimeSpan.FromMilliseconds(500d));
            qd.SetFromSpectrumColor(DataStateChange, ResultColor);
            ControlUpdateModeSetBrushQ(IELSourceButton.PaletteElement);
        }

        private System.Windows.Controls.ComboBox CreateAllPaletteButtons(System.Windows.Controls.ComboBox ResultComboBox)
        {
            ResultComboBox.Items.Clear();
            foreach (PaletteSpectrumEnum ElementPalette in Enum.GetValues<PaletteSpectrumEnum>())
            {
                ResultComboBox.Items.Add(ElementPalette.ToString());
            }
            return ResultComboBox;
        }

        private void UpdateCode()
        {
            TextBlockCodeView.Text =
                $"<IEL:PaletteSpectrum x:Key=\"{TextGeneratePalette.Text}\">\r\n" +
                "\t<IEL:PaletteSpectrum.BG>\r\n" +
                $"\t\t<IEL:QData Default=\"{IELSourceButton.Background.Default}\" Select=\"{IELSourceButton.Background.Select}\" " +
                    $"Used=\"{IELSourceButton.Background.Used}\" NotEnabled=\"{IELSourceButton.Background.NotEnabled}\"/>\r\n" +
                "\t</IEL:PaletteSpectrum.BG>\r\n" +
                "\t<IEL:PaletteSpectrum.BB>\r\n" +
                $"\t\t<IEL:QData Default=\"{IELSourceButton.BorderBrush.Default}\" Select=\"{IELSourceButton.BorderBrush.Select}\" " +
                    $"Used=\"{IELSourceButton.BorderBrush.Used}\" NotEnabled=\"{IELSourceButton.BorderBrush.NotEnabled}\"/>\r\n" +
                "\t</IEL:PaletteSpectrum.BB>\r\n" +
                "\t<IEL:PaletteSpectrum.FG>\r\n" +
                $"\t\t<IEL:QData Default=\"{IELSourceButton.Foreground.Default}\" Select=\"{IELSourceButton.Foreground.Select}\" " +
                    $"Used=\"{IELSourceButton.Foreground.Used}\" NotEnabled=\"{IELSourceButton.Foreground.NotEnabled}\"/>\r\n" +
                "\t</IEL:PaletteSpectrum.FG>\r\n" +
                "</IEL:PaletteSpectrum>";
        }
    }
#endif
}
