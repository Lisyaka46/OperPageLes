using AAC20.Windows.Pages.ActionPanel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace AAC20.Classes.Commands
{
    /// <summary>
    /// Консольная команда
    /// </summary>
    [MarkupExtensionReturnTypeAttribute(typeof(ConsoleCommand))]
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
        [AllowNull()]
        public Parameter[]? Parameters { get; private set; }

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
        public static CommandStateResult ReadAndExecuteCommand(Buffer BufferCommand, PageBufferActionPanel? PageBuffer,
            [NotNull()] ConsoleCommand[] ConsoleCommands, string TextCommand)
        {
            string[]? Parameters = null;
            string Name;
            if (TextCommand.Contains('*')) // command * param1, param2, param3 ...
            {
                Name = ClearReplySymbol(ICommandAAC.RegexNameCommand().Match(TextCommand).Value, ' ');
                Parameters = [..
                    ICommandAAC.RegexSortParamCommand().Matches(
                        ICommandAAC.RegexParameterCommand().Match(TextCommand).Value[1..])
                    .Select((i) => i.Value) ];
                for (int i = 0; i < Parameters.Length; i++)
                {
                    switch (Parameters[i][0])
                    {
                        case ' ':
                        case '~':
                            Parameters[i] = Parameters[i][1..];
                            break;
                    }
                    Parameters[i] = Parameters[i].Replace("%,", ",");
                    Parameters[i] = Parameters[i].Replace("%%", "%");
                }
            }
            else // command
            {
                Name = ClearReplySymbol(TextCommand, ' ');
            }
            Name = Name.Replace(" ", "_").ToLower();
            ConsoleCommand? SearchCommand = ConsoleCommands.SingleOrDefault(i => i.Name.Equals(Name));
            if (PageBuffer != null)
            {
                GUI.IELButtonCommand Button = BufferCommand.Add(SearchCommand, ref PageBuffer.GridBuffer, Name, Parameters ?? [], TextCommand);
                PageBuffer.IELButtonClearBuffer.IsEnabled = true;
                Button.OnActivateRightButtonMouse += () =>
                {
                    App.BufferCommand.Delete(PageBuffer.GridBuffer, Button);
                    PageBuffer.TextBlockCounterBuffer.Text = $"{App.BufferCommand.Count}/{App.BufferCommand.Length}";
                    if (App.BufferCommand.Count == 0) PageBuffer.IELButtonClearBuffer.IsEnabled = false;
                };
                PageBuffer.GridBuffer.Children.Add(Button);
                PageBuffer.TextBlockCounterBuffer.Text = $"{App.BufferCommand.Count}/{App.BufferCommand.Length}";
            }
            if (SearchCommand == null) return CommandStateResult.FaledCommand(Name);
            else
            {
                return SearchCommand.AbsolutlyRequiredParameters(Parameters) ?
                    SearchCommand.ExecuteCommand(Parameters) : CommandStateResult.FaledParameteres(SearchCommand.Name);
            }
        }

        //
        private static string ClearReplySymbol(string Text, char Symbol)
        {
            Text = new([.. Text.Reverse()]);
            for (int i = 0, count = 0; i < Text.Length; i++)
            {
                if (Text[i] == Symbol) count = i + 1;
                else return new([.. Text.Remove(0, count).Reverse()]);
            }
            return new([.. Text.Reverse()]);
        }

        /// <summary>
        /// Узнать написаны ли обязательные параметры команды
        /// </summary>
        /// <param name="WritingParameters">Написанные параметры</param>
        /// <returns>Совпадает правилу или нет</returns>
        public bool AbsolutlyRequiredParameters(string[]? WritingParameters) =>
            (WritingParameters?.Length ?? 0) >= (Parameters?.Count((i) => i.Absolutly == true) ?? 0);

        /// <summary>
        /// Создать выполнение команды
        /// </summary>
        public CommandStateResult ExecuteCommand(string[]? parameters) => Execute.Invoke(parameters ?? []).Result;
    }
}
