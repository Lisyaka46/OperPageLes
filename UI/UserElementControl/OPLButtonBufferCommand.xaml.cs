using IEL.CORE.BaseUserControls;
using IEL.CORE.BaseUserControls.Interfaces;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using System.Windows;
using System.Windows.Input;
using FontFamily = System.Windows.Media.FontFamily;

namespace ApplicationOperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLButtonBufferCommand.xaml
    /// </summary>
    public partial class OPLButtonBufferCommand : IELButton, IVisualIELButton
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
                _IELSettingObject = value;
            }
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

        #region IVisualIELButton
        /// <summary>
        /// Скругление границ
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => BorderButton.CornerRadius;
            set => BorderButton.CornerRadius = value;
        }

        /// <summary>
        /// Толщина границ
        /// </summary>
        public new Thickness BorderThickness
        {
            get => BorderButton.BorderThickness;
            set => BorderButton.BorderThickness = value;
        }

        /// <summary>
        /// Смещение контента в объекте
        /// </summary>
        public Thickness PaddingContent
        {
            get => BorderButton.Padding;
            set => BorderButton.Padding = value;
        }
        #endregion

        public OPLButtonBufferCommand(string Name, string FullTextCommand, int indexBuffer)
        {
            InitializeComponent();
            #region Background
            BorderButton.Background = QBackground.InicializeConnectedSolidColorBrush();
            #endregion

            #region BorderBrush
            BorderButton.BorderBrush = QBorderBrush.InicializeConnectedSolidColorBrush();
            #endregion

            #region Foreground
            TextBlockButtonName.Foreground = QForeground.InicializeConnectedSolidColorBrush();
            TextBlockButtonCommand.Foreground = QForeground.InicializeConnectedSolidColorBrush();
            TextBlockNumberCommand.Foreground = QForeground.InicializeConnectedSolidColorBrush();
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
                    SetActiveSpecrum(StateSpectrum.Select, true);
                    IELSettingObject.StartHover();
                }
            };

            MouseLeave += (sender, e) =>
            {
                if (IsEnabled)
                {
                    SetActiveSpecrum(StateSpectrum.Default, true);
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
                        SetActiveSpecrum(StateSpectrum.Used, false);
                        IELSettingObject.StopHover();
                    }
                }
            };

            MouseLeftButtonUp += (sender, e) =>
            {
                if (OnActivateMouseLeft != null)
                {
                    SetActiveSpecrum(StateSpectrum.Select, true);
                    OnActivateMouseLeft.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (OnActivateMouseRight != null)
                {
                    SetActiveSpecrum(StateSpectrum.Select, true);
                    OnActivateMouseRight.Invoke(this, e);
                }
            };

            IsEnabledChanged += (sender, e) =>
            {
                StateSpectrum NewValue = (bool)e.NewValue ? StateSpectrum.Default : StateSpectrum.NotEnabled;
                SetActiveSpecrum(NewValue, true);
            };
        }
    }
}
