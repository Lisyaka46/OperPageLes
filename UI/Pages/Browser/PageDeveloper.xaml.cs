using CefSharp;
using IEL.CORE.Enums;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;
using OperPage_les.CORE;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WinRT.Interop;
using Binding = System.Windows.Data.Binding;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace OperPage_les.Windows.Pages.Browser
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
            IELButtonGenerateImage.OnActivateMouseLeft += (sender, e, Key) =>
            {
                ImageMap.Source = GenImage();
            };
            IELButtonDownloadImage.OnActivateMouseLeft += (sender, e, Key) =>
            {
                //Bitmap bitmap = GenImage();
                //bitmap.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Gen.png");
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
            #region Sliders
            SliderX.ValueChanged += (sender, e) =>
            {
                TextBlockX.Text = $"X:{e.NewValue}";
            };
            SliderY.ValueChanged += (sender, e) =>
            {
                TextBlockY.Text = $"Y:{e.NewValue}";
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

        private BitmapSource GenImage()
        {
            int LengthY = 200, LengthX = 200;
            Bitmap bitmap = new(LengthX, LengthY);
            for (int Y = 0; Y < LengthY; Y++)
            {
                for (int X = 0; X < LengthX; X++)
                {
                    bitmap.SetPixel(X, Y, System.Drawing.Color.Black);
                    //ArrayRectangle[Y, X].SetBinding(Rectangle.FillProperty, ColorBinding);
                }
            }
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                   IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
