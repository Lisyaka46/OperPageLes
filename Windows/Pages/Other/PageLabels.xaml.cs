using AAC20.Classes;
using AAC20.Classes.Labels;
using AAC20.GUI;
using AAC20.Windows.Pages.ActionPanel;
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

namespace AAC20.Windows.Pages.Other
{
    /// <summary>
    /// Логика взаимодействия для PageLabels.xaml
    /// </summary>
    public partial class PageLabels : Page
    {
        /// <summary>
        /// Динамический массив ярлыков
        /// </summary>
        private readonly List<IELLabelCommand> ObjectsLabel;

        /// <summary>
        /// Страница ярлыка в панели действий
        /// </summary>
        private static readonly PageLabelActionPanel PageLabelActPanel = new();

        /// <summary>
        /// Настройка поведения панели действий для объектов ярлыка
        /// </summary>
        private SettingsPanelActionFrameworkElement SettingsPanelActionElement;

        public PageLabels()
        {
            InitializeComponent();
            SettingsPanelActionElement = new(GridMain, PageLabelActPanel, new(96, 130));
            ObjectsLabel = [];
            GridMain.ColumnDefinitions.Add(new() { Width = new GridLength(20d) });
        }

        internal void AddLabel(LabelAction label)
        {
            IELLabelCommand Label = new(label, ObjectsLabel.Count)
            {
                Width = 80,
                Height = 80,
                Margin = new(40, (80 + 10) * ObjectsLabel.Count, 0, 0),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                ContextMenu = null,
                IntervalHover = 800d,
            };
            /*Label.OnActivateMouseLeft += () =>
            {
                SummarizeCommandStateResult(ConsoleCommand.ReadAndExecuteCommand(null, [.. App.DataConsoleCommand], label.Command));
            };*/
            Label.OnActivateMouseRight += () =>
            {
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(SettingsPanelActionElement);
                /*if (!RefPageActionPanel?.PageName.Equals(nameof(PageLabelActionPanel)) ?? true)
                    App.MainWindowApplication.NextPageInActtionPanel(Pages.PageLabelActPanel, RefPageActionPanel?.KeyboardMode ?? false);*/
            };
            Label.MouseHover += (sender, e) =>
            {
                if (sender == null) return;
                IELLabelCommand Element = (IELLabelCommand)sender;
                string Text = Element.Label.Description ?? string.Empty;
                if (Text.Length > 0)
                    App.MainWindowApplication.IELMessageMain.UsingBorderInformation(Element, Text,
                        IELBlockMessage.OrientationBorderInfo.LeftDown);
            };
            Label.MouseLeave += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            Label.MouseDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            GridMain.Children.Add(Label);
            ObjectsLabel.Add(Label);
        }
    }
}
