using IEL.CORE.Classes;
using IEL.UserElementsControl;
using OIEL.CORE.Browser;
using OIEL.UserElementsControl;
using OperPageLes.CORE.Objects;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel.PageLabel;
using OperPageLes.UI.Windows.Dialogs;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using OPRES = OperPageLes.Properties.Resources;
using Point = System.Windows.Point;

namespace OperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageManagerAppPage.xaml
    /// </summary>
    public partial class PageManagerAppPage : Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        [LibraryImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        protected static partial bool SetCursorPos(int X, int Y);

        /// <summary>
        /// Массив всех страничных приложений подключённых к начальной странице
        /// </summary>
        private List<ApplicationPage> SourceAppPages;

        /// <summary>
        /// Массив всех страничных приложений доступный только для чтения
        /// </summary>
        internal ReadOnlyCollection<ApplicationPage> AppPages => SourceAppPages.AsReadOnly();

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
        public PageManagerAppPage()
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
            InitializeComponent();
            SctollViewerLabels.ClipToBounds = false;
            SctollViewerLabels.ClipToBoundsContainer = false;
            SctollViewerLabels.Content = StackPanelAllLabels;
            MainGridContainer.MouseLeftButtonUp += (sender, e) =>
            {
                if (App.MainWindow.IELActionPanelMain.PanelActionActivate)
                {
                    App.MainWindow.IELActionPanelMain.ClosePanelAction(IEL.CORE.Enums.PositionAnimActionPanel.CenterObject);
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
            };

            #region PageLabelActionPanel
            PageLabelActionPanel.IELButtonExecuteLabel.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                await SelectLabelActionPanel.Activate();
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
            };
            PageLabelActionPanel.IELButtonExecuteLabel.OnActivateMouseRight += async (sender, e, Key) =>
            {
                await SelectLabelActionPanel.Activate();
            };
            PageLabelActionPanel.IELButtonChangeLabel.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
                new DialogGenLabel().ChangeLabel(SelectLabelActionPanel.Label);
                SelectLabelActionPanel.UpdateVisualLabel();
            };
            PageLabelActionPanel.IELButtonRemoveLabel.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                StackPanelAllLabels.Children.Remove(SelectLabelActionPanel.VisualELement);
                SourceLabels.Remove(SelectLabelActionPanel);
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
                _SelectLabelActionPanel = null;
            };
            #endregion

            Loaded += (sender, e) =>
            {
                App.MainWindow.IELActionPanelMain.EventClosingPanelAction += (NameFramework) =>
                {
                    if (App.CurrentApp.MainBrowser.ActivateManagerPage)
                        Focus();
                };
            };
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
                    if (App.MainWindow.IELActionPanelMain.PanelActionActivate)
                    {
                        App.MainWindow.IELActionPanelMain.ClosePanelAction(IEL.CORE.Enums.PositionAnimActionPanel.CenterObject);
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
            SourceLabelAction[] Buffer = StructDirectoryResources.DeserializeObjectJson<SourceLabelAction>(PathJSON);
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
            _SelectLabelActionPanel = SelectLabel;
            App.MainWindow.IELActionPanelMain.UsingPanelAction(MainGridContainer, PageLabelActionPanel,
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
            RectangleSelectPosition.Stroke = SourceMoveLabel.SourceBorderBrush.SourceBrush;

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
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(SourceMoveLabel, OpacityProperty,
                0.8d, TimeSpan.FromMilliseconds(1500d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleSelectPosition, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(1500d));
            DoubleAnimation animation = App.ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
            animation.From = 0d;
            animation.To = 7.5d;
            animation.Duration = TimeSpan.FromSeconds(1d);
            animation.EasingFunction = null;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            RectangleSelectPosition.BeginAnimation(System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty, animation);
        }

        private void ScrollFromSelectLabel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                SctollViewerLabels.ScrollToHorizontalRight();
            else
                SctollViewerLabels.ScrollToHorizontalLeft();
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
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(SourceVisual, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(600d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleSelectPosition, OpacityProperty,
                0d, TimeSpan.FromMilliseconds(600d));
            RectangleSelectPosition.BeginAnimation(System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty, null);
        }

        /// <summary>
        /// Добавить отображение иконки в менеджере приложений страниц
        /// </summary>
        /// <param name="TypeAppPage">Тип создаваемого приложения страницы</param>
        internal void AddNewAppPage(Type TypeAppPage, string NameAppPage, PaletteSpectrum? Spectrum = null, ImageSource? Icon = null)
        {
            ApplicationPage Source = new(TypeAppPage, NameAppPage, new(100, 100));
            Source.VisualELement.PaletteElement = Spectrum ?? App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Gray];
            Source.VisualELement.Source = Icon ?? StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            Source.ApplicationPageActivate += Source_ApplicationPageActivate;
            MainPanel.Children.Add(Source.VisualELement);
        }

        private void Source_ApplicationPageActivate(object? sender, ApplicationPage e)
        {
            PageBrowser? InicializeInlay = App.CurrentApp.MainBrowser.SearchAnyPageType(e.TypeBrowserAppPage);
            if (InicializeInlay != null)
                App.CurrentApp.MainBrowser.ActivateInlayInBrowserPage(InicializeInlay);
            else InitAppPageFromType(in e);
        }

        /// <summary>
        /// Инициализировать страницу по хранимому типу в иконке
        /// </summary>
        /// <param name="Browser">Браузер страниц</param>
        /// <param name="UIAppPage">Иконка хранимого типа приложения страницы</param>
        /// <param name="Activate">Активировать созданную вкладку или нет</param>
        private static void InitAppPageFromType(in ApplicationPage AppPage)
        {
            PageBrowser ElementAppPage = (PageBrowser)(Activator.CreateInstance(AppPage.TypeBrowserAppPage) ??
                throw new Exception("Не удалось создать объект приложения страницы"));
            ElementAppPage.Title = AppPage.Name;
            IELButtonImage CloseButtonInlay = App.CurrentApp.MainBrowser.AddInlayPage(in ElementAppPage, AppPage.VisualELement.PaletteElement, true).GetButtonCloseInlay();
            CloseButtonInlay.MarginViewBox = new(0);
            CloseButtonInlay.PaletteElement = App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Red];
            CloseButtonInlay.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
        }
    }
}
