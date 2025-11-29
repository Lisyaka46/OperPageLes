using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.UI.UserElementControl;
using IEL.CORE.Classes;
using IEL.GUI;
using System;
using System.Collections.Generic;
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

namespace ApplicationOperPageLes.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowPaletteController.xaml
    /// </summary>
    public partial class WindowPaletteController : Window
    {
        public WindowPaletteController()
        {
            InitializeComponent();
            GridMainPaletteButtons.Opacity = 0d;

            Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
            {
                Grid GridLabels = await App.MainWindow.ExecuteVisualizateLoadingProcess("Загрузка данных палитры",
                    CreateAllPaletteButtons(GridMainPaletteButtons));
                App.DoubleAnimationType.AnimateEffect(GridMainPaletteButtons, OpacityProperty, 1d, TimeSpan.FromMilliseconds(1000d));
            });
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
				button.OnActivateMouseLeft += (sender, e, Key) => {
					//App.CurrentApp.SettingPaletteApplication.SourcePalette.Ge(ElementPalette).
					//SetFromSpectrumColor(QData.EnumDataSpectrum.Default, Colors.White);
				};
				await Dispatcher.InvokeAsync(() =>
				{ button.PaletteElement = App.CurrentApp.SettingPaletteApplication.SourcePalette[ElementPalette]; });
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
