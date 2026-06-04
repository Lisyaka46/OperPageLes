using IEL.UserElementsControl.Base;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using OperPageLes.CORE.Struct;
using OPLAnimation.CORE.Animation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Media.Devices;
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
        /// Текущий объект аудио девайса
        /// </summary>
        internal readonly WaveOutCapabilities CurrentDevice;

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

        public OPLCheckAudioDevice(in WaveOutCapabilities SourceDevice)
        {
            InitializeComponent();
            CurrentDevice = SourceDevice;
            DeviceButton.PaletteElement = App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Tangerine];
            DeviceButton.Text = $"{SourceDevice.ProductName}" ?? "Нет аудио вывода";
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
