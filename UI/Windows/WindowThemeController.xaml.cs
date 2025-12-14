using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Settings.PaletteElements;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementsControl;
using ApplicationOperPageLes.UI.Windows.Dialogs;
using IEL.CORE.Classes;
using IEL.UserElementsControl;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static IEL.CORE.Classes.QData;
using OPRES = ApplicationOperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;
using IEL.UserElementsControl.Base;

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
        private Theme ActiveManipulateTheme = new();

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
                if (ActiveManipulateTheme != null)
                {
                    FileStream stream = File.OpenWrite(ActiveManipulateTheme.DirectoryFile);
                    stream.Position = 0;
                    foreach (PaletteSpectrumEnum Element in Enum.GetValues<PaletteSpectrumEnum>())
                    {
                        PaletteSpectrum spectrum = ActiveManipulateTheme[Element];
                        PaletteSpectrum.WritePalettespectrum(ref stream, ref spectrum);
                    }
                    stream.Close();
                }
                IELButtonSaveTheme.IsEnabled = false;
            };
            ColumnDefinitionQDataViewer.MaxWidth = 0d;
            GridMainPaletteButtons.Opacity = 0d;
            GridPaletteSpectrumViewer.IsEnabled = false;
            GridPaletteSpectrumViewer.Opacity = 0d;
            BorderViewerQData.Opacity = 0d;

            DefaultPaletteElement.OnActivateMouseLeft += async (sender, e) =>
            {
                if (!MessageBoxActivateChangeThemeUnSave()) return;
                IELButtonSaveTheme.IsEnabled = false;
                ViewTheme(App.CurrentApp.ActiveThemeApplication);
            };

            IELButtonBackViewTheme.OnActivateMouseLeft += (sender, e) =>
            {
                if (!MessageBoxActivateChangeThemeUnSave()) return;
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
            ActiveManipulateTheme.ChangeSourceData(ref Source);
            if (ActiveManipulateSpectrum != null)
                UpdateVisualPaletteSpectrumFromBorder(ref ActiveManipulateSpectrum);
            TextBlockNameSelectTheme.Text = ActiveManipulateTheme.Name;
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
                ArrayInicializeThemes.Add(new Theme(Files[i]));
                button.Text = ArrayInicializeThemes[^1].Name;
                ResultGrid.Children.Add(button);
                button.UpdateLayout();
                button.Margin = new(5, i == 0 ? 5 : i * (button.ActualHeight + 10), 5, 0);
                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(button);
                button.OnActivateMouseLeft += (sender, e) =>
                {
                    if (MessageBoxActivateChangeThemeUnSave())
                    {
                        int index = ResultGrid.Children.IndexOf((UIElement)sender);
                        ViewTheme(ArrayInicializeThemes[index]);
                    }
                };
            }
            return ResultGrid;
        }

        /// <summary>
        /// Активировать вопрос о сохранении темы
        /// </summary>
        /// <returns></returns>
        private bool MessageBoxActivateChangeThemeUnSave()
        {
            if (IELButtonSaveTheme.IsEnabled)
            {
                MessageBoxResult Result = System.Windows.MessageBox.Show("Вы точно хотите закрыть тему?\nВсе изменённые данные будут утеряны", "Предупреждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return Result == MessageBoxResult.Yes;
            }
            else return true;
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
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Padding = new(0),
                FontSize = 18d,
                CornerRadius = new(10),
                CornerRadiusGuides = new(7.6),
                BorderThickness = new(2),
                Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Palette)),
                Height = 65,
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
                ActiveManipulateTheme[ElementPalette].ConnectPalleteFromIELElement(button);

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

            UpdateVisualPaletteSpectrumFromBorder(ref Source);
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
            IELButtonSaveTheme.IsEnabled = ActiveManipulateTheme.DirectoryFile.Length > 0;
            DialogQDataSpectrum DialogQDataChange = new();
            DialogQDataChange.ShowDialogChangeQData(SourceData, SpectrumManipulate);
            UpdateVisualPaletteSpectrumFromBorder(ref SourceData);
        }
    }
}
