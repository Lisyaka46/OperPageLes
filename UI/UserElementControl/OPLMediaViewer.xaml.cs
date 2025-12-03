using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl.Interfaces;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes.ObjectSettings;
using Newtonsoft.Json.Linq;
using System.Windows;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLViewerLoadingProcess.xaml
    /// </summary>
    public partial class OPLMediaViewer : IELButtonBase, IOPLObjectViewer<Uri>
    {
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

        #region Source
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(OPLMediaViewer),
                new(
                    (sender, e) =>
                    {
                        ((OPLMediaViewer)sender).IndicatorMedia.Source = (Uri)e.NewValue;
                    }));

        /// <summary>
        /// Данные пути к медиа загрузки объекта
        /// </summary>
        public Uri Source
        {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        #endregion

        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLMediaViewer),
                new("Text",
                    (sender, e) =>
                    {
                        ((OPLMediaViewer)sender).TextBlockName.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст отображаемый в элементе
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region ViewClose
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty ViewCloseProperty =
            DependencyProperty.Register("ViewClose", typeof(bool), typeof(OPLMediaViewer),
                new(true,
                    (sender, e) =>
                    {
                        ((OPLMediaViewer)sender).CancelIndicator.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
                    }));

        /// <summary>
        /// Состояние возможности отключения элемента
        /// </summary>
        public bool ViewClose
        {
            get => (bool)GetValue(ViewCloseProperty);
            set => SetValue(ViewCloseProperty, value);
        }
        #endregion

        #region ViewClose
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty ConnectedTokenProperty =
            DependencyProperty.Register("ConnectedToken", typeof(CancellationToken), typeof(OPLMediaViewer),
                new(
                    (sender, e) =>
                    {
                        ((OPLMediaViewer)sender).CancelIndicator.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
                    }));

        /// <summary>
        /// Состояние возможности отключения элемента
        /// </summary>
        public CancellationToken ConnectedToken
        {
            get => (CancellationToken)GetValue(ViewCloseProperty);
            set => SetValue(ViewCloseProperty, value);
        }
        #endregion

        public OPLMediaViewer()
        {
            InitializeComponent();
            #region BorderBrush
            CancelIndicator.BorderBrush = SourceBorderBrush.SourceBrush;
            BorderIndicator.BorderBrush = SourceBorderBrush.SourceBrush;
            #endregion

            #region Foreground
            TextBlockName.Foreground = SourceForeground.SourceBrush;
            TextBlockCancel.Foreground = SourceForeground.SourceBrush;
            #endregion

            TextBlockName.Text = "Text";
            CancelIndicator.Visibility = Visibility.Visible;

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
