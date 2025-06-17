using IEL.CORE.Classes;
using Interpreter.Interfaces;
using InterpreterCommand.Classes;
using OperPage_les.CORE.Enums;
using OperPage_les.UI.Dialogs;
using OperPage_les.UI.Pages.ActionPanel;
using OperPage_les.UI.Pages.Browser;
using OperPage_les.UI.UserElementControl;
using OperPage_les.Windows.Pages.ActionPanel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace OperPage_les.Windows.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageLabels.xaml
    /// </summary>
    public partial class PageLabels : Page
    {
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

        private readonly List<LabelAction> MainDataLabels;

        private readonly List<LabelAction> SortingDataLabels;

        /// <summary>
        /// Состояние поиска объектов по имени
        /// </summary>
        private bool SearchActivate;

        /// <summary>
        /// Массив индексов поиска ярлыков
        /// </summary>
        private int[] ArraySearchIndex;

        /// <summary>
        /// Таймер обновления поиска
        /// </summary>
        private readonly System.Timers.Timer SearchUpdateTimer;

        private SortingLabelEnum _SortingLabel;
        /// <summary>
        /// Вид сортировки для ярлыков
        /// </summary>
        internal SortingLabelEnum SortingLabelType
        {
            get => _SortingLabel;
            set
            {
                SortLabels(SortingDataLabels, value);
                _SortingLabel = value;
                UpdatePositionLabels(0);
            }
        }

        public PageLabels()
        {
            InitializeComponent();
            ArraySearchIndex = [];
            SearchActivate = false;
            IELButtonSearch.Imaging = App.LoadImage(Properties.Resources.Search);
            BorderScrollBackground.Width = 0d;
            MainDataLabels = App.CurrentApp.DataLabels;
            SortingDataLabels = [];
            SortingLabelType = SortingLabelEnum.Not;
            #region PanelAction
            #region PageLabel
            PageLabel.IELButtonCreateLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                int CountOld = MainDataLabels.Count;
                App.CurrentApp.ActivateActionCommand(null, "create_label");
                if (CountOld != MainDataLabels.Count)
                {
                    OPLLabelCommand Label = CreateVisualLabel(CountOld);
                    GridMainLabels.Children.Add(Label);
                    SortingDataLabels.Add(Label.SourceLabel);
                    SortLabels(SortingDataLabels, SortingLabelType);
                    UpdatePositionLabels(SortingLabelType != SortingLabelEnum.Not ? 0 : CountOld, false);
                    App.AnimateDoubleEffect(Label, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
                    UpdateTextInfoLabels();
                }
            };
            #endregion
            #region PageLabelElement
            PageLabelElement.IELButtonExecuteLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectLabelInPage != null)
                {
                    PageConsole? Console = App.MainWindowApplication.IELBrowserPageMain.SearchPageType<PageConsole>();
                    App.CurrentApp.ActivateActionCommand(Console, SelectLabelInPage.SourceLabel.Command);
                    SelectLabelInPage = null;
                }
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
            };
            PageLabelElement.IELButtonChangeLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
                if (SelectLabelInPage != null)
                {
                    new WindowGenLabel().ChangeLabel(SelectLabelInPage);
                    SelectLabelInPage = null;
                }
            };
            PageLabelElement.IELButtonRemoveLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectLabelInPage != null)
                {
                    RemoveLabelAt(App.CurrentApp.DataLabels.IndexOf(SelectLabelInPage.SourceLabel));
                    SelectLabelInPage = null;
                }
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
            };
            PageLabelElement.IELButtonCreateLabelTag.OnActivateMouseLeft += (sender, e, Key) =>
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
            PanelActionSettingsLabelElement = new(GridMain, PanelActionPageLabelElement, new(236d, 268d));
            #endregion
            IELButtonSorting.OnActivateMouseLeft += (sender, e, Key) =>
            {
                SortingLabelEnum[] ArrayValues = Enum.GetValues<SortingLabelEnum>();
                SortingLabelType = ArrayValues[((int)SortingLabelType + 1) % ArrayValues.Length];
            };
            IELButtonSorting.OnActivateMouseRight += (sender, e, Key) =>
            {
                SortingLabelType = SortingLabelEnum.Not;
                e.Handled = true;
            };
            #region Search Setting
            IELButtonSearch.OnActivateMouseLeft += (sender, e, Key) =>
            {
                SearchActivate = !SearchActivate;
                TextBlockLabelInfo.Text = SearchActivate ? $"Найдено ярлыков: 0 из {SortingDataLabels.Count}" : $"Ярлыков: {SortingDataLabels.Count}";
                IELButtonSearch.IELSettingObject.BackgroundSetting.SetUsedState(SearchActivate);
                if (IELTextBoxSearch.Text.Length == 0) return;
                SortLabels(SortingDataLabels, SortingLabelType);
                UpdateVisualSearchElements();
                UpdatePositionLabels(0);
            };
            SearchUpdateTimer = new()
            {
                Interval = 360d,
                AutoReset = false,
                Enabled = false,
            };
            SearchUpdateTimer.Elapsed += (sender, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    SearchUpdateTimer.Enabled = false;
                    SortLabels(SortingDataLabels, SortingLabelType);
                    UpdateVisualSearchElements();
                    UpdatePositionLabels(0);
                });
            };
            IELTextBoxSearch.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        TextBlockLabelInfo.Focus();
                        break;
                }
            };
            IELTextBoxSearch.TextChanged += (sender, e) =>
            {
                if (SearchActivate)
                {
                    SearchUpdateTimer.Enabled = true;
                    SearchUpdateTimer.Start();
                }
            };
            #endregion
            ScrollLabels.ScrollChanged += (sender, e) =>
            {
                UpdateScroll();
            };
            BorderDinamicLabels.MouseRightButtonUp += (sender, e) =>
            {
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(PanelActionSettingsLabel);
            };
            BorderDinamicLabels.MouseLeftButtonUp += (sender, e) =>
            {
                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
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
        /// Обновить визуализацию поиска
        /// </summary>
        private void UpdateVisualSearchElements()
        {
            for (int i = 0; i < GridMainLabels.Children.Count; i++)
            {
                OPLLabelCommand Element = (OPLLabelCommand)GridMainLabels.Children[i];
                if (!SearchActivate || IELTextBoxSearch.Text.Length == 0)
                {
                    Element.IELSettingObject.BackgroundSetting.SetUsedState(false);
                    continue;
                }
                else if (Element.SourceLabel.Name.Contains(IELTextBoxSearch.Text, StringComparison.CurrentCultureIgnoreCase))
                    Element.IELSettingObject.BackgroundSetting.SetUsedState(true);
                else Element.IELSettingObject.BackgroundSetting.SetUsedState(false);
            }
            UpdateTextInfoLabels();
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
        /// Обновить визуализацию прокрутки
        /// </summary>
        private void UpdatePositionLabels(int StartIndex, bool Animatable = true)
        {
            if (GridMainLabels.Children.Count == 0) return;
            BorderDinamicLabels.UpdateLayout();
            for (int i = StartIndex; i < GridMainLabels.Children.Count; i++)
            {
                OPLLabelCommand Element = (OPLLabelCommand)GridMainLabels.Children[i];
                int indexInData = SortingDataLabels.IndexOf(Element.SourceLabel);
                int CountOneLine = indexInData % CountOneLineLabel;
                int CountLine = indexInData / CountOneLineLabel;
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
        /// Обновить текст информации об ярлыках
        /// </summary>
        private void UpdateTextInfoLabels()
        {
            if (SearchActivate)
            {
                OPLLabelCommand[] ArrayLabelsElement = [..GridMainLabels.Children.Cast<OPLLabelCommand>()];
                TextBlockLabelInfo.Text = $"Найдено ярлыков: {ArrayLabelsElement.Count((i) => i.IELSettingObject.BackgroundSetting.GetUsedState())} из {SortingDataLabels.Count}";
            }
            else TextBlockLabelInfo.Text = $"Ярлыков: {SortingDataLabels.Count}";
        }

        /// <summary>
        /// Сгенерировать все объекты интерфейса ярлыка
        /// </summary>
        private void CreateAllVisualLabels()
        {
            for (int i = 0; i < SortingDataLabels.Count; i++)
            {
                OPLLabelCommand Label = CreateVisualLabel(i);
                GridMainLabels.Children.Add(Label);
                App.AnimateDoubleEffect(Label, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
                App.MainWindowApplication.UpdateLayout();
            }
            UpdateTextInfoLabels();
            //UpdatePositionLabels(0, false);
        }

        /// <summary>
        /// Сгенерировать объект интерфейса ярлыка
        /// </summary>
        private OPLLabelCommand CreateVisualLabel(int IndexData)
        {
            OPLLabelCommand Label = new(MainDataLabels[IndexData])
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Opacity = 0d,
                Width = WidthLabel,
                Height = HeightLabel,
            };
            Label.IELSettingObject.IntervalHover = 800d;
            Label.IELSettingObject.AnimationMillisecond = 230;
            Label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Label.MouseRightButtonDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            Label.OnActivateMouseRight += (sender, e, Key) =>
            {
                SelectLabelInPage = Label;
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(PanelActionSettingsLabelElement);
                PageLabelElement.UpdateVisibleTag([.. Label.SourceLabel.Tags]);
                e.Handled = true;
            };
            Label.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageConsole? Console = App.MainWindowApplication.IELBrowserPageMain.SearchPageType<PageConsole>();
                App.CurrentApp.ActivateActionCommand(Console, Label.SourceLabel.Command);
                e.Handled = true;
            };
            Label.MouseEnter += (sender, e) =>
            {
                //SelectLabelInMouse = Label;
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
            //Label.UpdateVisualStyle();
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
            MainDataLabels.Remove(SortingDataLabels[Index]);
            UpdateTextInfoLabels();
        }

        /// <summary>
        /// Отсортировать массив ярлыков
        /// </summary>
        /// <param name="SourceLabels">Массив объектов сортировки</param>
        /// <param name="StyleSort">Стиль сортировки</param>
        private void SortLabels(List<LabelAction> SourceLabels, SortingLabelEnum StyleSort)
        {
            SortingDataLabels.Clear();
            SortingDataLabels.AddRange(MainDataLabels);
            switch (StyleSort)
            {
                case SortingLabelEnum.Not:
                    //SortingDataLabels.AddRange(MainDataLabels);
                    break;
                case SortingLabelEnum.Tag:
                    SourceLabels.Sort(delegate (LabelAction x, LabelAction y)
                    {
                        if (x.Tags.Count == 0)
                        {
                            if (y.Tags.Count == 0) return 0;
                            else return -1;
                        }
                        else
                        {
                            if (y.Tags.Count == 0) return 1;
                            else return x.Tags[0].ValueTag.CompareTo(y.Tags[0].ValueTag);
                        }
                    });
                    break;
                case SortingLabelEnum.NameAZ:
                    SourceLabels.Sort(delegate (LabelAction x, LabelAction y)
                    {
                        if (x.Name.Length == 0 && y.Name.Length == 0) return 0;
                        else if (x.Name.Length == 0) return -1;
                        else if (y.Name.Length == 0) return 1;
                        else return x.Name.CompareTo(y.Name);
                    });
                    break;
                case SortingLabelEnum.NameZA:
                    SourceLabels.Sort(delegate (LabelAction x, LabelAction y)
                    {
                        if (x.Name.Length == 0 && y.Name.Length == 0) return 0;
                        else if (x.Name.Length == 0) return -1;
                        else if (y.Name.Length == 0) return 1;
                        else return x.Name.CompareTo(y.Name);
                    });
                    SourceLabels.Reverse();
                    break;
            }
            if (SearchActivate && IELTextBoxSearch.Text.Length > 0)
            {
                SourceLabels.Sort(delegate (LabelAction x, LabelAction y)
                {
                    if (x.Name.Contains(IELTextBoxSearch.Text, StringComparison.CurrentCultureIgnoreCase)) return -1;
                    else if (y.Name.Contains(IELTextBoxSearch.Text, StringComparison.CurrentCultureIgnoreCase)) return 0;
                    else return 0;
                });
            }
            IELButtonSorting.Imaging = StyleSort switch
            {
                SortingLabelEnum.Not => App.LoadImage(Properties.Resources.Sorting_Not),
                SortingLabelEnum.Tag => App.LoadImage(Properties.Resources.Sorting_Tag),
                SortingLabelEnum.NameAZ => App.LoadImage(Properties.Resources.Sorting_NameAZ),
                SortingLabelEnum.NameZA => App.LoadImage(Properties.Resources.Sorting_NameZA),
                _ => App.LoadImage(Properties.Resources.Sorting_Not)
            };
        }
    }
}
