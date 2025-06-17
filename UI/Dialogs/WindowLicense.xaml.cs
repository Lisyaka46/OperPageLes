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

        #region Other
        /// <summary>
        /// Количество нажатий на изображение иконки
        /// </summary>
        private int CountClickImageLogo = 0;

        private readonly DoubleAnimation LogoRotateAnimate = new()
        {
            To = 360d,
            Duration = TimeSpan.FromSeconds(20d),
            RepeatBehavior = RepeatBehavior.Forever,
        };
        #endregion

        public LicenseWindow()
        {
            InitializeComponent();
            ImageLogo.Margin = new(20);
            UpdateInfoThanks = new(10000d, (sender, e) => Dispatcher.BeginInvoke(UpdateThanks));
            Opacity = 0d;
            ImageLogo.MouseLeftButtonUp += (sender, e) =>
            {
                ExecuteEventClickImageLogo(++CountClickImageLogo);
            };
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
                RotateTransformImageIconApplication.BeginAnimation(RotateTransform.AngleProperty, LogoRotateAnimate);
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

        /// <summary>
        /// Совершить событие нажатия по изображению иконки
        /// </summary>
        /// <param name="CountClick">количество нажатий</param>
        private void ExecuteEventClickImageLogo(int CountClick)
        {
            switch (CountClick)
            {
                case 10:
                    LogoRotateAnimate.Duration = TimeSpan.FromSeconds(60d);
                    RotateTransformImageIconApplication.BeginAnimation(RotateTransform.AngleProperty, LogoRotateAnimate);
                    break;
                case 15:
                    RotateTransformImageIconApplication.BeginAnimation(RotateTransform.AngleProperty, null);
                    break;
                case 25:
                    System.Windows.Forms.MessageBox.Show("Всё, больше ничего не будет.");
                    System.Windows.Forms.MessageBox.Show("Я правду говорю");
                    break;
                case 50:
                case 60:
                case 65:
                case 75:
                case 87:
                    int RandomOffset = new Random(CountClick).Next(0, 45);
                    App.AnimateThicknessEffect(ImageLogo, MarginProperty, new(0, RandomOffset, 0, RandomOffset), TimeSpan.FromMilliseconds(800d));
                    break;
                case 99:
                    System.Windows.Forms.MessageBox.Show("Прекрати!");
                    break;
                case 101:
                    System.Windows.Forms.MessageBox.Show("Как хочешь...");
                    break;
                case 150:
                    App.AnimateDoubleEffect(ImageLogo, OpacityProperty, 0d, TimeSpan.FromMilliseconds(100d));
                    System.Windows.Forms.MessageBox.Show("АХАХАХ АХАХАХАА ХАХАХАХ");
                    break;
                case 200:
                    ImageLogo.BeginAnimation(OpacityProperty, null);
                    ImageLogo.Opacity = 1d;
                    ImageLogo.Source = App.LoadImage(Properties.Resources.BlackSquare);
                    ImageLogo.UpdateLayout();
                    for (int i = 0; i < 7; i++) System.Windows.Forms.MessageBox.Show(string.Empty);
                    break;
            }
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
