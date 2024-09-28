using AAC20.Classes;
using AAC20.Classes.Flaging;
using AAC20.Windows;
using Interpreter.Commands;
using System.Diagnostics;
using System.Net.NetworkInformation;
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
        private readonly ThreadGenericWhileProcess ThreadInternetCheckConnection;

        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        internal static readonly Flag InternetPinging = new(false);

        public App()
        {
            ThreadInternetCheckConnection = new(CheckInternetConnection, 900);
            ThreadInternetCheckConnection.Start();
        }

        /// <summary>
        /// Перезагрузить программу
        /// </summary>
        internal static void RebootApplication()
        {
            Process.Start(Process.GetCurrentProcess().ProcessName, Environment.GetCommandLineArgs());
            Current.Shutdown(0);
        }

        //
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
    }
}
