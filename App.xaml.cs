using AAC20.CORE;
using AAC20.CORE.Flaging;
using AAC20.CORE.Settings;
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
        /// Главное окно програмы
        /// </summary>
        internal static UI.Windows.MainWindow MainWindowApplication => (UI.Windows.MainWindow)Current.MainWindow;

        /// <summary>
        /// Поток обновляемый данные интернета
        /// </summary>
        private readonly ThreadGenericProcess ThreadInternetCheckConnection;

        /// <summary>
        /// Экземпляр созданного приложения
        /// </summary>
        internal static App CurrentApp => (App)Current;

        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        internal static readonly Flag InternetPinging = new(false);

        /// <summary>
        /// Массив ключей настроек <b>процесса</b>
        /// </summary>
        private Setting<EnumSettingProcess> SettingProcess;

        /// <summary>
        /// Массив ключей настроек <b>приложения</b>
        /// </summary>
        internal Setting<EnumSettingApplication> SettingApplication;

        /// <summary>
        /// Константа директории файла настроек <b>процесса</b>
        /// </summary>
        private const string PathSettingProcess = "CurrentSettings.so";

        /// <summary>
        /// Имя файла настроек <b>приложения</b>
        /// </summary>
        private const string NameFileApplicationSetting = "ApplicationSettings";

        /// <summary>
        /// Релятивная директория папки изображений приложения
        /// </summary>
        internal const string PathImageApplication = "/UI/Images";

        public App()
        {
            ThreadInternetCheckConnection = new(CheckInternetConnection, 900);
            ThreadInternetCheckConnection.Start();
            SettingProcess = new(PathSettingProcess,
            [
                // SettingApplicationPath
                $"{NameFileApplicationSetting}.so",
            ]);
            SettingApplication = new(SettingProcess.GetSettingValue(EnumSettingProcess.SettingApplicationPath),
            [
                // PathMenuImage
                "!"
            ]);
        }

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="e">Объект события начала работы прораммы</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            Current.MainWindow = new UI.Windows.MainWindow();
            Current.Exit += (sender, e) =>
            {
                ThreadInternetCheckConnection.Kill();
            };
            MainWindowApplication.Show();
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
    }
}
