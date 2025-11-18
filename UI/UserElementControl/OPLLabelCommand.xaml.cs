using ApplicationOperPageLes.CORE.Label;
using ApplicationOperPageLes.CORE.Struct;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using InterpreterCommand.Classes;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLLabelCommand.xaml
    /// </summary>
    public partial class OPLLabelCommand : IELButton
    {
        #region Styles
        internal static readonly QData[] BackgroundStyles =
        [
            new(
                        [
                        [255, 116, 220, 80],
                        [255, 180, 255, 154],
                        [255, 196, 239, 201],
                        [255, 222, 87, 87],
                        ]),
            new(
                        [
                        [255, 155, 179, 169],
                        [255, 160, 200, 175],
                        [255, 221, 254, 241],
                        [255, 111, 127, 121],
                        ]),
            new(
                        [
                        [255, 239, 250, 195],
                        [255, 240, 246, 210],
                        [255, 195, 218, 250],
                        [255, 250, 201, 195],
                        ]),
            new(
                        [
                        [255, 85, 150, 181],
                        [255, 90, 140, 185],
                        [255, 181, 137, 85],
                        [255, 84, 107, 117],
                        ]),
        ];
        internal static readonly QData[] Borderbrush_Foreground_Styles =
        [
            new(
                        [
                        [255, 0, 0, 0],
                        [255, 19, 35, 12],
                        [255, 47, 44, 9],
                        [255, 58, 8, 8],
                        ]),
            new(
                        [
                        [255, 62, 96, 82],
                        [255, 65, 100, 85],
                        [255, 100, 154, 133],
                        [255, 38, 68, 57],
                        ]),
            new(
                        [
                        [255, 126, 139, 73],
                        [255, 130, 150, 69],
                        [255, 79, 110, 152],
                        [255, 153, 100, 94],
                        ]),
            new(
                        [
                        [255, 24, 86, 116],
                        [255, 30, 83, 107],
                        [255, 109, 72, 28],
                        [255, 24, 47, 56],
                        ]),
        ];
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
                App.DoubleAnimationType.AnimateEffect(BorderSelectElement, OpacityProperty, value ? 1d : 0d, TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond));
                _Selected = value;
            }
        }

        public OPLLabelCommand(LabelAction Label)
        {
            InitializeComponent();
            #region Background
            BorderMainLabel.Background = SourceBackground.InicializeConnectedSolidColorBrush();
            #endregion

            #region BorderBrush
            BorderMainLabel.BorderBrush = SourceBorderBrush.InicializeConnectedSolidColorBrush();
            #endregion

            #region Foreground
            TextBlockNameLabel.Foreground = SourceForeground.InicializeConnectedSolidColorBrush();
            #endregion
            IELSettingObject = new();
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
            int IndexUseStyle;
            switch (name_command)
            {
                case "open_file":
                    IndexUseStyle = 1;
                    NameLabelImage = nameof(OPRES.File);
                    break;
                case "open_link":
                    IndexUseStyle = 2;
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
                    IndexUseStyle = 3;
                    NameLabelImage = nameof(OPRES.Folder);
                    break;
                default:
                    IndexUseStyle = 0;
                    NameLabelImage = nameof(OPRES.Command);
                    break;
            }
            ImageElementLabel.Source = StructDirectoryResources.GetResourceBitmap(NameLabelImage);
            ImageElementLabel.UpdateLayout();
            Background = BackgroundStyles[IndexUseStyle];
            BorderBrush = Borderbrush_Foreground_Styles[IndexUseStyle];
            Foreground = Borderbrush_Foreground_Styles[IndexUseStyle];
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
                CornerRadius = new(5),
                Text = string.Empty,
                Padding = new(4, 2, 4, 2),
                FontSize = 16d,
                Tag = NewTag,
            };
        }
    }
}
