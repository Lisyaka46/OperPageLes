using IEL.UserElementsControl;
using OperPageLes.CORE.Objects;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel.PageLabel;
using OperPageLes.UI.Windows.Dialogs;
using OPLAPI.CORE;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Browser;
using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.CORE.Browser.Base;
using OPLAPI.OIEL.UserElementsControl;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using OPRES = OperPageLes.Properties.Resources;
using Point = System.Windows.Point;

namespace OperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageManagerAppPage.xaml
    /// </summary>
    public partial class PageManagerAppPage : MainPageBrowser
    {
        /// <summary>
        /// Объект панели действий подключаемый к контенту страницы браузера OPL
        /// </summary>
        public new IELPanelAction? SourcePanelAction
        {
            get => base.SourcePanelAction;
            set
            {
                value?.EventClosingPanelAction += HandlerClosePanelAction;
                base.SourcePanelAction?.EventClosingPanelAction -= HandlerClosePanelAction;
                base.SourcePanelAction = value;
            }
        }

        /// <summary>
        /// Обработчик события закрытия панели действий
        /// </summary>
        private IELPanelAction.ClosingPanelAction HandlerClosePanelAction;

        /// <summary>
        /// Массив всех страничных приложений подключённых к начальной странице
        /// </summary>
        private List<AppPage> SourceAppPages;

        /// <summary>
        /// Массив всех страничных приложений доступный только для чтения
        /// </summary>
        internal ReadOnlyCollection<AppPage> AppPages => SourceAppPages.AsReadOnly();

        /// <summary>
        /// Объект отображения ярлыков
        /// </summary>
        private readonly StackPanel StackPanelAllLabels;

        /// <summary>
        /// Массив всех ярлыков
        /// </summary>
        private List<LabelAction> SourceLabels;

        /// <summary>
        /// Массив всех ярлыков доступный только для чтения
        /// </summary>
        internal ReadOnlyCollection<LabelAction> Labels => SourceLabels.AsReadOnly();

        #region RectangleNewPosition
        /// <summary>
        /// Квадрат отображающий выделенную позицию перемещения ярлыка
        /// </summary>
        private System.Windows.Shapes.Rectangle RectangleSelectPosition = new()
        {
            Margin = new(8d),
            Stroke = new SolidColorBrush(Colors.Black),
            RadiusX = 10,
            RadiusY = 10,
            StrokeThickness = 3,
            StrokeDashCap = PenLineCap.Round,
            StrokeDashArray = [4d, 4d, 4d],
            StrokeMiterLimit = 12,
            Height = 60,
            Width = 60,
        };

        //
        private int StartIndex = -1;

        //
        private int NextIndex = -1;

        //
        private Point PositionSourceVisualLeft;

        //
        private Point PositionSourceVisualRight;

        //
        private Point SourcePointCursor;

        //
        private readonly System.Windows.Size SizeLabel = new(60, 60);
        #endregion

        #region PageLabelActionPanel
        /// <summary>
        /// Выделенный ярлык для панели действий
        /// </summary>
        private LabelAction? _SelectLabelActionPanel = null;

        /// <summary>
        /// Выделенный ярлык для панели действий с обработкой его отсутствия
        /// </summary>
        private LabelAction SelectLabelActionPanel => _SelectLabelActionPanel ?? throw ExceptionNullableSelectLabel;

        /// <summary>
        /// Объект исключения обрабатывающий нулевое обращение к выделенному объекту ярлыка
        /// </summary>
        private static readonly Exception ExceptionNullableSelectLabel = new("Невозможно узнать выделенный объект ярлыка");

        /// <summary>
        /// Страница контекста действий над объектом ярлыка
        /// </summary>
        internal static PageLabelElementActionPanel PageLabelActionPanel = new();

        /// <summary>
        /// Состояние изменения позиционирования ярлыка
        /// </summary>
        private bool IsSelectMoveLabel = false;

        /// <summary>
        /// Активное состояние изменения позиционирования ярлыка
        /// </summary>
        private bool ActivateMoveLabel => SourceMoveLabel != null;

        /// <summary>
        /// Активный визуальный объект ярлыка кторого изменяется позиция
        /// </summary>
        private OPLVisualElementIM? SourceMoveLabel;
        #endregion

        /// <summary>
        /// Инициализировать начальную страницу
        /// </summary>
        public PageManagerAppPage() : base(new(80, 80))
        {
            SourceAppPages = [];
            SourceLabels = [];
            StackPanelAllLabels = new()
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                ClipToBounds = false,
                Margin = new(3d),
            };
            HandlerClosePanelAction = (NameFramework) =>
            {
                if (App.GUIE_Browser.ActivateMainPage)
                    Focus();
            };
            InitializeComponent();
            DefaultIconAppPage = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            MainGridContainer.Children.Add(MainPanelAllApplicationPages);
            MainPanelAllApplicationPages.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            MainPanelAllApplicationPages.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            SctollViewerLabels.ClipToBounds = false;
            SctollViewerLabels.Content = StackPanelAllLabels;
            MainGridContainer.MouseLeftButtonUp += (sender, e) =>
            {
                if (SourcePanelAction?.PanelActionActivate ?? false)
                {
                    SourcePanelAction.ClosePanelAction(IEL.CORE.Enums.PositionAnimActionPanel.CenterObject);
                }
            };

            KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.LeftCtrl:
                        IsSelectMoveLabel = true;
                        break;
                }
                e.Handled = true;
            };

            KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.LeftCtrl:
                        IsSelectMoveLabel = false;
                        if (SourceMoveLabel != null)
                            ClearVisualElementMove(SourceMoveLabel, new(Mouse.PrimaryDevice, 0));
                        break;
                }
                e.Handled = true;
            };

            #region PageLabelActionPanel
            PageLabelActionPanel.IELButtonExecuteLabel.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                await SelectLabelActionPanel.Activate();
                App.GUIE_PanelAction.ClosePanelAction();
            };
            PageLabelActionPanel.IELButtonExecuteLabel.OnActivateMouseRight += async (sender, e, Key) =>
            {
                await SelectLabelActionPanel.Activate();
            };
            PageLabelActionPanel.IELButtonChangeLabel.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                App.GUIE_PanelAction.ClosePanelAction();
                new DialogGenLabel().ChangeLabel(SelectLabelActionPanel.Label);
                SelectLabelActionPanel.UpdateVisualLabel();
            };
            PageLabelActionPanel.IELButtonRemoveLabel.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                StackPanelAllLabels.Children.Remove(SelectLabelActionPanel.VisualELement);
                SourceLabels.Remove(SelectLabelActionPanel);
                App.GUIE_PanelAction.ClosePanelAction();
                _SelectLabelActionPanel = null;
            };
            #endregion
        }

        /// <summary>
        /// Добавить новый ярлык по его данным
        /// </summary>
        /// <param name="Source">Данные ярлыка</param>
        internal void AddLabel(SourceLabelAction Source)
        {
            LabelAction Label = new(Source, SizeLabel);
            Label.VisualELement.Focusable = false;
            Label.VisualELement.Padding = new(3d);
            Label.VisualELement.VisualOrientationName = OrientationName.Up;
            Label.VisualELement.ManagerAnimation = ManagerAnimation;

            Label.VisualELement.OnActivateMouseLeft += async (sender, e) =>
            {
                await Label.Activate();
            };
            Label.VisualELement.OnActivateMouseRight += (sender, e) =>
            {
                if (!IsSelectMoveLabel)
                    SelectLabelElement(Label);
                else
                {
                    if (App.GUIE_PanelAction.PanelActionActivate)
                    {
                        App.GUIE_PanelAction.ClosePanelAction(IEL.CORE.Enums.PositionAnimActionPanel.CenterObject);
                    }
                    LabelSelectPosition(sender, e);
                }
            };


            //Label.VisualELement.MouseDown += LabelSelectPosition;

            StackPanelAllLabels.Children.Add(Label.VisualELement);
            SourceLabels.Add(Label);
        }

        /// <summary>
        /// Инициализировать все ярлыки по директории файла данных JSON
        /// </summary>
        /// <param name="PathJSON">Директория JSON</param>
        internal async Task AddLabelsFromJSON(string PathJSON)
        {
            SourceLabelAction[] Buffer = ConvertJSON.DeserializeObjectJson<SourceLabelAction>(PathJSON);
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < Buffer.Length; i++)
                {
                    AddLabel(Buffer[i]);
                    Task.Delay(100);
                }
            });
        }

        private void SelectLabelElement(LabelAction SelectLabel)
        {
            if (SourcePanelAction == null) return;
            _SelectLabelActionPanel = SelectLabel;
            SourcePanelAction.UsingPanelAction(MainGridContainer, PageLabelActionPanel,
                Orientation: IEL.CORE.Enums.OrientationPositionCursor.RightUp, DependencePointOnSize: false);
            PageLabelActionPanel.ChangeTextDescription(SelectLabel.Label.Description ?? "- Нет описания", true);
        }

        private void LabelSelectPosition(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SourceMoveLabel = (OPLVisualElementIM)sender;
            SourceMoveLabel.BeginAnimation(MarginProperty, null);
            StartIndex = StackPanelAllLabels.Children.IndexOf(SourceMoveLabel);
            NextIndex = StartIndex;
            StackPanelAllLabels.Children.Remove(SourceMoveLabel);
            StackPanelAllLabels.Children.Insert(StartIndex, RectangleSelectPosition);
            RectangleSelectPosition.Stroke = SourceMoveLabel.SourceBrushBorderBrush;

            MainGridContainer.Children.Add(SourceMoveLabel);
            SourceMoveLabel.MouseMove += MoveToPosVisualElement;
            SourceMoveLabel.MouseLeave += ClearVisualElementMove;
            SourceMoveLabel.MouseWheel += ScrollFromSelectLabel;
            if (NextIndex > 0)
            {
                PositionSourceVisualLeft = StackPanelAllLabels.Children[NextIndex - 1].TransformToAncestor(MainGridContainer)
                    .Transform(new Point(0, 0));
                //PositionSourceVisualLeft.Offset(-35, -35);
            }
            if (NextIndex < StackPanelAllLabels.Children.Count - 1)
            {
                PositionSourceVisualRight = StackPanelAllLabels.Children[NextIndex + 1].TransformToAncestor(MainGridContainer)
                    .Transform(new Point(0, 0));
                //PositionSourceVisualRight.Offset(-35, -35);
            }
            MoveToPosVisualElement(SourceMoveLabel, e);


            Canvas.SetZIndex(SourceMoveLabel, 2);
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, SourceMoveLabel, OpacityProperty,
                0.8d, TimeSpan.FromMilliseconds(1500d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RectangleSelectPosition, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(1500d));
            if (ManagerAnimation != null)
            {
                DoubleAnimation animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
                animation.From = 0d;
                animation.To = 7.5d;
                animation.Duration = TimeSpan.FromSeconds(1d);
                animation.EasingFunction = null;
                animation.RepeatBehavior = RepeatBehavior.Forever;
                RectangleSelectPosition.BeginAnimation(System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty, animation);
            }
        }

        private void ScrollFromSelectLabel(object sender, MouseWheelEventArgs e)
        {
            if (NextIndex > 0)
            {
                PositionSourceVisualLeft = StackPanelAllLabels.Children[NextIndex - 1].TransformToAncestor(MainGridContainer)
                    .Transform(new Point(0, 0));
                //PositionSourceVisualLeft.Offset(-35, -35);
            }
            if (NextIndex < StackPanelAllLabels.Children.Count - 1)
            {
                PositionSourceVisualRight = StackPanelAllLabels.Children[NextIndex + 1].TransformToAncestor(MainGridContainer)
                    .Transform(new Point(0, 0));
                //PositionSourceVisualRight.Offset(-35, -35);
            }
            MoveToPosVisualElement(sender, e);
        }

        //
        private void MoveToPosVisualElement(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SourcePointCursor = e.GetPosition(MainGridContainer);
            SourcePointCursor.Offset(-35, -35);
            ((FrameworkElement)sender).Margin = new(SourcePointCursor.X, SourcePointCursor.Y, 0, 0);
            if (NextIndex > 0 && SourcePointCursor.X < PositionSourceVisualLeft.X)
            {
                NextIndex--;
                StackPanelAllLabels.Children.Remove(RectangleSelectPosition);
                StackPanelAllLabels.Children.Insert(NextIndex, RectangleSelectPosition);
                PositionSourceVisualRight = PositionSourceVisualLeft;
                if (NextIndex > 0)
                {
                    PositionSourceVisualLeft = StackPanelAllLabels.Children[NextIndex - 1].TransformToAncestor(MainGridContainer)
                        .Transform(new Point(0, 0));
                    //PositionSourceVisualLeft.Offset(-35, -35);
                }
            }
            else if (NextIndex < StackPanelAllLabels.Children.Count - 1 && SourcePointCursor.X > PositionSourceVisualRight.X)
            {
                NextIndex++;
                StackPanelAllLabels.Children.Remove(RectangleSelectPosition);
                StackPanelAllLabels.Children.Insert(NextIndex, RectangleSelectPosition);
                PositionSourceVisualLeft = PositionSourceVisualRight;
                if (NextIndex < StackPanelAllLabels.Children.Count - 1)
                {
                    PositionSourceVisualRight = StackPanelAllLabels.Children[NextIndex + 1].TransformToAncestor(MainGridContainer)
                        .Transform(new Point(0, 0));
                    //PositionSourceVisualRight.Offset(-35, -35);
                }
            }
        }

        //
        private void ClearVisualElementMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            OPLVisualElementIM SourceVisual = (OPLVisualElementIM)sender;
            if (!SourceMoveLabel?.Equals(SourceVisual) ?? true) return;
            SourceMoveLabel = null;
            SourceVisual.MouseMove -= MoveToPosVisualElement;
            SourceVisual.MouseLeave -= ClearVisualElementMove;
            SourceVisual.MouseWheel -= ScrollFromSelectLabel;
            MainGridContainer.Children.Remove(SourceVisual);
            StackPanelAllLabels.Children.Remove(RectangleSelectPosition);
            StackPanelAllLabels.Children.Insert(NextIndex, SourceVisual);
            (SourceLabels[StartIndex], SourceLabels[NextIndex]) = (SourceLabels[NextIndex], SourceLabels[StartIndex]);
            Canvas.SetZIndex(SourceVisual, 0);
            SourceVisual.Margin = new(3d);
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, SourceVisual, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(600d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, RectangleSelectPosition, OpacityProperty,
                0d, TimeSpan.FromMilliseconds(600d));
            RectangleSelectPosition.BeginAnimation(System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty, null);
        }
    }
}
