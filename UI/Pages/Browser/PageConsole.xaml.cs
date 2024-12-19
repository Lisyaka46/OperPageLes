using AAC20.CORE;
using AAC20.Windows.Frames;
using AAC20.Windows.Pages.ActionPanel;
using AAC20.Windows.Pages.Browser;
using IEL;
using IEL.Interfaces.Core;
using Interpreter.Classes;
using Interpreter.Commands;
using Interpreter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace AAC20.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageConsole.xaml
    /// </summary>
    public partial class PageConsole : Page, IPageDefault
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageConsole);

        /// <summary>
        /// Главная страница панели действий в консоли
        /// </summary>
        private readonly PageMainConsolePanelAction PageConsolePA = new();

        /// <summary>
        /// Страница буфера в панели действий
        /// </summary>
        internal readonly PageBufferActionPanel PageBufferPA = new(App.HeightButtonBuffer);

        /// <summary>
        /// Настройки панели действий в консоли
        /// </summary>
        private readonly PanelActionSettingsFrameworkElement PASettingsConsole;

        /// <summary>
        /// Панель действий доступная в программе
        /// </summary>
        private static IELPanelAction IELActionPanelMain => App.MainWindowApplication.IELActionPanelMain;

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления double значением
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimate = new(0, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления Color значением
        /// </summary>
        private static readonly ColorAnimation ColorAnimate = new(Colors.Black, TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Константа высоты элемента подсказки к командам
        /// </summary>
        private const int HeightHintElement = 20;

        public PageConsole()
        {
            InitializeComponent();
            BorderHintCommand.Height = 0d;
            GridHintCommandParameter.Opacity = 0d;
            Canvas.SetZIndex(GridHintCommandParameter, -1);
            RichTextBoxMainMessage.Document = new();
            PASettingsConsole = new(RichTextBoxMainMessage, PageConsolePA, new(270d, 230d));

            ButtonReturnCommand.OnActivateMouseLeft += () => App.CurrentApp.ActivateActionCommand(this, TextBoxCommandInput.Text, true);

            #region PageConsolePA
            PageConsolePA.IELButtonCrearConsole.OnActivateMouseLeft += (AltMode) =>
            {
                RichTextBoxMainMessage.Document = new();
                IELActionPanelMain.ClosePanelAction();
            };
            PageConsolePA.IELButtonCrearConsole.OnActivateMouseRight += (AltMode) => RichTextBoxMainMessage.Document = new();

            PageConsolePA.IELButtonCommandBuffer.OnActivateMouseLeft += (AltMode) =>
            {
                IELActionPanelMain.NextPage(PageBufferPA);
            };

            PageConsolePA.IELButtonDiscriptionCommand.OnActivateMouseLeft += (AltMode) =>
            {
                IELActionPanelMain.ClosePanelAction();
                App.UsingDiscriptionCommand();
            };
            #endregion
            #region PageBufferPA
            PageBufferPA.IELButtonBackMainMenu.OnActivateMouseLeft += (AltMode) =>
            {
                IELActionPanelMain.NextPage(PageConsolePA, false);
            };
            #endregion
            #region RichTextBoxMainMessage
            RichTextBoxMainMessage.MouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && IELActionPanelMain.PanelActionActivate)
                    IELActionPanelMain.ClosePanelAction();
                else if (e.ChangedButton == MouseButton.Right) IELActionPanelMain.UsingPanelAction(PASettingsConsole);
            };

            RichTextBoxMainMessage.TextChanged += (sender, e) =>
            {
                RichTextBoxMainMessage.ScrollToEnd();
            };
            #endregion
            #region TextBoxCommandInput
            TextBoxCommandInput.PreviewKeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Back:
                        if (TextBoxCommandInput.Text.Length > 0)
                        {
                            if (TextBoxCommandInput.Text[^1] == '*')
                            {
                                UsingAnimateBorderHintCommand(false);
                                UsingAnimateBorderCollectionHintCommand(true);
                            }
                        }
                        return;
                }
            };
            TextBoxCommandInput.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(160, 245, 200), TimeSpan.FromMilliseconds(90d)));
                        UsingAnimateBorderCollectionHintCommand(false);
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(255, 122, 84), TimeSpan.FromMilliseconds(90d)));
                        UsingAnimateBorderCollectionHintCommand(false);
                        break;
                }
            };
            TextBoxCommandInput.TextChanged += (sender, e) =>
            {
                if (TextBoxCommandInput.Text.Length > 0)
                {
                    if (TextBoxCommandInput.Text[^1] == '*')
                    {
                        if (GridHint.Children.Count > 0)
                        {
                            string CommandText = ICommandAAC.ReadNameCommand(TextBoxCommandInput.Text[..^1]);
                            if (((TextBlock)GridHint.Children[0]).Text.Equals(CommandText))
                            {
                                UsingAnimateBorderHintCommand(true);
                                return;
                            }
                        }
                        UsingAnimateBorderCollectionHintCommand(false);
                        return;
                    }
                }
            };
            TextBoxCommandInput.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        App.CurrentApp.ActivateActionCommand(this, TextBoxCommandInput.Text, true);
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Text = string.Empty;
                        break;
                    case Key.Apps:
                        IELActionPanelMain.UsingPanelAction(PASettingsConsole);
                        break;
                }
                TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(120, 204, 160), TimeSpan.FromMilliseconds(430d)));

                DoubleAnimation animation = DoubleAnimate.Clone();
                animation.Duration = TimeSpan.FromMilliseconds(300d);
                if (!TextBoxCommandInput.Text.Contains('*')) UsingAnimateBorderCollectionHintCommand(TextBoxCommandInput.Text.Length > 0);
            };
            #endregion
            TextBoxCommandInput.Focus();
        }

        #region HintCommandManipulate
        /// <summary>
        /// Манипулировать анимацией борьера подсказок к командам через всю коллекцию
        /// </summary>
        /// <param name="Activate">Активировать или дизактивировать аинмацией</param>
        private void UsingAnimateBorderCollectionHintCommand(bool Activate)
        {
            DoubleAnimation animation = DoubleAnimate.Clone();
            animation.Duration = TimeSpan.FromMilliseconds(300d);
            if (Activate)
            {
                string CommandText = ICommandAAC.ReadNameCommand(TextBoxCommandInput.Text);
                string[] AllHintNames = [.. App.CurrentApp.AllNamesCommand.Where((i) => { return i.Contains(CommandText, StringComparison.CurrentCultureIgnoreCase); })];
                if (AllHintNames.Length == GridHint.Children.Count) return;
                GridHint.Children.Clear();
                if (AllHintNames.Length > 0)
                {
                    Sorting.SortNames(ref AllHintNames);
                    foreach (string Name in AllHintNames)
                    {
                        TextBlock block = CreateHintBlock(Name, GridHint.Children.Count);
                        GridHint.Children.Add(block);
                    }
                }
            }
            else GridHint.Children.Clear();
            if (Canvas.GetZIndex(GridHintCommandParameter) == 1 && !Activate) UsingAnimateBorderHintCommand(false);
            animation.To = Activate && GridHint.Children.Count > 0 ?
                GridHint.Children.Count * HeightHintElement + BorderHintCommand.Padding.Top + BorderHintCommand.Padding.Bottom : 0d;
            BorderHintCommand.BeginAnimation(HeightProperty, animation);
        }

        /// <summary>
        /// Манипулировать анимацией борьера подсказок к командам
        /// </summary>
        /// <param name="Activate">Активировать или дизактивировать аинмацией</param>
        /// <param name="CommandTextActualHint">Константный текст поиска команды</param>
        private void UsingAnimateBorderHintCommand(bool Activate, string? CommandTextActualHint = null)
        {
            DoubleAnimation animation = DoubleAnimate.Clone();
            animation.Duration = TimeSpan.FromMilliseconds(300d);
            if (Activate)
            {
                ICommandAAC? CommandHint = null;
                string TextCommand = CommandTextActualHint ?? ((TextBlock)GridHint.Children[0]).Text;
                CommandHint ??= ICommandAAC.ReadCommand([.. App.DataConsoleCommand], TextCommand);
                CommandHint ??= ICommandAAC.ReadCommand([.. App.CurrentApp.DataAliases], TextCommand);
                if (CommandHint == null) return;
                Parameter[] Parameters = CommandHint.Parameters ?? [];
                TextBlockHintCommand.Text = $"{CommandHint.Name}* ";
                for (int i = 0; i < Parameters.Length; i++)
                {
                    TextBlockHintCommand.Text += $"{Parameters[i].Name}" +
                        $"{(Parameters[i].Absolutly ? string.Empty : '?')}" +
                        $"{(i < Parameters.Length - 1 ? ", " : string.Empty)}";
                }
                animation.To = HeightHintElement +
                    BorderHintCommand.Padding.Top + BorderHintCommand.Padding.Bottom +
                    GridHintCommandParameter.Margin.Top + GridHintCommandParameter.Margin.Bottom;
                BorderHintCommand.BeginAnimation(HeightProperty, animation);
            }
            animation.To = Activate ? 1d : 0d;
            Canvas.SetZIndex(GridHintCommandParameter, Activate ? 1 : -1);
            GridHintCommandParameter.BeginAnimation(OpacityProperty, animation);

            animation.To = Activate ? 0d : 1d;
            GridHint.BeginAnimation(OpacityProperty, animation);

            animation.To = Activate ? 300d : 142d;
            BorderHintCommand.BeginAnimation(WidthProperty, animation);
        }

        /// <summary>
        /// Создать объект подсказки к команде
        /// </summary>
        /// <param name="Name">Имя команды</param>
        /// <param name="Index">Индекс местоположения по оси Y</param>
        /// <returns>Объект подсказки к команде</returns>
        private TextBlock CreateHintBlock(string Name, int Index)
        {
            ColorAnimation color_animation = ColorAnimate.Clone();
            color_animation.Duration = TimeSpan.FromMilliseconds(120d);
            TextBlock Result = new()
            {
                Height = HeightHintElement,
                Text = Name,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new(0, HeightHintElement * Index, 0, 0),
                Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                Cursor = Cursors.Hand,
            };
            Result.MouseEnter += (sender, e) =>
            {
                color_animation.To = Color.FromRgb(255, 255, 255);
                Result.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, color_animation);
            };
            Result.MouseLeave += (sender, e) =>
            {
                color_animation.To = Color.FromRgb(0, 0, 0);
                Result.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, color_animation);
            };
            Result.MouseLeftButtonUp += (sender, e) =>
            {
                TextBoxCommandInput.Text = $"{Result.Text}*";
                UsingAnimateBorderHintCommand(true, Result.Text);
            };
            return Result;
        }
        #endregion

        #region ManipulateText
        /// <summary>
        /// Добавить и отформатировать текст в консоль
        /// </summary>
        /// <param name="Text">Текст добавляемый в консоль</param>
        /// <param name="Formatted">Форматировать или нет</param>
        internal void AddTextInConsole(string Text, bool Formatted = true)
        {
            if (Text.Length == 0) return;
            Text = $"{App.ConsolePreMessage} {Text}";
            Paragraph Message = new();
            Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (Style)Application.Current.Resources["RussianRail G Pro"]
            };
            BindingOperations.SetBinding(Message, Paragraph.StyleProperty, binding);
            if (Formatted)
            {
                List<Inline> Inlines = [];
                foreach (Match match in RegexFormattedText().Matches(Text))
                {
                    Inlines.Add(FormattedTextDetect(match.Value));
                }
                Message.Inlines.AddRange(Inlines);
            }
            else Message.Inlines.Add(new Run(Text));
            RichTextBoxMainMessage.Document.Blocks.Add(Message);
        }

        /// <summary>
        /// Очистить текст консоли
        /// </summary>
        internal void ClearConsoleText() => RichTextBoxMainMessage.Document = new();

        /// <summary>
        /// Изменить формативность текста с учётом первых знаков
        /// </summary>
        /// <remarks>
        /// <code>
        /// %#FFFFFF** <b>Italic</b> **
        /// </code>
        /// ** <b>Bold</b> **
        /// <code></code>
        /// // <i>Italic</i> //
        /// <code></code>
        /// __ <u>UnderLine</u> __
        /// <code></code>
        /// </remarks>
        /// <param name="Text">Текст форматирования</param>
        /// <returns>Форматированный текст</returns>
        private static Inline FormattedTextDetect(string Text)
        {
            if (Text.Length == 0 || (Text.Length == 1 && Text[0] == '%')) return new Run(Text);
            if (Text[0] == '%') Text = Text[1..]; // удаление "%"
            SolidColorBrush? color = null;
            if (Text[0] == '#')
            {
                color = new((Color)ColorConverter.ConvertFromString(
                    RegexFormattedTextColor().Match(Text).Value));
                Text = Text[7..];
            }
            Inline Result = $"{Text[0]}{Text[^1]}" switch
            {
                "**" => new Bold(new Run(Text[2..^2])),
                "//" => new Italic(new Run(Text[2..^2])),
                "__" => new Underline(new Run(Text[2..^2])),
                _ => new Run(Text),
            };
            if (color != null) Result.Background = color;
            return Result;
        }
        #endregion

        #region Regex
        /// <summary>
        /// Функция регулярного выражения выделения текста в ковычках "текст"
        /// </summary>
        private static Regex StringCommandError(char symbol) => new($"([^\\{symbol}]+|\\{symbol}[^\\{symbol}]+\\{symbol}?)");

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        // Текст который является %#00FF00FF__%**регистрационным**__ и %#FFFFFF**может** %~~даже так~~ %--постоянно-- %__форматироваться__
        [GeneratedRegex(@"([^%]+|(\%(#[0-9A-F]{6})?)(\*{2}[^\*]+\*{2}|_{2}[^_]+_{2}|\/{2}[^\/]+\/{2})|\%)")]
        private static partial Regex RegexFormattedText();

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        // %   #FFFFFF   //%**d**//
        [GeneratedRegex(@"#[0-9A-F]{6}")]
        private static partial Regex RegexFormattedTextColor();
        #endregion
    }
}
