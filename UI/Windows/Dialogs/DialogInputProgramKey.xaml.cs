using LibraryPackKey.CORE;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Windows.Base;
using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            App.CurrentApp.LogWriteLine("Инициализация компонентов...");
            InitializeComponent();
            App.CurrentApp.LogWriteLine("...Готово");
            Pack = StructPack.NowPack;
            App.CurrentApp.LogWriteLine("...1");
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.ValidKeyIcon));
            App.CurrentApp.LogWriteLine("...2");
            TextBlockPack.Foreground = new SolidColorBrush(Colors.Black);
            IELTextBoxKey.Text = string.Empty;

            IELButtonCopyPack.OnActivateMouseLeft += (sender, e) =>
            {
                System.Windows.Forms.Clipboard.SetText(TextBlockPack.Text);
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, TextBlockPack.Foreground, SolidColorBrush.ColorProperty,
                    System.Windows.Media.Color.FromArgb(255, 0, 255, 0), Colors.Black, TimeSpan.FromMilliseconds(1000d));
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
            App.CurrentApp.LogWriteLine("! Инициализация диалога успешна");
        }

        /// <summary>
        /// Открыть окно добавления ключа
        /// </summary>
        /// <returns>Состояние успешности проверки валидности ключа</returns>
        internal PackKey? SetKeyValid(in OPLAnimationManager? Manager = null)
        {
            ManagerAnimation = Manager;
            TextBlockPack.Text = Pack.StringPack;
            Keyboard.PrimaryDevice.ClearFocus();
            if (ManagerAnimation != null)
            {
                TimeSpan SpanMiddle = TimeSpan.FromSeconds(2.5d);
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, this, OpacityProperty,
                    0d, 1d, SpanMiddle);
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, SourceScaleTransform, ScaleTransform.ScaleXProperty,
                    0.1d, 1d, SpanMiddle * 1.6d);
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation,SourceScaleTransform, ScaleTransform.ScaleYProperty,
                    0.1d, 1d, SpanMiddle * 1.6d);
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, SourceSkewTransform, SkewTransform.AngleYProperty,
                    80d, 0d, SpanMiddle);
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, SourceRotateTransform, RotateTransform.AngleProperty,
                    -90d, 0d, SpanMiddle / 1.4d);
            }
            App.CurrentApp.LogWriteLine("Открытие диалога");
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
                if (ManagerAnimation != null)
                {
                    TimeSpan SpanMiddle = TimeSpan.FromSeconds(1d);
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, this, OpacityProperty,
                        0d, SpanMiddle);
                    DoubleAnimation animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                    animation.Duration = SpanMiddle * 1.1d;
                    animation.To = 0.1;
                    animation.FillBehavior = FillBehavior.Stop;
                    animation.Completed += (sender, e) =>
                    {
                        Close();
                    };
                    SourceScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, SourceScaleTransform, ScaleTransform.ScaleYProperty,
                        0.1d, SpanMiddle * 1.1d);

                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, SourceSkewTransform, SkewTransform.AngleYProperty,
                        -40d, SpanMiddle);

                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, SourceRotateTransform, RotateTransform.AngleProperty,
                        50d, SpanMiddle);
                }
                else
                {
                    Close();
                }
            }
            catch
            {
                Keyboard.ClearFocus();
                IELTextBoxKey.SourceBackground.SetActiveSpecrum(Colors.Red);
            }
        }
    }
}
