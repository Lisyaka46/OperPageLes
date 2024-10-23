using AAC20.Classes.Flaging;
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

namespace AAC20.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowDiscriptionCommands.xaml
    /// </summary>
    public partial class WindowDiscriptionCommands : Window
    {
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
        private const int HeightElement = 34;

        /// <summary>
        /// Оффсет объектов описания
        /// </summary>
        private const int OffsetY = 4;

        public WindowDiscriptionCommands()
        {
            InitializeComponent();
            //IELInputSearchCommand.Text = string.Empty;
            ScrollBar = new((int)(GridMainElements.ActualHeight / HeightElement));
            GridMainDescription.Opacity = 0d;
            BorderContextMenuParameter.Opacity = 0d;
            TextBlockTextCommand.Foreground = new SolidColorBrush(Colors.Black);
            GridMainElements.MouseWheel += (sender, e) =>
            {
                if (ScrollBar.ScrollActivate && GridElements.Children.Count > 0)
                {
                    if (e.Delta > 0) ScrollBar.Up();
                    else if (e.Delta < 0) ScrollBar.Down();
                }
            };
            IELButtonConsole.OnActivateMouseLeft += () =>
            {
                int i = 0;
                DoubleAnimation animation = DoubleAnimate;
                animation.To = 1d;
                animation.BeginTime = TimeSpan.FromMilliseconds(10d);
                animation.Duration = TimeSpan.FromMilliseconds(700d);
                ScrollBar = CreateScrollBar(App.DataConsoleCommand.Count);
                foreach (ConsoleCommand commandAAC in App.DataConsoleCommand)
                {
                    IELButtonText Button = GenerateCommandButton();
                    Button.Opacity = 0d;
                    Button.Margin = new(3, (HeightElement + OffsetY) * i++ + OffsetY, 3, 0);
                    Button.Text = commandAAC.Name;
                    Button.OnActivateMouseLeft += () => DetectNewDiscriptionCommand(commandAAC);
                    GridElements.Children.Add(Button);
                    animation.BeginTime += TimeSpan.FromMilliseconds(10d);
                    Button.BeginAnimation(OpacityProperty, animation);
                }
            };
            SizeChanged += (sender, e) =>
            {
                if (GridMainElements.ActualHeight == 0d) return;
                int ScrollCountVisible = (int)((ActualHeight) / (HeightElement + OffsetY * 2)) * ScrollBar.TrafficShare;
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
                    if (ScrollBar.MaxValue > 0)
                    {
                        DoubleAnimate.To = ShareElement(ScrollBar.MaxValue);
                        RectangleScrollBar.BeginAnimation(HeightProperty, DoubleAnimate);
                        ThicknessAnimate.To = new(0, ShareElement(ScrollBar.MaxValue) * ScrollBar.Value, 0, 0);
                        RectangleScrollBar.BeginAnimation(MarginProperty, ThicknessAnimate);
                    }
                }
            };
            Closing += (sender, e) =>
            {
                App.MainWindowApplication?.Activate();
                App.AppWindows.DiscriptionCommands = null;
            };
            IELButtonInfoParameter.MouseUp += (sender, e) =>
            {
                if (!Flags.ContextMenuParameter)
                {
                    Flags.ContextMenuParameter.Value = true;
                    Canvas.SetZIndex(BorderContextMenuParameter, 1);
                    Point MousePoint = Mouse.GetPosition(GridDiscription);
                    BorderContextMenuParameter.Margin = new Thickness(MousePoint.X - BorderContextMenuParameter.ActualWidth - 4, MousePoint.Y + 4, 0, 0);
                    DoubleAnimate.To = 1d;
                    BorderContextMenuParameter.BeginAnimation(OpacityProperty, DoubleAnimate);
                }
            };
            IELButtonInfoParameter.MouseLeave += (sender, e) =>
            {
                if (Flags.ContextMenuParameter)
                {
                    Flags.ContextMenuParameter.Value = false;
                    Canvas.SetZIndex(BorderContextMenuParameter, -1);
                    DoubleAnimate.To = 0d;
                    BorderContextMenuParameter.BeginAnimation(OpacityProperty, DoubleAnimate);
                }
            };
            IELButtonCloneTextCommand.OnActivateMouseLeft += () =>
            {
                Clipboard.SetText(TextBlockTextCommand.Text);
                AnimationColor.To = Colors.Black;
                TextBlockTextCommand.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, AnimationColor);
            };
            IELButtonSearchCommand.MouseLeftButtonUp += (sender, e) =>
            {
                int[] IndexSearch = [..Enumerable.Range(0, App.DataConsoleCommand.Count).Where(
                    i => App.DataConsoleCommand[i].Name.Contains(IELInputSearchCommand.Text))];
                AnimationColor.To = Color.FromRgb(247, 246, 220);
                IELButtonText Button;
                foreach (int Index in IndexSearch)
                {
                    Button = (IELButtonText)GridElements.Children[Index];
                    //Button.SettingAnimate.BackgroundDNSU.Default = AnimationColor.To ?? default;
                    //Button.BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, AnimationColor);
                }
            };
            IELButtonSearchCommand.MouseRightButtonUp += (sender, e) =>
            {
                AnimationColor.To = Color.FromRgb(207, 206, 160);
                IELButtonText Button;
                foreach (UIElement Element in GridElements.Children)
                {
                    Button = (IELButtonText)Element;
                    //if (Button.SettingAnimate.BackgroundDNSU.Default == AnimationColor.To) continue;
                    //Button.SettingAnimate.BackgroundDNSU.Default = AnimationColor.To ?? default;
                    //Button.BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, AnimationColor);
                }
            };
            RectangleScrollBar.MouseUp += (sender, e) =>
            {
                if (ScrollBar.MaxValue > 0) ScrollBar.Value = 0;
            };
        }

        /// <summary>
        /// Доля прокрутки одного элемента
        /// </summary>
        private double ShareElement(int Max) => (BorderScroll.ActualHeight - BorderScroll.Padding.Top - BorderScroll.Padding.Bottom - 4) / (Max + 1);

        /// <summary>
        /// Сгенерировать объект скролл-бара
        /// </summary>
        /// <param name="CountElements">Количество созданных элементов</param>
        /// <returns>Скролл-бар</returns>
        private CounterScrollBar CreateScrollBar(int CountElements)
        {
            CounterScrollBar Bar = new((int)Math.Ceiling(GridMainElements.ActualHeight / HeightElement), CountElements: CountElements);
            Bar.ChangedValue += (NewValue) =>
            {
                ThicknessAnimate.To = new(0, -((HeightElement + OffsetY) + (OffsetY / 2)) * NewValue, 0, 0);
                GridElements.BeginAnimation(MarginProperty, ThicknessAnimate);
                ThicknessAnimate.To = new(0, ShareElement(Bar.MaxValue) * NewValue, 0, 0);
                RectangleScrollBar.BeginAnimation(MarginProperty, ThicknessAnimate);
            };
            DoubleAnimation animation = DoubleAnimate.Clone();
            Storyboard storyboard = new();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, ScrollColumnDefinition);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(ColumnDefinition.MaxWidth)"));
            if (Bar.MaxValue > 0)
            {
                DoubleAnimate.To = ShareElement(Bar.MaxValue);
                RectangleScrollBar.BeginAnimation(HeightProperty, DoubleAnimate);
                if (ScrollColumnDefinition.MaxWidth < 13d)
                {
                    animation.To = 13d;
                    storyboard.Begin();
                }
            }
            else if (ScrollColumnDefinition.MaxWidth > 4d)
            {
                animation.To = 4d;
                storyboard.Begin();
            }
            return Bar;
        }

        /// <summary>
        /// Предоставить новое описание команды
        /// </summary>
        /// <param name="Command">Команда для описания</param>
        private void DetectNewDiscriptionCommand(ICommandAAC Command)
        {
            if (GridMainDescription.Opacity == 0d)
            {
                SetInformationCommand(Command);
                DoubleAnimate.To = 1d;
                GridMainDescription.BeginAnimation(OpacityProperty, DoubleAnimate);
                return;
            }
            DoubleAnimation AnimateOpacity = new(0, TimeSpan.FromMilliseconds(250d))
            {
                DecelerationRatio = 0.2d,
                EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut },
                To = 0d,
                FillBehavior = FillBehavior.Stop,
            };
            AnimateOpacity.Completed += (sender, e) =>
            {
                GridMainDescription.BeginAnimation(OpacityProperty, null);
                GridMainDescription.Opacity = 0d;
                SetInformationCommand(Command);
                DoubleAnimate.To = 1d;
                GridMainDescription.BeginAnimation(OpacityProperty, DoubleAnimate);
            };
            GridMainDescription.BeginAnimation(OpacityProperty, AnimateOpacity);
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
                FontSize = 13.4d,

                /*DefaultBackground = Color.FromRgb(207, 206, 160),
                DefaultBorderBrush = Colors.Black,
                DefaultForeground = Colors.Black,

                SelectBackground = Color.FromRgb(240, 240, 233),
                SelectBorderBrush = Color.FromRgb(66, 66, 42),
                SelectForeground = Color.FromRgb(59, 69, 62),

                ClickedBackground = Color.FromRgb(131, 168, 171),
                ClickedBorderBrush = Color.FromRgb(16, 74, 31),
                ClickedForeground = Colors.Black*/
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
        private void SetInformationCommand(ICommandAAC Command)
        {
            int CountAbsolutly = Command.Parameters?.Count((i) => i.Absolutly) ?? 0;
            string TextRegistration = string.Join(", ", Command.Parameters?.Select(i => i.Name) ?? []);
            TextBlockNameCommand.Text = $"0 команда: \"{Command.Name}\"";
            TextBlockMainDescriptionCommand.Text = Command.Description;
            TextBlockDescriptionCountParameter.Text = CountAbsolutly == 0 ?
            $"Команда \"{Command.Name}\" не использует параметров" : $"Команда \"{Command.Name}\" включает в себя {CountAbsolutly} и больше параметров";
            TextBlockTextCommand.Text = Command.Name.Trim() + (CountAbsolutly == 0 ? string.Empty : "* " + TextRegistration);
        }
    }
}
