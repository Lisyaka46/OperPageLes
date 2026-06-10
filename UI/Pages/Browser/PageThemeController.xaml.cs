using IEL.CORE.Classes;
using IEL.CORE.Enums;
using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OPLAPI.OIEL.CORE.Browser;
using OperPageLes.CORE;
using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Settings.PaletteElements;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.ActionPanel.PaletteWindow;
using OperPageLes.UI.UserElementsControl.Theme;
using OperPageLes.UI.Windows.Dialogs;
using OPLAnimation.CORE.Animation;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static IEL.CORE.Classes.QData;
using OPRES = OperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;

namespace OperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageThemeController.xaml
    /// </summary>
    public partial class PageThemeController : PageBrowser, IOPLConnectElements
    {
        /// <summary>
        /// Объект панели действий подключаемый к элементу отображения OPL
        /// </summary>
        public IELPanelAction? SourcePanelAction { get; internal set; }

        /// <summary>
        /// Цвет индикатора активной темы
        /// </summary>
        private WnColor ActiveThemeColor = WnColor.FromArgb(255, 0, 255, 255);

        /// <summary>
        /// Активная палитра взаимодействия
        /// </summary>
        private Theme ActiveManipulateTheme = new();

        /// <summary>
        /// Активный объект спектра палитры над которым производится манипуляция
        /// </summary>
        private PaletteSpectrum? ActiveManipulateSpectrum;

        /// <summary>
        /// Страница управлением темой в панели действий
        /// </summary>
        private readonly PageMainPalettePanelAction PanelActionPagePalette = new();

        /// <summary>
        /// Массив инициализированных файлов
        /// </summary>
        private List<string> ArrayInicializeFilesTheme = [];

        /// <summary>
        /// Массив всех кистей отображения спектров QData
        /// </summary>
        private readonly SolidColorBrush[] DSUNE_ArrayBrush;

        /// <summary>
        /// Активный индекс визуализируемого спектра
        /// </summary>
        private int IndexActivateVisualizateSpectrum = -1;

        /// <summary>
        /// Индекс используемой темы в приложении
        /// </summary>
        private int ActiveThemeInApplicationIndex = -1;

        /// <summary>
        /// Индекс выделенной темы панелью действий
        /// </summary>
        private int SelectIndexTheme = -1;

        /// <summary>
        /// Контейнер всех объектов тем
        /// </summary>
        private StackPanel StackPanelThemes;

        /// <summary>
        /// Контейнер всех объектов спектров темы
        /// </summary>
        private StackPanel StackPanelSpectrum;

        public PageThemeController()
        {
            InitializeComponent();

            DSUNE_ArrayBrush =
            [
                new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.White),
                new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Black),
                new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.White),
            ];

            BorderD.Background = DSUNE_ArrayBrush[0];
            BorderS.Background = DSUNE_ArrayBrush[1];
            BorderU.Background = DSUNE_ArrayBrush[2];
            BorderNE.Background = DSUNE_ArrayBrush[3];

            BorderD.BorderBrush = DSUNE_ArrayBrush[4];
            BorderS.BorderBrush = DSUNE_ArrayBrush[5];
            BorderU.BorderBrush = DSUNE_ArrayBrush[6];
            BorderNE.BorderBrush = DSUNE_ArrayBrush[7];

            TextBlockD.Foreground = DSUNE_ArrayBrush[8];
            TextBlockS.Foreground = DSUNE_ArrayBrush[9];
            TextBlockU.Foreground = DSUNE_ArrayBrush[10];
            TextBlockNE.Foreground = DSUNE_ArrayBrush[11];

            CheckBoxEnabledExampleButtonPalette.IsChecked = true;
            //IELButtonSaveTheme.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Save));
            //IELButtonSaveTheme.IsEnabled = false;
            //IELButtonSaveTheme.OnActivateMouseLeft += (sender, e) =>
            //{
            //    if (ActiveManipulateTheme != null)
            //    {
            //        FileStream stream = File.OpenWrite(ActiveManipulateTheme.DirectoryFile);
            //        stream.Position = 0;
            //        foreach (PaletteSpectrumEnum Element in Enum.GetValues<PaletteSpectrumEnum>())
            //        {
            //            PaletteSpectrum spectrum = ActiveManipulateTheme[Element];
            //            PaletteSpectrum.WritePalettespectrum(ref stream, ref spectrum);
            //        }
            //        stream.Close();
            //    }
            //    IELButtonSaveTheme.IsEnabled = false;
            //};

            GridPaletteSpectrumViewer.IsEnabled = false;
            GridPaletteSpectrumViewer.Opacity = 0d;

            GridWiewButtonQData.Opacity = 0d;
            GridQdataStatesColor.Opacity = 0d;
            BorderViewerQData.IsEnabled = false;

            StackPanelSpectrum = new()
            {
                VerticalAlignment = VerticalAlignment.Top,
            };
            ScrollViewerSpectrum.ScrollForce = 35;
            ScrollViewerTheme.AutoUpdateVisibleHorizontalScroll = false;
            ScrollViewerSpectrum.Content = StackPanelSpectrum;

            StackPanelThemes = new()
            {
                VerticalAlignment = VerticalAlignment.Top,
            };
            ScrollViewerTheme.Opacity = 0d;
            ScrollViewerTheme.ScrollForce = 35;
            ScrollViewerTheme.AutoUpdateVisibleHorizontalScroll = false;
            ScrollViewerTheme.Content = StackPanelThemes;

            DefaultPaletteElement.ManagerAnimation = ManagerAnimation;
            DefaultPaletteElement.IsActivate = App.CurrentApp.SettingMainApplication.ThemeInstallName.Value.Length == 0;
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Chocolate].ConnectPalleteFromIELElement(DefaultPaletteElement);
            DefaultPaletteElement.SourceElement = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Palette));
            DefaultPaletteElement.MouseRightButtonUp += (sender, e) =>
            {
                ActivatePanelActionThemeSelect(DefaultPaletteElement, -1);
            };

            IELCreateNewTheme.OnActivateMouseLeft += (sender, e) =>
            {
                DialogGenerateTheme Dialog = new();
                Theme? ResultTheme = Dialog.ShowDialogCreateNewTheme([..ArrayInicializeFilesTheme]);
                if (ResultTheme == null) return;

                ArrayInicializeFilesTheme.Add(ResultTheme.DirectoryFile);
                OPLThemeFile button = CreateButtonTheme();
                button.TextNameFile = ResultTheme.Name;
                StackPanelThemes.Children.Add(button);
            };

            IELButtonBackViewTheme.OnActivateMouseLeft += (sender, e) =>
            {
                DiactivateSelectTheme();
            };

            CheckBoxEnabledExampleButtonPalette.Checked += (sender, e) =>
            {
                IELExampleButtonPalette.IsEnabled = true;
            };
            CheckBoxEnabledExampleButtonPalette.Unchecked += (sender, e) =>
            {
                IELExampleButtonPalette.IsEnabled = false;
            };

            BorderD.MouseLeftButtonUp += (sender, e) =>
            {
                if (ActiveManipulateSpectrum != null && ActiveManipulateTheme.DirectoryFile.Length > 0)
                    ActivateDialogManipulatePaletteSectrum(EnumDataSpectrum.Default);
            };
            BorderS.MouseLeftButtonUp += (sender, e) =>
            {
                if (ActiveManipulateSpectrum != null && ActiveManipulateTheme.DirectoryFile.Length > 0)
                    ActivateDialogManipulatePaletteSectrum(EnumDataSpectrum.Select);
            };
            BorderU.MouseLeftButtonUp += (sender, e) =>
            {
                if (ActiveManipulateSpectrum != null && ActiveManipulateTheme.DirectoryFile.Length > 0)
                    ActivateDialogManipulatePaletteSectrum(EnumDataSpectrum.Used);
            };
            BorderNE.MouseLeftButtonUp += (sender, e) =>
            {
                if (ActiveManipulateSpectrum != null && ActiveManipulateTheme.DirectoryFile.Length > 0)
                    ActivateDialogManipulatePaletteSectrum(EnumDataSpectrum.NotEnabled);
            };

            #region PanelActionPagePalette
            PanelActionPagePalette.IELButtonExecuteTheme.OnActivateMouseLeft += (sender, e, Key) =>
            {
                ActivateThemeInApplicationFromSelectIndex(SelectIndexTheme);
                SourcePanelAction?.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            };

            PanelActionPagePalette.IELButtonSelectTheme.OnActivateMouseLeft += (sender, e, Key) =>
            {
                ActivateThemeFromFile(SelectIndexTheme);
                SelectIndexTheme = -1;
                SourcePanelAction?.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            };

            PanelActionPagePalette.IELButtonDeleteTheme.OnActivateMouseLeft += (sender, e, Key) =>
            {
                string FileTheme = ArrayInicializeFilesTheme[SelectIndexTheme];
                MessageBoxResult Result = System.Windows.MessageBox.Show($"Вы точно хотите удалить файл темы?:\n{FileTheme}", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (Result == MessageBoxResult.Yes)
                {
                    File.Delete(FileTheme);
                    ArrayInicializeFilesTheme.RemoveAt(SelectIndexTheme);
                    StackPanelThemes.Children.RemoveAt(SelectIndexTheme);
                    SourcePanelAction?.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                    if (ActiveThemeInApplicationIndex == SelectIndexTheme)
                        ActivateThemeInApplicationFromSelectIndex(-1);
                    if (TextBlockNameSelectTheme.Text.Equals(Path.GetFileName(FileTheme)))
                    {
                        DiactivateSelectTheme();
                    }
                }
            };
            #endregion
        }

        //
        private void DiactivateSelectTheme()
        {
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, GridPaletteSpectrumViewer, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(400d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, GridWiewButtonQData, OpacityProperty,
                0d, TimeSpan.FromMilliseconds(400d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, GridQdataStatesColor, OpacityProperty,
                0d, TimeSpan.FromMilliseconds(400d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockNamingNoSelectPalette, OpacityProperty,
                0.4d, TimeSpan.FromMilliseconds(400d));
            BorderViewerQData.IsEnabled = false;
            GridPaletteSpectrumViewer.IsEnabled = false;
        }

        /// <summary>
        /// Активировать визуализацию окна менеджера тем
        /// </summary>
        internal void LoadingThemes()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
            {
                await App.CurrentApp.ExecuteVisualizateLoadingProcess("Загрузка спектров палитры",
                    CreateAllPaletteButtons(StackPanelSpectrum));
                if (ManagerAnimation != null)
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, StackPanelSpectrum, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
                else
                    StackPanelSpectrum.Opacity = 1d;
                await App.CurrentApp.ExecuteVisualizateLoadingProcess("Загрузка тем", CreateAllThemeButtons());
                if (ManagerAnimation != null)
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ScrollViewerTheme, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
                else
                    ScrollViewerTheme.Opacity = 1d;
            });
        }

        /// <summary>
        /// Визуализировать объект темы
        /// </summary>
        /// <param name="SourceData">Данные темы представляющий массив байтов всех спектов</param>
        private void ViewTheme(string DirectoryFile)
        {
            GridPaletteSpectrumViewer.IsEnabled = true;

            byte[] bytes = File.ReadAllBytes(DirectoryFile);
            ActiveManipulateTheme.Name = Path.GetFileName(DirectoryFile);
            ActiveManipulateTheme.DirectoryFile = DirectoryFile;
            ((Palette)ActiveManipulateTheme).ChangePaletteFromBytes(ref bytes);

            if (ActiveManipulateSpectrum != null)
                UpdateVisualPaletteSpectrumFromBorder(ref ActiveManipulateSpectrum);
            TextBlockNameSelectTheme.Text = ActiveManipulateTheme.Name;

            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, GridPaletteSpectrumViewer, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(1000d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockNamingNoSelectPalette, OpacityProperty,
                0d, TimeSpan.FromMilliseconds(400d));
        }

        #region Themes
        /// <summary>
        /// Инициализировать все объекты тем
        /// </summary>
        /// <param name="ResultGrid">Контейнер манипуляции</param>
        /// <returns></returns>
        private async Task CreateAllThemeButtons()
        {
            StackPanelThemes.Children.Clear();
            OPLThemeFile button;
            ArrayInicializeFilesTheme = [..Directory.GetFiles(StructDirectoryResources.DirectoryThemeApplication).Where((i) =>
                Path.GetExtension(i).Equals(".qd"))];
            string ActiveNameTheme = App.CurrentApp.SettingMainApplication.ThemeInstallName;
            for (int i = 0; i < ArrayInicializeFilesTheme.Count; i++)
            {
                button = CreateButtonTheme();
                button.TextNameFile = Path.GetFileNameWithoutExtension(ArrayInicializeFilesTheme[i]);
                if (ActiveNameTheme.Equals(button.TextNameFile, StringComparison.InvariantCultureIgnoreCase))
                {
                    button.IsActivate = true;
                    ActiveThemeInApplicationIndex = i;
                }
                StackPanelThemes.Children.Add(button);
            }
        }

        /// <summary>
        /// Создать объект представляющий тему
        /// </summary>
        /// <returns></returns>
        private OPLThemeFile CreateButtonTheme()
        {
            OPLThemeFile Button = new()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Padding = new(0d, 2d, 0d, 2d),
                FontSize = 18d,
                CornerRadius = new(12),
                IsActivate = false,
                BorderThickness = new(2),
                Margin = new(1, 2, 1, 2),
                SourceElement = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Palette)),
                ManagerAnimation = ManagerAnimation,
                Cursor = System.Windows.Input.Cursors.Hand,
                FontFamily = (System.Windows.Media.FontFamily)App.CurrentApp.Resources["Alphasano"],
            };
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PlumCrayola].ConnectPalleteFromIELElement(Button);
            Button.MouseLeftButtonUp += (sender, e) =>
            {
                if (SourcePanelAction != null && SourcePanelAction.PanelActionActivate)
                    SourcePanelAction.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                ActivateThemeFromFile(StackPanelThemes.Children.IndexOf((UIElement)sender));
            };
            Button.MouseRightButtonUp += (sender, e) =>
            {
                ActivatePanelActionThemeSelect((OPLThemeFile)sender, StackPanelThemes.Children.IndexOf((UIElement)sender));
            };
            return Button;
        }
        #endregion

        #region PaletteSpectrum
        /// <summary>
        /// Инициализировать все объекты спектров палитры
        /// </summary>
        /// <param name="UIResult">Контейнер манипуляции</param>
        /// <returns></returns>
        private async Task CreateAllPaletteButtons(StackPanel UIResult)
        {
            UIResult.Children.Clear();
            IELButtonText button = new();
            foreach (PaletteSpectrumEnum ElementPalette in Enum.GetValues<PaletteSpectrumEnum>())
            {
				button = await Dispatcher.InvokeAsync(CreateButtonPaletteSpectrum);
                button.Text = Enum.GetName(ElementPalette) ?? "Имя не инициализировано";
				button.OnActivateMouseLeft += (sender, e) => {
                    if (SourcePanelAction != null && SourcePanelAction.PanelActionActivate)
                        SourcePanelAction.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                    IELExampleButtonPalette.Text = ((IELButtonText)sender).Text;
                    IndexActivateVisualizateSpectrum = Grid.GetRow((IELObjectBase)sender);
                    if (!BorderViewerQData.IsEnabled)
                    {
                        OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, GridWiewButtonQData, OpacityProperty,
                            1d, TimeSpan.FromMilliseconds(400d));
                        OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, GridQdataStatesColor, OpacityProperty,
                            1d, TimeSpan.FromMilliseconds(400d));
                    }
                    BorderViewerQData.IsEnabled = true;
                    
                    SetPaletteViewer(((IELObjectBase)sender).PaletteElement);
                };
                ActiveManipulateTheme[ElementPalette].ConnectPalleteFromIELElement(button);

                UIResult.Children.Add(button);
			}
        }

        /// <summary>
        /// Создать объект представляющий спектр цвета палитры
        /// </summary>
        /// <returns></returns>
        private static IELButtonText CreateButtonPaletteSpectrum()
        {
            IELButtonText Button = new()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new(10, 5, 10, 5),
                Padding = new(0, 2, 0, 2),
                FontSize = 16d,
                CornerRadius = new(8),
                CornerRadiusGuides = new(7.6),
                VisualGuide = IEL.CORE.Enums.StateVisualGuide.RightArrow,
                BorderThickness = new(2),
                MarginViewBox = new(5, 8, 5, 2),
                Cursor = System.Windows.Input.Cursors.Hand,
                Height = 45d,
            };
            return Button;
        }
        #endregion

        private void SetPaletteViewer(PaletteSpectrum Source)
        {
            ActiveManipulateSpectrum = Source;
            IELExampleButtonPalette.PaletteElement = Source;

            UpdateVisualPaletteSpectrumFromBorder(ref Source);
        }

        /// <summary>
        /// Активировать панель действий для данного объекта темы под выделенным индексом
        /// </summary>
        /// <param name="Element">Выделенный объект темы</param>
        /// <param name="Index">Присваемый индекс</param>
        private void ActivatePanelActionThemeSelect(OPLThemeFile Element, int Index)
        {
            //PanelActionPagePalette.IELButtonExecuteTheme.IsEnabled = Element.CircleIndicatorFill?.Color != ActiveThemeColor;
            PanelActionPagePalette.IELButtonDeleteTheme.IsEnabled = Index > -1;
            PanelActionPagePalette.IELButtonSelectTheme.IsEnabled = Index > -1;
            SelectIndexTheme = Index;
            SourcePanelAction?.UsingPanelAction(MainGrid, PanelActionPagePalette, Orientation: IEL.CORE.Enums.OrientationPositionCursor.RightDown);
        }

        /// <summary>
        /// Активировать визуализацию темы по индексу файла
        /// </summary>
        /// <param name="Index">Индекс загруженного файла</param>
        private void ActivateThemeFromFile(int Index)
        {
            if (File.Exists(ArrayInicializeFilesTheme[Index]))
            ViewTheme(ArrayInicializeFilesTheme[Index]);
        }

        /// <summary>
        /// Активировать тему в приложении по выделенному индексу
        /// </summary>
        /// <param name="Index">Выделяемый индекс</param>
        private void ActivateThemeInApplicationFromSelectIndex(int Index)
        {
            if (Index == -1)
                ((Palette)App.CurrentApp.ActiveThemeApplication).ChangePaletteFromBytes(App.CurrentApp.DefaultPalette ??
                    throw new Exception("Непредвиденная ошибка нулевой палитры по умолчанию."));
            else
            {
                byte[] bytes = File.ReadAllBytes(ArrayInicializeFilesTheme[SelectIndexTheme]);
                ((Palette)App.CurrentApp.ActiveThemeApplication).ChangePaletteFromBytes(ref bytes);
            }

            if (ActiveThemeInApplicationIndex == -1) DefaultPaletteElement.IsActivate = false;
            else ((OPLThemeFile)StackPanelThemes.Children[ActiveThemeInApplicationIndex]).IsActivate = false;

            if (Index == -1) DefaultPaletteElement.IsActivate = true;
            else ((OPLThemeFile)StackPanelThemes.Children[Index]).IsActivate = true;

            App.CurrentApp.SettingMainApplication.ThemeInstallName.Value = Index == -1 ? string.Empty :
                Path.GetFileNameWithoutExtension(ArrayInicializeFilesTheme[SelectIndexTheme]);

            ActiveThemeInApplicationIndex = SelectIndexTheme;
            SelectIndexTheme = -1;
            App.CurrentApp.UpdateSettingApplication();
        }

        /// <summary>
        /// Обновить визуализацию спектров бордеров
        /// </summary>
        /// <param name="Source">Спектр палитры на значения которого изменяется отображение</param>
        private void UpdateVisualPaletteSpectrumFromBorder(ref PaletteSpectrum Source)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(400d);
            for (int i = 0; i < 4; i++)
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, DSUNE_ArrayBrush[i],
                    SolidColorBrush.ColorProperty, Source.BG.GetFromSpectrumColor((EnumDataSpectrum)i), span);
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, DSUNE_ArrayBrush[i + 4],
                    SolidColorBrush.ColorProperty, Source.BB.GetFromSpectrumColor((EnumDataSpectrum)i), span);
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, DSUNE_ArrayBrush[i + 8],
                    SolidColorBrush.ColorProperty, Source.FG.GetFromSpectrumColor((EnumDataSpectrum)i), span);
            }
        }

        private void ActivateDialogManipulatePaletteSectrum(EnumDataSpectrum SpectrumManipulate)
        {
            if (ActiveManipulateSpectrum == null)
                throw new Exception("Нет активного визуализируемого спектра палитры!");
            if (SourcePanelAction != null && SourcePanelAction.PanelActionActivate)
                SourcePanelAction.ClosePanelAction(PositionAnimActionPanel.CenterObject);
            DialogQDataSpectrum DialogQDataChange = new();
            DialogQDataChange.ShowDialogChangeQData(ActiveManipulateSpectrum, SpectrumManipulate);
            UpdateVisualPaletteSpectrumFromBorder(ref ActiveManipulateSpectrum);

            if (ActiveManipulateTheme.DirectoryFile.Length == 0)
                throw new Exception("Невозможно сохранить изменяемое значение в палитре. Нет доступного файла .qd");
            byte[] WriteBytes;
            using Stream stream = File.Open(ActiveManipulateTheme.DirectoryFile, FileMode.Open);
            for (int i = 0; i < PaletteSpectrum.CountQDataSpectrum; i++)
            {
                WriteBytes = i switch
                {
                    0 => ActiveManipulateSpectrum.BG.GetSourceBytes(),
                    1 => ActiveManipulateSpectrum.BB.GetSourceBytes(),
                    2 => ActiveManipulateSpectrum.FG.GetSourceBytes(),
                    _ => throw new Exception("Непредвиденное значение индекса спектра!")
                };
                stream.Position =
                    (WriteBytes.Length * PaletteSpectrum.CountQDataSpectrum) * IndexActivateVisualizateSpectrum + WriteBytes.Length * i;
                stream.Write(WriteBytes, 0, WriteBytes.Length);
            }
        }
    }
}
