using DataScroll;
using IEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AAC20.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowBrowserPagesManager.xaml
    /// </summary>
    public partial class WindowBrowserPagesManager : Window
    {
        private enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        private enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [LibraryImport("dwmapi.dll", StringMarshalling = StringMarshalling.Utf8, SetLastError = false)]
        private static partial int DwmSetWindowAttribute
            (IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

        /// <summary>
        /// Скроллбар выбора ярлыков
        /// </summary>
        private readonly CounterScrollBar ScrollBar;

        /// <summary>
        /// Объект браузера страниц
        /// </summary>
        public IELBrowserPage? BrowserPage;

        /// <summary>
        /// Состояние отмены
        /// </summary>
        private bool Cancel = true;

        /// <summary>
        /// Объект анимации для управления double значением
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimate = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        public WindowBrowserPagesManager()
        {
            InitializeComponent();

            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            DWM_WINDOW_CORNER_PREFERENCE DWMWCP_ROUND = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
            Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref DWMWCP_ROUND, sizeof(uint)));

            ScrollBar = new(1);

            #region IELButtonAddPageLabel
            IELButtonAddPageLabel.OnActivateMouseLeft += () =>
            {
                if (BrowserPage != null)
                {
                    BrowserPage.AddInlayPage(App.CurrentApp.AllPages.PageLabelsApplication, "Ярлыки",
                        "Ярлыки которые предаставляются программой для хранения важных команд");
                    Cancel = false;
                }
                Close();
            };
            IELButtonCancel.OnActivateMouseLeft += Close;
            #endregion

            #region IELButtonPageDeveloper
            IELButtonPageDeveloper.OnActivateMouseLeft += () =>
            {
                if (BrowserPage != null)
                {
                    BrowserPage.AddInlayPage(App.CurrentApp.AllPages.PageDeveloper, "Страница разработчика",
                        "Страница не предоставляется для обычных пользователей. " +
                        "Взаимодействие со страницей может повлечь за собой непредвиденное реагирование программы");
                    Cancel = false;
                }
                Close();
            };
            #endregion
        }

        /// <summary>
        /// Добавить в браузер страниц новую вкладку и активировать страницу
        /// </summary>
        /// <param name="BrowserPage">Браузер для взаимодействия</param>
        /// <returns>Успешно или нет</returns>
        public bool AddNewPageInBrowser(IELBrowserPage BrowserPage)
        {
            Opacity = 0d;
            DoubleAnimation animation = DoubleAnimate.Clone();
            animation.BeginTime = TimeSpan.FromMilliseconds(10d);
            animation.Duration = TimeSpan.FromMilliseconds(1200d);
            animation.From = 0d;
            animation.To = 0.97d;
            this.BrowserPage = BrowserPage;
            BeginAnimation(OpacityProperty, animation);
            ShowDialog();
            return !Cancel;
        }
    }
}
