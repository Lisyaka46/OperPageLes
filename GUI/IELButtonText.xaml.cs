using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AAC20.GUI
{
    /// <summary>
    /// Логика взаимодействия для IELButtonText.xaml
    /// </summary>
    public partial class IELButtonText : UserControl
    {
        private Color _DefaultBorderBrush;
        /// <summary>
        /// Цвет границы кнопки
        /// </summary>
        public Color DefaultBorderBrush
        {
            get => _DefaultBorderBrush;
            set
            {
                BorderButton.BorderBrush = new SolidColorBrush(value);
                _DefaultBorderBrush = value;
            }
        }

        private Color _DefaultBackground;
        /// <summary>
        /// Цвет фона кнопки
        /// </summary>
        public Color DefaultBackground
        {
            get => _DefaultBackground;
            set
            {
                BorderButton.Background = new SolidColorBrush(value);
                _DefaultBackground = value;
            }
        }

        private Color _DefaultForeground;
        /// <summary>
        /// Цвет текста в кнопке
        /// </summary>
        public Color DefaultForeground
        {
            get => _DefaultForeground;
            set
            {
                TextBlockButton.Foreground = new SolidColorBrush(value);
                _DefaultForeground = value;
            }
        }

        /// <summary>
        /// Выделенный цвет границы кнопки
        /// </summary>
        public Color SelectBorderBrush { get; set; }

        /// <summary>
        /// Выделенный цвет фона кнопки
        /// </summary>
        public Color SelectBackground { get; set; }

        /// <summary>
        /// Выделенный цвет текста в кнопке
        /// </summary>
        public Color SelectForeground { get; set; }

        /// <summary>
        /// Нажатый цвет границы кнопки
        /// </summary>
        public Color ClickedBorderBrush { get; set; }

        /// <summary>
        /// Нажатый цвет фона кнопки
        /// </summary>
        public Color ClickedBackground { get; set; }

        /// <summary>
        /// Нажатый цвет текста в кнопке
        /// </summary>
        public Color ClickedForeground { get; set; }

        /// <summary>
        /// Выключенный цвет границы кнопки
        /// </summary>
        public Color NotEnabledBorderBrush { get; set; }

        /// <summary>
        /// Выключенный цвет фона кнопки
        /// </summary>
        public Color NotEnabledBackground { get; set; }

        /// <summary>
        /// Выключенный цвет текста в кнопке
        /// </summary>
        public Color NotEnabledForeground { get; set; }

        private int _AnimationMillisecond = 0;
        /// <summary>
        /// Количество миллисекунд для анимации (по умолчанию 80)
        /// </summary>
        public int AnimationMillisecond
        {
            get => _AnimationMillisecond;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Значение должно быть больше нуля!");
                else _AnimationMillisecond = value;
            }
        }

        /// <summary>
        /// Текст кнопки
        /// </summary>
        public string Text
        {
            get => TextBlockButton.Text;
            set => TextBlockButton.Text = value;
        }

        /// <summary>
        /// Скругление границ кнопки (по умолчанию 10, 10, 10, 10)
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => BorderButton.CornerRadius;
            set => BorderButton.CornerRadius = value;
        }

        /// <summary>
        /// Шрифт текста в кнопке
        /// </summary>
        public FontFamily TextFontFamily
        {
            get => TextBlockButton.FontFamily;
            set => TextBlockButton.FontFamily = value;
        }

        /// <summary>
        /// Размер текста в кнопке
        /// </summary>
        public double TextFontSize
        {
            get => TextBlockButton.FontSize;
            set => TextBlockButton.FontSize = value;
        }

        /// <summary>
        /// Анимация кнопки
        /// </summary>
        private readonly ColorAnimation animation;

        public IELButtonText()
        {
            InitializeComponent();
            AnimationMillisecond = 80;
            animation = new ColorAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(AnimationMillisecond)
            };
            TextFontFamily = new FontFamily("Arial");
            TextFontSize = 12;
            Text = "Text";
            CornerRadius = new CornerRadius(10);

            DefaultBorderBrush = Colors.Black;
            SelectBorderBrush = Colors.DarkGray;
            ClickedBorderBrush = Colors.Gray;
            NotEnabledBorderBrush = Colors.Brown;

            DefaultBackground = Colors.White;
            SelectBackground = Colors.Gray;
            ClickedBackground = Colors.WhiteSmoke;
            NotEnabledBackground = Colors.IndianRed;

            DefaultForeground = Colors.Black;
            SelectForeground = Colors.DarkGray;
            ClickedForeground = Colors.Gray;
            NotEnabledForeground = Colors.DarkRed;

            MouseEnter += (sender, e) => MouseEnterAnimation();

            MouseLeave += (sender, e) => MouseLeaveAnimation();

            MouseDown += (sender, e) =>
            {
                BorderButton.BorderBrush = new SolidColorBrush(ClickedBorderBrush);
                BorderButton.Background = new SolidColorBrush(ClickedBackground);
                TextBlockButton.Foreground = new SolidColorBrush(ClickedForeground);
            };

            MouseUp += (sender, e) => MouseEnterAnimation();

            IsEnabledChanged += (sender, e) =>
            {
                BorderButton.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
                BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, null);
                TextBlockButton.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, null);
                BorderButton.BorderBrush = new SolidColorBrush((bool)e.NewValue ? DefaultBorderBrush : NotEnabledBorderBrush);
                BorderButton.Background = new SolidColorBrush((bool)e.NewValue ? DefaultBackground : NotEnabledBackground);
                TextBlockButton.Foreground = new SolidColorBrush((bool)e.NewValue ? DefaultForeground : NotEnabledForeground);
            };
        }

        /// <summary>
        /// Анимация выделения кнопки мышью
        /// </summary>
        private void MouseEnterAnimation()
        {
            animation.To = SelectBorderBrush;
            BorderButton.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            animation.To = SelectBackground;
            BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            animation.To = SelectForeground;
            TextBlockButton.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        /// <summary>
        /// Анимация отключения выделения мышью
        /// </summary>
        private void MouseLeaveAnimation()
        {
            animation.To = DefaultBorderBrush;
            BorderButton.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            animation.To = DefaultBackground;
            BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            animation.To = DefaultForeground;
            TextBlockButton.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        /// <summary>
        /// Анимация мерцания
        /// </summary>
        public void BlinkAnimation()
        {
            animation.From = ClickedBorderBrush;
            animation.To = DefaultBorderBrush;
            BorderButton.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            animation.From = ClickedBackground;
            animation.To = DefaultBackground;
            BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            animation.From = ClickedForeground;
            animation.To = DefaultForeground;
            TextBlockButton.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }
    }
}
