using ApplicationOperPageLes.UI.UserElementsControl.Base;
using ApplicationOperPageLes.UI.UserElementsControl.Interfaces;
using System.Windows.Media;

namespace ApplicationOperPageLes.UI.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLViewerLoadingProcess.xaml
    /// </summary>
    public partial class OPLImageViewer : OPLElementViewerBase, IOPLObjectViewer<ImageSource>
    {
        /// <summary>
        /// Данные пути к медиа загрузки объекта
        /// </summary>
        public ImageSource Source
        {
            get => IndicatorImage.Source;
            set => IndicatorImage.Source = value;
        }

        public OPLImageViewer()
        {
            InitializeComponent();
        }
    }
}
