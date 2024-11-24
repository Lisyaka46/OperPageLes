using AAC20.Windows.Pages.ActionPanel;
using AAC20.Windows.Pages.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AAC20.CORE
{
    internal class PagesApplication()
    {
        /// <summary>
        /// Страница буфера в панели действий
        /// </summary>
        internal readonly PageBufferActionPanel PageBuffer = new(App.HeightButtonBuffer);

        /// <summary>
        /// Страница буфера в панели действий
        /// </summary>
        internal readonly PageLabels PageLabelsApplication = new();

        /// <summary>
        /// Страница разработчика
        /// </summary>
        internal readonly PageDeveloper PageDeveloper = new();
    }
}
