using ApplicationOperPageLes;
using IEL.CORE.Classes;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;

namespace ApplicationOperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLButtonBufferCommand.xaml
    /// </summary>
    public partial class OPLButtonBufferCommand : System.Windows.Controls.UserControl, IIELButton
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
            #region Background
            _Background = new();
            BorderButton.Background = new SolidColorBrush(Background.ActiveSpectrumColor);
            Background.SetSpectrumAction((Args) =>
            {
                ((SolidColorBrush)BorderButton.Background).Color = Args.Value;
            });
            #endregion

            #region BorderBrush
            _BorderBrush = new();
            BorderButton.BorderBrush = new SolidColorBrush(BorderBrush.ActiveSpectrumColor);
            BorderBrush.SetSpectrumAction((Args) =>
            {
                ((SolidColorBrush)BorderButton.BorderBrush).Color = Args.Value;
            });
            #endregion

            #region Foreground
            _Foreground = new();
            TextBlockButtonName.Foreground = new SolidColorBrush(Foreground.ActiveSpectrumColor);
            TextBlockButtonCommand.Foreground = new SolidColorBrush(Foreground.ActiveSpectrumColor);
            TextBlockNumberCommand.Foreground = new SolidColorBrush(Foreground.ActiveSpectrumColor);
            Foreground.SetSpectrumAction((Args) =>
            {
                ((SolidColorBrush)TextBlockButtonName.Foreground).Color = Args.Value;
                ((SolidColorBrush)TextBlockButtonCommand.Foreground).Color = Args.Value;
                ((SolidColorBrush)TextBlockNumberCommand.Foreground).Color = Args.Value;
            });
            #endregion
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
                    Background.SetActiveSpecrum(StateSpectrum.Select, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Select, true);
                    IELSettingObject.StartHover();
                }
            };

            MouseLeave += (sender, e) =>
            {
                ButtonActivate = false;
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
                        ButtonActivate = true;
                        Background.SetActiveSpecrum(StateSpectrum.Used, false);
                        BorderBrush.SetActiveSpecrum(StateSpectrum.Used, false);
                        Foreground.SetActiveSpecrum(StateSpectrum.Used, false);
                        IELSettingObject.StopHover();
                    }
                }
            };

            MouseLeftButtonUp += (sender, e) =>
            {
                if (ButtonActivate)
                {
                    ButtonActivate = false;
                    Background.SetActiveSpecrum(StateSpectrum.Select, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Select, true);
                    OnActivateMouseLeft?.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (ButtonActivate)
                {
                    ButtonActivate = false;
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
    }
}
