using IEL.CORE.Classes.Browser;
using IEL.GUI;
using OperPageLes.CORE;
using OperPageLes.UI.Pages.Browser;
using System.Windows;
using System.Windows.Media.Animation;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowBrowserPagesManager.xaml
    /// </summary>
    public partial class DialogBrowserPagesManager : Window
    {
        /// <summary>
        /// Идёт ли создание объекта страницы
        /// </summary>
        private bool Creating = false;

        /// <summary>
        /// Состояние добавляемого объекта браузера страниц
        /// </summary>
        private BrowserPage? AppendElementPage = null;

        /// <summary>
        /// Объект анимации для управления double значением
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimate = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        public DialogBrowserPagesManager()
        {
            InitializeComponent();
            KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        Close();
                        break;
                }
            };

            IELButtonCancel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Close();
            };

            #region IELButtonAddPageLabel
            IELButtonAddPageLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (Creating) return;
                Creating = true;
                AppendElementPage = new(new PageLabels(), "Ярлыки", "Средство быстрого выполнения командных инструкций в программе");
                AppendElementPage.Disposed += (sender) =>
                {

                };
                Close();
            };
            #endregion

            #region IELButtonPageDeveloper
            IELButtonPageDeveloper.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageDeveloper page = new();
                AppendElementPage = new(page, "Страница разработчика", null);
                AppendElementPage.Disposed += (sender) =>
                {

                };
                Close();
            };
            #endregion

            #region IELButtonPageConsole
            IELButtonPageConsole.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageConsole page = new();
                AppendElementPage = new(page, "Консоль", "Гибкий инструмент управления программой с помощью вводимых команд");
                AppendElementPage.Disposed += (sender) =>
                {

                };
                Close();
            };
            #endregion

            #region IELButtonPageBrowser
            IELButtonPageBrowser.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageWebBrowser page = new();
                AppendElementPage = new(page, "Веб-браузер", null);
                AppendElementPage.Disposed += (sender) =>
                {
                    page.WebBrowserElement.Dispose();
                };
                Close();
            };
            #endregion
        }

        internal static new void Show() => throw new Exception("Данное окно нельзя открыть не использовав специальный метод перегрузки с объектом привязки браузера");
        internal static new void ShowDialog() => throw new Exception("Данное окно нельзя открыть не использовав специальный метод перегрузки с объектом привязки браузера");

        /// <summary>
        /// Добавить в браузер страниц новую вкладку и активировать страницу
        /// </summary>
        /// <param name="BrowserPage">Браузер для взаимодействия</param>
        /// <returns>Успешно или нет</returns>
        internal BrowserPage? AddNewPageInBrowser(IELBrowserPage browserPage)
        {
            Opacity = 0d;
            DoubleAnimation animation = DoubleAnimate.Clone();
            //animation.BeginTime = TimeSpan.FromMilliseconds(10d);
            animation.Duration = TimeSpan.FromMilliseconds(670d);
            animation.From = 0d;
            animation.To = 0.97d;
            BeginAnimation(OpacityProperty, animation, HandoffBehavior.SnapshotAndReplace);

            IELButtonAddPageLabel.IsEnabled = browserPage.SearchPageType<PageLabels>() == null;
            IELButtonPageDeveloper.IsEnabled = browserPage.SearchPageType<PageDeveloper>() == null;

            base.ShowDialog();
            return AppendElementPage;
        }
    }
}
