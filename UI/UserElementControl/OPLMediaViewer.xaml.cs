using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl.Interfaces;
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
    public partial class OPLMediaViewer : System.Windows.Controls.UserControl, IIELButton, IOPLObjectViewer<Uri>
    {
        #region Color Setting
        /// <summary>
        /// Ресурсный объект настройки состояний фона
        /// </summary>
        private readonly new BrushSettingQ Background;
        /// <summary>
        /// Объект настройки состояний фона
        /// </summary>
        public BrushSettingQ QBackground
        {
            get => Background;
            set
            {
                Background.SetQData(value);
            }
        }

        /// <summary>
        /// Ресурсный объект настройки состояний границы
        /// </summary>
        private readonly new BrushSettingQ BorderBrush;
        /// <summary>
        /// Объект настройки состояний границы
        /// </summary>
        public BrushSettingQ QBorderBrush
        {
            get => BorderBrush;
            set
            {
                BorderBrush.SetQData(value);
            }
        }

        /// <summary>
        /// Ресурсный объект настройки состояний текста
        /// </summary>
        private readonly new BrushSettingQ Foreground;
        /// <summary>
        /// Объект настройки состояний текста
        /// </summary>
        public BrushSettingQ QForeground
        {
            get => Foreground;
            set
            {
                Foreground.SetQData(value);
            }
        }
        #endregion

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
        /// Смещение контента в объекте
        /// </summary>
        public Thickness PaddingContent
        {
            get => Padding;
            set => Padding = value;
        }

        /// <summary>
        /// Объект события активации кнопки левым щелчком мыши
        /// </summary>
        public IIELButton.ActivateHandler? OnActivateMouseLeft { get; set; }

        /// <summary>
        /// Объект события активации кнопки правым щелчком мыши
        /// </summary>
        public IIELButton.ActivateHandler? OnActivateMouseRight { get; set; }

        /// <summary>
        /// Данные пути к медиа загрузки объекта
        /// </summary>
        public Uri SourceView
        {
            get => IndicatorMedia.Source;
            set => IndicatorMedia.Source = value;
        }

        /// <summary>
        /// Скругление границ
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => BorderMain.CornerRadius;
            set
            {
                BorderMain.CornerRadius = value;
            }
        }

        /// <summary>
        /// Толщина границ
        /// </summary>
        public Thickness BorderThicknessBlock
        {
            get => BorderMain.BorderThickness;
            set
            {
                BorderMain.BorderThickness = value;
            }
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

        private bool _IsCanceledManipulate;
        /// <summary>
        /// Состояние отмены загрузки
        /// </summary>
        internal bool IsCanceledManipulate
        {
            get => _IsCanceledManipulate;
            set
            {
                CancelIndicator.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                _IsCanceledManipulate = value;
            }
        }

        public OPLMediaViewer()
        {
            InitializeComponent();
            #region Background
            Background = new();
            BorderMain.Background = new SolidColorBrush(Background.ActiveSpectrumColor);

            Background.ConnectSolidColorBrush((SolidColorBrush)BorderMain.Background);
            #endregion

            #region BorderBrush
            BorderBrush = new();
            BorderMain.BorderBrush = new SolidColorBrush(BorderBrush.ActiveSpectrumColor);
            CancelIndicator.BorderBrush = new SolidColorBrush(BorderBrush.ActiveSpectrumColor);

            BorderBrush.ConnectSolidColorBrush((SolidColorBrush)BorderMain.BorderBrush);
            BorderBrush.ConnectSolidColorBrush((SolidColorBrush)CancelIndicator.BorderBrush);
            #endregion

            #region Foreground
            Foreground = new();
            TextBlockName.Foreground = new SolidColorBrush(Foreground.ActiveSpectrumColor);
            TextBlockCancel.Foreground = new SolidColorBrush(Foreground.ActiveSpectrumColor);

            Foreground.ConnectSolidColorBrush((SolidColorBrush)TextBlockName.Foreground);
            Foreground.ConnectSolidColorBrush((SolidColorBrush)TextBlockCancel.Foreground);
            #endregion
            IsCanceledManipulate = true;
            IndicatorMedia.MediaEnded += (sender, e) =>
            {
                IndicatorMedia.Position = TimeSpan.FromMilliseconds(1);
            };
            IndicatorMedia.Opacity = 0d;
            IndicatorMedia.Source = new(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingDefault)));
            IsEnabled = false;

            MouseEnter += (sender, e) =>
            {
                if (IsEnabled)
                {
                    Background.SetActiveSpecrum(StateSpectrum.Select, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Select, true);
                    IELSettingObject.StartHover();
                }
            };

            MouseLeave += (sender, e) =>
            {
                if (IsEnabled)
                {
                    Background.SetActiveSpecrum(StateSpectrum.Default, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Default, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Default, true);
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
                        Background.SetActiveSpecrum(StateSpectrum.Used, false);
                        BorderBrush.SetActiveSpecrum(StateSpectrum.Used, false);
                        Foreground.SetActiveSpecrum(StateSpectrum.Used, false);
                        IELSettingObject.StopHover();
                    }
                }
            };

            MouseLeftButtonUp += (sender, e) =>
            {
                if (IsEnabled && IsCanceledManipulate && OnActivateMouseLeft != null)
                {
                    Background.SetActiveSpecrum(StateSpectrum.Select, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Select, true);
                    OnActivateMouseLeft?.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (IsEnabled && IsCanceledManipulate && OnActivateMouseRight != null)
                {
                    Background.SetActiveSpecrum(StateSpectrum.Select, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Select, true);
                    OnActivateMouseRight?.Invoke(this, e);
                }
            };

            IsEnabledChanged += (sender, e) =>
            {
                StateSpectrum NewValue = (bool)e.NewValue ? StateSpectrum.Default : StateSpectrum.NotEnabled;
                Background.SetActiveSpecrum(NewValue, true);
                BorderBrush.SetActiveSpecrum(NewValue, true);
                Foreground.SetActiveSpecrum(NewValue, true);
            };
        }

        /// <summary>
        /// Визуализировать выключение элемента
        /// </summary>
        internal void VisualClose()
        {
            IsEnabled = false;
            App.DoubleAnimationType.AnimateEffect(IndicatorMedia, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1300d));
        }

        /// <summary>
        /// Визуализировать включение элемента
        /// </summary>
        internal void VisualOpen()
        {
            IsEnabled = true;
            App.DoubleAnimationType.AnimateEffect(IndicatorMedia, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
        }
    }
}
