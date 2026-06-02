using IEL.CORE.Enums;
using Interpreter.Interfaces;
using InterpreterCommand.Classes;
using InterpreterCommand.Commands;
using OIEL.UserElementsControl.Interfaces;
using OperPageLes.CORE.Enums;
using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace OperPageLes.UI.UserElementsControl.Default
{
    /// <summary>
    /// Логика взаимодействия для OPLHitInterpreter.xaml
    /// </summary>
    public partial class OPLHitInterpreter : IEL.UserElementsControl.Base.IELContainerBase, IOPLAnimate
    {
        #region Hit

        /// <summary>
        /// Объект управляемых визуализаторов команд
        /// </summary>
        internal StackPanel StackPanelAllHit { get; private set; }

        /// <summary>
        /// Интерпретатор к командам которого реализуются подсказки к командам
        /// </summary>
        private COMInterpreter<IOPERCommandViewer>? SourceConnectInterpreter;

        /// <summary>
        /// Элемент ввода команды
        /// </summary>
        private System.Windows.Controls.TextBox? SourceInputElement;

        /// <summary>
        /// Состояние видимости подсказок
        /// </summary>
        internal HitStateEnum StateVisibleHit { get; private set; }

        /// <summary>
        /// Сохранённое состояние видимости подсказок
        /// </summary>
        internal HitStateEnum SaveStateHit { get; private set; }

        /// <summary>
        /// Активный индекс команды в подсказках к командам для строки ввода
        /// </summary>
        internal int ActiveIndexHitCommandInput { get; private set; }

        /// <summary>
        /// Состояние скрытия панели подсказок к командам
        /// </summary>
        private bool HidedHitPanel = false;
        #endregion

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Цвет используемый для выделения элемента
        /// </summary>
        private readonly SolidColorBrush SelectColorHitElement;

        /// <summary>
        /// Объект аргументов действия мыши
        /// </summary>
        private static readonly System.Windows.Input.MouseEventArgs SourceMouseEventArgs = new(Mouse.PrimaryDevice, 0);

        /// <summary>
        /// Объект аргументов действия нажатия мыши
        /// </summary>
        private static readonly MouseButtonEventArgs SourceMouseButtonEventArgs = new(Mouse.PrimaryDevice, 0, MouseButton.Left);

        public OPLHitInterpreter()
        {
            InitializeComponent();
            SelectColorHitElement = new(Color.FromArgb(255, 81, 177, 219))
            {
                Opacity = 0d
            };
            StackPanelAllHit = new()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
            };
            IELHitScroll.AutoUpdateVisibleHorizontalScroll = false;
            IELHitScroll.AutoUpdateVisibleVerticalScroll = false;
            IELHitScroll.Content = StackPanelAllHit;
            StateVisibleHit = HitStateEnum.Hidden;
            ActiveIndexHitCommandInput = -1;
            Height = 0d;
            GridHintOneCommand.Opacity = 0d;
            Canvas.SetZIndex(GridHintOneCommand, -1);

            MouseRightButtonUp += (sender, e) =>
            {
                if (HidedHitPanel)
                {
                    HidedHitPanel = false;
                    HeadHitPanelGrid.IsEnabled = true;
                    if (StateVisibleHit == HitStateEnum.VisibleOneCommand) AnimateHitPanelFromOneCommand();
                    else ChangeVisualHintCommand(StateVisibleHit);
                }
                else
                {
                    App.ManagerAnimation.DoubleAnimationType.AnimateEffect(this, HeightProperty, 10d, TimeSpan.FromMilliseconds(400d));
                    HeadHitPanelGrid.IsEnabled = false;
                    HidedHitPanel = true;
                    //Keyboard.ClearFocus();
                }
            };

            SizeChanged += (sender, e) =>
            {
                if (StackPanelAllHit.ActualHeight > MaxHeight)
                {
                    if (!IELHitScroll.IsVisibleScrollBar(IEL.CORE.Enums.ScrollOrientation.Vertical))
                        IELHitScroll.ActivateVerticalScrollBar();
                    IELHitScroll.UpdateHeightScrollBar();
                }
                else if (IELHitScroll.IsVisibleScrollBar(IEL.CORE.Enums.ScrollOrientation.Vertical))
                    IELHitScroll.DiactivateVerticalScrollBar();
            };
        }

        /// <summary>
        /// Присоеденить интерпретатор команд к подсказкам команд
        /// </summary>
        /// <param name="SourceInterpreter">Присоеденяемый интерпретатор</param>
        public void Connect(in COMInterpreter<IOPERCommandViewer> SourceInterpreter, in System.Windows.Controls.TextBox InputElement)
        {
            SourceConnectInterpreter = SourceInterpreter;
            SourceInputElement?.KeyUp -= SourceInputElement_KeyUp;
            SourceInputElement?.MouseUp -= SourceInputElement_MouseUp;
            SourceInputElement = InputElement;
            SourceInputElement.KeyUp += SourceInputElement_KeyUp;
            SourceInputElement.MouseUp += SourceInputElement_MouseUp;
        }

        /// <summary>
        /// Обновить состояние подсказок к командам
        /// </summary>
        /// <param name="Text">Текст на основе которого происходит обновление</param>
        /// <param name="SeparatorCommand">Сепаратор строки команды разделяющий имя и параметры</param>
        public void UpdateState(string Text, char SeparatorCommand = '*')
        {
            if (Text.Length > 0 && Text.Contains(SeparatorCommand) &&
                    StateVisibleHit != OPLHitInterpreter.HitStateEnum.VisibleOneCommand)
            {
                UsingOneHitCommand(Text);
                return;
            }
            else if (Text.Length == 0 && StateVisibleHit != OPLHitInterpreter.HitStateEnum.Hidden)
                ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);
            else if (!Text.Contains(SeparatorCommand) && Text.Length > 0)
            {
                UsingAllHintCommand(Text);
            }
        }

        /// <summary>
        /// Функция обработки скрытия подсказки к командам
        /// </summary>
        private void SourceInputElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (HidedHitPanel)
            {
                HidedHitPanel = false;
                HeadHitPanelGrid.IsEnabled = true;
                if (StateVisibleHit == HitStateEnum.VisibleOneCommand) AnimateHitPanelFromOneCommand();
                else ChangeVisualHintCommand(StateVisibleHit);
            }
        }

        /// <summary>
        /// Функция обработки нажатия клавиши
        /// </summary>
        private void SourceInputElement_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key == Key.Enter || e.Key == Key.Escape) && ActiveIndexHitCommandInput > -1)
            {
                if (e.Key == Key.Enter)
                    SetTextElementHit(StackPanelAllHit.Children[ActiveIndexHitCommandInput],
                        OPLHitInterpreter.SourceMouseButtonEventArgs);
                else if (e.Key == Key.Escape)
                    ChangeColorElementHitCommandMouseLeave((TextBlock)StackPanelAllHit.Children[ActiveIndexHitCommandInput],
                        OPLHitInterpreter.SourceMouseEventArgs);
                ActiveIndexHitCommandInput = -1;
                e.Handled = true;
                return;
            }
            else if ((e.Key != Key.Up && e.Key != Key.Down) || StackPanelAllHit.Children.Count == 0) return;
            else if (ActiveIndexHitCommandInput == -1)
            {
                ActiveIndexHitCommandInput = e.Key == Key.Up ? StackPanelAllHit.Children.Count - 1 : 0;
            }
            else
            {
                ChangeColorElementHitCommandMouseLeave((TextBlock)StackPanelAllHit.Children[ActiveIndexHitCommandInput],
                        OPLHitInterpreter.SourceMouseEventArgs);
                if (e.Key == Key.Up)
                {
                    ActiveIndexHitCommandInput = ActiveIndexHitCommandInput > 0 ?
                        ActiveIndexHitCommandInput - 1 : StackPanelAllHit.Children.Count - 1;
                }
                else
                {
                    ActiveIndexHitCommandInput = ActiveIndexHitCommandInput < StackPanelAllHit.Children.Count - 1 ?
                        ActiveIndexHitCommandInput + 1 : 0;
                }
            }
            ChangeColorElementHitCommandMouseEnter((TextBlock)StackPanelAllHit.Children[ActiveIndexHitCommandInput],
                        OPLHitInterpreter.SourceMouseEventArgs);
            //SourceInputElement?.Text = SelectElement.Text;

            // Смещение позиции области относительно внешнего элемента
            System.Windows.Point OffsetPosElement = StackPanelAllHit.Children[ActiveIndexHitCommandInput].TransformToAncestor(
                StackPanelAllHit).Transform(new System.Windows.Point(0, 0));

            if (OffsetPosElement.Y + ((FrameworkElement)StackPanelAllHit.Children[ActiveIndexHitCommandInput]).ActualHeight >= 
                HeadHitPanelGrid.ActualHeight)
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

            if (IsEnabled && StateVisibleHit == HitStateEnum.Hidden && SourceInputElement != null)
            {
                if (SourceInputElement.Text.Length > 0 && SourceInputElement.Text.Contains('*') && StateVisibleHit != HitStateEnum.VisibleOneCommand)
                {
                    UsingOneHitCommand(SourceInputElement.Text);
                    return;
                }
                else if (SourceInputElement.Text.Length == 0 && StateVisibleHit != HitStateEnum.Hidden) ChangeVisualHintCommand(HitStateEnum.Hidden);
                else if (!SourceInputElement.Text.Contains('*') && SourceInputElement.Text.Length > 0)
                {
                    UsingAllHintCommand(SourceInputElement.Text);
                }
            }
        }

        #region HintCommandManipulate
        /// <summary>
        /// Отобразить подсказки ко всем командам
        /// </summary>
        /// <param name="SourceText">Обрабатываемый текст</param>
        public void UsingAllHintCommand(string SourceText)
        {
            if (SourceConnectInterpreter == null)
                throw new InvalidOperationException("Невозможно произвести использование подказки к командам не имея подключённого интерпретатора");
            TimeSpan span = TimeSpan.FromMilliseconds(300d);

            string CommandText = COMInterpreterBase.ReadNameCommand(SourceText);
            string[] AllHintNames =
                [.. SourceConnectInterpreter.CommandWhere((i) => i.Name.Contains(CommandText, StringComparison.CurrentCultureIgnoreCase)).Select((i) => i.Name)];
            StackPanelAllHit.Children.Clear();
            if (AllHintNames.Length == 0)
            {
                ChangeVisualHintCommand(HitStateEnum.Hidden);
                return;
            }
            else
            {
                AllHintNames.Sort((x, y) =>
                {
                    if (x.Length == 0 && y.Length == 0) return 0;
                    else if (x.Length == 0) return -1;
                    else if (y.Length == 0) return 1;
                    else return x.CompareTo(y);
                });
            }
            Width = 0d;
            Height = 0d;
            foreach (string Name in AllHintNames)
            {
                TextBlock block = CreateHintBlock(Name);
                StackPanelAllHit.Children.Add(block);
                block.UpdateLayout();
            }
            ActiveIndexHitCommandInput = -1;
            ChangeVisualHintCommand(HitStateEnum.VisibleMainCommands);
        }

        /// <summary>
        /// Изменить визуализацию подсказок к командам
        /// </summary>
        /// <param name="StateHit">Изменяемое состояние</param>
        public void ChangeVisualHintCommand(HitStateEnum StateHit)
        {
            if (StateVisibleHit != StateHit)
            {
                //if (StateHit == ConsoleHitStateEnum.Hidden) SetSelectNavigation(SelectNavigationPageConsoleEnum.None);
                TimeSpan span = TimeSpan.FromMilliseconds(300d);
                Canvas.SetZIndex(GridHintOneCommand, StateHit == HitStateEnum.VisibleOneCommand ? 1 : -1);
                if (ManagerAnimation != null)
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(GridHintOneCommand, OpacityProperty, StateHit == HitStateEnum.VisibleOneCommand ? 1d : 0d, span);
                else
                    GridHintOneCommand.Opacity = StateHit == HitStateEnum.VisibleOneCommand ? 1d : 0d;

                Canvas.SetZIndex(StackPanelAllHit, StateHit == HitStateEnum.VisibleOneCommand ? -1 : 1);
                if (ManagerAnimation != null)
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(IELHitScroll, OpacityProperty, StateHit == HitStateEnum.VisibleOneCommand ? 0d : 1d, span);
                else
                    IELHitScroll.Opacity = StateHit == HitStateEnum.VisibleOneCommand ? 0d : 1d;
                if ((StateHit is HitStateEnum.VisibleOneCommand or HitStateEnum.Hidden) &&
                    IELHitScroll.IsVisibleScrollBar(IEL.CORE.Enums.ScrollOrientation.Vertical))
                    IELHitScroll.DiactivateVerticalScrollBar();

                if (ManagerAnimation != null)
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(this, OpacityProperty, StateHit == HitStateEnum.Hidden ? 0d : 1d, span);
                else
                    Opacity = StateHit == HitStateEnum.Hidden ? 0d : 1d;
                StateVisibleHit = StateHit;
            }
            if (StateHit == HitStateEnum.Hidden)
            {
                //HidedHitPanel = false;
                ActiveIndexHitCommandInput = -1;
            }
            AnimateSizeHintPanel(0d, 0d, StateHit != HitStateEnum.Hidden);
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
                AnimateWidth += Padding.Left + Padding.Right + 8;
                AnimateHeight += Padding.Top + Padding.Bottom + 8;
                if (AnimateHeight > MaxHeight) AnimateHeight = MaxHeight;
            }
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(this, WidthProperty, AnimateWidth + 15, span);
            else
                Width = AnimateWidth + 15;
            if (!HidedHitPanel)
            {
                if (ManagerAnimation != null)
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(this, HeightProperty, AnimateHeight, span);
                else
                    Height = AnimateHeight;
            }
            else
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(this, HeightProperty, 10d, TimeSpan.FromMilliseconds(400d));
            }
        }

        /// <summary>
        /// Отобразить подсказку к конкретной команде
        /// </summary>
        /// <param name="TextCommand">Константный текст поиска команды</param>
        public void UsingOneHitCommand(string TextCommand)
        {
            CommandOPER<IOPERCommandViewer>? CommandHint = App.CurrentApp.Interpreter.ReadCommand(TextCommand);
            if (CommandHint == null)
            {
                ChangeVisualHintCommand(HitStateEnum.Hidden);
                return;
            }
            string[] Parameters = [.. CommandHint.Parameters?.Select((i) => $"{i.Name}{(i.Absolutly ? string.Empty : "?")}") ?? []];
            TextBlockHintCommand.Text = $"{CommandHint.Name}* {string.Join(", ", Parameters)}";
            TextBlockHintCommand.UpdateLayout();

            TextBlockDescriptionHintCommand.ClearValue(WidthProperty);
            TextBlockDescriptionHintCommand.Text = CommandHint.Description;
            TextBlockDescriptionHintCommand.UpdateLayout();
            TextBlockDescriptionHintCommand.Width = TextBlockHintCommand.ActualWidth < 100d ? 100d : TextBlockHintCommand.ActualWidth;
            TextBlockDescriptionHintCommand.UpdateLayout();

            ChangeVisualHintCommand(HitStateEnum.VisibleOneCommand);

            AnimateHitPanelFromOneCommand();

        }

        /// <summary>
        /// Анимировать размер подсказок к конкретной команде исходя их её предпочтительных размеров
        /// </summary>
        private void AnimateHitPanelFromOneCommand()
        {
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(this, WidthProperty, TextBlockDescriptionHintCommand.Width + 10d, TimeSpan.FromMilliseconds(300d));
            else
                Width = TextBlockDescriptionHintCommand.Width + 10d;
            if (!HidedHitPanel)
            {
                if (ManagerAnimation != null)
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(this, HeightProperty,
                        TextBlockDescriptionHintCommand.ActualHeight + TextBlockHintCommand.ActualHeight + 8d, TimeSpan.FromMilliseconds(300d));
                else
                    Height = TextBlockDescriptionHintCommand.ActualHeight + TextBlockHintCommand.ActualHeight + 8d;
            }
            else
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(this, HeightProperty, 10d, TimeSpan.FromMilliseconds(400d));
            }
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
                Background = SourceBackground.SourceBrush.Clone(),
            };
            Result.MouseEnter += ChangeColorElementHitCommandMouseEnter;
            Result.MouseLeave += ChangeColorElementHitCommandMouseLeave;
            Result.MouseLeftButtonUp += SetTextElementHit;
            return Result;
        }

        private void SetTextElementHit(object sender, MouseButtonEventArgs e)
        {
            TextBlock Element = (TextBlock)sender;
            SourceInputElement?.Text = $"{Element.Text}* ";
            SourceInputElement?.SelectionStart = SourceInputElement.Text.Length;
            UsingOneHitCommand(Element.Text);
        }

        private void ChangeColorElementHitCommandMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlock Element = (TextBlock)sender;
            if (ManagerAnimation != null)
                ManagerAnimation.ColorAnimationType.AnimateEffect(e.Timestamp == 0 ? Element.Background : Element.Foreground,
                    SolidColorBrush.ColorProperty, e.Timestamp == 0 ? Background.Default : Color.FromRgb(11, 43, 68),
                    TimeSpan.FromMilliseconds(120d));
            else
                ((SolidColorBrush)Element.Foreground).Color = Color.FromRgb(11, 43, 68);
        }

        private void ChangeColorElementHitCommandMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            TextBlock Element = (TextBlock)sender;
            if (ManagerAnimation != null)
            {
                ManagerAnimation.ColorAnimationType.AnimateEffect(e.Timestamp == 0 ? Element.Background : Element.Foreground, 
                    SolidColorBrush.ColorProperty, e.Timestamp == 0 ? Background.Select : Color.FromRgb(168, 217, 255),
                    TimeSpan.FromMilliseconds(120d));
            }
            else
                ((SolidColorBrush)Element.Foreground).Color = Color.FromRgb(168, 217, 255);
        }
        #endregion

        /// <summary>
        /// Состояния отображения подсказки к командам
        /// </summary>
        public enum HitStateEnum
        {
            /// <summary>
            /// Отображение отлючено
            /// </summary>
            Hidden = 0,

            /// <summary>
            /// Отображение свех команд
            /// </summary>
            VisibleMainCommands = 1,

            /// <summary>
            /// Отображение одной команды
            /// </summary>
            VisibleOneCommand = 2,
        }
    }
}
