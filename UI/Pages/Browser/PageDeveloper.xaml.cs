using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace OperPage_les.UI.Pages.Browser
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
            for (Y = 0; Y < Height; Y++)
            {
                for (X = 0; X < Width; X++)
                {
                    await Task.Run(() =>
                    {
                        byte R = (byte)(Math.Cos(X / 5));
                        byte G = (byte)0;
                        byte B = (byte)0;
                        bitmap.SetPixel(X, Y, System.Drawing.Color.FromArgb(R, G, B));
                        Dispatcher.Invoke(() => TextblockInformation.Text = $"X:{X} || Y:{Y}");
                    });
                }
            }
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                   IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
        } /* Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                   IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());*/
    }
}
