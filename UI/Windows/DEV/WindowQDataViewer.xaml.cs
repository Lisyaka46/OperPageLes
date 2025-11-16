using IEL.CORE.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            IELSourceButton.QBackground.SetQData(ArrayQDataSource[0]);
            IELSourceButton.QBorderBrush.SetQData(ArrayQDataSource[1]);
            IELSourceButton.QForeground.SetQData(ArrayQDataSource[2]);
            CheckBoxIsEnabledController.Checked += (sender, e) =>
            {
                IELSourceButton.IsEnabled = true;
            };
            CheckBoxIsEnabledController.Unchecked += (sender, e) =>
            {
                IELSourceButton.IsEnabled = false;
            };
            
            ControlUpdateModeSetBrushQ();
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
                TextBlockCodeView.Text = String.Empty;
                for (int i = 0; i < 3; i++)
                    TextBlockCodeView.Text += $"<IEL:BrushSettingQ x:Key=\"{i switch { 0 => "BG", 1 => "BB", 2 => "FG", _ => "???"}}\" " +
                        $"DurationBrushSettingQ=\"00:00:00.2000000\"" +
                        $"Default=\"{ArrayQDataSource[i].GetFromSpectrumColor(QData.EnumDataSpectrum.Default)}\" " +
                        $"Select=\"{ArrayQDataSource[i].GetFromSpectrumColor(QData.EnumDataSpectrum.Select)}\" " +
                        $"Used=\"{ArrayQDataSource[i].GetFromSpectrumColor(QData.EnumDataSpectrum.Used)}\" " +
                        $"NotEnabled=\"{ArrayQDataSource[i].GetFromSpectrumColor(QData.EnumDataSpectrum.NotEnabled)}\"/>" +
                        $"{(i < 2 ? '\n' : '\0')}";
            };

            ButtonCopyCode.Click += (sender, e) =>
            {
                System.Windows.Clipboard.SetText(TextBlockCodeView.Text);
            };

            BorderDefaultSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(QData.EnumDataSpectrum.Default);
            BorderSelectSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(QData.EnumDataSpectrum.Select);
            BorderUsedSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(QData.EnumDataSpectrum.Used);
            BorderNotEnabledSpectrum.MouseLeftButtonUp += (sender, e) => SetNewColorFromDialog(QData.EnumDataSpectrum.NotEnabled);
        }

        private void ControlUpdateModeSetBrushQ()
        {
            BorderDefaultSpectrum.Background = new SolidColorBrush(ArrayQDataSource[0].GetFromSpectrumColor(QData.EnumDataSpectrum.Default));
            BorderSelectSpectrum.Background = new SolidColorBrush(ArrayQDataSource[0].GetFromSpectrumColor(QData.EnumDataSpectrum.Select));
            BorderUsedSpectrum.Background = new SolidColorBrush(ArrayQDataSource[0].GetFromSpectrumColor(QData.EnumDataSpectrum.Used));
            BorderNotEnabledSpectrum.Background = new SolidColorBrush(ArrayQDataSource[0].GetFromSpectrumColor(QData.EnumDataSpectrum.NotEnabled));

            BorderDefaultSpectrum.BorderBrush = new SolidColorBrush(ArrayQDataSource[1].GetFromSpectrumColor(QData.EnumDataSpectrum.Default));
            BorderSelectSpectrum.BorderBrush = new SolidColorBrush(ArrayQDataSource[1].GetFromSpectrumColor(QData.EnumDataSpectrum.Select));
            BorderUsedSpectrum.BorderBrush = new SolidColorBrush(ArrayQDataSource[1].GetFromSpectrumColor(QData.EnumDataSpectrum.Used));
            BorderNotEnabledSpectrum.BorderBrush = new SolidColorBrush(ArrayQDataSource[1].GetFromSpectrumColor(QData.EnumDataSpectrum.NotEnabled));

            TextBlockDefault.Foreground = new SolidColorBrush(ArrayQDataSource[2].GetFromSpectrumColor(QData.EnumDataSpectrum.Default));
            TextBlockSelect.Foreground = new SolidColorBrush(ArrayQDataSource[2].GetFromSpectrumColor(QData.EnumDataSpectrum.Select));
            TextBlockUsed.Foreground = new SolidColorBrush(ArrayQDataSource[2].GetFromSpectrumColor(QData.EnumDataSpectrum.Used));
            TextBlockNotEnabled.Foreground = new SolidColorBrush(ArrayQDataSource[2].GetFromSpectrumColor(QData.EnumDataSpectrum.NotEnabled));
        }

        private void SetNewColorFromDialog(QData.EnumDataSpectrum DataStateChange)
        {
            WnColor SourceColor = ArrayQDataSource[ComboBoxSelectQData.SelectedIndex].GetFromSpectrumColor(DataStateChange);
            Dialog.Color = System.Drawing.Color.FromArgb(SourceColor.A, SourceColor.R, SourceColor.G, SourceColor.B);
            System.Windows.Forms.DialogResult Result = Dialog.ShowDialog();
            if (Result == System.Windows.Forms.DialogResult.Cancel) return;
            WnColor ResultColor = WnColor.FromArgb(Dialog.Color.A, Dialog.Color.R, Dialog.Color.G, Dialog.Color.B);
            App.ColorAnimationType.AnimateEffect(ArrayBrushSource[ComboBoxSelectQData.SelectedIndex][(int)DataStateChange],
                        SolidColorBrush.ColorProperty, ResultColor, TimeSpan.FromMilliseconds(500d));
            ArrayQDataSource[ComboBoxSelectQData.SelectedIndex].SetFromSpectrumColor(DataStateChange, ResultColor);
        }
    }
#endif
}
