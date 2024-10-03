using AAC20.Classes;
using AAC20.Windows.Pages.License;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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
        /// Массив помошников 
        /// </summary>
        private readonly AssistentThanks[] Assistents =
        [
            new("Lisyaka", "\"Не знаю...\"",
                "- За всю разработку.",
                new Uri("https://sun9-46.userapi.com/impg/euj8JteQPLq-XpWDbR03hU2Dlz3IhzwLs4W9DA/bYNM9VcaP-w.jpg?size=800x800&quality=95&sign=b761945cee478f88087602b209cff6f9&type=album"),
                new Uri(@"C:\Users\killm\Рабочий стол\Main\Programm\С#\AAC20\Windows\WindowsImages\Logo02.png")
                )
            {
                ColorNickName = Color.FromRgb(245, 225, 101),
                ColorPhrase =  Color.FromRgb(219, 177, 205)
            },
            new("Minsi", "\"Спасибо что живая.\"",
                "- За помощь в разработке.\n- За проектирование программы."
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
            //MediaElementLicense.Source = new(@"C:\Users\killm\Рабочий стол\Main\Programm\С#\AAC20\Windows\WindowsImages\LicensePreview.gif");
            FrameLicense.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            ME.Source = new Uri(@"C:\Users\killm\Рабочий стол\Main\Programm\С#\AAC20\Windows\WindowsImages\LicensePreview.wmv");
            ME.MediaEnded += (sender, e) =>
            {
                ME.Position = TimeSpan.FromMilliseconds(1d);
                ME.Play();
            };
        }

        public new void ShowDialog()
        {
            DoubleAnimateAppOpacity.BeginTime = TimeSpan.FromMilliseconds(70d);
            DoubleAnimateAppOpacity.To = 1d;
            BeginAnimation(OpacityProperty, DoubleAnimateAppOpacity);

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
            ThicknessAnimatePos.BeginTime = TimeSpan.FromMilliseconds(2200d);
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
            if (Value == Assistents.Length - 1) Value = 0;
            else Value++;
            PageThanks.NextUser(Assistents[Value]);
        }
    }
}
