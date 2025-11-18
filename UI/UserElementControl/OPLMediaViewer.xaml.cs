using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl.Interfaces;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes.ObjectSettings;
using System.Windows;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLViewerLoadingProcess.xaml
    /// </summary>
    public partial class OPLMediaViewer : IELButton, IOPLObjectViewer<Uri>
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
        public Uri SourceView
        {
            get => IndicatorMedia.Source;
            set => IndicatorMedia.Source = value;
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

        private bool _IsCanceledManipulate;
        /// <summary>
        /// Состояние отмены загрузки
        /// </summary>
        internal bool IsCanceledManipulate
        {
            get => _IsCanceledManipulate;
            set
            {
                CancelIndicator.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                _IsCanceledManipulate = value;
            }
        }

        public OPLMediaViewer()
        {
            InitializeComponent();
            #region Background
            BorderMain.Background = SourceBackground.InicializeConnectedSolidColorBrush();
            #endregion

            #region BorderBrush
            BorderMain.BorderBrush = SourceBorderBrush.InicializeConnectedSolidColorBrush();
            CancelIndicator.BorderBrush = SourceBorderBrush.InicializeConnectedSolidColorBrush();
            #endregion

            #region Foreground
            TextBlockName.Foreground = SourceForeground.InicializeConnectedSolidColorBrush();
            TextBlockCancel.Foreground = SourceForeground.InicializeConnectedSolidColorBrush();
            #endregion
            IsCanceledManipulate = true;
            IndicatorMedia.MediaEnded += (sender, e) =>
            {
                IndicatorMedia.Position = TimeSpan.FromMilliseconds(1);
            };
            IndicatorMedia.Opacity = 0d;
            IndicatorMedia.Source = new(StructDirectoryResources.GetResourcePath(nameof(OPRES.MediaLoadingDefault)));
            IsEnabled = false;
        }

        /// <summary>
        /// Визуализировать выключение элемента
        /// </summary>
        internal void VisualClose()
        {
            IsEnabled = false;
            App.DoubleAnimationType.AnimateEffect(IndicatorMedia, OpacityProperty, 0d, TimeSpan.FromMilliseconds(1300d));
        }

        /// <summary>
        /// Визуализировать включение элемента
        /// </summary>
        internal void VisualOpen()
        {
            IsEnabled = true;
            App.DoubleAnimationType.AnimateEffect(IndicatorMedia, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
        }
    }
}
