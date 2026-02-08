using ApplicationOperPageLes.CORE.Label;
using ApplicationOperPageLes.CORE.Struct;
using IEL.CORE.Classes;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.UserElementsControl.Base;
using InterpreterCommand.Classes;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLLabel.xaml
    /// </summary>
    public partial class OPLLabel : IELObjectBase
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

        /// <summary>
        /// Состояние выделенного элемента
        /// </summary>
        internal bool Selected { get; private set; }

        public OPLLabel(LabelAction Label)
        {
            InitializeComponent();
            #region Background
            BorderSelectElement.Background = SourceBackground.SourceBrush;
            BorderMain.Background = SourceBackground.SourceBrush;
            #endregion

            #region BorderBrush
            BorderMain.BorderBrush = SourceBorderBrush.SourceBrush;
            #endregion

            #region Foreground
            TextBlockNameLabel.Foreground = SourceForeground.SourceBrush;
            TextBlockNumberSelect.Foreground = SourceForeground.SourceBrush;
            #endregion

            MouseEnter += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Select, true);
            };
            MouseLeave += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Default, true);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Default, true);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Default, true);
            };
            MouseDown += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Used, false);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Used, false);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Used, false);
            };
            MouseLeftButtonUp += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Select, true);
            };
            MouseRightButtonUp += (sender, e) =>
            {
                SourceBackground.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                SourceForeground.SetActiveSpecrum(StateSpectrum.Select, true);
            };

            Selected = false;
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
        /// Включить состояние выделения ярлыка
        /// </summary>
        /// <param name="index">Отображаемый индекс</param>
        public void SelectOn()
        {
            App.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
            Selected = true;
            ImageSelect.Margin = new(0, 5, 0, 5);
            TextBlockNumberSelect.Text = string.Empty;
        }

        /// <summary>
        /// Включить состояние выделения ярлыка
        /// </summary>
        /// <param name="ListSource">Массив куда записывается выделяемый ярлык</param>
        internal void SelectOn(ref List<OPLLabel> ListSource)
        {
            ListSource.Add(this);
            App.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
            Selected = true;
            ImageSelect.Margin = new(0, 0, 0, 10);
            TextBlockNumberSelect.Text = ListSource.Count.ToString();
        }

        /// <summary>
        /// Выключить состояние выделения ярлыка
        /// </summary>
        public void SelectOff()
        {
            App.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, 0d, TimeSpan.FromMilliseconds(200d));
            Selected = false;
            ImageSelect.Margin = new(0, 5, 0, 5);
            TextBlockNumberSelect.Text = string.Empty;
        }

        /// <summary>
        /// Выключить состояние выделения ярлыка
        /// </summary>
        /// <param name="ListSource">Массив куда записывается выделяемый ярлык</param>
        internal void SelectOff(ref List<OPLLabel> ListSource)
        {
            ListSource.Remove(this);
            App.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, 0d, TimeSpan.FromMilliseconds(200d));
            Selected = false;
            ImageSelect.Margin = new(0, 5, 0, 5);
            TextBlockNumberSelect.Text = string.Empty;
        }

        /// <summary>
        /// Обновить изображение стиля команды
        /// </summary>
        internal void UpdateVisualStyle()
        {
            string name_command = COMInterpreterBase.ReadNameCommand(SourceLabel.Command);
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
                        ImageFaviconLabel.Source = await App.DownloadFavicon(new Uri(COMInterpreterBase.ReadParametersCommand(SourceLabel.Command)[0]));
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
