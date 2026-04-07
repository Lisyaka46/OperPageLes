using OperPageLes.CORE.Struct;
using IEL.CORE.Classes;
using IEL.UserElementsControl;
using OIEL.CORE.Browser;
using OIEL.Interfaces.Core;
using OIEL.UserElementsControl;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageManagerAppPage.xaml
    /// </summary>
    public partial class PageManagerAppPage : Page
    {
        /// <summary>
        /// Размер иконок
        /// </summary>
        private double SizeIcons = 100d;

        public PageManagerAppPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Добавить отображение иконки в менеджере приложений страниц
        /// </summary>
        /// <param name="TypeAppPage">Тип создаваемого приложения страницы</param>
        internal void AddNewAppPage(in OPLBrowserPage Browser, Type TypeAppPage, string NameAppPage, PaletteSpectrum? Spectrum = null, ImageSource? Icon = null)
        {
            OPLApplicationPage AppPage = GetNewAppPage(Browser, TypeAppPage);
            AppPage.SetSizeIconApp(new(SizeIcons, SizeIcons));
            AppPage.ManagerAnimation = App.ManagerAnimation;
            AppPage.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            AppPage.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            AppPage.PaletteElement = Spectrum ?? App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Gray];
            AppPage.Source = Icon ?? StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication));
            AppPage.Text = NameAppPage;
            //MainGrid.UpdateLayout();
            ////if (MainGrid.Children.Count + 1 > CountIconsOneLine)
            ////{
            ////    CountIconLines++;
            ////    CurrentPos = 0;
            ////    MainGrid.RowDefinitions.Add(new() { Height = new(0d, System.Windows.GridUnitType.Auto) });
            ////}
            //Grid.SetRow(AppPage, CountIconLines);
            //Grid.SetColumn(AppPage, CurrentPos++);
            //MainGrid.ColumnDefinitions.Add(new() { Width = new(1d, System.Windows.GridUnitType.Star) });
            MainPanel.Children.Add(AppPage);
        }

        /// <summary>
        /// Создать объект иконки придожения страницы
        /// </summary>
        /// <param name="SourcePage">Ссылка на страницу</param>
        /// <returns>Визуальный объект иконки</returns>
        private static OPLApplicationPage GetNewAppPage(OPLBrowserPage Browser, Type TypeAppPage)
        {
            OPLApplicationPage AppPage = new(TypeAppPage);
            AppPage.OnActivateMouseLeft += (sender, e) =>
            {
                InitAppPageFromType(in Browser, (OPLApplicationPage)sender, true);
            };
            AppPage.OnActivateMouseRight += (sender, e) =>
            {
                InitAppPageFromType(in Browser, (OPLApplicationPage)sender, false);
            };
            return AppPage;
        }

        /// <summary>
        /// Инициализировать страницу по хранимому типу в иконке
        /// </summary>
        /// <param name="Browser">Браузер страниц</param>
        /// <param name="UIAppPage">Иконка хранимого типа приложения страницы</param>
        /// <param name="Activate">Активировать созданную вкладку или нет</param>
        private static void InitAppPageFromType(in OPLBrowserPage Browser, in OPLApplicationPage UIAppPage, bool Activate = true)
        {
            PageBrowser ElementAppPage = (PageBrowser)(Activator.CreateInstance(UIAppPage.TypeBrowserAppPage) ??
                throw new Exception("Не удалось создать объект приложения страницы"));
            ElementAppPage.Title = UIAppPage.Text;
            IELButtonImage CloseButtonInlay = Browser.AddInlayPage(in ElementAppPage, UIAppPage.PaletteElement, Activate).GetButtonCloseInlay();
            CloseButtonInlay.PaletteElement = App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Red];
            CloseButtonInlay.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
        }
    }
}
