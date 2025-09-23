using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OperPage_les.CORE;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace OperPage_les.UI.UserElementControl
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
                        ColorAnimation anim = App.GetColorAnimate(TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond));
                        anim.To = NewValue;
                        BorderMain.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
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
                        ColorAnimation anim = App.GetColorAnimate(TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond));
                        anim.To = NewValue;
                        BorderMain.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                    }
                    else
                    {
                        SolidColorBrush color = new(NewValue);
                        BorderMain.BorderBrush = color;
                    }
                });
                value.ForegroundSetting.SetActionColorChanged((Spectrum, NewValue, Animated) =>
                {
                    if (Animated)
                    {
                        ColorAnimation anim = App.GetColorAnimate(TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond));
                        anim.To = NewValue;
                        TextBlockName.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                    }
                    else
                    {
                        SolidColorBrush color = new(NewValue);
                        TextBlockName.Foreground = color;
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
        public IIELButton.Activate? OnActivateMouseLeft { get; set; }

        /// <summary>
        /// Объект события активации кнопки правым щелчком мыши
        /// </summary>
        public IIELButton.Activate? OnActivateMouseRight { get; set; }

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

        public OPLViewerLoadingProcess()
        {
            InitializeComponent();
            IndicatorLoading.MediaEnded += (sender, e) =>
            {
                IndicatorLoading.Position = TimeSpan.FromMilliseconds(1);
            };
            ProgressBarLoading.Value = 0d;
            IndicatorLoading.Opacity = 0d;
            IndicatorLoading.Source = new(App.DirectoryFileLoadingDefault);
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
                if (IsEnabled && OnActivateMouseLeft != null)
                {
                    IELSettingObject.UseActiveQSetting(StateSpectrum.Select);
                    OnActivateMouseLeft?.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseRight != null)
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
                App.AnimateDoubleEffect(ProgressBarLoading, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1300d));
            App.AnimateDoubleEffect(IndicatorLoading, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1300d));
        }

        /// <summary>
        /// Визуализировать включение процесса загрузки
        /// </summary>
        internal void VisualOpenLoading()
        {
            ProgressBarLoading.Value = 0d;
            IsEnabled = true;
            if (ProgressBarLoading.Visibility == Visibility.Visible)
                App.AnimateDoubleEffect(ProgressBarLoading, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
            App.AnimateDoubleEffect(IndicatorLoading, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
        }
    }
}
