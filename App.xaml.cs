using AAC20.Classes;
using Interpreter.Commands;
using AAC20.Windows;
using System.Diagnostics;
using System.Windows;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Http;
using AAC20.Classes.Flaging;
using AAC20.Classes.Labels;

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
        /// Объект пинговки сайта
        /// </summary>
        private readonly Ping ObjPing = new();

        /// <summary>
        /// Поток обновляемый данные интернета
        /// </summary>
        private readonly Thread ThreadInternetCheckConnection;

        /// <summary>
        /// Состояние подключения к интернету
        /// </summary>
        internal static readonly Flag InternetPinging = new(false);

        public App()
        {
            ThreadInternetCheckConnection = new Thread(delegate ()
            {
                while (true)
                {
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
                    Thread.Sleep(1000);
                }
            });
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

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="e">Объект события начала работы прораммы</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            Current.MainWindow = new MainWindow();
            MainWindowApplication.Show();
        }
    }
}
