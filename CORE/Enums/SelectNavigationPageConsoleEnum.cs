using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperPageLes.CORE.Enums
{
    internal enum SelectNavigationPageConsoleEnum
    {
        /// <summary>
        /// Нет выделения навигации
        /// </summary>
        None = 0,

        /// <summary>
        /// Выделние навигации на команды буфера
        /// </summary>
        BufferCommandTextBox = 1,

        /// <summary>
        /// Выделение навигации на команды в подсказках
        /// </summary>
        HitCommands = 2,

    }
}
