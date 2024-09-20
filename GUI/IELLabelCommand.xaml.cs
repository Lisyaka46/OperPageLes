using AAC20.Interfaces;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace AAC20.GUI
{
    /// <summary>
    /// Логика взаимодействия для IELLabelCommand.xaml
    /// </summary>
    public partial class IELLabelCommand : UserControl
    {

        private GradientBrush _DefaultBorderBrush = new RadialGradientBrush(Colors.White, Colors.Black);
        /// <summary>
        /// Цвет границы кнопки
        /// </summary>
        public GradientBrush DefaultBorderBrush
        {
            get => _DefaultBorderBrush;
            set
            {
                BorderMain.BorderBrush = value;
                _DefaultBorderBrush = value;
            }
        }

        private Color _DefaultBackground = Colors.Black;
        /// <summary>
        /// Цвет фона кнопки
        /// </summary>
        public Color DefaultBackground
        {
            get => _DefaultBackground;
            set
            {
                SolidColorBrush color = new(value);
                BorderMain.Background = color;
                _DefaultBackground = value;
            }
        }

        private Color _DefaultForeground = Colors.Black;
        /// <summary>
        /// Цвет текста в кнопке
        /// </summary>
        public Color DefaultForeground
        {
            get => _DefaultForeground;
            set
            {
                SolidColorBrush color = new(value);
                TextBlockIndex.Foreground = color;
                TextBlockName.Foreground = color;
                _DefaultForeground = value;
            }
        }

        /// <summary>
        /// Выделенный цвет границы кнопки
        /// </summary>
        public GradientBrush SelectBorderBrush { get; set; }

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
        public GradientBrush ClickedBorderBrush { get; set; }

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
        public GradientBrush NotEnabledBorderBrush { get; set; }

        /// <summary>
        /// Выключенный цвет фона кнопки
        /// </summary>
        public Color NotEnabledBackground { get; set; }

        /// <summary>
        /// Выключенный цвет текста в кнопке
        /// </summary>
        public Color NotEnabledForeground { get; set; }

        /// <summary>
        /// Объект события активации кнопки левым щелчком мыши
        /// </summary>
        public IIELObject.Activate? OnActivateMouseLeft { get; internal set; }

        /// <summary>
        /// Объект события активации кнопки правым щелчком мыши
        /// </summary>
        public IIELObject.Activate? OnActivateMouseRight { get; internal set; }

        #region animateObjects
        /// <summary>
        /// Анимация цвета кнопки
        /// </summary>
        private readonly ColorAnimation ButtonAnimationColor = new()
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Анимация позиции
        /// </summary>
        private readonly ThicknessAnimation ButtonAnimationThickness = new()
        {
            EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Анимация прозрачности
        /// </summary>
        private readonly DoubleAnimation ButtonAnimationDouble = new()
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };
        #endregion

        #region AnimationMillisecond
        private int _AnimationMillisecond;
        /// <summary>
        /// Длительность анимации в миллисекундах
        /// </summary>
        public int AnimationMillisecond
        {
            get => _AnimationMillisecond;
            set
            {
                TimeSpan time = TimeSpan.FromMilliseconds(value);
                ButtonAnimationColor.Duration = time;
                ButtonAnimationDouble.Duration = time;
                ButtonAnimationThickness.Duration = time;
                _AnimationMillisecond = value;

            }
        }
        #endregion

        public IELLabelCommand()
        {
            InitializeComponent();
            AnimationMillisecond = 200;

            BorderMain.Background = new SolidColorBrush(Colors.Black);
            BorderMain.BorderBrush = new RadialGradientBrush(Colors.White, Colors.Black);

            TextBlockName.Foreground = new SolidColorBrush(Colors.Black);
            TextBlockName.Foreground = new SolidColorBrush(Colors.Black);

            DefaultBackground = Color.FromRgb(128, 179, 189);
            DefaultBorderBrush = new RadialGradientBrush(Color.FromRgb(1, 2, 3), Color.FromRgb(69, 98, 127))
            {
                RadiusX = 0.8d,
                RadiusY = 0.8d,
            };
            DefaultForeground = Colors.Black;

            SelectBackground = Color.FromRgb(111, 199, 173);
            SelectBorderBrush = new RadialGradientBrush(Colors.White, Color.FromRgb(69, 98, 127))
            {
                RadiusX = 0.8d,
                RadiusY = 0.8d,
            };
            SelectForeground = Color.FromRgb(0, 80, 60);

            ClickedBackground = Color.FromRgb(69, 154, 101);
            ClickedBorderBrush = new RadialGradientBrush(Color.FromRgb(1, 2, 3), Color.FromRgb(69, 127, 83))
            {
                RadiusX = 0.8d,
                RadiusY = 0.8d,
            };
            ClickedForeground = Color.FromRgb(40, 60, 41);

            NotEnabledBackground = Color.FromRgb(181, 102, 102);
            NotEnabledBorderBrush = new RadialGradientBrush(Color.FromRgb(1, 2, 3), Color.FromRgb(255, 90, 90))
            {
                RadiusX = 0.8d,
                RadiusY = 0.8d,
            };
            NotEnabledForeground = Colors.Black;

            IsEnabledChanged += (sender, e) =>
            {
                Color
                Foreground = (bool)e.NewValue ? DefaultForeground : NotEnabledForeground,
                Background = (bool)e.NewValue ? DefaultBackground : NotEnabledBackground;
                GradientBrush BorderBrush = (bool)e.NewValue ? DefaultBorderBrush : NotEnabledBorderBrush;

                ButtonAnimationColor.To = Background;
                BorderMain.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);

                ButtonAnimationColor.To = BorderBrush.GradientStops[0].Color;
                ((GradientBrush)BorderMain.BorderBrush).GradientStops[0].BeginAnimation(GradientStop.ColorProperty, ButtonAnimationColor);
                ButtonAnimationColor.To = BorderBrush.GradientStops[1].Color;
                ((GradientBrush)BorderMain.BorderBrush).GradientStops[1].BeginAnimation(GradientStop.ColorProperty, ButtonAnimationColor);

                ButtonAnimationColor.To = Foreground;
                TextBlockIndex.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
                TextBlockName.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            };

            MouseEnter += (sender, e) =>
            {
                if (IsEnabled) MouseEnterAnimation();
            };

            MouseLeave += (sender, e) =>
            {
                if (IsEnabled) MouseLeaveAnimation();
            };

            MouseDown += (sender, e) =>
            {
                if (IsEnabled)
                {
                    if (e.LeftButton == MouseButtonState.Pressed && OnActivateMouseLeft != null) ClickDownAnimation();
                    else if (e.RightButton == MouseButtonState.Pressed && OnActivateMouseRight != null) ClickDownAnimation();
                }
            };

            MouseLeftButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseLeft != null)
                {
                    MouseEnterAnimation();
                    OnActivateMouseLeft?.Invoke(false);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseRight != null)
                {
                    MouseEnterAnimation();
                    OnActivateMouseRight?.Invoke(false);
                }
            };
        }

        /// <summary>
        /// Анимация выделения объекта мышью
        /// </summary>
        private void MouseEnterAnimation()
        {
            ButtonAnimationColor.To = SelectBackground;
            BorderMain.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);

            ButtonAnimationColor.To = SelectBorderBrush.GradientStops[0].Color;
            ((GradientBrush)BorderMain.BorderBrush).GradientStops[0].BeginAnimation(GradientStop.ColorProperty, ButtonAnimationColor);
            ButtonAnimationColor.To = SelectBorderBrush.GradientStops[1].Color;
            ((GradientBrush)BorderMain.BorderBrush).GradientStops[1].BeginAnimation(GradientStop.ColorProperty, ButtonAnimationColor);

            ButtonAnimationColor.To = SelectForeground;
            TextBlockIndex.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            TextBlockName.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);

            ButtonAnimationThickness.To = new(8);
            ImageElement.BeginAnimation(MarginProperty, ButtonAnimationThickness);
        }

        /// <summary>
        /// Анимация отключения выделения мышью
        /// </summary>
        private void MouseLeaveAnimation()
        {
            ButtonAnimationColor.To = DefaultBackground;
            BorderMain.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);

            ButtonAnimationColor.To = DefaultBorderBrush.GradientStops[0].Color;
            ((GradientBrush)BorderMain.BorderBrush).GradientStops[0].BeginAnimation(GradientStop.ColorProperty, ButtonAnimationColor);
            ButtonAnimationColor.To = DefaultBorderBrush.GradientStops[1].Color;
            ((GradientBrush)BorderMain.BorderBrush).GradientStops[1].BeginAnimation(GradientStop.ColorProperty, ButtonAnimationColor);

            ButtonAnimationColor.To = DefaultForeground;
            TextBlockIndex.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            TextBlockName.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);

            ButtonAnimationThickness.To = new(10);
            ImageElement.BeginAnimation(MarginProperty, ButtonAnimationThickness);
        }

        /// <summary>
        /// Анимировать нажатие на кнопку (Down)
        /// </summary>
        /// <param name="StyleClickColor">Стиль нажатия на кнопку</param>
        private void ClickDownAnimation()
        {
            ButtonAnimationColor.To = ClickedBackground;
            BorderMain.Background.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);

            ButtonAnimationColor.To = ClickedBorderBrush.GradientStops[0].Color;
            ((GradientBrush)BorderMain.BorderBrush).GradientStops[0].BeginAnimation(GradientStop.ColorProperty, ButtonAnimationColor);
            ButtonAnimationColor.To = ClickedBorderBrush.GradientStops[1].Color;
            ((GradientBrush)BorderMain.BorderBrush).GradientStops[1].BeginAnimation(GradientStop.ColorProperty, ButtonAnimationColor);

            ButtonAnimationColor.To = ClickedForeground;
            TextBlockIndex.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);
            TextBlockName.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ButtonAnimationColor);

            ButtonAnimationThickness.To = new(9);
            ImageElement.BeginAnimation(MarginProperty, ButtonAnimationThickness);
        }
    }
}
