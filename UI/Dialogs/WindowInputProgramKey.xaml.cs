using ConsoleManipulateKey.CORE;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace OperPage_les.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowInputProgramKey.xaml
    /// </summary>
    public partial class WindowInputProgramKey : Window
    {
        bool Cancel = true;

        public WindowInputProgramKey()
        {
            InitializeComponent();
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
                TextBlockPack.Text = Manipulate.GenerateKeyPack();
            };
            IELButtonCancel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                DialogResult Result = System.Windows.Forms.MessageBox.Show("При отмене ввода ключа приложение закроется.\nВы уперены что хотите выйти?", "Подтверждение действия",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                Cancel = true;
                if (Result == System.Windows.Forms.DialogResult.Yes) Close();
            };
            IELButtonInput.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Check();
            };
            IELTextBoxKey.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        Check();
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
        internal bool SetKeyValid()
        {
            TextBlockPack.Text = Manipulate.GenerateKeyPack();
            ShowDialog();
            return !Cancel;
        }

        /// <summary>
        /// Произвести попытку проверки валидности ключа
        /// </summary>
        private void Check()
        {
            try
            {
                Cancel = !Manipulate.CheckKeyValid(TextBlockPack.Text, IELTextBoxKey.Text);
            }
            catch
            {
                Cancel = true;
            }
            if (!Cancel)
            {
                System.IO.File.WriteAllText(App.DirectoryKeyValidFile, $"{Manipulate.GetCodeUUID()} {TextBlockPack.Text} {IELTextBoxKey.Text}");
                Close();
            }
            else App.AnimateColorEffect(IELTextBoxKey.Background, SolidColorBrush.ColorProperty,
                System.Windows.Media.Color.FromArgb(255, 255, 0, 0), System.Windows.Media.Color.FromArgb(0, 255, 0, 0),
                TimeSpan.FromMilliseconds(1000d));
        }
    }
}
