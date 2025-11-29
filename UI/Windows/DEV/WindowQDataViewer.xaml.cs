using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Settings;
using ApplicationOperPageLes.CORE.Settings.PaletteElements;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes;
using IEL.GUI;
using System.IO;
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
        private readonly PaletteSpectrum PaletteSpectrumSource;
        private readonly System.Windows.Media.Brush[][] ArrayBrushSource;
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
                IELSourceButton.PaletteElement =
                    App.CurrentApp.SettingPaletteApplication.SourcePalette[(PaletteSpectrumEnum)ComboBoxSelectInitQData.SelectedIndex];

                ControlUpdateModeSetBrushQ(IELSourceButton.PaletteElement);
                UpdateCode();
                ButtonBack.IsEnabled = true;
            };

            ReadFileQData.Click += async (sender, e) =>
            {
                await App.CurrentApp.SettingPaletteApplication.ChangeSourcePalette(
                    new(File.ReadAllBytes("C:/Users/killm/Рабочий стол/Новая папка/QData.qd")));
            };

            WriteFileQData.Click += async (sender, e) =>
            {
                FileStream stream = File.OpenWrite("C:/Users/killm/Рабочий стол/Новая папка/QData.qd");
                stream.Position = 0;
                foreach (PaletteSpectrumEnum Element in Enum.GetValues<PaletteSpectrumEnum>())
                {
                    PaletteSpectrum spectrum = App.CurrentApp.SettingPaletteApplication.SourcePalette[Element];
                    await WriteQdata(stream, spectrum.BG);
                    await WriteQdata(stream, spectrum.BB);
                    await WriteQdata(stream, spectrum.FG);
                }
                stream.Close();
                stream.Dispose();
            };

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
            App.ColorAnimationType.AnimateEffect(ArrayBrushSource[ComboBoxSelectQData.SelectedIndex][(int)DataStateChange],
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
                TextBlockCodeView.Text +=
                    $"{source.Default.A} {source.Default.R} {source.Default.G} {source.Default.B}\n" +
                    $"{source.Select.A} {source.Select.R} {source.Select.G} {source.Select.B}\n" +
                    $"{source.Used.A} {source.Used.R} {source.Used.G} {source.Used.B}\n" +
                    $"{source.NotEnabled.A} {source.NotEnabled.R} {source.NotEnabled.G} {source.NotEnabled.B}";
                if (i < 2) TextBlockCodeView.Text += "\n";
            }
        }

        /// <summary>
        /// Прочитать из потока данных файла данные QData
        /// </summary>
        /// <param name="Stream">Поток файла</param>
        /// <returns></returns>
        /// <exception cref="Exception">Исключение несоответствия режима открытия файла</exception>
        private async Task<QData> ReadQdata(FileStream Stream)
        {
            if (!Stream.CanRead) throw new Exception("Поток работы с файлом не открыт для чтения!");
            byte[][] bytes = new byte[QData.CountSpectrumColor][];
            for (int i = 0; i < QData.CountSpectrumColor; i++)
            {
                bytes[i] = new byte[QData.CountBytesFromColor];
                IAsyncResult result = Stream.BeginRead(bytes[i], 0, QData.CountBytesFromColor, null, null);
                await Task.Run(() => { while (!result.IsCompleted); });
                Stream.EndRead(result);
            }
            return new QData(bytes);
        }

        /// <summary>
        /// Записать в поток данных файла данные QData
        /// </summary>
        /// <param name="Stream">Поток файла</param>
        /// <param name="Source">Данные спектров</param>
        /// <returns></returns>
        /// <exception cref="Exception">Исключение несоответствия режима открытия файла</exception>
        private async Task WriteQdata(FileStream Stream, QData Source)
        {
            if (!Stream.CanWrite) throw new Exception("Поток работы с файлом не открыт для записи!");
            var bytes = Source.GetSourceBytes();
            IAsyncResult result = Stream.BeginWrite(bytes, 0, bytes.Length, null, null);
            await Task.Run(() => { while (!result.IsCompleted); });
            Stream.EndWrite(result);
        }
    }
#endif
}
