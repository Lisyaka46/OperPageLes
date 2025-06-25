using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FontFamily = System.Windows.Media.FontFamily;
using Color = System.Windows.Media.Color;
using OperPage_les;

namespace IEL
{
    /// <summary>
    /// Логика взаимодействия для OPLButtonBufferCommand.xaml
    /// </summary>
    public partial class OPLButtonBufferCommand : System.Windows.Controls.UserControl, IIELButton
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
                    BorderButton.Background = color;
                };
                value.BorderBrushQChanged += (NewValue) =>
                {
                    SolidColorBrush color = new(NewValue);
                    BorderButton.BorderBrush = color;
                };
                value.ForegroundQChanged += (NewValue) =>
                {
                    SolidColorBrush color = new(NewValue);
                    TextBlockButtonName.Foreground = color;
                    TextBlockButtonCommand.Foreground = color;
                    TextBlockNumberCommand.Foreground = color;
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
            get => BorderButton.Padding;
            set => BorderButton.Padding = value;
        }

        /// <summary>
        /// Текст кнопки
        /// </summary>
        public string Text
        {
            get => TextBlockButtonName.Text;
            set => TextBlockButtonName.Text = value;
        }

        /// <summary>
        /// Текст команды
        /// </summary>
        public string TextCommand
        {
            get => TextBlockButtonCommand.Text;
            set => TextBlockButtonCommand.Text = value;
        }

        private int _Index;
        /// <summary>
        /// Индекс элемента 
        /// </summary>
        public int Index
        {
            get => _Index;
            set
            {
                TextBlockNumberCommand.Text = $"#{value + 1}";
                _Index = value;
            }
        }

        /// <summary>
        /// Скругление границ кнопки (по умолчанию 10, 10, 10, 10)
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => BorderButton.CornerRadius;
            set => BorderButton.CornerRadius = value;
        }

        /// <summary>
        /// Шрифт текста в кнопке
        /// </summary>
        public FontFamily TextFontFamily
        {
            get => TextBlockButtonName.FontFamily;
            set => TextBlockButtonName.FontFamily = value;
        }

        /// <summary>
        /// Размер текста в кнопке
        /// </summary>
        public double TextFontSize
        {
            get => TextBlockButtonName.FontSize;
            set => TextBlockButtonName.FontSize = value;
        }

        /// <summary>
        /// Толщина границ
        /// </summary>
        public Thickness BorderThicknessBlock
        {
            get => BorderButton.BorderThickness;
            set => BorderButton.BorderThickness = value;
        }

        /// <summary>
        /// Активация действия кнопки
        /// </summary>
        private bool ButtonActivate = false;

        /// <summary>
        /// Объект события активации левым щелчком мыши
        /// </summary>
        public IIELButton.Activate? OnActivateMouseLeft { get; set; }

        /// <summary>
        /// Объект события активации правым щелчком мыши
        /// </summary>
        public IIELButton.Activate? OnActivateMouseRight { get; set; }

        public OPLButtonBufferCommand(string Name, string FullTextCommand, int indexBuffer)
        {
            InitializeComponent();
            IELSettingObject = new();
            TextFontFamily = new FontFamily("Arial");
            TextFontSize = 14;
            TextBlockButtonName.FontWeight = FontWeights.Bold;
            Text = Name;
            TextBlockButtonCommand.Text = FullTextCommand;
            CornerRadius = new CornerRadius(10);
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Top;
            Height = 27;
            Width = 230;
            BorderButton.CornerRadius = new CornerRadius(4);
            Index = indexBuffer;

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
                ButtonActivate = false;
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
                        ButtonActivate = true;
                        ClickDownAnimation();
                        IELSettingObject.StopHover();
                    }
                }
            };       

            MouseLeftButtonUp += (sender, e) =>
            {
                if (ButtonActivate)
                {
                    ButtonActivate = false;
                    MouseEnterAnimation();
                    OnActivateMouseLeft?.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (ButtonActivate)
                {
                    ButtonActivate = false;
                    MouseEnterAnimation();
                    OnActivateMouseRight?.Invoke(this, e);
                }
            };

            IsEnabledChanged += (sender, e) =>
            {
                bool NewValue = (bool)e.NewValue;
                Color
                    Foreground = NewValue ? IELSettingObject.ForegroundSetting.Default : IELSettingObject.ForegroundSetting.NotEnabled,
                    Background = NewValue ? IELSettingObject.BackgroundSetting.Default : IELSettingObject.BackgroundSetting.NotEnabled,
                    BorderBrush = NewValue ? IELSettingObject.BorderBrushSetting.Default : IELSettingObject.BorderBrushSetting.NotEnabled;
                BorderButton.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
                BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, null);
                TextBlockButtonName.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, null);
                TextBlockButtonCommand.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, null);
                BorderButton.BorderBrush = new SolidColorBrush(BorderBrush);
                BorderButton.Background = new SolidColorBrush(Background);
                TextBlockButtonName.Foreground = new SolidColorBrush(Foreground);
                TextBlockButtonCommand.Foreground = new SolidColorBrush(Foreground);
            };
        }

        /// <summary>
        /// Анимировать нажатие на кнопку (Down)
        /// </summary>
        private void ClickDownAnimation()
        {
            BeginVisualObjectAnimation(StateSpectrum.Used, false);
        }

        /// <summary>
        /// Анимация выделения кнопки мышью
        /// </summary>
        private void MouseEnterAnimation()
        {
            BeginVisualObjectAnimation(StateSpectrum.Select);
        }

        /// <summary>
        /// Анимация отключения выделения мышью
        /// </summary>
        private void MouseLeaveAnimation()
        {
            BeginVisualObjectAnimation(StateSpectrum.Default);
        }

        /// <summary>
        /// Создать анимацию изменения цвета объекта по определённому спекту
        /// </summary>
        /// <param name="spectrum">Изменяемый спектр</param>
        /// <param name="Animate">Состояние анимирования</param>
        private void BeginVisualObjectAnimation(StateSpectrum spectrum, bool Animate = true)
        {
            Color
                Foreground = IELSettingObject.ForegroundSetting.ColorData.GetIndexingColor(spectrum),
                Background = IELSettingObject.BackgroundSetting.ColorData.GetIndexingColor(spectrum),
                BorderBrush = IELSettingObject.BorderBrushSetting.ColorData.GetIndexingColor(spectrum);
            if (Animate)
            {
                TimeSpan span = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);

                App.AnimateColorEffect(BorderButton.Background, SolidColorBrush.ColorProperty, Background, span);

                App.AnimateColorEffect(BorderButton.BorderBrush, SolidColorBrush.ColorProperty, BorderBrush, span);

                App.AnimateColorEffect(TextBlockButtonName.Foreground, SolidColorBrush.ColorProperty, Foreground, span);
                App.AnimateColorEffect(TextBlockButtonCommand.Foreground, SolidColorBrush.ColorProperty, Foreground, span);
            }
            else
            {
                BorderButton.BorderBrush = new SolidColorBrush(BorderBrush);
                BorderButton.Background = new SolidColorBrush(Background);
                TextBlockButtonName.Foreground = new SolidColorBrush(Foreground);
                TextBlockButtonCommand.Foreground = new SolidColorBrush(Foreground);
            }
        }
    }
}
