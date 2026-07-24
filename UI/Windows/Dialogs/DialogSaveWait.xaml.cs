using OperPageLes.CORE;
using OPLAPI.CORE.Animation;
using OPLAPI.OIEL.UserElementsControl.Base;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowSaveWait.xaml
    /// </summary>
    public partial class DialogManipulateActionWait : OPLWindowBase
    {
        #region Properties

        #region TitleHead
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TitleHeadProperty =
            DependencyProperty.Register("TitleHead", typeof(string), typeof(DialogManipulateActionWait),
                new("???"));

        /// <summary>
        /// Внутренний заголовок окна
        /// </summary>
        public string TitleHead
        {
            get => (string)GetValue(TitleHeadProperty);
            set => SetValue(TitleHeadProperty, value);
        }
        #endregion

        #region ExitMessage
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty ExitMessageProperty =
            DependencyProperty.Register("ExitMessage", typeof(string), typeof(DialogManipulateActionWait),
                new("???"));

        /// <summary>
        /// Конечное сообщение перед закрытием окна
        /// </summary>
        public string ExitMessage
        {
            get => (string)GetValue(ExitMessageProperty);
            set => SetValue(ExitMessageProperty, value);
        }
        #endregion

        #endregion
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

        /// <summary>
        /// Состояние активации перемещения окна по экрану
        /// </summary>
        private bool ActivateMoveWindow = false;

        /// <summary>
        /// Секундный таймер для отображения времени потраченного на сохранение
        /// </summary>
        private DispatcherTimer TimerSecond;

        /// <summary>
        /// Контроллер рандомного числа
        /// </summary>
        private Random RandomController;

        public DialogManipulateActionWait()
        {
            InitializeComponent();
            RandomController = new(DateTime.Now.Millisecond);
            Opacity = 0d;
            VisualLoading.Opacity = 0d;
            LineProgress.X1 = 3;
            TimerSecond = new()
            {
                Interval = TimeSpan.FromMilliseconds(1000d),
            };
            TimerSecond.Tick += TimerSecond_TickHandler;
            MouseLeftButtonDown += (sender, e) =>
            {
                ActivateMoveWindow = true;
                DragMove();
                ActivateMoveWindow = false;
            };
            UpdateLayout();
            Hide();
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
            TextBlockHead.Text = TitleHead;
            VisualLoading.OpenLoading();
            LineProgress.BeginAnimation(Line.X2Property, null);
            LineProgress.X2 = 3;
            Count = 1;
            TextBlockTime.Text = Count.ToString();
            TimerSecond.Start();
            Show();
            Opacity = 0d;
            UpdateLayout();
            Focus();
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, this, OpacityProperty,
                0d, 1d, TimeSpan.FromMilliseconds(1270d));
            for (int i = 0; i < StageActions.Length; i++)
            {
                SetVisualStageAction(StageActions[i].Name, (double)i / (double)StageActions.Length * 100d);
                await StageActions[i].InvokeActionSave(Dispatcher);
            }
            SetVisualStageAction(ExitMessage, 100d);
            VisualLoading.CloseLoading();
            if (ActivateMoveWindow)
            {
                TextBlockHead.Text = "!! ОТПУСТИ МЕНЯ !!";
                while (ActivateMoveWindow)
                    await Task.Delay(1000);
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
