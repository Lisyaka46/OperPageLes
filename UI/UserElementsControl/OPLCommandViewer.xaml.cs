using ApplicationOperPageLes.CORE.Interfaces;
using ApplicationOperPageLes.CORE.Struct;
using IEL.UserElementsControl;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using OPRES = ApplicationOperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;

namespace ApplicationOperPageLes.UI.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLCommandViewer.xaml
    /// </summary>
    public partial class OPLCommandViewer : IEL.UserElementsControl.Base.IELContainerBase, IOPERCommandViewer
    {
        /// <summary>
        /// Главный объект отображения текста
        /// </summary>
        private TextBlock TextBlockHead;

        /// <summary>
        /// Токен отмены асинхронной загрузочной операции
        /// </summary>
        private CancellationTokenSource? SourceTokenAsyncLoading = null;

        /// <summary>
        /// Имеется ли активный/исполняемый токен асинхонной загрузки
        /// </summary>
        public bool IsTokenAsyncLoadingEnabled => (SourceTokenAsyncLoading?.Token.CanBeCanceled) ?? false;

        /// <summary>
        /// Событие добавления контента в объект визуализатора
        /// </summary>
        public event EventHandler? AddContentInViewer;

        /// <summary>
        /// Состояние асинхронного постоянного исполнения
        /// </summary>
        public bool IsTokenAsyncWhileEnabled => (SourceTokenAsyncWhile?.Token.CanBeCanceled) ?? false;

        /// <summary>
        /// Токен отмены асинхронной циклической операции
        /// </summary>
        private CancellationTokenSource? SourceTokenAsyncWhile = null;

        public OPLCommandViewer()
        {
            InitializeComponent();
            AsyncIndicator.Opacity = 0d;
            BorderInfo.BorderBrush = SourceBorderBrush.SourceBrush;
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELButtonDeleteElement);
            IndicatorLoading.Opacity = 0d;
            IndicatorLoading.Source = null;
            IndicatorLoading.MediaEnded += (sender, e) =>
            {
                IndicatorLoading.Position = TimeSpan.FromMilliseconds(5);
            };
            IndicatorLoading.Stop();
            Container.Children.Clear();

            TextBlockHead = CreateHeadTextBlock();
            Container.Children.Add(TextBlockHead);
        }

        /// <summary>
        /// Добавить новый <b>не форматированный</b> текст
        /// </summary>
        /// <param name="Source">Добавляемый текст</param>
        public void AddString(string Source)
        {
            Dispatcher.Invoke(() =>
            {
                P_AddString(Source);
                UpdateLayout();
            });
            AddContentInViewer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Добавить новый <b>не форматированный</b> текст
        /// <b>БЕЗ СОБЫТИЯ</b>
        /// </summary>
        /// <param name="Source">Добавляемый текст</param>
        private void P_AddString(string Source)
        {
            if (TextBlockHead.Inlines.Count > 0)
                TextBlockHead.Inlines.Add(new LineBreak());
            TextBlockHead.Inlines.Add(Source);
        }

        /// <summary>
        /// Добавить новый текст исходя из входящего объекта
        /// </summary>
        /// <param name="Array">Массив зависимых объектов</param>
        /// <param name="Function">Преобразование данных объекта в строку</param>
        public void AddString<TSource>(TSource[] Array, Func<TSource, string?> Function)
        {
            string? Source = null;
            foreach (TSource item in Array)
            {
                Source = Function.Invoke(item);
                if (Source != null)
                    P_AddString(Source);
            }
            UpdateLayout();
            AddContentInViewer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Добавить новый <b>форматированный</b> текст
        /// </summary>
        /// <param name="Source">Добавляемый текст</param>
        public void AddFormattedString(string Source)
        {
            if (TextBlockHead.Inlines.Count > 0)
                TextBlockHead.Inlines.Add(new LineBreak());
            TextBlockHead.Inlines.Add(FormattedAllTextDetect(Source));
            UpdateLayout();
            AddContentInViewer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Добавить новый элемент управления в консоль
        /// </summary>
        /// <param name="Source">Добавляемый элемент</param>
        public void AddNewUIElement(UIElement Source)
        {
            Container.Children.Add(Source);

            TextBlockHead = CreateHeadTextBlock();
            Container.Children.Add(TextBlockHead);
            UpdateLayout();
            AddContentInViewer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Зарегестрировать циклическую асинхронную операцию
        /// </summary>
        /// <param name="Source">Асинхронная операция</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns></returns>
        /// <exception cref="Exception">Исключение невозможной регистрации операции</exception>
        public async Task WaitWhileTaskOperation(Action Source, bool ExceptionRealized = true)
        {
            SourceTokenAsyncWhile = new();
            if (SourceTokenAsyncWhile.IsCancellationRequested)
                throw new Exception("Невозможно зарегестрировать токен операция которого уже была отменена.");

            DoubleAnimation AnimationDoubleGradientStops = App.DoubleAnimationType.SourceAnimation.Clone();
            AnimationDoubleGradientStops.RepeatBehavior = RepeatBehavior.Forever;
            AnimationDoubleGradientStops.AutoReverse = true;
            AnimationDoubleGradientStops.EasingFunction = null;
            AnimationDoubleGradientStops.Duration = TimeSpan.FromSeconds(1d);
            AnimationDoubleGradientStops.From = 0d;
            AnimationDoubleGradientStops.To = 1d;

            AsyncIndicator.BeginAnimation(OpacityProperty, AnimationDoubleGradientStops);
            Task task = new(() =>
            {
                while (!SourceTokenAsyncWhile.Token.IsCancellationRequested)
                    Source.Invoke();
            }, SourceTokenAsyncWhile.Token);
            task.Start();
            await ExecuteTask(task, SourceTokenAsyncWhile, ExceptionRealized);
        }

        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки без ожидаемого значения
        /// </summary>
        /// <param name="Method">Исполняемый асинхронный процесс</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        public async Task ExecuteVisualizateTask(Task Method, bool ExceptionRealized = true)
        {
            if (IsTokenAsyncLoadingEnabled) throw new Exception(
                "Невозможно визуализировать ожидание так как текущее ожидание не завершилось!\n" +
                "Завершение визуализации команды.");
            IndicatorLoading.Source = StructDirectoryResources.GetResourceUri(nameof(OPRES.MediaLoadingDefault));
            DoubleAnimation animation = App.DoubleAnimationType.SourceAnimation.Clone();
            animation.To = 0d;
            animation.Duration = TimeSpan.FromMilliseconds(480d);
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (sender, e) =>
            {
                IndicatorLoading.Opacity = 0d;
                IndicatorLoading.Stop();
                IndicatorLoading.Source = null;
            };
            SourceTokenAsyncLoading = new();
            IndicatorLoading.Play();
            App.DoubleAnimationType.AnimateEffect(IndicatorLoading, OpacityProperty, 1d, TimeSpan.FromMilliseconds(480d));

            try { await ExecuteTask(Method, SourceTokenAsyncLoading, ExceptionRealized); }
            finally
            {
                IndicatorLoading.BeginAnimation(OpacityProperty, animation);
                SourceTokenAsyncLoading.Dispose();
                GC.Collect();
                SourceTokenAsyncLoading = null;
            }
            if (Method.IsCanceled)
                throw new OperationCanceledException(
                    "Операция исполнения команды была прервана через визуализатор!\n" +
                    "Завершение визуализации команды.");
        }

        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки без ожидаемого значения
        /// </summary>
        /// <param name="Method">Исполняемый асинхронный процесс</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        public async Task<T> ExecuteVisualizateTask<T>(Task<T> Method, bool ExceptionRealized = true)
        {
            if (IsTokenAsyncLoadingEnabled) throw new Exception(
                "Невозможно визуализировать ожидание так как текущее ожидание не завершилось!\n" +
                "Завершение визуализации команды.");
            IndicatorLoading.Source = StructDirectoryResources.GetResourceUri(nameof(OPRES.MediaLoadingDefault));
            DoubleAnimation animation = App.DoubleAnimationType.SourceAnimation.Clone();
            animation.To = 0d;
            animation.Duration = TimeSpan.FromMilliseconds(480d);
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (sender, e) =>
            {
                IndicatorLoading.Opacity = 0d;
                IndicatorLoading.Stop();
                IndicatorLoading.Source = null;
            };
            SourceTokenAsyncLoading = new();
            IndicatorLoading.Play();
            App.DoubleAnimationType.AnimateEffect(IndicatorLoading, OpacityProperty, 1d, TimeSpan.FromMilliseconds(480d));

            T Result;
            try { Result = await ExecuteTask(Method, SourceTokenAsyncLoading, ExceptionRealized); }
            finally
            {
                IndicatorLoading.BeginAnimation(OpacityProperty, animation);
                SourceTokenAsyncLoading.Dispose();
                GC.Collect();
                SourceTokenAsyncLoading = null;
            }
            if (Method.IsCanceled)
                throw new OperationCanceledException(
                    "Операция исполнения команды была прервана через визуализатор!\n" +
                    "Завершение визуализации команды.");
            return Result;
        }

        #region AsyncWait
        /// <summary>
        /// Исполнить Task и исполнить ожидание исполнения
        /// </summary>
        /// <param name="Source">Асинхронная операция</param>
        /// <param name="SourceToken">Управляемый токен</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns></returns>
        private async Task ExecuteTask(Task Source, CancellationTokenSource SourceToken, bool ExceptionRealized)
        {
            try { await Source.WaitAsync(SourceToken.Token); }
            catch
            {
                if (ExceptionRealized)
                    AddFormattedString(
                    "%#FFBABA__Произошла ошибка в исполнении операции:__\n");// +
                    //$"\"%//{ex.Message}//\"");
                else throw;
            }
        }

        /// <summary>
        /// Исполнить Task и исполнить ожидание исполнения
        /// </summary>
        /// <param name="Source">Асинхронная операция</param>
        /// <param name="SourceToken">Управляемый токен</param>
        /// <param name="ExceptionRealized">Выводить ли сообщение об ошибке</param>
        /// <returns></returns>
        private async Task<T> ExecuteTask<T>(Task<T> Source, CancellationTokenSource SourceToken, bool ExceptionRealized = true)
        {
            T? Result = default;
            try { Result = await Source.WaitAsync(SourceToken.Token); }
            catch (Exception ex)
            {
                if (ExceptionRealized)
                    AddFormattedString(
                    "%#FFBABA__Произошла ошибка в исполнении операции:__\n" +
                    $"\"%//{ex.Message}//\"");
                else throw;
            }
            return Result != null ? Result : throw new Exception("Непредвиденное возвращение нулевого объекта в ожидании.");
        }
        #endregion

        /// <summary>
        /// Отменить выполнение асинхронной операции
        /// </summary>
        public void CancelExecuteTaskCommand()
        {
            if (SourceTokenAsyncLoading == null) throw new Exception("Невозможно отменить выполнение асинхронной операции не запустив её!");
            SourceTokenAsyncLoading.Cancel();
        }

        /// <summary>
        /// Осуществить выход из циклической асинхронной операции
        /// </summary>
        public void ExitAsyncWhileOperation()
        {
            if (SourceTokenAsyncWhile == null) throw new Exception("Невозможно отменить выполнение асинхронной операции не запустив её!");
            AsyncIndicator.Dispatcher.Invoke(() =>
                App.DoubleAnimationType.AnimateEffect(AsyncIndicator, OpacityProperty, 0d, TimeSpan.FromMilliseconds(400d)));
            SourceTokenAsyncWhile.Cancel();
        }

        /// <summary>
        /// Создать управляемый объект для текста
        /// </summary>
        /// <returns></returns>
        private static TextBlock CreateHeadTextBlock()
        {
            TextBlock Element = new()
            {
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.WordEllipsis,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new(3),
            };
            return Element;
        }

        #region ManipulateText
        /// <summary>
        /// Изменить формативность текста с учётом первых знаков
        /// </summary>
        /// <remarks>
        /// %#FFFFFF** <b>Italic</b> **<br/>
        /// <br/>
        /// ** <b>Bold</b> **<br/>
        /// // <i>Italic</i> //<br/>
        /// __ <u>UnderLine</u> __<br/>
        /// </remarks>
        /// <param name="Text">Текст форматирования</param>
        /// <returns>Форматированный текст</returns>
        private static Span FormattedAllTextDetect(string Text)
        {
            // %//Italic %**Bold**//
            Span Result = new();
            foreach (Match match in RegexFormattedText().Matches(Text))
            {
                Result.Inlines.AddRange(FormattedBlockText(match.Value));
            }
            return Result;
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
                BackgroundColor = new((WnColor)System.Windows.Media.ColorConverter.ConvertFromString(
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
                else
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
