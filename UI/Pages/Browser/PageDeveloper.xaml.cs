//using GLGraphs.CartesianGraph;
//using GLGraphs.Wpf;
//using OpenTK.Graphics.OpenGL;
//using OpenTK.Graphics.OpenGL4;
//using OpenTK.Mathematics;
using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.UI.Windows;
using ApplicationOperPageLes.UI.Windows.DEV;
using ApplicationOperPageLes.UI.Windows.Dialogs;
using IEL.GUI;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using DrColor = System.Drawing.Color;
using Point = System.Windows.Point;

namespace ApplicationOperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : System.Windows.Controls.Page
    {
        private bool ScaleMode = false;
        private bool KeyEventActivate = false;
        //private GraphSeries<string>? SeriesSource;

        private Point StartPositionMouse;
        private bool Activate = false;

        public PageDeveloper()
        {
            InitializeComponent();

            //var GLGraphControl = new GLCartesianGraphControl<string>();
            //GLGraphControl.Height = 270;
            //GLGraphControl.Loaded += (sender, e) =>
            //{
            //    CartesianGraphSettings Settings = new()
            //    {
            //        PointSize = 60f,
            //        TextScale = 1.6f,
            //        BackgroundColor = new(20, 20, 20, 255),
            //        SelectionMode = GraphSelectionMode.Click,
            //        LineSize = 10f,
            //        ForceSquareGrid = false,
            //        SelectedCol = new(230, 200, 130, 200),

            //    };
            //    GLGraphControl.Graph = new(Settings);
            //    //GLGraphControl.Graph.State.AddRegion(new(-1.5f, -1.5f, 1.5f, 1.5f), new(230, 130, 30, 200));
            //    GLGraphControl.Graph.State.XGridSpacing.Major = 0.4f;
            //    GLGraphControl.Graph.State.XGridSpacing.Minor = 0.2f;
            //    GLGraphControl.Graph.State.XGridSpacing.Automatic = false;
            //    GLGraphControl.Graph.State.YGridSpacing.Major = 0.4f;
            //    GLGraphControl.Graph.State.YGridSpacing.Minor = 0.2f;
            //    GLGraphControl.Graph.State.YGridSpacing.Automatic = false;
            //    GLGraphControl.Graph.State.IsCameraAutoControlled = false;
            //    //GLGraphControl.Graph.State.

            //    GLGraphControl.UseLayoutRounding = false;
            //    GLGraphControl.Graph.State.Camera.VerticalSizeDampeningFactor = 0.1f;
            //    GLGraphControl.Graph.State.Camera.PositionDampeningFactor = 0.16f;
            //    //GLGraphControl.Graph.State.AutoMajorGridDivisions = 1;
            //    //GLGraphControl.Graph.State.AutoMinorGridDivisions = 1;
            //    //GLGraphControl.
            //    //GLGraphControl.Graph.State.Bounds = new(-2, -2, 2, 2);
            //    SeriesSource = GLGraphControl.Graph.State.AddSeries(SeriesType.Point, "x^2");

            //    SeriesSource.PointShape = SeriesPointShape.Triangle;
            //    SeriesSource.IsVisible = true;
            //    SeriesSource.Color = new(230, 100, 30, 200);
            //};
            //GridMain.Children.Add(GLGraphControl);
            //Grid.SetRow(GLGraphControl, 1);

            //BorderImage.RenderTransform = new TransformGroup() { Children = [ScaleBorderTransform, SkewBorderTransform] };
            //App.DoubleAnimationType.AnimateEffect(ScaleBorderTransform, ScaleTransform.ScaleXProperty, 0.5d, 1d, TimeSpan.FromMilliseconds(500d));
            //App.DoubleAnimationType.AnimateEffect(ScaleBorderTransform, ScaleTransform.ScaleYProperty, 0.9d, 1d, TimeSpan.FromMilliseconds(500d));

            //App.DoubleAnimationType.AnimateEffect(SkewBorderTransform, SkewTransform.AngleXProperty, 20d, 0d, TimeSpan.FromMilliseconds(500d));
            //App.DoubleAnimationType.AnimateEffect(SkewBorderTransform, SkewTransform.AngleYProperty, -12, 0d, TimeSpan.FromMilliseconds(500d));

            MyAnimatedObject.MouseDown += (sender, e) =>
            {
                myAngleRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
                StartPositionMouse = Mouse.GetPosition(App.Current.MainWindow);
                Activate = true;
                myAngleRotation.Angle = 0d;
            };
            MyAnimatedObject.MouseLeave += (sender, e) =>
            {
                if (!Activate) return;
                Activate = false;
                App.DoubleAnimationType.AnimateEffect(myAngleRotation, AxisAngleRotation3D.AngleProperty, 0d, TimeSpan.FromMilliseconds(500d));
            };
            MyAnimatedObject.MouseMove += (sender, e) =>
            {
                if (!Activate) return;
                const double d = 1.6d;
                Point CurrentPos = Mouse.GetPosition(App.Current.MainWindow);
                double X = StartPositionMouse.X - CurrentPos.X, Y = StartPositionMouse.Y - CurrentPos.Y;
                X /= d; Y /= d;
                double XY = Math.Abs(X) + Math.Abs(Y);
                XY /= d;
                if (Math.Abs(X) < 500)
                {
                    myAngleRotation.Axis = new(myAngleRotation.Axis.X, -X, 0);
                }
                if (Math.Abs(Y) < 500)
                {
                    myAngleRotation.Axis = new(-Y, myAngleRotation.Axis.Y, 0);
                }
                myAngleRotation.Angle = XY < 25 ? XY : 25;
            };



            IELButtonDownloadImage.OnActivateMouseLeft += (sender, e, Key) =>
            {
            };
            GridScale.MouseWheel += (sender, e) =>
            {
                if (ScaleMode)
                {
                    double UpdateScale = e.Delta > 0 ? -0.23d : 0.23d;
                    ScaleTransformBox.ScaleX += UpdateScale;
                    ScaleTransformBox.ScaleY += UpdateScale;
                }
            };
            Map.KeyDown += (sender, e) =>
            {
                if (KeyEventActivate) return;
                KeyEventActivate = true;
                switch (e.Key)
                {
                    case System.Windows.Input.Key.LeftCtrl:
                        ScaleMode = true;
                        break;
                }
                e.Handled = true;
            };
            Map.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.LeftCtrl:
                        ScaleMode = false;
                        break;
                }
                KeyEventActivate = false;
                e.Handled = true;
            };
            IELButtonGenerateImage.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Thread t = new(() =>
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, async () =>
                    {
                        ImageMap.Source = await App.MainWindow.ExecuteVisualizateLoadingProcess("Генерация изображения",
                            GenImage(100, 100));
                    });
                })
                {
                    IsBackground = true,
                };
                t.SetApartmentState(ApartmentState.STA);
                t.Priority = ThreadPriority.Highest;
                t.Start();
            };
            IELButtonTest.Focusable = true;
            IELButtonTest.MouseRightButtonUp += (sender, e) =>
            {
                WindowPaletteController y = new();
                y.Show();
            };
            IELButtonTest.MouseLeftButtonUp += (sender, e) =>
            {
                WindowQDataViewer y = new();
                y.Show();
                //App.CurrentApp.SettingPaletteApplication.SourcePalette.GetQdataFromEnum(PaletteValuesEnum.BG_Red).SetFromSpectrumData(
                //    IEL.CORE.Classes.QData.EnumDataSpectrum.Default, 255, 255, 255, 255);
                //IEL.CORE.Classes.QData[] Data =
                //    [
                //        new(
                //        [
                //            [255, 255, 255, 255],
                //            [0, 10, 10, 0],
                //            [10, 10, 100, 100],
                //            [100, 100, 10, 10]
                //        ]),
                //        new(
                //        [
                //            [90, 90, 90, 90],
                //            [4, 3, 2, 1],
                //            [10, 20, 100, 100],
                //            [200, 100, 20, 10]
                //        ]),
                //    ];
                ////byte[] A = Data.Data[0];
                ////byte[] R = Data.Data[1];
                ////byte[] G = Data.Data[2];
                ////byte[] B = Data.Data[3];
                ////IList<byte> d = [.. Data.Data[0]];
                //IEnumerable<IEnumerable<IList<byte>>> d = Enumerable.Range(0, Data.Length).Select(
                //    (i) => Enumerable.Range(0, 4).Select(
                //        (j) => (IList<byte>)Data[i].Data[j]));
                //string SettingProcessJSON = JsonConvert.SerializeObject(d, Formatting.Indented);
                //File.WriteAllText($"C:/Users/killm/Рабочий стол/0/QData_{++c}.json", SettingProcessJSON);
                ////SeriesSource?.Add("Point", SeriesSource.Points.Count, (float)Math.Cos(SeriesSource.Points.Count));

                //IEnumerable<byte[][]> t = JsonConvert.DeserializeObject<IEnumerable<byte[][]>>(SettingProcessJSON) ??
                //    throw new Exception("Не удалось преобразовать JSON в управляемый объект QData");
                //foreach (byte[][] data in t)
                //{
                //    IEL.CORE.Classes.QData JsonData = new(data);
                //    _ = t;
                //}
                App.MainWindow.BlurMainAnimateColor(Colors.Blue);
            };
            IELButtonTest.MouseRightButtonUp += (sender, e) =>
            {
                //App.ColorAnimationType.AnimateEffect(App.MainWindow.ShadowMainBorderEffect,
                //    DropShadowEffect.ColorProperty, Colors.Black, TimeSpan.FromMilliseconds(300d));
                //App.DoubleAnimationType.AnimateEffect(App.MainWindow.ShadowMainBorderEffect,
                //    DropShadowEffect.BlurRadiusProperty, 28, 18, TimeSpan.FromMilliseconds(300d));
                //// Create a storyboard to apply the animation.
                //ThicknessAnimation animation = App.ThicknessAnimationType.SourceAnimation.Clone();
                //animation.Duration = TimeSpan.FromMilliseconds(300d);
                //animation.From = new(25);
                //animation.To = new(15);
                //Storyboard.SetTargetName(animation, "BorderWindowMain");
                //Storyboard.SetTargetProperty(
                //    animation, new PropertyPath(Border.BorderThicknessProperty));
                //Storyboard ellipseStoryboard = new();
                //ellipseStoryboard.Children.Add(animation);
                //ellipseStoryboard.Begin(App.MainWindow);
            };
        }

        /// <summary>
        /// Сгенерировать изображение по формуле
        /// </summary>
        /// <param name="Width">Ширина изображения</param>
        /// <param name="Height">Высота изображения</param>
        /// <returns>Объект карты цвета изображения</returns>
        internal async Task<BitmapSource> GenImage(int Width, int Height)
        {
            int X, Y;
            Bitmap bitmap = new(Width, Height);
            Func<int, int, DrColor> d = new((X, Y) =>
            {
                byte R = (byte)(Math.Cos(Width / 20d - X) * 255);
                byte G = (byte)(Math.Cos(Height / 20d - Y) * 255);
                byte B = (byte)(0);
                return DrColor.FromArgb(R, G, B);
            });
            for (Y = 0; Y < Height; Y++)
            {
                for (X = 0; X < Width; X++)
                {
                    await Task.Run(() => 
                    bitmap.SetPixel(X, Y, d.Invoke(X, Y)));
                    //Dispatcher.Invoke(() => TextblockInformation.Text = $"X:{X} || Y:{Y}");
                }
            }
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                   IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
