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
        /// <summary>
        /// Данные пути к медиа загрузки объекта
        /// </summary>
        public ImageSource Source
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
            CancelIndicator.BorderBrush = SourceBorderBrush.SourceBrush;
            BorderIndicator.BorderBrush = SourceBorderBrush.SourceBrush;
            RectangleUp.Fill = SourceBorderBrush.SourceBrush;
            RectangleDown.Fill = SourceBorderBrush.SourceBrush;
            #endregion

            #region Foreground
            TextBlockName.Foreground = SourceForeground.SourceBrush;
            TextBlockCancel.Foreground = SourceForeground.SourceBrush;
            #endregion
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
