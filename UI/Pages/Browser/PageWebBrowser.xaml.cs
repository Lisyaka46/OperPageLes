using Microsoft.Win32;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementControl;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageWebBrowser.xaml
    /// </summary>
    public partial class PageWebBrowser : Page
    {
        public PageWebBrowser()
        {
            App.CurrentApp.LogWriteLine("Инициализация объектов станицы браузера");
            InitializeComponent();

            //int BrowserVer, RegVal;
            //// get the installed IE version
            //using (System.Windows.Forms.WebBrowser Wb = new())
            //    BrowserVer = Wb.Version.Major;
            //// set the appropriate IE version
            //if (BrowserVer >= 11)
            //    RegVal = 11001;
            //else if (BrowserVer == 10)
            //    RegVal = 10001;
            //else if (BrowserVer == 9)
            //    RegVal = 9999;
            //else if (BrowserVer == 8)
            //    RegVal = 8888;
            //else
            //    RegVal = 7000;
            //// set the actual key
            //using (RegistryKey Key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", RegistryKeyPermissionCheck.ReadWriteSubTree))
            //    if (Key.GetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe") == null)
            //        Key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", RegVal, RegistryValueKind.DWord);

            IELButtonReloadPage.Imaging = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Reload));
            IELButtonUnopenPageSystemBrowser.Imaging = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.BrowserChangeSystem));
            App.CurrentApp.LogWriteLine("Инициализация станицы браузера");
            #region WebBrowserElement_Events
            WebBrowserElement.Navigated += (sender, e) =>
            {
                TextBoxLink.Text = WebBrowserElement.Source.ToString();
            };
            //WebBrowserElement.CoreWebView2InitializationCompleted += (sender, e) =>
            //{
            //    WebBrowserElement.CoreWebView2.NavigationStarting += (sender, e) =>
            //    {
            //        if (ViewerLoading != null) ViewerLoading.Text = $"Загрузка {e.Uri}";
            //        else
            //        {
            //            ViewerLoading = App.MainWindow.GenerateVisualizateLoadingProcess($"Загрузка {e.Uri}");
            //            App.MainWindow.StartVisualizateLoadingProcess(ViewerLoading);
            //        }
            //        WebBrowserElement.Source = new Uri(e.Uri);
            //    };
            //    WebBrowserElement.CoreWebView2.NavigationCompleted += (sender, e) =>
            //    {
            //        if (ViewerLoading == null) return;
            //        App.MainWindow.CompleteVisualizateLoadingProcess(ViewerLoading);
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
                        WebBrowserElement.Focus();
                        break;
                    default:
                        break;
                }
                ;
            };
            IELButtonReloadPage.OnActivateMouseLeft += (sender, e, Key) =>
            {
                WebBrowserElement.Refresh();
            };
            IELButtonUnopenPageSystemBrowser.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Process.Start(new ProcessStartInfo(TextBoxLink.Text) { UseShellExecute = true });
                //WebBrowserElement.Stop();
            };
            string DefaultUrl = App.CurrentApp.SettingMainApplication.DefaultOpenUrlWebView;
            if (DefaultUrl.Length > 0)
            {
                WebViewGoUrl(DefaultUrl);
            }
            App.CurrentApp.LogWriteLine("Инициализация станицы браузера - Готово!");
        }

        internal void WebViewGoUrl(string Url)
        {
            WebBrowserElement.Source = new Uri(Url);
            WebBrowserElement.Focus();
        }
    }
}
