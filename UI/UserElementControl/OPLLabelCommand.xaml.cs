using IEL.CORE.Classes;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using InterpreterCommand.Classes;
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
        #region Styles
        internal static readonly BrushSettingQ[] BackgroundStyles =
        [
            new(new byte[,]
                        {
                        { 255, 116, 220, 80 },
                        { 255, 180, 255, 154 },
                        { 255, 196, 239, 201 },
                        { 255, 222, 87, 87 },
                        }),
            new(new byte[,]
                        {
                        { 255, 155, 179, 169 },
                        { 255, 160, 200, 175 },
                        { 255, 221, 254, 241 },
                        { 255, 111, 127, 121 },
                        }),
            new(new byte[,]
                        {
                        { 255, 239, 250, 195 },
                        { 255, 240, 246, 210 },
                        { 255, 195, 218, 250 },
                        { 255, 250, 201, 195 },
                        }),
            new(new byte[,]
                        {
                        { 255, 85, 150, 181 },
                        { 255, 90, 140, 185 },
                        { 255, 181, 137, 85 },
                        { 255, 84, 107, 117 },
                        }),
        ];
        internal static readonly BrushSettingQ[] Borderbrush_Foreground_Styles =
        [
            new(new byte[,]
                        {
                        { 255, 0, 0, 0 },
                        { 255, 19, 35, 12 },
                        { 255, 47, 44, 9 },
                        { 255, 58, 8, 8 },
                        }),
            new(new byte[,]
                        {
                        { 255, 62, 96, 82 },
                        { 255, 65, 100, 85 },
                        { 255, 100, 154, 133 },
                        { 255, 38, 68, 57 },
                        }),
            new(new byte[,]
                        {
                        { 255, 126, 139, 73 },
                        { 255, 130, 150, 69 },
                        { 255, 79, 110, 152 },
                        { 255, 153, 100, 94 },
                        }),
            new(new byte[,]
                        {
                        { 255, 24, 86, 116 },
                        { 255, 30, 83, 107 },
                        { 255, 109, 72, 28 },
                        { 255, 24, 47, 56 },
                        }),
        ];
        #endregion

        private IELUsingObjectSetting _IELSettingObject;
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
                };
                value.BorderBrushQChanged += (NewValue) =>
                {
                    SolidColorBrush color = new(NewValue);
                    BorderMainLabel.BorderBrush = color;
                };
                value.ForegroundQChanged += (NewValue) =>
                {
                    SolidColorBrush color = new(NewValue);
                    TextBlockNameLabel.Foreground = color;
                };
                _IELSettingObject = value;
                _IELSettingObject.UseActiveQSetting();
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
            }
        }

        /// <summary>
        /// Элемент ярлыка
        /// </summary>
        internal LabelAction SourceLabel { get; set; }

        public OPLLabelCommand(LabelAction Label)
        {
            InitializeComponent();
            _IELSettingObject = IELSettingObject = new();
            SourceLabel = Label;
            SourceLabel.AddTag += (Old, New) =>
            {
            };
            SourceLabel.DeleteTag += (Old, New) =>
            {
            };
            UpdateVisualStyle();

            TextBlockNameLabel.Text = Label.Name;
            
            IsEnabledChanged += (sender, e) =>
            {
                bool NewValue = (bool)e.NewValue;
                Color
                    Foreground = NewValue ? IELSettingObject.ForegroundSetting.Default : IELSettingObject.ForegroundSetting.NotEnabled,
                    Background = NewValue ? IELSettingObject.BackgroundSetting.Default : IELSettingObject.BackgroundSetting.NotEnabled,
                    BorderBrush = NewValue ? IELSettingObject.BorderBrushSetting.Default : IELSettingObject.BorderBrushSetting.NotEnabled;
                TimeSpan span = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);

                App.AnimateColorEffect(BorderMainLabel.Background, SolidColorBrush.ColorProperty, Background, span);

                App.AnimateColorEffect(BorderMainLabel.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);

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
                    OnActivateMouseLeft?.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseRight != null)
                {
                    MouseEnterAnimation();
                    OnActivateMouseRight?.Invoke(this, e);
                }
            };
        }

        /// <summary>
        /// Обновить изображение стиля команды
        /// </summary>
        internal void UpdateVisualStyle()
        {
            string name_command = COMInterpreter.ReadNameCommand(SourceLabel.Command);
            byte[] ByteLabelImage;
            int IndexUseStyle;
            switch (name_command)
            {
                case "open_link":
                    IndexUseStyle = 2;
                    ByteLabelImage = Properties.Resources.Link;
                    break;
                case "open_file":
                    IndexUseStyle = 1;
                    ByteLabelImage = Properties.Resources.File;
                    break;
                case "open_directory":
                    IndexUseStyle = 3;
                    ByteLabelImage = Properties.Resources.Folder;
                    break;
                default:
                    IndexUseStyle = 0;
                    ByteLabelImage = Properties.Resources.Command;
                    break;
            };
            ImageElementLabel.Source = App.LoadImage(ByteLabelImage);
            ImageElementLabel.UpdateLayout();
            IELSettingObject.BackgroundSetting = (BrushSettingQ)BackgroundStyles[IndexUseStyle].Clone();
            IELSettingObject.BorderBrushSetting = (BrushSettingQ)Borderbrush_Foreground_Styles[IndexUseStyle].Clone();
            IELSettingObject.ForegroundSetting = (BrushSettingQ)Borderbrush_Foreground_Styles[IndexUseStyle].Clone();
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

            App.AnimateColorEffect(BorderMainLabel.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);

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

            App.AnimateColorEffect(BorderMainLabel.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);

            App.AnimateColorEffect(TextBlockNameLabel.Foreground, SolidColorBrush.ColorProperty, Foreground, span);
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

            App.AnimateColorEffect(BorderMainLabel.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);

            App.AnimateColorEffect(TextBlockNameLabel.Foreground, SolidColorBrush.ColorProperty, Foreground, span);
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
