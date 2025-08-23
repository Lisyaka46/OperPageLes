using IEL.CORE.Classes;
using Interpreter.Interfaces;
using InterpreterCommand.Classes;
using OperPage_les.CORE;
using OperPage_les.CORE.Enums;
using OperPage_les.Windows.Frames;
using OperPage_les.UI.Pages.ActionPanel;
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
        #region PanelActionConsole
        #region Source
        private static readonly PageMainConsolePanelAction ConsolePage = new();
        /// <summary>
        /// Страница буфера в панели действий
        /// </summary>
        internal static readonly PageBufferPanelAction BufferPage = new();
        #endregion

        /// <summary>
        /// Настройки панели действий для страниц во вкладке консоли
        /// </summary>
        private readonly PanelActionSettingVisual PanelActionSettingsConsole;

        /// <summary>
        /// Главная страница панели действий во вкладке консоли
        /// </summary>
        private readonly PagePanelAction PanelActionConsolePage = new(ConsolePage);

        /// <summary>
        /// Страница буфера панели действий во вкладке консоли
        /// </summary>
        private readonly PagePanelAction PanelActionBufferPage = new(BufferPage);
        #endregion

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

        #region Hit
        /// <summary>
        /// Состояние видимости подсказок
        /// </summary>
        private ConsoleHitStateEnum StateVisibleHit;

        /// <summary>
        /// Подключение подсказок к командам
        /// </summary>
        private static bool HitUse => App.CurrentApp.SettingMainApplication.HitUse;

        /// <summary>
        /// Сохранённое состояние видимости подсказок
        /// </summary>
        private ConsoleHitStateEnum SaveStateHit;
        #endregion

        /// <summary>
        /// Активный индекс команды в буфере для строки ввода
        /// </summary>
        private int ActiveIndexBufferInput;

        /// <summary>
        /// Сохранённая строка для обозначения введённого текста перед перечислением элементов (Вверх/Вниз)
        /// </summary>
        private string SaveStringPrintBuffer;

        public PageConsole()
        {
            InitializeComponent();
            StateVisibleHit = ConsoleHitStateEnum.Hidden;
            ActiveIndexBufferInput = -1;
            SaveStringPrintBuffer = string.Empty;
            BorderHintCommand.Height = 0d;
            GridHintOneCommand.Opacity = 0d;
            Canvas.SetZIndex(GridHintOneCommand, -1);
            RichTextBoxMainMessage.Document = new();
            ButtonReturnCommand.OnActivateMouseLeft += (sender, e, Key) => App.CurrentApp.ActivateActionCommand(this, TextBoxCommandInput.Text, true);
            #region Setting
            App.CurrentApp.SettingMainApplication.HitUse.Changed += (Old, New) =>
            {
                if (!New && StateVisibleHit != ConsoleHitStateEnum.Hidden)
                {
                    SaveStateHit = StateVisibleHit;
                    HideHitCommand();
                }
                else if (New && SaveStateHit != ConsoleHitStateEnum.Hidden)
                {
                    if (SaveStateHit == ConsoleHitStateEnum.VisibleMainCommands) UsingAllHintCommand();
                    else if (SaveStateHit == ConsoleHitStateEnum.VisibleOneCommand) UsingOneHitCommand(TextBoxCommandInput.Text);
                }
            };
            #endregion
            #region PanelAction
            #region ConsolePage
            ConsolePage.IELButtonCrearConsole.OnActivateMouseLeft += (sender, e, Key) =>
            {
                RichTextBoxMainMessage.Document = new();
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
            };
            ConsolePage.IELButtonCrearConsole.OnActivateMouseRight += (sender, e, Key) => RichTextBoxMainMessage.Document = new();

            ConsolePage.IELButtonCommandBuffer.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindowApplication.IELActionPanelMain.NextPage(PanelActionBufferPage);
            };

            ConsolePage.IELButtonDiscriptionCommand.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
                App.CurrentApp.UsingDiscriptionCommand();
            };
            #endregion
            #region BufferPage
            BufferPage.IELButtonBackMainMenu.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindowApplication.IELActionPanelMain.NextPage(PanelActionConsolePage, false);
            };
            #endregion
            PanelActionConsolePage.IsKeyboardModeChanged += (Source, NewValue) =>
            {
                ConsolePage.IELButtonCrearConsole.CharKeyboardActivate = NewValue;
                ConsolePage.IELButtonCommandBuffer.CharKeyboardActivate = NewValue;
                ConsolePage.IELButtonDiscriptionCommand.CharKeyboardActivate = NewValue;
            };
            PanelActionBufferPage.IsKeyboardModeChanged += (Source, NewValue) =>
            {
                BufferPage.IELButtonBackMainMenu.CharKeyboardActivate = NewValue;
                BufferPage.IELButtonClearBuffer.CharKeyboardActivate = NewValue;
            };
            App.MainWindowApplication.IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                if (Name == nameof(RichTextBoxMainMessage)) TextBoxCommandInput.Focus();
            };
            PanelActionSettingsConsole = new(RichTextBoxMainMessage, PanelActionConsolePage, new(305d, 240d));
            #endregion
            #region RichTextBoxMainMessage
            RichTextBoxMainMessage.MouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && App.MainWindowApplication.IELActionPanelMain.PanelActionActivate)
                    App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
                else if (e.ChangedButton == MouseButton.Right) App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(PanelActionSettingsConsole);
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
                        if (!HitUse) return;
                        if (TextBoxCommandInput.Text.Length > 1)
                        {
                            if (TextBoxCommandInput.Text[^1] == '*'
                            && TextBoxCommandInput.Text.AsSpan().Count('*') == 1
                            && StateVisibleHit == ConsoleHitStateEnum.VisibleOneCommand)
                            {
                                UsingAllHintCommand();
                            }
                        }
                        else HideHitCommand();
                        return;
                }
            };
            TextBoxCommandInput.KeyDown += (sender, e) =>
            {
                if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Enter && e.Key != Key.Escape)
                {
                    ActiveIndexBufferInput = -1;
                    SaveStringPrintBuffer = string.Empty;
                }
                switch (e.Key)
                {
                    case Key.Enter:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(160, 245, 200), TimeSpan.FromMilliseconds(90d)));
                        if (HitUse) HideHitCommand();
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(Color.FromRgb(255, 122, 84), TimeSpan.FromMilliseconds(90d)));
                        if (HitUse) HideHitCommand();
                        break;
                }
            };
            TextBoxCommandInput.TextChanged += (sender, e) =>
            {
                if (!HitUse) return;
                if (TextBoxCommandInput.Text.Length > 0 && TextBoxCommandInput.Text.Contains('*') && StateVisibleHit != ConsoleHitStateEnum.VisibleOneCommand)
                {
                    UsingOneHitCommand(TextBoxCommandInput.Text);
                    return;
                }
                else if (TextBoxCommandInput.Text.Length == 0 && StateVisibleHit != ConsoleHitStateEnum.Hidden) HideHitCommand();
            };
            TextBoxCommandInput.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        ActiveIndexBufferInput = -1;
                        SaveStringPrintBuffer = string.Empty;
                        App.CurrentApp.ActivateActionCommand(this, TextBoxCommandInput.Text, true);
                        if (HitUse) HideHitCommand();
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Text = SaveStringPrintBuffer.Length > 0 ? SaveStringPrintBuffer : string.Empty;
                        SaveStringPrintBuffer = string.Empty;
                        ActiveIndexBufferInput = -1;
                        break;
                    case Key.Apps:
                        App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(PanelActionSettingsConsole);
                        break;
                    case Key.Up:
                        if (BufferPage.BufferCommand.Count == 0) return;
                        if (ActiveIndexBufferInput == -1)
                        {
                            SaveStringPrintBuffer = TextBoxCommandInput.Text;
                            ActiveIndexBufferInput = BufferPage.BufferCommand.Count - 1;
                        }
                        else ActiveIndexBufferInput = ActiveIndexBufferInput > 0 ? ActiveIndexBufferInput - 1 : BufferPage.BufferCommand.Count - 1;
                        TextBoxCommandInput.Text = BufferPage.BufferCommand.BufferElements[ActiveIndexBufferInput];
                        break;
                    case Key.Down:
                        if (BufferPage.BufferCommand.Count == 0) return;
                        if (ActiveIndexBufferInput == -1)
                        {
                            SaveStringPrintBuffer = TextBoxCommandInput.Text;
                            ActiveIndexBufferInput = 0;
                        }
                        else ActiveIndexBufferInput = ActiveIndexBufferInput < BufferPage.BufferCommand.Count - 1 ? ActiveIndexBufferInput + 1 : 0;
                        TextBoxCommandInput.Text = BufferPage.BufferCommand.BufferElements[ActiveIndexBufferInput];
                        break;
                }
                TextBoxCommandInput.Background.BeginAnimation(SolidColorBrush.ColorProperty,
                            new ColorAnimation(TextBoxCommandInput.IELSettingObject.BackgroundSetting.Used, TimeSpan.FromMilliseconds(430d)));

                DoubleAnimation animation = DoubleAnimate.Clone();
                animation.Duration = TimeSpan.FromMilliseconds(300d);
                if (!TextBoxCommandInput.Text.Contains('*') && TextBoxCommandInput.Text.Length > 0 && HitUse) UsingAllHintCommand();
            };
            #endregion
            TextBoxCommandInput.Focus();
        }

        #region HintCommandManipulate
        /// <summary>
        /// Отобразить подсказки ко всем командам
        /// </summary>
        private void UsingAllHintCommand()
        {
            StateVisibleHit = ConsoleHitStateEnum.VisibleMainCommands;
            TimeSpan span = TimeSpan.FromMilliseconds(300d);

            string CommandText = COMInterpreter.ReadNameCommand(TextBoxCommandInput.Text);
            string[] AllHintNames =
                [.. App.CurrentApp.Interpreter.CommandWhere((i) => i.Name.Contains(CommandText, StringComparison.CurrentCultureIgnoreCase)).Select((i) => i.Name)];
            StackPanelAllHit.Children.Clear();
            double AnimateWidth = 0d, AnimateHeight = 0d;
            BorderHintCommand.Width = 0d;
            BorderHintCommand.Height = 0d;
            //GridHint.RowDefinitions.Clear();
            if (AllHintNames.Length > 0)
            {
                Sorting.SortNames(ref AllHintNames);
                foreach (string Name in AllHintNames)
                {
                    TextBlock block = CreateHintBlock(Name);
                    //ScrollViewerAllHit.Off
                    //GridHint.RowDefinitions.Add(new() { Height = new(HeightHintElement), });
                    //Grid.SetRow(block, GridHint.RowDefinitions.Count - 1);
                    StackPanelAllHit.Children.Add(block);
                    block.UpdateLayout();
                    block.Width = block.ActualWidth;
                    block.Height = block.ActualHeight;
                    if (block.ActualWidth > AnimateWidth) AnimateWidth = block.ActualWidth;
                    AnimateHeight += block.ActualHeight;
                }
                AnimateWidth += BorderHintCommand.Padding.Left + BorderHintCommand.Padding.Right + 8;
                AnimateHeight += BorderHintCommand.Padding.Top + BorderHintCommand.Padding.Bottom + 8;
                if (AnimateHeight > BorderHintCommand.MaxHeight) AnimateHeight = BorderHintCommand.MaxHeight;
            }
            else HideHitCommand();

            Canvas.SetZIndex(GridHintOneCommand, -1);
            App.AnimateDoubleEffect(GridHintOneCommand, OpacityProperty, 0d, span);

            Canvas.SetZIndex(StackPanelAllHit, 1);
            App.AnimateDoubleEffect(StackPanelAllHit, OpacityProperty, 1d, span);

            App.AnimateDoubleEffect(BorderHintCommand, WidthProperty, AnimateWidth, span);
            App.AnimateDoubleEffect(BorderHintCommand, HeightProperty, AnimateHeight, span);
            App.AnimateDoubleEffect(BorderHintCommand, OpacityProperty, 1d, span);
        }

        /// <summary>
        /// Отобразить подсказку к конкретной команде
        /// </summary>
        /// <param name="TextCommand">Константный текст поиска команды</param>
        private void UsingOneHitCommand(string TextCommand)
        {
            ICommandOPER? CommandHint;
            CommandHint = App.CurrentApp.Interpreter.ReadCommand(TextCommand);
            if (CommandHint == null) return;
            StateVisibleHit = ConsoleHitStateEnum.VisibleOneCommand;
            TimeSpan span = TimeSpan.FromMilliseconds(300d);
            string[] Parameters = [.. CommandHint.Parameters?.Select((i) => $"{i.Name}{(i.Absolutly ? string.Empty : "?")}") ?? []];
            TextBlockHintCommand.Text = $"{CommandHint.Name}* {string.Join(",", Parameters)}";
            TextBlockHintCommand.UpdateLayout();

            Canvas.SetZIndex(GridHintOneCommand, 1);
            App.AnimateDoubleEffect(GridHintOneCommand, OpacityProperty, 1d, span);

            Canvas.SetZIndex(StackPanelAllHit, -1);
            App.AnimateDoubleEffect(StackPanelAllHit, OpacityProperty, 0d, span);

            App.AnimateDoubleEffect(BorderHintCommand, WidthProperty, TextBlockHintCommand.ActualWidth, span);
            App.AnimateDoubleEffect(BorderHintCommand, HeightProperty, 35d, span);
            App.AnimateDoubleEffect(BorderHintCommand, OpacityProperty, 1d, span);
        }

        /// <summary>
        /// Скрыть отображение подсказок к командам
        /// </summary>
        private void HideHitCommand()
        {
            StateVisibleHit = ConsoleHitStateEnum.Hidden;
            TimeSpan span = TimeSpan.FromMilliseconds(300d);

            App.AnimateDoubleEffect(BorderHintCommand, WidthProperty, 0d, span);
            App.AnimateDoubleEffect(BorderHintCommand, HeightProperty, 0d, span);
            App.AnimateDoubleEffect(BorderHintCommand, OpacityProperty, 0d, span);
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
                TextAlignment = TextAlignment.Left,
                TextTrimming = TextTrimming.None,
                TextWrapping = TextWrapping.NoWrap,
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new(6, 2, 6, 2),
                Foreground = new SolidColorBrush(Color.FromRgb(11, 43, 68)),
                Cursor = System.Windows.Input.Cursors.Hand,
                FontSize = 16d,
                FontFamily = new System.Windows.Media.FontFamily("Cascadia Code"),
            };
            Result.MouseEnter += (sender, e) =>
            {
                ColorAnimate.Duration = TimeSpan.FromMilliseconds(120d);
                ColorAnimate.To = Color.FromRgb(168, 217, 255);
                Result.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
            };
            Result.MouseLeave += (sender, e) =>
            {
                ColorAnimate.Duration = TimeSpan.FromMilliseconds(120d);
                ColorAnimate.To = Color.FromRgb(11, 43, 68);
                Result.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimate);
            };
            Result.MouseLeftButtonUp += (sender, e) =>
            {
                TextBoxCommandInput.Text = $"{Result.Text}*";
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
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["RussianRail G Pro"]
            };
            BindingOperations.SetBinding(Message, Paragraph.FontFamilyProperty, binding);
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
