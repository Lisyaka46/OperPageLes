using IEL.Classes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AAC20.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowGenLabel.xaml
    /// </summary>
    public partial class WindowGenLabel : Window
    {
        /// <summary>
        /// Состояние отмены создания ярлыка
        /// </summary>
        private bool Cancel = true;

        /// <summary>
        /// Анимация цвета
        /// </summary>
        private readonly ColorAnimation ButtonAnimationColor = new(Colors.Black, TimeSpan.FromMilliseconds(2000d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        public WindowGenLabel()
        {
            InitializeComponent();
            Width = 300;
            Height = 300;
            IELButtonCancel.OnActivateMouseLeft += delegate ()
            {
                Cancel = true;
                Close();
            };
            IELButtonCreateLabel.OnActivateMouseLeft += delegate ()
            {
                if (IELTextBoxNameLabel.Text.Length == 0)
                {
                    ButtonAnimationColor.From = Colors.Red;
                    ButtonAnimationColor.To = IELTextBoxNameLabel.BackgroundSetting.Default;
                    IELTextBoxNameLabel.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
                    return;
                }
                if (IELTextBoxCommand.Text.Length == 0)
                {
                    ButtonAnimationColor.From = Colors.Red;
                    ButtonAnimationColor.To = IELTextBoxCommand.BackgroundSetting.Default;
                    IELTextBoxCommand.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
                    return;
                }
                Cancel = false;
                Close();
            };
        }

        /// <summary>
        /// Создать ярлык с помощью диалогового окна
        /// </summary>
        /// <returns>Созданный объект ярлыка</returns>
        internal LabelAction? CreateLabel()
        {
            ShowDialog();
            if (Cancel) return null;
            return new(
                IELTextBoxNameLabel.Text,
                IELTextBoxDescription.Text.Length > 0 ? IELTextBoxDescription.Text : string.Empty,
                IELTextBoxCommand.Text);
        }
    }
}
