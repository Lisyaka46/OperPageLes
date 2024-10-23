using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AAC20.Windows.Pages.License
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class PageLicense : Page
    {
        /// <summary>
        /// Объект анимации для управления прозрачностью объекта
        /// </summary>
        private readonly DoubleAnimation DoubleAnimate = new(0, TimeSpan.FromMilliseconds(800d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private readonly ThicknessAnimation ThicknessAnimatePos = new(new Thickness(0), TimeSpan.FromMilliseconds(700d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        public PageLicense()
        {
            InitializeComponent();
            Opacity = 0d;
            TextBlockAAC.Margin = new(0, 20, 0, 0);
            TextBlockLicenseText.Margin = new(0, 67, 0, 0);
            DoubleAnimate.BeginTime = TimeSpan.FromMilliseconds(700d);
            ThicknessAnimatePos.BeginTime = TimeSpan.FromMilliseconds(700d);
            TextBlockLicenseText.Foreground.RelativeTransform = new RotateTransform(0d, 0.5d, 0.5d);
            Start();
        }

        /// <summary>
        /// Начать анимацию страницы
        /// </summary>
        private void Start()
        {
            DoubleAnimate.To = 1d;
            BeginAnimation(OpacityProperty, DoubleAnimate);
            ThicknessAnimatePos.To = new(0, 10, 0, 0);
            TextBlockAAC.BeginAnimation(MarginProperty, ThicknessAnimatePos);
            ThicknessAnimatePos.BeginTime += TimeSpan.FromMilliseconds(72d);
            ThicknessAnimatePos.To = new(0, 57, 0, 0);
            TextBlockLicenseText.BeginAnimation(MarginProperty, ThicknessAnimatePos);

            DoubleAnimation anim = new()
            {
                From = 0d,
                To = 360d,
                Duration = TimeSpan.FromMilliseconds(4000d),
                RepeatBehavior = RepeatBehavior.Forever
            };
            TextBlockLicenseText.Foreground.RelativeTransform.BeginAnimation(RotateTransform.AngleProperty, anim);
        }
    }
}
