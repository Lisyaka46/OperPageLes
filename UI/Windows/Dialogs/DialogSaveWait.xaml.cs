using OperPageLes.CORE;
using OPLAPI.CORE.Animation;
using OPLAPI.OIEL.UserElementsControl.Base;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowSaveWait.xaml
    /// </summary>
    public partial class DialogSaveWait : OPLWindowBase
    {
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public override OPLAnimationManager? ManagerAnimation
        {
            get => base.ManagerAnimation;
            set
            {
                base.ManagerAnimation = value;
                VisualLoading.ManagerAnimation = value;
            }
        }

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

        /// <summary>
        /// Секундный таймер для отображения времени потраченного на сохранение
        /// </summary>
        private System.Windows.Forms.Timer TimerSecond;

        /// <summary>
        /// Контроллер рандомного числа
        /// </summary>
        private Random RandomController;

        public DialogSaveWait()
        {
            InitializeComponent();
            RandomController = new(DateTime.Now.Millisecond);
            VisualLoading.Opacity = 0d;
            LineProgress.X1 = 3;
            TimerSecond = new()
            {
                Interval = 1000
            };
            TimerSecond.Tick += TimerSecond_TickHandler;
            MouseLeftButtonDown += (sender, e) =>
            {
                ActivateMoveWindow = true;
                DragMove();
                ActivateMoveWindow = false;
            };
        }

        /// <summary>
        /// Активировать визуальное отображение выполнение процессов
        /// </summary>
        /// <remarks>
        /// Отображает окно и производит визуализацию выполнения процессов.<br/>
        /// В качестве параметра принимает массив процессов <see cref="ActionManipulateData"/>, которые фоново выполняются в процессе визуализации
        /// </remarks>
        internal async Task ActivateVisualManipulate(ActionManipulateData[] StageActions)
        {
            VisualLoading.OpenLoading();
            LineProgress.BeginAnimation(Line.X2Property, null);
            LineProgress.X2 = 3;
            Count = 1;
            TextBlockTime.Text = Count.ToString();
            TimerSecond.Start();
            Show();
            Focus();
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, this, OpacityProperty,
                0d, 1d, TimeSpan.FromMilliseconds(1270d));
            for (int i = 0; i < StageActions.Length; i++)
            {
                SetVisualStageAction(StageActions[i].Name, (double)i / (double)StageActions.Length * 100d);
                await Dispatcher.Invoke(StageActions[i].InvokeActionSave);
            }
            SetVisualStageAction("Ожидание завершения...", 100d);
            VisualLoading.CloseLoading();
            if (ActivateMoveWindow)
            {
                TextBlockHead.Text = "!! ОТПУСТИ МЕНЯ !!";
                while (ActivateMoveWindow)
                    await Task.Delay(100);
            }
            await Task.Delay(400);
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, this, OpacityProperty,
                1d, 0d, TimeSpan.FromMilliseconds(570d));
            await Task.Delay(700);
            Opacity = 0d;
            BeginAnimation(OpacityProperty, null);
            UpdateLayout();
            Hide();
            TimerSecond.Stop();
        }

        /// <summary>
        /// Обработчик события секундного тика таймера
        /// </summary>
        private void TimerSecond_TickHandler(object? sender, EventArgs e)
        {
            TextBlockTime.Text = $"{++Count}";
            ActionBackgroundChange();
        }

        /// <summary>
        /// Изменить позицию бликающегося градиента на фоне окна
        /// </summary>
        private void ActionBackgroundChange()
        {
            double x_y = RandomController.Next(30, 80) / 100d;
            Point_Animation.To = new(x_y, x_y);
            RadialGradientBackground.BeginAnimation(RadialGradientBrush.CenterProperty, Point_Animation);
            RadialGradientBackground.BeginAnimation(RadialGradientBrush.GradientOriginProperty, Point_Animation);
        }

        /// <summary>
        /// Обновить визуализацию выполнения процесса
        /// </summary>
        /// <param name="Text">Отображаемый текст</param>
        /// <param name="ValueIndicator">Значение устанавливаемое для индикатора. 100.0 полностью заполнено / 0.0 пусто</param>
        internal void SetVisualStageAction(string Text, double ValueIndicator)
        {
            TextBlockInfoSaving.Text = Text;
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, LineProgress, Line.X2Property,
                ValueIndicator / 100 * 438 + 3, TimeSpan.FromMilliseconds(400d));
            UpdateLayout();
        }
    }
}
