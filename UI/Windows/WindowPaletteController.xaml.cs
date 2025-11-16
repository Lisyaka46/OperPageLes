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
using ApplicationOperPageLes.CORE.Enums;
using IEL.CORE.Classes;
using IEL.GUI;

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
            int SpectrumCoeff = 0;
            IELButtonText button = new();
            foreach (PaletteValuesEnum ElementPalette in Enum.GetValues<PaletteValuesEnum>())
            {
                if (SpectrumCoeff == 0)
                {
                    button = await Dispatcher.InvokeAsync(CreateButtonPalette);
                    button.Text = Enum.GetName(ElementPalette) ?? "Имя не инициализировано";
                    button.OnActivateMouseLeft += (sender, e, Key) => {
                        //App.CurrentApp.SettingPaletteApplication.Ge(ElementPalette).
                        //SetFromSpectrumColor(QData.EnumDataSpectrum.Default, Colors.White);
                    };
                    ResultGrid.Children.Add(button);
                    Grid.SetRow(button, ResultGrid.RowDefinitions.Count);
                    ResultGrid.RowDefinitions.Add(new() { Height = new(0d, GridUnitType.Auto) });
                }
                await Dispatcher.InvokeAsync(() =>
                {
                    (SpectrumCoeff switch
                    {
                        0 => button.QBackground,
                        1 => button.QBorderBrush,
                        2 => button.QForeground,
                        _ => throw new ArgumentException("Текущий логический аргумент принял недопустимое значение"),
                    }).SetQData(App.CurrentApp.SettingPaletteApplication.GetQdataFromEnum(ElementPalette));
                });
                SpectrumCoeff = ++SpectrumCoeff % 3;
            }
            return ResultGrid;
        }

        private static IELButtonText CreateButtonPalette()
        {
            IELButtonText Button = new()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new(5),
                PaddingContent = new(5),
                FontSize = 15d,
                BorderThicknessBlock = new(2),
            };
            return Button;
        }
    }
}
