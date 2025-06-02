using CefSharp;
using CefSharp.Wpf;
using IEL.Interfaces.Core;
using Microsoft.Maui.Platform;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                WebBrowserElement.CoreWebView2.NavigationStarting += (sender, e) =>
                {
                    App.MainWindowApplication.ActivateLoadingIndicator();
                    WebBrowserElement.Source = new Uri(e.Uri);
                };
                WebBrowserElement.CoreWebView2.NavigationCompleted += (sender, e) =>
                {
                    App.MainWindowApplication.DiactivateLoadingIndicator();
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
                };
            };
            IELButtonReloadPage.OnActivateMouseLeft += (sender, Key) =>
            {
                WebBrowserElement.Reload();
            };
            IELButtonUnopenPageSystemBrowser.OnActivateMouseLeft += (sender, Key) =>
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
