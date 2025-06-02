using IEL.CORE.Classes;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using OperPage_les.CORE.Label;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;

namespace OperPage_les.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLLabelCommand.xaml
    /// </summary>
    public partial class OPLLabelCommand : System.Windows.Controls.UserControl, IIELButton
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
        /// Скругление границ
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => BorderMainLabel.CornerRadius;
            set
            {
                BorderMainLabel.CornerRadius = value;
                BorderTags.CornerRadius = value;
            }
        }

        /// <summary>
        /// Толщина границ
        /// </summary>
        public Thickness BorderThicknessBlock
        {
            get => BorderMainLabel.BorderThickness;
            set
            {
                BorderMainLabel.BorderThickness = value;
                BorderTags.BorderThickness = value;
            }
        }

        /// <summary>
        /// Элемент ярлыка
        /// </summary>
        internal LabelAction SourceLabel { get; set; }

        public OPLLabelCommand(LabelAction Label)
        {
            InitializeComponent();
            SourceLabel = Label;
            SourceLabel.AddTag += (Old, New) =>
            {
                if (New == null) return;
                StackPanelTags.Children.Add(CreateVisualTag(New));
            };
            SourceLabel.DeleteTag += (Old, New) =>
            {
                if (Old == null) return;
                StackPanelTags.Children.RemoveAt(SourceLabel.Tags.IndexOf(Old));
            };
            BorderTags.Margin = new(-35, 0, 0, 0);
            foreach (LabelTag Tag in Label.Tags)
            {
                StackPanelTags.Children.Add(CreateVisualTag(Tag));
            }

            TextBlockNameLabel.Text = Label.Name;
            
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
                    OnActivateMouseLeft?.Invoke(this);
                }
                e.Handled = true;
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseRight != null)
                {
                    MouseEnterAnimation();
                    OnActivateMouseRight?.Invoke(this);
                }
                e.Handled = true;
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
            BorderMainLabel.CornerRadius = new(6, 0, 0, 6);
            App.AnimateDoubleEffect(BorderTags, OpacityProperty, 1d, span);
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

            App.AnimateThicknessEffect(BorderTags, MarginProperty, new(-BorderTags.ActualWidth / 1.2d, 0, 0, 0), span);
            BorderMainLabel.CornerRadius = new(6);
            App.AnimateDoubleEffect(BorderTags, OpacityProperty, 0d, span);
        }

        /// <summary>
        /// Создать визуальный элемент тега
        /// </summary>
        /// <param name="value">Значение отображаемое тега</param>
        /// <returns></returns>
        internal static OPLLabelTag CreateVisualTag(LabelTag NewTag)
        {
            return new()
            {
                BorderThicknessBlock = new(1),
                CornerRadius = new(5),
                Text = string.Empty,
                PaddingContent = new(4, 2, 4, 2),
                FontSize = 14d,
                Tag = NewTag,
                IELSettingObject = new()
                {
                    BackgroundSetting = new(new byte[,]
                        {
                        { 255, 116, 220, 80 },
                        { 255, 180, 255, 154 },
                        { 255, 196, 239, 201 },
                        { 255, 222, 87, 87 },
                        }),
                    BorderBrushSetting = new(new byte[,]
                        {
                        { 255, 0, 0, 0 },
                        { 255, 19, 35, 12 },
                        { 255, 47, 44, 9 },
                        { 255, 58, 8, 8 },
                        }),
                    ForegroundSetting = new(new byte[,]
                        {
                        { 255, 0, 0, 0 },
                        { 255, 19, 35, 12 },
                        { 255, 47, 44, 9 },
                        { 255, 58, 8, 8 },
                        }),
                },
            };
        }
    }
}
