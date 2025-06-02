using CefSharp;
using IEL.CORE.Enums;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;
using OperPage_les.CORE;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WinRT.Interop;

namespace OperPage_les.Windows.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : System.Windows.Controls.Page
    {
        public PageDeveloper()
        {
            InitializeComponent();
            IELButtonGenerateImage.OnActivateMouseLeft += (sender, Key) =>
            {
                ImageElement.Source = Imaging.CreateBitmapSourceFromHBitmap(GenImage().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                ImageElement.UpdateLayout();
            };
            SliderElementWidth.ValueChanged += (sender, e) =>
            {
                TextBlockSliderValueWidth.Text = $"{(e.NewValue < 10 ? "0" : string.Empty)}{(int)e.NewValue}";
            };
            SliderElementHeight.ValueChanged += (sender, e) =>
            {
                TextBlockSliderValueHeight.Text = $"{(e.NewValue < 10 ? "0" : string.Empty)}{(int)e.NewValue}";
            };
            IELButtonDownloadImage.OnActivateMouseLeft += (sender, Key) =>
            {
                Bitmap bitmap = GenImage();
                bitmap.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Gen.png");
            };
        } 
        
        private Bitmap GenImage()
        {
            int X, Y;
            Bitmap bitmap = new((int)SliderElementWidth.Value, (int)SliderElementHeight.Value);
            for (Y = 0; Y < bitmap.Height; Y++)
            {
                double NumY = Math.Tan(Y + 1) * 100;
                for (X = 0; X < bitmap.Width; X++)
                {
                    double Num = Math.Cos(X + 1) * 100;
                    double R = Math.Tan(Num) * 10 - (Math.Ceiling(NumY / 2) + Math.Acosh(NumY) * 10) / (Math.Tan(Num) * 10) + Math.Atanh(Num);
                    double G = Math.Cbrt(NumY) * 10 + (Math.Cosh(NumY) - Math.Tan(Num) * 10) / (Math.Atan(NumY) * Math.Cos(Num)) + Math.Exp(Num) * 10;
                    double B = (Math.Truncate(Math.Cbrt(Num) + Math.Atan(NumY) * 10) - Math.Atan(NumY)) / (Math.Cbrt(Math.Cos(Num)) / Math.Ceiling(NumY)) - Math.Tan(Num) * 10;
                    bitmap.SetPixel(X, Y, System.Drawing.Color.FromArgb((byte)R, (byte)G, (byte)B));
                }
            }
            return bitmap;
        }
    }
}
