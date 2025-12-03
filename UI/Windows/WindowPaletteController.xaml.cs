using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Settings.PaletteElements;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl;
using IEL.CORE.Classes;
using IEL.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowPaletteController.xaml
    /// </summary>
    public partial class WindowPaletteController : Window
    {
        /// <summary>
        /// Активная тема взаимодействия
        /// </summary>
        private Theme? ActiveManipulateTheme;

        public WindowPaletteController()
        {
            InitializeComponent();
            IELButtonSaveTheme.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Save));
            IELButtonSaveTheme.IsEnabled = false;
            IELButtonSaveTheme.OnActivateMouseLeft += async (sender, e) =>
            {
                if (ActiveManipulateTheme != null)
                {
                    FileStream stream = File.OpenWrite("C:/Users/killm/Рабочий стол/Новая папка/QData.qd");
                    stream.Position = 0;
                    foreach (PaletteSpectrumEnum Element in Enum.GetValues<PaletteSpectrumEnum>())
                    {
                        PaletteSpectrum spectrum = ActiveManipulateTheme[Element];
                        await spectrum.BG.WriteQdata(stream);
                        await spectrum.BB.WriteQdata(stream);
                        await spectrum.FG.WriteQdata(stream);
                    }
                    stream.Close();
                    stream.Dispose();
                }
                IELButtonSaveTheme.IsEnabled = false;
            };
            PanelActionMain.ClosePanelAction(IEL.CORE.Enums.PositionAnimActionPanel.CenterObject);
            PanelActionMain.Width = 0d;
            PanelActionMain.Height = 0d;
            PanelActionMain.Opacity = 0d;
            GridMainPaletteButtons.Opacity = 0d;
            GridThemeViewer.Opacity = 0d;

            DefaultPaletteElement.VisualOpen();
            DefaultPaletteElement.OnActivateMouseLeft += async (sender, e) =>
            {
                await App.CurrentApp.ActiveThemeApplication.ChangeSourceTheme(
                    new(StructDirectoryResources.GetResourcePath(nameof(OPRES.PaletteDictionary))));
                ViewTheme(App.CurrentApp.ActiveThemeApplication);
            };
            DEFP.VisualOpen();
            DEFP.OnActivateMouseLeft += async (sender, e) =>
            {
                await App.CurrentApp.ActiveThemeApplication.ChangeSourceTheme(
                    new(File.ReadAllBytes("C:/Users/killm/Рабочий стол/Новая папка/QData.qd")));
                ViewTheme(App.CurrentApp.ActiveThemeApplication);
            };

            Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
            {
                Grid Element = await App.MainWindow.ExecuteVisualizateLoadingProcess("Загрузка данных палитры",
                    CreateAllPaletteButtons(GridMainPaletteButtons));
                App.DoubleAnimationType.AnimateEffect(GridMainPaletteButtons, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
            });
        }

        /// <summary>
        /// Визуализировать объект темы
        /// </summary>
        /// <param name="Source">Данные темы</param>
        private void ViewTheme(Theme Source)
        {
            ActiveManipulateTheme = Source;
            IELButtonSaveTheme.IsEnabled = true;
            TextBlockNameSelectTheme.Text = Source.Name;
            App.DoubleAnimationType.AnimateEffect(GridThemeViewer, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
            for (uint i = 0; i < GridMainPaletteButtons.Children.Count; i++)
            {
                OPLButtonQData button = (OPLButtonQData)GridMainPaletteButtons.Children[(int)i];
                button.PaletteElement = Source[(PaletteSpectrumEnum)i];
                button.UpdateVisualQDataSpectrum();
            }
        }

        private async Task<Grid> CreateAllPaletteButtons(Grid ResultGrid)
        {
            ResultGrid.Children.Clear();
            ResultGrid.RowDefinitions.Clear();
            OPLButtonQData button = new();
            foreach (PaletteSpectrumEnum ElementPalette in Enum.GetValues<PaletteSpectrumEnum>())
            {
				button = await Dispatcher.InvokeAsync(CreateButtonPalette);
				//button.OnActivateMouseLeft += (sender, e, Key) =>
				//{
				//    App.CurrentApp.SettingPaletteApplication.SourcePalette.GetQdataFromEnum(PaletteValuesEnum.BG_Tangerine).Default = Colors.White;
				//};
				button.Text = Enum.GetName(ElementPalette) ?? "Имя не инициализировано";
				button.OnActivateMouseLeft += (sender, e) => {
					//App.CurrentApp.SettingPaletteApplication.SourcePalette.Ge(ElementPalette).
					//SetFromSpectrumColor(QData.EnumDataSpectrum.Default, Colors.White);
				};
				await Dispatcher.InvokeAsync(() =>
				{ button.PaletteElement = App.CurrentApp.ActiveThemeApplication[ElementPalette]; });
				ResultGrid.Children.Add(button);
				Grid.SetRow(button, ResultGrid.RowDefinitions.Count);
				ResultGrid.RowDefinitions.Add(new() { Height = new(0d, GridUnitType.Auto) });
			}
            return ResultGrid;
        }

        private static OPLButtonQData CreateButtonPalette()
        {
            OPLButtonQData Button = new()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new(5),
                Padding = new(5),
                FontSize = 15d,
                CornerRadius = new(5),
                BorderThickness = new(2),
            };
            return Button;
        }
    }
}
