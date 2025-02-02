using AAC20.CORE.Settings;
using IEL.Interfaces.Core;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace AAC20.UI.Pages.Settings
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

        internal PageGeneralSetting()
        {
            InitializeComponent();
            #region PathMenuImage
            string PathBackgroundImage = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.PathMenuImage);
            TextBlockFailedImageSetup.Opacity = 0d;
            if (PathBackgroundImage.Length > 0)
            {
                TextBoxPathMenuImage.Text = PathBackgroundImage;
                ImageBackground.Source = new BitmapImage(new Uri(PathBackgroundImage, UriKind.RelativeOrAbsolute));
                IELButtonClearImage.IsEnabled = true;
            }
            else
            {
                ImageBackground.Opacity = 0d;
                TextBoxPathMenuImage.Text = String.Empty;
                IELButtonClearImage.IsEnabled = false;
            }
            IELButtonDialogDirectoryFile.OnActivateMouseLeft += () =>
            {
                OpenFileDialog dialog = new()
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
                SetImageUriValue(Clipboard.GetText());
            };
            IELButtonClearImage.OnActivateMouseLeft += () =>
            {
                DoubleAnimation animation = DoubleAnimate.Clone();
                animation.Duration = TimeSpan.FromMilliseconds(2000d);
                animation.To = 0d;
                ImageBackground.BeginAnimation(OpacityProperty, animation);
                TextBoxPathMenuImage.Text = String.Empty;
                IELButtonClearImage.IsEnabled = false;
                EventChangeValue?.Invoke(EnumSettingApplication.PathMenuImage, "!");
            };
            #endregion
            #region BufferSize
            string StringBufferSize = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.BufferSize);
            int RealySizeBuffer = Convert.ToInt32(StringBufferSize);
            SliderBufferSize.ValueChanged += (sender, e) =>
            {
                TextBlockSliderBufferSize.Text = e.NewValue.ToString();
                if (BorderSettingBufferSize.Margin.Bottom != (e.NewValue == RealySizeBuffer ? 35 : 2))
                {
                    ThicknessAnimation animation = ThicknessAnimate.Clone();
                    animation.To = new(BorderSettingBufferSize.Margin.Left, 0, BorderSettingBufferSize.Margin.Right, e.NewValue == RealySizeBuffer ? 35 : 2);
                    BorderSettingBufferSize.BeginAnimation(MarginProperty, animation);
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
            SliderBufferSize.Value = RealySizeBuffer;
            #endregion
            #region BlurBackgroundDataTime
            string BlurState = App.CurrentApp.SettingApplication.GetSettingValue(EnumSettingApplication.BlurBackgroundDataTime);
            CheckBoxBlurDataTimeImage.IsChecked = BlurState.Equals("T");
            CheckBoxBlurDataTimeImage.Checked += (sender, e) =>
            {
                EventChangeValue?.Invoke(EnumSettingApplication.BlurBackgroundDataTime, "T");
                App.MainWindowApplication.ChangeBlurImageInDataTime(true);
            };
            CheckBoxBlurDataTimeImage.Unchecked += (sender, e) =>
            {
                EventChangeValue?.Invoke(EnumSettingApplication.BlurBackgroundDataTime, "F");
                App.MainWindowApplication.ChangeBlurImageInDataTime(false);
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
                    TextBoxPathMenuImage.Text = Uri;
                    ImageBackground.Source = image;
                    App.AnimateBlurEffect(BlurEffectImageBackground, 10u, 2000d);
                    IELButtonClearImage.IsEnabled = true;
                    EventChangeValue?.Invoke(EnumSettingApplication.PathMenuImage, Uri);

                    animation.Duration = TimeSpan.FromMilliseconds(1000d);
                    animation.To = 0.15d;
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
