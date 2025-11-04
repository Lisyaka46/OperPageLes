using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using OperPageLes.CORE.Struct;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLViewerLoadingProcess.xaml
    /// </summary>
    public partial class OPLViewerLoadingProcess : System.Windows.Controls.UserControl, IIELButton
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
                value.BackgroundSetting.SetActionColorChanged((Spectrum, NewValue, Animated) =>
                {
                    if (Animated)
                    {
                        ColorAnimation anim = App.ColorAnimationType.SourceAnimation.Clone();
                        anim.Duration = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);
                        anim.To = NewValue;
                        BorderMain.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                    }
                    else
                    {
                        SolidColorBrush color = new(NewValue);
                        BorderMain.Background = color;
                    }
                });
                value.BorderBrushSetting.SetActionColorChanged((Spectrum, NewValue, Animated) =>
                {
                    if (Animated)
                    {
                        ColorAnimation anim = App.ColorAnimationType.SourceAnimation.Clone();
                        anim.Duration = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);
                        anim.To = NewValue;
                        BorderMain.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                        CancelIndicator.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                    }
                    else
                    {
                        SolidColorBrush color = new(NewValue);
                        BorderMain.BorderBrush = color;
                        CancelIndicator.BorderBrush = color;
                    }
                });
                value.ForegroundSetting.SetActionColorChanged((Spectrum, NewValue, Animated) =>
                {
                    if (Animated)
                    {
                        ColorAnimation anim = App.ColorAnimationType.SourceAnimation.Clone();
                        anim.Duration = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);
                        anim.To = NewValue;
                        TextBlockName.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                        TextBlockCancel.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.SnapshotAndReplace);
                    }
                    else
                    {
                        SolidColorBrush color = new(NewValue);
                        TextBlockName.Foreground = color;
                        TextBlockCancel.Foreground = color;
                    }
                });
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
                    IELSettingObject.UseActiveQSetting(StateSpectrum.Select);
                    IELSettingObject.StartHover();
                }
            };

            MouseLeave += (sender, e) =>
            {
                if (IsEnabled)
                {
                    IELSettingObject.UseActiveQSetting(StateSpectrum.Default);
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
                        IELSettingObject.UseActiveQSetting(StateSpectrum.Used, false);
                        IELSettingObject.StopHover();
                    }
                }
            };

            MouseLeftButtonUp += (sender, e) =>
            {
                if (IsEnabled && IsCanceledManipulate && OnActivateMouseLeft != null)
                {
                    IELSettingObject.UseActiveQSetting(StateSpectrum.Select);
                    OnActivateMouseLeft?.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (IsEnabled && IsCanceledManipulate && OnActivateMouseRight != null)
                {
                    IELSettingObject.UseActiveQSetting(StateSpectrum.Select);
                    OnActivateMouseRight?.Invoke(this, e);
                }
            };

            IsEnabledChanged += (sender, e) =>
            {
                bool NewValue = (bool)e.NewValue;
                IELSettingObject.UseActiveQSetting(NewValue ? StateSpectrum.Default : StateSpectrum.NotEnabled);
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
