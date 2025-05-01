using OperPage_les.CORE.Flaging;
using Interpreter.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using IEL;
using DataScroll;
using System.Windows.Data;
using System.Runtime.CompilerServices;
using IEL.Interfaces.Front;
using Interpreter.Classes;
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
        /// Объект анимации для управления прозрачностью
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimate = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления цветом
        /// </summary>
        private readonly ColorAnimation AnimationColor = new(Colors.Black, TimeSpan.FromMilliseconds(250d))
        {
            From = Colors.White,
        };

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Скролл-бар для данных элемнтов описания
        /// </summary>
        private CounterScrollBar ScrollBar;

        /// <summary>
        /// Константа размера Height для элементов описания
        /// </summary>
        private const int HeightElement = 55;

        /// <summary>
        /// Расстояние между объектами описания
        /// </summary>
        private const int OffsetY = 4;

        /// <summary>
        /// Состояние описания
        /// </summary>
        private ActivateStateDiscription StateDiscription;

        /// <summary>
        /// Массив индексов поиска элементов описания команд
        /// </summary>
        private int[] IndexSearch = [];

        public WindowDiscriptionCommands()
        {
            InitializeComponent();
            StateDiscription = ActivateStateDiscription.NotActivated;
            //IELInputSearchCommand.Text = string.Empty;
            ScrollBar = new((int)(GridMainElements.ActualHeight / HeightElement));
            IELMessageInfo.Opacity = 0d;
            TextBlockTextCommand.Foreground = new SolidColorBrush(Colors.Black);
            GridMainElements.MouseWheel += (sender, e) =>
            {
                if (ScrollBar.ScrollActivate && GridElements.Children.Count > 0)
                {
                    if (e.Delta > 0) ScrollBar.Up();
                    else if (e.Delta < 0) ScrollBar.Down();
                }
            };
            #region IELButtonConsole
            IELButtonConsole.MouseEnter += (sender, e) => AnimateButtonBookmark(IELButtonConsole, 4);
            IELButtonConsole.MouseLeave += (sender, e) => AnimateButtonBookmark(IELButtonConsole, 0);
            IELButtonConsole.OnActivateMouseLeft += () =>
            {
                if (StateDiscription == ActivateStateDiscription.ConsoleCommand) return;
                IELButtonConsole.BackgroundSetting.UsedState = true;
                StateDiscription = ActivateStateDiscription.ConsoleCommand;
                DoubleAnimation animation = DoubleAnimate;
                animation.To = 1d;
                animation.BeginTime = TimeSpan.FromMilliseconds(10d);
                animation.Duration = TimeSpan.FromMilliseconds(700d);
                ScrollBar = CreateScrollBar(App.DataConsoleCommand.Count);
                if (GridElements.Children.Count > 0) GridElements.Children.Clear();
                for (int i = 0; i < App.DataConsoleCommand.Count; i++)
                {
                    ConsoleCommand commandAAC = App.DataConsoleCommand[i];
                    IELButtonText Button = GenerateCommandButton();
                    Button.Opacity = 0d;
                    Button.Margin = new(3, HeightElement * i + OffsetY * (i + 1), 3, 0);
                    Button.Text = commandAAC.Name;
                    Button.OnActivateMouseLeft += () => DetectNewDiscriptionCommand(commandAAC);
                    GridElements.Children.Add(Button);
                    animation.BeginTime += TimeSpan.FromMilliseconds(10d);
                    Button.BeginAnimation(OpacityProperty, animation);
                }
            };
            IELButtonConsole.OnActivateMouseRight += () =>
            {
                Keyboard.ClearFocus();
                if (StateDiscription != ActivateStateDiscription.ConsoleCommand) return;
                StateDiscription = ActivateStateDiscription.NotActivated;
                IELButtonConsole.BackgroundSetting.UsedState = false;
                ScrollBar.Value = 0;
                ThicknessAnimation AnimationMargin = ThicknessAnimate.Clone();
                AnimationMargin.BeginTime = TimeSpan.Zero;
                DoubleAnimation AnimationOpacity = DoubleAnimate.Clone();
                AnimationOpacity.To = 0d;
                Thickness MarginChild;
                for (int i = 0; i < GridElements.Children.Count; i++)
                {
                    MarginChild = ((FrameworkElement)GridElements.Children[i]).Margin;
                    AnimationMargin.BeginTime += TimeSpan.FromMilliseconds(1d * i);
                    AnimationOpacity.BeginTime = AnimationMargin.BeginTime;
                    AnimationMargin.To = new(MarginChild.Left, MarginChild.Top - 4, MarginChild.Right, MarginChild.Bottom + 4);
                    if (i == GridElements.Children.Count - 1)
                    {
                        AnimationOpacity.FillBehavior = FillBehavior.Stop;
                        AnimationOpacity.Completed += (sender, e) =>
                        {
                            if (StateDiscription == ActivateStateDiscription.NotActivated)
                                GridElements.Children.Clear();
                        };
                    }
                    GridElements.Children[i].BeginAnimation(MarginProperty, AnimationMargin);
                    GridElements.Children[i].BeginAnimation(OpacityProperty, AnimationOpacity);
                }
                UpdateVisibleScrollBar(-1);
                DetectNewDiscriptionCommand<ICommandAAC>(null);
            };
            #endregion
            #region IELButtonUserCom
            IELButtonUserCom.MouseEnter += (sender, e) => AnimateButtonBookmark(IELButtonUserCom, 4);
            IELButtonUserCom.MouseLeave += (sender, e) => AnimateButtonBookmark(IELButtonUserCom, 0);
            #endregion
            #region IELButtonSearchCommand
            IELButtonSearchCommand.MouseEnter += (sender, e) => AnimateButtonBookmark(IELButtonSearchCommand, 4);
            IELButtonSearchCommand.MouseLeave += (sender, e) => AnimateButtonBookmark(IELButtonSearchCommand, 0);
            IELButtonSearchCommand.OnActivateMouseLeft += () =>
            {
                switch (StateDiscription)
                {
                    case ActivateStateDiscription.NotActivated: return;
                    case ActivateStateDiscription.ConsoleCommand:
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
                        break;
                }
                IELButtonText Button;
                foreach (int Index in IndexSearch)
                {
                    Button = (IELButtonText)GridElements.Children[Index];
                    if (Button.BackgroundSetting.UsedState) continue;
                    Button.BackgroundSetting.UsedState = true;
                }
            };
            IELButtonSearchCommand.OnActivateMouseRight += () =>
            {
                Keyboard.ClearFocus();
                if (IndexSearch.Length == 0) return;
                if (StateDiscription != ActivateStateDiscription.NotActivated)
                {
                    IELButtonText Button;
                    foreach (int Index in IndexSearch)
                    {
                        Button = (IELButtonText)GridElements.Children[Index];
                        if (!Button.BackgroundSetting.UsedState) continue;
                        Button.BackgroundSetting.UsedState = false;
                    }
                }
                IndexSearch = [];
            };
            #endregion
            SizeChanged += (sender, e) =>
            {
                if (GridMainElements.ActualHeight == 0d) return;
                int ScrollCountVisible = (int)((GridMainElements.ActualHeight) / (HeightElement + OffsetY)) * ScrollBar.TrafficShare;
                if (ScrollCountVisible != ScrollBar.CountVisibleElements)
                {
                    int Value = Math.Abs(ScrollCountVisible - ScrollBar.CountVisibleElements) / ScrollBar.TrafficShare;
                    if (ScrollCountVisible > ScrollBar.CountVisibleElements)
                    {
                        ScrollBar.VisibleUp(Value);
                    }
                    else if (ScrollCountVisible < ScrollBar.CountVisibleElements)
                    {
                        ScrollBar.VisibleDown(Value);
                    }
                }
                if (ScrollBar.MaxValue > 0)
                {
                    DoubleAnimate.To = ShareElement(ScrollBar.MaxValue);
                    RectangleScrollBar.BeginAnimation(HeightProperty, DoubleAnimate);
                    ThicknessAnimate.To = new(0, ShareElement(ScrollBar.MaxValue) * ScrollBar.Value, 0, 0);
                    RectangleScrollBar.BeginAnimation(MarginProperty, ThicknessAnimate);
                }
            };
            Closing += (sender, e) =>
            {
                App.MainWindowApplication?.Activate();
                App.AppWindows.DiscriptionCommands = null;
            };
            IELButtonInfoParameter.MouseEnter += (sender, e) =>
            {
                IELMessageInfo.UsingBorderInformation(IELButtonInfoParameter,
                    nameof(IELButtonInfoParameter),
                    "Символ \"~\" является пропускным символом, альтернативой \" \", для записи пропущенного символа в параметры нужно ввести \"~~\"\n\n" +
                    "Символ \"%\" является специальным символом (Одинарный символ пропускается):\n" +
                    "- Для записи \"%\" в параметры нужно ввести \"%%\"\n" +
                    "- Для записи \",\" в параметры нужно ввести \"%,\"",
                    IELBlockMessage.OrientationBorderInfo.LeftDown);
            };
            IELButtonInfoParameter.MouseLeave += (sender, e) => IELMessageInfo.CloseBorderInformation();
            IELButtonCloneTextCommand.OnActivateMouseLeft += () =>
            {
                Clipboard.SetText(TextBlockTextCommand.Text);
                AnimationColor.To = Colors.Black;
                TextBlockTextCommand.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, AnimationColor);
            };
            RectangleScrollBar.MouseUp += (sender, e) =>
            {
                if (ScrollBar.MaxValue > 0) ScrollBar.Value = 0;
            };
        }

        /// <summary>
        /// Доля прокрутки одного элемента
        /// </summary>
        private double ShareElement(int Max) => (BorderScroll.ActualHeight - OffsetY) / (Max + 1);

        /// <summary>
        /// Сгенерировать объект скролл-бара
        /// </summary>
        /// <param name="CountElements">Количество созданных элементов</param>
        /// <returns>Скролл-бар</returns>
        private CounterScrollBar CreateScrollBar(int CountElements)
        {
            CounterScrollBar Bar = new((int)GridMainElements.ActualHeight / (HeightElement + OffsetY), CountElements: CountElements);
            Bar.ChangedValue += (NewValue) =>
            {
                ThicknessAnimate.To = new(0, -(HeightElement + OffsetY) * NewValue, 0, 0);
                GridElements.BeginAnimation(MarginProperty, ThicknessAnimate);
                ThicknessAnimate.To = new(0, OffsetY / 2 + ShareElement(Bar.MaxValue) * NewValue, 0, 0);
                RectangleScrollBar.BeginAnimation(MarginProperty, ThicknessAnimate);
            };
            if (Bar.MaxValue > 0)
            {
                DoubleAnimate.To = ShareElement(Bar.MaxValue);
                RectangleScrollBar.BeginAnimation(HeightProperty, DoubleAnimate);
            }
            UpdateVisibleScrollBar(Bar.MaxValue);
            return Bar;
        }

        /// <summary>
        /// Предоставить новое описание команды
        /// </summary>
        /// <param name="Command">Команда для описания</param>
        private void DetectNewDiscriptionCommand<T>(T? Command) where T : ICommandAAC
        {
            if (Command == null)
            {
                TextBlockNameCommand.Text = "Команда не выбрана";
                TextBlockDescriptionCountParameter.Text = string.Empty;
                TextBlockTextCommand.Text = string.Empty;
                TextBlockMainDescriptionCommand.Text = string.Empty;
                IELButtonCloneTextCommand.IsEnabled = false;
                return;
            }
            else
            {
                IELButtonCloneTextCommand.IsEnabled = true;
                switch (StateDiscription)
                {
                    case ActivateStateDiscription.ConsoleCommand:
                        SetInformationCommand((IConsoleCommand)Command);
                        break;
                    default:
                        SetInformationCommand(Command);
                        break;
                }
            }
        }

        /// <summary>
        /// Сгенерировать кнопку команнды описания
        /// </summary>
        /// <returns>Кнопка команды</returns>
        private static IELButtonText GenerateCommandButton()
        {
            IELButtonText Element = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Height = HeightElement,
                VisibleMouseImaging = false,
                FontSize = 13d,
                BorderThicknessBlock = new(2),
            };
            Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (Style)Application.Current.Resources["Brenzo Slab Personal Use"]
            };
            BindingOperations.SetBinding(Element, IELButtonText.StyleProperty, binding);
            return Element;
        }

        /// <summary>
        /// Установить информацию о команде
        /// </summary>
        /// <param name="Command">Команда для описания</param>
        private void SetInformationCommand(IConsoleCommand Command)
        {
            Parameter[] Parameters = Command.Parameters ?? [];
            int CountParameters = Parameters.Length;
            string TextRegistration = string.Empty;
            for (int i = 0; i < Parameters.Length; i++)
            {
                TextRegistration += $"{Parameters[i].Name}" +
                    $"{(Parameters[i].Absolutly ? string.Empty : '?')}" +
                    $"{(i < Parameters.Length - 1 ? ", " : string.Empty)}";
            }
            string StateCommand = StateDiscription switch
            {
                ActivateStateDiscription.ConsoleCommand => "Консольная",
                _ => string.Empty
            };
            TextBlockNameCommand.Text = $"{StateCommand} команда: \"{Command.Name}\"";
            TextBlockMainDescriptionCommand.Text = Command.Description;
            TextBlockDescriptionCountParameter.Text = CountParameters == 0 ?
            $"Команда \"{Command.Name}\" не использует параметров" : $"Команда \"{Command.Name}\" включает в себя {CountParameters} и больше параметров";
            TextBlockTextCommand.Text = Command.Name.Trim() + (CountParameters == 0 ? string.Empty : "* " + TextRegistration);
        }

        /// <summary>
        /// Установить информацию о команде
        /// </summary>
        /// <param name="Command">Команда для описания</param>
        private void SetInformationCommand(ICommandAAC Command)
        {
            string StateCommand = StateDiscription switch
            {
                ActivateStateDiscription.ConsoleCommand => "Консольная",
                _ => string.Empty
            };
            TextBlockNameCommand.Text = $"{StateCommand} команда: \"{Command.Name}\"";
            TextBlockMainDescriptionCommand.Text = string.Empty;
            TextBlockDescriptionCountParameter.Text = $"Команда \"{Command.Name}\" не использует параметров";
            TextBlockTextCommand.Text = Command.Name.Trim();
        }

        /// <summary>
        /// Анимировать кнопку как закладку
        /// </summary>
        /// <param name="Button">Объект кнопки</param>
        /// <param name="Offset">Оффсет вытягивания</param>
        private static void AnimateButtonBookmark(FrameworkElement Button, int Offset)
        {
            ThicknessAnimation animation = ThicknessAnimate.Clone();
            animation.To = new(Button.Margin.Left, 0, Button.Margin.Right, 7 - Offset);
            Button.BeginAnimation(MarginProperty, animation);
        }

        /// <summary>
        /// Обновить видимость скролл-бара
        /// </summary>
        /// <param name="Max">Максимальное значение объекта скролл-бара</param>
        private void UpdateVisibleScrollBar(int Max)
        {
            DoubleAnimation animation = DoubleAnimate.Clone();
            animation.To = Max > 0 ? 13d : 4d;
            Storyboard storyboard = new();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, ScrollColumnDefinition);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(ColumnDefinition.MaxWidth)"));
            storyboard.Begin();
        }
    }
}
