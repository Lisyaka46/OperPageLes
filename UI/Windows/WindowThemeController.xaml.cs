using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Settings.PaletteElements;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl;
using ApplicationOperPageLes.UI.Windows.Dialogs;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes;
using IEL.GUI;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static IEL.CORE.Classes.QData;
using OPRES = ApplicationOperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;

namespace ApplicationOperPageLes.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowThemeController.xaml
    /// </summary>
    public partial class WindowThemeController : Window
    {
        /// <summary>
        /// Активная палитра взаимодействия
        /// </summary>
        private Palette ActiveManipulatePalette = new(StructDirectoryResources.GetResourcePath(nameof(OPRES.PaletteDictionary)));

        private string DitectoryOpenManipulatePalette = string.Empty;

        /// <summary>
        /// Активный объект спектра палитры над которым производится манипуляция
        /// </summary>
        private PaletteSpectrum? ActiveManipulateSpectrum;

        /// <summary>
        /// Массив инициализированных тем
        /// </summary>
        private List<Theme> ArrayInicializeThemes = [];

        /// <summary>
        /// Массив всех кистей отображения спектров QData
        /// </summary>
        private readonly SolidColorBrush[] DSUNE_ArrayBrush;

        public WindowThemeController()
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
            IELButtonSaveTheme.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Save));
            IELButtonSaveTheme.IsEnabled = false;
            IELButtonSaveTheme.OnActivateMouseLeft += (sender, e) =>
            {
                if (ActiveManipulatePalette != null)
                {
                    FileStream stream = File.OpenWrite(DitectoryOpenManipulatePalette);
                    stream.Position = 0;
                    foreach (PaletteSpectrumEnum Element in Enum.GetValues<PaletteSpectrumEnum>())
                    {
                        PaletteSpectrum spectrum = ActiveManipulatePalette[Element];
                        PaletteSpectrum.WritePalettespectrum(ref stream, ref spectrum);
                    }
                    stream.Close();
                }
                IELButtonSaveTheme.IsEnabled = false;
            };
            ColumnDefinitionQDataViewer.MaxWidth = 0d;
            PanelActionMain.ClosePanelAction(IEL.CORE.Enums.PositionAnimActionPanel.CenterObject);
            PanelActionMain.Width = 0d;
            PanelActionMain.Height = 0d;
            PanelActionMain.Opacity = 0d;
            GridMainPaletteButtons.Opacity = 0d;
            GridPaletteSpectrumViewer.IsEnabled = false;
            GridPaletteSpectrumViewer.Opacity = 0d;
            BorderViewerQData.Opacity = 0d;

            DefaultPaletteElement.VisualOpen();
            DefaultPaletteElement.OnActivateMouseLeft += async (sender, e) =>
            {
                IELButtonSaveTheme.IsEnabled = false;
                ViewTheme(App.CurrentApp.ActiveThemeApplication);
            };

            IELButtonBackViewTheme.OnActivateMouseLeft += (sender, e) =>
            {
                if (IELButtonSaveTheme.IsEnabled)
                {
                    MessageBoxResult Result = System.Windows.MessageBox.Show("Вы точно хотите закрыть тему?\nВсе изменённые данные будут утеряны", "Предупреждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (Result == MessageBoxResult.No) return;
                }
                App.DoubleAnimationType.AnimateEffect(GridPaletteSpectrumViewer, OpacityProperty, 0d, TimeSpan.FromMilliseconds(400d));
                GridPaletteSpectrumViewer.IsEnabled = false;
                IELButtonSaveTheme.IsEnabled = false;
            };

            IELButtonBackViewQData.OnActivateMouseLeft += (sender, e) =>
            {
                App.DoubleAnimationType.AnimateEffect(ColumnDefinitionQDataViewer, ColumnDefinition.MaxWidthProperty, 0d, TimeSpan.FromMilliseconds(500d));
                App.DoubleAnimationType.AnimateEffect(BorderViewerQData, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
                ActiveManipulateSpectrum = null;
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
                if (ActiveManipulateSpectrum != null)
                    ActivateDialogManipulatePaletteSectrum(ActiveManipulateSpectrum, EnumDataSpectrum.Default);
            };
            BorderS.MouseLeftButtonUp += (sender, e) =>
            {
                if (ActiveManipulateSpectrum != null)
                    ActivateDialogManipulatePaletteSectrum(ActiveManipulateSpectrum, EnumDataSpectrum.Select);
            };
            BorderU.MouseLeftButtonUp += (sender, e) =>
            {
                if (ActiveManipulateSpectrum != null)
                    ActivateDialogManipulatePaletteSectrum(ActiveManipulateSpectrum, EnumDataSpectrum.Used);
            };
            BorderNE.MouseLeftButtonUp += (sender, e) =>
            {
                if (ActiveManipulateSpectrum != null)
                    ActivateDialogManipulatePaletteSectrum(ActiveManipulateSpectrum, EnumDataSpectrum.NotEnabled);
            };

            Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
            {
                await App.MainWindow.ExecuteVisualizateLoadingProcess("Загрузка спектров палитры",
                    CreateAllPaletteButtons(GridMainPaletteButtons));
                App.DoubleAnimationType.AnimateEffect(GridMainPaletteButtons, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
                await App.MainWindow.ExecuteVisualizateLoadingProcess("Загрузка тем",
                    CreateAllThemeButtons(GridThemes));
                App.DoubleAnimationType.AnimateEffect(GridThemes, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
            });
        }

        /// <summary>
        /// Визуализировать объект темы
        /// </summary>
        /// <param name="Source">Данные темы</param>
        private void ViewTheme(Theme Source)
        {
            GridPaletteSpectrumViewer.IsEnabled = true;
            ActiveManipulatePalette.ChangeSourcePaletteData((Palette)Source);
            TextBlockNameSelectTheme.Text = Source.Name;
            DitectoryOpenManipulatePalette = Source.DirectoryFile;
            App.DoubleAnimationType.AnimateEffect(GridPaletteSpectrumViewer, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
        }

        #region Themes
        /// <summary>
        /// Инициализировать все объекты тем
        /// </summary>
        /// <param name="ResultGrid">Контейнер манипуляции</param>
        /// <returns></returns>
        private async Task<Grid> CreateAllThemeButtons(Grid ResultGrid)
        {
            ArrayInicializeThemes.Clear();
            ResultGrid.Children.Clear();
            OPLImageViewer button = new();
            string[] Files = [..Directory.GetFiles(StructDirectoryResources.DirectoryThemeApplication).Where((i) =>
                File.Exists(i) || Path.GetExtension(i).Equals(".qd"))];
            for (int i = 0; i < Files.Length; i++)
            {
                button = CreateButtonTheme();
                button.Margin = new(5, i * 45, 5, 0);
                ArrayInicializeThemes.Add(new Theme(Files[i]));
                button.Text = ArrayInicializeThemes[^1].Name;
                ResultGrid.Children.Add(button);
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(button);
                button.OnActivateMouseLeft += (sender, e) =>
                {
                    int index = ResultGrid.Children.IndexOf((UIElement)sender);
                    ViewTheme(ArrayInicializeThemes[index]);
                };
            }
            return ResultGrid;
        }

        /// <summary>
        /// Создать объект представляющий тему
        /// </summary>
        /// <returns></returns>
        private static OPLImageViewer CreateButtonTheme()
        {
            OPLImageViewer Button = new()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new(5),
                Padding = new(0),
                FontSize = 18d,
                CornerRadius = new(10),
                CornerRadiusGuides = new(7.6),
                BorderThickness = new(2),
                Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Palette)),
                Height = 50,
            };
            return Button;
        }
        #endregion

        #region PaletteSpectrum
        /// <summary>
        /// Инициализировать все объекты спектров палитры
        /// </summary>
        /// <param name="ResultGrid">Контейнер манипуляции</param>
        /// <returns></returns>
        private async Task<Grid> CreateAllPaletteButtons(Grid ResultGrid)
        {
            ResultGrid.Children.Clear();
            ResultGrid.RowDefinitions.Clear();
            IELButtonText button = new();
            foreach (PaletteSpectrumEnum ElementPalette in Enum.GetValues<PaletteSpectrumEnum>())
            {
				button = await Dispatcher.InvokeAsync(CreateButtonPaletteSpectrum);
                button.Text = Enum.GetName(ElementPalette) ?? "Имя не инициализировано";
				button.OnActivateMouseLeft += (sender, e) => {
                    App.DoubleAnimationType.AnimateEffect(ColumnDefinitionQDataViewer, ColumnDefinition.MaxWidthProperty, 200d, TimeSpan.FromMilliseconds(500d));
                    App.DoubleAnimationType.AnimateEffect(BorderViewerQData, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                    SetPaletteViewer(((IELObjectBase)sender).PaletteElement);
                };
                ActiveManipulatePalette[ElementPalette].ConnectPalleteFromIELElement(button);

                ResultGrid.Children.Add(button);
				Grid.SetRow(button, ResultGrid.RowDefinitions.Count);
				ResultGrid.RowDefinitions.Add(new() { Height = new(0d, GridUnitType.Auto) });
			}
            return ResultGrid;
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
                Padding = new(0, 10, 0, 10),
                FontSize = 18d,
                CornerRadius = new(5),
                CornerRadiusGuides = new(7.6),
                VisualGuide = IEL.CORE.Enums.StateVisualGuide.LeftArrow,
                BorderThickness = new(2),
            };
            return Button;
        }
        #endregion

        private void SetPaletteViewer(PaletteSpectrum Source)
        {
            ActiveManipulateSpectrum = Source;
            IELExampleButtonPalette.PaletteElement = Source;

            UpdateVisualQData(ref Source);
        }

        private void UpdateVisualQData(ref PaletteSpectrum Source)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(400d);
            for (int i = 0; i < 4; i++)
            {
                App.ColorAnimationType.AnimateEffect(DSUNE_ArrayBrush[i],
                    SolidColorBrush.ColorProperty, Source.BG.GetFromSpectrumColor((EnumDataSpectrum)i), span);
                App.ColorAnimationType.AnimateEffect(DSUNE_ArrayBrush[i + 4],
                    SolidColorBrush.ColorProperty, Source.BB.GetFromSpectrumColor((EnumDataSpectrum)i), span);
                App.ColorAnimationType.AnimateEffect(DSUNE_ArrayBrush[i + 8],
                    SolidColorBrush.ColorProperty, Source.FG.GetFromSpectrumColor((EnumDataSpectrum)i), span);
            }
        }

        private void ActivateDialogManipulatePaletteSectrum(PaletteSpectrum SourceData, EnumDataSpectrum SpectrumManipulate)
        {
            IELButtonSaveTheme.IsEnabled = DitectoryOpenManipulatePalette.Length > 0;
            DialogQDataSpectrum DialogQDataChange = new();
            DialogQDataChange.ShowDialogChangeQData(SourceData, SpectrumManipulate);
            UpdateVisualQData(ref SourceData);
        }
    }
}
