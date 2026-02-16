using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Interfaces;
using ApplicationOperPageLes.UI.Pages.ActionPanel.PageConsole;
using OIEL.UserElementsControl;
using IEL.CORE.Classes;
using IEL.CORE.Enums;
using Interpreter.Interfaces;
using InterpreterCommand.Classes;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
using OPRES = ApplicationOperPageLes.Properties.Resources;
using ApplicationOperPageLes.CORE.Struct;
using OIEL.CORE.Browser;

namespace ApplicationOperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageConsole.xaml
    /// </summary>
    public partial class PageConsole : PageBrowser
    {
        [LibraryImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetCursorPos(int X, int Y);

        #region PanelActionConsole
        internal static readonly MainPagePanelAction PageConsoleActionPanelMain = new();
        /// <summary>
        /// Страница буфера в панели действий
        /// </summary>
        internal BufferPagePanelAction? BufferPage;
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

        /// <summary>
        /// Состояние скрытия панели подсказок к командам
        /// </summary>
        private bool HidedHitPanel = false;

        /// <summary>
        /// Объект управляемых визуализаторов команд
        /// </summary>
        internal StackPanel StackPanelConsole { get; private set; }

        /// <summary>
        /// Сетка элементов прокрутки подсказок к командам
        /// </summary>
        private Grid GridHitScroll;

        /// <summary>
        /// Элемент отображающий выделение объекта подсказки
        /// </summary>
        internal System.Windows.Shapes.Rectangle RectangleSelect { get; private set; }

        /// <summary>
        /// Объект управляемых визуализаторов команд
        /// </summary>
        internal StackPanel StackPanelAllHit { get; private set; }

        public PageConsole()
        {
            InitializeComponent();
            GridHitScroll = new();
            RectangleSelect = new()
            {
                Height = 13d,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                Width = 0d,
                Margin = new(0),
                RadiusX = 5d,
                RadiusY = 5d,
                Fill = new SolidColorBrush(Color.FromArgb(255, 81, 177, 219))
            };
            StackPanelAllHit = new()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
            };
            GridHitScroll.Children.Add(RectangleSelect);
            IELHitScroll.AutoUpdateVisibleHorizontalScroll = false;
            IELHitScroll.AutoUpdateVisibleVerticalScroll = false;
            IELHitScroll.Content = StackPanelAllHit;

            StackPanelConsole = new()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
            };
            IELScrollConsole.ScrollForce = App.CurrentApp.SettingMainApplication.ConsoleScrollForce;
            IELScrollConsole.VerticalScrollAligment = VerticalScrollAlignment.Right;
            IELScrollConsole.AutoUpdateVisibleHorizontalScroll = false;
            IELScrollConsole.AutoUpdateVisibleVerticalScroll = true;
            IELScrollConsole.Content = StackPanelConsole;

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(ButtonReturnCommand);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Violet].ConnectPalleteFromIELElement(TextBoxCommandInput);
            SelectNavigation = SelectNavigationPageConsoleEnum.None;
            SaveKeyDown = false;
            StateVisibleHit = ConsoleHitStateEnum.Hidden;
            ActiveIndexBufferInput = -1;
            ActiveIndexHitCommandInput = -1;
            SaveStringPrintBuffer = string.Empty;
            BorderHintCommand.Height = 0d;
            GridHintOneCommand.Opacity = 0d;
            RectangleSelect.Opacity = 0d;

            Canvas.SetZIndex(GridHintOneCommand, -1);
            ButtonReturnCommand.OnActivateMouseLeft += async (sender, e) =>
            {
                if (HitUse) ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                TextBlockInformation.Text = "Команда отправлена на обработку и исполнение.";
                await ActivateCommand();
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

            App.MainWindow.IELActionPanelMain.EventClosingPanelAction += (Name) =>
            {
                if (Name == nameof(BorderConsole)) TextBoxCommandInput.Focus();
            };
            #endregion

            #region RichTextBoxMainMessage
            BorderHintCommand.MouseRightButtonUp += (sender, e) =>
            {
                if (HidedHitPanel)
                {
                    HidedHitPanel = false;
                    HeadHitPanelGrid.IsEnabled = true;
                    if (StateVisibleHit == ConsoleHitStateEnum.VisibleOneCommand) AnimateHitPanelFromOneCommand();
                    else ChangeVisualHintCommand(StateVisibleHit);
                }
                else
                {
                    App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderHintCommand, HeightProperty, 10d, TimeSpan.FromMilliseconds(400d));
                    HeadHitPanelGrid.IsEnabled = false;
                    HidedHitPanel = true;
                    Keyboard.ClearFocus();
                }
            };
            BorderConsole.MouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && App.MainWindow.IELActionPanelMain.PanelActionActivate)
                    App.MainWindow.IELActionPanelMain.ClosePanelAction();
                else if (e.ChangedButton == MouseButton.Right)
                {
                    UsingPanelActionFromConsolePage(null);
                }
            };
            #endregion

            #region BufferPage
            BufferPage = App.CurrentApp.AppPageBuffer;
            #endregion

            #region TextBoxCommandInput
            TextBoxCommandInput.MouseUp += (sender, e) =>
            {
                if (HidedHitPanel)
                {
                    HidedHitPanel = false;
                    HeadHitPanelGrid.IsEnabled = true;
                    if (StateVisibleHit == ConsoleHitStateEnum.VisibleOneCommand) AnimateHitPanelFromOneCommand();
                    else ChangeVisualHintCommand(StateVisibleHit);
                }
            };
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
                        SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                    if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Enter && e.Key != Key.Escape)
                    {
                        SaveStringPrintBuffer = string.Empty;
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
                        TextBoxCommandInput.SetActiveSpecrum(StateSpectrum.Used, true);
                        if (SelectNavigation == SelectNavigationPageConsoleEnum.HitCommands)
                        {
                            SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                            TextBoxCommandInput.Text += "* ";
                        }
                        else
                        {
                            if (StateVisibleHit != ConsoleHitStateEnum.Hidden) ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);

                            await ActivateCommand();
                        }
                        break;
                    case Key.Escape:
                        TextBoxCommandInput.Text = SaveStringPrintBuffer.Length > 0 ? SaveStringPrintBuffer : string.Empty;
                        SaveStringPrintBuffer = string.Empty;
                        if (TextBoxCommandInput.Text.Length > 0)
                        {
                            SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                        }
                        else if (StateVisibleHit != ConsoleHitStateEnum.Hidden) ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                        break;
                    case Key.Apps:
                        //if (!App.MainWindow.IELActionPanelMain.PanelActionActivate)
                        //    App.MainWindow.IELActionPanelMain.OpenPanelAction(RichTextBoxMainMessage, ConsolePage,
                        //        PositionAnimActionPanel.CenterObject, OrientationPositionCursor.LeftUp);
                        //else
                        //    App.MainWindow.IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                        break;
                    case Key.Down:
                    case Key.Up:
                        ProcessingActialSelectNavigating(e.Key);
                        break;
                    default:
                        if (SelectNavigation == SelectNavigationPageConsoleEnum.HitCommands)
                            SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                        break;
                }
                if (HitUse && SelectNavigation != SelectNavigationPageConsoleEnum.HitCommands)
                {
                    if (TextBoxCommandInput.Text.Length > 0 && TextBoxCommandInput.Text.Contains('*') && StateVisibleHit != ConsoleHitStateEnum.VisibleOneCommand)
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
            BorderHintCommand.SizeChanged += (sender, e) =>
            {
                if (StackPanelAllHit.ActualHeight > BorderHintCommand.MaxHeight)
                {
                    if (!IELHitScroll.IsVisibleScrollBar(IEL.CORE.Enums.ScrollOrientation.Vertical))
                        IELHitScroll.ActivateVerticalScrollBar();
                    IELHitScroll.UpdateHeightScrollBar();
                }
                else if (IELHitScroll.IsVisibleScrollBar(IEL.CORE.Enums.ScrollOrientation.Vertical))
                    IELHitScroll.DiactivateVerticalScrollBar();
            };
            #endregion

            TextBoxCommandInput.Focus();
            TextBlockInformation.Text = "Страница успешно инициализирована.";
        }

        #region PanelActionManipulate
        /// <summary>
        /// Исполнить использование панели действий под страницу консоли
        /// </summary>
        /// <param name="SelectViewer">Визуализатор команды который был выделен</param>
        private void UsingPanelActionFromConsolePage(OPLCommandViewer? SelectViewer)
        {
            PageConsoleActionPanelMain.CommandViewerSelect = SelectViewer;
            PageConsoleActionPanelMain.IELButtonCommandBuffer.IsEnabled = BufferPage != null;
            App.MainWindow.IELActionPanelMain.UsingPanelAction(BorderConsole, PageConsoleActionPanelMain,
                Orientation: OrientationPositionCursor.RightDown);
        }
        #endregion

        #region HintCommandManipulate
        /// <summary>
        /// Отсортировать в массиве строки
        /// </summary>
        /// <param name="Names">Массив строк</param>
        public static void SortNames(ref string[] Names)
        {
            char[][] CharMassName = [.. Names.Select((i) => i.ToArray())];
            int Index, IndexActivate = CharMassName.Length - 1;
            for (int i = 0; i < CharMassName.Length; i++)
            {
                Index = CharMassName.Length - 1;
                for (int j = Index - 1; j > -1; j--)
                {
                    if (CharMassName[IndexActivate].Length != CharMassName[j].Length)
                    {
                        if (CharMassName[IndexActivate].Length < CharMassName[j].Length) Index--;
                        continue;
                    }
                    for (int k = 0; k < CharMassName[IndexActivate].Length; k++)
                    {
                        if (CharMassName[IndexActivate][k] != CharMassName[j][k])
                        {
                            if (CharMassName[IndexActivate][k] < CharMassName[j][k]) Index--;
                            break;
                        }
                    }
                }
                if (IndexActivate != Index)
                {
                    (CharMassName[IndexActivate], CharMassName[Index]) = (CharMassName[Index], CharMassName[IndexActivate]);
                    continue;
                }
            }
            Names = [.. CharMassName.Select((i) => new string(i))];
        }

        /// <summary>
        /// Отобразить подсказки ко всем командам
        /// </summary>
        private void UsingAllHintCommand()
        {
            TimeSpan span = TimeSpan.FromMilliseconds(300d);

            string CommandText = COMInterpreterBase.ReadNameCommand(TextBoxCommandInput.Text);
            string[] AllHintNames =
                [.. App.CurrentApp.Interpreter.CommandWhere((i) => i.Name.Contains(CommandText, StringComparison.CurrentCultureIgnoreCase)).Select((i) => i.Name)];
            if (AllHintNames.Length == 0)
            {
                ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                StackPanelAllHit.Children.Clear();
                return;
            }
            StackPanelAllHit.Children.Clear();
            BorderHintCommand.Width = 0d;
            BorderHintCommand.Height = 0d;
            SortNames(ref AllHintNames);
            foreach (string Name in AllHintNames)
            {
                TextBlock block = CreateHintBlock(Name);
                StackPanelAllHit.Children.Add(block);
                block.UpdateLayout();
                RectangleSelect.Height = block.ActualHeight;
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
                if (StateHit == ConsoleHitStateEnum.Hidden) SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                TimeSpan span = TimeSpan.FromMilliseconds(300d);
                Canvas.SetZIndex(GridHintOneCommand, StateHit == ConsoleHitStateEnum.VisibleOneCommand ? 1 : -1);
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(GridHintOneCommand, OpacityProperty, StateHit == ConsoleHitStateEnum.VisibleOneCommand ? 1d : 0d, span);

                Canvas.SetZIndex(StackPanelAllHit, StateHit == ConsoleHitStateEnum.VisibleOneCommand ? -1 : 1);
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(IELHitScroll, OpacityProperty, StateHit == ConsoleHitStateEnum.VisibleOneCommand ? 0d : 1d, span);
                if ((StateHit is ConsoleHitStateEnum.VisibleOneCommand or ConsoleHitStateEnum.Hidden) && 
                    IELHitScroll.IsVisibleScrollBar(IEL.CORE.Enums.ScrollOrientation.Vertical))
                        IELHitScroll.DiactivateVerticalScrollBar();

                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderHintCommand, OpacityProperty, StateHit == ConsoleHitStateEnum.Hidden ? 0d : 1d, span);
                StateVisibleHit = StateHit;
            }
            if (StateHit == ConsoleHitStateEnum.Hidden) HidedHitPanel = false;
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
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderHintCommand, WidthProperty, AnimateWidth + 15, span);
            if (!HidedHitPanel) App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderHintCommand, HeightProperty, AnimateHeight, span);
        }

        /// <summary>
        /// Отобразить подсказку к конкретной команде
        /// </summary>
        /// <param name="TextCommand">Константный текст поиска команды</param>
        private void UsingOneHitCommand(string TextCommand)
        {
            ICommandOPER<IOPERCommandViewer>? CommandHint;
            CommandHint = App.CurrentApp.Interpreter.ReadCommand(TextCommand);
            if (CommandHint == null)
            {
                ChangeVisualHintCommand(ConsoleHitStateEnum.Hidden);
                return;
            }
            string[] Parameters = [.. CommandHint.Parameters?.Select((i) => $"{i.Name}{(i.Absolutly ? string.Empty : "?")}") ?? []];
            TextBlockHintCommand.Text = $"{CommandHint.Name}* {string.Join(",", Parameters)}";
            TextBlockHintCommand.UpdateLayout();

            TextBlockDescriptionHintCommand.ClearValue(WidthProperty);
            TextBlockDescriptionHintCommand.Text = CommandHint.Description;
            TextBlockDescriptionHintCommand.UpdateLayout();
            TextBlockDescriptionHintCommand.Width = TextBlockHintCommand.ActualWidth < 100d ? 100d : TextBlockHintCommand.ActualWidth;
            TextBlockDescriptionHintCommand.UpdateLayout();

            ChangeVisualHintCommand(ConsoleHitStateEnum.VisibleOneCommand);

            AnimateHitPanelFromOneCommand();

        }

        /// <summary>
        /// Анимировать размер подсказок к конкретной команде исходя их её предпочтительных размеров
        /// </summary>
        private void AnimateHitPanelFromOneCommand()
        {
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderHintCommand, WidthProperty, TextBlockDescriptionHintCommand.Width + 10d, TimeSpan.FromMilliseconds(300d));
            if (!HidedHitPanel) App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderHintCommand, HeightProperty,
                TextBlockDescriptionHintCommand.ActualHeight + TextBlockHintCommand.ActualHeight + 8d, TimeSpan.FromMilliseconds(300d));
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
                App.ManagerAnimation.ColorAnimationType.AnimateEffect(Result.Foreground, SolidColorBrush.ColorProperty,
                    Color.FromRgb(168, 217, 255), TimeSpan.FromMilliseconds(120d));
            };
            Result.MouseLeave += (sender, e) =>
            {
                App.ManagerAnimation.ColorAnimationType.AnimateEffect(Result.Foreground, SolidColorBrush.ColorProperty,
                    Color.FromRgb(11, 43, 68), TimeSpan.FromMilliseconds(120d));
            };
            Result.MouseLeftButtonUp += (sender, e) =>
            {
                TextBoxCommandInput.Text = $"{Result.Text}* ";
                UsingOneHitCommand(Result.Text);
                if (SelectNavigation == SelectNavigationPageConsoleEnum.HitCommands)
                    SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
            };
            return Result;
        }

        private void SetSelectNavigation(SelectNavigationPageConsoleEnum Value)
        {
            switch (Value)
            {
                case SelectNavigationPageConsoleEnum.None:
                    if (SelectNavigation == SelectNavigationPageConsoleEnum.HitCommands)
                        App.ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleSelect, OpacityProperty, 0d, TimeSpan.FromMilliseconds(400d));
                    break;
                case SelectNavigationPageConsoleEnum.HitCommands:
                    App.ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleSelect, OpacityProperty, 1d, TimeSpan.FromMilliseconds(400d));
                    break;
            }
            SelectNavigation = Value;
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
                    SetSelectNavigation(SelectNavigationPageConsoleEnum.HitCommands);
                    ActiveIndexHitCommandInput = -1;
                }
                else if (BufferPage != null)
                {
                    SelectNavigation = SelectNavigationPageConsoleEnum.BufferCommandTextBox;
                    ActiveIndexBufferInput = -1;
                }
                else return;
            }
            if (BufferPage?.BufferCommand == null) return;
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

                    // Смещение позиции области относительно внешнего элемента
                    System.Windows.Point OffsetPosElement = StackPanelAllHit.Children[ActiveIndexHitCommandInput].TransformToAncestor(
                        StackPanelAllHit).Transform(new System.Windows.Point(0, 0));
                    
                    App.ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleSelect, WidthProperty,
                            ((TextBlock)StackPanelAllHit.Children[ActiveIndexHitCommandInput]).ActualWidth, TimeSpan.FromMilliseconds(400d));
                    if (OffsetPosElement.Y + RectangleSelect.Height >= HeadHitPanelGrid.ActualHeight)
                    {
                        IELHitScroll.ScrollToVerticalOffset(
                            IELHitScroll.VerticalOffset +
                            (IELHitScroll.ScrollableHeight <= HeadHitPanelGrid.ActualHeight ?
                            IELHitScroll.ScrollableHeight : HeadHitPanelGrid.ActualHeight));
                    }
                    else if (OffsetPosElement.Y < IELHitScroll.VerticalOffset)
                    {
                        IELHitScroll.ScrollToVerticalOffset(
                            IELHitScroll.VerticalOffset <= HeadHitPanelGrid.ActualHeight ? 0d :
                            IELHitScroll.VerticalOffset - HeadHitPanelGrid.ActualHeight);
                    }
                    App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(RectangleSelect, MarginProperty, new(0, OffsetPosElement.Y, 0, 0), TimeSpan.FromMilliseconds(400d));
                    break;
            }
        }
        #endregion

        #region Command
        /// <summary>
        /// Создать новый визуализационный объект контента выполнения консольной команды
        /// </summary>
        /// <returns></returns>
        public OPLCommandViewer CreateNewCommandViewer(string Command)
        {
            OPLCommandViewer Viewer = new()
            {
                Margin = new(0),
                FontSize = 16d,
                CornerRadius = new(6),
                BorderThickness = new(2),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Opacity = 0d,
                Source = StructDirectoryResources.GetResourceUri(nameof(OPRES.MediaLoadingDefault)),
                DeleteButtonSource = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross)),
                Text = Command,
            };
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(Viewer);
            System.Windows.Data.Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["RussianRail G Pro"]
            };
            BindingOperations.SetBinding(Viewer, FontFamilyProperty, binding);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.PastelBlue].ConnectPalleteFromIELElement(Viewer);

            Viewer.MouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left && App.MainWindow.IELActionPanelMain.PanelActionActivate)
                    App.MainWindow.IELActionPanelMain.ClosePanelAction();
                else if (e.ChangedButton == MouseButton.Right) UsingPanelActionFromConsolePage(sender as OPLCommandViewer);
                e.Handled = true;
            };

            Viewer.ButtonDelete_OnActivateMouseLeft += (sender, e) =>
            {
                if (Viewer.IsTokenAsyncLoadingEnabled || Viewer.IsTokenAsyncWhileEnabled)
                {
                    MessageBoxResult Result =
                        System.Windows.MessageBox.Show("Вы точно хотите принудительно завершить выполнение команды?", "Подтверждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (Result == MessageBoxResult.No)
                    {
                        e.Handled = true;
                        return;
                    }
                    else if (Viewer.IsTokenAsyncLoadingEnabled)
                        Viewer.CancelExecuteTaskCommand();
                    else if (Viewer.IsTokenAsyncWhileEnabled)
                        Viewer.ExitAsyncWhileOperation();
                }
                DeleteCommandViewer(Viewer);
                e.Handled = true;
            };

            Viewer.AddContentInViewer += (sender, e) =>
            {
                //if (sender is OPLCommandViewer viewer && GridConsole.Children.IndexOf(viewer) + 1 < GridConsole.Children.Count)
                //{
                //    double ChangeTop = viewer.Margin.Top + viewer.ActualHeight + 10d;
                //    FrameworkElement Element;
                //    for (int i = GridConsole.Children.IndexOf(viewer) + 1; i < GridConsole.Children.Count; i++)
                //    {
                //        Element = (FrameworkElement)GridConsole.Children[i];
                //        Element.BeginAnimation(MarginProperty, null);
                //        Element.Margin = new(5, ChangeTop, 5, 5);
                //    }
                //}
            };

            StackPanelConsole.Children.Add(Viewer);
            //IELScrollConsole.SourceViewer.ScrollToEnd();
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(Viewer, MarginProperty, new(5), TimeSpan.FromMilliseconds(600d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(Viewer, OpacityProperty, 1d, TimeSpan.FromMilliseconds(600d));
            return Viewer;
        }

        /// <summary>
        /// Удалить элемент визуализации команды из страницы консоли
        /// </summary>
        /// <param name="Element">Удаляемый визуализационный элемент</param>
        internal void DeleteCommandViewer(OPLCommandViewer Element)
        {
            //StackPanelConsole.Children.Remove(Element);
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(Element, MarginProperty, new(0), TimeSpan.FromMilliseconds(300d));
            //Element.Height = Element.ActualHeight;
            DoubleAnimation animation = App.ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
            animation.Duration = TimeSpan.FromMilliseconds(400d);
            animation.From = Element.ActualHeight;
            animation.To = 0d;
            Element.BeginAnimation(HeightProperty, animation);
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (sender, e) =>
            {
                //Element.BeginAnimation(OpacityProperty, null);
                //Element.BeginAnimation(HeightProperty, null);
                //Element.Height = 0d;
                //Element.Opacity = 0d;
                StackPanelConsole.Children.Remove(Element);
            };
            animation.From = 1d;
            Element.BeginAnimation(OpacityProperty, animation);
        }

        /// <summary>
        /// Активировать команду под логикой страницы консоли<br/>
        /// Команда учитывается из текстового поля
        /// </summary>
        /// <returns></returns>
        public async Task ActivateCommand()
        {
            string Command = TextBoxCommandInput.Text;
            TextBoxCommandInput.Text = string.Empty;
            BufferPage?.InsertCommandFromBuffer(Command, this);

            await App.CurrentApp.ActivateActionCommand(CreateNewCommandViewer(COMInterpreterBase.ReadNameCommand(Command)), Command);
        }
        #endregion
    }
}
