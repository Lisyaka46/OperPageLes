using IEL;
using IEL.CORE.Classes.Browser;
using Microsoft.Maui.Controls.Xaml;
using OperPage_les.CORE;
using OperPage_les.UI.Pages.Browser;
using OperPage_les.Windows.Pages.Browser;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using System.Xaml;
using Microsoft.Build.Utilities;

namespace OperPage_les.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowBrowserPagesManager.xaml
    /// </summary>
    public partial class WindowBrowserPagesManager : Window
    {
        /// <summary>
        /// Фоновое создание страницы
        /// </summary>
        private readonly UpdateBackgroundData CreatingBackgroundPage;

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

        public WindowBrowserPagesManager()
        {
            InitializeComponent();
            CreatingBackgroundPage = new(delegate { });
            KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        Close();
                        break;
                }
            };

            IELButtonCancel.OnActivateMouseLeft += (sender, e, Key) => Close();
            #region IELButtonAddPageLabel
            IELButtonAddPageLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (Creating) return;
                Creating = true;
                App.MainWindowApplication.ActivateLoadingIndicator();
                CreatingBackgroundPage.EventElapsed = (sender, e) => Dispatcher.BeginInvoke(() =>
                {
                    if (App.CurrentApp.MainPageLabels == null) App.CurrentApp.MainPageLabels = new();
                    AppendElementPage = new(App.CurrentApp.MainPageLabels, "Ярлыки", "Средство быстрого выполнения командных инструкций в программе");
                    AppendElementPage.Disposed += (sender) =>
                    {

                    };
                    App.MainWindowApplication.DiactivateLoadingIndicator();
                    Close();
                });
                CreatingBackgroundPage.Start();
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

            #region NEW
            IELButtonGenerate.OnActivateMouseLeft += (sender, e, Key) =>
            {
                //#region C#
                //CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                //CompilerParameters cp = new CompilerParameters();
                //// Generate an executable instead of
                //// a class library.
                //cp.GenerateExecutable = false;

                //// Specify the assembly file name to generate.
                ////cp.OutputAssembly = exeName;

                //// Save the assembly as a physical file.
                //cp.GenerateInMemory = false;

                //// Set whether to treat all warnings as errors.
                //cp.TreatWarningsAsErrors = false;

                //CompilerResults cr = provider.(cp,
                //    @"C:\Users\killm\Рабочий стол\PageTest.xaml.cs");
                //#endregion
                //Microsoft.Build.Utilities.
                //string Xaml = File.ReadAllText(@"C:\Users\killm\Рабочий стол\PageTest.xaml");
                //Assembly assembly = System.Reflection.Assembly.LoadFile(@"C:\Users\killm\Рабочий стол\PageTest.xaml.cs");

                //string defaultNamespace = "Test";
                //string folderName = "UI.Pages";
                //string fileName = "PageTest.xaml";

                //string path = String.Format("{0}.{1}.{2}", defaultNamespace, folderName, fileName);
                //XamlObjectReader reader = new()
                //XamlObjectWriterSettings.RootObjectInstance = assembly;
                //object page = XamlServices.Load(@"C:\Users\killm\Рабочий стол\PageTest.xaml");
                // new System.Windows.Controls.Page().LoadFromXaml(Xaml);
                //object XAMLObj = System.Xaml.XamlServices.Load(@"C:\Users\killm\Рабочий стол\PageTest.xaml");
                //Page root = (Page)App.LoadComponent(new Uri("pack://application:,,,/WpfApp1;component/PageTest.xaml", UriKind.Relative));

                    //XmlReader xmlReader = new XmlReader(@"C:\Users\killm\Рабочий стол\PageTest.xaml");
                    //UIElement elementLoaded = (UIElement)XamlReader.Load(xmlReader);
                    //System.Windows.Resources.StreamResourceInfo res =
                    //    System.Windows.Application.GetResourceStream(new(@"C:\Users\killm\Рабочий стол\PageTest.xaml", UriKind.Absolute));
                    //CompilerResults PageCompuler = cr.LoadFromXaml(Xaml);
                    //Assembly assembly = PageCompuler.CompiledAssembly;
                    //Type classType = assembly.GetType("Page") ?? throw new Exception("Скомпилированный файл не имеет ожидаемый тип Page");
                    //Page SourcePage = (Page)(Activator.CreateInstance(classType) ?? throw new Exception("Скомпилированный файл неконвертируемый тип Page"));
                //AppendElementPage = new((System.Windows.Controls.Page)page, "TEST", null);
                //AppendElementPage.Disposed += (sender) =>
                //{

                //};
                //Close();
            };
            #endregion
        }

        internal static new void Show() => throw new Exception("Данное окно нельзя открыть не использовав специальный метод перегрузки с объектом привязки браузера");
        internal static new void ShowDialog() => throw new Exception("Данное окно нельзя открыть не использовав специальный метод перегрузки с объектом привязки браузера");

        internal void Show(IELBrowserPage browserPage)
        {
            IELButtonAddPageLabel.IsEnabled = browserPage.SearchPageType<PageLabels>() == null;
            IELButtonPageDeveloper.IsEnabled = browserPage.SearchPageType<PageDeveloper>() == null;
            base.Show();
        }

        internal void ShowDialog(IELBrowserPage browserPage)
        {
            IELButtonAddPageLabel.IsEnabled = browserPage.SearchPageType<PageLabels>() == null;
            IELButtonPageDeveloper.IsEnabled = browserPage.SearchPageType<PageDeveloper>() == null;
            base.ShowDialog();
        }

        /// <summary>
        /// Добавить в браузер страниц новую вкладку и активировать страницу
        /// </summary>
        /// <param name="BrowserPage">Браузер для взаимодействия</param>
        /// <returns>Успешно или нет</returns>
        internal BrowserPage? AddNewPageInBrowser(IELBrowserPage browserPage)
        {
            Opacity = 0d;
            DoubleAnimation animation = DoubleAnimate.Clone();
            animation.BeginTime = TimeSpan.FromMilliseconds(10d);
            animation.Duration = TimeSpan.FromMilliseconds(670d);
            animation.From = 0d;
            animation.To = 0.97d;
            BeginAnimation(OpacityProperty, animation);
            ShowDialog(browserPage);
            return AppendElementPage;
        }
    }
}
