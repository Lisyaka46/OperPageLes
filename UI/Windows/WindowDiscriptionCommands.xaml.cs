using IEL.GUI;
using IEL.CORE.Classes;
using IEL.CORE.Enums;
using Interpreter.Commands;
using Interpreter.Interfaces;
using OperPage_les.CORE.Flaging;
using OperPage_les.UI.Pages.Description;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace OperPage_les.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowDiscriptionCommands.xaml
    /// </summary>
    public partial class WindowDiscriptionCommands : Window
    {
        /// <summary>
        /// Перечисление состояний описания
        /// </summary>
        private enum ActivateStateDiscription
        {
            /// <summary>
            /// Не активное состояние
            /// </summary>
            NotActivated = -1,

            /// <summary>
            /// Консольные команды
            /// </summary>
            ConsoleCommand = 0,
        }

        /// <summary>
        /// Страница описания поведения консольной команды
        /// </summary>
        private readonly PageDescriptionConsole DescriptionConsole;

        /// <summary>
        /// Страница описания алиасов
        /// </summary>
        private readonly PageDescriptionAlias DescriptionAlias;

        /// <summary>
        /// Флаги данной формы
        /// </summary>
        private readonly struct Flags
        {
            /// <summary>
            /// Флаг состояния активности контекстной панели описания параметров
            /// </summary>
            public static readonly Flag ContextMenuParameter = new(false);
        };

        /// <summary>
        /// Константа размера Height для элементов описания
        /// </summary>
        private const int HeightElement = 55;

        /// <summary>
        /// Настройка отображения элементов списка описания
        /// </summary>
        private readonly static BrushSettingQ BorderForegroundSetting = new(new byte[,]
                        {
                        { 255, 20, 64, 106 },
                        { 255, 131, 184, 202 },
                        { 255, 131, 184, 202 },
                        { 255, 188, 208, 218 },
                        });

        /// <summary>
        /// Массив индексов поиска элементов описания команд
        /// </summary>
        private int[] IndexSearch = [];

        public WindowDiscriptionCommands()
        {
            App.Log("Создание окна описания элементов");
            InitializeComponent();
            IELButtonAlias.IsEnabled = App.CurrentApp.Interpreter.AliasesCount > 0;
            IELButtonSearchCommand.Imaging = App.LoadImage(Properties.Resources.Search);

            IELMessageInfo.Opacity = 0d;
            DescriptionConsole = new()
            {
                Opacity = 0d
            };
            DescriptionConsole.IELButtonInfoParameter.MouseEnter += (sender, e) =>
            {
                IELMessageInfo.UsingBorderInformation(DescriptionConsole.IELButtonInfoParameter,
                    "Символ \"~\" является пропускным символом, альтернативой \" \", для записи пропущенного символа в параметры нужно ввести \"~~\"\n\n" +
                    "Символ \"%\" является специальным символом (Одинарный символ пропускается):\n" +
                    "- Для записи \"%\" в параметры нужно ввести \"%%\"\n" +
                    "- Для записи \",\" в параметры нужно ввести \"%,\"",
                    OrientationBorderPosition.LeftDown);
            };
            DescriptionConsole.IELButtonInfoParameter.MouseLeave += (sender, e) => IELMessageInfo.CloseBorderInformation();
            DescriptionAlias = new()
            {
                Opacity = 0d
            };

            #region IELButtonConsole
            IELButtonConsole.MouseEnter += (sender, e) => AnimateButtonBookmark(IELButtonConsole, 4);
            IELButtonConsole.MouseLeave += (sender, e) => AnimateButtonBookmark(IELButtonConsole, 0);
            IELButtonConsole.OnActivateMouseLeft += (sender, e, Key) =>
            {
                IELButtonConsole.IELSettingObject.BackgroundSetting.SetUsedState(true);
                if (App.CurrentApp.Interpreter.AliasesCount == 0 && IELButtonAlias.IsEnabled)
                    IELButtonAlias.IELSettingObject.BackgroundSetting.SetUsedState(false);
                IELControllerDescription.NextPage(DescriptionConsole, false);
                GridMainElements.Opacity = 0d;
                GridMainElements.Children.Clear();
                GridMainElements.RowDefinitions.Clear();
                foreach (KeyValuePair<string, ICommandOPER> Element in App.CurrentApp.Interpreter.Commands)
                {
                    IELButtonText Button = GenerateCommandButton();
                    Button.Text = Element.Value.Name;
                    Button.OnActivateMouseLeft += (sender, e, Key) =>
                    {
                        DescriptionConsole.UpdateInformation(Element.Value);
                        App.AnimateDoubleEffect(DescriptionConsole, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                    };
                    GridMainElements.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(0d, GridUnitType.Auto)
                    });
                    Grid.SetRow(Button, GridMainElements.RowDefinitions.Count - 1);
                    GridMainElements.Children.Add(Button);
                }
                App.AnimateDoubleEffect(GridMainElements, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
            };
            #endregion
            #region IELButtonAlias
            IELButtonAlias.MouseEnter += (sender, e) => AnimateButtonBookmark(IELButtonAlias, 4);
            IELButtonAlias.MouseLeave += (sender, e) => AnimateButtonBookmark(IELButtonAlias, 0);
            IELButtonAlias.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (App.CurrentApp.Interpreter.AliasesCount == 0 && IELButtonAlias.IsEnabled) return;
                IELButtonAlias.IELSettingObject.BackgroundSetting.SetUsedState(true);
                IELButtonConsole.IELSettingObject.BackgroundSetting.SetUsedState(false);
                IELControllerDescription.NextPage(DescriptionAlias, true);
                GridMainElements.Opacity = 0d;
                GridMainElements.Children.Clear();
                GridMainElements.RowDefinitions.Clear();
                foreach (KeyValuePair<string, AliasCommand<ICommandOPER>> Element in App.CurrentApp.Interpreter.Aliases)
                {
                    IELButtonText Button = GenerateCommandButton();
                    Button.Text = Element.Value.Name;
                    Button.OnActivateMouseLeft += (sender, e, Key) =>
                    {
                        DescriptionAlias.UpdateInformation(Element.Value);
                        App.AnimateDoubleEffect(DescriptionAlias, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                    };
                    GridMainElements.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(0d, GridUnitType.Auto)
                    });
                    Grid.SetRow(Button, GridMainElements.RowDefinitions.Count - 1);
                    GridMainElements.Children.Add(Button);
                }
                App.AnimateDoubleEffect(GridMainElements, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
            };
            #endregion
            #region IELButtonSearchCommand
            IELButtonSearchCommand.MouseEnter += (sender, e) => AnimateButtonBookmark(IELButtonSearchCommand, 4);
            IELButtonSearchCommand.MouseLeave += (sender, e) => AnimateButtonBookmark(IELButtonSearchCommand, 0);
            IELButtonSearchCommand.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (GridMainElements.Children.Count == 0) return;
                IELButtonText Button;
                if (IndexSearch.Length > 0)
                {
                    foreach (int Index in IndexSearch)
                    {
                        Button = (IELButtonText)GridMainElements.Children[Index];
                        if (!Button.IELSettingObject.BackgroundSetting.GetUsedState()) continue;
                        Button.IELSettingObject.BackgroundSetting.SetUsedState(false);
                    }
                    IndexSearch = [];
                }
                ICommandOPER[] SearchCommands = [.. App.CurrentApp.Interpreter.CommandWhere((i) => i.Name.Contains(IELInputSearchCommand.Text))];
                if (SearchCommands.Length > 0)
                {
                    List<int> ArraySearch = [];
                    for (int i = 0; i < GridMainElements.Children.Count; i++)
                    {
                        Button = (IELButtonText)GridMainElements.Children[i];
                        for (int j = 0; j < SearchCommands.Length; j++)
                        {
                            if (Button.Text.Contains(SearchCommands[j].Name))
                            {
                                ArraySearch.Add(i);
                                break;
                            }
                        }
                    }
                    IndexSearch = [.. ArraySearch];
                }
                else return;
                foreach (int Index in IndexSearch)
                {
                    Button = (IELButtonText)GridMainElements.Children[Index];
                    if (Button.IELSettingObject.BackgroundSetting.GetUsedState()) continue;
                    Button.IELSettingObject.BackgroundSetting.SetUsedState(true);
                }
            };
            IELButtonSearchCommand.OnActivateMouseRight += (sender, e, Key) =>
            {
                Keyboard.ClearFocus();
                if (IndexSearch.Length == 0) return;
                IELButtonText Button;
                foreach (int Index in IndexSearch)
                {
                    Button = (IELButtonText)GridMainElements.Children[Index];
                    if (!Button.IELSettingObject.BackgroundSetting.GetUsedState()) continue;
                    Button.IELSettingObject.BackgroundSetting.SetUsedState(false);
                }
                IndexSearch = [];
            };
            #endregion
            IELInputSearchCommand.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        Keyboard.ClearFocus();
                        break;
                }
            };
            Closing += (sender, e) =>
            {
                App.MainWindowApplication?.Activate();
            };
            App.Log("Готово!");
        }

        /// <summary>
        /// Сгенерировать кнопку команнды описания
        /// </summary>
        /// <returns>Кнопка команды</returns>
        private static IELButtonText GenerateCommandButton()
        {
            IELButtonText Element = new()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Height = HeightElement,
                FontSize = 16d,
                BorderThicknessBlock = new(2),
                IELSettingObject = new()
                {
                    VisibleMouseImaging = false,
                    BackgroundSetting = new(new byte[,]
                        {
                        { 255, 161, 204, 232 },
                        { 255, 92, 131, 157 },
                        { 255, 122, 172, 205 },
                        { 255, 166, 181, 190 },
                        }),
                    BorderBrushSetting = BorderForegroundSetting,
                    ForegroundSetting = BorderForegroundSetting
                },
                Margin = new(3),
            };
            System.Windows.Data.Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["Deledda Open Regular"]
            };
            BindingOperations.SetBinding(Element, IELButtonText.FontFamilyProperty, binding);
            return Element;
        }

        /// <summary>
        /// Анимировать кнопку как закладку
        /// </summary>
        /// <param name="Button">Объект кнопки</param>
        /// <param name="Offset">Оффсет вытягивания</param>
        private static void AnimateButtonBookmark(FrameworkElement Button, int Offset)
        {
            App.AnimateThicknessEffect(Button, MarginProperty, new(Button.Margin.Left, 0, Button.Margin.Right, 7 - Offset));
        }
    }
}
