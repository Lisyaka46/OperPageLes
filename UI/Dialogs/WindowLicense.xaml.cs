using OperPage_les.CORE;
using OperPage_les.Windows.Pages.License;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Color = System.Windows.Media.Color;

namespace OperPage_les.UI.Dialogs
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
        /// Массив помошников 
        /// </summary>
        private readonly AssistentThanks[] Assistents =
        [
            new("Lisyaka", "\"Не знаю...\"",
                "- За всю разработку.",
                new Uri("https://sun9-46.userapi.com/impg/euj8JteQPLq-XpWDbR03hU2Dlz3IhzwLs4W9DA/bYNM9VcaP-w.jpg?size=800x800&quality=95&sign=b761945cee478f88087602b209cff6f9&type=album"),
                new Uri($"{App.PathImageApplication}/IconMainGray.png", UriKind.Relative)
                )
            {
                ColorNickName = Color.FromRgb(245, 225, 101),
                ColorPhrase =  Color.FromRgb(219, 177, 205)
            },
            new("Minsi", "\"Спасибо что живая.\"",
                "- За помощь в разработке.\n- За проектирование программы.\n- За оценку качества программы."
                )
            {
                ColorNickName = Color.FromRgb(86, 255, 120),
                ColorPhrase =  Color.FromRgb(195, 189, 222)
            },
        ];

        /// <summary>
        /// Индекс отображения благодарственного сообщения
        /// </summary>
        private int Value = -1;

        public LicenseWindow()
        {
            InitializeComponent();
            UpdateInfoThanks = new(10000d, (sender, e) => Dispatcher.BeginInvoke(UpdateThanks));
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
            FrameLicense.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            Closed += (sender, e) => GC.Collect(2, GCCollectionMode.Forced);
        }

        public new void ShowDialog()
        {
            DoubleAnimateAppOpacity.BeginTime = TimeSpan.FromMilliseconds(70d);
            DoubleAnimateAppOpacity.To = 1d;
            BeginAnimation(OpacityProperty, DoubleAnimateAppOpacity);

            ThicknessAnimatePos.BeginTime = DoubleAnimateAppOpacity.BeginTime + TimeSpan.FromMilliseconds(20d);
            ThicknessAnimatePos.Duration = TimeSpan.FromMilliseconds(1600d);
            ThicknessAnimatePos.From = new(0, 24, 0, 24);
            ThicknessAnimatePos.To = new(0, 20, 0, 20);
            ImageLogo.BeginAnimation(MarginProperty, ThicknessAnimatePos);
            ThicknessAnimatePos.Duration = TimeSpan.FromMilliseconds(800d);
            ThicknessAnimatePos.From = null;
            ThicknessAnimatePos.To = new(0, 96, 0, 0);
            PageLicense license = new();
            FrameLicense.Navigate(license);
            AnimationThanks();
            DoubleAnimateAppOpacity.BeginTime = TimeSpan.Zero;
            base.ShowDialog();
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
            if (Value == Assistents.Length - 1) Value = 0;
            else Value++;
            PageThanks.NextUser(Assistents[Value]);
        }
    }
}
