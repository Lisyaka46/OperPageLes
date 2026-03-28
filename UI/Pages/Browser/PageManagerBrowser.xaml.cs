using ApplicationOperPageLes.CORE.Enums;
using OIEL.CORE.Browser;
using System.Windows.Controls;
using System.Windows.Input;

namespace ApplicationOperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageManagerBrowser.xaml
    /// </summary>
    public partial class PageManagerBrowser : Page
    {
        /// <summary>
        /// Событие при котором была выделена страница браузера
        /// </summary>
        internal event EventHandler<PageBrowser?>? BrowserPageSelect;

        public PageManagerBrowser()
        {
            InitializeComponent();

            #region Palette
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Red].ConnectPalleteFromIELElement(IELButtonCancel);

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Violet].ConnectPalleteFromIELElement(IELButtonPageBrowser);

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Olive].ConnectPalleteFromIELElement(IELButtonPageConsole);

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(IELButtonAddPageLabel);

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(IELButtonPageDeveloper);
            #endregion

            KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        BrowserPageSelect?.Invoke(this, null);
                        break;
                }
            };

            IELButtonCancel.OnActivateMouseLeft += (sender, e) =>
            {
                BrowserPageSelect?.Invoke(this, null);
            };

            #region IELButtonAddPageLabel
            IELButtonAddPageLabel.OnActivateMouseLeft += (sender, e) =>
            {
                PageLabels SourcePageLabels = new()
                {
                    Title = "Ярлыки",
                    Description = "Средство быстрого выполнения командных инструкций в программе",
                };
                SourcePageLabels.Disposed += (sender) =>
                {

                };
                BrowserPageSelect?.Invoke(this, SourcePageLabels);
            };
            #endregion

            #region IELButtonPageDeveloper
            IELButtonPageDeveloper.OnActivateMouseLeft += (sender, e) =>
            {
                IELButtonPageDeveloper.IsEnabled = false;
                BrowserPageSelect?.Invoke(this, App.ApplicationPageDeveloper);
            };
            #endregion

            #region IELButtonPageConsole
            IELButtonPageConsole.OnActivateMouseLeft += (sender, e) =>
            {
                PageConsole SourcePageConsole = new()
                {
                    Title = "Консоль",
                    Description = "Гибкий инструмент управления программой с помощью вводимых команд",
                };
                SourcePageConsole.Disposed += (sender) =>
                {

                };
                BrowserPageSelect?.Invoke(this, SourcePageConsole);
            };
            #endregion

            #region IELButtonPageNetwork
            IELButtonPageNetwork.OnActivateMouseLeft += (sender, e) =>
            {
                BrowserPageNetwork.PageNetwork SourcePageNetwork = new()
                {
                    Title = "Страница сети",
                    Description = "Средство управления сетевыми инструментами",
                };
                SourcePageNetwork.Disposed += (sender) =>
                {
                    SourcePageNetwork.Dispose();
                };
                BrowserPageSelect?.Invoke(this, SourcePageNetwork);
            };
            #endregion

            #region IELButtonPageBrowser
            TextBlockDeveloping.Opacity = 1d;
            IELButtonPageBrowser.OnActivateMouseLeft += (sender, e) =>
            {
                PageWebBrowser SourcePageWebBrowser = new()
                {
                    Title = "Веб-браузер"
                };
                SourcePageWebBrowser.Disposed += (sender) =>
                {
                    SourcePageWebBrowser.WebBrowserElement.Dispose();
                };
                BrowserPageSelect?.Invoke(this, SourcePageWebBrowser);
            };
            #endregion
        }
    }
}
