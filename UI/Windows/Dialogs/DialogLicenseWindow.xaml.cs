using OperPageLes.CORE;
using OperPageLes.CORE.Struct;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для LicenseWindow.xaml
    /// </summary>
    public partial class DialogLicenseWindow : Window
    {
        /// <summary>
        /// Дата начала разработки программы
        /// </summary>
        private static readonly DateTime HappyDay = new(2022, 04, 19); // 04 19

        /// <summary>
        /// Константа времени изчезновения страницы
        /// </summary>
        const double MillisecondsHide = 1250d;

        /// <summary>
        /// Константа времени появления страницы
        /// </summary>
        const double MillisecondsShow = 1200d;

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

        /// <summary>
        /// Загруженные данные о картинках асистентов
        /// </summary>
        private static BitmapImage[] BitmapsAssistents = [..Assistents.AllAssistents.Select((i) =>
                    App.LoadImage(i.ImageSource ?? Properties.Resources.IconMainGray))];

        /// <summary>
        /// Поток отображаемый данные об асистентах
        /// </summary>
        private readonly Thread ThreadUpdateVisualAssistents;
        #endregion

        public DialogLicenseWindow()
        {
            InitializeComponent();
            ThreadUpdateVisualAssistents = new(() =>
            {
                Assistents.AssistentElement assistent;
                int i = -1;
                ThicknessAnimation animation = Dispatcher.Invoke(() =>
                {
                    ThicknessAnimation animate = App.ThicknessAnimationType.SourceAnimation.Clone();
                    animate.Duration = TimeSpan.FromSeconds(6d);
                    return animate;
                });
                Dispatcher.Invoke(() =>
                {
                    animation.EasingFunction = new PowerEase()
                    {
                        EasingMode = EasingMode.EaseOut,
                        Power = 6d,
                    };
                    animation.From = new(-10);
                    animation.To = new(-60);
                });
                while (true)
                {
                    i = ++i % Assistents.AllAssistents.Count;
                    assistent = Assistents.AllAssistents[i];
                    Dispatcher.BeginInvoke(() =>
                    {
                        ((SolidColorBrush)TextBlockNickName.Foreground).Color = assistent.ColorNickName;
                        ((SolidColorBrush)TextBlockPhrase.Foreground).Color = assistent.ColorPhrase;
                        TextBlockNickName.Text = assistent.NickName;
                        TextBlockPhrase.Text = $"\"{assistent.Phrase}\"";
                        TextBlockMessage.Text = assistent.Message;
                        ImageIconNickName.Source = BitmapsAssistents[i];
                        ImageIconNickName.UpdateLayout();
                        App.DoubleAnimationType.AnimateEffect(ImageIconNickName, OpacityProperty, 0.4d, TimeSpan.FromMilliseconds(3000d));
                        App.DoubleAnimationType.AnimateEffect(MainGrid, OpacityProperty, 1d, TimeSpan.FromMilliseconds(MillisecondsShow));
                        App.ThicknessAnimationType.AnimateEffect(MainGrid, MarginProperty, new(0), TimeSpan.FromMilliseconds(MillisecondsShow));

                        ImageIconNickName.BeginAnimation(MarginProperty, animation, HandoffBehavior.SnapshotAndReplace);
                    });
                    Thread.Sleep(13600);
                    Dispatcher.BeginInvoke(() =>
                    {
                        App.DoubleAnimationType.AnimateEffect(MainGrid, OpacityProperty, 0d, TimeSpan.FromMilliseconds(MillisecondsHide));
                        App.ThicknessAnimationType.AnimateEffect(MainGrid, MarginProperty, new(0, 30, 0, 0), TimeSpan.FromMilliseconds(MillisecondsHide));
                        App.DoubleAnimationType.AnimateEffect(ImageIconNickName, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1000d));
                    });
                    Thread.Sleep((int)MillisecondsHide + 100);
                }
            })
            {
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true,
            };
            ThreadUpdateVisualAssistents.SetApartmentState(ApartmentState.STA);
            TextBlockVersion.Text = "Версия: " +
#if DEBUG
                "DEBUG";
#endif
#if !DEBUG
                $"{App.Version}";
#endif
            MainGrid.Opacity = 0d;
            MainGrid.Margin = new(0, 28, 0, 0);
            MediaHappy.Source = new Uri(StructDirectoryResources.DirectoryFileHappy);
            MediaHappy.MediaEnded += (sender, e) =>
            {
                MediaHappy.Position = TimeSpan.FromMilliseconds(1);
            };
            ImageLogo.Margin = new(20);
            ImageLogo.MouseLeftButtonUp += (sender, e) =>
            {
                if (CountClickImageLogo >= 200) return;
                ExecuteEventClickImageLogo(++CountClickImageLogo);
            };
            Closed += (sender, e) =>
            {
                GC.Collect();
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
            };
            BorderHappy.MouseLeftButtonUp += (sender, e) =>
            {
                HideHappy();
            };
            ImageIconNickName.Opacity = 0d;
            BlurEffectAllGrid.Radius = 0d;
            MediaHappy.Opacity = 0d;
            BorderHappy.Opacity = 0d;
            BorderHappy.Margin = new(5);
            ImageLogo.Opacity = 0d;
            Opacity = 0d;
            TextBlockNextInfo.Opacity = 0d;
            BorderHappy.Visibility = Visibility.Hidden;
            ImageIconNickName.Source = null;

            TextBlockNickName.Foreground = new SolidColorBrush(Colors.Black);
            TextBlockPhrase.Foreground = new SolidColorBrush(Colors.Black);
        }

        /// <summary>
        /// Активировать анимацию дня рождения программы
        /// </summary>
        internal void ShowHappy()
        {
            ThicknessAnimation animThickness = App.ThicknessAnimationType.SourceAnimation.Clone();
            animThickness.BeginTime = TimeSpan.FromMilliseconds(100d);
            animThickness.Duration = TimeSpan.FromMilliseconds(2000d);

            DoubleAnimation animDouble = App.DoubleAnimationType.SourceAnimation.Clone();
            animDouble.Duration = TimeSpan.FromMilliseconds(1600d);
            animDouble.BeginTime = TimeSpan.FromMilliseconds(400d);

            uint YearRealy = (uint)(DateTime.Now.Year - HappyDay.Year);
            string SyntaxYear = string.Empty;
            if (YearRealy < 5u || (YearRealy % 10 == 1 && YearRealy != 11))
                SyntaxYear += $"год{(YearRealy > 1u && YearRealy < 5u ? "а" : string.Empty)}";
            else SyntaxYear += "лет";
            TextBlockHappyYear.Text = $"Системе исполнитось {YearRealy} {SyntaxYear}";
            Canvas.SetZIndex(BorderHappy, 1);
            MediaHappy.Play();

            animThickness.To = new(0);
            BorderHappy.BeginAnimation(MarginProperty, animThickness, HandoffBehavior.SnapshotAndReplace);

            animDouble.To = 1d;
            TextBlockNextInfo.BeginAnimation(OpacityProperty, animDouble, HandoffBehavior.SnapshotAndReplace);
            MediaHappy.BeginAnimation(OpacityProperty, animDouble, HandoffBehavior.SnapshotAndReplace);
            BorderHappy.BeginAnimation(OpacityProperty, animDouble, HandoffBehavior.SnapshotAndReplace);
            BorderHappy.Visibility = Visibility.Visible;
            GridHappy.Visibility = Visibility.Visible;

            ScaleEffectElement.BeginAnimation(ScaleTransform.ScaleXProperty, animDouble, HandoffBehavior.SnapshotAndReplace);
            ScaleEffectElement.BeginAnimation(ScaleTransform.ScaleYProperty, animDouble, HandoffBehavior.SnapshotAndReplace);

            animDouble.To = 10d;
            BlurEffectAllGrid.BeginAnimation(BlurEffect.RadiusProperty, animDouble, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// Диактивировать анимацию дня рождения программы
        /// </summary>
        internal void HideHappy()
        {
            TimeSpan span = TimeSpan.FromMilliseconds(800d);
            Canvas.SetZIndex(GridHappy, -1);
            App.DoubleAnimationType.AnimateEffect(BorderHappy, OpacityProperty, 0d, span);
            App.DoubleAnimationType.AnimateEffect(MediaHappy, OpacityProperty, 0d, span);
            App.DoubleAnimationType.AnimateEffect(BlurEffectAllGrid, BlurEffect.RadiusProperty, 0d, span);
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
                case 15:
                case 25:
                case 50:
                case 60:
                case 65:
                case 75:
                case 87:
                    int RandomOffset = new Random(CountClick).Next(0, 45);
                    App.ThicknessAnimationType.AnimateEffect(ImageLogo, MarginProperty, new(0, RandomOffset, 0, RandomOffset), TimeSpan.FromMilliseconds(800d));
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
                    App.DoubleAnimationType.AnimateEffect(ImageLogo, OpacityProperty, 0d, TimeSpan.FromMilliseconds(100d));
                    ImmuneClosing = true;
                    System.Windows.Forms.MessageBox.Show("АХАХАХ АХАХАХАА ХАХАХАХ");
                    ImmuneClosing = false;
                    break;
                case 200:
                    ImageLogo.BeginAnimation(OpacityProperty, null, HandoffBehavior.SnapshotAndReplace);
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
            App.DoubleAnimationType.AnimateEffect(ImageLogo, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
            App.DoubleAnimationType.AnimateEffect(this, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1200d));
            if (!IsActivatedShow)
            {
                IsActivatedShow = true;
                DoubleAnimation anim = new()
                {
                    From = 0d,
                    To = 360d,
                    Duration = TimeSpan.FromSeconds(3d),
                    EasingFunction = new ElasticEase()
                    {
                        EasingMode = EasingMode.EaseOut,
                        Oscillations = 1,
                        Springiness = 4d,
                    }
                };
                RotateTransformImageIconApplication.BeginAnimation(RotateTransform.AngleProperty, anim, HandoffBehavior.SnapshotAndReplace);
                anim.Duration = TimeSpan.FromSeconds(10d);
                anim.EasingFunction = new PowerEase()
                {
                    EasingMode = EasingMode.EaseInOut,
                    Power = 6d,
                };
                anim.RepeatBehavior = RepeatBehavior.Forever;
                RotateTransformTextAutor.BeginAnimation(RotateTransform.AngleProperty, anim, HandoffBehavior.SnapshotAndReplace);
                ThicknessAnimation animThickness = App.ThicknessAnimationType.SourceAnimation.Clone();
                animThickness.BeginTime = TimeSpan.FromMilliseconds(80d);
                animThickness.Duration = TimeSpan.FromSeconds(4d);
                animThickness.To = new(5);
                animThickness.EasingFunction = new BackEase()
                {
                    EasingMode = EasingMode.EaseOut,
                    Amplitude = 1d,
                };
                ImageLogo.BeginAnimation(MarginProperty, animThickness, HandoffBehavior.SnapshotAndReplace);
            };

            base.Show();
            Focus();
            if (DateTime.Now.Month == HappyDay.Month && DateTime.Now.Day == HappyDay.Day) ShowHappy();
            ThreadUpdateVisualAssistents.Start();
        }
    }
}
