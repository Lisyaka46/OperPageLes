using ApplicationOperPageLes.CORE.Label;
using ApplicationOperPageLes.CORE.Struct;
using IEL.CORE.Classes;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using InterpreterCommand.Classes;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OPRES = ApplicationOperPageLes.Properties.Resources;
using IEL.UserElementsControl.Base;

namespace ApplicationOperPageLes.UI.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLLabel.xaml
    /// </summary>
    public partial class OPLLabel : IELButtonBase
    {
        ///// <summary>
        ///// Объект события активации кнопки левым щелчком мыши
        ///// </summary>
        //public IIELButton.ActivateHandler? OnActivateMouseLeft { get; set; }

        ///// <summary>
        ///// Объект события активации кнопки правым щелчком мыши
        ///// </summary>
        //public IIELButton.ActivateHandler? OnActivateMouseRight { get; set; }

        /// <summary>
        /// Данные изображения объекта
        /// </summary>
        public ImageSource ImageSource
        {
            get => ImageElementLabel.Source;
            set => ImageElementLabel.Source = value;
        }

        /// <summary>
        /// Элемент ярлыка
        /// </summary>
        internal LabelAction SourceLabel { get; set; }

        private bool _Selected;
        /// <summary>
        /// Состояние выделенного элемента
        /// </summary>
        internal bool Selected
        {
            get => _Selected;
            set
            {
                App.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, value ? 1d : 0d, TimeSpan.FromMilliseconds(200d));
                _Selected = value;
            }
        }

        public OPLLabel(LabelAction Label)
        {
            InitializeComponent();
            #region Background
            BorderSelectElement.Background = SourceBackground.SourceBrush;
            #endregion

            #region BorderBrush
            #endregion

            #region Foreground
            TextBlockNameLabel.Foreground = SourceForeground.SourceBrush;
            TextBlockNumberSelect.Foreground = SourceForeground.SourceBrush;
            #endregion
            _Selected = false;
            BorderSelectElement.Opacity = 0d;
            SourceLabel = Label;
            SourceLabel.SetTag += (Old, New) =>
            {
            };
            SourceLabel.DeleteTag += (Old, New) =>
            {
            };
            UpdateVisualStyle();

            TextBlockNameLabel.Text = Label.Name;
        }

        /// <summary>
        /// Обновить изображение стиля команды
        /// </summary>
        internal void UpdateVisualStyle()
        {
            string name_command = COMInterpreter.ReadNameCommand(SourceLabel.Command);
            string NameLabelImage;
            switch (name_command)
            {
                case "open_file":
                    App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Gray].ConnectPalleteFromIELElement(this);
                    NameLabelImage = nameof(OPRES.File);
                    break;
                case "open_link":
                    App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(this);
                    NameLabelImage = nameof(OPRES.Link);

                    Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
                    {
                        ImageFaviconLabel.Source = await App.DownloadFavicon(new Uri(COMInterpreter.ReadParametersCommand(SourceLabel.Command)[0]));
                        ImageFaviconLabel.Width = 20;
                        ImageFaviconLabel.Height = 20;
                        ImageFaviconLabel.Opacity = 0d;

                        App.DoubleAnimationType.AnimateEffect(ImageFaviconLabel, OpacityProperty, 1d, TimeSpan.FromMilliseconds(900d));
                        App.DoubleAnimationType.AnimateEffect(ImageFaviconLabel, WidthProperty, 40d, TimeSpan.FromMilliseconds(1100d));
                        App.DoubleAnimationType.AnimateEffect(ImageFaviconLabel, HeightProperty, 40d, TimeSpan.FromMilliseconds(1100d));
                    });
                    //while (worker.IsBusy) System.Windows.Forms.Application.DoEvents();
                    //action.Invoke();
                    break;
                case "open_directory":
                    App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.LightBlue].ConnectPalleteFromIELElement(this);
                    NameLabelImage = nameof(OPRES.Folder);
                    break;
                default:
                    App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(this);
                    NameLabelImage = nameof(OPRES.Command);
                    break;
            }
            ImageElementLabel.Source = StructDirectoryResources.GetResourceBitmap(NameLabelImage);
            ImageElementLabel.UpdateLayout();
        }

        /// <summary>
        /// Создать визуальный элемент тега
        /// </summary>
        /// <param name="value">Значение отображаемое тега</param>
        /// <returns></returns>
        internal static OPLLabelTag CreateVisualTag(LabelTag NewTag)
        {
            return new()
            {
                BorderThickness = new(1),
                Text = string.Empty,
                Padding = new(4, 2, 4, 2),
                FontSize = 16d,
                Tag = NewTag,
            };
        }
    }
}
