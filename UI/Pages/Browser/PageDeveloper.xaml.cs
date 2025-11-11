//using GLGraphs.CartesianGraph;
//using GLGraphs.Wpf;
//using OpenTK.Graphics.OpenGL;
//using OpenTK.Graphics.OpenGL4;
//using OpenTK.Mathematics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrColor = System.Drawing.Color;

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
                            GenImage((int)SliderX.Value, (int)SliderY.Value));
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
            IELButtonTest.MouseLeftButtonUp += (sender, e) =>
            {
                //SeriesSource?.Add("Point", SeriesSource.Points.Count, (float)Math.Cos(SeriesSource.Points.Count));
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
            #region Sliders
            SliderX.ValueChanged += (sender, e) =>
            {
                TextBlockX_Value.Text = $"X:{Math.Round(e.NewValue, 2)}";
            };
            SliderY.ValueChanged += (sender, e) =>
            {
                TextBlockY_Value.Text = $"Y:{Math.Round(e.NewValue, 2)}";
            };
            #endregion
        }

        //
        internal System.Drawing.Color SetColorFunction(int X, int Y)
        {
            return System.Drawing.Color.FromArgb(
                (byte)(Math.Cos(X / SliderX.Value) * 255),
                (byte)(Math.Cos(Y / SliderY.Value) * 255),
                (byte)(Math.Sin(X / SliderY.Value) * 255));
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
                    Dispatcher.Invoke(() => TextblockInformation.Text = $"X:{X} || Y:{Y}");
                }
            }
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                   IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
