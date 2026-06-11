using OperPageLes.CORE.Struct;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.OIEL.UserElementsControl;
using OPLAPI.OIEL.UserElementsControl.Base;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для PageGeneral.xaml
    /// </summary>
    public partial class PageGeneralSetting : Page, IOPLAnimate
    {
        /// <summary>
        /// Размер буфера из настроек
        /// </summary>
        private readonly int OriginalSizeBuffer = -1;

        private OPLAnimationManager? _ManagerAnimation;
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation
        {
            get => _ManagerAnimation;
            set
            {
                _ManagerAnimation = value;
                CheckBoxInternetConnectionMillisecond.ManagerAnimation = value;
                CheckBoxLoadingBorderVisualizate.ManagerAnimation = value;
                CheckBoxUsePageBrowser.ManagerAnimation = value;
                CheckBoxUseOnlyCreatePageBrowser.ManagerAnimation = value;
            }
        }

        internal PageGeneralSetting()
        {
            InitializeComponent();

            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Olive].ConnectPalleteFromIELElement(IELTextBoxDirectoryBackground);
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Red].ConnectPalleteFromIELElement(IELButtonClearImage);
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELButtonSetTextClipboard);
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonDialogDirectoryFile);

            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonTextClearValue);

            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Violet].ConnectPalleteFromIELElement(IELTextBoxDefaultUrl);

            #region PathMenuImage
            string PathBackgroundImage = App.CurrentApp.SettingMainApplication.PathMenuImage;
            TextBlockFailedImageSetup.Opacity = 0d;
            ImageErrorBitmapBackground.Opacity = 0d;
            if (PathBackgroundImage.Length > 0)
            {
                IELTextBoxDirectoryBackground.Text = PathBackgroundImage;
                try
                {
                    ImageBackground.Source = new BitmapImage(new Uri(PathBackgroundImage, UriKind.RelativeOrAbsolute));
                    IELButtonClearImage.IsEnabled = true;
                }
                catch
                {
                    ImageErrorBitmapBackground.Opacity = 1d;
                    ImageBackground.Opacity = 0d;
                    IELButtonClearImage.IsEnabled = false;
                }
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
            IELButtonDialogDirectoryFile.OnActivateMouseLeft += (sender, e) =>
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
            IELButtonSetTextClipboard.OnActivateMouseLeft += (sender, e) =>
            {
                SetImageUriValue(System.Windows.Clipboard.GetText());
            };
            IELButtonClearImage.OnActivateMouseLeft += (sender, e) =>
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ImageBackground, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(2000d));
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
                    if (ManagerAnimation != null)
                    {
                        DoubleAnimation animation = ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
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
                    else
                    {
                        RowDefinitionBufferSize.MaxHeight = e.NewValue != OriginalSizeBuffer ? RowDefinitionBufferSize.Height.Value : RowDefinitionBufferSize.MinHeight;
                        BorderSettingBufferSize.Opacity = e.NewValue != OriginalSizeBuffer ? 1d : 0d;
                    }
                }
            };
            IELButtonTextClearValue.OnActivateMouseLeft += (sender, e) =>
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

            #region MillisecondInternetConnection
            CheckBoxInternetConnectionMillisecond.ImageOpacityTexture = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Wifi));
            CheckBoxInternetConnectionMillisecond.IsChecked = App.CurrentApp.SettingMainApplication.MillisecondInternetConnection;
            CheckBoxInternetConnectionMillisecond.IsCheckedChanged += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.MillisecondInternetConnection.Value = e;
            };
            #endregion
            
            #region LoadingBorderVisualizate
            CheckBoxLoadingBorderVisualizate.IsChecked = App.CurrentApp.SettingMainApplication.LoadingBorderVisualizate;
            CheckBoxLoadingBorderVisualizate.IsCheckedChanged += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.LoadingBorderVisualizate.Value = e;
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
            CheckBoxUsePageBrowser.ImageOpacityTexture = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.World));
            CheckBoxUsePageBrowser.IsChecked = App.CurrentApp.SettingMainApplication.UseOpenLinkInPageBrowser;
            CheckBoxUsePageBrowser.IsCheckedChanged += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.UseOpenLinkInPageBrowser.Value = e;
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, CheckBoxUseOnlyCreatePageBrowser, OpacityProperty,
                    e ? 1d : 0.4d, TimeSpan.FromMilliseconds(300d));
            };
            #endregion

            #region UseOnlyCreatePageWebBrowser
            CheckBoxUseOnlyCreatePageBrowser.Opacity = CheckBoxUsePageBrowser.IsChecked ? 1d : 0.4d;
            CheckBoxUseOnlyCreatePageBrowser.ImageOpacityTexture = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.NewElement));
            CheckBoxUseOnlyCreatePageBrowser.IsChecked = App.CurrentApp.SettingMainApplication.UseOnlyCreatePageWebBrowser;
            CheckBoxUseOnlyCreatePageBrowser.IsCheckedChanged += (sender, e) =>
            {
                App.CurrentApp.SettingMainApplication.UseOnlyCreatePageWebBrowser.Value = e;
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
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ImageErrorBitmapBackground, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(100d));
                if (image.PixelWidth > 0 && image.PixelHeight > 0)
                {
                    IELTextBoxDirectoryBackground.Text = Uri;
                    ImageBackground.Source = image;
                    OPLWindowBase.AnimateBlurEffect(ManagerAnimation, BlurEffectImageBackground, 10u, 2000d);
                    IELButtonClearImage.IsEnabled = true;
                    App.CurrentApp.SettingMainApplication.PathMenuImage.Value = Uri;

                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ImageBackground, OpacityProperty,
                        0.6d, TimeSpan.FromMilliseconds(1000d));
                }
            }
            catch
            {
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, TextBlockFailedImageSetup, OpacityProperty,
                    1d, 0d, TimeSpan.FromMilliseconds(5000d));
            }
        }
    }
}
