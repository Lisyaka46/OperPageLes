using AAC20.Windows.Pages.Discription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AAC20.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowDiscriptionCommands.xaml
    /// </summary>
    public partial class WindowDiscriptionCommands : Window
    {
        /// <summary>
        /// Структура всех страниц окна
        /// </summary>
        private readonly struct Pages
        {
            /// <summary>
            /// Страница описания всех консольных команд
            /// </summary>
            internal static readonly PageDiscriptionConsole DiscriptionConsole = new();
        }

        public WindowDiscriptionCommands()
        {
            InitializeComponent();
            Closing += (sender, e) => App.AppWindows.DiscriptionCommands = null;
            IELButtonConsole.OnActivateMouseLeft += (Mode) =>
            {

            };
        }
    }
}
