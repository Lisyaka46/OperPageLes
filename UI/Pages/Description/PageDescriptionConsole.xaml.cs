using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using OperPageLes.CORE;
using System.Windows.Controls;
using System.Windows.Media;

namespace OperPageLes.UI.Pages.Description
{
    /// <summary>
    /// Логика взаимодействия для PageDescriptionConsole.xaml
    /// </summary>
    public partial class PageDescriptionConsole : Page, IDiscriptionPage<ICommandOPER>
    {
        /// <summary>
        /// Активно ли выделение команды
        /// </summary>
        internal bool SelectCommand { get; private set; } = false;

        /// <summary>
        /// Событие изменения выделения команды
        /// </summary>
        internal event IDiscriptionPageEventsHandler.ChangeStateHandler<bool>? ChangeStateSelectCommand;

        public PageDescriptionConsole()
        {
            InitializeComponent();
            ClearInformationOnCommand();
        }
         
        /// <summary>
        /// Обновить описание
        /// </summary>
        /// <param name="command">Описываемый элемент</param>
        public void UpdateInformation(ICommandOPER command)
        {
            SelectCommand = true;
            ChangeStateSelectCommand?.Invoke(SelectCommand);
            GridDiscription.Visibility = System.Windows.Visibility.Visible;
            Parameter[] Parameters = command.Parameters ?? [];
            int CountParameters = Parameters.Length;
            string TextRegistration = string.Empty;
            for (int i = 0; i < Parameters.Length; i++)
            {
                TextRegistration += $"{Parameters[i].Name}" +
                    $"{(Parameters[i].Absolutly ? string.Empty : '?')}" +
                    $"{(i < Parameters.Length - 1 ? ", " : string.Empty)}";
            }
            TextBlockNameCommand.Text = $"Консольная команда: \"{command.Name}\"";
            if (command.GetType() == typeof(ConsoleCommand)) TextBlockMainDescriptionCommand.Text = ((ConsoleCommand)command).Description;
            TextBlockDescriptionCountParameter.Text = CountParameters == 0 ?
            $"Команда \"{command.Name}\" не использует параметров" : $"Команда \"{command.Name}\" включает в себя {CountParameters} и больше параметров";
            TextBlockTextCommand.Text = command.Name.Trim() + (CountParameters == 0 ? string.Empty : "* " + TextRegistration);
        }

        /// <summary>
        /// Узнать синтаксис команды
        /// </summary>
        public string? GetCommandText() => SelectCommand ? TextBlockTextCommand.Text : null;

        /// <summary>
        /// Убрать информацию о команде
        /// </summary>
        public void ClearInformationOnCommand()
        {
            SelectCommand = false;
            ChangeStateSelectCommand?.Invoke(SelectCommand);
            TextBlockNameCommand.Text = "Команда не выбрана";
            GridDiscription.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
