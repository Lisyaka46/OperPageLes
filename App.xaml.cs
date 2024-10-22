using AAC20.Classes;
using AAC20.Classes.Flaging;
using AAC20.Windows;
using Interpreter.Commands;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows;

namespace AAC20
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Структура флагов программы
        /// </summary>
        internal readonly struct AppFlags
        {
            
        }

        /// <summary>
        /// Структура всех окон программы
        /// </summary>
        internal readonly struct AppWindows
        {
            /// <summary>
            /// Окно описания всех команд
            /// </summary>
            internal static WindowDiscriptionCommands? DiscriptionCommands = null;
        }

        /// <summary>
        /// Массив консольных команд
        /// </summary>
        internal static readonly List<ConsoleCommand> DataConsoleCommand =
        [
            new ConsoleCommand("reboot", "Перезагружает программу", (Command, param) =>
            {
                RebootApplication();
                return Task.FromResult(CommandStateResult.Completed(Command.Name));
            }),

            new ConsoleCommand("close", "Закрывает программу", (Command, param) =>
            {
                Current.Shutdown(0);
                return Task.FromResult(CommandStateResult.Completed(Command.Name));
            }),
        ];

        /// <summary>
        /// Буфер объектов команд
        /// </summary>
        internal static Interpreter.Classes.Buffer BufferCommand = new(50);

        /// <summary>
        /// Главное окно програмы
        /// </summary>
        internal static MainWindow MainWindowApplication => (MainWindow)Current.MainWindow;

        /// <summary>
        /// Поток обновляемый данные интернета
        /// </summary>
        private readonly ThreadGenericProcess ThreadInternetCheckConnection;

        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        internal static readonly Flag InternetPinging = new(false);

        /// <summary>
        /// Массив ключей настроек <b>процесса</b>
        /// </summary>
        private Dictionary<string, string> SettingProcess;

        /// <summary>
        /// Массив ключей настроек <b>приложения</b>
        /// </summary>
        internal Dictionary<string, string> SettingApplication;

        /// <summary>
        /// Константа директории файла настроек <b>процесса</b>
        /// </summary>
        private const string PathSettingProcess = "CurrentSettings.so";

        /// <summary>
        /// Имя файла настроек <b>приложения</b>
        /// </summary>
        private const string NameFileApplicationSetting = "ApplicationSettings";

        /// <summary>
        /// Экземпляр созданного приложения
        /// </summary>
        internal static App CurrentApp => (App)Current;

        public App()
        {
            ThreadInternetCheckConnection = new(CheckInternetConnection, 900);
            ThreadInternetCheckConnection.Start();
            if (!File.Exists(PathSettingProcess))
            {
                File.WriteAllLines(PathSettingProcess,
                    [
                        $"SettingApplicationPath:{NameFileApplicationSetting}.so",
                    ]);
            }
            SettingProcess = ReadSettingFile(PathSettingProcess);
            string PathSetting = SettingProcess["SettingApplicationPath"];
            if (!File.Exists(PathSetting))
            {
                File.WriteAllLines(PathSetting,
                    [
                        $"SettingApplicationPath:{NameFileApplicationSetting}.so",
                        "PathMenuImage:!"
                    ]);
            }
            SettingApplication = ReadSettingFile(PathSetting);
        }

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="e">Объект события начала работы прораммы</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            Current.MainWindow = new MainWindow();
            Current.Exit += (sender, e) =>
            {
                ThreadInternetCheckConnection.Kill();
            };
            MainWindowApplication.Show();
        }

        /// <summary>
        /// Прочитать файл настроек
        /// </summary>
        /// <param name="Path">Директория файла настроек .so</param>
        /// <returns>Массив ключей объектов настроек</returns>
        private Dictionary<string, string> ReadSettingFile(string Path)
        {
            Dictionary<string, string> Result = [];
            string[] LinesText = File.ReadAllLines(Path);
            lock (Result)
            {
                Regex 
                    regexName = RegexNameSettingParameter(),
                    regexValue = RegexValueSettingParameter();
                foreach (string Line in LinesText)
                {
                    // Text:Value
                    Result.Add(regexName.Match(Line).Value[..^1], regexValue.Match(Line).Value[1..]);
                }
            }
            return Result;
        }

        /// <summary>
        /// Перезагрузить программу
        /// </summary>
        internal static void RebootApplication()
        {
            Process.Start(Process.GetCurrentProcess().ProcessName, Environment.GetCommandLineArgs());
            Current.Shutdown(0);
        }

        /// <summary>
        /// Проверка подключения интернета
        /// </summary>
        private static void CheckInternetConnection()
        {
            Ping ObjPing = new();
            try
            {
                InternetPinging.Wait = true;
                PingReply reply = ObjPing.SendPingAsync("yandex.ru", 800).Result;
                InternetPinging.Wait = false;
                InternetPinging.Value = reply.Status == IPStatus.Success;
            }
            catch
            {
                InternetPinging.Wait = false;
                InternetPinging.Value = false;
            }
        }

        [GeneratedRegex("\\b[^:]+:")]
        private static partial Regex RegexNameSettingParameter();

        [GeneratedRegex(":[^\n]+")]
        private static partial Regex RegexValueSettingParameter();
    }
}
