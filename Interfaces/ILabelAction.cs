using Interpreter.Commands;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAC20.Interfaces
{
    public interface ILabelAction
    {
        /// <summary>
        /// Имя ярлыка
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Описчание ярлыка
        /// </summary>
        internal string? Description { get; }

        /// <summary>
        /// Команда реализуемая ярлыком
        /// </summary>
        internal string Command { get; }
    }
}
