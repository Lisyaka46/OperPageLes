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
        /// Индекс выделенного элемента курсором мыши
        /// </summary>
        private OPLLabelCommand? SelectLabelInMouse;

        /// <summary>
        /// Индекс выделенного элемента панелью
        /// </summary>
        private OPLLabelCommand? SelectLabelInPage;

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
        /// Константа размера одного ярлыка
        /// </summary>
        public const int WidthLabel = 145;

        /// <summary>
        /// Константа размера одного ярлыка
        /// </summary>
        public const int HeightLabel = 115;

        /// <summary>
        /// Константа отступа одного ярлыка
        /// </summary>
        public const int MarginLabel = 4;

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
        private int CountOneLineLabel => (int)(BorderDinamicLabels.ActualWidth / FULL_WidthLabel);

        /// <summary>
        /// Сохранённое значение количества ярлыков в одной линии
        /// </summary>
        private int SaveCountOneLineLabel = 0;

        ///// <summary>
        ///// Настройка отображения элементов списка ярлыков
        ///// </summary>
        //private readonly static BrushSettingQ BorderForegroundSetting = new(new byte[,]
        //                {
        //                { 255, 150, 31, 96 },
        //                { 255, 243, 164, 207 },
        //                { 255, 243, 164, 207 },
        //                { 255, 248, 218, 233 },
        //                });

        ///// <summary>
        ///// Настройка отображения элементов списка ярлыков
        ///// </summary>
        //private readonly static BrushSettingQ BackgroundSetting = new(new byte[,]
        //                {
        //                { 255, 243, 164, 207 },
        //                { 255, 173, 97, 138 },
        //                { 255, 243, 136, 194 },
        //                { 255, 190, 166, 181 },
        //                });
        #region Styles
        internal static readonly BrushSettingQ[] BackgroundStyles =
        [
            new(new byte[,]
                        {
                        { 255, 116, 220, 80 },
                        { 255, 180, 255, 154 },
                        { 255, 196, 239, 201 },
                        { 255, 222, 87, 87 },
                        }),
            new(new byte[,]
                        {
                        { 255, 155, 179, 169 },
                        { 255, 160, 200, 175 },
                        { 255, 221, 254, 241 },
                        { 255, 111, 127, 121 },
                        }),
            new(new byte[,]
                        {
                        { 255, 239, 250, 195 },
                        { 255, 240, 246, 210 },
                        { 255, 195, 218, 250 },
                        { 255, 250, 201, 195 },
                        }),
            new(new byte[,]
                        {
                        { 255, 85, 150, 181 },
                        { 255, 90, 140, 185 },
                        { 255, 181, 137, 85 },
                        { 255, 84, 107, 117 },
                        }),
        ];
        internal static readonly BrushSettingQ[] Borderbrush_Foreground_Styles =
        [
            new(new byte[,]
                        {
                        { 255, 0, 0, 0 },
                        { 255, 19, 35, 12 },
                        { 255, 47, 44, 9 },
                        { 255, 58, 8, 8 },
                        }),
            new(new byte[,]
                        {
                        { 255, 62, 96, 82 },
                        { 255, 65, 100, 85 },
                        { 255, 100, 154, 133 },
                        { 255, 38, 68, 57 },
                        }),
            new(new byte[,]
                        {
                        { 255, 126, 139, 73 },
                        { 255, 130, 150, 69 },
                        { 255, 79, 110, 152 },
                        { 255, 153, 100, 94 },
                        }),
            new(new byte[,]
                        {
                        { 255, 24, 86, 116 },
                        { 255, 30, 83, 107 },
                        { 255, 109, 72, 28 },
                        { 255, 24, 47, 56 },
                        }),
        ];
        #endregion

        public PageLabels()
        {
            InitializeComponent();
            BorderScrollBackground.Width = 0d;

            #region PanelAction
            #region PageLabel
            PageLabel.IELButtonCreateLabel.OnActivateMouseLeft += (sender, Key) =>
            {
                int CountOld = App.CurrentApp.DataLabels.Count;
                App.CurrentApp.ActivateActionCommand(null, "create_label");
                if (CountOld != App.CurrentApp.DataLabels.Count)
                {
                    OPLLabelCommand Label = CreateVisualLabel(CountOld);
                    GridMainLabels.Children.Add(Label);
                    UpdatePositionLabels(CountOld, false);
                    App.AnimateDoubleEffect(Label, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
                    TextBlockCount.Text = $"ярлыков: {App.CurrentApp.DataLabels.Count}";
                }
            };
            #endregion
            #region PageLabelElement
            PageLabelElement.IELButtonExecuteLabel.OnActivateMouseLeft += (sender, Key) =>
            {
                if (SelectLabelInPage != null)
                {
                    PageConsole? Console = App.MainWindowApplication.IELBrowserPageMain.SearchPageType<PageConsole>();
                    App.CurrentApp.ActivateActionCommand(Console, SelectLabelInPage.SourceLabel.Command);
                    SelectLabelInPage = null;
                }
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
            };
            PageLabelElement.IELButtonChangeLabel.OnActivateMouseLeft += (sender, Key) =>
            {
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
                if (SelectLabelInPage != null)
                {
                    new WindowGenLabel().ChangeLabel(SelectLabelInPage.SourceLabel);
                    SelectLabelInPage = null;
                }
            };
            PageLabelElement.IELButtonRemoveLabel.OnActivateMouseLeft += (sender, Key) =>
            {
                if (SelectLabelInPage != null)
                {
                    RemoveLabelAt(App.CurrentApp.DataLabels.IndexOf(SelectLabelInPage.SourceLabel));
                    SelectLabelInPage = null;
                }
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
            };
            PageLabelElement.IELButtonCreateLabelTag.OnActivateMouseLeft += (sender, Key) =>
            {
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
                if (SelectLabelInPage != null)
                {
                    new WindowManipulateLabelTags(SelectLabelInPage).ShowDialog();
                    //SelectLabelInPage.AppendTag(new("Tag"));
                    SelectLabelInPage = null;
                }
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
                PageLabelElement.IELButtonCreateLabelTag.CharKeyboardActivate = NewValue;
            };
            PanelActionSettingsLabelElement = new(GridMain, PanelActionPageLabelElement, new(230d, 236d));
            #endregion

            MouseRightButtonUp += (sender, e) =>
            {
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(PanelActionSettingsLabel);
            };
            MouseLeftButtonUp += (sender, e) =>
            {
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
            };
            ScrollLabels.ScrollChanged += (sender, e) =>
            {
                UpdateScroll();
            };
            SizeChanged += (sender, e) =>
            {
                if (SaveCountOneLineLabel == CountOneLineLabel) return;
                SaveCountOneLineLabel = CountOneLineLabel;
                UpdatePositionLabels(0);
            };
            CreateAllVisualLabels();
        }

        /// <summary>
        /// Обновить визуализацию прокрутки
        /// </summary>
        private void UpdatePositionLabels(int StartIndex, bool Animatable = true)
        {
            if (GridMainLabels.Children.Count == 0) return;
            BorderDinamicLabels.UpdateLayout();
            for (int i = StartIndex; i < GridMainLabels.Children.Count; i++)
            {
                OPLLabelCommand Element = (OPLLabelCommand)GridMainLabels.Children[i];
                int CountOneLine = i % CountOneLineLabel;
                int CountLine = i / CountOneLineLabel;
                int Left = CountOneLine == 0 ? MarginLabel : CountOneLine * FULL_WidthLabel;
                int Top = CountLine == 0 ? MarginLabel : CountLine * FULL_HeightLabel;
                if (Element.Margin.Left == Left && Element.Margin.Top == Top) continue;
                if (Animatable)
                {
                    ThicknessAnimation animation = App.GetThicknessAnimate(TimeSpan.FromMilliseconds(Element.IELSettingObject.AnimationMillisecond));
                    animation.To = new(Left, Top, 0, 0);
                    animation.FillBehavior = FillBehavior.Stop;
                    animation.Completed += (sender, e) =>
                    {
                        Element.Margin = new(Left, Top, 0, 0);
                    };
                    Element.ApplyAnimationClock(MarginProperty, animation.CreateClock());
                }
                else Element.Margin = new(Left, Top, 0, 0);
            }
        }

        /// <summary>
        /// Обновить визуализацию прокрутки
        /// </summary>
        private void UpdateScroll()
        {
            double Offset = 0d;
            if (ScrollLabels.ScrollableHeight > 0d)
            {
                double One = BorderNamingLabel.ActualWidth / ScrollLabels.ScrollableHeight;
                Offset = One * ScrollLabels.VerticalOffset;
            }
            App.AnimateDoubleEffect(BorderScrollBackground, WidthProperty, Offset, TimeSpan.FromMilliseconds(300d));
        }

        /// <summary>
        /// Сгенерировать все объекты интерфейса ярлыка
        /// </summary>
        private void CreateAllVisualLabels()
        {
            for (int i = 0; i < App.CurrentApp.DataLabels.Count; i++)
            {
                OPLLabelCommand Label = CreateVisualLabel(i);
                GridMainLabels.Children.Add(Label);
                App.AnimateDoubleEffect(Label, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
                App.MainWindowApplication.UpdateLayout();
            }
            TextBlockCount.Text = $"ярлыков: {App.CurrentApp.DataLabels.Count}";
            //UpdatePositionLabels(0, false);
        }

        /// <summary>
        /// Сгенерировать объект интерфейса ярлыка
        /// </summary>
        private OPLLabelCommand CreateVisualLabel(int IndexData)
        {
            string name_command = ICommandOPER.ReadNameCommand(App.CurrentApp.DataLabels[IndexData].Command);
            byte[] ByteLabelImage;
            int IndexUseStyle;
            switch(name_command)
            {
                case "open_link":
                    IndexUseStyle = 2;
                    ByteLabelImage = Properties.Resources.Link;
                    break;
                case "open_file":
                    IndexUseStyle = 1;
                    ByteLabelImage = Properties.Resources.File;
                    break;
                case "open_directory":
                    IndexUseStyle = 3;
                    ByteLabelImage = Properties.Resources.Folder;
                    break;
                default:
                    IndexUseStyle = 0;
                    ByteLabelImage = Properties.Resources.Command;
                    break;
            };
            OPLLabelCommand Label = new(App.CurrentApp.DataLabels[IndexData])
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                ImageSource = App.LoadImage(ByteLabelImage),
                IELSettingObject = new()
                {
                    IntervalHover = 800d,
                    AnimationMillisecond = 230,
                    BackgroundSetting = BackgroundStyles[IndexUseStyle],
                    BorderBrushSetting = Borderbrush_Foreground_Styles[IndexUseStyle],
                    ForegroundSetting = Borderbrush_Foreground_Styles[IndexUseStyle],
                },
                Opacity = 0d,
                ColumnWidth = new()
                {
                    Width = new(WidthLabel)
                },
                Width = WidthLabel + 10,
                Height = HeightLabel,
            };
            Label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Label.MouseRightButtonDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            Label.OnActivateMouseRight += (sender, Key) =>
            {
                SelectLabelInPage = Label;
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(PanelActionSettingsLabelElement);
            };
            Label.OnActivateMouseLeft += (sender, Key) =>
            {
                PageConsole? Console = App.MainWindowApplication.IELBrowserPageMain.SearchPageType<PageConsole>();
                App.CurrentApp.ActivateActionCommand(Console, Label.SourceLabel.Command);
            };
            Label.MouseEnter += (sender, e) =>
            {
                SelectLabelInMouse = Label;
            };
            Label.IELSettingObject.MouseHover += (sender, e) =>
            {
                if (sender == null) return;
                string Text = Label.SourceLabel.Description ?? string.Empty;
                if (Text.Length > 0)
                    App.MainWindowApplication.IELMessageMain.UsingBorderInformation(Label, Text,
                        IEL.CORE.Enums.OrientationBorderPosition.Auto);
            };
            Label.MouseLeave += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            Label.MouseLeftButtonDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            return Label;
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
                UpdatePositionLabels(Index);
            };
            UIElement Label = GridMainLabels.Children[Index];
            Label.BeginAnimation(OpacityProperty, animation);
            App.CurrentApp.DataLabels.RemoveAt(Index);
            TextBlockCount.Text = $"ярлыков: {App.CurrentApp.DataLabels.Count}";
        }
    }
}
