using LibraryPackKey.CORE;
using OperPageLes.CORE.Struct;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowInputProgramKey.xaml
    /// </summary>
    public partial class DialogInputProgramKey : Window
    {
        private StructPack Pack;

        private PackKey? ResultKey = null;

        public DialogInputProgramKey()
        {
            InitializeComponent();
            Pack = StructPack.NowPack;
            Icon = App.LoadImage(Properties.Resources.ValidKeyIcon);
            TextBlockPack.Foreground = new SolidColorBrush(Colors.Black);
            IELTextBoxKey.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 255, 0, 0));
            IELButtonCopyPack.OnActivateMouseLeft += (sender, e, Key) =>
            {
                System.Windows.Clipboard.SetText(TextBlockPack.Text);
                App.AnimateColorEffect(TextBlockPack.Foreground, SolidColorBrush.ColorProperty,
                    System.Windows.Media.Color.FromArgb(255, 0, 255, 0), Colors.Black,
                    TimeSpan.FromMilliseconds(1000d));
            };
            IELButtonUpdatePack.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Pack = StructPack.NowPack;
                TextBlockPack.Text = Pack.StringPack;
            };
            IELButtonCancel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                DialogResult Result = System.Windows.Forms.MessageBox.Show(
                    "При отмене ввода ключа приложение закроется.\nВы уперены что хотите выйти?", "Подтверждение действия",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result == System.Windows.Forms.DialogResult.Yes) Close();
            };
            IELButtonInput.OnActivateMouseLeft += (sender, e, Key) =>
            {
                CompleteInputKey();
            };
            IELTextBoxKey.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        CompleteInputKey();
                        break;
                    case Key.Escape:
                        Keyboard.ClearFocus();
                        break;
                }
            };
        }

        /// <summary>
        /// Открыть окно добавления ключа
        /// </summary>
        /// <returns>Состояние успешности проверки валидности ключа</returns>
        internal PackKey? SetKeyValid()
        {
            TextBlockPack.Text = Pack.StringPack;
            ShowDialog();
            return ResultKey;
        }

        /// <summary>
        /// Произвести попытку проверки валидности ключа
        /// </summary>
        private void CompleteInputKey()
        {
            try
            {
                ResultKey = PackKey.GenKey(Pack, IELTextBoxKey.Text);
                //if (File.Exists(App.DirectoryKeyValidFile)) File.Delete(App.DirectoryKeyValidFile);
                string SaveKeyOPL = $"{App.GetID()} {TextBlockPack.Text} {ResultKey.SourcePack.UnixTimeCode - 1} {IELTextBoxKey.Text}";
                File.WriteAllText(StructDirectoryResources.DirectoryKeyValidFile, Convert.ToHexString([..SaveKeyOPL.Select((i) => (byte)i)]));
                Close();
            }
            catch
            {
                App.AnimateColorEffect(IELTextBoxKey.Background, SolidColorBrush.ColorProperty,
                System.Windows.Media.Color.FromArgb(255, 255, 0, 0), System.Windows.Media.Color.FromArgb(0, 255, 0, 0),
                TimeSpan.FromMilliseconds(1000d));
            }
        }
    }
}
