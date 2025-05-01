using OperPage_les.UI.Pages.Browser;
using OperPage_les.Windows.Pages.ActionPanel;
using OperPage_les.Windows.Pages.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OperPage_les.CORE
{
    internal class PagesApplication()
    {
        /// <summary>
        /// Страница буфера в панели действий
        /// </summary>
        internal readonly PageLabels PageLabelsApplication = new();

        /// <summary>
        /// Страница разработчика
        /// </summary>
        internal readonly PageDeveloper PageDeveloper = new();

        /// <summary>
        /// Страница консоли команд
        /// </summary>
        internal readonly PageConsole PageConsoleApplication = new();
    }
}
