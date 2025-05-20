using IEL.CORE.Classes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace OperPage_les.UI.Dialogs
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
            Width = 315d;
            Height = 336d;
            IELButtonCancel.OnActivateMouseLeft += (Key) =>
            {
                Cancel = true;
                Close();
            };
            IELButtonCreateLabel.OnActivateMouseLeft += (Key) =>
            {
                Create();
            };
            IELTextBoxNameLabel.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Down:
                    case Key.Enter:
                        IELTextBoxCommand.Focus();
                        break;
                    case Key.Escape:
                        Close();
                        break;
                }
            };
            IELTextBoxCommand.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Down:
                    case Key.Enter:
                        IELTextBoxDescription.Focus();
                        break;
                    case Key.Up:
                        IELTextBoxNameLabel.Focus();
                        break;
                    case Key.Escape:
                        Close();
                        break;
                }
            };
            IELTextBoxDescription.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        Create();
                        break;
                    case Key.Up:
                        IELTextBoxCommand.Focus();
                        break;
                    case Key.Escape:
                        Close();
                        break;
                }
            };
            Loaded += (sender, e) =>
            {
                IELTextBoxNameLabel.Focus();
            };
        }
        
        /// <summary>
        /// Сгенерировать итоговое исполнение создания ярлыка
        /// </summary>
        private void Create()
        {
            if (IELTextBoxNameLabel.Text.Length == 0)
            {
                ButtonAnimationColor.From = Colors.Red;
                ButtonAnimationColor.To = IELTextBoxNameLabel.IELSettingObject.BackgroundSetting.Default;
                IELTextBoxNameLabel.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
                return;
            }
            if (IELTextBoxCommand.Text.Length == 0)
            {
                ButtonAnimationColor.From = Colors.Red;
                ButtonAnimationColor.To = IELTextBoxCommand.IELSettingObject.BackgroundSetting.Default;
                IELTextBoxCommand.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
                return;
            }
            Cancel = false;
            Close();
        }

        /// <summary>
        /// Создать ярлык с помощью диалогового окна
        /// </summary>
        /// <returns>Созданный объект ярлыка</returns>
        internal LabelAction CreateLabel()
        {
            Title = "Создание ярлыка";
            IELButtonCreateLabel.Text = "Создать ярлык";
            Focus();
            ShowDialog();
            if (Cancel) return LabelAction.Empty;
            return new(
                IELTextBoxNameLabel.Text,
                IELTextBoxDescription.Text.Length > 0 ? IELTextBoxDescription.Text : string.Empty,
                IELTextBoxCommand.Text);
        }

        /// <summary>
        /// Изменить ярлык с помощью диалогового окна
        /// </summary>
        /// <param name="Source">Изменяемый объект ярлыка</param>
        /// <returns>Изменённый объект ярлыка</returns>
        internal LabelAction ChangeLabel(LabelAction Source)
        {
            IELTextBoxNameLabel.Text = Source.Name;
            IELTextBoxDescription.Text = Source.Description ?? string.Empty;
            IELTextBoxCommand.Text = Source.Command;
            IELButtonCreateLabel.Text = "Изменить ярлык";
            Title = "Изменение ярлыка";
            ShowDialog();
            if (Cancel ||
                (
                    IELTextBoxNameLabel.Text.Equals(Source.Name) &&
                    IELTextBoxDescription.Text.Equals(Source.Description) &&
                    IELTextBoxCommand.Text.Equals(Source.Command)
                )) return Source;
            return new(
                IELTextBoxNameLabel.Text,
                IELTextBoxDescription.Text.Length > 0 ? IELTextBoxDescription.Text : string.Empty,
                IELTextBoxCommand.Text);
        }
    }
}
