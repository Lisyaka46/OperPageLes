using DataScroll;
using IEL;
using OperPage_les.UI.Pages.Browser;
using OperPage_les.Windows.Pages.Browser;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using IEL.CORE.Classes.Browser;

namespace OperPage_les.UI.Dialogs
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

        //[LibraryImport("dwmapi.dll", StringMarshalling = StringMarshalling.Utf8, SetLastError = false)]
        //private static partial int DwmSetWindowAttribute
        //    (IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

        /// <summary>
        /// Скроллбар выбора ярлыков
        /// </summary>
        private readonly CounterScrollBar ScrollBar;

        /// <summary>
        /// Объект браузера страниц
        /// </summary>
        public IELBrowserPage? MainBrowserPage;

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

            //IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            //DWM_WINDOW_CORNER_PREFERENCE DWMWCP_ROUND = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
            //Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref DWMWCP_ROUND, sizeof(uint)));

            ScrollBar = new(1);
            KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        Close();
                        break;
                }
            };

            IELButtonCancel.OnActivateMouseLeft += (Key) => Close();
            #region IELButtonAddPageLabel
            IELButtonAddPageLabel.OnActivateMouseLeft += (Key) =>
            {
                if (MainBrowserPage != null)
                {
                    MainBrowserPage.AddInlayPage(new BrowserPage(new PageLabels()), "Ярлыки",
                        "Ярлыки которые предаставляются программой для хранения важных команд.");
                    Cancel = false;
                }
                Close();
            };
            #endregion

            #region IELButtonPageDeveloper
            IELButtonPageDeveloper.OnActivateMouseLeft += (Key) =>
            {
                if (MainBrowserPage != null)
                {
                    MainBrowserPage.AddInlayPage(new BrowserPage(new PageDeveloper()), "Страница разработчика",
                        "Страница не предоставляется для обычных пользователей. " +
                        "Взаимодействие со страницей может повлечь за собой непредвиденное реагирование программы.");
                    Cancel = false;
                }
                Close();
            };
            #endregion

            #region IELButtonPageConsole
            IELButtonPageConsole.OnActivateMouseLeft += (Key) =>
            {
                if (MainBrowserPage != null)
                {
                    MainBrowserPage.AddInlayPage(new BrowserPage(new PageConsole()), "Консоль",
                        "Консоль программы для более гибкой настройки и взаимодействия с программой.");
                    Cancel = false;
                }
                Close();
            };
            #endregion

            #region IELButtonPageBrowser
            IELButtonPageBrowser.OnActivateMouseLeft += (Key) =>
            {
                App.Log("Создаю браузер.");
                if (MainBrowserPage != null)
                {
                    App.Log("Успешная проверка на наличие браузера страниц");
                    MainBrowserPage.AddInlayPage(new BrowserPage(new PageWebBrowser()), "Веб-браузер");
                    Cancel = false;
                }
                App.Log("Инициализация готова!");
                Close();
            };
            #endregion

            Loaded += (sender, e) =>
            {
                IELButtonAddPageLabel.IsEnabled = MainBrowserPage?.SearchPageType<PageLabels>() == null;
                IELButtonPageDeveloper.IsEnabled = MainBrowserPage?.SearchPageType<PageDeveloper>() == null;
            };
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
            this.MainBrowserPage = BrowserPage;
            BeginAnimation(OpacityProperty, animation);
            ShowDialog();
            return !Cancel;
        }
    }
}
