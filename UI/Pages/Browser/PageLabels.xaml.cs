using DataScroll;
using IEL;
using IEL.CORE.Classes;
using IEL.CORE.Enums;
using Interpreter.Interfaces;
using OperPage_les.CORE.Flaging;
using OperPage_les.UI.Dialogs;
using OperPage_les.UI.Pages.ActionPanel;
using OperPage_les.UI.Pages.Browser;
using OperPage_les.UI.UserElementControl;
using OperPage_les.Windows.Pages.ActionPanel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace OperPage_les.Windows.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageLabels.xaml
    /// </summary>
    public partial class PageLabels : Page
    {
        /// <summary>
        /// Динамический массив ярлыков
        /// </summary>
        private readonly List<LabelCommand> ObjectsLabel;

        /// <summary>
        /// Индекс выделенного элемента
        /// </summary>
        private int SelectIndexElementLabel = -1;

        #region PanelAction
        #region Source
        /// <summary>
        /// Страница взаимодействия с ярлыками
        /// </summary>
        private static readonly PageLabelMainActionPanel PageLabel = new();

        /// <summary>
        /// Страница элемента ярлыка в панели действий
        /// </summary>
        private static readonly PageLabelElementActionPanel PageLabelElement = new();
        #endregion
        /// <summary>
        /// Настройки панели действий для страниц во вкладке ярлыков
        /// </summary>
        private readonly PanelActionSettingVisual PanelActionSettingsLabel;

        /// <summary>
        /// Настройки панели действий для страниц объекта ярлыка
        /// </summary>
        private readonly PanelActionSettingVisual PanelActionSettingsLabelElement;

        /// <summary>
        /// Страница панели действий взаимодействия с ярлыками
        /// </summary>
        private readonly PagePanelAction PanelActionPageLabel = new(PageLabel);

        /// <summary>
        /// Страница панели действий взаимодействия с объектом ярлыка
        /// </summary>
        private readonly PagePanelAction PanelActionPageLabelElement = new(PageLabelElement);
        #endregion

        /// <summary>
        /// Узнать количество созданных ярлыков
        /// </summary>
        internal int CountLabel => ObjectsLabel.Count;

        /// <summary>
        /// Возможен ли повтор анимации загрузки
        /// </summary>
        private readonly Flag AnimateForeverLoading = new(false);

        /// <summary>
        /// Константа размера одного ярлыка
        /// </summary>
        public const int WidthLabel = 165;

        /// <summary>
        /// Константа размера одного ярлыка
        /// </summary>
        public const int HeightLabel = 130;

        /// <summary>
        /// Константа отступа одного ярлыка
        /// </summary>
        public const int MarginLabel = 2;

        /// <summary>
        /// Константа размера одного ярлыка с отступом
        /// </summary>
        public const int FULL_WidthLabel = WidthLabel + (MarginLabel * 2);

        /// <summary>
        /// Константа размера одного ярлыка с отступом
        /// </summary>
        public const int FULL_HeightLabel = HeightLabel + (MarginLabel * 2);

        /// <summary>
        /// Количество объектов в одной линии
        /// </summary>
        public int CountOneLineLabel => (int)(BorderDinamicLabels.ActualWidth / FULL_WidthLabel);

        /// <summary>
        /// Настройка отображения элементов списка ярлыков
        /// </summary>
        private readonly static BrushSettingQ BorderForegroundSetting = new(new byte[,]
                        {
                        { 255, 150, 31, 96 },
                        { 255, 243, 164, 207 },
                        { 255, 243, 164, 207 },
                        { 255, 248, 218, 233 },
                        });

        /// <summary>
        /// Настройка отображения элементов списка ярлыков
        /// </summary>
        private readonly static BrushSettingQ BackgroundSetting = new(new byte[,]
                        {
                        { 255, 243, 164, 207 },
                        { 255, 173, 97, 138 },
                        { 255, 243, 136, 194 },
                        { 255, 190, 166, 181 },
                        });

        public PageLabels()
        {
            InitializeComponent();
            ObjectsLabel = [];
            BorderScrollBackground.Width = 0d;

            #region PanelAction
            #region PageLabel
            PageLabel.IELButtonCreateLabel.OnActivateMouseLeft += (Key) =>
            {
                App.CurrentApp.ActivateActionCommand(null, "create_label");
            };
            #endregion
            #region PageLabelElement
            PageLabelElement.IELButtonExecuteLabel.OnActivateMouseLeft += (Key) =>
            {
                ObjectsLabel[SelectIndexElementLabel].OnActivateMouseLeft?.Invoke();
                SelectIndexElementLabel = -1;
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
            };
            PageLabelElement.IELButtonChangeLabel.OnActivateMouseLeft += (Key) =>
            {
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
                ObjectsLabel[SelectIndexElementLabel].Label =
                    new WindowGenLabel().ChangeLabel(ObjectsLabel[SelectIndexElementLabel].Label);
                SelectIndexElementLabel = -1;
            };
            PageLabelElement.IELButtonRemoveLabel.OnActivateMouseLeft += (Key) =>
            {
                RemoveLabelAt(SelectIndexElementLabel);
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
                SelectIndexElementLabel = -1;
            };
            #endregion
            PanelActionPageLabel.IsKeyboardModeChanged += (Source, NewValue) =>
            {
                PageLabel.IELButtonCreateLabel.CharKeyboardActivate = NewValue;
            };
            PanelActionSettingsLabel = new(this, PanelActionPageLabel, new(210d, 220d));

            PanelActionPageLabelElement.IsKeyboardModeChanged += (Source, NewValue) =>
            {
                PageLabelElement.IELButtonExecuteLabel.CharKeyboardActivate = NewValue;
                PageLabelElement.IELButtonChangeLabel.CharKeyboardActivate = NewValue;
                PageLabelElement.IELButtonRemoveLabel.CharKeyboardActivate = NewValue;
            };
            PanelActionSettingsLabelElement = new(GridMain, PanelActionPageLabelElement, new(150d, 190d));
            #endregion

            BorderNamingLabel.MouseRightButtonUp += (sender, e) =>
            {
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(PanelActionSettingsLabel);
            };
            BorderNamingLabel.MouseLeftButtonUp += (sender, e) =>
            {
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
            };

        }

        /// <summary>
        /// Сгенерировать объект интерфейса ярлыка
        /// </summary>
        /// <param name="label">Объект ссылки на данные ярлыка</param>
        /// <param name="Data">Массив ярлыков</param>
        /// <param name="grid">Контейнер нахождения ярлыка</param>
        /// <returns>Объект интерфейса ярлыка</returns>
        private LabelCommand CreateLabel(LabelAction label)
        {
            byte[] ByteLabelImage = ICommandOPER.ReadNameCommand(label.Command) switch
            {
                "open_link" => Properties.Resources.Link,
                "open_file" => Properties.Resources.File,
                "open_directory" => Properties.Resources.Folder,
                _ => Properties.Resources.Command
            };
            LabelCommand Label = new(label, ObjectsLabel.Count)
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                ImageSource = App.LoadImage(ByteLabelImage),
                IELSettingObject = new()
                {
                    IntervalHover = 800d,
                    AnimationMillisecond = 300,
                    BackgroundSetting = BackgroundSetting,
                    BorderBrushSetting = BorderForegroundSetting,
                    ForegroundSetting = BorderForegroundSetting,
                }
            };
            Label.OnActivateMouseLeft += (Key) =>
            {
                PageConsole? Console = App.MainWindowApplication.IELBrowserPageMain.SearchPageType<PageConsole>();
                App.CurrentApp.ActivateActionCommand(Console, Label.Label.Command);
            };
            Label.IELSettingObject.MouseHover += (sender, e) =>
            {
                if (sender == null) return;
                string Text = Label.Label.Description ?? string.Empty;
                if (Text.Length > 0)
                    App.MainWindowApplication.IELMessageMain.UsingBorderInformation(Label, Text,
                        OrientationBorderPosition.Auto);
            };
            Label.MouseLeave += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            Label.MouseLeftButtonDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            return Label;
        }

        ///// <summary>
        ///// Добавить в страницу SQL элемент ялрыка
        ///// </summary>
        ///// <param name="label">Добавляеммый элемент ярлыка</param>
        //internal void AddSQLLabel(LabelAction label)
        //{
        //    IELLabelCommand Label = CreateLabel(label, ref ObjectsSQLLabel, ref GridSQLLabels);
        //    Label.ImageTagSource = new BitmapImage(new Uri($"{App.PathImageApplication}/Wifi.png", UriKind.RelativeOrAbsolute));
        //    Label.ImageTagVisible = true;
        //    ObjectsSQLLabel.Add(Label);
        //    GridSQLLabels.Children.Add(Label);
        //    Grid.SetColumn(Label, (ObjectsSQLLabel.Count - 1) % GridSQLLabels.ColumnDefinitions.Count);

        //    TextBlockCount.Text = $"{CountLabel} Ярлыков";
        //    DoubleAnimateObj.To = 1d;
        //    Label.BeginAnimation(OpacityProperty, DoubleAnimateObj);
        //    ScrollBar.MaxUp(1);
        //}

        /// <summary>
        /// Добавить в страницу элемент ялрыка
        /// </summary>
        /// <param name="label">Добавляеммый элемент ярлыка</param>
        internal void AddLabel(LabelAction label)
        {
            LabelCommand Label = CreateLabel(label);
            Label.Opacity = 0d;
            Label.Width = WidthLabel;
            Label.Height = HeightLabel;
            Label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Label.MouseRightButtonDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            Label.OnActivateMouseRight += (Key) =>
            {
                SelectIndexElementLabel = Label.Index;
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(PanelActionSettingsLabelElement);
            };
            int CountOneLine = CountLabel % CountOneLineLabel;
            int CountLine = CountLabel / CountOneLineLabel;
            int Left = CountOneLine == 0 ? MarginLabel : CountOneLine * FULL_WidthLabel;
            int Top = CountLine == 0 ? MarginLabel : CountLine * FULL_HeightLabel;
            Label.Margin = new(Left, Top, 0, 0);
            ObjectsLabel.Add(Label);
            GridMainLabels.Children.Add(Label);
            //StackPanelLabels.Children.Add(Label);

            TextBlockCount.Text = $"ярлыков: {CountLabel}";
            App.AnimateDoubleEffect(Label, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
        }

        /// <summary>
        /// Удалить ярлык по индексу
        /// </summary>
        /// <param name="Index">индекс удаляемого ярлыка</param>
        private void RemoveLabelAt(int Index)
        {
            DoubleAnimation animation = App.GetDoubleAnimate(TimeSpan.FromMilliseconds(120d));
            animation.To = 0d;
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (sender, e) =>
            {
                GridMainLabels.Children.RemoveAt(Index);
            };
            LabelCommand Label = ObjectsLabel[Index];
            Label.BeginAnimation(OpacityProperty, animation);
            ObjectsLabel.RemoveAt(Index);
            for (int i = Index; i < ObjectsLabel.Count; i++)
            {
                int CountOneLine = i % CountOneLineLabel;
                int CountLine = i / CountOneLineLabel;
                int Left = CountOneLine == 0 ? MarginLabel : CountOneLine * FULL_WidthLabel;
                int Top = CountLine == 0 ? MarginLabel : CountLine * FULL_HeightLabel;
                App.AnimateThicknessEffect(ObjectsLabel[i], MarginProperty, new(Left, Top, 0, 0), TimeSpan.FromMilliseconds(270d));
                ObjectsLabel[i].Index = i;
            }
            TextBlockCount.Text = $"ярлыков: {CountLabel}";
        }
    }
}
