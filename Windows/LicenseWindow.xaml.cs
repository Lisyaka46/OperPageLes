using AAC20.Classes;
using AAC20.Windows.Pages.License;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace AAC20.Windows
{
    /// <summary>
    /// Логика взаимодействия для LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        /// <summary>
        /// Объект анимации для управления прозрачностью приложения
        /// </summary>
        private readonly DoubleAnimation DoubleAnimateAppOpacity;

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private readonly ThicknessAnimation ThicknessAnimatePos;

        /// <summary>
        /// Объект фонового обновления благодарностей
        /// </summary>
        private readonly UpdateBackgroundData UpdateInfoThanks;

        /// <summary>
        /// Объект страницы благодарностей
        /// </summary>
        private readonly PageUserThanks PageThanks = new();

        /// <summary>
        /// Индекс отображения благодарственного сообщения
        /// </summary>
        private int Value = -1;

        /// <summary>
        /// Массив ников
        /// </summary>
        private readonly string[] NickNamesThanks =
        [
            "Lisyaka",
            "Minsi",
        ];

        /// <summary>
        /// Массив фраз
        /// </summary>
        private readonly string[] PhrasesThanks =
        [
            "\"Я не знаю...\"",
            "\"Спасибо что живая.\"",
        ];

        /// <summary>
        /// Массив благодарственных сообщений
        /// </summary>
        private readonly string[] MessageThanks =
        [
            "- За всю разработку.",
            "- За помощь в разработке.\n- За проектирование программы.",
        ];

        /// <summary>
        /// Массив путей для иконок
        /// </summary>
        private readonly Uri?[] UriIconThanks =
        [
            new Uri("https://sun9-46.userapi.com/impg/euj8JteQPLq-XpWDbR03hU2Dlz3IhzwLs4W9DA/bYNM9VcaP-w.jpg?size=800x800&quality=95&sign=b761945cee478f88087602b209cff6f9&type=album"),
            null,
        ];

        /// <summary>
        /// Массив объектов цвета, для персонализвации пользователя в благодарностях (Ник, Фраза)
        /// </summary>
        private readonly (Color?, Color?)[] ForegroundColorThanks =
        [
            (Color.FromRgb(245, 225, 101), Color.FromRgb(219, 177, 205)),
            (Color.FromRgb(86, 255, 120), Color.FromRgb(195, 189, 222)),
        ];

        public LicenseWindow()
        {
            InitializeComponent();
            UpdateInfoThanks = new(6000d, (sender, e) => Dispatcher.BeginInvoke(UpdateThanks));
            DoubleAnimateAppOpacity = new(0, TimeSpan.FromMilliseconds(1050d))
            {
                DecelerationRatio = 0.2d,
                EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
            };
            ThicknessAnimatePos = new(new Thickness(0), TimeSpan.FromMilliseconds(800d))
            {
                DecelerationRatio = 0.6d,
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };
            Opacity = 0d;
            ImageLicense.Opacity = 0d;
            ImageLicense.RenderSize = new Size(70, 70);
            ImageLicense.Margin = new(0, 45 - 15, 0, 35 - 15);
            ImageLogo.Margin = new(12, 30 + 10, 0, 0);
            Image20.Margin = new(-12, 84 - 10, 0, 0);
            FrameLicense.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        public new void ShowDialog()
        {
            DoubleAnimateAppOpacity.BeginTime = TimeSpan.FromMilliseconds(70d);
            DoubleAnimateAppOpacity.To = 1d;
            BeginAnimation(OpacityProperty, DoubleAnimateAppOpacity);

            ThicknessAnimatePos.BeginTime = DoubleAnimateAppOpacity.BeginTime + TimeSpan.FromMilliseconds(20d);
            ThicknessAnimatePos.To = new(0, 30, 0, 0);
            ImageLogo.BeginAnimation(MarginProperty, ThicknessAnimatePos);
            ThicknessAnimatePos.To = new(0, 84, 0, 0);
            Image20.BeginAnimation(MarginProperty, ThicknessAnimatePos);
            PageLicense license = new();
            FrameLicense.Navigate(license);
            AnimationThanks();
            LicenseAnimation();
            DoubleAnimateAppOpacity.BeginTime = TimeSpan.Zero;
            base.ShowDialog();
        }

        /// <summary>
        /// Активировать анимацию страницы лицензии
        /// </summary>
        private void LicenseAnimation()
        {
            ThicknessAnimatePos.BeginTime = TimeSpan.FromMilliseconds(1200d);
            ThicknessAnimatePos.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            ThicknessAnimatePos.Duration = TimeSpan.FromMilliseconds(300d);
            DoubleAnimateAppOpacity.BeginTime = TimeSpan.FromMilliseconds(1300d);
            DoubleAnimateAppOpacity.Duration = TimeSpan.FromMilliseconds(600d);
            ThicknessAnimatePos.To = new(0, 45, 0, 35);
            DoubleAnimateAppOpacity.To = 1d;
            ImageLicense.BeginAnimation(MarginProperty, ThicknessAnimatePos);
            ImageLicense.BeginAnimation(OpacityProperty, DoubleAnimateAppOpacity);
        }

        /// <summary>
        /// Начать анимацию благодарностей
        /// </summary>
        private void AnimationThanks()
        {
            FrameThanks.Navigate(PageThanks);
            UpdateThanks();
            UpdateInfoThanks.TimerDataUpdate.Start();
        }

        /// <summary>
        /// Обновить панель благодарности
        /// </summary>
        private void UpdateThanks()
        {
            if (Value == NickNamesThanks.Length - 1) Value = 0;
            else Value++;
            PageThanks.NextUser(
                NickNamesThanks[Value],
                PhrasesThanks[Value],
                MessageThanks[Value],
                UriIconThanks[Value],
                ForegroundColorThanks[Value].Item1 ?? Colors.Black,
                ForegroundColorThanks[Value].Item2 ?? Colors.Black
                );
        }
    }
}
