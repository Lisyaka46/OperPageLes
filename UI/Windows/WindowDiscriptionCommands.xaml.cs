using IEL;
using IEL.CORE.Classes;
using IEL.CORE.Enums;
using Interpreter.Commands;
using OperPage_les.CORE.Flaging;
using OperPage_les.UI.Pages.Description;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Interpreter.Interfaces;

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
            IELButtonAlias.IsEnabled = App.CurrentApp.DataAliases.Count > 0;
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
            IELButtonConsole.OnActivateMouseLeft += (Key) =>
            {
                IELButtonConsole.IELSettingObject.BackgroundSetting.UsedState = true;
                if (App.CurrentApp.DataAliases.Count == 0 && IELButtonAlias.IsEnabled)
                    IELButtonAlias.IELSettingObject.BackgroundSetting.UsedState = false;
                IELControllerDescription.NextPage(DescriptionConsole, false);
                GridMainElements.Opacity = 0d;
                GridMainElements.Children.Clear();
                GridMainElements.RowDefinitions.Clear();
                foreach (ConsoleCommand Element in App.DataConsoleCommand)
                {
                    IELButtonText Button = GenerateCommandButton();
                    Button.Text = Element.Name;
                    Button.OnActivateMouseLeft += (Key) =>
                    {
                        DescriptionConsole.UpdateInformation(Element);
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
            IELButtonAlias.OnActivateMouseLeft += (Key) =>
            {
                if (App.CurrentApp.DataAliases.Count == 0 && IELButtonAlias.IsEnabled) return;
                IELButtonAlias.IELSettingObject.BackgroundSetting.UsedState = true;
                IELButtonConsole.IELSettingObject.BackgroundSetting.UsedState = false;
                IELControllerDescription.NextPage(DescriptionAlias, true);
                GridMainElements.Opacity = 0d;
                GridMainElements.Children.Clear();
                GridMainElements.RowDefinitions.Clear();
                foreach (AliasCommand<ICommandOPER> Element in App.CurrentApp.DataAliases)
                {
                    IELButtonText Button = GenerateCommandButton();
                    Button.Text = Element.Name;
                    Button.OnActivateMouseLeft += (Key) =>
                    {
                        DescriptionAlias.UpdateInformation(Element);
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
            IELButtonSearchCommand.OnActivateMouseLeft += (Key) =>
            {
                if (GridMainElements.Children.Count == 0) return;
                int[] Indexes = [..Enumerable.Range(0, App.DataConsoleCommand.Count).Where(
                            i => App.DataConsoleCommand[i].Name.Contains(IELInputSearchCommand.Text))];
                if (IndexSearch.Length == 0)
                {
                    IndexSearch = Indexes;
                }
                else
                {
                    IEnumerable<int> EnumIndex = IndexSearch.AsEnumerable();
                    foreach (int Index in Indexes)
                    {
                        if (EnumIndex.Contains(Index)) continue;
                        EnumIndex = EnumIndex.Append(Index);
                    }
                    IndexSearch = [.. EnumIndex];
                }
                IELButtonText Button;
                foreach (int Index in IndexSearch)
                {
                    Button = (IELButtonText)GridMainElements.Children[Index];
                    if (Button.IELSettingObject.BackgroundSetting.UsedState) continue;
                    Button.IELSettingObject.BackgroundSetting.UsedState = true;
                }
            };
            IELButtonSearchCommand.OnActivateMouseRight += (Key) =>
            {
                Keyboard.ClearFocus();
                if (IndexSearch.Length == 0) return;
                IELButtonText Button;
                foreach (int Index in IndexSearch)
                {
                    Button = (IELButtonText)GridMainElements.Children[Index];
                    if (!Button.IELSettingObject.BackgroundSetting.UsedState) continue;
                    Button.IELSettingObject.BackgroundSetting.UsedState = false;
                }
                IndexSearch = [];
            };
            #endregion
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
                FontSize = 13d,
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
                Source = (Style)System.Windows.Application.Current.Resources["Brenzo Slab Personal Use"]
            };
            BindingOperations.SetBinding(Element, IELButtonText.StyleProperty, binding);
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
