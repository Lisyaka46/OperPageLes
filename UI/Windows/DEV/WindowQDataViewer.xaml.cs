using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Settings;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes;
using IEL.GUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using static IEL.CORE.Classes.QData;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using WnColor = System.Windows.Media.Color;

namespace ApplicationOperPageLes.UI.Windows.DEV
{
#if DEBUG
    /// <summary>
    /// Логика взаимодействия для WindowQDataViewer.xaml
    /// </summary>
    public partial class WindowQDataViewer : Window
    {
        private ColorDialog Dialog;
        private readonly QData[] ArrayQDataSource;
        private readonly System.Windows.Media.Brush[][] ArrayBrushSource;
        public WindowQDataViewer()
        {
            /*
             <IEL:BrushSettingQ x:Key="FG_PastelBlue" DurationBrushSettingQ="00:00:00.2000000" Default="Black" Select="Black" Used="Black" NotEnabled="Black"/>
             */
            Dialog = new();
            ArrayQDataSource = [new(), new(), new()];
            InitializeComponent();
            TextBlockCodeView.Text = String.Empty;
            CheckBoxIsEnabledController.IsChecked = IELSourceButton.IsEnabled;
            ButtonBack.IsEnabled = false;
            SetSource();
            ButtonBack.Click += (sender, e) =>
            {
                ButtonBack.IsEnabled = false;
                ControlUpdateModeSetBrushQ(ArrayQDataSource);
                SetSource();
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
            
            ControlUpdateModeSetBrushQ(ArrayQDataSource);
            ArrayBrushSource =
                [
                [BorderDefaultSpectrum.Background, BorderSelectSpectrum.Background, BorderUsedSpectrum.Background, BorderNotEnabledSpectrum.Background],
                [BorderDefaultSpectrum.BorderBrush, BorderSelectSpectrum.BorderBrush, BorderUsedSpectrum.BorderBrush, BorderNotEnabledSpectrum.BorderBrush],
                [TextBlockDefault.Foreground, TextBlockSelect.Foreground, TextBlockUsed.Foreground, TextBlockNotEnabled.Foreground]
                ];

            ComboBoxSelectQData.SelectedIndex = 0;

            IELSourceButton.OnActivateMouseLeft += (sender, e, Key) => { };
            IELSourceButton.OnActivateMouseRight += (sender, e, Key) => { };

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
                IELSourceButton.Background =
                    App.SettingPaletteApplication.GetQdataFromEnum((PaletteValuesEnum)(ComboBoxSelectInitQData.SelectedIndex * 3));

                IELSourceButton.BorderBrush =
                    App.SettingPaletteApplication.GetQdataFromEnum((PaletteValuesEnum)(ComboBoxSelectInitQData.SelectedIndex * 3 + 1));

                IELSourceButton.Foreground =
                    App.SettingPaletteApplication.GetQdataFromEnum((PaletteValuesEnum)(ComboBoxSelectInitQData.SelectedIndex * 3 + 2));

                ControlUpdateModeSetBrushQ([IELSourceButton.Background, IELSourceButton.BorderBrush, IELSourceButton.Foreground]);
                UpdateCode();
                ButtonBack.IsEnabled = true;
            };

            BorderDefaultSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(EnumDataSpectrum.Default);
            BorderSelectSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(EnumDataSpectrum.Select);
            BorderUsedSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(EnumDataSpectrum.Used);
            BorderNotEnabledSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(EnumDataSpectrum.NotEnabled);
            CreateAllPaletteButtons(ComboBoxSelectInitQData);
        }

        private void ControlUpdateModeSetBrushQ(QData[] ArraySpectrum)
        {
            BorderDefaultSpectrum.Background = new SolidColorBrush(ArraySpectrum[0].GetFromSpectrumColor(EnumDataSpectrum.Default));
            BorderSelectSpectrum.Background = new SolidColorBrush(ArraySpectrum[0].GetFromSpectrumColor(EnumDataSpectrum.Select));
            BorderUsedSpectrum.Background = new SolidColorBrush(ArraySpectrum[0].GetFromSpectrumColor(EnumDataSpectrum.Used));
            BorderNotEnabledSpectrum.Background = new SolidColorBrush(ArraySpectrum[0].GetFromSpectrumColor(EnumDataSpectrum.NotEnabled));

            BorderDefaultSpectrum.BorderBrush = new SolidColorBrush(ArraySpectrum[1].GetFromSpectrumColor(EnumDataSpectrum.Default));
            BorderSelectSpectrum.BorderBrush = new SolidColorBrush(ArraySpectrum[1].GetFromSpectrumColor(EnumDataSpectrum.Select));
            BorderUsedSpectrum.BorderBrush = new SolidColorBrush(ArraySpectrum[1].GetFromSpectrumColor(EnumDataSpectrum.Used));
            BorderNotEnabledSpectrum.BorderBrush = new SolidColorBrush(ArraySpectrum[1].GetFromSpectrumColor(EnumDataSpectrum.NotEnabled));

            TextBlockDefault.Foreground = new SolidColorBrush(ArraySpectrum[2].GetFromSpectrumColor(EnumDataSpectrum.Default));
            TextBlockSelect.Foreground = new SolidColorBrush(ArraySpectrum[2].GetFromSpectrumColor(EnumDataSpectrum.Select));
            TextBlockUsed.Foreground = new SolidColorBrush(ArraySpectrum[2].GetFromSpectrumColor(EnumDataSpectrum.Used));
            TextBlockNotEnabled.Foreground = new SolidColorBrush(ArraySpectrum[2].GetFromSpectrumColor(EnumDataSpectrum.NotEnabled));
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
            App.ColorAnimationType.AnimateEffect(ArrayBrushSource[ComboBoxSelectQData.SelectedIndex][(int)DataStateChange],
                        SolidColorBrush.ColorProperty, ResultColor, TimeSpan.FromMilliseconds(500d));
            qd.SetFromSpectrumColor(DataStateChange, ResultColor);
            ControlUpdateModeSetBrushQ([IELSourceButton.Background, IELSourceButton.BorderBrush, IELSourceButton.Foreground]);
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
            TextBlockCodeView.Text = String.Empty;
            QData source;
            for (int i = 0; i < 3; i++)
            {
                source = i switch
                {
                    0 => IELSourceButton.Background,
                    1 => IELSourceButton.BorderBrush,
                    2 => IELSourceButton.Foreground,
                    _ => throw new Exception("Недоступное значение выделенного индекса.")
                };
                TextBlockCodeView.Text += $"<IEL:QData x:Key=\"{i switch { 0 => "BG", 1 => "BB", 2 => "FG", _ => "???" }}\" " +
                    $"Default=\"{source.GetFromSpectrumColor(QData.EnumDataSpectrum.Default)}\" " +
                    $"Select=\"{source.GetFromSpectrumColor(QData.EnumDataSpectrum.Select)}\" " +
                    $"Used=\"{source.GetFromSpectrumColor(QData.EnumDataSpectrum.Used)}\" " +
                    $"NotEnabled=\"{source.GetFromSpectrumColor(QData.EnumDataSpectrum.NotEnabled)}\"/>" +
                    $"{(i < 2 ? '\n' : '\0')}";
            }
        }

        private void SetSource()
        {
            IELSourceButton.Background = ArrayQDataSource[0];
            IELSourceButton.BorderBrush = ArrayQDataSource[1];
            IELSourceButton.Foreground = ArrayQDataSource[2];
        }
    }
#endif
}
