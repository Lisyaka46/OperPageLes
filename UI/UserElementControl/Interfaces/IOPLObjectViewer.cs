using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationOperPageLes.UI.UserElementControl.Interfaces
{
    internal interface IOPLObjectViewer<T> where T : IFormattable
    {
        /// <summary>
        /// Объект данных отображаючий отображаемое содержимое
        /// </summary>
        public T SourceView { get; }

    }
}
