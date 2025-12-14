using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.Windows;
using ApplicationOperPageLes.UI.Windows.DEV;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using DrColor = System.Drawing.Color;
using OPRES = ApplicationOperPageLes.Properties.Resources;
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

        private Point StartPositionMouse;
        private bool Activate = false;

        public PageDeveloper()
        {
            InitializeComponent();

            Loaded += async (sender, e) =>
            {
                const int CountParticles = 30, DelayOneParticle = 100;
                TimeSpan TimeParticle = TimeSpan.FromMilliseconds(DelayOneParticle * 8);
                Random randomColor = new();
                //IELObjectBase Particle;
                FrameworkElement Particle;
                int i = 0;
                for (int K = 0; K < CountParticles; K++)
                {
                    DoubleAnimation animationD = new()
                    {
                        Duration = TimeParticle,
                        From = 1d,
                        To = 0d
                    };
                    DoubleAnimation animationW = new()
                    {
                        Duration = TimeParticle,
                        From = 15d,
                        To = 60d
                    };
                    DoubleAnimation animationH = new()
                    {
                        Duration = TimeParticle,
                        From = 15d,
                        To = 60d
                    };
                    //Particle = new System.Windows.Shapes.Rectangle()
                    //{
                    //    Height = 3,
                    //    Width = 3,
                    //    Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                    //        (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255))),
                    //    Opacity = 0d
                    //};
                    //Particle = new IELBlockInfoText()
                    //{
                    //    Width = 20,
                    //    Height = 20,
                    //    Opacity = 0d,
                    //    CornerRadius = new(3),
                    //    Text = "F",
                    //};
                    Particle = new System.Windows.Controls.Image()
                    {
                        Height = 15,
                        Width = 15,
                        //Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                        //    (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255))),
                        Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication)),
                        Opacity = 0d
                    };
                    //App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Gray].ConnectPalleteFromIELElement(Particle);
                    Thickness Start = new(
                        Particles.ActualWidth / 2 - Particle.Width / 2,
                        Particles.ActualHeight / 2 - Particle.Height / 2, 0, 0);

                    Particle.Margin = Start;
                    Particles.Children.Add(Particle);

                    ThicknessAnimation animationT = App.ThicknessAnimationType.SourceAnimation.Clone();
                    animationT.Duration = TimeParticle;
                    animationT.From = Start;
                    RepeatBoard(animationT, randomColor, Start);

                    animationT.FillBehavior = FillBehavior.Stop;
                    animationT.Completed += (sender, e) =>
                    {
                        Particles.Children[i].BeginAnimation(MarginProperty, null);
                        Particles.Children[i].BeginAnimation(OpacityProperty, null);
                        Particles.Children[i].BeginAnimation(WidthProperty, null);
                        Particles.Children[i].BeginAnimation(HeightProperty, null);

                        RepeatBoard(animationT, randomColor, Start);

                        Particles.Children[i].BeginAnimation(MarginProperty, animationT);
                        Particles.Children[i].BeginAnimation(OpacityProperty, animationD);
                        Particles.Children[i].BeginAnimation(WidthProperty, animationW);
                        Particles.Children[i].BeginAnimation(HeightProperty, animationH);
                        i = ++i % Particles.Children.Count;
                    };

                    Particle.BeginAnimation(MarginProperty, animationT);
                    Particle.BeginAnimation(OpacityProperty, animationD);
                    Particle.BeginAnimation(WidthProperty, animationW);
                    Particle.BeginAnimation(HeightProperty, animationH);
                    await Task.Delay(DelayOneParticle);
                }
                i = 0;
            };

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



            IELButtonDownloadImage.OnActivateMouseLeft += (sender, e) =>
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
            IELButtonGenerateImage.OnActivateMouseLeft += (sender, e) =>
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
                WindowThemeController y = new();
                y.Show();
            };
            IELButtonTest.MouseLeftButtonUp += (sender, e) =>
            {
                WindowQDataViewer y = new();
                y.Show();
                App.MainWindow.BlurMainAnimateColor(Colors.Blue);
            };
            IELButtonTest.MouseRightButtonUp += (sender, e) =>
            {
            };
        }

        //
        private void RepeatBoard(ThicknessAnimation Board, Random random, Thickness Start)
        {
            int W, H;
            W = random.Next(-((int)Particles.ActualWidth / 3), (int)Particles.ActualWidth / 3);
            H = random.Next(-Math.Abs(W), Math.Abs(W));
            Board.To = new(Start.Left + W, Start.Top + H, 0, 0);
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
