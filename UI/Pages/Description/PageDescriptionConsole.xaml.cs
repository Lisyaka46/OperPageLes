using Interpreter.Classes;
using Interpreter.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OperPage_les.UI.Pages.Description
{
    /// <summary>
    /// Логика взаимодействия для PageDescriptionConsole.xaml
    /// </summary>
    public partial class PageDescriptionConsole : Page
    {
        public PageDescriptionConsole()
        {
            InitializeComponent();
            TextBlockTextCommand.Foreground = new SolidColorBrush(Colors.Black);
            IELButtonCloneTextCommand.OnActivateMouseLeft += (Key) =>
            {
                System.Windows.Clipboard.SetText(TextBlockTextCommand.Text);
                App.AnimateColorEffect(TextBlockTextCommand.Foreground, SolidColorBrush.ColorProperty,
                    System.Windows.Media.Colors.White, ((SolidColorBrush)TextBlockTextCommand.Foreground).Color, TimeSpan.FromMilliseconds(300d));
            };
        }

        /// <summary>
        /// Обновить описание
        /// </summary>
        /// <param name="command"></param>
        public void UpdateInformation(ConsoleCommand command)
        {
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
            TextBlockMainDescriptionCommand.Text = command.Description;
            TextBlockDescriptionCountParameter.Text = CountParameters == 0 ?
            $"Команда \"{command.Name}\" не использует параметров" : $"Команда \"{command.Name}\" включает в себя {CountParameters} и больше параметров";
            TextBlockTextCommand.Text = command.Name.Trim() + (CountParameters == 0 ? string.Empty : "* " + TextRegistration);
        }
    }
}
