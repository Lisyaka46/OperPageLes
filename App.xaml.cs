using AAC20.Classes;
using AAC20.Classes.Commands;
using AAC20.GUI;
using AAC20.Interfaces;
using AAC20.Windows;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

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
            /// <summary>
            /// Флаг активации правого нажатия с помощью кнопки CTRL в панели действий
            /// </summary>
            internal static readonly Flag FlagCtrlActivateActionButtonAltMode = new(false);
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
            new ConsoleCommand("reboot", "Перезагружает программу", (param) =>
            {
                RebootApplication();
                return Task.FromResult(CommandStateResult.Completed);
            }),

            new ConsoleCommand("close", "Закрывает программу", (param) =>
            {
                Current.Shutdown(0);
                return Task.FromResult(CommandStateResult.Completed);
            }),
        ];

        /// <summary>
        /// Буфер объектов команд
        /// </summary>
        internal static Classes.Buffer BufferCommand = new();

        /// <summary>
        /// Главное окно програмы
        /// </summary>
        internal static MainWindow MainWindowApplication => (MainWindow)Current.MainWindow;

        /// <summary>
        /// Перезагрузить программу
        /// </summary>
        internal static void RebootApplication()
        {
            Process.Start(Process.GetCurrentProcess().ProcessName, Environment.GetCommandLineArgs());
            Current.Shutdown(0);
        }
    }
}
