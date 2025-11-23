using ApplicationOperPageLes.CORE.Enums;
using IEL.CORE.Classes.Browser;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        internal event EventHandler<BrowserPage?>? BrowserPageSelect;

        public PageManagerBrowser()
        {
            InitializeComponent();

            #region Palette
            App.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonCancel, PaletteSpectrumEnum.Red);

            App.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonPageBrowser, PaletteSpectrumEnum.Violet);

            App.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonPageConsole, PaletteSpectrumEnum.Olive);

            App.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonAddPageLabel, PaletteSpectrumEnum.Lime);

            App.SettingPaletteApplication.ConnectPalleteFromIELElement(IELButtonPageDeveloper, PaletteSpectrumEnum.PastelBlue);
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

            IELButtonCancel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                BrowserPageSelect?.Invoke(this, null);
            };

            #region IELButtonAddPageLabel
            IELButtonAddPageLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageLabels page = new();
                BrowserPage AppendElementPage = new(page, "Ярлыки", "Средство быстрого выполнения командных инструкций в программе");
                AppendElementPage.Disposed += (sender) =>
                {

                };
                BrowserPageSelect?.Invoke(this, AppendElementPage);
            };
            #endregion

            #region IELButtonPageDeveloper
            IELButtonPageDeveloper.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageDeveloper page = new();
                BrowserPage AppendElementPage = new(page, "Страница разработчика", null);
                AppendElementPage.Disposed += (sender) =>
                {

                };
                BrowserPageSelect?.Invoke(this, AppendElementPage);
            };
            #endregion

            #region IELButtonPageConsole
            IELButtonPageConsole.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageConsole page = new();
                BrowserPage AppendElementPage = new(page, "Консоль", "Гибкий инструмент управления программой с помощью вводимых команд");
                AppendElementPage.Disposed += (sender) =>
                {

                };
                BrowserPageSelect?.Invoke(this, AppendElementPage);
            };
            #endregion

            #region IELButtonPageBrowser
            IELButtonPageBrowser.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageWebBrowser page = new();
                BrowserPage AppendElementPage = new(page, "Веб-браузер", null);
                AppendElementPage.Disposed += (sender) =>
                {
                    page.WebBrowserElement.Dispose();
                };
                BrowserPageSelect?.Invoke(this, AppendElementPage);
            };
            #endregion
        }
    }
}
