using IEL;
using IEL.Classes;
using IEL.Interfaces.Core;
using Interpreter.Classes;
using Interpreter.Interfaces;
using OperPage_les.CORE;
using OperPage_les.Windows.Frames;
using OperPage_les.Windows.Pages.ActionPanel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;

namespace OperPage_les.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageConsole.xaml
    /// </summary>
    public partial class PageConsole : Page
    {
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
        private double HeightHintElement => TextBlockHintCommand.RenderSize.Height +
            BorderHintCommand.BorderThickness.Top + BorderHintCommand.BorderThickness.Bottom + 6d;

        public PageConsole()
        {
            InitializeComponent();
            BorderHintCommand.Height = 0d;
            GridHintOneCommand.Opacity = 0d;
            Canvas.SetZIndex(GridHintOneCommand, -1);
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
                GridHint.RowDefinitions.Clear();
                if (AllHintNames.Length > 0)
                {
                    Sorting.SortNames(ref AllHintNames);
                    foreach (string Name in AllHintNames)
                    {
                        TextBlock block = CreateHintBlock(Name);
                        GridHint.RowDefinitions.Add(new() { Height = new(HeightHintElement), });
                        Grid.SetRow(block, GridHint.RowDefinitions.Count - 1);
                        GridHint.Children.Add(block);
                    }
                }
            }
            else GridHint.Children.Clear();
            if (Canvas.GetZIndex(GridHintOneCommand) == 1 && !Activate) UsingAnimateBorderHintCommand(false);
            animation.To = Activate && GridHint.Children.Count > 0 ?
                GridHint.Children.Count * HeightHintElement + BorderHintCommand.Padding.Top + BorderHintCommand.Padding.Bottom : 0d;
            BorderHintCommand.BeginAnimation(HeightProperty, animation);
        }

        /// <summary>
        /// Манипулировать анимацией борьера подсказок к конкретной команде
        /// </summary>
        /// <param name="Activate">Активировать или дизактивировать анимацией</param>
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
                TextBlockHintCommand.UpdateLayout();
                animation.To = HeightHintElement +
                    BorderHintCommand.Padding.Top + BorderHintCommand.Padding.Bottom +
                    GridHintOneCommand.Margin.Top + GridHintOneCommand.Margin.Bottom;
                BorderHintCommand.BeginAnimation(HeightProperty, animation);
            }
            animation.To = Activate ? 1d : 0d;
            Canvas.SetZIndex(GridHintOneCommand, Activate ? 1 : -1);
            GridHintOneCommand.BeginAnimation(OpacityProperty, animation);

            animation.To = Activate ? 0d : 1d;
            GridHint.BeginAnimation(OpacityProperty, animation);

            double OffsetWidth = BorderHintCommand.ActualWidth - GridHintOneCommand.ActualWidth;
            OffsetWidth += TextBlockHintCommand.Padding.Right;
            animation.To = Activate ? TextBlockHintCommand.RenderSize.Width + OffsetWidth : 142d;
            BorderHintCommand.BeginAnimation(WidthProperty, animation);
        }

        /// <summary>
        /// Создать объект подсказки к команде
        /// </summary>
        /// <param name="Name">Имя команды</param>
        /// <returns>Объект подсказки к команде</returns>
        private TextBlock CreateHintBlock(string Name)
        {
            TextBlock Result = new()
            {
                Text = Name,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.None,
                TextWrapping = TextWrapping.NoWrap,
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new(0),
                Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                Cursor = System.Windows.Input.Cursors.Hand,
                FontSize = 14d,
            };
            Result.MouseEnter += (sender, e) =>
            {
                ColorAnimate.Duration = TimeSpan.FromMilliseconds(120d);
                ColorAnimate.To = Color.FromRgb(255, 255, 255);
                Result.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
            };
            Result.MouseLeave += (sender, e) =>
            {
                ColorAnimate.Duration = TimeSpan.FromMilliseconds(120d);
                ColorAnimate.To = Color.FromRgb(0, 0, 0);
                Result.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
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
            Paragraph Message;
            if (Formatted) FormattedAllTextDetect(out Message, Text);
            else Message = new(new Run(Text));
            System.Windows.Data.Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (Style)System.Windows.Application.Current.Resources["RussianRail G Pro"]
            };
            BindingOperations.SetBinding(Message, Paragraph.StyleProperty, binding);
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
        private static void FormattedAllTextDetect(out Paragraph Result, string Text)
        {
            // %//Italic %**Bold**//
            Result = new();
            foreach (Match match in RegexFormattedText().Matches(Text))
            {
                Result.Inlines.AddRange(FormattedBlockText(match.Value));
            }
        }

        private static Inline[] FormattedBlockText(string Text)
        {
            Span Result = new();
            if (Text.Length < 2 || Text[0] != '%')
            {
                Result.Inlines.Add(Text);
                return [.. Result.Inlines];
            }

            Text = Text[1..]; // удаление "%"

            // логика цвета
            SolidColorBrush? BackgroundColor = null;
            if (Text[0] == '#')
            {
                BackgroundColor = new((Color)System.Windows.Media.ColorConverter.ConvertFromString(
                    RegexFormattedTextColor().Match(Text).Value));
                Text = Text[7..];
            }

            MatchCollection CollectionRecurce = RegexFormattedText().Matches(Text[2..^2]);
            foreach (Match match in CollectionRecurce)
            {
                if (match.Value[0] == '%' && match.Value.Length > 1)
                {
                    foreach (Inline Element in FormattedBlockText(match.Value))
                    {
                        Result.Inlines.Add(SwitchBlockText([Text[0], Text[1]], Element));
                        Result.Inlines.LastInline.Background = BackgroundColor;
                    }
                    continue;
                }
                Result.Inlines.Add(SwitchBlockText([Text[0], Text[1]], new Run(match.Value)));
                Result.Inlines.LastInline.Background = BackgroundColor;
            }
            return [.. Result.Inlines];
        }

        private static Inline SwitchBlockText(char[] Parrent, Inline Context)
        {
            Contract.Requires(Parrent.Length == 2);
            return string.Concat(Parrent) switch
            {
                "**" => new Bold(Context),
                "//" => new Italic(Context),
                "__" => new Underline(Context),
                _ => Context,
            };
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
        [GeneratedRegex(@"([^%]+|(\%(#[0-9A-F]{6})?)(\*{2}([^\*]+(\*{3,}|\*)){1,}\*|_{2}([^_]+(_{3,}|_)){1,}_|\/{2}([^\/]+(\/{3,}|\/)){1,}\/)|\%)")]
        private static partial Regex RegexFormattedText();

        /// <summary>
        /// Регулярное выражение сортировки параметров от специальных символов
        /// </summary>
        /// <returns>Регулярное выражение</returns>
        // %   #FFFFFF   //**d**//
        [GeneratedRegex(@"#[0-9A-F]{6}")]
        private static partial Regex RegexFormattedTextColor();
        #endregion
    }
}
