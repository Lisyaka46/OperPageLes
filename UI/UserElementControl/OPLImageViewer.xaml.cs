using ApplicationOperPageLes.UI.UserElementControl.Interfaces;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes.ObjectSettings;
using System.Windows.Media;

namespace ApplicationOperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLViewerLoadingProcess.xaml
    /// </summary>
    public partial class OPLImageViewer : IELButtonBase, IOPLObjectViewer<ImageSource>
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
        /// Данные пути к медиа загрузки объекта
        /// </summary>
        public ImageSource SourceView
        {
            get => IndicatorImage.Source;
            set => IndicatorImage.Source = value;
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

        public OPLImageViewer()
        {
            InitializeComponent();

            #region BorderBrush
            CancelIndicator.BorderBrush = SourceBorderBrush.InicializeConnectedSolidColorBrush();
            #endregion

            #region Foreground
            TextBlockName.Foreground = SourceForeground.InicializeConnectedSolidColorBrush();
            TextBlockCancel.Foreground = SourceForeground.InicializeConnectedSolidColorBrush();
            #endregion
            IndicatorImage.Opacity = 0d;
            IsEnabled = true;
        }

        /// <summary>
        /// Визуализировать выключение элемента
        /// </summary>
        internal void VisualClose()
        {
            IsEnabled = false;
            App.DoubleAnimationType.AnimateEffect(IndicatorImage, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1300d));
        }

        /// <summary>
        /// Визуализировать включение элемента
        /// </summary>
        internal void VisualOpen()
        {
            IsEnabled = true;
            App.DoubleAnimationType.AnimateEffect(IndicatorImage, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
        }
    }
}
