using AAC20.Classes;
using AAC20.Windows.Pages.ActionPanel;
using Interpreter.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using IEL.Classes;
using IEL;
using IEL.Interfaces.Core;
using System.Windows.Media;
using AAC20.Classes.Flaging;
using MySql.Data.MySqlClient;

namespace AAC20.Windows.Pages.Other
{
    /// <summary>
    /// Логика взаимодействия для PageLabels.xaml
    /// </summary>
    public partial class PageLabels : Page, IPageDefault
    {
        /// <summary>
        /// Модуль страницы
        /// </summary>
        public ModulePage ModulePage { get; }

        /// <summary>
        /// Главная страница компонента
        /// </summary>
        public Grid MainGrid => GridMain;

        /// <summary>
        /// Динамический массив ярлыков
        /// </summary>
        private readonly List<IELLabelCommand> ObjectsLabel;

        /// <summary>
        /// Индекс выделенного элемента
        /// </summary>
        private int SelectIndexElementLabel = -1;
        
        /// <summary>
        /// Страница ярлыка в панели действий
        /// </summary>
        private static readonly PageLabelActionPanel PageLabelActPanel = new();

        /// <summary>
        /// Настройка поведения панели действий для объектов ярлыка
        /// </summary>
        private SettingsPanelActionFrameworkElement SettingsPanelActionElement;

        /// <summary>
        /// Скролл-бар страницы ярлыков
        /// </summary>
        internal readonly CounterScrollBar ScrollBar;

        /// <summary>
        /// Объект анимации для управления double значением
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimateObj = new(0, TimeSpan.FromMilliseconds(400d))
        {
            DecelerationRatio = 0.2d,
            EasingFunction = new QuinticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(550d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Возможен ли повтор анимации загрузки
        /// </summary>
        private readonly Flag AnimateForeverLoading = new(false);

        /// <summary>
        /// Флаг поиска данных о ярлыках в базе данных
        /// </summary>
        private readonly Flag SearchInfoSQL = new(false);

        /// <summary>
        /// Массив доступный из потока чтения базы данных
        /// </summary>
        private volatile LabelAction[] SQLLabelActions;

        /// <summary>
        /// Флаг успешного завершения чтения ярлыков из базы данных
        /// </summary>
        internal bool SQLCompleteSearch { get; private set; } = false;

        /// <summary>
        /// Поток чтения ярлыков из базы данных
        /// </summary>
        private readonly ThreadGenericProcess SQLLoadInformation;

        public PageLabels()
        {
            InitializeComponent();
            BorderScrollBackground.Width = 0d;
            ModulePage = new(nameof(PageLabels));
            ScrollBar = new(10, TrafficShare: 2);
            SettingsPanelActionElement = new(GridMain, PageLabelActPanel, new(110, 130));
            ((RadialGradientBrush)BorderNamingLabel.BorderBrush).Center = new(-1d, 0.5d);
            SQLLabelActions = [];
            ObjectsLabel = [];
            PageLabelActPanel.IELButtonExecuteLabel.OnActivateMouseLeft += (Key) =>
            {
                ObjectsLabel[SelectIndexElementLabel].OnActivateMouseLeft?.Invoke();
            };

            GridMain.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });
            GridMain.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });

            ScrollBar.ChangedValue += (NewValue) =>
            {
                ThicknessAnimate.To = new(0, 0 - (75 + 3) * NewValue, 0, 0);
                GridMain.BeginAnimation(MarginProperty, ThicknessAnimate);
                DoubleAnimateObj.To = ActualWidth / (int)(ScrollBar.MaxValue + 0.5d) * NewValue;
                BorderScrollBackground.BeginAnimation(WidthProperty, DoubleAnimateObj);
            };
            GridMain.MouseWheel += (sender, e) =>
            {
                if (ScrollBar.MaxValue > 0 && ObjectsLabel.Count > 0)
                {
                    if (e.Delta > 0) ScrollBar.Up();
                    else if (e.Delta < 0) ScrollBar.Down();
                }
            };

            BorderNamingLabel.MouseEnter += (sender, e) =>
            {
                ThicknessAnimate.To = new(0, 5, 0, 5);
                Storyboard.SetTargetProperty(ThicknessAnimate, new PropertyPath(Border.BorderThicknessProperty));
                Storyboard ellipseStoryboard = new();
                ellipseStoryboard.Children.Add(ThicknessAnimate);
                ellipseStoryboard.Begin(BorderNamingLabel);
                //BorderNamingLabel.BeginAnimation(, )
            };

            BorderNamingLabel.MouseLeave += (sender, e) =>
            {
                ThicknessAnimate.To = new(0, 3, 0, 3);
                Storyboard.SetTargetProperty(ThicknessAnimate, new PropertyPath(Border.BorderThicknessProperty));
                Storyboard ellipseStoryboard = new();
                ellipseStoryboard.Children.Add(ThicknessAnimate);
                ellipseStoryboard.Begin(BorderNamingLabel);
            };

            BorderNamingLabel.MouseLeftButtonUp += (sender, e) =>
            {
                StartLoadSQL();
            };

            BorderNamingLabel.MouseRightButtonUp += (sender, e) =>
            {
                ScrollBar.Value = 0;
            };

            SQLLoadInformation = new(() =>
            {
                MySqlConnection Connection = new("Server=localhost; DataBase=aac20_control; Uid=root; Pwd=; charset=utf8;");
                try
                {
                    Connection.Open();
                    MySqlCommand command = new("SELECT labels.LabelConstruct FROM `labels` WHERE labels.id LIKE '%9%'", Connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    List<LabelAction> labels = [];
                    while (reader.Read())
                    {
                        string? Text = reader["LabelConstruct"].ToString();
                        if (Text == null) continue;
                        labels.Add(AACConverter.ConvertRegexToLabelAction(Text));
                    }
                    SQLLabelActions = [.. labels];
                }
                catch
                {

                }
                AnimationLoadingStop();
            });
            void ProcessLoadSQLKill(bool NewValueFlag)
            {
                if (!NewValueFlag)
                {
                    SQLLoadInformation.Kill();
                    if (SQLLabelActions.Length > 0)
                    {
                        foreach (LabelAction Element in SQLLabelActions)
                        {
                            AddLabel(Element);
                        }
                        SQLLabelActions = [];
                        SQLCompleteSearch = true;
                    }
                }
            }
            AnimateForeverLoading.ChangeStateFlag += ProcessLoadSQLKill;
        }

        internal void StartLoadSQL()
        {
            if (SearchInfoSQL.Wait || SQLCompleteSearch) return;
            SearchInfoSQL.Value = true;
            SearchInfoSQL.Wait = true;
            AnimationLoadingStart();
            SQLLoadInformation.Start();
            SearchInfoSQL.Wait = false;
            SearchInfoSQL.Value = false;
        }

        /// <summary>
        /// Добавить в страницу элемент ялрыка
        /// </summary>
        /// <param name="label">Добавляеммый элемент ярлыка</param>
        internal void AddLabel(LabelAction label)
        {
            IELLabelCommand Label = new(label, ObjectsLabel.Count)
            {
                Width = 75,
                Height = 75,
                Margin = new(0, (75 + 3) * (ObjectsLabel.Count / GridMain.ColumnDefinitions.Count) + 2, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                ContextMenu = null,
                IntervalHover = 800d,
                Opacity = 0d,
            };
            Label.OnActivateMouseLeft += () =>
            {
                App.MainWindowApplication.SummarizeCommandStateResult(ConsoleCommand.ReadAndExecuteCommand(null, [.. App.DataConsoleCommand], label.Command));
            };
            Label.OnActivateMouseRight += () =>
            {
                SelectIndexElementLabel = Label.Index;
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(SettingsPanelActionElement);
            };
            Label.MouseHover += (sender, e) =>
            {
                if (sender == null) return;
                IELLabelCommand Element = (IELLabelCommand)sender;
                string Text = Element.Label.Description ?? string.Empty;
                if (Text.Length > 0)
                    App.MainWindowApplication.IELMessageMain.UsingBorderInformation(Element, Label.Name, Text,
                        IELBlockMessage.OrientationBorderInfo.LeftDown);
            };
            Label.MouseLeave += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            Label.MouseDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            ObjectsLabel.Add(Label);
            TextBlockCount.Text = $"{ObjectsLabel.Count} Ярлыков";
            GridMain.Children.Add(Label);
            Grid.SetColumn(Label, (ObjectsLabel.Count - 1) % GridMain.ColumnDefinitions.Count);
            DoubleAnimateObj.To = 1d;
            Label.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            ScrollBar.MaxUp(1);
        }

        /// <summary>
        /// Создать анимацию загрузки
        /// </summary>
        internal void AnimationLoadingStart()
        {
            if (!AnimateForeverLoading.Wait && !AnimateForeverLoading) AnimateForeverLoading.Value = true;
            else
            {
                AnimateForeverLoading.Wait = false;
                return;
            }
            PointAnimation PointAnimate = new()
            {
                From = new(-1d, 0.5d),
                To = new(2d, 0.5d),
                Duration = TimeSpan.FromMilliseconds(1000d),
                FillBehavior = FillBehavior.Stop,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
            };
            PointAnimate.Completed += PointAnimate_Completed;
            ((RadialGradientBrush)BorderNamingLabel.BorderBrush).BeginAnimation(RadialGradientBrush.CenterProperty, PointAnimate);

            DoubleAnimateObj.To = 1d;
            ((RadialGradientBrush)BorderNamingLabel.BorderBrush).BeginAnimation(OpacityProperty, DoubleAnimateObj);

            void PointAnimate_Completed(object? sender, EventArgs e)
            {
                ((RadialGradientBrush)BorderNamingLabel.BorderBrush).BeginAnimation(RadialGradientBrush.CenterProperty,
                    AnimateForeverLoading.Wait ? null : PointAnimate);
                if (AnimateForeverLoading.Wait)
                {
                    AnimateForeverLoading.Wait = false;
                    AnimateForeverLoading.Value = false;
                }
            }
        }

        /// <summary>
        /// Остановить анимацию загрузки
        /// </summary>
        internal void AnimationLoadingStop() => AnimateForeverLoading.Wait = true;
    }
}
