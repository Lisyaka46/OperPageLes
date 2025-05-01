using OperPage_les.Windows.Frames;
using IEL.Classes;
using IEL.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static IEL.Interfaces.Core.IQData;
using System.Windows.Forms;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using Microsoft.Maui.Controls;

namespace OperPage_les.Windows.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : System.Windows.Controls.Page
    {
        private Bitmap image;

        public PageDeveloper()
        {
            InitializeComponent();
            image = new(1, 1);
            ComboBoxStateInlay.SelectionChanged += (sender, e) =>
            {
                StateSpectrum Spectrum = ComboBoxStateInlay.SelectedIndex switch
                {
                    0 => StateSpectrum.Default,
                    1 => StateSpectrum.NotEnabled,
                    2 => StateSpectrum.Select,
                    3 => StateSpectrum.Used,
                    _ => StateSpectrum.Default,
                };
                HeadInlay.BackgroundSetting.InvokeObjectUsedStateColor(Spectrum);
                HeadInlay.BorderBrushSetting.InvokeObjectUsedStateColor(Spectrum);
                HeadInlay.ForegroundSetting.InvokeObjectUsedStateColor(Spectrum);
            };
            ButtonPixelColorCopy.Click += (sender, e) =>
            {
                System.Windows.Clipboard.SetText(TextBoxColorCode.Text);
            };
        }   
    }
}
