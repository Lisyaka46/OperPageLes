using OIEL.CORE.Browser;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Windows;
using OperPageLes.UI.Windows.DEV;
using System.Reflection;
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
using OPRES = OperPageLes.Properties.Resources;
using Point = System.Windows.Point;

namespace OperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : PageBrowser
    {
        private Point StartPositionMouse;
        private bool Activate = false;
        Storyboard myStoryboard = new();
        private DoubleAnimation anim = App.ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
        private Vector3DAnimation Vector3DAnim = new()
        {
            From = null,
            To = null,
            Duration = TimeSpan.FromMilliseconds(500d),
            EasingFunction = new CircleEase()
            {
                EasingMode = EasingMode.EaseInOut,
            }
        };

        public PageDeveloper()
        {
            InitializeComponent();

            //Loaded += async (sender, e) =>
            //{
            //    const int CountParticles = 90, DelayOneParticle = 100;
            //    TimeSpan TimeParticle = TimeSpan.FromMilliseconds(DelayOneParticle * 8);
            //    Random randomColor = new();
            //    //IELObjectBase Particle;
            //    FrameworkElement Particle;
            //    int i = 0;
            //    for (int K = 0; K < CountParticles; K++)
            //    {
            //        DoubleAnimation animationD = new()
            //        {
            //            Duration = TimeParticle,
            //            From = 1d,
            //            To = 0d
            //        };
            //        DoubleAnimation animationW = new()
            //        {
            //            Duration = TimeParticle,
            //            From = 15d,
            //            To = 60d
            //        };
            //        DoubleAnimation animationH = new()
            //        {
            //            Duration = TimeParticle,
            //            From = 15d,
            //            To = 60d
            //        };
            //        Particle = new System.Windows.Shapes.Rectangle()
            //        {
            //            Height = 4,
            //            Width = 4,
            //            Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
            //                (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255))),
            //            Opacity = 0d,
            //            RadiusX = 1,
            //            RadiusY = 1,
            //        };
            //        //Particle = new IELBlockInfoText()
            //        //{
            //        //    Width = 20,
            //        //    Height = 20,
            //        //    Opacity = 0d,
            //        //    CornerRadius = new(3),
            //        //    Text = "F",
            //        //};
            //        //Particle = new System.Windows.Controls.Image()
            //        //{
            //        //    Height = 15,
            //        //    Width = 15,
            //        //    //Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
            //        //    //    (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255))),
            //        //    Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication)),
            //        //    Opacity = 0d
            //        //};
            //        //App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Gray].ConnectPalleteFromIELElement(Particle);
            //        Thickness Start = new(
            //            Particles.ActualWidth / 2 - Particle.Width / 2,
            //            Particles.ActualHeight / 2 - Particle.Height / 2, 0, 0);

            //        Particle.Margin = Start;
            //        Particles.Children.Add(Particle);

            //        ThicknessAnimation animationT = App.ThicknessAnimationType.SourceAnimation.Clone();
            //        animationT.Duration = TimeParticle;
            //        animationT.From = Start;
            //        RepeatBoard(animationT, randomColor, Start);

            //        animationT.FillBehavior = FillBehavior.Stop;
            //        animationT.Completed += (sender, e) =>
            //        {
            //            Particles.Children[i].BeginAnimation(MarginProperty, null);
            //            Particles.Children[i].BeginAnimation(OpacityProperty, null);
            //            //Particles.Children[i].BeginAnimation(WidthProperty, null);
            //            //Particles.Children[i].BeginAnimation(HeightProperty, null);

            //            RepeatBoard(animationT, randomColor, Start);

            //            Particles.Children[i].BeginAnimation(MarginProperty, animationT);
            //            Particles.Children[i].BeginAnimation(OpacityProperty, animationD);
            //            //Particles.Children[i].BeginAnimation(WidthProperty, animationW);
            //            //Particles.Children[i].BeginAnimation(HeightProperty, animationH);
            //            i = ++i % Particles.Children.Count;
            //        };

            //        Particle.BeginAnimation(MarginProperty, animationT);
            //        Particle.BeginAnimation(OpacityProperty, animationD);
            //        //Particle.BeginAnimation(WidthProperty, animationW);
            //        //Particle.BeginAnimation(HeightProperty, animationH);
            //        await Task.Delay(DelayOneParticle);
            //    }
            //};
            anim.From = null;
            anim.Duration = TimeSpan.FromMilliseconds(3000d);
            myStoryboard.Children.Add(anim);
            ImageBrushModel.ImageSource = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.VECTOR));
            MyAnimatedObject.MouseEnter += (sender, e) =>
            {
                myAngleRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
                Point CurrentPosModel = Model.TransformToAncestor((Visual)App.Current.MainWindow).TransformBounds(Model.Content.Bounds).Location;
                StartPositionMouse = new(
                    CurrentPosModel.X + Model.Content.Bounds.SizeX * myPerspectiveCamera.FieldOfView,
                    CurrentPosModel.Y + Model.Content.Bounds.SizeY * myPerspectiveCamera.FieldOfView);
                //StartPositionMouse = Mouse.GetPosition(App.Current.MainWindow);
                Activate = true;
                GetAngleModelRotate();
            };
            MyAnimatedObject.MouseLeave += (sender, e) =>
            {
                if (!Activate) return;
                Activate = false;
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(myAngleRotation, AxisAngleRotation3D.AngleProperty, 0d, TimeSpan.FromMilliseconds(500d));
            };
            MyAnimatedObject.MouseMove += (sender, e) =>
            {
                if (!Activate) return;
                GetAngleModelRotate();
                //myAngleRotation.Angle = GetAngleModelRotate();
            };
            WindowGenerateQdata.Click += (sender, e) =>
            {
                WindowQDataViewer window = new();
                window.Show();
            };
        }

        //
        private void GetAngleModelRotate()
        {
            Point CurrentPos = Mouse.GetPosition(App.Current.MainWindow);
            double X = StartPositionMouse.X - CurrentPos.X, Y = StartPositionMouse.Y - CurrentPos.Y;
            X /= Model.Content.Bounds.SizeX * myPerspectiveCamera.FieldOfView;
            Y /= Model.Content.Bounds.SizeY * myPerspectiveCamera.FieldOfView;
            myAngleRotation.Axis = new(-Y, -X, 0);
            myAngleRotation.Angle = 15 * (Math.Abs(X) + Math.Abs(Y)) / 2;
            Element.Text = $"Axis: ({Math.Round(-X, 2)}~2 | {Math.Round(-Y, 2)}~2 | 0~)   Angle: {Math.Round(myAngleRotation.Angle, 2)}~2/2";
        }

        /// <summary>
        /// Добавить новый элемент текста в стек визуализации
        /// </summary>
        /// <param name="TextElement">Отображаемый текст в элементе</param>
        /// <returns>Элемент цвета визуализации</returns>
        internal TextBlock AddNewStackTextBlock(string TextElement)
        {
            TextBlock Element = new()
            {
                Text = TextElement,
                FontSize = 16d,
                Foreground = new SolidColorBrush(Colors.Black),
            };
            StackPanelElementsVisual.Children.Add(Element);
            return Element;
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
