using CefSharp;
using CefSharp.Wpf;
using Microsoft.Win32;
using OperPageLes.CORE.Struct;
using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.UserElementsControl;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Pages.Browser.InlayPages
{
    /// <summary>
    /// Логика взаимодействия для PageWebBrowser.xaml
    /// </summary>
    public partial class PageWebBrowser : PageBrowser
    {
        public PageWebBrowser()
        {
            App.LogWriteLine("Инициализация объектов станицы веб-браузера");
            InitializeComponent();
            //WebBrowserElement.CreateBrowser(HwndSource.FromHwnd(
            //    new System.Windows.Interop.WindowInteropHelper(App.Current.MainWindow).EnsureHandle()), new(800, 800));
            //WebBrowserElement.BrowserSettings = new BrowserSettings(true)
            //{
            //    WindowlessFrameRate = 1
            //};
            //SourceBrowser = WebBrowserElement.GetBrowser();

            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.World));

            IELButtonReloadPage.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Reload));
            IELButtonUnopenPageSystemBrowser.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.BrowserChangeSystem));
            App.LogWriteLine("Инициализация станицы веб-браузера");

            #region WebBrowserElement_Events
            WebBrowserElement.GotFocus += (sender, e) =>
            {
                if (SourcePanelAction?.PanelActionActivate ?? false)
                    SourcePanelAction.ClosePanelAction();
            };
            //WebBrowserElement.
            //WebBrowserElement.KeyUp += (sender, e) =>
            //{
            //    WebBrowserElement.RaiseEvent(e);
            //};
            //WebBrowserElement.Na += (sender, e) =>
            //{
            //    TextBoxLink.Text = WebBrowserElement.Source.ToString();
            //};
            //WebBrowserElement.CoreWebView2InitializationCompleted += (sender, e) =>
            //{
            //    WebBrowserElement.CoreWebView2.NavigationStarting += (sender, e) =>
            //    {
            //        TextBoxLink.Text = WebBrowserElement.Source.ToString();
            //        //if (ViewerLoading != null) ViewerLoading.Text = $"Загрузка {e.Uri}";
            //        //else
            //        //{
            //        //    ViewerLoading = App.MainWindow.ExecuteVisualizateLoadingProcess($"Загрузка {e.Uri}");
            //        //    App.MainWindow.StartVisualizateLoadingProcess(ViewerLoading);
            //        //}
            //        WebBrowserElement.Source = new Uri(e.Uri);
            //    };
            //    WebBrowserElement.CoreWebView2.NavigationCompleted += (sender, e) =>
            //    {
            //        //if (ViewerLoading == null) return;
            //        //App.MainWindow.CompleteVisualizateLoadingProcess(ViewerLoading);
            //    };
            //    WebBrowserElement.CoreWebView2.NewWindowRequested += (sender, e) =>
            //    {
            //        e.NewWindow = WebBrowserElement.CoreWebView2;
            //    };
            //};
            #endregion

            TextBoxLink.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        WebViewGoUrl(TextBoxLink.Text);
                        break;
                    case Key.Escape:
                        //WebBrowserElement.Focus();
                        break;
                    default:
                        break;
                }
                ;
            };
            IELButtonReloadPage.OnActivateMouseLeft += (sender, e) =>
            {
                //SourceBrowser.Reload();
            };
            IELButtonUnopenPageSystemBrowser.OnActivateMouseLeft += (sender, e) =>
            {
                Process.Start(new ProcessStartInfo(WebBrowserElement.Address) { UseShellExecute = true });
            };
            Loaded += (sender, e) =>
            {
                //SourceBrowser = WebBrowserElement.GetBrowser();
            };


            string DefaultUrl = string.Empty; // App.CurrentApp.SettingMainApplication.DefaultOpenUrlWebView;
            TextBoxLink.Text = DefaultUrl;
            if (DefaultUrl.Length > 0)
            {
                WebViewGoUrl(DefaultUrl);
            }
            App.LogWriteLine("Инициализация станицы браузера - Готово!");
        }

        internal void WebViewGoUrl(string Url)
        {
            WebBrowserElement.Load(Url);
            TextBoxLink.Text = Url;
            WebBrowserElement.Focus();
            //WebBrowserElement.Source = new Uri(Url);
            //WebBrowserElement.Focus();
        }
    }
}
