using ApplicationOperPageLes.CORE;
using ApplicationOperPageLes.CORE.Struct;
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
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.Windows.Dialogs
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

        public DialogLicenseWindow()
        {
            InitializeComponent();
            
            //{
            //    Priority = ThreadPriority.BelowNormal,
            //    IsBackground = true,
            //};
            //ThreadUpdateVisualAssistents.SetApartmentState(ApartmentState.STA);
            TextBlockVersion.Text = "Версия: " +
#if DEBUG
                "DEBUG";
#endif
#if !DEBUG
                $"{App.Version}";
#endif
            BorderMainAssistentVisual.Opacity = 0d;
            BorderMainAssistentVisual.Margin = new(0, 28, 0, 0);
            ImageLogo.Margin = new(20);
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
                    Close();
                }
                catch
                {

                }
            };
            Activated += (sender, e) =>
            {
                Task.Run(() =>
                {
                    Assistents.AssistentElement assistent;
                    int i = -1;
                    ThicknessAnimation animation = Dispatcher.Invoke(() =>
                    {
                        ThicknessAnimation animate = App.ManagerAnimation.ThicknessAnimationType.SourceAnimation.Clone();
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
                            TextBlockNickName.Text = assistent.NickName;
                            TextBlockMessage.Text = assistent.Message;
                            ImageIconNickName.ImageSource = StructDirectoryResources.GetResourceBitmap(assistent.NameImageSource ?? nameof(OPRES.IconMainApplication));
                            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageIconNickName, ImageBrush.OpacityProperty, 0.4d, TimeSpan.FromMilliseconds(3000d));
                            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderMainAssistentVisual, OpacityProperty, 1d, TimeSpan.FromMilliseconds(MillisecondsShow));
                            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(BorderMainAssistentVisual, MarginProperty, new(0), TimeSpan.FromMilliseconds(MillisecondsShow));

                            App.ManagerAnimation.RectAnimationType.AnimateEffect(ImageIconNickName, ImageBrush.ViewboxProperty, new(0.025, 0.025, 0.95, 0.95), new(0, 0, 1, 1), TimeSpan.FromMilliseconds(MillisecondsShow));
                        });
                        Thread.Sleep(13600);
                        Dispatcher.BeginInvoke(() =>
                        {
                            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderMainAssistentVisual, OpacityProperty, 0d, TimeSpan.FromMilliseconds(MillisecondsHide));
                            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(BorderMainAssistentVisual, MarginProperty, new(0, 30, 0, 0), TimeSpan.FromMilliseconds(MillisecondsHide));
                            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageIconNickName, ImageBrush.OpacityProperty, 0d, TimeSpan.FromMilliseconds(1000d));
                        });
                        Thread.Sleep((int)MillisecondsHide + 100);
                    }
                });
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
            ImageIconNickName.ImageSource = null;

            TextBlockNickName.Foreground = new SolidColorBrush(Colors.Black);
        }

        /// <summary>
        /// Активировать анимацию дня рождения программы
        /// </summary>
        internal void ShowHappy()
        {
            MediaHappy.Source = StructDirectoryResources.GetResourceUri(nameof(OPRES.MediaHappy));
            MediaHappy.MediaEnded += (sender, e) =>
            {
                MediaHappy.Position = TimeSpan.FromMilliseconds(1);
            };
            ThicknessAnimation animThickness = App.ManagerAnimation.ThicknessAnimationType.SourceAnimation.Clone();
            animThickness.BeginTime = TimeSpan.FromMilliseconds(100d);
            animThickness.Duration = TimeSpan.FromMilliseconds(2000d);

            DoubleAnimation animDouble = App.ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
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

            animDouble.From = 0.9d;
            ScaleEffectElement.BeginAnimation(ScaleTransform.ScaleXProperty, animDouble, HandoffBehavior.SnapshotAndReplace);
            ScaleEffectElement.BeginAnimation(ScaleTransform.ScaleYProperty, animDouble, HandoffBehavior.SnapshotAndReplace);

            animDouble.From = null;
            animDouble.To = 10d;
            BlurEffectAllGrid.BeginAnimation(BlurEffect.RadiusProperty, animDouble, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// Диактивировать анимацию дня рождения программы
        /// </summary>
        internal void HideHappy()
        {
            TimeSpan span = TimeSpan.FromMilliseconds(800d);
            DoubleAnimation animDouble = App.ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
            animDouble.FillBehavior = FillBehavior.Stop;
            animDouble.Duration = span;
            animDouble.To = 0d;
            animDouble.Completed += (sender, e) =>
            {
                MediaHappy.Source = null;
            };
            Canvas.SetZIndex(GridHappy, -1);
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderHappy, OpacityProperty, 0d, span);
            MediaHappy.BeginAnimation(OpacityProperty, animDouble);
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BlurEffectAllGrid, BlurEffect.RadiusProperty, 0d, span);
        }

        /// <summary>
        /// Отобразить окно лицензии
        /// </summary>
        public new void Show()
        {      
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageLogo, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(this, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1200d));
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
                ThicknessAnimation animThickness = App.ManagerAnimation.ThicknessAnimationType.SourceAnimation.Clone();
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
            //ShowHappy();
        }
    }
}
