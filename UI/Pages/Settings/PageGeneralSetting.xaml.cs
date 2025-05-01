using OperPage_les.CORE.Settings;
using IEL.Interfaces.Core;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace OperPage_les.UI.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для PageGeneral.xaml
    /// </summary>
    public partial class PageGeneralSetting : Page, IPageSetting<EnumSettingApplication>
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageGeneralSetting);

        /// <summary>
        /// Объект страницы
        /// </summary>
        public Page PageContent => this;

        /// <summary>
        /// Событие изменения значений настроек
        /// </summary>
        internal IPageSetting<EnumSettingApplication>.ChangeValue? EventChangeValue;

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления Color значением
        /// </summary>
        private static readonly ColorAnimation ColorAnimate = new(Colors.Black, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления double значением
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimate = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        private int RealySizeBuffer = -1;

        internal PageGeneralSetting()
        {
            InitializeComponent();
            #region PathMenuImage
            string PathBackgroundImage = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.PathMenuImage);
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
            IELButtonDialogDirectoryFile.OnActivateMouseLeft += () =>
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
            IELButtonSetTextClipboard.OnActivateMouseLeft += () =>
            {
                SetImageUriValue(System.Windows.Clipboard.GetText());
            };
            IELButtonClearImage.OnActivateMouseLeft += () =>
            {
                DoubleAnimation animation = DoubleAnimate.Clone();
                animation.Duration = TimeSpan.FromMilliseconds(2000d);
                animation.To = 0d;
                ImageBackground.BeginAnimation(OpacityProperty, animation);
                IELTextBoxDirectoryBackground.Text = string.Empty;
                IELButtonClearImage.IsEnabled = false;
                EventChangeValue?.Invoke(EnumSettingApplication.PathMenuImage, "!");
            };
            #endregion
            #region BufferSize
            string StringBufferSize = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.BufferSize);
            RealySizeBuffer = Convert.ToInt32(StringBufferSize);
            SliderBufferSize.Value = RealySizeBuffer;
            TextBlockSliderBufferSize.Text = SliderBufferSize.Value.ToString();
            //BorderSettingBufferSize.Margin = new(BorderSettingBufferSize.Margin.Left, 0, BorderSettingBufferSize.Margin.Right, 35);
            SliderBufferSize.ValueChanged += (sender, e) =>
            {
                TextBlockSliderBufferSize.Text = e.NewValue.ToString();
                if (RowDefinitionBufferSize.MaxHeight != (e.NewValue != RealySizeBuffer ? RowDefinitionBufferSize.Height.Value : RowDefinitionBufferSize.MinHeight))
                {
                    DoubleAnimation animation = App.GetDoubleAnimate();
                    animation.Duration = TimeSpan.FromMilliseconds(1200d);
                    animation.To = e.NewValue != RealySizeBuffer ? RowDefinitionBufferSize.Height.Value : RowDefinitionBufferSize.MinHeight;
                    Storyboard storyboard = new();
                    storyboard.Children.Add(animation);
                    Storyboard.SetTarget(animation, RowDefinitionBufferSize);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("(RowDefinition.MaxHeight)"));
                    storyboard.Begin();
                }
            };
            IELButtonTextClearValue.OnActivateMouseLeft += () =>
            {
                SliderBufferSize.Value = RealySizeBuffer;
                EventChangeValue?.Invoke(EnumSettingApplication.BufferSize, SliderBufferSize.Value.ToString());
            };
            SliderBufferSize.MouseLeave += (sender, e) =>
            {
                if (SliderBufferSize.Value != RealySizeBuffer)
                    EventChangeValue?.Invoke(EnumSettingApplication.BufferSize, SliderBufferSize.Value.ToString());
            };
            #endregion
            #region BlurBackgroundDataTime
            string BlurState = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.BlurBackgroundDataTime);
            CheckBoxBlurDataTimeImage.IsChecked = BlurState.Equals("T");
            CheckBoxBlurDataTimeImage.Checked += (sender, e) =>
            {
                EventChangeValue?.Invoke(EnumSettingApplication.BlurBackgroundDataTime, "T");
            };
            CheckBoxBlurDataTimeImage.Unchecked += (sender, e) =>
            {
                EventChangeValue?.Invoke(EnumSettingApplication.BlurBackgroundDataTime, "F");
            };
            #endregion
            #region MillisecondInternetConnection
            string MillisecondConnection = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.MillisecondInternetConnection);
            CheckBoxInternetConnectionMillisecond.IsChecked = MillisecondConnection.Equals("T");
            CheckBoxInternetConnectionMillisecond.Checked += (sender, e) =>
            {
                EventChangeValue?.Invoke(EnumSettingApplication.MillisecondInternetConnection, "T");
            };
            CheckBoxInternetConnectionMillisecond.Unchecked += (sender, e) =>
            {
                EventChangeValue?.Invoke(EnumSettingApplication.MillisecondInternetConnection, "F");
            };
            #endregion
            #region DefaultOpenUrlWebView
            string DefaultUrl = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.DefaultOpenUrlWebView);
            IELTextBoxDefaultUrl.Text = DefaultUrl;
            IELTextBoxDefaultUrl.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        BorderPathImageBackground.Focus();
                        break;
                }
            };
            IELTextBoxDefaultUrl.TextChanged += (sender, e) =>
            {
                EventChangeValue?.Invoke(EnumSettingApplication.DefaultOpenUrlWebView, IELTextBoxDefaultUrl.Text);
            };
            #endregion
        }

        /// <summary>
        /// Установить настройку фоновой картинки
        /// </summary>
        /// <param name="Uri">Ссылка или директория на элемент картинки</param>
        private void SetImageUriValue(string Uri)
        {
            DoubleAnimation animation = DoubleAnimate.Clone();
            try
            {
                BitmapImage image = new(new Uri(Uri, UriKind.RelativeOrAbsolute));
                if (image.PixelWidth > 0 && image.PixelHeight > 0)
                {
                    IELTextBoxDirectoryBackground.Text = Uri;
                    ImageBackground.Source = image;
                    App.AnimateBlurEffect(BlurEffectImageBackground, 10u, 2000d);
                    IELButtonClearImage.IsEnabled = true;
                    EventChangeValue?.Invoke(EnumSettingApplication.PathMenuImage, Uri);

                    animation.Duration = TimeSpan.FromMilliseconds(1000d);
                    animation.To = 0.6d;
                    ImageBackground.BeginAnimation(OpacityProperty, animation);
                }
            }
            catch
            {
                animation.Duration = TimeSpan.FromMilliseconds(5000d);
                animation.From = 1d;
                animation.To = 0d;
                TextBlockFailedImageSetup.BeginAnimation(OpacityProperty, animation);
            }
        }
    }
}
