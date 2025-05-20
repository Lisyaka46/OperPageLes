using IEL.CORE.Classes;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;

namespace OperPage_les.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для IELLabelCommand.xaml
    /// </summary>
    public partial class LabelCommand : System.Windows.Controls.UserControl
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
                value.BackgroundQChanged += (NewValue) =>
                {
                    SolidColorBrush color = new(NewValue);
                    BorderMainLabel.Background = color;
                    BorderTags.Background = color;
                };
                value.BorderBrushQChanged += (NewValue) =>
                {
                    SolidColorBrush color = new(NewValue);
                    BorderMainLabel.BorderBrush = color;
                    BorderTags.BorderBrush = color;
                };
                value.ForegroundQChanged += (NewValue) =>
                {
                    SolidColorBrush color = new(NewValue);
                    TextBlockNameLabel.Foreground = color;
                };
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
        /// Данные изображения объекта
        /// </summary>
        public ImageSource ImageSource
        {
            get => ImageElementLabel.Source;
            set => ImageElementLabel.Source = value;
        }

        /// <summary>
        /// Данные изображения тега
        /// </summary>
        public ImageSource ImageTagSource
        {
            get => ImageTag.Source;
            set => ImageTag.Source = value;
        }

        private bool _ImageTagVisible;
        /// <summary>
        /// Видимость изображения тега
        /// </summary>
        public bool ImageTagVisible
        {
            get => _ImageTagVisible;
            set
            {
                App.AnimateDoubleEffect(ImageTag, OpacityProperty, value ? 1d : 0d);
                _ImageTagVisible = value;
            }
        }

        private LabelAction _Label = LabelAction.Empty;
        /// <summary>
        /// Ярлык который выполняется объектом
        /// </summary>
        public LabelAction Label
        {
            get => _Label;
            set
            {
                _Label = value;
                if (value == LabelAction.Empty) IsEnabled = false;
                else
                {
                    if (!IsEnabled) IsEnabled = true;
                    TextBlockNameLabel.Text = _Label.Name;
                }
            }
        }

        private int _Index;
        public int Index
        {
            get => _Index;
            set
            {
                int pre = value + 1;
                IELBlockTagIndex.Text = $"{(pre < 10 ? "0" : string.Empty)}{pre}";
                _Index = value;
            }
        }

        public LabelCommand(LabelAction Label, int Index = 0)
        {
            InitializeComponent();
            BorderTags.Margin = new(-35, 0, 0, 0);
            this.Label = Label;
            this.Index = Index;

            TextBlockNameLabel.Text = this.Label.Name;
            ImageTagVisible = false;
            
            IsEnabledChanged += (sender, e) =>
            {
                Color
                Foreground = (bool)e.NewValue ? IELSettingObject.ForegroundSetting.Default : IELSettingObject.ForegroundSetting.NotEnabled,
                Background = (bool)e.NewValue ? IELSettingObject.BackgroundSetting.Default : IELSettingObject.BackgroundSetting.NotEnabled,
                BorderBrush = (bool)e.NewValue ? IELSettingObject.BorderBrushSetting.Default : IELSettingObject.BorderBrushSetting.NotEnabled;
                TimeSpan span = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);

                App.AnimateColorEffect(BorderMainLabel.Background, SolidColorBrush.ColorProperty, Background, span);
                App.AnimateColorEffect(BorderTags.Background, SolidColorBrush.ColorProperty, Background, span);

                App.AnimateColorEffect(BorderMainLabel.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);
                App.AnimateColorEffect(BorderTags.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);

                App.AnimateColorEffect(TextBlockNameLabel.Foreground, SolidColorBrush.ColorProperty, Foreground, span);
            };

            MouseEnter += (sender, e) =>
            {
                if (IsEnabled)
                {
                    MouseEnterAnimation();
                    IELSettingObject.StartHover();
                }
            };

            MouseLeave += (sender, e) =>
            {
                if (IsEnabled)
                {
                    MouseLeaveAnimation();
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
                        ClickDownAnimation();
                        IELSettingObject.StopHover();
                    }
                }
            };

            MouseLeftButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseLeft != null)
                {
                    MouseEnterAnimation();
                    OnActivateMouseLeft?.Invoke();
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseRight != null)
                {
                    MouseEnterAnimation();
                    OnActivateMouseRight?.Invoke();
                }
            };
        }

        /// <summary>
        /// Анимировать нажатие на кнопку (Down)
        /// </summary>
        /// <param name="StyleClickColor">Стиль нажатия на кнопку</param>
        private void ClickDownAnimation()
        {
            Color
                Background = IELSettingObject.BackgroundSetting.Used,
                BorderBrush = IELSettingObject.BorderBrushSetting.Used,
                Foreground = IELSettingObject.ForegroundSetting.Used;
            TimeSpan span = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);

            App.AnimateColorEffect(BorderMainLabel.Background, SolidColorBrush.ColorProperty, Background, span);
            App.AnimateColorEffect(BorderTags.Background, SolidColorBrush.ColorProperty, Background, span);

            App.AnimateColorEffect(BorderMainLabel.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);
            App.AnimateColorEffect(BorderTags.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);

            App.AnimateColorEffect(TextBlockNameLabel.Foreground, SolidColorBrush.ColorProperty, Foreground, span);
        }

        /// <summary>
        /// Анимация выделения объекта мышью
        /// </summary>
        private void MouseEnterAnimation()
        {
            Color
                Foreground = IELSettingObject.ForegroundSetting.Select,
                Background = IELSettingObject.BackgroundSetting.Select,
                BorderBrush = IELSettingObject.BorderBrushSetting.Select;
            TimeSpan span = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);

            App.AnimateColorEffect(BorderMainLabel.Background, SolidColorBrush.ColorProperty, Background, span);
            App.AnimateColorEffect(BorderTags.Background, SolidColorBrush.ColorProperty, Background, span);

            App.AnimateColorEffect(BorderMainLabel.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);
            App.AnimateColorEffect(BorderTags.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);

            App.AnimateColorEffect(TextBlockNameLabel.Foreground, SolidColorBrush.ColorProperty, Foreground, span);

            App.AnimateThicknessEffect(BorderTags, MarginProperty, new(0), span);
        }

        /// <summary>
        /// Анимация отключения выделения мышью
        /// </summary>
        private void MouseLeaveAnimation()
        {
            Color
                Foreground = IELSettingObject.ForegroundSetting.Default,
                Background = IELSettingObject.BackgroundSetting.Default,
                BorderBrush = IELSettingObject.BorderBrushSetting.Default;
            TimeSpan span = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);

            App.AnimateColorEffect(BorderMainLabel.Background, SolidColorBrush.ColorProperty, Background, span);
            App.AnimateColorEffect(BorderTags.Background, SolidColorBrush.ColorProperty, Background, span);

            App.AnimateColorEffect(BorderMainLabel.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);
            App.AnimateColorEffect(BorderTags.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);

            App.AnimateColorEffect(TextBlockNameLabel.Foreground, SolidColorBrush.ColorProperty, Foreground, span);

            App.AnimateThicknessEffect(BorderTags, MarginProperty, new(-35, 0, 0, 0), span);
        }
    }
}
