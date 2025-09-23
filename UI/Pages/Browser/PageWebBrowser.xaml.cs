using OperPage_les.UI.UserElementControl;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace OperPage_les.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageWebBrowser.xaml
    /// </summary>
    public partial class PageWebBrowser : Page
    {
        public PageWebBrowser()
        {
            App.Log("Инициализация объектов станицы браузера");
            InitializeComponent();
            IELButtonReloadPage.Imaging = App.LoadImage(Properties.Resources.Reload);
            IELButtonUnopenPageSystemBrowser.Imaging = App.LoadImage(Properties.Resources.BrowserChangeSystem);
            App.Log("Инициализация станицы браузера");
            #region WebBrowserElement_Events
            WebBrowserElement.SourceChanged += (sender, e) =>
            {
                TextBoxLink.Text = WebBrowserElement.Source.ToString();
            };
            WebBrowserElement.CoreWebView2InitializationCompleted += (sender, e) =>
            {
                OPLViewerLoadingProcess ViewerLoading = App.MainWindow.GenerateVisualizateLoadingProcess("Test");
                WebBrowserElement.CoreWebView2.NavigationStarting += (sender, e) =>
                {
                    App.MainWindow.StartVisualizateLoadingProcess(ViewerLoading);
                    WebBrowserElement.Source = new Uri(e.Uri);
                };
                WebBrowserElement.CoreWebView2.NavigationCompleted += (sender, e) =>
                {
                    App.MainWindow.CompleteVisualizateLoadingProcess(ViewerLoading);
                };
                WebBrowserElement.CoreWebView2.NewWindowRequested += (sender, e) =>
                {
                    e.NewWindow = WebBrowserElement.CoreWebView2;
                };
            };
            #endregion
            TextBoxLink.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        WebViewGoUrl(TextBoxLink.Text);
                        break;
                    case Key.Escape:
                        WebBrowserElement.Focus();
                        break;
                    default:
                        break;
                }
                ;
            };
            IELButtonReloadPage.OnActivateMouseLeft += (sender, e, Key) =>
            {
                WebBrowserElement.Reload();
            };
            IELButtonUnopenPageSystemBrowser.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Process.Start(new ProcessStartInfo(TextBoxLink.Text) { UseShellExecute = true });
                WebBrowserElement.Stop();
            };
            string DefaultUrl = App.CurrentApp.SettingMainApplication.DefaultOpenUrlWebView;
            if (DefaultUrl.Length > 0)
            {
                WebViewGoUrl(DefaultUrl);
            }
            App.Log("Инициализация станицы браузера - Готово!");
        }

        internal void WebViewGoUrl(string Url)
        {
            WebBrowserElement.Source = new Uri(Url);
            WebBrowserElement.Focus();
        }
    }
}
