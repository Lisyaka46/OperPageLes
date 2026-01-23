using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementsControl.Base;
using ApplicationOperPageLes.UI.UserElementsControl.Interfaces;
using IEL.CORE.Classes.ObjectSettings;
using IEL.UserElementsControl.Base;
using Newtonsoft.Json.Linq;
using System.Windows;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLViewerLoadingProcess.xaml
    /// </summary>
    public partial class OPLMediaViewer : OPLElementViewerBase, IOPLObjectViewer<Uri>
    {
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

        public OPLMediaViewer()
        {
            InitializeComponent();

            IndicatorMedia.MediaEnded += (sender, e) =>
            {
                IndicatorMedia.Position = TimeSpan.FromMilliseconds(1);
            };
            IndicatorMedia.Opacity = 0d;
            IndicatorMedia.Source = StructDirectoryResources.GetResourceUri(nameof(OPRES.MediaLoadingDefault));
        }
    }
}
