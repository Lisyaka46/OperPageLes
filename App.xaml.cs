using AAC20.Classes.Commands;
using AAC20.GUI;
using AAC20.Interfaces;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AAC20
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
                Environment.Exit(0);
                return Task.FromResult(CommandStateResult.Completed);
            }),
        ];

        /// <summary>
        /// Буфер объектов команд
        /// </summary>
        internal static Classes.Buffer BufferCommand = new();

        /// <summary>
        /// Перезагрузить программу
        /// </summary>
        internal static void RebootApplication()
        {
            Process.Start(Process.GetCurrentProcess().ProcessName, Environment.GetCommandLineArgs());
            Current.Shutdown();
        }

        /// <summary>
        /// Активировать кнопку типа "IELButtonText" в странице
        /// </summary>
        /// <param name="VisualObject">Ссылка на объект поиска</param>
        /// <param name="key">Ключ клавиши</param>
        internal static void ActivateButtonInKey(Visual VisualObject, Key key)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(VisualObject); i++)
            {
                Visual ChildVisualElement = (Visual)VisualTreeHelper.GetChild(VisualObject, i);
                if (ChildVisualElement.GetType() == typeof(IELButtonText))
                {
                    IELButtonText Button = (IELButtonText)ChildVisualElement;
                    if (Button.CharKeyKeyboard == key) Button.OnActivate?.Invoke(true);
                }
                else
                {
                    ActivateButtonInKey(ChildVisualElement, key);
                }
            }
            //return false;
            //throw new Exception($"Ключ клавиши \"{key}\" не имеет не одна кнопка, в данном случае выведено исключение");
        }
    }
}
