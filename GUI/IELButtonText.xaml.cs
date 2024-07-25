using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AAC20.GUI
{
    /// <summary>
    /// Логика взаимодействия для IELButtonText.xaml
    /// </summary>
    public partial class IELButtonText : UserControl
    {
        /// <summary>
        /// Перечисление состояний отображения кнопки
        /// </summary>
        public enum StateButton
        {
            /// <summary>
            /// Обычное отображение кнопки
            /// </summary>
            Default = 0,

            /// <summary>
            /// Отображение кнопки с левосторонней стрелкой
            /// </summary>
            LeftArrow = 1,

            /// <summary>
            /// Отображение кнопки с правосторонней стрелкой
            /// </summary>
            RightArrow = 2,
        }

        /// <summary>
        /// Перечисление стилей цвета нажатия на кнопку
        /// </summary>
        private enum ActivateClickColor
        {
            /// <summary>
            /// Обычный цвет нажатия на кнопку
            /// </summary>
            Clicked = 0,

            /// <summary>
            /// Отключённый цвет нажатия на кнопку
            /// </summary>
            IsNotEnabled = 1
        }

        private StateButton _StateVisualizationButton = StateButton.LeftArrow;
        /// <summary>
        /// Состояние отображения кнопки
        /// </summary>
        public StateButton StateVisualizationButton
        {
            get => _StateVisualizationButton;
            set
            {
                if (_StateVisualizationButton == value) return;
                ColumnLeftArrow.Width = new(value == StateButton.LeftArrow ? 25 : 0);
                ColumnRightArrow.Width = new(value == StateButton.RightArrow ? 25 : 0);
                BorderLeftArrow.Opacity = value == StateButton.LeftArrow ? 1d : 0d;
                BorderRightArrow.Opacity = value == StateButton.RightArrow ? 1d : 0d;
                _StateVisualizationButton = value;
            }
        }

        private Color _DefaultBorderBrush;
        /// <summary>
        /// Цвет границы кнопки
        /// </summary>
        public Color DefaultBorderBrush
        {
            get => _DefaultBorderBrush;
            set
            {
                SolidColorBrush color = new(value);
                BorderButton.BorderBrush = color;
                BorderLeftArrow.BorderBrush = color;
                BorderRightArrow.BorderBrush = color;
                BorderCharKeyboard.BorderBrush = color;
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
                SolidColorBrush color = new(value);
                BorderButton.Background = color;
                BorderCharKeyboard.Background = color;
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
                SolidColorBrush color = new(value);
                TextBlockButton.Foreground = color;
                TextBlockLeftArrow.Foreground = color;
                TextBlockRightArrow.Foreground = color;
                TextBlockCharKey.Foreground = color;
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

        private int _AnimationMillisecond = 80;
        /// <summary>
        /// Количество миллисекунд для анимации (по умолчанию 80)
        /// </summary>
        public int AnimationMillisecond
        {
            get => _AnimationMillisecond;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Значение должно быть больше нуля!");
                else
                {
                    TimeSpan time = TimeSpan.FromMilliseconds(value);
                    ButtonAnimationColor.Duration = time;
                    ButtonAnimationThickness.Duration = time;
                    ButtonAnimationOpacity.Duration = time;
                    _AnimationMillisecond = value;
                }
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

        public bool _CharKeyKeyboardActivate = false;
        /// <summary>
        /// Активность видимости символа действия активации кнопки
        /// </summary>
        public bool CharKeyKeyboardActivate
        {
            get => _CharKeyKeyboardActivate;
            set
            {
                ButtonAnimationOpacity.To = value ? 1d : 0d;
                ButtonAnimationThickness.To = new(!value ? -24 : 0, 0, 0, 0);
                BorderButton.BeginAnimation(MarginProperty, ButtonAnimationThickness);
                BorderCharKeyboard.BeginAnimation(OpacityProperty, ButtonAnimationOpacity);
                _CharKeyKeyboardActivate = value;
            }
        }

        /// <summary>
        /// Анимация цвета кнопки
        /// </summary>
        private readonly ColorAnimation ButtonAnimationColor;

        /// <summary>
        /// Анимация позиции стрелок кнопки
        /// </summary>
        private readonly ThicknessAnimation ButtonAnimationThickness;

        /// <summary>
        /// Анимация прозрачности для символа клавиатуры
        /// </summary>
        private readonly DoubleAnimation ButtonAnimationOpacity;

        public IELButtonText()
        {
            InitializeComponent();
            StateVisualizationButton = StateButton.Default;
            ButtonAnimationOpacity = new()
            {
                Duration = TimeSpan.FromMilliseconds(AnimationMillisecond)
            };
            ButtonAnimationThickness = new()
            {
                Duration = TimeSpan.FromMilliseconds(AnimationMillisecond)
            };
            ButtonAnimationColor = new()
            {
                Duration = TimeSpan.FromMilliseconds(AnimationMillisecond)
            };
            BorderButton.Margin = new(-24, 0, 0, 0);
            BorderCharKeyboard.Opacity = 0d;
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

            MouseLeftButtonDown += (sender, e) => ClickDownAnimation(ActivateClickColor.Clicked);

            MouseUp += (sender, e) => MouseEnterAnimation();

            IsEnabledChanged += (sender, e) =>
            {
                Color
                Foreground = (bool)e.NewValue ? DefaultForeground : NotEnabledForeground,
                Background = (bool)e.NewValue ? DefaultBackground : NotEnabledBackground,
                BorderBrush = (bool)e.NewValue ? DefaultBorderBrush : NotEnabledBorderBrush;
                if (StateVisualizationButton != StateButton.Default)
                {
                    if (StateVisualizationButton == StateButton.LeftArrow)
                    {
                        TextBlockLeftArrow.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, null);
                        TextBlockLeftArrow.Foreground = new SolidColorBrush(Foreground);
                        BorderLeftArrow.BeginAnimation(MarginProperty, null);
                        BorderLeftArrow.BorderBrush = new SolidColorBrush(BorderBrush);
                    }
                    else
                    {
                        TextBlockRightArrow.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, null);
                        TextBlockRightArrow.Foreground = new SolidColorBrush(Foreground);
                        BorderRightArrow.BeginAnimation(MarginProperty, null);
                        BorderRightArrow.BorderBrush = new SolidColorBrush(BorderBrush);
                    }
                }
                if (CharKeyKeyboardActivate)
                {
                    BorderCharKeyboard.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
                    BorderCharKeyboard.Background.BeginAnimation(SolidColorBrush.ColorProperty, null);
                    TextBlockCharKey.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, null);
                    BorderCharKeyboard.BorderBrush = new SolidColorBrush(BorderBrush);
                    BorderCharKeyboard.Background = new SolidColorBrush(Background);
                    TextBlockCharKey.Foreground = new SolidColorBrush(Foreground);
                }
                BorderButton.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
                BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, null);
                TextBlockButton.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, null);
                BorderButton.BorderBrush = new SolidColorBrush(BorderBrush);
                BorderButton.Background = new SolidColorBrush(Background);
                TextBlockButton.Foreground = new SolidColorBrush(Foreground);
            };
        }

        /// <summary>
        /// Анимировать нажатие на кнопку (Down)
        /// </summary>
        /// <param name="StyleClickColor">Стиль нажатия на кнопку</param>
        private void ClickDownAnimation(ActivateClickColor StyleClickColor)
        {
            Color
            Foreground = StyleClickColor == ActivateClickColor.Clicked ? ClickedForeground : NotEnabledForeground,
            Background = StyleClickColor == ActivateClickColor.Clicked ? ClickedBackground : NotEnabledBackground,
            BorderBrush = StyleClickColor == ActivateClickColor.Clicked ? ClickedBorderBrush : NotEnabledBorderBrush;
            if (StateVisualizationButton != StateButton.Default)
            {
                if (StateVisualizationButton == StateButton.LeftArrow)
                {
                    TextBlockLeftArrow.Foreground = new SolidColorBrush(Foreground);
                    BorderLeftArrow.BeginAnimation(MarginProperty, null);
                    BorderLeftArrow.Margin = new(0, 1, -2, 0);
                }
                else
                {
                    TextBlockRightArrow.Foreground = new SolidColorBrush(Foreground);
                    BorderRightArrow.BeginAnimation(MarginProperty, null);
                    BorderRightArrow.Margin = new(-2, 1, 0, 0);
                }
            }
            if (CharKeyKeyboardActivate)
            {
                BorderCharKeyboard.BorderBrush = new SolidColorBrush(BorderBrush);
                BorderCharKeyboard.Background = new SolidColorBrush(Background);
            }
            BorderButton.BorderBrush = new SolidColorBrush(BorderBrush);
            BorderButton.Background = new SolidColorBrush(Background);
            TextBlockButton.Foreground = new SolidColorBrush(Foreground);
        }

        /// <summary>
        /// Анимация выделения кнопки мышью
        /// </summary>
        private void MouseEnterAnimation()
        {
            if (StateVisualizationButton != StateButton.Default)
            {
                ButtonAnimationThickness.To = new(
                    StateVisualizationButton == StateButton.RightArrow ? 1 : 0,
                    1,
                    StateVisualizationButton == StateButton.LeftArrow ? 1 : 0,
                    0);
                if (StateVisualizationButton == StateButton.LeftArrow)
                    BorderLeftArrow.BeginAnimation(MarginProperty, ButtonAnimationThickness);
                else BorderRightArrow.BeginAnimation(MarginProperty, ButtonAnimationThickness);
            }

            ButtonAnimationColor.To = SelectBorderBrush;
            BorderButton.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            if (StateVisualizationButton != StateButton.Default)
            {
                if (StateVisualizationButton == StateButton.LeftArrow)
                    BorderLeftArrow.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
                else BorderRightArrow.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            }
            if (CharKeyKeyboardActivate)
            {
                BorderCharKeyboard.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            }

            ButtonAnimationColor.To = SelectBackground;
            BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            if (CharKeyKeyboardActivate)
            {
                BorderCharKeyboard.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            }

            ButtonAnimationColor.To = SelectForeground;
            TextBlockButton.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            if (StateVisualizationButton != StateButton.Default)
            {
                if (StateVisualizationButton == StateButton.LeftArrow)
                    TextBlockLeftArrow.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
                else TextBlockRightArrow.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            }
            if (CharKeyKeyboardActivate)
            {
                TextBlockCharKey.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            }
        }

        /// <summary>
        /// Анимация отключения выделения мышью
        /// </summary>
        private void MouseLeaveAnimation()
        {
            if (StateVisualizationButton != StateButton.Default)
            {
                ButtonAnimationThickness.To = new(
                    StateVisualizationButton == StateButton.RightArrow ? -5 : 0,
                    1,
                    StateVisualizationButton == StateButton.LeftArrow ? -5 : 0,
                    0);
                if (StateVisualizationButton == StateButton.LeftArrow)
                    BorderLeftArrow.BeginAnimation(MarginProperty, ButtonAnimationThickness);
                else BorderRightArrow.BeginAnimation(MarginProperty, ButtonAnimationThickness);
            }

            ButtonAnimationColor.To = DefaultBorderBrush;
            BorderButton.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            if (StateVisualizationButton != StateButton.Default)
            {
                if (StateVisualizationButton == StateButton.LeftArrow)
                    BorderLeftArrow.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
                else BorderRightArrow.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            }
            if (CharKeyKeyboardActivate)
            {
                BorderCharKeyboard.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            }

            ButtonAnimationColor.To = DefaultBackground;
            BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            if (CharKeyKeyboardActivate)
            {
                BorderCharKeyboard.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            }

            ButtonAnimationColor.To = DefaultForeground;
            TextBlockButton.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            if (CharKeyKeyboardActivate)
            {
                TextBlockCharKey.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            }
        }

        /// <summary>
        /// Анимация мерцания
        /// </summary>
        public void BlinkAnimation()
        {
            ButtonAnimationColor.From = ClickedBorderBrush;
            ButtonAnimationColor.To = DefaultBorderBrush;
            BorderButton.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);

            ButtonAnimationColor.From = ClickedBackground;
            ButtonAnimationColor.To = DefaultBackground;
            BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);

            ButtonAnimationColor.From = ClickedForeground;
            ButtonAnimationColor.To = DefaultForeground;
            TextBlockButton.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
        }
    }
}
