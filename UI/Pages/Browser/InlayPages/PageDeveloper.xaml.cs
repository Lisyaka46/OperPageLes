using OperPageLes.CORE.Struct;
using OperPageLes.UI.UserElementsControl.Default;
using OperPageLes.UI.Windows.DEV;
using OPLAPI.CORE.Animation;
using OPLAPI.OIEL.CORE.Browser;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using DrColor = System.Drawing.Color;
using OPRES = OperPageLes.Properties.Resources;
using Point = System.Windows.Point;

namespace OperPageLes.UI.Pages.Browser.InlayPages
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : PageBrowser
    {
        private Point StartPositionMouse;
        private bool Activate = false;
        Storyboard myStoryboard = new();
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

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public override OPLAnimationManager? ManagerAnimation
        {
            get => base.ManagerAnimation;
            set
            {
                base.ManagerAnimation = value;
                Check.ManagerAnimation = value;
                Reel1.ManagerAnimation = value;
                Reel2.ManagerAnimation = value;
                Reel3.ManagerAnimation = value;
                Reel4.ManagerAnimation = value;
                Reel5.ManagerAnimation = value;
                Reel6.ManagerAnimation = value;
            }
        }

        public PageDeveloper()
        {
            InitializeComponent();
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.ValidKeyIcon));
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
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, myAngleRotation, AxisAngleRotation3D.AngleProperty,
                    0d, TimeSpan.FromMilliseconds(500d));
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

            WindowCheckIcons.Click += (sender, e) =>
            {
                WindowTestIcons window = new();
                window.Show();
            };

            IELButtonSpin.OnActivateMouseLeft += async (sender, e) => await SpinButton_Click(sender, e);

            AddSymbolsReel(Reel1);
            AddSymbolsReel(Reel2);
            AddSymbolsReel(Reel3);
            AddSymbolsReel(Reel4);
            AddSymbolsReel(Reel5);
            AddSymbolsReel(Reel6);
        }

        private void AddSymbolsReel(OPLReel SourceReel)
        {
            TextBlock SourceSymbol;
            SourceReel.AddSymbol("🍒");
            SourceReel.AddSymbol("🍉");
            SourceSymbol = SourceReel.AddSymbol("⭐");
            SourceSymbol.FontSize = 35d;
            SourceSymbol.Padding = new(0d, 0d, 0d, 4d);
            SourceSymbol = SourceReel.AddSymbol("💎");
            SourceSymbol.Padding = new(0d, 0d, 0d, 4d);
            SourceSymbol = SourceReel.AddSymbol("🔔");
            SourceSymbol = SourceReel.AddSymbol("🍋");
            SourceSymbol = SourceReel.AddSymbol("7️⃣");
            SourceSymbol.Padding = new(0d, 0d, 0d, 4d);
        }

        private async Task SpinButton_Click(object sender, MouseButtonEventArgs e)
        {
            IELButtonSpin.IsEnabled = false;
            Random _random = new();
            Task TR1 = Reel1.SpinAsync(_random.Next(Reel1.SymbolsCount));
            await Task.Delay(100);
            Task TR2 = Reel2.SpinAsync(_random.Next(Reel2.SymbolsCount));
            await Task.Delay(100);
            Task TR3 = Reel3.SpinAsync(_random.Next(Reel3.SymbolsCount));
            await Task.Delay(100);
            Task TR4 = Reel4.SpinAsync(_random.Next(Reel4.SymbolsCount));
            await Task.Delay(100);
            Task TR5 = Reel5.SpinAsync(_random.Next(Reel5.SymbolsCount));
            await Task.Delay(100);
            Task TR6 = Reel6.SpinAsync(_random.Next(Reel6.SymbolsCount));
            await Task.Delay(100);

            await Task.WhenAll(TR1, TR2, TR3, TR4, TR5, TR6);

            IELButtonSpin.IsEnabled = true;
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
