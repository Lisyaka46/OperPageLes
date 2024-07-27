using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AAC20.Classes.Commands
{
    public partial interface ICommandAAC
    {
        /// <summary>
        /// Делегат события выполнения команды
        /// </summary>
        /// <param name="ParametersValue">Параметры команды</param>
        /// <returns>Итог выполнения команды</returns>
        public delegate Task<CommandStateResult> ExecuteCom(string[] ParametersValue);

        /// <summary>
        /// Действие которое выполняет команда
        /// </summary>
        internal event ExecuteCom Execute
        {
            add => Execute += value;
            remove => Execute -= value;
        }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        internal CommandStateResult ExecuteCommand(string[]? parameters);

        [GeneratedRegex(@"( |\*|,)([^,]|,,)+")]
        internal static partial Regex RegexParameterCommand();
        [GeneratedRegex(@"\b[^\*~!@#$<>,.\/\\?|'"";:`%^&*()\[\]{} \-=+]+\* ?")]
        internal static partial Regex RegexSortCommand();
    }
}
