using AAC20.Classes;
using AAC20.Windows.Pages.ActionPanel;
using Interpreter.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using IEL.Classes;
using IEL;
using IEL.Interfaces.Core;

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
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(250d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut }
        };

        public PageLabels()
        {
            InitializeComponent();
            ModulePage = new(nameof(PageLabels));
            ScrollBar = new(10, TrafficShare: 2);
            SettingsPanelActionElement = new(GridMain, PageLabelActPanel, new(110, 130));
            ObjectsLabel = [];
            /*PageLabelActPanel.IELButtonExecuteLabel.OnActivateMouseLeft += (Key) =>
            {
                ObjectsLabel[SelectIndexElementLabel].OnActivateMouseLeft?.Invoke();
            };*/

            GridMain.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });
            GridMain.ColumnDefinitions.Add(new() { Width = new GridLength(90d, GridUnitType.Star) });

            ScrollBar.ChangedValue += (NewValue) =>
            {
                ThicknessAnimate.To = new(0, 0 - (75 + 9) * NewValue, 0, 0);
                GridMain.BeginAnimation(MarginProperty, ThicknessAnimate);
            };
            GridMain.MouseWheel += (sender, e) =>
            {
                if (ScrollBar.MaxValue > 0 && ObjectsLabel.Count > 0)
                {
                    if (e.Delta > 0) ScrollBar.Up();
                    else if (e.Delta < 0) ScrollBar.Down();
                }
            };
        }

        internal void AddLabel(LabelAction label)
        {
            IELLabelCommand Label = new(label, ObjectsLabel.Count)
            {
                Width = 75,
                Height = 75,
                Margin = new(0, (75 + 9) * (ObjectsLabel.Count / GridMain.ColumnDefinitions.Count) + 4, 0, 0),
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
                //App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(SettingsPanelActionElement);
            };
            Label.MouseHover += (sender, e) =>
            {
                if (sender == null) return;
                IELLabelCommand Element = (IELLabelCommand)sender;
                string Text = Element.Label.Description ?? string.Empty;
                //if (Text.Length > 0)
                //    App.MainWindowApplication.IELMessageMain.UsingBorderInformation(Element, Label.Name, Text,
                //        IELBlockMessage.OrientationBorderInfo.LeftDown);
            };
            //Label.MouseLeave += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            //Label.MouseDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            ObjectsLabel.Add(Label);
            GridMain.Children.Add(Label);
            Grid.SetColumn(Label, (ObjectsLabel.Count - 1) % GridMain.ColumnDefinitions.Count);
            DoubleAnimateObj.To = 1d;
            Label.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            ScrollBar.MaxUp(1);
        }
    }
}
