using OperPageLes.CORE;
using OperPageLes.CORE.Struct;
using OPLAnimation.CORE.Animation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowSaveWait.xaml
    /// </summary>
    public partial class DialogSaveWait : Window
    {
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

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

        /// <summary>
        /// Состояние активации перемещения окна по экрану
        /// </summary>
        private bool ActivateMoveWindow = false;

        public DialogSaveWait()
        {
            InitializeComponent();
            //VisualLoading.ManagerAnimation = App.CurrentApp.ManagerAnimation;
            VisualLoading.Opacity = 0d;
            LineProgress.X1 = 3;
            LineProgress.X2 = 3;
            TaskTokenComplete = new(false);
            BorderMain.MouseLeftButtonDown += (sender, e) =>
            {
                ActivateMoveWindow = true;
                DragMove();
                ActivateMoveWindow = false;
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
            VisualLoading.OpenLoading();
            void ActionBackgroundChange()
            {
                double x_y = random.Next(30, 80) / 100d;
                Point_Animation.To = new(x_y, x_y);
                RadialGradientBackground.BeginAnimation(RadialGradientBrush.CenterProperty, Point_Animation);
                RadialGradientBackground.BeginAnimation(RadialGradientBrush.GradientOriginProperty, Point_Animation);
            }
            void SetText() => TextBlockTime.Text = $"{++Count}";

            Task.Run(() =>
            {
                while (true)
                {
                    Dispatcher.Invoke(SetText);
                    Thread.Sleep(1000);
                }
            }, TaskTokenComplete);

            Task.Run(() =>
            {
                while (true)
                {
                    Dispatcher.Invoke(ActionBackgroundChange);
                    Thread.Sleep(2000);
                }
            }, TaskTokenComplete);

            TextBlockTime.Text = "0";
            if (ManagerAnimation != null)
            {
                Opacity = 0d;
                DoubleAnimation animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animation.BeginTime = TimeSpan.FromMilliseconds(20d);
                animation.Duration = TimeSpan.FromMilliseconds(1270d);
                animation.From = 0d;
                animation.To = 1d;
                BeginAnimation(OpacityProperty, animation);
            }
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
            //System.Windows.Shapes.Rectangle Marker = CreateMarker();
            //Thickness StartMargin = Marker.Margin = new(GridProgressBar.ActualWidth / ProgressBarIndicator.Maximum * ValueIndicator, 0, 0, -2);
            //GridProgressBar.Children.Add(Marker);
            //Marker.UpdateLayout();
            //App.CurrentApp.ManagerAnimation.ThicknessAnimationType.AnimateEffect(Marker, MarginProperty, new(StartMargin.Left, 0, 0, 0), TimeSpan.FromMilliseconds(700d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, LineProgress, Line.X2Property,
                ValueIndicator / 100 * 438 + 3, TimeSpan.FromMilliseconds(400d));
        }

        /// <summary>
        /// Завершить загрузку
        /// </summary>
        internal async Task Complete()
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(VisualLoading.CloseLoading);
                TaskTokenComplete.ThrowIfCancellationRequested();
                if (ActivateMoveWindow)
                {
                    Dispatcher.Invoke(() => TextBlockHead.Text = "!! ОТПУСТИ МЕНЯ !!");
                    while (ActivateMoveWindow)
                        Thread.Sleep(500);
                }
                Thread.Sleep(1000);
                Dispatcher.Invoke(Close);
            });
        }
    }
}
