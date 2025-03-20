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

namespace OperPage_les.Windows.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : Page, IPageDefault
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageDeveloper);

        /// <summary>
        /// Объект страницы
        /// </summary>
        public new Page Content => this;

        private Bitmap image;

        public PageDeveloper()
        {
            InitializeComponent();
            image = new(1, 1);
            ListBoxDeveloper.Items.Add("[0] CountVisible=*");
            ListBoxDeveloper.Items.Add("[1] ActualHeight=*");

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
            ButtonPixelColor.Click += (sender, e) =>
            {
                OpenFileDialog FileDialog = new();
                FileDialog.ShowDialog();
                image = new Bitmap(FileDialog.FileName);
                ImageSource.Source = new BitmapImage(new Uri(FileDialog.FileName, UriKind.Absolute));
            };
            ButtonPixelColorCopy.Click += (sender, e) =>
            {
                System.Windows.Clipboard.SetText(TextBoxColorCode.Text);
            };
        }   
    }
}
