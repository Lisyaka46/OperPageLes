using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace OperPageLes.UI.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для PageGeneral.xaml
    /// </summary>
    public partial class PageGeneralSetting : Page
    {
        /// <summary>
        /// Размер буфера из настроек
        /// </summary>
        private readonly int OriginalSizeBuffer = -1;

        internal PageGeneralSetting()
        {
            InitializeComponent();
            #region PathMenuImage
            string PathBackgroundImage = App.CurrentApp.SettingMainApplication.PathMenuImage;
            TextBlockFailedImageSetup.Opacity = 0d;
            if (PathBackgroundImage.Length > 0)
            {
                IELTextBoxDirectoryBackground.Text = PathBackgroundImage;
                ImageBackground.Source = new BitmapImage(new Uri(PathBackgroundImage, UriKind.RelativeOrAbsolute));
                IELButtonClearImage.IsEnabled = true;
            }
            else
            {
                ImageBackground.Opacity = 0d;
                IELTextBoxDirectoryBackground.Text = string.Empty;
                IELButtonClearImage.IsEnabled = false;
            }
            IELTextBoxDirectoryBackground.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        BorderPathImageBackground.Focus();
                        break;
                }
            };
            IELButtonDialogDirectoryFile.OnActivateMouseLeft += (sender, e, Key) =>
            {
                System.Windows.Forms.OpenFileDialog dialog = new()
                {
                    //FileName = "Обзор файла изображения", // Default file name
                    DefaultExt = ".png", // Default file extension
                    Filter =
                    "Все поддерживаемые файлы изображений|*.png;*.jpeg;*.jpg;*.bmp|" +
                    "Растровое изображение|*.png|" +
                    "Сжатое изображение|*.jpeg;*.jpg|" +
                    "Карта растрового изображения|*.bmp" // Filter files by extension
                };
                dialog.FileOk += (sender, e) =>
                {
                    SetImageUriValue(dialog.FileName);
                };
                dialog.ShowDialog();
            };
            IELButtonSetTextClipboard.OnActivateMouseLeft += (sender, e, Key) =>
            {
                SetImageUriValue(System.Windows.Clipboard.GetText());
            };
            IELButtonClearImage.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.AnimateDoubleEffect(ImageBackground, OpacityProperty, 0d, TimeSpan.FromMilliseconds(2000d));
                IELTextBoxDirectoryBackground.Text = string.Empty;
                IELButtonClearImage.IsEnabled = false;
                App.CurrentApp.SettingMainApplication.PathMenuImage.Value = string.Empty;
            };
            #endregion
            #region BufferSize
            BorderSettingBufferSize.Opacity = 0d;
            RowDefinitionBufferSize.MaxHeight = RowDefinitionBufferSize.MinHeight;
            OriginalSizeBuffer = App.CurrentApp.SettingMainApplication.BufferSize;
            SliderBufferSize.Value = OriginalSizeBuffer;
            TextBlockSliderBufferSize.Text = SliderBufferSize.Value.ToString();
            //BorderSettingBufferSize.Margin = new(BorderSettingBufferSize.Margin.Left, 0, BorderSettingBufferSize.Margin.Right, 35);
            SliderBufferSize.ValueChanged += (sender, e) =>
            {
                TextBlockSliderBufferSize.Text = e.NewValue.ToString();
                if (RowDefinitionBufferSize.MaxHeight != (e.NewValue != OriginalSizeBuffer ? RowDefinitionBufferSize.Height.Value : RowDefinitionBufferSize.MinHeight))
                {
                    DoubleAnimation animation = App.GetDoubleAnimate();
                    animation.BeginTime = TimeSpan.FromMilliseconds(BorderSettingBufferSize.Opacity != 0d && BorderSettingBufferSize.Opacity != 1d ? 0d : 130d);
                    animation.Duration = TimeSpan.FromMilliseconds(1200d);
                    animation.To = e.NewValue != OriginalSizeBuffer ? RowDefinitionBufferSize.Height.Value : RowDefinitionBufferSize.MinHeight;
                    Storyboard storyboard = new();
                    storyboard.Children.Add(animation);
                    Storyboard.SetTarget(animation, RowDefinitionBufferSize);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(RowDefinition.MaxHeight)"));
                    storyboard.Begin();

                    animation.To = e.NewValue != OriginalSizeBuffer ? 1d : 0d;
                    BorderSettingBufferSize.BeginAnimation(OpacityProperty, animation);
                }
            };
            IELButtonTextClearValue.OnActivateMouseLeft += (sender, e, Key) =>
            {
                SliderBufferSize.Value = OriginalSizeBuffer;
                App.CurrentApp.SettingMainApplication.BufferSize.Value = OriginalSizeBuffer;
            };
            SliderBufferSize.MouseLeave += (sender, e) =>
            {
                if (SliderBufferSize.Value != OriginalSizeBuffer)
                    App.CurrentApp.SettingMainApplication.BufferSize.Value = (int)SliderBufferSize.Value;
            };
            #endregion
            #region BlurBackgroundDataTime
            CheckBoxBlurDataTimeImage.IsChecked = App.CurrentApp.SettingMainApplication.BlurBackgroundDataTime;
            CheckBoxBlurDataTimeImage.Checked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.BlurBackgroundDataTime.Value = true;
            };
            CheckBoxBlurDataTimeImage.Unchecked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.BlurBackgroundDataTime.Value = false;
            };
            #endregion
            #region MillisecondInternetConnection
            CheckBoxInternetConnectionMillisecond.IsChecked = App.CurrentApp.SettingMainApplication.MillisecondInternetConnection;
            CheckBoxInternetConnectionMillisecond.Checked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.MillisecondInternetConnection.Value = true;
            };
            CheckBoxInternetConnectionMillisecond.Unchecked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.MillisecondInternetConnection.Value = false;
            };
            #endregion
            #region DefaultOpenUrlWebView
            IELTextBoxDefaultUrl.Text = App.CurrentApp.SettingMainApplication.DefaultOpenUrlWebView;
            IELTextBoxDefaultUrl.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        Focus();
                        break;
                }
            };
            IELTextBoxDefaultUrl.TextChanged += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.DefaultOpenUrlWebView.Value = IELTextBoxDefaultUrl.Text;
            };
            #endregion
            #region UseOpenLinkInPageBrowser
            CheckBoxUsePageBrowser.IsChecked = App.CurrentApp.SettingMainApplication.UseOpenLinkInPageBrowser;
            CheckBoxUsePageBrowser.Checked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.UseOpenLinkInPageBrowser.Value = true;
            };
            CheckBoxUsePageBrowser.Unchecked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.UseOpenLinkInPageBrowser.Value = false;
            };
            #endregion
            #region UseOnlyCreatePageWebBrowser
            CheckBoxUseOnlyCreatePageBrowser.IsChecked = App.CurrentApp.SettingMainApplication.UseOnlyCreatePageWebBrowser;
            CheckBoxUseOnlyCreatePageBrowser.Checked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.UseOnlyCreatePageWebBrowser.Value = true;
            };
            CheckBoxUseOnlyCreatePageBrowser.Unchecked += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.UseOnlyCreatePageWebBrowser.Value = false;
            };
            #endregion
        }

        /// <summary>
        /// Установить настройку фоновой картинки
        /// </summary>
        /// <param name="Uri">Ссылка или директория на элемент картинки</param>
        private void SetImageUriValue(string Uri)
        {
            try
            {
                BitmapImage image = new(new Uri(Uri, UriKind.RelativeOrAbsolute));
                if (image.PixelWidth > 0 && image.PixelHeight > 0)
                {
                    IELTextBoxDirectoryBackground.Text = Uri;
                    ImageBackground.Source = image;
                    App.AnimateBlurEffect(BlurEffectImageBackground, 10u, 2000d);
                    IELButtonClearImage.IsEnabled = true;
                    App.CurrentApp.SettingMainApplication.PathMenuImage.Value = Uri;

                    App.AnimateDoubleEffect(ImageBackground, OpacityProperty, 0.6d, TimeSpan.FromMilliseconds(1000d));
                }
            }
            catch
            {
                App.AnimateDoubleEffect(TextBlockFailedImageSetup, OpacityProperty, 1d, 0d, TimeSpan.FromMilliseconds(5000d));
            }
        }
    }
}
