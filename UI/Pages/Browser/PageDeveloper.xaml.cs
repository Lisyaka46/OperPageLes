using OperPageLes.CORE;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace OperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : System.Windows.Controls.Page
    {
        private bool ScaleMode = false;
        private bool KeyEventActivate = false;
        public PageDeveloper()
        {
            InitializeComponent();
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

                // Create a storyboard to apply the animation.
                ThicknessAnimation animation = App.ThicknessAnimationType.SourceAnimation.Clone();
                animation.Duration = TimeSpan.FromMilliseconds(1000d);
                animation.From = new(3);
                animation.To = new(10);
                animation.AutoReverse = true;
                Storyboard.SetTargetName(animation, "BorderWindowMain");
                Storyboard.SetTargetProperty(
                    animation, new PropertyPath(Border.BorderThicknessProperty));
                Storyboard ellipseStoryboard = new();
                ellipseStoryboard.Children.Add(animation);
                ellipseStoryboard.Begin(App.MainWindow);
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
            Func<int, int, Color> d = new((X, Y) =>
            {
                byte R = (byte)(Math.Cos(Width / 20d - X) * 255);
                byte G = (byte)(Math.Cos(Height / 20d - Y) * 255);
                byte B = (byte)(0);
                return System.Drawing.Color.FromArgb(R, G, B);
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
