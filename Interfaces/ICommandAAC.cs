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
        /// Параметры команды
        /// </summary>
        public Parameter[]? Parameters { get; }

        /// <summary>
        /// Название команды
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Описание команды
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        internal CommandStateResult ExecuteCommand(string[]? parameters);

        /// <summary>
        /// Узнать написаны ли обязательные параметры команды
        /// </summary>
        /// <param name="WritingParameters">Написанные параметры</param>
        /// <returns>Совпадает правилу или нет</returns>
        internal abstract bool AbsolutlyRequiredParameters(string[]? WritingParameters);

        [GeneratedRegex(@"[^*]+")]
        internal static partial Regex RegexNameCommand();
        [GeneratedRegex(@"\*.*")]
        internal static partial Regex RegexParameterCommand();
        [GeneratedRegex(@"([^%,]+|%,|%%)+")]
        internal static partial Regex RegexSortParamCommand();
    }
}
