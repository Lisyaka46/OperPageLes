using IEL.CORE.Enums;
using IEL.UserElementsControl;
using Interpreter.Interfaces;
using OperPageLes.CORE;
using OperPageLes.CORE.Enums.Theme;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Pages.Description;
using OPLAPI.OIEL.UserElementsControl.Base;
using OPLAPI.OIEL.UserElementsControl.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowDiscriptionCommands.xaml
    /// </summary>
    public partial class WindowDiscriptionCommands : OPLWindowBase
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
        /// Объект отображения элементов в визуализационном стековом контенте
        /// </summary>
        private StackPanel? VisualElementsInformations;

        public WindowDiscriptionCommands()
        {
            App.LogWriteLine("Создание окна описания элементов");
            StartAnimation = false;
            SearchActivate = false;
            InitializeComponent();
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.File));
            ImageSelectIndicator.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.ArrowRight));

            #region Palette
            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.Cocoa].ConnectPalleteFromIELElement(IELInputSearchCommand);
            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.Tangerine].ConnectPalleteFromIELElement(IELButtonSearchCommand);

            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.Jade].ConnectPalleteFromIELElement(IELButtonConsole);
            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.Jade].ConnectPalleteFromIELElement(IELButtonAlias);

            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.Green].ConnectPalleteFromIELElement(IELButtonBack);
            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.LightBlue].ConnectPalleteFromIELElement(IELButtonCloneTextCommand);
            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.PastelBlue].ConnectPalleteFromIELElement(IELButtonInfoParameter);
            #endregion

            IELButtonAlias.IsEnabled = App.CurrentApp.Interpreter.AliasesCount > 0;
            IELButtonSearchCommand.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Search));

            IELMessageInfo.Opacity = 0d;
            DescriptionConsole = new();
            DescriptionConsole.ChangeStateSelectCommand += SetStateManipulateInformation;
            DescriptionAlias = new();
            DescriptionAlias.ChangeStateSelectCommand += SetStateManipulateInformation;
            SetStateManipulateInformation(false);

            IELButtonCloneTextCommand.OnActivateMouseLeft += (sender, e) =>
            {
                string? Text;
                if (StateDiscription == ActivateStateDiscription.ConsoleCommand) Text = DescriptionConsole.GetCommandText();
                else if (StateDiscription == ActivateStateDiscription.AliasCommand) Text = DescriptionAlias.GetCommandText();
                else return;
                System.Windows.Clipboard.SetText(Text ?? string.Empty);
            };

            IELButtonBack.OnActivateMouseLeft += (sender, e) =>
            {
                if (StateDiscription == ActivateStateDiscription.ConsoleCommand) DescriptionConsole.ClearInformationOnCommand();
                else if (StateDiscription == ActivateStateDiscription.AliasCommand) DescriptionAlias.ClearInformationOnCommand();
                else return;
            };

            #region IELImageButtonClose
            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.Red].ConnectPalleteFromIELElement(IELImageButtonClose);
            IELImageButtonClose.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
            IELImageButtonClose.OnActivateMouseLeft += (sender, e) =>
            {
                Close();
            };
            #endregion

            #region IELButtonInfoParameter
            IELButtonInfoParameter.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.LightBulb));
            IELButtonInfoParameter.MouseEnter += (sender, e) =>
            {
                IELMessageInfo.UsingBorderInformation(IELButtonInfoParameter,
                    "Символ \"~\" является пропускным символом, альтернативой \" \", для записи пропущенного символа в параметры нужно ввести \"~~\"\n\n" +
                    "Символ \"%\" является специальным символом (Одинарный символ пропускается):\n" +
                    "- Для записи \"%\" в параметры нужно ввести \"%%\"\n" +
                    "- Для записи \",\" в параметры нужно ввести \"%,\"",
                    OrientationPositionCursor.LeftDown);
            };
            IELButtonInfoParameter.MouseLeave += (sender, e) => IELMessageInfo.CloseBorderInformation();
            #endregion

            #region IELButtonConsole
            IELButtonConsole.OnActivateMouseLeft += (sender, e) =>
            {
                StateDiscription = ActivateStateDiscription.ConsoleCommand;
                SetStateManipulateInformation(DescriptionConsole.SelectCommand);
                AnimateEnterPageButton(IELButtonConsole);
                IELControllerDescription.NextElement(DescriptionConsole, false);

                Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
                {
                    if (VisualElementsInformations != null)
                        ClearVisualElementsManager();
                    VisualElementsInformations = await App.CurrentApp.ExecuteVisualizateLoadingProcess(
                        InitializeVisualElementDiscription([.. App.CurrentApp.Interpreter.Commands.Values], DescriptionConsole));
                    ScrollViewerElements.Content = VisualElementsInformations;
                    //App.CurrentApp.ManagerAnimation.DoubleAnimationType.AnimateEffect(VisualElementsInformations, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                });
            };
            #endregion

            #region IELButtonAlias
            IELButtonAlias.OnActivateMouseLeft += (sender, e) =>
            {
                StateDiscription = ActivateStateDiscription.AliasCommand;
                SetStateManipulateInformation(DescriptionAlias.SelectCommand);
                AnimateEnterPageButton(IELButtonAlias);
                IELControllerDescription.NextElement(DescriptionAlias, true);

                Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
                {
                    if (VisualElementsInformations != null)
                        ClearVisualElementsManager();
                    VisualElementsInformations = await App.CurrentApp.ExecuteVisualizateLoadingProcess(
                        InitializeVisualElementDiscription([.. App.CurrentApp.Interpreter.Aliases.Values], DescriptionAlias));
                    ScrollViewerElements.Content = VisualElementsInformations;
                    //App.CurrentApp.ManagerAnimation.DoubleAnimationType.AnimateEffect(VisualElementsInformations, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                });
            };
            #endregion

            #region IELButtonSearchCommand
            IELButtonSearchCommand.OnActivateMouseLeft += (sender, e) =>
            {
                if (ScrollViewerElements.Content == null || IELInputSearchCommand.Text.Length == 0 ||
                    VisualElementsInformations == null) return;
                ICommandOPER<IOPERCommandViewer>[] SearchCommands =
                    [.. App.CurrentApp.Interpreter.CommandWhere((i) => i.Name.Contains(IELInputSearchCommand.Text))];
                if (SearchCommands.Length > 0)
                {
                    SearchActivate = true;
                    IELButtonText Button;
                    IELButtonSearchCommand.SourceBackground.UsedState = true;
                    for (int i = 0; i < VisualElementsInformations.Children.Count; i++)
                    {
                        Button = (IELButtonText)VisualElementsInformations.Children[i];
                        bool SearchComplete = SearchCommands.Any((k) => k.Name.Equals(Button.Text));

                        Button.IsEnabled = SearchComplete;
                        //App.AnimationManager.DoubleAnimationType.AnimateEffect(Button, HeightProperty,
                        //    SearchComplete ? Button.ActualHeight : 0d, TimeSpan.FromMilliseconds(300d));
                        //App.CurrentApp.ManagerAnimation.DoubleAnimationType.AnimateEffect(Button, OpacityProperty,
                        //    SearchComplete ? 1d : 0d, TimeSpan.FromMilliseconds(300d));
                    }
                }
            };
            IELButtonSearchCommand.OnActivateMouseRight += (sender, e) =>
            {
                if (!SearchActivate || VisualElementsInformations == null) return;
                Keyboard.ClearFocus();
                IELButtonSearchCommand.SourceBackground.UsedState = false;
                IELButtonText Button;
                for (int Index = 0; Index < VisualElementsInformations.Children.Count; Index++)
                {
                    Button = (IELButtonText)VisualElementsInformations.Children[Index];
                    Button.IsEnabled = true;
                    //App.AnimationManager.DoubleAnimationType.AnimateEffect(Button, HeightProperty, Button.ActualHeight, TimeSpan.FromMilliseconds(300d));
                    //App.CurrentApp.ManagerAnimation.DoubleAnimationType.AnimateEffect(Button, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
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
                }
            };
            App.LogWriteLine("Готово!");
        }

        /// <summary>
        /// Очистить все отображаемые элементы
        /// </summary>
        private void ClearVisualElementsManager()
        {
            if (VisualElementsInformations == null)
                throw new Exception("Невозможно очистить объект который являетмя нулевым.");
            VisualElementsInformations.Children.Clear();
            VisualElementsInformations.UpdateLayout();
            ScrollViewerElements.ClearValue(IELScrollViewer.ContentProperty);
            GC.Collect();
        }

        /// <summary>
        /// Асинхронный метод обновления списка всех описываемых элементов
        /// </summary>
        /// <typeparam name="T">Тип ожидаемых элеменов для описания</typeparam>
        /// <param name="Elements">Массив элеменов подвергаемые к описанию</param>
        /// <param name="ElementDiscriptionPage">Страница с помощью которой будет описываться объект</param>
        /// <returns>Сетка с распределёнными объектами описания</returns>
        private async Task<StackPanel> InitializeVisualElementDiscription<T>(T[] Elements, IDiscriptionPage<T> ElementDiscriptionPage)
            where T : ICommandOPER<IOPERCommandViewer>
        {
            StackPanel StackPanelElements = new()
            {
                Opacity = 0d,
                Orientation = System.Windows.Controls.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
            };
            int i = 0;
            foreach (T Element in Elements)
            {
                await Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        IELButtonText Button = GenerateCommandButton();
                        Button.Opacity = 0d;
                        //Button.Margin = GetElementMarginFromPositionIndex(i);
                        Button.Text = Element.Name;
                        Button.OnActivateMouseLeft += (sender, e) =>
                        {
                            ElementDiscriptionPage.UpdateInformation(Element);
                            //App.AnimateDoubleEffect(DescriptionConsole, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                        };
                        StackPanelElements.Children.Add(Button);
                        //App.CurrentApp.ManagerAnimation.DoubleAnimationType.AnimateEffect(Button, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                    });
                    Thread.Sleep(200);
                    i++;
                });
            }
            return StackPanelElements;
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
            //App.CurrentApp.ManagerAnimation.ThicknessAnimationType.AnimateEffect(ImageSelectIndicator, MarginProperty,
            //        new(ElementSelect.TranslatePoint(new(0, 0), GridMainButtonsPagesInformation).X + ElementSelect.ActualWidth / 2d - 6, -9, 0, -7),
            //        TimeSpan.FromMilliseconds(200d));
            //if (RotateTransformImageSelectIndicator.Angle == 0d)
            //    App.CurrentApp.ManagerAnimation.DoubleAnimationType.AnimateEffect(RotateTransformImageSelectIndicator, RotateTransform.AngleProperty, -90d, TimeSpan.FromMilliseconds(200d));
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
                Margin = new(3),
                CornerRadius = new(10),
                Padding = new(0, 5, 0, 5),
                MarginViewBox = new(5, 10, 5, 10),
            };
            //App.CurrentApp.ActiveThemeApplication[PaletteEnum.Jade].ConnectPalleteFromIELElement(Element);
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
