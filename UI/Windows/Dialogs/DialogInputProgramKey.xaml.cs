using LibraryPackKey.CORE;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Windows.Base;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowInputProgramKey.xaml
    /// </summary>
    public partial class DialogInputProgramKey : OPLWindowBase
    {
        private StructPack Pack;

        private PackKey? ResultKey = null;

        public DialogInputProgramKey()
        {
            InitializeComponent();
            ManagerAnimation = App.ManagerAnimation;
            Pack = StructPack.NowPack;
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.ValidKeyIcon));
            TextBlockPack.Foreground = new SolidColorBrush(Colors.Black);
            IELTextBoxKey.Text = string.Empty;

            IELButtonCopyPack.OnActivateMouseLeft += (sender, e) =>
            {
                System.Windows.Forms.Clipboard.SetText(TextBlockPack.Text);
                App.ManagerAnimation.ColorAnimationType.AnimateEffect(TextBlockPack.Foreground, SolidColorBrush.ColorProperty,
                    System.Windows.Media.Color.FromArgb(255, 0, 255, 0), Colors.Black,
                    TimeSpan.FromMilliseconds(1000d));
            };
            IELButtonUpdatePack.OnActivateMouseLeft += (sender, e) =>
            {
                Pack = StructPack.NowPack;
                TextBlockPack.Text = Pack.StringPack;
            };
            IELButtonCancel.OnActivateMouseLeft += (sender, e) =>
            {
                DialogResult Result = System.Windows.Forms.MessageBox.Show(
                    "При отмене ввода ключа приложение закроется.\nВы уперены что хотите выйти?", "Подтверждение действия",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result == System.Windows.Forms.DialogResult.Yes) Close();
            };
            IELButtonInput.OnActivateMouseLeft += (sender, e) =>
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
            Keyboard.PrimaryDevice.ClearFocus();
            if (ManagerAnimation != null)
            {
                TimeSpan SpanFast = TimeSpan.FromSeconds(2d);
                TimeSpan SpanMiddle = TimeSpan.FromSeconds(2.5d);
                ManagerAnimation.DoubleAnimationType.AnimateEffect(this, OpacityProperty, 0d, 1d, SpanMiddle);
                ManagerAnimation.DoubleAnimationType.AnimateEffect(SourceScaleTransform, ScaleTransform.ScaleXProperty, 0.2d, 1d, SpanMiddle);
                ManagerAnimation.DoubleAnimationType.AnimateEffect(SourceScaleTransform, ScaleTransform.ScaleYProperty, 0.2d, 1d, SpanMiddle);

                ManagerAnimation.DoubleAnimationType.AnimateEffect(SourceSkewTransform, SkewTransform.AngleXProperty, 40d, 0d, SpanFast);
                ManagerAnimation.DoubleAnimationType.AnimateEffect(SourceSkewTransform, SkewTransform.AngleYProperty, 40d, 0d, SpanFast);
            }
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
                Close();
            }
            catch
            {
                Keyboard.ClearFocus();
                IELTextBoxKey.SourceBackground.SetActiveSpecrum(Colors.Red);
            }
        }
    }
}
