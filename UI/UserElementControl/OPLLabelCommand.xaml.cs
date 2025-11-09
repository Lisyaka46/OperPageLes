using ApplicationOperPageLes.CORE.Label;
using ApplicationOperPageLes.CORE.Struct;
using IEL.CORE.Classes;
using IEL.CORE.Classes.ObjectSettings;
using IEL.CORE.Enums;
using IEL.Interfaces.Front;
using InterpreterCommand.Classes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLLabelCommand.xaml
    /// </summary>
    public partial class OPLLabelCommand : System.Windows.Controls.UserControl, IIELButton
    {
        #region Styles
        internal static readonly QData[] BackgroundStyles =
        [
            new(new byte[,]
                        {
                        { 255, 116, 220, 80 },
                        { 255, 180, 255, 154 },
                        { 255, 196, 239, 201 },
                        { 255, 222, 87, 87 },
                        }),
            new(new byte[,]
                        {
                        { 255, 155, 179, 169 },
                        { 255, 160, 200, 175 },
                        { 255, 221, 254, 241 },
                        { 255, 111, 127, 121 },
                        }),
            new(new byte[,]
                        {
                        { 255, 239, 250, 195 },
                        { 255, 240, 246, 210 },
                        { 255, 195, 218, 250 },
                        { 255, 250, 201, 195 },
                        }),
            new(new byte[,]
                        {
                        { 255, 85, 150, 181 },
                        { 255, 90, 140, 185 },
                        { 255, 181, 137, 85 },
                        { 255, 84, 107, 117 },
                        }),
        ];
        internal static readonly QData[] Borderbrush_Foreground_Styles =
        [
            new(new byte[,]
                        {
                        { 255, 0, 0, 0 },
                        { 255, 19, 35, 12 },
                        { 255, 47, 44, 9 },
                        { 255, 58, 8, 8 },
                        }),
            new(new byte[,]
                        {
                        { 255, 62, 96, 82 },
                        { 255, 65, 100, 85 },
                        { 255, 100, 154, 133 },
                        { 255, 38, 68, 57 },
                        }),
            new(new byte[,]
                        {
                        { 255, 126, 139, 73 },
                        { 255, 130, 150, 69 },
                        { 255, 79, 110, 152 },
                        { 255, 153, 100, 94 },
                        }),
            new(new byte[,]
                        {
                        { 255, 24, 86, 116 },
                        { 255, 30, 83, 107 },
                        { 255, 109, 72, 28 },
                        { 255, 24, 47, 56 },
                        }),
        ];
        #endregion

        #region Color Setting
        /// <summary>
        /// Ресурсный объект настройки состояний фона
        /// </summary>
        public BrushSettingQ _Background;
        /// <summary>
        /// Объект настройки состояний фона
        /// </summary>
        public new BrushSettingQ Background
        {
            get => _Foreground;
            set => _Foreground.ColorData = value.ColorData;
        }

        /// <summary>
        /// Ресурсный объект настройки состояний границы
        /// </summary>
        public BrushSettingQ _BorderBrush;
        /// <summary>
        /// Объект настройки состояний границы
        /// </summary>
        public new BrushSettingQ BorderBrush
        {
            get => _Foreground;
            set => _Foreground.ColorData = value.ColorData;
        }

        /// <summary>
        /// Ресурсный объект настройки состояний текста
        /// </summary>
        public BrushSettingQ _Foreground;
        /// <summary>
        /// Объект настройки состояний текста
        /// </summary>
        public new BrushSettingQ Foreground
        {
            get => _Foreground;
            set => _Foreground.ColorData = value.ColorData;
        }
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

        /// <summary>
        /// Смещение контента в объекте
        /// </summary>
        public Thickness PaddingContent
        {
            get => Padding;
            set => Padding = value;
        }

        /// <summary>
        /// Объект события активации кнопки левым щелчком мыши
        /// </summary>
        public IIELButton.ActivateHandler? OnActivateMouseLeft { get; set; }

        /// <summary>
        /// Объект события активации кнопки правым щелчком мыши
        /// </summary>
        public IIELButton.ActivateHandler? OnActivateMouseRight { get; set; }

        /// <summary>
        /// Данные изображения объекта
        /// </summary>
        public ImageSource ImageSource
        {
            get => ImageElementLabel.Source;
            set => ImageElementLabel.Source = value;
        }

        /// <summary>
        /// Скругление границ
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => BorderMainLabel.CornerRadius;
            set
            {
                BorderMainLabel.CornerRadius = value;
            }
        }

        /// <summary>
        /// Толщина границ
        /// </summary>
        public Thickness BorderThicknessBlock
        {
            get => BorderMainLabel.BorderThickness;
            set
            {
                BorderMainLabel.BorderThickness = value;
            }
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
            _Background = new();
            BorderMainLabel.Background = new SolidColorBrush(Background.ActiveSpectrumColor);
            Background.SetSpectrumAction((Args) =>
            {
                if (Args.AnimatedEvent)
                {
                    App.ColorAnimationType.AnimateEffect(BorderMainLabel.Background,
                        SolidColorBrush.ColorProperty, Args.Value, TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond));
                }
                else
                {
                    ((SolidColorBrush)BorderMainLabel.Background).Color = Args.Value;
                }
            });
            #endregion

            #region BorderBrush
            _BorderBrush = new();
            BorderMainLabel.BorderBrush = new SolidColorBrush(BorderBrush.ActiveSpectrumColor);
            BorderBrush.SetSpectrumAction((Args) =>
            {
                if (Args.AnimatedEvent)
                {
                    App.ColorAnimationType.AnimateEffect(BorderMainLabel.BorderBrush,
                        SolidColorBrush.ColorProperty, Args.Value, TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond));
                }
                else
                {
                    ((SolidColorBrush)BorderMainLabel.BorderBrush).Color = Args.Value;
                }
            });
            #endregion

            #region Foreground
            _Foreground = new();
            TextBlockNameLabel.Foreground = new SolidColorBrush(Foreground.ActiveSpectrumColor);
            Foreground.SetSpectrumAction((Args) =>
            {
                if (Args.AnimatedEvent)
                {
                    App.ColorAnimationType.AnimateEffect(TextBlockNameLabel.Foreground,
                        SolidColorBrush.ColorProperty, Args.Value, TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond));
                }
                else
                {
                    ((SolidColorBrush)TextBlockNameLabel.Foreground).Color = Args.Value;
                }
            });
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

            IsEnabledChanged += (sender, e) =>
            {
                TimeSpan span = TimeSpan.FromMilliseconds(IELSettingObject.AnimationMillisecond);
                StateSpectrum NewValue = (bool)e.NewValue ? StateSpectrum.Default : StateSpectrum.NotEnabled;
                Background.SetActiveSpecrum(NewValue, true);
                BorderBrush.SetActiveSpecrum(NewValue, true);
                Foreground.SetActiveSpecrum(NewValue, true);
            };

            MouseEnter += (sender, e) =>
            {
                if (IsEnabled)
                {
                    Background.SetActiveSpecrum(StateSpectrum.Select, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Select, true);
                    IELSettingObject.StartHover();
                }
            };

            MouseLeave += (sender, e) =>
            {
                if (IsEnabled)
                {
                    Background.SetActiveSpecrum(StateSpectrum.Default, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Default, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Default, true);
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
                        Background.SetActiveSpecrum(StateSpectrum.Used, false);
                        BorderBrush.SetActiveSpecrum(StateSpectrum.Used, false);
                        Foreground.SetActiveSpecrum(StateSpectrum.Used, false);
                        IELSettingObject.StopHover();
                    }
                }
            };

            MouseLeftButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseLeft != null)
                {
                    Background.SetActiveSpecrum(StateSpectrum.Select, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Select, true);
                    OnActivateMouseLeft?.Invoke(this, e);
                }
            };

            MouseRightButtonUp += (sender, e) =>
            {
                if (IsEnabled && OnActivateMouseRight != null)
                {
                    Background.SetActiveSpecrum(StateSpectrum.Select, true);
                    BorderBrush.SetActiveSpecrum(StateSpectrum.Select, true);
                    Foreground.SetActiveSpecrum(StateSpectrum.Select, true);
                    OnActivateMouseRight?.Invoke(this, e);
                }
            };
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
            Background.ColorData = (QData)BackgroundStyles[IndexUseStyle].Clone();
            BorderBrush.ColorData = (QData)Borderbrush_Foreground_Styles[IndexUseStyle].Clone();
            Foreground.ColorData = (QData)Borderbrush_Foreground_Styles[IndexUseStyle].Clone();
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
                BorderThicknessBlock = new(1),
                CornerRadius = new(5),
                Text = string.Empty,
                PaddingContent = new(4, 2, 4, 2),
                FontSize = 16d,
                Tag = NewTag,
                IELSettingObject = new()
                {
                    AnimationMillisecond = 200d,
                },
            };
        }
    }
}
