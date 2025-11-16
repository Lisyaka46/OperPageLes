using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl.Interfaces;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLViewerLoadingProcess.xaml
    /// </summary>
    public partial class OPLImageViewer : IELButton, IOPLObjectViewer<ImageSource>
    {
        private IELUsingObjectSetting _IELSettingObject = new();
        /// <summary>
        /// Настройка использования объекта
        /// </summary>
        public IELUsingObjectSetting IELSettingObject
        {
            get => _IELSettingObject;
            set
            {
                _IELSettingObject = value;
            }
        }

        /// <summary>
        /// Данные пути к медиа загрузки объекта
        /// </summary>
        public ImageSource SourceView
        {
            get => IndicatorImage.Source;
            set => IndicatorImage.Source = value;
        }

        /// <summary>
        /// Размер текста
        /// </summary>
        public new double FontSize
        {
            get => base.FontSize;
            set
            {
                base.FontSize = value;
                TextBlockName.FontSize = value;
            }
        }

        /// <summary>
        /// Шрифт текста в элементе
        /// </summary>
        public new System.Windows.Media.FontFamily FontFamily
        {
            get => base.FontFamily;
            set
            {
                base.FontFamily = value;
                TextBlockName.FontFamily = value;
            }
        }

        /// <summary>
        /// Тект наименование процесса загрузки
        /// </summary>
        public string Text
        {
            get => TextBlockName.Text;
            set
            {
                TextBlockName.Text = value;
            }
        }

        public OPLImageViewer()
        {
            InitializeComponent();
            #region Background
            BorderMain.Background = QBackground.InicializeConnectedSolidColorBrush();
            #endregion

            #region BorderBrush
            BorderMain.BorderBrush = QBorderBrush.InicializeConnectedSolidColorBrush();
            CancelIndicator.BorderBrush = QBorderBrush.InicializeConnectedSolidColorBrush();
            #endregion

            #region Foreground
            TextBlockName.Foreground = QForeground.InicializeConnectedSolidColorBrush();
            TextBlockCancel.Foreground = QForeground.InicializeConnectedSolidColorBrush();
            #endregion
            IndicatorImage.Opacity = 0d;
            IsEnabled = true;

            MouseEnter += (sender, e) =>
            {
                if (IsEnabled)
                {
                    SetActiveSpecrum(StateSpectrum.Select, true);
                    IELSettingObject.StartHover();
                }
            };

            MouseLeave += (sender, e) =>
            {
                if (IsEnabled)
                {
                    SetActiveSpecrum(StateSpectrum.Default, true);
                    IELSettingObject.StopHover();
                }
            };

            MouseDown += (sender, e) =>
            {
                if (IsEnabled)
                {
                    if (
                    (e.LeftButton == MouseButtonState.Pressed && OnActivateMouseLeft != null) ||
                    (e.RightButton == MouseButtonState.Pressed && OnActivateMouseRight != null))
                    {
                        SetActiveSpecrum(StateSpectrum.Used, false);
                        IELSettingObject.StopHover();
                    }
                }
            };

            MouseLeftButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseLeft != null)
                {
                    SetActiveSpecrum(StateSpectrum.Select, true);
                    OnActivateMouseLeft?.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseRight != null)
                {
                    SetActiveSpecrum(StateSpectrum.Select, true);
                    OnActivateMouseRight?.Invoke(this, e);
                }
            };

            IsEnabledChanged += (sender, e) =>
            {
                StateSpectrum NewValue = (bool)e.NewValue ? StateSpectrum.Default : StateSpectrum.NotEnabled;
                SetActiveSpecrum(NewValue, true);
            };
        }

        /// <summary>
        /// Визуализировать выключение элемента
        /// </summary>
        internal void VisualClose()
        {
            IsEnabled = false;
            App.DoubleAnimationType.AnimateEffect(IndicatorImage, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1300d));
        }

        /// <summary>
        /// Визуализировать включение элемента
        /// </summary>
        internal void VisualOpen()
        {
            IsEnabled = true;
            App.DoubleAnimationType.AnimateEffect(IndicatorImage, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
        }
    }
}
