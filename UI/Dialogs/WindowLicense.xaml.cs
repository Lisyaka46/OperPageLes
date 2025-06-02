using OperPage_les.CORE;
using OperPage_les.Windows.Pages.License;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Color = System.Windows.Media.Color;
using System.Windows.Input;

namespace OperPage_les.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
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
            new("Lisyaka", "Не знаю...",
                "- За всю разработку.")
            {
                UriImage = new Uri("https://sun9-46.userapi.com/impg/euj8JteQPLq-XpWDbR03hU2Dlz3IhzwLs4W9DA/bYNM9VcaP-w.jpg?size=800x800&quality=95&sign=b761945cee478f88087602b209cff6f9&type=album"),
                //PathImage = new Uri($"{App.PathImageApplication}/IconMainGray.png", UriKind.Relative)
            },
            new("Minsi", "Спасибо что живая.",
                "- За помощь в разработке." +
                "\n- За проектирование программы." +
                "\n- За оценку качества программы.")
            {
                ColorNickName = Color.FromRgb(86, 255, 120),
                ColorPhrase = Color.FromRgb(195, 189, 222),
                MapImage = App.LoadImage(Properties.Resources.MINSI)
            },
            new("Vector", "Разработчик это художник, а дизайнер это кисть.",
                "- За работу в дизайне." +
                "\n- За проектирование стиля." +
                "\n- За планировку вида.")
            {
                ColorNickName = Color.FromRgb(62, 180, 137),
                ColorPhrase = Color.FromRgb(80, 200, 120),
                MapImage = App.LoadImage(Properties.Resources.VECTOR)
            },
        ];

        /// <summary>
        /// Индекс отображения благодарственного сообщения
        /// </summary>
        private int Value = -1;

        public LicenseWindow()
        {
            InitializeComponent();
            ImageLogo.Margin = new(20);
            UpdateInfoThanks = new(10000d, (sender, e) => Dispatcher.BeginInvoke(UpdateThanks));
            Opacity = 0d;
            Closed += (sender, e) => GC.Collect(2, GCCollectionMode.Forced);
            Loaded += (sender, e) =>
            {
                DoubleAnimation anim = new()
                {
                    From = 0d,
                    To = 360d,
                    Duration = TimeSpan.FromSeconds(4d),
                    RepeatBehavior = RepeatBehavior.Forever,
                };
                RotateTransformTextAutor.BeginAnimation(RotateTransform.AngleProperty, anim);
                anim.Duration = TimeSpan.FromSeconds(20d);
                RotateTransformImageIconApplication.BeginAnimation(RotateTransform.AngleProperty, anim);
            };
            KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        Close();
                        break;
                }
            };
        }

        public new void ShowDialog()
        {
            DoubleAnimation animDouble = App.GetDoubleAnimate(TimeSpan.FromMilliseconds(1200d));
            animDouble.BeginTime = TimeSpan.FromMilliseconds(70d);
            animDouble.To = 1d;
            BeginAnimation(OpacityProperty, animDouble);

            ThicknessAnimation animThickness = App.GetThicknessAnimate(TimeSpan.FromMilliseconds(1200d));
            animThickness.BeginTime = animDouble.BeginTime + TimeSpan.FromMilliseconds(20d);
            animThickness.Duration = TimeSpan.FromMilliseconds(1600d);
            animThickness.To = new(0);
            animThickness.EasingFunction = new BackEase()
            {
                EasingMode = EasingMode.EaseOut,
                Amplitude = 0.78d,
            };
            ImageLogo.BeginAnimation(MarginProperty, animThickness);
            AnimationThanks();
            base.ShowDialog();
        }

        /// <summary>
        /// Начать анимацию благодарностей
        /// </summary>
        private void AnimationThanks()
        {
            FrameThanks.Navigate(PageThanks);
            UpdateThanks();
            UpdateInfoThanks.Start();
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
