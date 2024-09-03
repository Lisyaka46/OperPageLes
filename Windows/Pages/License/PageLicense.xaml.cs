using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        private readonly DoubleAnimation DoubleAnimateOpacity = new(0, TimeSpan.FromMilliseconds(800d))
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
            DoubleAnimateOpacity.BeginTime = TimeSpan.FromMilliseconds(700d);
            ThicknessAnimatePos.BeginTime = TimeSpan.FromMilliseconds(700d);
            Start();
        }

        /// <summary>
        /// Начать анимацию страницы
        /// </summary>
        private void Start()
        {
            DoubleAnimateOpacity.To = 1d;
            BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
            ThicknessAnimatePos.To = new(0, 10, 0, 0);
            TextBlockAAC.BeginAnimation(MarginProperty, ThicknessAnimatePos);
            ThicknessAnimatePos.BeginTime += TimeSpan.FromMilliseconds(70d);
            ThicknessAnimatePos.To = new(0, 57, 0, 0);
            TextBlockLicenseText.BeginAnimation(MarginProperty, ThicknessAnimatePos);
        }
    }
}
