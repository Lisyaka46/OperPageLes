using AAC20.Classes.Flaging;
using Interpreter.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using IEL;

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
        private static readonly DoubleAnimation DoubleAnimateOpacity = new(0, TimeSpan.FromMilliseconds(250d))
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

        public WindowDiscriptionCommands()
        {
            InitializeComponent();
            IELInputSearchCommand.Text = string.Empty;
            GridMainDescription.Opacity = 0d;
            BorderContextMenuParameter.Opacity = 0d;
            TextBlockTextCommand.Foreground = new SolidColorBrush(Colors.Black);
            int i = 0;
            foreach (ConsoleCommand commandAAC in App.DataConsoleCommand)
            {
                IELButtonText Button = GenerateCommandButton();
                Button.Margin = new(3, 32 * i++ + 4, 3, 0);
                Button.Text = commandAAC.Name;
                Button.OnActivateMouseLeft += () => DetectNewDiscriptionCommand(commandAAC);
                GridElements.Children.Add(Button);
            }
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
                    DoubleAnimateOpacity.To = 1d;
                    BorderContextMenuParameter.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
                }
            };
            IELButtonInfoParameter.MouseLeave += (sender, e) =>
            {
                if (Flags.ContextMenuParameter)
                {
                    Flags.ContextMenuParameter.Value = false;
                    Canvas.SetZIndex(BorderContextMenuParameter, -1);
                    DoubleAnimateOpacity.To = 0d;
                    BorderContextMenuParameter.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
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
                    Button.DefaultBackground = AnimationColor.To ?? default;
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
                    if (Button.DefaultBackground == AnimationColor.To) continue;
                    Button.DefaultBackground = AnimationColor.To ?? default;
                    //Button.BorderButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, AnimationColor);
                }
            };
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
                DoubleAnimateOpacity.To = 1d;
                GridMainDescription.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
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
                DoubleAnimateOpacity.To = 1d;
                GridMainDescription.BeginAnimation(OpacityProperty, DoubleAnimateOpacity);
            };
            GridMainDescription.BeginAnimation(OpacityProperty, AnimateOpacity);
        }

        /// <summary>
        /// Сгенерировать кнопку команнды описания
        /// </summary>
        /// <returns>Кнопка команды</returns>
        private static IELButtonText GenerateCommandButton()
        {
            return new IELButtonText()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 30,
                VisibleMouseImaging = false,

                DefaultBackground = Color.FromRgb(207, 206, 160),
                DefaultBorderBrush = Colors.Black,
                DefaultForeground = Colors.Black,

                SelectBackground = Color.FromRgb(240, 240, 233),
                SelectBorderBrush = Color.FromRgb(66, 66, 42),
                SelectForeground = Color.FromRgb(59, 69, 62),

                ClickedBackground = Color.FromRgb(131, 168, 171),
                ClickedBorderBrush = Color.FromRgb(16, 74, 31),
                ClickedForeground = Colors.Black
            };
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
