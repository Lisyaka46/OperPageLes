using OperPage_les.CORE;
using OperPage_les.UI.Pages.License;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using WpfAnimatedGif;

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
        /// Индекс отображения благодарственного сообщения
        /// </summary>
        private int Value = -1;

        /// <summary>
        /// Отображалась ли программа хотя бы раз
        /// </summary>
        private bool IsActivatedShow = false;

        /// <summary>
        /// Имуннитет к закрытию окна при смещении фокуса на другое окно
        /// </summary>
        private bool ImmuneClosing = false;


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
            MediaHappy.Source = new Uri(App.DirectoryImageHappy);
            MediaHappy.MediaEnded += (sender, e) =>
            {
                MediaHappy.Position = TimeSpan.FromMilliseconds(1);
            };

            UpdateInfoThanks = new(10000d, (sender, e) => Dispatcher.BeginInvoke(UpdateThanks));
            ImageLogo.Margin = new(20);
            ImageLogo.MouseLeftButtonUp += (sender, e) =>
            {
                ExecuteEventClickImageLogo(++CountClickImageLogo);
            };
            Closed += (sender, e) =>
            {
                GC.Collect(2, GCCollectionMode.Forced);
                UpdateInfoThanks.Stop();
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
            Deactivated += (sender, e) =>
            {
                try
                {
                    if (ImmuneClosing) return;
                    Close();
                }
                catch
                {

                }
                //Opacity = 0d;
                //MediaHappy.Opacity = 0d;
                //BorderHappy.Opacity = 0d;
                //BorderHappy.Margin = new(5);
                //BlurEffectAllGrid.Radius = 0d;
            };
            BorderHappy.MouseLeftButtonUp += (sender, e) =>
            {
                HideHappy();
            };
            BlurEffectAllGrid.Radius = 0d;
            MediaHappy.Opacity = 0d;
            BorderHappy.Opacity = 0d;
            BorderHappy.Margin = new(5);
            ImageLogo.Opacity = 0d;
            Opacity = 0d;
            TextBlockNextInfo.Opacity = 0d;
            BorderHappy.Visibility = Visibility.Hidden;

            DoubleAnimation anim = new()
            {
                To = 360d,
                Duration = TimeSpan.FromSeconds(4d),
                RepeatBehavior = RepeatBehavior.Forever,
            };
            RotateTransformTextAutor.BeginAnimation(RotateTransform.AngleProperty, anim);
            RotateTransformImageIconApplication.BeginAnimation(RotateTransform.AngleProperty, LogoRotateAnimate);
        }

        /// <summary>
        /// Активировать анимацию дня рождения программы
        /// </summary>
        internal void ShowHappy()
        {
            ThicknessAnimation animThickness = App.GetThicknessAnimate(TimeSpan.FromMilliseconds(2000d));
            animThickness.BeginTime = TimeSpan.FromMilliseconds(100d);
            DoubleAnimation animDouble = App.GetDoubleAnimate(TimeSpan.FromMilliseconds(1600d));
            animDouble.BeginTime = TimeSpan.FromMilliseconds(400d);
            DateTime HappyDay = new(2022, 04, 19);
            uint YearRealy = (uint)(DateTime.Now.Year - HappyDay.Year);
            string SyntaxYear = string.Empty;
            if (YearRealy < 5u || (YearRealy % 10 == 1 && YearRealy != 11))
                SyntaxYear += $"год{(YearRealy > 1u && YearRealy < 5u ? "а" : string.Empty)}";
            else SyntaxYear += "лет";
            TextBlockHappyYear.Text = $"Системе исполнитось {YearRealy} {SyntaxYear}";
            Canvas.SetZIndex(BorderHappy, 1);
            MediaHappy.Play();

            animThickness.To = new(0);
            BorderHappy.BeginAnimation(MarginProperty, animThickness);

            animDouble.To = 1d;
            TextBlockNextInfo.BeginAnimation(OpacityProperty, animDouble);
            MediaHappy.BeginAnimation(OpacityProperty, animDouble);
            BorderHappy.BeginAnimation(OpacityProperty, animDouble);
            BorderHappy.Visibility = Visibility.Visible;

            ScaleEffectElement.BeginAnimation(ScaleTransform.ScaleXProperty, animDouble);
            ScaleEffectElement.BeginAnimation(ScaleTransform.ScaleYProperty, animDouble);

            animDouble.To = 10d;
            BlurEffectAllGrid.BeginAnimation(BlurEffect.RadiusProperty, animDouble);
        }

        /// <summary>
        /// Диактивировать анимацию дня рождения программы
        /// </summary>
        internal void HideHappy()
        {
            TimeSpan span = TimeSpan.FromMilliseconds(800d);
            Canvas.SetZIndex(BorderHappy, -1);
            App.AnimateDoubleEffect(BorderHappy, OpacityProperty, 0d, span);
            App.AnimateDoubleEffect(MediaHappy, OpacityProperty, 0d, span);
            App.AnimateDoubleEffect(BlurEffectAllGrid, BlurEffect.RadiusProperty, 0d, span);
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
                    ImmuneClosing = true;
                    System.Windows.Forms.MessageBox.Show("Всё, больше ничего не будет.");
                    System.Windows.Forms.MessageBox.Show("Я правду говорю");
                    ImmuneClosing = false;
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
                    ImmuneClosing = true;
                    System.Windows.Forms.MessageBox.Show("Прекрати!");
                    ImmuneClosing = false;
                    break;
                case 101:
                    ImmuneClosing = true;
                    System.Windows.Forms.MessageBox.Show("Как хочешь...");
                    ImmuneClosing = false;
                    break;
                case 150:
                    App.AnimateDoubleEffect(ImageLogo, OpacityProperty, 0d, TimeSpan.FromMilliseconds(100d));
                    ImmuneClosing = true;
                    System.Windows.Forms.MessageBox.Show("АХАХАХ АХАХАХАА ХАХАХАХ");
                    ImmuneClosing = false;
                    break;
                case 200:
                    ImageLogo.BeginAnimation(OpacityProperty, null);
                    ImageLogo.Opacity = 1d;
                    ImageLogo.Source = App.LoadImage(Properties.Resources.BlackSquare);
                    ImageLogo.UpdateLayout();
                    ImmuneClosing = true;
                    for (int i = 0; i < 7; i++) System.Windows.Forms.MessageBox.Show(string.Empty);
                    ImmuneClosing = false;
                    break;
            }
        }

        /// <summary>
        /// Отобразить окно лицензии
        /// </summary>
        public new void Show()
        {      
            App.AnimateDoubleEffect(ImageLogo, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
            App.AnimateDoubleEffect(this, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1200d));
            if (!IsActivatedShow)
            {
                ThicknessAnimation animThickness = App.GetThicknessAnimate(TimeSpan.FromMilliseconds(1200d));
                animThickness.BeginTime = TimeSpan.FromMilliseconds(80d);
                animThickness.Duration = TimeSpan.FromMilliseconds(1600d);
                animThickness.To = new(0);
                animThickness.EasingFunction = new BackEase()
                {
                    EasingMode = EasingMode.EaseOut,
                    Amplitude = 0.78d,
                };
                ImageLogo.BeginAnimation(MarginProperty, animThickness);
                FrameThanks.Navigate(PageThanks);
                UpdateThanks();
                UpdateInfoThanks.Start();
            }
            IsActivatedShow = true;
            base.Show();
            Focus();
            ShowHappy();
        }

        /// <summary>
        /// Обновить панель благодарности
        /// </summary>
        private void UpdateThanks()
        {
            PageThanks.NextUser(Assistents.AllAssistents[Value = ++Value % Assistents.AllAssistents.Length]);
        }
    }
}
