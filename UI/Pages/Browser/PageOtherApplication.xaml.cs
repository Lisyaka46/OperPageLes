using IEL.Interfaces.Core;
using Microsoft.CSharp;
using Microsoft.Maui.Controls.Xaml;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using System.Xaml;
using Microsoft.CodeAnalysis;


namespace OperPage_les.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageOtherApplication.xaml
    /// </summary>
    public partial class PageOtherApplication : Page
    {
        /// <summary>
        /// Процесс приложения который исполняет страница
        /// </summary>
        internal Process? CurrentProcessApplicaton { get; set; } = null;

        private readonly Exception ExceptionNullProcess = new("Невозможно произвести манипуляцию, процесс является нулевым!");
        private readonly Exception ExceptionRunningProcess = new("Невозможно произвести манипуляцию, текущий процесс является запущенным!");

        public PageOtherApplication()
        {
            InitializeComponent();
            ParentApplication.Opacity = 0d;
            IELButtonTextActivate.OnActivateMouseLeft += () =>
            {
                //Uri uri = new Uri(@"C:\Users\killm\Рабочий стол\Main\Programm\С#\Project OperPage les\OperPage_les\UI\Pages\Test.xaml", UriKind.Relative);
                //StreamResourceInfo info = App.GetResourceStream(uri);
                //System.Windows.Markup.XamlReader reader = new System.Windows.Markup.XamlReader();
                //Page page = (Page)reader.LoadAsync(info.Stream);
                //ParentApplication.Content = page;
//                StreamReader reader = new(@"C:\Users\killm\Рабочий стол\Main\Programm\С#\WpfApp1\MainWindow.xaml.cs");
//                string CsData = reader.ReadToEnd();
//                reader.Close();
//                CSharpCodeProvider codeProvider = new();

//#pragma warning disable CS0618 // Тип или член устарел
//                ICodeCompiler icc = codeProvider.CreateCompiler();
//#pragma warning restore CS0618 // Тип или член устарел

//                CompilerParameters parameters = new CompilerParameters
//                {
//                    //Make sure we generate an EXE, not a DLL
//                    GenerateExecutable = false,
//                    OutputAssembly = "Output",

//                };
                //CompilerResults results = codeProvider.CompileAssemblyFromSource(new(), @"C:\Users\killm\Рабочий стол\Main\Programm\С#\WpfApp1\MainWindow.xaml.cs");
                //ParentApplication.Content = XamlServices.Load(@"C:\Users\killm\Рабочий стол\Main\Programm\С#\WpfApp1\MainWindow.xaml") as IPageDefault;
                StreamReader reader = new StreamReader(@"C:\Users\killm\Рабочий стол\Main\Programm\С#\WpfApp1\MainWindow.xaml");
                string XamlData = reader.ReadToEnd();
                reader.Close();
                ParentApplication.LoadFromXaml(XamlData);
                DoubleAnimation animation = new(0, TimeSpan.FromMilliseconds(250d))
                {
                    DecelerationRatio = 0.2d,
                    EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut },
                    To = 1d
                };
                ParentApplication.BeginAnimation(OpacityProperty, animation);
                //ActivateProcess();
            };
        }

        #region Manipulate Process
        /// <summary>
        /// Запустить новый процесс
        /// </summary>
        /// <param name="process">Объект процесса приложения</param>
        /// <returns>Совершён или нет запуск процесса</returns>
        /// <exception cref="Exception">Исключение при незавершённом процессе</exception>
        internal bool ActivateProcess(Process process)
        {
            if (CurrentProcessApplicaton != null) CloseProcess();
            CurrentProcessApplicaton = process;
            return ActivateProcess();
        }

        /// <summary>
        /// Запустить присвоенный процесс
        /// </summary>
        /// <returns>Совершён или нет запуск процесса</returns>
        /// <exception cref="Exception">Исключение при незавершённом процессе</exception>
        internal bool ActivateProcess()
        {
            if (CurrentProcessApplicaton == null) throw ExceptionNullProcess;
            //if (!CurrentProcessApplicaton.HasExited) throw ExceptionRunningProcess;
            CurrentProcessApplicaton.StartInfo.CreateNoWindow = true;
            CurrentProcessApplicaton.StartInfo.UseShellExecute = false;
            CurrentProcessApplicaton.StartInfo.UseCredentialsForNetworkingOnly = false;

            //CurrentProcessApplicaton.WaitForInputIdle(700);
            CurrentProcessApplicaton.Start();
            //SetParent(CurrentProcessApplicaton.MainWindowHandle, WindowManipulateApplication.Handle);
            CurrentProcessApplicaton.Refresh();
            _ = CurrentProcessApplicaton.MainWindowHandle;
            return true;
        }

        /// <summary>
        /// Закрыть процесс если он запущен
        /// </summary>
        internal void CloseProcess()
        {
            if (CurrentProcessApplicaton == null) throw ExceptionNullProcess;
            if (CurrentProcessApplicaton.HasExited) return;
            CurrentProcessApplicaton.Close();
        }
        #endregion
    }
}
