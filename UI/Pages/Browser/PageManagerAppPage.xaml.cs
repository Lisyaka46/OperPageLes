using IEL.CORE.Classes;
using IEL.UserElementsControl;
using OIEL.CORE.Browser;
using OIEL.UserElementsControl;
using OperPageLes.CORE.Objects;
using OperPageLes.CORE.Struct;
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
        private bool ActivateMoveLabel = false;

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
        }

        /// <summary>
        /// Добавить новый ярлык по его данным
        /// </summary>
        /// <param name="Source">Данные ярлыка</param>
        internal void AddLabel(SourceLabelAction Source)
        {
            LabelAction Label = new(Source, SizeLabel);
            Label.VisualELement.Padding = new(3d);
            Label.VisualELement.VisualOrientationName = OrientationName.Up;

            Label.VisualELement.OnActivateMouseLeft += async (sender, e) =>
            {
                await Label.Activate();
            };
            Label.VisualELement.MouseRightButtonDown += LabelSelectPosition;

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

        private void LabelSelectPosition(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OPLVisualElementIM SourceVisual = (OPLVisualElementIM)sender;
            SourceVisual.BeginAnimation(MarginProperty, null);
            ActivateMoveLabel = true;
            StartIndex = StackPanelAllLabels.Children.IndexOf(SourceVisual);
            NextIndex = StartIndex;
            StackPanelAllLabels.Children.Remove(SourceVisual);
            StackPanelAllLabels.Children.Insert(StartIndex, RectangleSelectPosition);
            RectangleSelectPosition.Stroke = SourceVisual.SourceBorderBrush.SourceBrush;

            MainGridContainer.Children.Add(SourceVisual);
            SourceVisual.MouseRightButtonDown -= LabelSelectPosition;
            SourceVisual.MouseRightButtonUp += ClearVisualElementMove;
            SourceVisual.MouseMove += MoveToPosVisualElement;
            SourceVisual.MouseLeave += ClearVisualElementMove;
            SourceVisual.MouseWheel += ScrollFromSelectLabel;
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
            MoveToPosVisualElement(SourceVisual, e);


            Canvas.SetZIndex(SourceVisual, 2);
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(SourceVisual, OpacityProperty,
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
            ActivateMoveLabel = false;
            SourceVisual.MouseRightButtonDown += LabelSelectPosition;
            SourceVisual.MouseMove -= MoveToPosVisualElement;
            SourceVisual.MouseRightButtonUp -= ClearVisualElementMove;
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
            object? InicializeInlay = App.CurrentApp.MainBrowser.SearchAnyPageType(e.TypeBrowserAppPage);
            if (InicializeInlay != null)
                App.CurrentApp.MainBrowser.ActivateInlayInBrowserPage((PageBrowser)InicializeInlay);
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
            CloseButtonInlay.PaletteElement = App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Red];
            CloseButtonInlay.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
        }
    }
}
