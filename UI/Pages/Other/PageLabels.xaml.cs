using AAC20.CORE;
using AAC20.Windows.Pages.ActionPanel;
using Interpreter.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using IEL.Classes;
using IEL;
using IEL.Interfaces.Core;
using System.Windows.Media;
using AAC20.CORE.Flaging;
using MySql.Data.MySqlClient;
using System.Windows.Media.Imaging;
using DataScroll;

namespace AAC20.Windows.Pages.Other
{
    /// <summary>
    /// Логика взаимодействия для PageLabels.xaml
    /// </summary>
    public partial class PageLabels : Page, IPageDefault
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageLabels);

        /// <summary>
        /// Динамический массив ярлыков
        /// </summary>
        private List<IELLabelCommand> ObjectsLabel;

        /// <summary>
        /// Динамический массив ярлыков
        /// </summary>
        private List<IELLabelCommand> ObjectsSQLLabel;

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
        /// Узнать количество созданных ярлыков
        /// </summary>
        internal int CountLabel => ObjectsLabel.Count + ObjectsSQLLabel.Count;

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

        /// <summary>
        /// Константа размера одного ярлыка
        /// </summary>
        public const int WidthHeightLabel = 77;

        public PageLabels()
        {
            InitializeComponent();
            RowDefinitionSQLLabels.Height = new(0, GridUnitType.Star);
            BorderScrollBackground.Width = 0d;
            SettingsPanelActionElement = new(GridMain, PageLabelActPanel, new(150, 140));
            ((RadialGradientBrush)BorderNamingLabel.BorderBrush).Center = new(-1d, 0.5d);
            SQLLabelActions = [];
            ObjectsLabel = [];
            ObjectsSQLLabel = [];
            PageLabelActPanel.IELButtonExecuteLabel.OnActivateMouseLeft += (Key) =>
            {
                ObjectsLabel[SelectIndexElementLabel].OnActivateMouseLeft?.Invoke();
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
            };

            GridDinamicLabels.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });
            GridDinamicLabels.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });
            GridDinamicLabels.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });

            GridSQLLabels.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });
            GridSQLLabels.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });
            GridSQLLabels.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });

            ScrollBar = new(12, TrafficShare: (ushort)GridDinamicLabels.ColumnDefinitions.Count);

            ScrollBar.ChangedValue += (NewValue) =>
            {
                ThicknessAnimate.To = new(0, 0 - (WidthHeightLabel + BorderDinamicLabels.Padding.Top) * NewValue, 0, 0);
                GridMain.BeginAnimation(MarginProperty, ThicknessAnimate);
                DoubleAnimateObj.To = ActualWidth / (int)(ScrollBar.MaxValue) * NewValue;
                BorderScrollBackground.BeginAnimation(WidthProperty, DoubleAnimateObj);
            };
            MouseWheel += (sender, e) =>
            {
                if (ScrollBar.ScrollActivate && CountLabel > 0)
                {
                    if (App.MainWindowApplication.IELActionPanelMain.NameFrameElement.Equals(SettingsPanelActionElement.ElementInPanel.Name))
                    {
                        SelectIndexElementLabel = -1;
                        App.MainWindowApplication.IELActionPanelMain.ClosePanelAction(IELPanelAction.PositionAnimActionPanel.CenterObject);
                    }
                    if (e.Delta > 0) ScrollBar.Up();
                    else if (e.Delta < 0) ScrollBar.Down();
                }
            };

            BorderNamingLabel.MouseEnter += (sender, e) =>
            {
                ThicknessAnimate.To = new(0, 0, 0, 7);
                Storyboard.SetTargetProperty(ThicknessAnimate, new PropertyPath(Border.BorderThicknessProperty));
                Storyboard ellipseStoryboard = new();
                ellipseStoryboard.Children.Add(ThicknessAnimate);
                ellipseStoryboard.Begin(BorderNamingLabel);
            };

            BorderNamingLabel.MouseLeave += (sender, e) =>
            {
                ThicknessAnimate.To = new(0, 0, 0, 4);
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
                MySqlConnection Connection = new("Server=localhost; DataBase=aac_control; Uid=root; Pwd=; charset=utf8;");
                try
                {
                    Connection.Open();
                    MySqlCommand command = new("SELECT labels.LabelConstruct FROM `labels` WHERE labels.Id LIKE '%9%'", Connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    List<LabelAction> labels = [];
                    while (reader.Read())
                    {
                        string? Text = reader["LabelConstruct"].ToString();
                        if (Text == null) continue;
                        foreach (LabelAction Element in AACConverter.ConvertRegexToMassLabelAction(Text))
                        {
                            labels.Add(Element);
                        }
                    }
                    SQLLabelActions = [.. labels];
                }
                catch
                {

                }
                AnimationLoadingStop();
            });

            SizeChanged += (sender, e) =>
            {
                if (GridMain.ActualHeight == 0d) return;
                int ScrollCountVisible = (int)(ActualHeight / (WidthHeightLabel + BorderDinamicLabels.Padding.Top)) * ScrollBar.TrafficShare;
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
                }
            };

            void ProcessLoadSQLKill(bool NewValueFlag)
            {
                if (!NewValueFlag)
                {
                    SQLLoadInformation.Kill();
                    if (SQLLabelActions.Length > 0)
                    {
                        BorderDinamicLabels.BorderThickness = new(0, 1, 0, 0);
                        BorderSQLLabels.BorderThickness = new(0, 0, 0, 1);
                        RowDefinitionSQLLabels.Height = new(
                            SQLLabelActions.Length / 2 * (WidthHeightLabel + BorderDinamicLabels.Padding.Top) + BorderSQLLabels.Padding.Bottom,
                            GridUnitType.Pixel);
                        foreach (LabelAction Element in SQLLabelActions)
                        {
                            AddSQLLabel(Element);
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
        /// Сгенерировать объект интерфейса ярлыка
        /// </summary>
        /// <param name="label">Объект ссылки на данные ярлыка</param>
        /// <param name="Data">Массив ярлыков</param>
        /// <param name="grid">Контейнер нахождения ярлыка</param>
        /// <returns>Объект интерфейса ярлыка</returns>
        private IELLabelCommand CreateLabel(LabelAction label, ref List<IELLabelCommand> Data, ref Grid grid)
        {
            string NameFileLabelImage = ConsoleCommand.ReadNameCommand(label.Command) switch
            {
                "open_link" => "Link.png",
                "open_file" => "File.png",
                "open_directory" => "Folder.png",
                _ => "Command.png"
            };
            Uri UriIconLabel = new($@"C:\Users\killm\Рабочий стол\Main\Programm\С#\AAC20\Windows\WindowsImages\Labels\{NameFileLabelImage}");
            IELLabelCommand Label = new(label, Data.Count)
            {
                Width = WidthHeightLabel,
                Height = WidthHeightLabel,
                Margin = new(0, (WidthHeightLabel + BorderDinamicLabels.Padding.Top) * (Data.Count / grid.ColumnDefinitions.Count), 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                ContextMenu = null,
                IntervalHover = 800d,
                Opacity = 0d,
                ImageSource = new BitmapImage(UriIconLabel),
                AnimationMillisecond = 300,
                BorderThicknessBlock = new(2),
            };
            Label.OnActivateMouseLeft += () =>
            {
                App.MainWindowApplication.SummarizeCommandStateResult(ConsoleCommand.ReadAndExecuteCommand(null, [.. App.DataConsoleCommand], label.Command));
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
            Label.MouseLeftButtonDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            return Label;
        }

        /// <summary>
        /// Добавить в страницу SQL элемент ялрыка
        /// </summary>
        /// <param name="label">Добавляеммый элемент ярлыка</param>
        internal void AddSQLLabel(LabelAction label)
        {
            IELLabelCommand Label = CreateLabel(label, ref ObjectsSQLLabel, ref GridSQLLabels);
            Label.ImageTagSource = new BitmapImage(new Uri(@"C:\Users\killm\Рабочий стол\Main\Programm\С#\AAC20\Windows\WindowsImages\Wifi.png"));
            Label.ImageTagVisible = true;
            ObjectsSQLLabel.Add(Label);
            GridSQLLabels.Children.Add(Label);
            Grid.SetColumn(Label, (ObjectsSQLLabel.Count - 1) % GridSQLLabels.ColumnDefinitions.Count);

            TextBlockCount.Text = $"{CountLabel} Ярлыков";
            DoubleAnimateObj.To = 1d;
            Label.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            ScrollBar.MaxUp(1);
        }

        /// <summary>
        /// Добавить в страницу элемент ялрыка
        /// </summary>
        /// <param name="label">Добавляеммый элемент ярлыка</param>
        internal void AddLabel(LabelAction label)
        {
            IELLabelCommand Label = CreateLabel(label, ref ObjectsLabel, ref GridDinamicLabels);
            Label.MouseRightButtonDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            Label.OnActivateMouseRight += () =>
            {
                SelectIndexElementLabel = Label.Index;
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(SettingsPanelActionElement);
            };
            ObjectsLabel.Add(Label);
            GridDinamicLabels.Children.Add(Label);
            Grid.SetColumn(Label, (ObjectsLabel.Count - 1) % GridDinamicLabels.ColumnDefinitions.Count);

            TextBlockCount.Text = $"{CountLabel} Ярлыков";
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
