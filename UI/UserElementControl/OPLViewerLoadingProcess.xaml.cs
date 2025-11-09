using ApplicationOperPageLes.CORE.Struct;
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
    public partial class OPLViewerLoadingProcess : System.Windows.Controls.UserControl, IIELButton
    {
        #region Color Setting
        /// <summary>
        /// Ресурсный объект настройки состояний фона
        /// </summary>
        public BrushSettingQ _Background;
        /// <summary>
        /// Объект настройки состояний фона
        /// </summary>
        public new BrushSettingQ Background
        {
            get => _Foreground;
            set => _Foreground.ColorData = value.ColorData;
        }

        /// <summary>
        /// Ресурсный объект настройки состояний границы
        /// </summary>
        public BrushSettingQ _BorderBrush;
        /// <summary>
        /// Объект настройки состояний границы
        /// </summary>
        public new BrushSettingQ BorderBrush
        {
            get => _Foreground;
            set => _Foreground.ColorData = value.ColorData;
        }

        /// <summary>
        /// Ресурсный объект настройки состояний текста
        /// </summary>
        public BrushSettingQ _Foreground;
        /// <summary>
        /// Объект настройки состояний текста
        /// </summary>
        public new BrushSettingQ Foreground
        {
            get => _Foreground;
            set => _Foreground.ColorData = value.ColorData;
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
        /// Параметр указатель на значение загрузки
        /// </summary>
        public static readonly DependencyProperty ValueLoadingProperty = DependencyProperty.Register("ValueLoading", typeof(double), typeof(OPLViewerLoadingProcess));

        /// <summary>
        /// Данные изображения в кнопке закрытия объекта
        /// </summary>
        public double ValueLoading
        {
            get => ProgressBarLoading.Value;
            set => ProgressBarLoading.Value = value;
        }

        /// <summary>
        /// Видимость загрузки
        /// </summary>
        public Visibility VisibilityLoading
        {
            get => ProgressBarLoading.Visibility;
            set => ProgressBarLoading.Visibility = value;
        }

        /// <summary>
        /// Данные пути к медиа загрузки объекта
        /// </summary>
        public Uri SourceMediaLoading
        {
            get => IndicatorLoading.Source;
            set => IndicatorLoading.Source = value;
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

        public OPLViewerLoadingProcess()
        {
            InitializeComponent();
            #region Background
            _Background = new();
            BorderMain.Background = new SolidColorBrush(Background.ActiveSpectrumColor);
            Background.SetSpectrumAction((Args) =>
            {
                if (Args.AnimatedEvent)
                {
                    ColorAnimation anim = App.ColorAnimationType.SourceAnimation.Clone();
                    anim.Duration = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);
                    anim.To = Args.Value;
                    BorderMain.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                }
                else
                {
                    ((SolidColorBrush)BorderMain.Background).Color = Args.Value;
                }
            });
            #endregion

            #region BorderBrush
            _BorderBrush = new();
            BorderMain.BorderBrush = new SolidColorBrush(BorderBrush.ActiveSpectrumColor);
            CancelIndicator.BorderBrush = new SolidColorBrush(BorderBrush.ActiveSpectrumColor);
            BorderBrush.SetSpectrumAction((Args) =>
            {
                if (Args.AnimatedEvent)
                {
                    ColorAnimation anim = App.ColorAnimationType.SourceAnimation.Clone();
                    anim.Duration = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);
                    anim.To = Args.Value;
                    BorderMain.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                    CancelIndicator.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                }
                else
                {
                    ((SolidColorBrush)BorderMain.BorderBrush).Color = Args.Value;
                    ((SolidColorBrush)CancelIndicator.BorderBrush).Color = Args.Value;
                }
            });
            #endregion

            #region Foreground
            _Foreground = new();
            TextBlockName.Foreground = new SolidColorBrush(Foreground.ActiveSpectrumColor);
            TextBlockCancel.Foreground = new SolidColorBrush(Foreground.ActiveSpectrumColor);
            Foreground.SetSpectrumAction((Args) =>
            {
                if (Args.AnimatedEvent)
                {
                    ColorAnimation anim = App.ColorAnimationType.SourceAnimation.Clone();
                    anim.Duration = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);
                    anim.To = Args.Value;
                    TextBlockName.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                    TextBlockCancel.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                }
                else
                {
                    ((SolidColorBrush)TextBlockName.Foreground).Color = Args.Value;
                    ((SolidColorBrush)TextBlockCancel.Foreground).Color = Args.Value;
                }
            });
            #endregion
            IsCanceledManipulate = true;
            IndicatorLoading.MediaEnded += (sender, e) =>
            {
                IndicatorLoading.Position = TimeSpan.FromMilliseconds(1);
            };
            ProgressBarLoading.Value = 0d;
            IndicatorLoading.Opacity = 0d;
            IndicatorLoading.Source = new(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingDefault)));
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
        /// Визуализировать выключение или завершение процесса загрузки
        /// </summary>
        internal void VisualCloseLoading()
        {
            IsEnabled = false;
            if (ProgressBarLoading.Visibility == Visibility.Visible)
                App.DoubleAnimationType.AnimateEffect(ProgressBarLoading, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1300d));
            App.DoubleAnimationType.AnimateEffect(IndicatorLoading, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1300d));
        }

        /// <summary>
        /// Визуализировать включение процесса загрузки
        /// </summary>
        internal void VisualOpenLoading()
        {
            ProgressBarLoading.Value = 0d;
            IsEnabled = true;
            if (ProgressBarLoading.Visibility == Visibility.Visible)
                App.DoubleAnimationType.AnimateEffect(ProgressBarLoading, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
            App.DoubleAnimationType.AnimateEffect(IndicatorLoading, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
        }
    }
}
