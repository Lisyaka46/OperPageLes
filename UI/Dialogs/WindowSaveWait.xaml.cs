using OperPageLes.CORE;
using OperPageLes.CORE.Struct;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace OperPageLes.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowSaveWait.xaml
    /// </summary>
    public partial class WindowSaveWait : Window
    {
        /// <summary>
        /// Токен управляемой асинхронной операцией отображения обновления информации управляемая завершением сохранения
        /// </summary>
        private CancellationToken TaskTokenComplete;

        /// <summary>
        /// Количество секунд потраченых на сохранение
        /// </summary>
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
            TaskTokenComplete = new(false);
            ProgressBarIndicator.Value = 0d;
            IndicatorLoading.Source = new Uri(StructDirectoryResources.DirectoryFileLoadingDefault);

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

            Task.Run(() =>
            {
                while (true)
                {
                    Dispatcher.Invoke(() => TextBlockTime.Text = $"{++Count}");
                    Thread.Sleep(1000);
                }
            }, TaskTokenComplete);

            Task.Run(() =>
            {
                while (true)
                {
                    double x_y = random.Next(30, 80) / 100d;
                    Dispatcher.Invoke(() =>
                    {
                        Point_Animation.To = new(x_y, x_y);
                        RadialGradientBackground.BeginAnimation(RadialGradientBrush.CenterProperty, Point_Animation);
                        RadialGradientBackground.BeginAnimation(RadialGradientBrush.GradientOriginProperty, Point_Animation);
                    });
                    Thread.Sleep(2000);
                }
            }, TaskTokenComplete);

            TextBlockTime.Text = "0";
            Opacity = 0d;
            DoubleAnimation animation = App.GetDoubleAnimate();
            animation.BeginTime = TimeSpan.FromMilliseconds(20d);
            animation.Duration = TimeSpan.FromMilliseconds(1270d);
            animation.From = 0d;
            animation.To = 1d;
            BeginAnimation(OpacityProperty, animation);
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
                OpacityMask = new LinearGradientBrush()
                {
                    GradientStops = [
                        new GradientStop(Colors.Black, 0.298d),
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
        internal async Task Complete()
        {
            await Task.Run(() =>
            {
                TaskTokenComplete.ThrowIfCancellationRequested();
                Thread.Sleep(1000);
                Dispatcher.Invoke(Close);
            });
        }
    }
}
