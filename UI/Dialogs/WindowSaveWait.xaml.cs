using OperPage_les.CORE;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace OperPage_les.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowSaveWait.xaml
    /// </summary>
    public partial class WindowSaveWait : Window
    {
        private UpdateBackgroundData UpdateCheckCompleteSave;
        private UpdateBackgroundData UpdateBackground;
        private bool Compliting = false;
        private int Count = 0;
        private static readonly PointAnimation Point_Animation = new()
        {
            EasingFunction = new QuadraticEase()
            {
                EasingMode = EasingMode.EaseInOut,
            },
            Duration = TimeSpan.FromMilliseconds(2000d),
        };
        public WindowSaveWait()
        {
            InitializeComponent();
            ProgressBarIndicator.Value = 0d;
            UpdateCheckCompleteSave = new((sender, e) => { });
            UpdateBackground = new((sender, e) => { });
            IndicatorLoading.Source = new Uri(App.DirectoryFileLoadingDefault);

            IndicatorLoading.MediaEnded += (sender, e) =>
            {
                IndicatorLoading.Position = TimeSpan.FromMilliseconds(1);
            };
            BorderMain.MouseLeftButtonDown += (sender, e) =>
            {
                DragMove();
            };
        }

        /// <summary>
        /// Добавить в браузер страниц новую вкладку и активировать страницу
        /// </summary>
        /// <param name="BrowserPage">Браузер для взаимодействия</param>
        /// <returns>Успешно или нет</returns>
        internal void OpenOnToComplete()
        {
            Random random = new(DateTime.Now.Millisecond);
            UpdateCheckCompleteSave = new(1000d, (sender, e) => Dispatcher.BeginInvoke(() =>
            {
                TextBlockTime.Text = $"{++Count}";
                double x_y = random.NextDouble();
                Point_Animation.To = new(x_y, x_y);
                RadialGradientBackground.BeginAnimation(RadialGradientBrush.CenterProperty, Point_Animation);
                if (Compliting)
                {
                    Close();
                    UpdateCheckCompleteSave.Stop();
                    UpdateBackground.Stop();
                }
            }));
            UpdateBackground = new(2000d, (sender, e) => Dispatcher.BeginInvoke(() =>
            {
                double x_y = random.Next(30, 80) / 100d;
                Point_Animation.To = new(x_y, x_y);
                RadialGradientBackground.BeginAnimation(RadialGradientBrush.CenterProperty, Point_Animation);
                RadialGradientBackground.BeginAnimation(RadialGradientBrush.GradientOriginProperty, Point_Animation);
            }));
            TextBlockTime.Text = "0";
            Opacity = 0d;
            DoubleAnimation animation = App.GetDoubleAnimate();
            animation.BeginTime = TimeSpan.FromMilliseconds(20d);
            animation.Duration = TimeSpan.FromMilliseconds(1270d);
            animation.From = 0d;
            animation.To = 1d;
            BeginAnimation(OpacityProperty, animation);
            UpdateCheckCompleteSave.Start();
            UpdateBackground.Start();
            Show();
        }

        /// <summary>
        /// Отобразить текст что конкретно происходит
        /// </summary>
        /// <param name="Text">Отображаемый текст</param>
        internal void SetVisualTextSaving(string Text)
        {
            TextBlockInfoSaving.Text = Text;
        }

        /// <summary>
        /// Создать маркер
        /// </summary>
        /// <returns>Объект маркера</returns>
        private static System.Windows.Shapes.Rectangle CreateMarker()
        {
            System.Windows.Shapes.Rectangle rectangle = new()
            {
                Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 0)),
                Width = 4,
                Height = 20,
                Opacity = 0.6d,
                Margin = new(0),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                OpacityMask = new System.Windows.Media.LinearGradientBrush()
                {
                    GradientStops = [
                        new GradientStop(System.Windows.Media.Colors.Black, 0.298d),
                        new GradientStop(System.Windows.Media.Color.FromArgb(0, 0, 0, 0), 0.445d),
                        ],
                    StartPoint = new System.Windows.Point(0.5d, 0),
                    EndPoint = new System.Windows.Point(0.5d, 1),
                },
                RadiusX = 2,
                RadiusY = 2,
            };
            Grid.SetRowSpan(rectangle, 2);
            return rectangle;
        }

        /// <summary>
        /// Отобразить текст что конкретно происходит
        /// </summary>
        /// <param name="Text">Отображаемый текст</param>
        /// <param name="ValueIndicator">Значение устанавливаемое для индикатора</param>
        internal void SetVisualTextSaving(string Text, double ValueIndicator)
        {
            TextBlockInfoSaving.Text = Text;
            System.Windows.Shapes.Rectangle Marker = CreateMarker();
            Thickness StartMargin = Marker.Margin = new(GridProgressBar.ActualWidth / ProgressBarIndicator.Maximum * ValueIndicator, 0, 0, -2);
            GridProgressBar.Children.Add(Marker);
            Marker.UpdateLayout();
            App.AnimateThicknessEffect(Marker, MarginProperty, new(StartMargin.Left, 0, 0, 0), TimeSpan.FromMilliseconds(700d));
            App.AnimateDoubleEffect(ProgressBarIndicator, System.Windows.Controls.ProgressBar.ValueProperty, ValueIndicator, TimeSpan.FromMilliseconds(400d));
        }

        /// <summary>
        /// Завершить загрузку
        /// </summary>
        internal void Complete()
        {
            Compliting = true;
        }
    }
}
