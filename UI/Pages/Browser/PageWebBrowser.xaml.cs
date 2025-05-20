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
using CefSharp.Wpf;
using CefSharp;

namespace OperPage_les.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageWebBrowser.xaml
    /// </summary>
    public partial class PageWebBrowser : Page
    {
        /// <summary>
        /// Объект анимации для управления double значением
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimate = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        public PageWebBrowser()
        {
            App.Log("Инициализация объектов станицы браузера");
            InitializeComponent();
            IELButtonReloadPage.Imaging = App.LoadImage(Properties.Resources.Reload);
            App.Log("Инициализация станицы браузера");
            //WebBrowserElement.BrowserSettings.Javascript = CefState.Enabled;
            //WebBrowserElement.BrowserSettings.ImageLoading = CefState.Enabled;
            //WebBrowserElement.BrowserSettings.JavascriptAccessClipboard = CefState.Enabled;
            //WebBrowserElement.BrowserSettings.JavascriptDomPaste = CefState.Enabled;
            //WebBrowserElement.BrowserSettings.Databases = CefState.Enabled;
            //WebBrowserElement.BrowserSettings.BackgroundColor = 0;
            ////WebDriver.FindElement(By.Name(nameof(WebBrowserElement)));
            //if (WebBrowserElement.DataContext != null) ((FrameworkElement)WebBrowserElement.DataContext).Opacity = 0.1d;
            TextBoxLink.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        ActivateWebViewUrl();
                        break;
                    case Key.Escape:
                        WebBrowserElement.Focus();
                        break;
                    default:
                        break;
                };
            };
            WebBrowserElement.SourceUpdated += (sender, e) =>
            {
                TextBoxLink.Text = WebBrowserElement.Uid;
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
            };
            Initialized += (sender, e) =>
            {
                string DefaultUrl = App.CurrentApp.SettingMainApplication.DefaultOpenUrlWebView;
                if (DefaultUrl.Length > 0)
                {
                    WebViewGoUrl(DefaultUrl);
                }
            };
            IELButtonReloadPage.OnActivateMouseLeft += (Key) =>
            {
                WebViewGoUrl(TextBoxLink.Text);
            };
            App.Log("Инициализация станицы браузера - Готово!");
        }

        internal void WebViewGoUrl(string Url)
        {
            TextBoxLink.Text = Url;
            ActivateWebViewUrl();
        }

        private void ActivateWebViewUrl()
        {
            try
            {
                WebBrowserElement.Source = new Uri(TextBoxLink.Text);
                WebBrowserElement.Focus();
                //webbrowserelement.address = textboxlink.text;
                ////if (webbrowserelement.corewebview2 == null) app.mainwindowapplication.activateloadingindicator();
                //webbrowserelement.focus();
            }
            catch
            {

            }
        }
    }
}
