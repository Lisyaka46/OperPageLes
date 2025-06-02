using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperPage_les.CORE.Enums
{
    internal enum LabelTagEnum
    {
        /// <summary>
        /// Обычный тег
        /// </summary>
        Default = 0,

        /// <summary>
        /// Высший тег
        /// </summary>
        Kind = 1,

        /// <summary>
        /// Папочный тег
        /// </summary>
        Folder = 2,

        /// <summary>
        /// Ссылочный тег
        /// </summary>
        Link = 3,

        /// <summary>
        /// Уникальный тег
        /// </summary>
        Unique = 4,
    }
}
