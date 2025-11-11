using IEL.CORE.Classes;
using IEL.CORE.Enums;
using IEL.GUI;
using Interpreter.Interfaces;
using ApplicationOperPageLes.CORE;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.Pages.Description;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.Windows
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

            /// <summary>
            /// Алиасы
            /// </summary>
            AliasCommand = 1,
        }

        /// <summary>
        /// Состояние активной страницы описания
        /// </summary>
        private ActivateStateDiscription StateDiscription = ActivateStateDiscription.NotActivated;

        /// <summary>
        /// Воспроизводилась ли стартовая анимация
        /// </summary>
        private bool StartAnimation;

        /// <summary>
        /// Активен ли поиск элементов
        /// </summary>
        private bool SearchActivate;

        /// <summary>
        /// Страница описания поведения консольной команды
        /// </summary>
        private readonly PageDescriptionConsole DescriptionConsole;

        /// <summary>
        /// Страница описания алиасов
        /// </summary>
        private readonly PageDescriptionAlias DescriptionAlias;

        /// <summary>
        /// Константа размера Height для элементов описания
        /// </summary>
        private const int HeightElement = 55;

        /// <summary>
        /// Настройка отображения элементов списка описания
        /// </summary>
        private readonly static QData BorderForegroundSetting = new(new byte[,]
                        {
                        { 255, 20, 64, 106 },
                        { 255, 131, 184, 202 },
                        { 255, 131, 184, 202 },
                        { 255, 188, 208, 218 },
                        });

        public WindowDiscriptionCommands()
        {
            App.CurrentApp.LogWriteLine("Создание окна описания элементов");
            StartAnimation = false;
            SearchActivate = false;
            InitializeComponent();
            IELButtonAlias.IsEnabled = App.CurrentApp.Interpreter.AliasesCount > 0;
            IELButtonSearchCommand.Imaging = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Search));
            //IELButtonCloneTextCommand.Foreground = new SolidColorBrush(Colors.Black);

            IELMessageInfo.Opacity = 0d;
            DescriptionConsole = new();
            DescriptionConsole.ChangeStateSelectCommand += SetStateManipulateInformation;
            DescriptionAlias = new();
            DescriptionAlias.ChangeStateSelectCommand += SetStateManipulateInformation;
            SetStateManipulateInformation(false);

            IELButtonCloneTextCommand.OnActivateMouseLeft += (sender, e, Key) =>
            {
                string? Text;
                if (StateDiscription == ActivateStateDiscription.ConsoleCommand) Text = DescriptionConsole.GetCommandText();
                else if (StateDiscription == ActivateStateDiscription.AliasCommand) Text = DescriptionAlias.GetCommandText();
                else return;
                System.Windows.Clipboard.SetText(Text);
            };
            IELButtonBack.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (StateDiscription == ActivateStateDiscription.ConsoleCommand) DescriptionConsole.ClearInformationOnCommand();
                else if (StateDiscription == ActivateStateDiscription.AliasCommand) DescriptionAlias.ClearInformationOnCommand();
                else return;
            };
            #region IELButtonInfoParameter
            IELButtonInfoParameter.MouseEnter += (sender, e) =>
            {
                IELMessageInfo.UsingBorderInformation(IELButtonInfoParameter,
                    "Символ \"~\" является пропускным символом, альтернативой \" \", для записи пропущенного символа в параметры нужно ввести \"~~\"\n\n" +
                    "Символ \"%\" является специальным символом (Одинарный символ пропускается):\n" +
                    "- Для записи \"%\" в параметры нужно ввести \"%%\"\n" +
                    "- Для записи \",\" в параметры нужно ввести \"%,\"",
                    OrientationBorderPosition.LeftDown);
            };
            IELButtonInfoParameter.MouseLeave += (sender, e) => IELMessageInfo.CloseBorderInformation();
            #endregion

            #region IELButtonConsole
            IELButtonConsole.OnActivateMouseLeft += (sender, e, Key) =>
            {
                StateDiscription = ActivateStateDiscription.ConsoleCommand;
                SetStateManipulateInformation(DescriptionConsole.SelectCommand);
                AnimateEnterPageButton(IELButtonConsole);
                IELControllerDescription.NextPage(DescriptionConsole, false);

                Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
                {
                    ClearVisualGrid();
                    Grid GridElements = await App.MainWindow.ExecuteVisualizateLoadingProcess("Загрузка каталога консольных команд",
                        InitializeVisualElementDiscription([.. App.CurrentApp.Interpreter.Commands.Values], DescriptionConsole));
                    ScrollViewerElements.Content = GridElements;
                    App.DoubleAnimationType.AnimateEffect(GridElements, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                });
            };
            #endregion
            #region IELButtonAlias
            IELButtonAlias.OnActivateMouseLeft += (sender, e, Key) =>
            {
                StateDiscription = ActivateStateDiscription.AliasCommand;
                SetStateManipulateInformation(DescriptionAlias.SelectCommand);
                AnimateEnterPageButton(IELButtonAlias);
                IELControllerDescription.NextPage(DescriptionAlias, true);

                Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
                {
                    ClearVisualGrid();
                    Grid GridElements = await App.MainWindow.ExecuteVisualizateLoadingProcess("Загрузка каталога алиасов",
                        InitializeVisualElementDiscription([.. App.CurrentApp.Interpreter.Aliases.Values], DescriptionAlias));
                    ScrollViewerElements.Content = GridElements;
                    App.DoubleAnimationType.AnimateEffect(GridElements, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                });

                //Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
                //{
                //    ClearVisualGrid();
                //    Grid GridElements = await InitializeVisualElementDiscription([.. App.CurrentApp.Interpreter.Aliases.Values], DescriptionAlias);
                //    ScrollViewerElements.Content = GridElements;
                //    App.AnimateDoubleEffect(GridElements, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                //});
            };
            #endregion
            #region IELButtonSearchCommand
            IELButtonSearchCommand.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (ScrollViewerElements.Content == null || IELInputSearchCommand.Text.Length == 0) return;
                if (((Grid)ScrollViewerElements.Content).Children.Count == 0) return;
                SearchActivate = true;
                IELButtonSearchCommand.QBackground.SetUsedState(true);
                IELButtonText Button;
                //if (IndexSearch.Length > 0)
                //{
                //    foreach (int Index in IndexSearch)
                //    {
                //        Button = (IELButtonText)GridMainElements.Children[Index];
                //        if (!Button.IELSettingObject.BackgroundSetting.GetUsedState()) continue;
                //        Button.IELSettingObject.BackgroundSetting.SetUsedState(false);
                //    }
                //    IndexSearch = [];
                //}
                ICommandOPER[] SearchCommands = [.. App.CurrentApp.Interpreter.CommandWhere((i) => i.Name.Contains(IELInputSearchCommand.Text))];
                if (SearchCommands.Length > 0)
                {
                    Grid MainElementsContainer = (Grid)ScrollViewerElements.Content;
                    int IndexSearching = 0;
                    for (int i = 0; i < MainElementsContainer.Children.Count; i++)
                    {
                        Button = (IELButtonText)MainElementsContainer.Children[i];
                        bool SearchComplete = SearchCommands.Any((k) => k.Name.Equals(Button.Text));

                        Button.IsEnabled = SearchComplete;
                        Canvas.SetZIndex(Button, SearchComplete ? 0 : -1);
                        App.ThicknessAnimationType.AnimateEffect(Button, MarginProperty,
                            GetElementMarginFromPositionIndex(SearchComplete ? IndexSearching++ : -1), TimeSpan.FromMilliseconds(300d));
                        App.DoubleAnimationType.AnimateEffect(Button, OpacityProperty, SearchComplete ? 1d : 0d, TimeSpan.FromMilliseconds(300d));
                    }
                }
                else return;
            };
            IELButtonSearchCommand.OnActivateMouseRight += (sender, e, Key) =>
            {
                if (!SearchActivate) return;
                Keyboard.ClearFocus();
                IELButtonSearchCommand.QBackground.SetUsedState(false);
                IELButtonText Button;
                Grid MainElementsContainer = (Grid)ScrollViewerElements.Content;
                for (int Index = 0; Index < MainElementsContainer.Children.Count; Index++)
                {
                    Button = (IELButtonText)MainElementsContainer.Children[Index];
                    Button.IsEnabled = true;
                    Canvas.SetZIndex(Button, 0);
                    App.ThicknessAnimationType.AnimateEffect(Button, MarginProperty,
                        GetElementMarginFromPositionIndex(Index), TimeSpan.FromMilliseconds(300d));
                    App.DoubleAnimationType.AnimateEffect(Button, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                }
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
            Activated += (sender, e) =>
            {
                if (!StartAnimation)
                {
                    StartAnimation = true;
                    App.ColorAnimationType.AnimateEffect(GradientStopBackground1, GradientStop.ColorProperty,
                        System.Windows.Media.Color.FromRgb(10, 30, 37), System.Windows.Media.Color.FromRgb(22, 73, 82), TimeSpan.FromMilliseconds(2600d));
                    App.ColorAnimationType.AnimateEffect(GradientStopBackground2, GradientStop.ColorProperty,
                        System.Windows.Media.Color.FromRgb(5, 100, 60), System.Windows.Media.Color.FromRgb(57, 158, 151), TimeSpan.FromMilliseconds(2000d));
                }
            };
            App.CurrentApp.LogWriteLine("Готово!");
        }

        //
        private void ClearVisualGrid()
        {
            if (ScrollViewerElements.Content != null)
            {
                Grid ContentVisualElements = (Grid)ScrollViewerElements.Content;
                ContentVisualElements.Children.Clear();
                ScrollViewerElements.Content = null;
            }
        }

        /// <summary>
        /// Асинхронный метод обновления списка всех описываемых элементов
        /// </summary>
        /// <typeparam name="T">Тип ожидаемых элеменов для описания</typeparam>
        /// <param name="Elements">Массив элеменов подвергаемые к описанию</param>
        /// <param name="ElementDiscriptionPage">Страница с помощью которой будет описываться объект</param>
        /// <returns>Сетка с распределёнными объектами описания</returns>
        private async Task<Grid> InitializeVisualElementDiscription<T>(T[] Elements, IDiscriptionPage<T> ElementDiscriptionPage) where T : ICommandOPER
        {
            Grid GridElements = new()
            {
                Opacity = 0d,
            };
            int i = 0;
            foreach (T Element in Elements)
            {
                await Task.Run((Action)(() =>
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        IELButtonText Button = GenerateCommandButton();
                        Button.Opacity = 0d;
                        Button.Margin = GetElementMarginFromPositionIndex(i);
                        Button.Text = Element.Name;
                        Button.OnActivateMouseLeft += (sender, e, Key) =>
                        {
                            ElementDiscriptionPage.UpdateInformation(Element);
                            //App.AnimateDoubleEffect(DescriptionConsole, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                        };
                        GridElements.Children.Add(Button);
                        App.DoubleAnimationType.AnimateEffect(Button, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                    }));
                    Thread.Sleep(200);
                    i++;
                }));
            }
            return GridElements;
        }

        /// <summary>
        /// Узнать позицию элемента по его позиционному индексу
        /// </summary>
        /// <param name="Index">Позиционный индекс (0 первый элемент и т.д)</param>
        /// <returns>Позиция которая принадлежит текущему элементу</returns>
        private static Thickness GetElementMarginFromPositionIndex(int Index) => new(3, Index > 0 ? 58 * Index + 3 : 3, 3, 3);

        /// <summary>
        /// Установить управление информацией описания с помощью кнопок
        /// </summary>
        /// <param name="Value">Устанавливаемое состояние</param>
        private void SetStateManipulateInformation(bool Value)
        {
            IELButtonCloneTextCommand.IsEnabled = Value;
            IELButtonBack.IsEnabled = Value;
        }

        /// <summary>
        /// Анимировать отображение выделения кнопки
        /// </summary>
        /// <param name="ElementSelect">Объект выделяемой кнопки</param>
        private void AnimateEnterPageButton(FrameworkElement ElementSelect)
        {
            App.ThicknessAnimationType.AnimateEffect(ImageSelectIndicator, MarginProperty,
                    new(ElementSelect.TranslatePoint(new(0, 0), GridMainButtonsPagesInformation).X + ElementSelect.ActualWidth / 2d - 6, -9, 0, -7),
                    TimeSpan.FromMilliseconds(200d));
            if (RotateTransformImageSelectIndicator.Angle == 0d)
                App.DoubleAnimationType.AnimateEffect(RotateTransformImageSelectIndicator, RotateTransform.AngleProperty, -90d, TimeSpan.FromMilliseconds(200d));
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
                },
                Margin = new(3),
            };
            Element.QBackground.ColorData = new(new byte[,]
                        {
                        { 255, 161, 204, 232 },
                        { 255, 92, 131, 157 },
                        { 255, 122, 172, 205 },
                        { 255, 166, 181, 190 },
                        });
            Element.QBorderBrush.ColorData = (QData)BorderForegroundSetting.Clone();
            Element.QForeground.ColorData = (QData)BorderForegroundSetting.Clone();
            System.Windows.Data.Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["RussianRail G Pro"]
            };
            BindingOperations.SetBinding(Element, IELButtonText.FontFamilyProperty, binding);
            return Element;
        }
    }
}
