using System.Diagnostics;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;

namespace AAC20.Classes.Commands
{
    /// <summary>
    /// Консольная команда
    /// </summary>
    public partial class ConsoleCommand
    {
        /// <summary>
        /// Делегат события выполнения команды
        /// </summary>
        /// <param name="ParametersValue">Параметры команды</param>
        /// <returns>Итог выполнения команды</returns>
        public delegate Task<CommandStateResult> ExecuteCom(string[] ParametersValue);

        /// <summary>
        /// Имя команды
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Описание консольной команды
        /// </summary>
        public readonly string Explanation;

        /// <summary>
        /// Параметры команды
        /// </summary>
        public readonly Parameter[]? Parameters;

        /// <summary>
        /// Действие которое выполняет команда
        /// </summary>
        private event ExecuteCom Execute;

        /// <summary>
        /// Инициализировать объект консольной команды с параметрами
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Parameters">Параметры команды</param>
        /// <param name="Explanation">Описание команды</param>
        /// <param name="Execute">Действие выполнения</param>
        public ConsoleCommand(string Name, Parameter[] Parameters, string Explanation, ExecuteCom Execute)
        {
            this.Name = Name;
            this.Parameters = Parameters;
            this.Explanation = Explanation;
            this.Execute = Execute;

        }

        /// <summary>
        /// Инициализировать объект консольной команды без параметров
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Explanation">Описание команды</param>
        /// <param name="Execute">Действие выполнения</param>
        public ConsoleCommand(string Name, string Explanation, ExecuteCom Execute)
        {
            this.Name = Name;
            this.Explanation = Explanation;
            this.Execute = Execute;
        }

        /// <summary>
        /// Прочитать и выполнить команду
        /// </summary>
        /// <param name="ConsoleCommands">Массив поиска консольных команд</param>
        /// <param name="TextCommand">Читаемая команда</param>
        public static CommandStateResult ReadAndExecuteCommand(ConsoleCommand[] ConsoleCommands, string TextCommand)
        {
            string[]? Parameters = null;
            while (TextCommand.Length > 0)
            {
                if (TextCommand[^1] == ' ') TextCommand = TextCommand.Remove(TextCommand.Length - 1);
                else if (TextCommand.Contains("  ")) TextCommand = TextCommand.Replace("  ", " ");
                else break;
            }
            if (TextCommand.Contains('*') && TextCommand[^1] != '*') // command* param1, param2, param3 ...
            {
                if (TextCommand[TextCommand.IndexOf('*') + 1] != ' ') TextCommand = TextCommand.Replace("*", "* ");
                TextCommand = TextCommand[0..TextCommand.IndexOf('*')].Replace(" ", "_").ToLower() + TextCommand[TextCommand.IndexOf('*')..];
                Parameters = [.. RegexParameterCommand().Matches(TextCommand).Select(i => i.Value[2..])];
                TextCommand = RegexSortCommand().Match(TextCommand).Value.ToString().Replace("*", string.Empty).Replace(" ", string.Empty);
            }
            else // command
            {
                TextCommand = TextCommand.Replace(" ", "_").Replace("*", string.Empty).ToLower();
            }
            ConsoleCommand? SearchCommand = ConsoleCommands.SingleOrDefault(i => i.Name.Equals(TextCommand));
            return SearchCommand?.ExecuteCommand(Parameters).Result ?? new(Commands.ResultState.Failed, $"Invalid command \"{TextCommand}\"", $"Команда \"{TextCommand}\" не найдена");
        }

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        public async Task<CommandStateResult> ExecuteCommand(string[]? parameters) => await Execute.Invoke(parameters ?? []);

        [GeneratedRegex(@"( |\*|,)([^,]|,,)+")]
        private static partial Regex RegexParameterCommand();
        [GeneratedRegex(@"\b[^\*~!@#$<>,.\/\\?|'"";:`%^&*()\[\]{} \-=+]+\* ?")]
        private static partial Regex RegexSortCommand();
    }
}
