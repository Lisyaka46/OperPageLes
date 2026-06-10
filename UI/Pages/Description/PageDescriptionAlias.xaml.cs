using OperPageLes.CORE;
using OPLAPI.OIEL.UserElementsControl.Interfaces;
using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using InterpreterCommand.Commands;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace OperPageLes.UI.Pages.Description
{
    /// <summary>
    /// Логика взаимодействия для PageDescriptionAlias.xaml
    /// </summary>
    public partial class PageDescriptionAlias : Page, IDiscriptionPage<AliasCommand<CommandOPER<IOPERCommandViewer>, IOPERCommandViewer>>
    {
        /// <summary>
        /// Активно ли выделение команды
        /// </summary>
        internal bool SelectCommand { get; private set; } = false;

        /// <summary>
        /// Событие изменения выделения команды
        /// </summary>
        internal event IDiscriptionPageEventsHandler.ChangeStateHandler<bool>? ChangeStateSelectCommand;

        public PageDescriptionAlias()
        {
            InitializeComponent();
            ClearInformationOnCommand();
        }

        /// <summary>
        /// Обновить описание
        /// </summary>
        /// <param name="command"></param>
        public void UpdateInformation(AliasCommand<CommandOPER<IOPERCommandViewer>, IOPERCommandViewer> command)
        {
            SelectCommand = true;
            ChangeStateSelectCommand?.Invoke(SelectCommand);
            GridDiscription.Visibility = System.Windows.Visibility.Visible;
            string NameCommand = RegexNameCommand().Match(command.NameCommand).Value;
            TextBlockNameAlias.Text = command.Name;
            TextBlockAlias.Text = "alias* " + command.Name;

            Parameter[] Parameters = command.Parameters ?? [];
            if (Parameters.Length > 0)
            {
                string TextRegistration = "*";
                for (int i = 0; i < Parameters.Length; i++)
                {
                    TextRegistration += $"{Parameters[i].Name}" +
                        $"{(Parameters[i].Absolutly ? string.Empty : '?')}" +
                        $"{(i < Parameters.Length - 1 ? ", " : string.Empty)}";
                }
                TextBlockAliasCommand.Text = $"({command.NameCommand + TextRegistration})";
            }
            else TextBlockAliasCommand.Text = $"({command.NameCommand})"; ;

            ICommandOPER<IOPERCommandViewer>? SourceCommandAlias = App.CurrentApp.Interpreter.GetCommandFindName(NameCommand, CommandLevel.LowLevel);
            TextBlockDescriptionAliasCommand.Text = SourceCommandAlias != null ? SourceCommandAlias.Description : "Такой команды не существует.";
            TextBlockDescriptionAlias.Text = command.Description;
        }

        /// <summary>
        /// Узнать синтаксис команды
        /// </summary>
        public string? GetCommandText() => SelectCommand ? TextBlockNameAlias.Text + "*" : null;

        /// <summary>
        /// Убрать информацию о команде
        /// </summary>
        public void ClearInformationOnCommand()
        {
            SelectCommand = false;
            ChangeStateSelectCommand?.Invoke(SelectCommand);
            TextBlockNameAlias.Text = "Алиас не выбран";
            GridDiscription.Visibility = System.Windows.Visibility.Hidden;
        }

        #region Regex
        /// <summary>
        /// Регулярное выражение имени консольной команды
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        [GeneratedRegex(@"\b[^*]+")]
        private static partial Regex RegexNameCommand();
        #endregion
    }
}
