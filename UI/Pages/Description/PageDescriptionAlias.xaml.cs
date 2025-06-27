using Interpreter.Commands;
using Interpreter.Interfaces;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace OperPage_les.UI.Pages.Description
{
    /// <summary>
    /// Логика взаимодействия для PageDescriptionAlias.xaml
    /// </summary>
    public partial class PageDescriptionAlias : Page
    {
        public PageDescriptionAlias()
        {
            InitializeComponent();
            IELButtonCopyCommandAlias.OnActivateMouseLeft += (sender, e, Key) =>
            {
                System.Windows.Clipboard.SetText(TextBlockAliasCommand.Text);
            };
        }

        /// <summary>
        /// Обновить описание
        /// </summary>
        /// <param name="command"></param>
        public void UpdateInformation(AliasCommand<ICommandOPER> command)
        {
            string NameCommand = RegexNameCommand().Match(command.NameCommand).Value;
            TextBlockNameAlias.Text = command.Name;
            TextBlockAlias.Text = "alias* " + command.NameCommand;
            TextBlockAliasCommand.Text = command.NameCommand;
            ICommandOPER? SourceCommandAlias = App.CurrentApp.Interpreter.GetCommandFindName(NameCommand);
            TextBlockDescriptionAliasCommand.Text = SourceCommandAlias != null ? SourceCommandAlias.Description : "Такой команды не существует.";
            TextBlockDescriptionAlias.Text = command.Description;
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
