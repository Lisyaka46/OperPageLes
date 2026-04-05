using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.Pages.ActionPanel.PageLabel;
using ApplicationOperPageLes.UI.Windows.Dialogs;
using IEL.CORE.Enums;
using InterpreterCommand.Classes;
using OIEL.CORE.Browser;
using OIEL.UserElementsControl;
using OIEL.UserElementsControl.Base.LabelBase;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageLabels.xaml
    /// </summary>
    public partial class PageLabels : PageBrowser
    {
        /// <summary>
        /// Выделенный элемент панелью действий
        /// </summary>
        private OPLLabelAction? SelectLabelInPage;

        #region PanelAction
        /// <summary>
        /// Страница взаимодействия с ярлыками
        /// </summary>
        private readonly PageLabelMainActionPanel PageLabel = new();

        /// <summary>
        /// Страница элемента ярлыка в панели действий
        /// </summary>
        private readonly PageLabelElementActionPanel PageLabelElement = new();
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

        /// <summary>
        /// Вид сортировки для ярлыков
        /// </summary>
        private SortingLabelEnum SortingLabelType;

        private bool _SelectLabelsMode = false;
        /// <summary>
        /// Режим выделения ярлыков для манипуляции
        /// </summary>
        internal bool SelectLabelsMode
        {
            get => _SelectLabelsMode;
            private set
            {
                PageLabel.IELButtonClearAllSelect.IsEnabled = value;
                PageLabelElement.IELButtonExecuteLabel.Text = value ? "Выполнить последовательно" : "Выполнить";
                PageLabelElement.IELButtonChangeLabel.IsEnabled = !value;
                PageLabelElement.IELButtonRemoveLabel.Text = value ? "Удалить выделенное" : "Удалить";
                PageLabelElement.IELButtonSetLabelTag.IsEnabled = !value;

                PageLabel.IELButtonCreateLabel.IsEnabled = !value;
                PageLabel.IELButtonClearAllSelect.IsEnabled = value;

                _SelectLabelsMode = value;
            }
        }

        /// <summary>
        /// Массив ярлыков которые были выделены
        /// </summary>
        private List<OPLLabelAction> ListSelectLabel = [];

        /// <summary>
        /// Состояние запуска последовательного выполнения выделенных ярлыков
        /// </summary>
        private bool ActivateConsistentExecuteSelectLabels = false;

        public PageLabels()
        {
            InitializeComponent();

            #region Palette
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(IELButtonSorting);
            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(IELButtonSearch);

            App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(IELTextBoxSearch);
            #endregion

            SearchActivate = false;
            IELButtonSearch.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Search));
            IELButtonSorting.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Sorting_NameAZ));
            IELButtonSorting.IsEnabled = false;
            IELButtonSearch.IsEnabled = false;
            IELTextBoxSearch.IsEnabled = false;
            SortingLabelType = SortingLabelEnum.NameAZ;
            BorderScrollBackground.Width = 0d;
            TextBlockEventInfo.Text = String.Empty;
            TextBlockLabelInfo.Opacity = 0d;
            GridMainLabels.Opacity = 0d;
            GridMainLabels.Visibility = Visibility.Hidden;

            #region PanelAction

            #region PageLabel
            PageLabel.IELButtonCreateLabel.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                int CountOld = App.CurrentApp.DataLabels.Count;
                await App.CurrentApp.ActivateActionCommand(null, "create_label");
                if (CountOld != App.CurrentApp.DataLabels.Count)
                {
                    await AppendNewOPLLbel(CountOld);
                }
            };
            PageLabel.IELButtonManipulateTags.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
                new DialogManipulateLabelTags().ShowManipulateTags();
            };
            PageLabel.IELButtonSelectAllLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageLabel.IELButtonClearAllSelect.IsEnabled = true;
                OPLLabelAction[] LabelsElements = [.. GridMainLabels.Children.OfType<OPLLabelAction>()];
                for (int i = 0; i < LabelsElements.Length; i++)
                {
                    if (!LabelsElements[i].Selected)
                        LabelsElements[i].SelectOn();
                }
                SelectLabelsMode = true;
                UpdateTextInfoLabels();
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
            };
            PageLabel.IELButtonClearAllSelect.IsEnabled = false;
            PageLabel.IELButtonClearAllSelect.OnActivateMouseLeft += (sender, e, Key) =>
            {
                PageLabel.IELButtonClearAllSelect.IsEnabled = false;
                OPLLabelAction[] LabelsElements = [.. GridMainLabels.Children.OfType<OPLLabelAction>().Where((i) => i.Selected)];
                for (int i = 0; i < LabelsElements.Length; i++)
                {
                    if (!ListSelectLabel.Contains(LabelsElements[i]))
                        LabelsElements[i].SelectOff();
                }
                if (ListSelectLabel.Count == 0) SelectLabelsMode = false;
                UpdateTextInfoLabels();
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
            };
            #endregion

            #region PageLabelElement
            PageLabelElement.IELButtonExecuteLabel.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                PageConsole? Console = App.CurrentApp.MainBrowser.SearchAnyPageType<PageConsole>();
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
                if (SelectLabelsMode)
                {
                    PageLabelElement.IELButtonExecuteLabel.IsEnabled = false;
                    ActivateConsistentExecuteSelectLabels = true;
                    if (!GridMainLabels.Children.OfType<OPLLabelAction>().Any((i) => i.Selected))
                        SelectLabelsMode = false;
                    UpdateTextInfoLabels();
                    for (int i = 0; i < ListSelectLabel.Count; i++)
                    {
                        TextBlockEventInfo.Text = $"Последовательное выполнение ярлыков {i + 1}/{ListSelectLabel.Count}";
                        ListSelectLabel[i].SourceBorderBrush.SetActiveSpecrum(Colors.White);
                        OPLCommandViewer? viewer = Console?.CreateNewCommandViewer(
                                COMInterpreterBase.ReadNameCommand(ListSelectLabel[i].SourceLabel.Command));
                        await Task.Delay(700);
                        await App.CurrentApp.ActivateActionCommand(viewer, ListSelectLabel[i].SourceLabel.Command);
                        ListSelectLabel[i].SourceBorderBrush.SetActiveSpecrum(StateSpectrum.Default, true);
                        ListSelectLabel[i].SelectOff();
                    }
                    ListSelectLabel.Clear();
                    if (!GridMainLabels.Children.OfType<OPLLabelAction>().Any((i) => i.Selected)) SelectLabelsMode = false;
                    ActivateConsistentExecuteSelectLabels = false;
                    UpdateTextInfoLabels();
                    PageLabelElement.IELButtonExecuteLabel.IsEnabled = true;
                    TextBlockEventInfo.Text = "Готово";
                }
                else if (SelectLabelInPage != null)
                {
                    await App.CurrentApp.ActivateActionCommand(
                        Console?.CreateNewCommandViewer(
                            COMInterpreterBase.ReadNameCommand(SelectLabelInPage.SourceLabel.Command)),
                        SelectLabelInPage.SourceLabel.Command);
                }
                SelectLabelInPage = null;
            };
            PageLabelElement.IELButtonChangeLabel.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
                if (SelectLabelInPage != null)
                {
                    DialogGenLabel GenLabel = new();
                    App.ActiveDialog = GenLabel;
                    GenLabel.ChangeLabel(SelectLabelInPage);
                    App.ActiveDialog = null;
                    SelectLabelInPage = null;
                }
            };
            PageLabelElement.IELButtonRemoveLabel.OnActivateMouseLeft += async (sender, e, Key) =>
            {
                if (SelectLabelInPage != null)
                {
                    if (SelectLabelsMode)
                    {
                        OPLLabelAction[] LabelsElements = [.. GridMainLabels.Children.OfType<OPLLabelAction>().ToArray().Where((i) => i.Selected)];
                        for (int i = 0; i < LabelsElements.Length; i++)
                            await RemoveLabel(LabelsElements[i]);
                        SelectLabelsMode = false;
                        UpdateTextInfoLabels();
                    }
                    else await RemoveLabel(SelectLabelInPage);
                    SelectLabelInPage = null;
                }
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
            };
            PageLabelElement.IELButtonSetLabelTag.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindow.IELActionPanelMain.ClosePanelAction();
                if (SelectLabelInPage != null)
                {
                    SelectLabelInPage.SelectOn();
                    LabelTag? Tag = new DialogManipulateLabelTags().ShowSelectOneTag();
                    if (Tag != null)
                    {
                        SelectLabelInPage.SourceLabel.AppendTag(Tag);
                        TextBlockEventInfo.Text = "Тег успешно установлен";
                    }
                    SelectLabelInPage.SelectOff();
                    SelectLabelInPage = null;
                }
            };
            PageLabelElement.IELButtonActivateSelectMenu.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindow.IELActionPanelMain.NextPageInObject(PageLabelElement.PageLabelSelectManipulate);
            };

            #region PageLabelSelectManipulate
            PageLabelElement.PageLabelSelectManipulate.IELButtonBack.OnActivateMouseLeft += (sender, e, Key) =>
            {
                App.MainWindow.IELActionPanelMain.NextPageInObject(PageLabelElement, RightAlgin: false);
            };
            PageLabelElement.PageLabelSelectManipulate.IELButtonExecuteSelect.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectLabelInPage == null) return;
                else if (ListSelectLabel.Count == 0)
                {
                    SelectLabelsMode = true;
                    UpdateTextInfoLabels();
                }
                SelectLabelInPage.SelectOn(ref ListSelectLabel);
                PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.IsEnabled = true;
                PageLabelElement.PageLabelSelectManipulate.IELButtonExecuteSelect.IsEnabled = false;
            };
            PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectLabelInPage == null) return;
                if (ListSelectLabel.Contains(SelectLabelInPage)) SelectLabelInPage.SelectOff(ref ListSelectLabel);
                else SelectLabelInPage.SelectOff();
                if (ListSelectLabel.Count == 0 && !GridMainLabels.Children.OfType<OPLLabelAction>().Any((i) => i.Selected))
                {
                    SelectLabelsMode = false;
                    UpdateTextInfoLabels();
                }
                PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.IsEnabled = false;
                PageLabelElement.PageLabelSelectManipulate.IELButtonExecuteSelect.IsEnabled = true;
                UpdateVisualButtonsFromLabel();
            };
            PageLabelElement.IELBlockInfoTagLabel.MouseHover += (sender, e) =>
            {
                if (SelectLabelInPage == null && SelectLabelInPage?.SourceLabel.Tag == null) return;
                App.MainWindow.IELMessageMain.UsingBorderInformation(PageLabelElement.IELBlockInfoTagLabel,
                    SelectLabelInPage.SourceLabel.Tag ?? string.Empty, OrientationPositionCursor.Auto);
            };
            PageLabelElement.IELBlockInfoTagLabel.MouseLeave += (sender, e) =>
            {
                App.MainWindow.IELMessageMain.CloseBorderInformation();
            };
            #endregion

            #endregion

            #endregion

            IELButtonSorting.OnActivateMouseLeft += async (sender, e) =>
            {
                if (App.MainWindow.IELActionPanelMain.PanelActionActivate)
                    App.MainWindow.IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                SortingLabelEnum[] ArrayValues = Enum.GetValues<SortingLabelEnum>();
                await ChangeSortingLabelType(ArrayValues[((int)SortingLabelType + 1) % ArrayValues.Length]);
            };
            IELButtonSorting.OnActivateMouseRight += async (sender, e) =>
            {
                if (App.MainWindow.IELActionPanelMain.PanelActionActivate)
                    App.MainWindow.IELActionPanelMain.ClosePanelAction(PositionAnimActionPanel.CenterObject);
                await ChangeSortingLabelType(SortingLabelEnum.NameAZ);
                e.Handled = true;
            };
            #region Search Setting
            IELButtonSearch.OnActivateMouseLeft += async (sender, e) =>
            {
                SearchActivate = !SearchActivate;
                if (IELTextBoxSearch.Text.Length == 0)
                {
                    UpdateTextInfoLabels();
                    return;
                }
                SortLabels(App.CurrentApp.DataLabels, SortingLabelType);
                UpdateVisualSearchElements();
                await UpdatePositionLabels(0);
            };
            SearchUpdateTimer = new()
            {
                Interval = 360d,
                AutoReset = false,
                Enabled = false,
            };
            SearchUpdateTimer.Elapsed += (sender, e) =>
            {
                Dispatcher.BeginInvoke(async () =>
                {
                    SearchUpdateTimer.Enabled = false;
                    SortLabels(App.CurrentApp.DataLabels, SortingLabelType);
                    UpdateVisualSearchElements();
                    await UpdatePositionLabels(0);
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
                App.MainWindow.IELActionPanelMain.UsingPanelAction(this, PageLabel,
                    Orientation: OrientationPositionCursor.RightDown);
            };
            BorderDinamicLabels.MouseLeftButtonUp += (sender, e) =>
            {
                if (App.MainWindow.IELActionPanelMain.PanelActionActivate)
                    App.MainWindow.IELActionPanelMain.ClosePanelAction();
            };
            SizeChanged += async (sender, e) =>
            {
                if (SaveCountOneLineLabel == CountOneLineLabel) return;
                SaveCountOneLineLabel = CountOneLineLabel;
                await UpdatePositionLabels(0);
            };

            Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
            {
                Grid GridLabels = await App.MainWindow.ExecuteVisualizateLoadingProcess("Загрузка списка ярлыков",
                    CreateAllVisualLabels(GridMainLabels));
                TextBlockEventInfo.Text = "Готово";
                IELButtonSorting.IsEnabled = true;
                IELButtonSearch.IsEnabled = true;
                IELTextBoxSearch.IsEnabled = true;
                GridLabels.Visibility = Visibility.Visible;
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(GridLabels, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(TextBlockLabelInfo, OpacityProperty, 1d, TimeSpan.FromMilliseconds(200d));
            });
        }

        /// <summary>
        /// Обновить визуализацию поиска
        /// </summary>
        private void UpdateVisualSearchElements()
        {
            for (int i = 0; i < GridMainLabels.Children.Count; i++)
            {
                OPLLabelAction Element = (OPLLabelAction)GridMainLabels.Children[i];
                if (!SearchActivate || IELTextBoxSearch.Text.Length == 0)
                {
                    Element.SourceBackground.SetUsedState(false);
                    continue;
                }
                else if (Element.SourceLabel.Name.Contains(IELTextBoxSearch.Text, StringComparison.CurrentCultureIgnoreCase))
                    Element.SourceBackground.SetUsedState(true);
                else Element.SourceBackground.SetUsedState(false);
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
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderScrollBackground, WidthProperty, Offset, TimeSpan.FromMilliseconds(300d));
        }

        /// <summary>
        /// Изменить вид сортировки для ярлыков
        /// </summary>
        internal async Task ChangeSortingLabelType(SortingLabelEnum NewValue)
        {
            SortLabels(App.CurrentApp.DataLabels, NewValue);
            SortingLabelType = NewValue;
            await UpdatePositionLabels(0);
        }

        /// <summary>
        /// Добавить новый объект интерфейса ярлыка по индексу данных
        /// </summary>
        /// <param name="Index">Индекс данных</param>
        internal async Task AppendNewOPLLbel(int Index)
        {
            OPLLabelAction Label = CreateVisualLabel(Index);
            SortLabels(App.CurrentApp.DataLabels, SortingLabelType);
            GridMainLabels.Children.Add(Label);
            await UpdatePositionLabels(0, false);
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(Label, OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(400d));
            UpdateTextInfoLabels();
        }

        /// <summary>
        /// Обновить визуализацию прокрутки
        /// </summary>
        private async Task UpdatePositionLabels(int StartIndex, bool Animatable = true)
        {
            if (GridMainLabels.Children.Count == 0) return;
            BorderDinamicLabels.UpdateLayout();
            await Dispatcher.BeginInvoke(() =>
            {
                for (int i = StartIndex; i < GridMainLabels.Children.Count; i++)
                {
                    OPLLabelAction Element = (OPLLabelAction)GridMainLabels.Children[i];
                    int indexInData = App.CurrentApp.DataLabels.IndexOf(Element.SourceLabel);
                    int CountOneLine = indexInData % CountOneLineLabel;
                    int CountLine = indexInData / CountOneLineLabel;
                    int Left = CountOneLine == 0 ? MarginLabel : CountOneLine * FULL_WidthLabel;
                    int Top = CountLine == 0 ? MarginLabel : CountLine * FULL_HeightLabel;
                    if (Element.Margin.Left == Left && Element.Margin.Top == Top) continue;
                    if (Animatable) 
                        App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(Element, MarginProperty, new(Left, Top, 0, 0), TimeSpan.FromMilliseconds(360d));
                    else Element.Margin = new(Left, Top, 0, 0);
                }
            });
        }

        /// <summary>
        /// Обновить текст информации об ярлыках
        /// </summary>
        private void UpdateTextInfoLabels()
        {
            if (ActivateConsistentExecuteSelectLabels) TextBlockLabelInfo.Text = "(Последовательное выполнение) ";
            else if (SelectLabelsMode) TextBlockLabelInfo.Text = "(Режим выделения) ";
            else TextBlockLabelInfo.Text = string.Empty;
            if (SearchActivate)
            {
                OPLLabelAction[] ArrayLabelsElement = [.. GridMainLabels.Children.Cast<OPLLabelAction>()];
                TextBlockLabelInfo.Text += "Найдено ярлыков: " +
                    $"{ArrayLabelsElement.Count((i) => i.SourceBackground.GetUsedState())} из {App.CurrentApp.DataLabels.Count}";
            }
            else
            {
                TextBlockLabelInfo.Text += $"Ярлыков: ";
                if (ActivateConsistentExecuteSelectLabels)
                    TextBlockLabelInfo.Text += $"{ListSelectLabel.Count} <<< {App.CurrentApp.DataLabels.Count}";
                else TextBlockLabelInfo.Text += $"{App.CurrentApp.DataLabels.Count}";
            }
        }

        /// <summary>
        /// Сгенерировать все объекты интерфейса ярлыка
        /// </summary>
        /// <param name="GridLabels">Сетка в которую помещаются ярлыки</param>
        private async Task<Grid> CreateAllVisualLabels(Grid GridLabels)
        {
            for (int i = 0; i < App.CurrentApp.DataLabels.Count; i++)
            {
                await Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        TextBlockEventInfo.Text = $"Идёт загрузка всех ярлыков ({i}/{App.CurrentApp.DataLabels.Count})";
                        OPLLabelAction Element = CreateVisualLabel(i);
                        Element.Opacity = 1d;
                        if (Element.SourceLabel.Tag != null)
                        {
                            if (!App.CurrentApp.DataLabelTags.Any(i => i.ValueTag.Equals(Element.SourceLabel.Tag)))
                                Element.SourceLabel.RemoveTag();
                        }
                        GridLabels.Children.Add(Element);
                    });
                    Thread.Sleep(20);
                });
            }
            UpdateTextInfoLabels();
            await UpdatePositionLabels(0);
            return GridLabels;
        }

        /// <summary>
        /// Сгенерировать объект интерфейса ярлыка
        /// </summary>
        private OPLLabelAction CreateVisualLabel(int IndexData)
        {
            OPLLabelAction Label = new(App.CurrentApp.DataLabels[IndexData])
            {
                Opacity = 0d,
                Width = WidthLabel,
                Height = HeightLabel,
                ImageSelectSource = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Check)),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                ManagerAnimation = App.ManagerAnimation,
            };
            string name_command = COMInterpreterBase.ReadNameCommand(Label.SourceLabel.Command);
            string NameLabelImage;
            switch (name_command)
            {
                case "open_file":
                    App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Gray].ConnectPalleteFromIELElement(Label);
                    NameLabelImage = nameof(OPRES.File);
                    break;
                case "open_link":
                    App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Jade].ConnectPalleteFromIELElement(Label);
                    NameLabelImage = nameof(OPRES.Link);

                    Dispatcher.BeginInvoke(DispatcherPriority.Background, async () =>
                    {
                        string[] Param = COMInterpreterBase.ReadParametersCommand(Label.SourceLabel.Command);
                        if (Param.Length > 0)
                            await Label.SetFaviconIcon(new Uri(Param[0]));
                    });
                    //while (worker.IsBusy) System.Windows.Forms.Application.DoEvents();
                    //action.Invoke();
                    break;
                case "open_directory":
                    App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.LightBlue].ConnectPalleteFromIELElement(Label);
                    NameLabelImage = nameof(OPRES.Folder);
                    break;
                default:
                    App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(Label);
                    NameLabelImage = nameof(OPRES.Command);
                    break;
            }
            Label.ImageSource = StructDirectoryResources.GetResourceBitmap(NameLabelImage);
            Label.UpdateLayout();
            Label.MouseRightButtonDown += (sender, e) => App.MainWindow.IELMessageMain.CloseBorderInformation();
            Label.MouseRightButtonUp += (sender, e) =>
            {
                SelectLabelInPage = Label;
                UpdateVisualButtonsFromLabel();
                App.MainWindow.IELActionPanelMain.UsingPanelAction(GridMain, PageLabelElement,
                    Orientation: OrientationPositionCursor.RightDown);
                e.Handled = true;
            };
            Label.MouseLeftButtonUp += async (sender, e) =>
            {
                if (ActivateConsistentExecuteSelectLabels)
                {
                    App.MainWindow.GenerateVisualizateImage("Невозможно запустить ярлык при последовательном исполнении.");
                    return;
                }
                else if (SelectLabelsMode)
                {
                    if (Label.Selected && ListSelectLabel.Contains(Label))
                    {
                        for (int i = ListSelectLabel.IndexOf(Label) + 1; i < ListSelectLabel.Count; i++)
                            ListSelectLabel[i].SetIndexVisual(i);
                        Label.SelectOff(ref ListSelectLabel);
                    }
                    else Label.SelectOn(ref ListSelectLabel);

                    if (ListSelectLabel.Count == 0)
                    {
                        if (!GridMainLabels.Children.OfType<OPLLabelAction>().Any((i) => i.Selected)) SelectLabelsMode = false;
                        ListSelectLabel.Clear();
                        UpdateTextInfoLabels();
                    }
                    return;
                }
                PageConsole? Console = App.CurrentApp.MainBrowser.SearchAnyPageType<PageConsole>();
                await App.CurrentApp.ActivateActionCommand(
                    Console?.CreateNewCommandViewer(
                        COMInterpreterBase.ReadNameCommand(Label.SourceLabel.Command)),
                    Label.SourceLabel.Command);
                e.Handled = true;
            };
            Label.MouseLeave += (sender, e) => App.MainWindow.IELMessageMain.CloseBorderInformation();
            Label.MouseLeftButtonDown += (sender, e) => App.MainWindow.IELMessageMain.CloseBorderInformation();
            return Label;
        }

        /// <summary>
        /// Удалить ярлык по индексу
        /// </summary>
        /// <param name="Index">индекс удаляемого ярлыка</param>
        private async Task RemoveLabel(OPLLabelAction Source)
        {
            GridMainLabels.Children.Remove(Source);
            App.CurrentApp.DataLabels.Remove(Source.SourceLabel);
            await UpdatePositionLabels(0);
            UpdateTextInfoLabels();
        }

        /// <summary>
        /// Обновить отображение кнопок и их активность
        /// </summary>
        private void UpdateVisualButtonsFromLabel()
        {
            PageLabelElement.IELButtonExecuteLabel.IsEnabled = !ActivateConsistentExecuteSelectLabels && ListSelectLabel.Count > 0 || !SelectLabelsMode;
            PageLabelElement.PageLabelSelectManipulate.IELButtonExecuteSelect.IsEnabled = !ActivateConsistentExecuteSelectLabels &&
                    (!SelectLabelInPage?.Selected ?? false);
            PageLabelElement.PageLabelSelectManipulate.IELButtonClearSelect.IsEnabled = !ActivateConsistentExecuteSelectLabels &&
                    (SelectLabelInPage?.Selected ?? false);
            PageLabelElement.IELBlockInfoTagLabel.IsEnabled = SelectLabelInPage?.SourceLabel.Tag != null;
            PageLabelElement.IELButtonSetLabelTag.IsEnabled = App.CurrentApp.DataLabelTags.Count > 0 && !SelectLabelsMode;
            PageLabelElement.IELButtonSetLabelTag.Text = SelectLabelInPage?.SourceLabel.Tag != null ? "Изменить тег" : "Добавить тег";
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
                        if (x.Tag == null)
                        {
                            if (y.Tag == null) return 0;
                            else return -1;
                        }
                        else
                        {
                            if (y.Tag == null) return 1;
                            else return x.Tag.CompareTo(y.Tag);
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
            IELButtonSorting.Source = StyleSort switch
            {
                SortingLabelEnum.Tag => StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Sorting_Tag)),
                SortingLabelEnum.NameAZ => StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Sorting_NameAZ)),
                SortingLabelEnum.NameZA => StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Sorting_NameZA)),
                _ => StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Sorting_Not))
            };
        }
    }
}
