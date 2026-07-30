using IEL.UserElementsControl.Base;
using OperPageLes.CORE.Enums.Theme;
using OperPageLes.CORE.Struct;
using OPLAPI.CORE.Animation;
using System.Windows;
using System.Windows.Media;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.UserElementsControl.Default
{
    /// <summary>
    /// Логика взаимодействия для OPLCheckAudioDevice.xaml
    /// </summary>
    public partial class OPLCheckAudioDevice : System.Windows.Controls.UserControl
    {
        #region Properties

        #region Content
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(OPLCheckAudioDevice),
                new(
                    (sender, e) =>
                    {
                        ((OPLCheckAudioDevice)sender).ImageBrushIcon.ImageSource = (ImageSource)e.NewValue;
                    }));

        /// <summary>
        /// Иконка активности
        /// </summary>
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        #endregion

        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLCheckAudioDevice),
                new(
                    (sender, e) =>
                    {
                        ((OPLCheckAudioDevice)sender).DeviceButton.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст маименования аудио девайса
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Объект события активации левым щелчком мыши
        /// </summary>
        public event IELButtonBase.ActivateHandler? OnActivateMouseLeft;

        /// <summary>
        /// Объект события активации правым щелчком мыши
        /// </summary>
        public event IELButtonBase.ActivateHandler? OnActivateMouseRight;

        /// <summary>
        /// Текущий индекс аудио девайса
        /// </summary>
        internal int IndexCurrentDevice;

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Свойство данных активации
        /// </summary>
        private bool _Activate;

        /// <summary>
        /// Состояние активации элемента
        /// </summary>
        public bool Activate
        {
            get => _Activate;
            set
            {
                if (_Activate == value) return;
                else if (ManagerAnimation != null)
                {
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ImageBrushIcon, ImageBrush.OpacityProperty,
                        value ? 1d : 0d, TimeSpan.FromMilliseconds(600d));
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderActive, WidthProperty,
                        value ? 20d : 0d, TimeSpan.FromMilliseconds(500d));
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderActive, HeightProperty,
                        value ? 20d : 0d, TimeSpan.FromMilliseconds(500d));
                }
                else
                {
                    ImageBrushIcon.Opacity = value ? 1d : 0d;
                    BorderActive.Width = value ? 20d : 0d;
                    BorderActive.Height = value ? 20d : 0d;
                }
                _Activate = value;
            }
        }

        public OPLCheckAudioDevice()
        {
            InitializeComponent();
            //DeviceButton.Palette = App.CurrentApp.ActiveThemeApplication[PaletteEnum.Tangerine];
            DeviceButton.Text = "Неизвестный";
            DeviceButton.OnActivateMouseLeft += (sender, e) => OnActivateMouseLeft?.Invoke(this, e);
            DeviceButton.OnActivateMouseRight += (sender, e) => OnActivateMouseRight?.Invoke(this, e);
            ImageBrushIcon.Opacity = 0d;
            ImageBrushIcon.ImageSource = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Check));
            BorderActive.Width = 0d;
            BorderActive.Height = 0d;
            BorderActive.BorderBrush = DeviceButton.SourceBorderBrush.SourceBrush;
            Activate = false;
        }
    }
}
