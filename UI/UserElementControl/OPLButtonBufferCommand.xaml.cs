using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using OperPageLes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;

namespace OperPageLes.UI.UserElementControl
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
                value.BackgroundSetting.SetActionColorChanged((Spectrum, NewValue, Animated) =>
                {
                    SolidColorBrush color = new(NewValue);
                    BorderButton.Background = color;
                });
                value.BorderBrushSetting.SetActionColorChanged((Spectrum, NewValue, Animated) =>
                {
                    SolidColorBrush color = new(NewValue);
                    BorderButton.BorderBrush = color;
                });
                value.ForegroundSetting.SetActionColorChanged((Spectrum, NewValue, Animated) =>
                {
                    SolidColorBrush color = new(NewValue);
                    TextBlockButtonName.Foreground = color;
                    TextBlockButtonCommand.Foreground = color;
                    TextBlockNumberCommand.Foreground = color;
                });
                _IELSettingObject = value;
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
        public IIELButton.ActivateHandler? OnActivateMouseLeft { get; set; }

        /// <summary>
        /// Объект события активации правым щелчком мыши
        /// </summary>
        public IIELButton.ActivateHandler? OnActivateMouseRight { get; set; }

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
                    IELSettingObject.UseActiveQSetting(StateSpectrum.Select);
                    IELSettingObject.StartHover();
                }
            };

            MouseLeave += (sender, e) =>
            {
                ButtonActivate = false;
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
                        ButtonActivate = true;
                        IELSettingObject.UseActiveQSetting(StateSpectrum.Used, false);
                        IELSettingObject.StopHover();
                    }
                }
            };

            MouseLeftButtonUp += (sender, e) =>
            {
                if (ButtonActivate)
                {
                    ButtonActivate = false;
                    IELSettingObject.UseActiveQSetting(StateSpectrum.Select);
                    OnActivateMouseLeft?.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (ButtonActivate)
                {
                    ButtonActivate = false;
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
    }
}
