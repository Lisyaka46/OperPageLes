using IEL.CORE.Classes;
using Interpreter.Interfaces;
using InterpreterCommand.Classes;
using ApplicationOperPageLes.CORE;
using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.UI.Pages.ActionPanel.PageConsole;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using IEL.CORE.Enums;
using Color = System.Windows.Media.Color;

namespace ApplicationOperPageLes.UI.Pages.Browser
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
        /// Активный индекс команды в подсказках к командам для строки ввода
        /// </summary>
        private int ActiveIndexHitCommandInput;

        /// <summary>
        /// Сохранённая строка для обозначения введённого текста перед перечислением элементов (Вверх/Вниз)
        /// </summary>
        private string SaveStringPrintBuffer;

        /// <summary>
        /// Производилось ли вычисление события KeyDown
        /// </summary>
        private bool SaveKeyDown;

        /// <summary>
        /// Текущая навигация в текущей странице
        /// </summary>
        private SelectNavigationPageConsoleEnum SelectNavigation;

#if DEBUG
        TextBlock DEVTextBlockSelectNavigation;
#endif

        public PageConsole()
        {
            InitializeComponent();
#if DEBUG
            DEVTextBlockSelectNavigation = App.CurrentApp.Is_WindowDeveloper.BlockInlays[0].AddNewTextElement();
            DEVTextBlockSelectNavigation.Text = $"SN: {SelectNavigation}";
#endif
            SelectNavigation = SelectNavigationPageConsoleEnum.None;
            SaveKeyDown = false;
            StateVisibleHit = ConsoleHitStateEnum.Hidden;
            ActiveIndexBufferInput = -1;
            ActiveIndexHitCommandInput = -1;
            SaveStringPrintBuffer = string.Empty;
            BorderHintCommand.Height = 0d;
            GridHintOneCommand.Opacity = 0d;
            RectangleSelect.Width = 0d;
            Canvas.SetZIndex(GridHintOneCommand, -1);
            RichTextBoxMainMessage.Document = new();
            ButtonReturnCommand.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                if (HitUse) ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                await App.CurrentApp.ActivateActionCommand(this, TextBoxCommandInput.Text, true);
            };
            #region Setting
            App.CurrentApp.SettingMainApplication.HitUse.Changed += (Old, New) =>
            {
                if (!New && StateVisibleHit != ConsoleHitStateEnum.Hidden)
                {
                    SaveStateHit = StateVisibleHit;
                    ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
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
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
            };
            ConsolePage.IELButtonCrearConsole.OnActivateMouseRight += (sender, e, Key) => RichTextBoxMainMessage.Document = new();

            ConsolePage.IELButtonCommandBuffer.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindow.IELActionPanelMain.NextPageInObject(PanelActionBufferPage);
            };

            ConsolePage.IELButtonDiscriptionCommand.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
                //App.CurrentApp.UsingDiscriptionCommand();
            };
            #endregion
            #region BufferPage
            BufferPage.IELButtonBackMainMenu.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindow.IELActionPanelMain.NextPageInObject(PanelActionConsolePage, false);
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
            App.MainWindow.IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                if (Name == nameof(RichTextBoxMainMessage)) TextBoxCommandInput.Focus();
            };
            PanelActionSettingsConsole = new(RichTextBoxMainMessage, PanelActionConsolePage, new(305d, 260d));
            #endregion
            #region RichTextBoxMainMessage
            RichTextBoxMainMessage.MouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && App.MainWindow.IELActionPanelMain.PanelActionActivate)
                    App.MainWindow.IELActionPanelMain.ClosePanelAction();
                else if (e.ChangedButton == MouseButton.Right) App.MainWindow.IELActionPanelMain.UsingPanelAction(PanelActionSettingsConsole, OrientationPanelActionPosition.RightUp);
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
                        else ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                        return;
                }
            };
            TextBoxCommandInput.KeyDown += (sender, e) =>
            {
                if (!SaveKeyDown)
                {
                    SaveKeyDown = true;
                    if (SelectNavigation == SelectNavigationPageConsoleEnum.BufferCommandTextBox)
                        SelectNavigation = SelectNavigationPageConsoleEnum.None;
#if DEBUG
                    DEVTextBlockSelectNavigation.Text = $"SN: {SelectNavigation}";
#endif
                    if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Enter && e.Key != Key.Escape)
                    {
                        SaveStringPrintBuffer = string.Empty;
                    }
                    switch (e.Key)
                    {
                        case Key.Escape:
                        case Key.Enter:
                            //TextBoxCommandInput.Background..BeginAnimation(SolidColorBrush.ColorProperty,
                            //    new ColorAnimation(TextBoxCommandInput.IELSettingObject.BackgroundSetting.Used,
                            //    e.Key switch
                            //    {
                            //        Key.Enter => Color.FromRgb(160, 245, 200),
                            //        Key.Escape => Color.FromRgb(255, 122, 84),
                            //        _ => throw new NotImplementedException(),
                            //    }, TimeSpan.FromMilliseconds(80d)));
                            break;
                    }
                }
            };
            TextBoxCommandInput.KeyUp += async (sender, e) =>
            {
                SaveKeyDown = false;
                switch (e.Key)
                {
                    case Key.Enter:
                        SaveStringPrintBuffer = string.Empty;
                        TextBoxCommandInput.QBackground.SetActiveSpecrum(StateSpectrum.Used, true);
                        if (SelectNavigation == SelectNavigationPageConsoleEnum.HitCommands)
                        {
                            SelectNavigation = SelectNavigationPageConsoleEnum.None;
                            TextBoxCommandInput.Text += '*';
#if DEBUG
                            DEVTextBlockSelectNavigation.Text = $"SN: {SelectNavigation}";
#endif
                        }
                        else
                        {
                            if (StateVisibleHit != ConsoleHitStateEnum.Hidden) ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                            await App.CurrentApp.ActivateActionCommand(this, TextBoxCommandInput.Text, true);
                        }
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Text = SaveStringPrintBuffer.Length > 0 ? SaveStringPrintBuffer : string.Empty;
                        SaveStringPrintBuffer = string.Empty;
                        if (TextBoxCommandInput.Text.Length > 0)
                        {
                            SelectNavigation = SelectNavigationPageConsoleEnum.None;
                        }
                        else if (StateVisibleHit != ConsoleHitStateEnum.Hidden) ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                        TextBoxCommandInput.QBackground.SetActiveSpecrum(StateSpectrum.Used, true);
                        break;
                    case Key.Apps:
                        App.MainWindow.IELActionPanelMain.UsingPanelAction(PanelActionSettingsConsole, OrientationPanelActionPosition.RightUp);
                        break;
                    case Key.Down:
                    case Key.Up:
                        ProcessingActialSelectNavigating(e.Key);
                        break;
                    default:
                        if (SelectNavigation == SelectNavigationPageConsoleEnum.HitCommands)
                            SelectNavigation = SelectNavigationPageConsoleEnum.None;
                        break;
                }
                if (HitUse && SelectNavigation != SelectNavigationPageConsoleEnum.HitCommands)
                {
                    if (TextBoxCommandInput.Text.Length > 0 && TextBoxCommandInput.Text.Contains('*'))
                    {
                        UsingOneHitCommand(TextBoxCommandInput.Text);
                        return;
                    }
                    else if (TextBoxCommandInput.Text.Length == 0 && StateVisibleHit != ConsoleHitStateEnum.Hidden) ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                    else if (!TextBoxCommandInput.Text.Contains('*') && TextBoxCommandInput.Text.Length > 0)
                    {
                        UsingAllHintCommand();
                    }
                }
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
            TimeSpan span = TimeSpan.FromMilliseconds(300d);

            string CommandText = COMInterpreter.ReadNameCommand(TextBoxCommandInput.Text);
            string[] AllHintNames =
                [.. App.CurrentApp.Interpreter.CommandWhere((i) => i.Name.Contains(CommandText, StringComparison.CurrentCultureIgnoreCase)).Select((i) => i.Name)];
            if (AllHintNames.Length == 0)
            {
                ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                return;
            }
            StackPanelAllHit.Children.Clear();
            BorderHintCommand.Width = 0d;
            BorderHintCommand.Height = 0d;
            Sorting.SortNames(ref AllHintNames);
            foreach (string Name in AllHintNames)
            {
                TextBlock block = CreateHintBlock(Name);
                StackPanelAllHit.Children.Add(block);
                block.UpdateLayout();
            }
            ChangeVisualHintCommand(ConsoleHitStateEnum.VisibleMainCommands);
        }

        /// <summary>
        /// Изменить визуализацию подсказок к командам
        /// </summary>
        /// <param name="StateHit">Изменяемое состояние</param>
        private void ChangeVisualHintCommand(ConsoleHitStateEnum StateHit)
        {
            if (StateVisibleHit != StateHit)
            {
                if (StateHit == ConsoleHitStateEnum.Hidden) SelectNavigation = SelectNavigationPageConsoleEnum.None;
#if DEBUG
                DEVTextBlockSelectNavigation.Text = $"SN: {SelectNavigation}";
#endif
                TimeSpan span = TimeSpan.FromMilliseconds(300d);
                Canvas.SetZIndex(GridHintOneCommand, StateHit == ConsoleHitStateEnum.VisibleOneCommand ? 1 : -1);
                App.DoubleAnimationType.AnimateEffect(GridHintOneCommand, OpacityProperty, StateHit == ConsoleHitStateEnum.VisibleOneCommand ? 1d : 0d, span);

                Canvas.SetZIndex(StackPanelAllHit, StateHit == ConsoleHitStateEnum.VisibleOneCommand ? -1 : 1);
                App.DoubleAnimationType.AnimateEffect(StackPanelAllHit, OpacityProperty, StateHit == ConsoleHitStateEnum.VisibleOneCommand ? 0d : 1d, span);

                App.DoubleAnimationType.AnimateEffect(BorderHintCommand, OpacityProperty, StateHit == ConsoleHitStateEnum.Hidden ? 0d : 1d, span);
                StateVisibleHit = StateHit;
            }
            AnimateSizeHintPanel(0d, 0d, StateHit != ConsoleHitStateEnum.Hidden);
        }

        /// <summary>
        /// Расчитать размер по всем найденным элементам подсказок и выполнить анимацию
        /// </summary>
        /// <param name="AnimateWidth">Коэффициент горизонтального значения анимирования</param>
        /// <param name="AnimateHeight">Коэффициент вертикального значения анимирования</param>
        /// <param name="AutoChildren">Авто-расчёт коэффициентов по количеству дочерних элементов</param>
        private void AnimateSizeHintPanel(double AnimateWidth = 0d, double AnimateHeight = 0d, bool AutoChildren = true)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(300d);
            if (AutoChildren)
            {
                foreach (UIElement Element in StackPanelAllHit.Children)
                {
                    if (((TextBlock)Element).ActualWidth > AnimateWidth) AnimateWidth = ((TextBlock)Element).ActualWidth;
                    AnimateHeight += ((TextBlock)Element).ActualHeight;
                }
                AnimateWidth += BorderHintCommand.Padding.Left + BorderHintCommand.Padding.Right + 8;
                AnimateHeight += BorderHintCommand.Padding.Top + BorderHintCommand.Padding.Bottom + 8;
                if (AnimateHeight > BorderHintCommand.MaxHeight) AnimateHeight = BorderHintCommand.MaxHeight;
            }
            App.DoubleAnimationType.AnimateEffect(BorderHintCommand, WidthProperty, AnimateWidth, span);
            App.DoubleAnimationType.AnimateEffect(BorderHintCommand, HeightProperty, AnimateHeight, span);
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
            TimeSpan span = TimeSpan.FromMilliseconds(300d);
            string[] Parameters = [.. CommandHint.Parameters?.Select((i) => $"{i.Name}{(i.Absolutly ? string.Empty : "?")}") ?? []];
            TextBlockHintCommand.Text = $"{CommandHint.Name}* {string.Join(",", Parameters)}";
            TextBlockHintCommand.UpdateLayout();

            TextBlockDescriptionHintCommand.ClearValue(WidthProperty);
            TextBlockDescriptionHintCommand.Text = CommandHint.Description;
            TextBlockDescriptionHintCommand.UpdateLayout();
            TextBlockDescriptionHintCommand.Width = TextBlockHintCommand.ActualWidth < 100d ? 100d : TextBlockHintCommand.ActualWidth;
            TextBlockDescriptionHintCommand.UpdateLayout();

            ChangeVisualHintCommand(ConsoleHitStateEnum.VisibleOneCommand);

            App.DoubleAnimationType.AnimateEffect(BorderHintCommand, WidthProperty, TextBlockDescriptionHintCommand.Width + 10d, span);
            App.DoubleAnimationType.AnimateEffect(BorderHintCommand, HeightProperty,
                TextBlockDescriptionHintCommand.ActualHeight + TextBlockHintCommand.ActualHeight + 8d, span);

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
                App.ColorAnimationType.AnimateEffect(Result.Foreground, SolidColorBrush.ColorProperty,
                    Color.FromRgb(168, 217, 255), TimeSpan.FromMilliseconds(120d));
            };
            Result.MouseLeave += (sender, e) =>
            {
                App.ColorAnimationType.AnimateEffect(Result.Foreground, SolidColorBrush.ColorProperty,
                    Color.FromRgb(11, 43, 68), TimeSpan.FromMilliseconds(120d));
            };
            Result.MouseLeftButtonUp += (sender, e) =>
            {
                TextBoxCommandInput.Text = $"{Result.Text}*";
                UsingOneHitCommand(TextBoxCommandInput.Text);
            };
            return Result;
        }
        #endregion

        #region Navigation
        /// <summary>
        /// Обработать клавишу по текущей навигации страницы
        /// </summary>
        /// <param name="key">Обрабатываемая клавиша</param>
        private void ProcessingActialSelectNavigating(Key key)
        {
            if (SelectNavigation == SelectNavigationPageConsoleEnum.None)
            {
                if (StateVisibleHit == ConsoleHitStateEnum.VisibleMainCommands)
                {
                    SelectNavigation = SelectNavigationPageConsoleEnum.HitCommands;
                    ActiveIndexHitCommandInput = -1;
                }
                else
                {
                    SelectNavigation = SelectNavigationPageConsoleEnum.BufferCommandTextBox;
                    ActiveIndexBufferInput = -1;
                }
#if DEBUG
            DEVTextBlockSelectNavigation.Text = $"SN: {SelectNavigation}";
#endif
            }

            switch (SelectNavigation)
            {
                case SelectNavigationPageConsoleEnum.BufferCommandTextBox:
                    if (key == Key.Up)
                    {
                        if (BufferPage.BufferCommand.Count == 0) return;
                        if (ActiveIndexBufferInput == -1)
                        {
                            SaveStringPrintBuffer = TextBoxCommandInput.Text;
                            ActiveIndexBufferInput = BufferPage.BufferCommand.Count - 1;
                        }
                        else ActiveIndexBufferInput = ActiveIndexBufferInput > 0 ? ActiveIndexBufferInput - 1 : BufferPage.BufferCommand.Count - 1;
                        TextBoxCommandInput.Text = BufferPage.BufferCommand.BufferElements[ActiveIndexBufferInput];
                    }
                    else if (key == Key.Down)
                    {
                        if (BufferPage.BufferCommand.Count == 0) return;
                        if (ActiveIndexBufferInput == -1)
                        {
                            SaveStringPrintBuffer = TextBoxCommandInput.Text;
                            ActiveIndexBufferInput = 0;
                        }
                        else ActiveIndexBufferInput = ActiveIndexBufferInput < BufferPage.BufferCommand.Count - 1 ? ActiveIndexBufferInput + 1 : 0;
                        TextBoxCommandInput.Text = BufferPage.BufferCommand.BufferElements[ActiveIndexBufferInput];
                    }
                    break;
                case SelectNavigationPageConsoleEnum.HitCommands:
                    if (key == Key.Up)
                    {
                        if (StackPanelAllHit.Children.Count == 0) return;
                        if (ActiveIndexHitCommandInput == -1)
                        {
                            SaveStringPrintBuffer = TextBoxCommandInput.Text;
                            ActiveIndexHitCommandInput = StackPanelAllHit.Children.Count - 1;
                        }
                        else ActiveIndexHitCommandInput = ActiveIndexHitCommandInput > 0 ? ActiveIndexHitCommandInput - 1 : StackPanelAllHit.Children.Count - 1;
                        TextBoxCommandInput.Text = ((TextBlock)StackPanelAllHit.Children[ActiveIndexHitCommandInput]).Text;
                    }
                    else if (key == Key.Down)
                    {
                        if (StackPanelAllHit.Children.Count == 0) return;
                        if (ActiveIndexHitCommandInput == -1)
                        {
                            SaveStringPrintBuffer = TextBoxCommandInput.Text;
                            ActiveIndexHitCommandInput = 0;
                        }
                        else ActiveIndexHitCommandInput = ActiveIndexHitCommandInput < StackPanelAllHit.Children.Count - 1 ? ActiveIndexHitCommandInput + 1 : 0;
                        TextBoxCommandInput.Text = ((TextBlock)StackPanelAllHit.Children[ActiveIndexHitCommandInput]).Text;
                    }
                    break;
            }
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
