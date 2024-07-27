using System.Diagnostics;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace AAC20.Classes.Commands
{
    /// <summary>
    /// Консольная команда
    /// </summary>
    public partial class ConsoleCommand : ICommandAAC
    {
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
        private event ICommandAAC.ExecuteCom Execute;

        /// <summary>
        /// Инициализировать объект консольной команды с параметрами
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Parameters">Параметры команды</param>
        /// <param name="Explanation">Описание команды</param>
        /// <param name="Execute">Действие выполнения</param>
        public ConsoleCommand(string Name, Parameter[] Parameters, string Explanation, ICommandAAC.ExecuteCom Execute)
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
        public ConsoleCommand(string Name, string Explanation, ICommandAAC.ExecuteCom Execute)
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
        public static CommandStateResult ReadAndExecuteCommand(Buffer BufferCommand, ConsoleCommand[] ConsoleCommands, string TextCommand)
        {
            string RegistriernCommand = TextCommand;
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
                Parameters = [.. ICommandAAC.RegexParameterCommand().Matches(TextCommand).Select(i => i.Value[2..])];
                TextCommand = ICommandAAC.RegexSortCommand().Match(TextCommand).Value.ToString().Replace("*", string.Empty).Replace(" ", string.Empty);
            }
            else // command
            {
                TextCommand = TextCommand.Replace(" ", "_").Replace("*", string.Empty).ToLower();
            }
            ConsoleCommand? SearchCommand = ConsoleCommands.SingleOrDefault(i => i.Name.Equals(TextCommand));
            if (SearchCommand == null)
            {
                Paragraph Massage = new();
                Massage.Inlines.Add(new Bold(new Run(">>> ")));
                Massage.Inlines.Add(new Run("Invalid command "));
                Massage.Inlines.Add(new Italic(new Run($"\"{TextCommand}\"") { Background = new SolidColorBrush(Colors.IndianRed) }));
                return CommandStateResult.Failed(Massage, $"Команда \"{TextCommand}\" не найдена");
            }
            else
            {
                BufferCommand.Add(SearchCommand, SearchCommand.Name, Parameters ?? [], RegistriernCommand);
                return AbsolutlyRequiredParameters(SearchCommand, Parameters) ?
                    SearchCommand.ExecuteCommand(Parameters) : CommandStateResult.FaledParameteres(SearchCommand.Name);
            }
        }

        /// <summary>
        /// Узнать написаны ли обязательные параметры команды
        /// </summary>
        /// <param name="WritingParameters">Написанные параметры</param>
        /// <returns>Совпадает правилу или нет</returns>
        private static bool AbsolutlyRequiredParameters(ConsoleCommand Command, string[]? WritingParameters) =>
            (WritingParameters?.Length ?? 0) >= (Command.Parameters?.Count((i) => i.Absolutly == true) ?? 0);

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        public CommandStateResult ExecuteCommand(string[]? parameters) => Execute.Invoke(parameters ?? []).Result;
    }
}
