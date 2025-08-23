using IEL.CORE.Classes;
using IEL.CORE.Enums;
using OperPage_les.CORE.Enums;
using OperPage_les.UI.Dialogs;
using OperPage_les.UI.Pages.ActionPanel.PageLabel;
using OperPage_les.UI.UserElementControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace OperPage_les.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageLabels.xaml
    /// </summary>
    public partial class PageLabels : Page
    {
        /// <summary>
        /// Выделенный элемент панелью действий
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

        /// <summary>
        /// Состояние поиска объектов по имени
        /// </summary>
        private bool SearchActivate;

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
                SortLabels(App.CurrentApp.DataLabels, value);
                _SortingLabel = value;
                UpdatePositionLabels(0);
            }
        }

        private bool _SelectLabelsMode = false;
        /// <summary>
        /// Режим выделения ярлыков для манипуляции
        /// </summary>
        internal bool SelectLabelsMode
        {
            get => _SelectLabelsMode;
            private set
            {
                PageLabel.IELButtonSelectLabel.Text = value ? "Выйти из режима выделения" : "Режим выделения";
                PageLabel.IELButtonSelectLabel.FontSize = value ? 12d : 16d;

                PageLabelElement.IELButtonExecuteLabel.IsEnabled = !value;
                PageLabelElement.IELButtonChangeLabel.IsEnabled = !value;
                PageLabelElement.IELButtonRemoveLabel.Text = value ? "Удалить выделенное" : "Удалить";
                PageLabelElement.IELButtonCreateLabelTag.IsEnabled = !value;

                PageLabel.IELButtonCreateLabel.IsEnabled = !value;

                _SelectLabelsMode = value;
            }
        }

        public PageLabels()
        {
            InitializeComponent();
            SearchActivate = false;
            IELButtonSearch.Imaging = App.LoadImage(Properties.Resources.Search);
            BorderScrollBackground.Width = 0d;
            SortingLabelType = SortingLabelEnum.NameAZ;
            #region PanelAction
            #region PageLabel
            PageLabel.IELButtonCreateLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                int CountOld = App.CurrentApp.DataLabels.Count;
                App.CurrentApp.ActivateActionCommand(null, "create_label");
                if (CountOld != App.CurrentApp.DataLabels.Count)
                {
                    AppendNewOPLLbel(CountOld);
                }
            };
            PageLabel.IELButtonSelectLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                SelectLabelsMode = !SelectLabelsMode;
                UpdateTextInfoLabels();

                if (!SelectLabelsMode)
                {
                    OPLLabelCommand[] LabelsElements = [.. GridMainLabels.Children.OfType<OPLLabelCommand>().ToArray().Where((i) => i.Selected)];
                    for (int i = 0; i < LabelsElements.Length; i++)
                    {
                        LabelsElements[i].Selected = false;
                    }
                }

                App.MainWindowApplication.IELActionPanelMain.ClosePanelAction();
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
                    WindowGenLabel GenLabel = new();
                    App.MainWindowApplication.ActiveDialog = GenLabel;
                    GenLabel.ChangeLabel(SelectLabelInPage);
                    App.MainWindowApplication.ActiveDialog = null;
                    SelectLabelInPage = null;
                }
            };
            PageLabelElement.IELButtonRemoveLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectLabelInPage != null)
                {
                    if (SelectLabelsMode)
                    {
                        OPLLabelCommand[] LabelsElements = [.. GridMainLabels.Children.OfType<OPLLabelCommand>().ToArray().Where((i) => i.Selected)];
                        for (int i = 0; i < LabelsElements.Length; i++)
                            RemoveLabel(LabelsElements[i]);
                        SelectLabelsMode = false;
                        UpdateTextInfoLabels();
                    }
                    else RemoveLabel(SelectLabelInPage);
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
            PageLabelElement.IELButtonActivateSelectMenu.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindowApplication.IELActionPanelMain.NextPage(PageLabelElement.PanelActionPageSelectLabel);
            };
            #region PageLabelSelectManipulate
            PageLabelElement.PageLabelSelectManipulate.IELButtonBack.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindowApplication.IELActionPanelMain.NextPage(PanelActionSettingsLabelElement.ActiveSource, false);
            };
            PageLabelElement.PageLabelSelectManipulate.IELButtonExecuteSelect.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectLabelInPage == null) return;
                PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.IsEnabled = true;
                if (SelectLabelsMode)
                {
                    SelectLabelInPage.Selected = true;
                    return;
                }

                SelectLabelsMode = true;
                UpdateTextInfoLabels();
                SelectLabelInPage.Selected = true;
            };
            PageLabelElement.PageLabelSelectManipulate.IELButtonExecuteSelect.OnActivateMouseRight += (sender, e, Key) =>
            {
                PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.IsEnabled = true;
                OPLLabelCommand[] LabelsElements = [.. GridMainLabels.Children.OfType<OPLLabelCommand>()];
                for (int i = 0; i < LabelsElements.Length; i++)
                {
                    LabelsElements[i].Selected = true;
                }
                SelectLabelsMode = true;
                UpdateTextInfoLabels();
            };
            PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectLabelInPage == null) return;
                if (SelectLabelsMode)
                {
                    SelectLabelInPage.Selected = false;
                    PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.IsEnabled = false;
                    if (!GridMainLabels.Children.OfType<OPLLabelCommand>().Any((i) => i.Selected))
                    {
                        SelectLabelsMode = false;
                        UpdateTextInfoLabels();
                    }
                }
            };
            PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.OnActivateMouseRight += (sender, e, Key) =>
            {
                PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.IsEnabled = false;
                OPLLabelCommand[] LabelsElements = [.. GridMainLabels.Children.OfType<OPLLabelCommand>().Where((i) => i.Selected)];
                for (int i = 0; i < LabelsElements.Length; i++)
                {
                    LabelsElements[i].Selected = false;
                }
                SelectLabelsMode = false;
                UpdateTextInfoLabels();
            };
            #endregion
            #endregion
            PanelActionPageLabel.IsKeyboardModeChanged += (Source, NewValue) =>
            {
                PageLabel.IELButtonCreateLabel.CharKeyboardActivate = NewValue;
                PageLabel.IELButtonSelectLabel.CharKeyboardActivate = NewValue;
            };
            PanelActionSettingsLabel = new(this, PanelActionPageLabel, new(210d, 220d));

            PanelActionPageLabelElement.IsKeyboardModeChanged += (Source, NewValue) =>
            {
                PageLabelElement.IELButtonExecuteLabel.CharKeyboardActivate = NewValue;
                PageLabelElement.IELButtonChangeLabel.CharKeyboardActivate = NewValue;
                PageLabelElement.IELButtonRemoveLabel.CharKeyboardActivate = NewValue;
                PageLabelElement.IELButtonCreateLabelTag.CharKeyboardActivate = NewValue;
                PageLabelElement.IELButtonActivateSelectMenu.CharKeyboardActivate = NewValue;
            };
            PanelActionSettingsLabelElement = new(GridMain, PanelActionPageLabelElement, new(236d, 323d));

            PageLabelElement.PanelActionPageSelectLabel.IsKeyboardModeChanged += (Source, NewValue) =>
            {
                PageLabelElement.PageLabelSelectManipulate.IELButtonBack.CharKeyboardActivate = NewValue;
                PageLabelElement.PageLabelSelectManipulate.IELButtonExecuteSelect.CharKeyboardActivate = NewValue;
                PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.CharKeyboardActivate = NewValue;
            };
            #endregion
            IELButtonSorting.OnActivateMouseLeft += (sender, e, Key) =>
            {
                SortingLabelEnum[] ArrayValues = Enum.GetValues<SortingLabelEnum>();
                SortingLabelType = ArrayValues[((int)SortingLabelType + 1) % ArrayValues.Length];
            };
            IELButtonSorting.OnActivateMouseRight += (sender, e, Key) =>
            {
                SortingLabelType = SortingLabelEnum.NameAZ;
                e.Handled = true;
            };
            #region Search Setting
            IELButtonSearch.OnActivateMouseLeft += (sender, e, Key) =>
            {
                SearchActivate = !SearchActivate;
                //TextBlockLabelInfo.Text = SearchActivate ? $"Найдено ярлыков: 0 из {App.CurrentApp.DataLabels.Count}" : $"Ярлыков: {App.CurrentApp.DataLabels.Count}";
                IELButtonSearch.IELSettingObject.BackgroundSetting.SetUsedState(SearchActivate);
                if (IELTextBoxSearch.Text.Length == 0)
                {
                    UpdateTextInfoLabels();
                    return;
                }
                SortLabels(App.CurrentApp.DataLabels, SortingLabelType);
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
                    SortLabels(App.CurrentApp.DataLabels, SortingLabelType);
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
                Element.IELSettingObject.BackgroundSetting.InvokeObjectUsedStateColor(StateSpectrum.Default);
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
        /// Добавить новый объект интерфейса ярлыка по индексу данных
        /// </summary>
        /// <param name="Index">Индекс данных</param>
        internal void AppendNewOPLLbel(int Index)
        {
            OPLLabelCommand Label = CreateVisualLabel(Index);
            //App.CurrentApp.DataLabels.Add(Label.SourceLabel);
            SortLabels(App.CurrentApp.DataLabels, SortingLabelType);
            GridMainLabels.Children.Add(Label);
            UpdatePositionLabels(0, false);
            App.AnimateDoubleEffect(Label, OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(400d));
            UpdateTextInfoLabels();
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
                int indexInData = App.CurrentApp.DataLabels.IndexOf(Element.SourceLabel);
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
            if (SelectLabelsMode) TextBlockLabelInfo.Text = "(Режим выделения) ";
            else TextBlockLabelInfo.Text = string.Empty;
            if (SearchActivate)
            {
                OPLLabelCommand[] ArrayLabelsElement = [.. GridMainLabels.Children.Cast<OPLLabelCommand>()];
                TextBlockLabelInfo.Text += $"Найдено ярлыков: {ArrayLabelsElement.Count((i) => i.IELSettingObject.BackgroundSetting.GetUsedState())} из {App.CurrentApp.DataLabels.Count}";
            }
            else TextBlockLabelInfo.Text += $"Ярлыков: {App.CurrentApp.DataLabels.Count}";
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
            UpdateTextInfoLabels();
            //UpdatePositionLabels(0, false);
        }

        /// <summary>
        /// Сгенерировать объект интерфейса ярлыка
        /// </summary>
        private OPLLabelCommand CreateVisualLabel(int IndexData)
        {
            OPLLabelCommand Label = new(App.CurrentApp.DataLabels[IndexData])
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Opacity = 0d,
                Width = WidthLabel,
                Height = HeightLabel,
            };
            Label.ImageSelect.Source = App.LoadImage(Properties.Resources.Check);
            Label.IELSettingObject.IntervalHover = 800d;
            Label.IELSettingObject.AnimationMillisecond = 230;
            Label.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Label.MouseRightButtonDown += (sender, e) => App.MainWindowApplication.IELMessageMain.CloseBorderInformation();
            Label.OnActivateMouseRight += (sender, e, Key) =>
            {
                SelectLabelInPage = Label;
                PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.IsEnabled = SelectLabelInPage.Selected && SelectLabelsMode;
                App.MainWindowApplication.IELActionPanelMain.UsingPanelAction(PanelActionSettingsLabelElement);
                PageLabelElement.UpdateVisibleTag([.. Label.SourceLabel.Tags]);
                e.Handled = true;
            };
            Label.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectLabelsMode)
                {
                    Label.Selected = !Label.Selected;
                    if (Label.Selected) return;
                    if (!GridMainLabels.Children.OfType<OPLLabelCommand>().Any((i) => i.Selected))
                    {
                        SelectLabelsMode = false;
                        UpdateTextInfoLabels();
                    }
                    return;
                }
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
        private void RemoveLabel(OPLLabelCommand Source)
        {
            int UpdateIndex = GridMainLabels.Children.IndexOf(Source);
            GridMainLabels.Children.Remove(Source);
            App.CurrentApp.DataLabels.Remove(Source.SourceLabel);
            UpdatePositionLabels(0);
            UpdateTextInfoLabels();
        }

        /// <summary>
        /// Отсортировать массив ярлыков
        /// </summary>
        /// <param name="SourceLabels">Массив объектов сортировки</param>
        /// <param name="StyleSort">Стиль сортировки</param>
        private void SortLabels(List<LabelAction> SourceLabels, SortingLabelEnum StyleSort)
        {
            switch (StyleSort)
            {
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
                    try
                    {
                        if (x.Name.Equals(y.Name)) return 0;
                        else if (x.Name.Contains(IELTextBoxSearch.Text, StringComparison.CurrentCultureIgnoreCase)) return -1;
                        else if (y.Name.Contains(IELTextBoxSearch.Text, StringComparison.CurrentCultureIgnoreCase)) return 1;
                        else return 0;
                    }
                    catch { return 1; }
                });
            }
            IELButtonSorting.Imaging = StyleSort switch
            {
                //SortingLabelEnum.Not => App.LoadImage(Properties.Resources.Sorting_Not),
                SortingLabelEnum.Tag => App.LoadImage(Properties.Resources.Sorting_Tag),
                SortingLabelEnum.NameAZ => App.LoadImage(Properties.Resources.Sorting_NameAZ),
                SortingLabelEnum.NameZA => App.LoadImage(Properties.Resources.Sorting_NameZA),
                _ => App.LoadImage(Properties.Resources.Sorting_Not)
            };
        }
    }
}
