using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OperPageLes.CORE.Enums.Theme;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Settings;
using OPLAPI.CORE.Settings.Base;
using OPLAPI.CORE.Settings.Parameters;
using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.UserElementsControl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace OperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageSetting.xaml
    /// </summary>
    public partial class PageSetting : PageBrowser
    {
        /// <summary>
        /// Словарь всех категорий настроек
        /// </summary>
        private Dictionary<string, StackPanel> SourceVisualCategories;

        /// <summary>
        /// Стековое представление кнопок категорий в настройках
        /// </summary>
        private StackPanel StackPanelButtonsCategories;

        /// <summary>
        /// Активный объект кнопки категории, которая в данный момент открыта
        /// </summary>
        private IELButtonText? ActiveButtonCategory;

        /// <summary>
        /// Активный объект категории, который в данный момент отображается
        /// </summary>
        private StackPanel? ActivePanelCategory;

        #region Binding
        /// <summary>
        /// Объект присоеденения пользовательского шрифта к визуальному объекту параметра настроек
        /// </summary>
        private System.Windows.Data.Binding BindingParameterText;

        /// <summary>
        /// Объект присоеденения пользовательского шрифта к визуальному объекту категории настроек
        /// </summary>
        private System.Windows.Data.Binding BindingCategoryText;
        #endregion

        public PageSetting()
        {
            InitializeComponent();

            #region Binding
            BindingCategoryText = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["RussianRail G Pro"]
            };
            BindingParameterText = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["Bree CYR var"]
            };
            #endregion

            StackPanelButtonsCategories = new()
            {
                ClipToBounds = true,
                Orientation = System.Windows.Controls.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,

            };
            IELScrollCategoryButtons.Content = StackPanelButtonsCategories;
            SourceVisualCategories = [];
        }

        #region Handlers
        /// <summary>
        /// Обработчик события добавления новой категории
        /// </summary>
        /// <param name="sender">Нулевой объект</param>
        /// <param name="e">Добавляемая категория</param>
        internal void HandlerAppendCategory(object? sender, CategorySettingBase e)
        {
            StackPanel NewVisualCategory = new()
            {
                Opacity = 0d,
                Margin = new(-5d),
                IsEnabled = false
            };
            Canvas.SetZIndex(NewVisualCategory, -1);
            ControllerCategories.Children.Add(NewVisualCategory);
            SourceVisualCategories.Add(e.KeyCategory, NewVisualCategory);
            StackPanelButtonsCategories.Children.Add(GenerateNewButtonCategory(e));
            e.ParameterAppend += HandlerParameterAppend;
        }

        /// <summary>
        /// Обработчик события добавления нового параметра в категорию
        /// </summary>
        /// <param name="sender">Категория в которую добавляется параметр</param>
        /// <param name="e">Параметры события добавления параметра</param>
        private void HandlerParameterAppend(object? sender, CategorySettingBase.AppendParameterEventArgs e)
        {
            CategorySettingBase Category = sender as CategorySettingBase ??
                    throw new Exception("Неудалось преобразовать объект в базовый класс категории.");
            FrameworkElement VisualParameter = GenerateNewVisualParameter(e.NewParameter);
            SourceVisualCategories[Category.KeyCategory].Children.Add(VisualParameter);
        }

        /// <summary>
        /// Функция обработки события открытия категории
        /// </summary>
        /// <param name="sender">Объект кнопки нажимаемый для открытия категории</param>
        /// <param name="e">Данные события нажатия</param>
        private void HandlerActivateCategory(object sender, MouseButtonEventArgs e)
        {
            IELButtonText Element = (IELButtonText)sender;
            if (ActiveButtonCategory != null)
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ActiveButtonCategory, IELButtonText.MarginProperty,
                new Thickness(0d, 2d, 15d, 2d), TimeSpan.FromMilliseconds(500d));
                ActiveButtonCategory.SourceBackground.SetUsedState(false);
                if (ActiveButtonCategory.Equals(Element))
                {
                    ActiveButtonCategory = null;
                    ActivateCategory(null);
                    return;
                }
            }
            ActiveButtonCategory = Element;
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Element, IELButtonText.MarginProperty,
                new Thickness(0d, 2d, 2d, 2d), TimeSpan.FromMilliseconds(500d));
            Element.SourceBackground.SetUsedState(true);
            ActivateCategory(SourceVisualCategories[(string)Element.Tag]);
        }
        #endregion

        #region Manipulate
        /// <summary>
        /// Активировать категорию
        /// </summary>
        /// <remarks>
        /// При нулевом значении визуального объекта параметров, текущая категория, если она активна, будет просто закрыта
        /// </remarks>
        /// <param name="ActivateCategory">Визуальный объект представления параметров категории</param>
        private void ActivateCategory(in StackPanel? ActivateCategory)
        {
            if (ActivePanelCategory != null)
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ActivePanelCategory, MarginProperty,
                    new Thickness(-5d), TimeSpan.FromMilliseconds(500d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ActivePanelCategory, OpacityProperty,
                    0d, TimeSpan.FromMilliseconds(500d));
                ActivePanelCategory.IsEnabled = false;
                Canvas.SetZIndex(ActivePanelCategory, -1);
                ActivePanelCategory = null;
            }
            if (ActivateCategory != null)
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ActivateCategory, MarginProperty,
                    new Thickness(0d), TimeSpan.FromMilliseconds(500d));
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, ActivateCategory, OpacityProperty,
                    1d, TimeSpan.FromMilliseconds(500d));
            }
            else return;
            Canvas.SetZIndex(ActivateCategory, 0);
            ActivateCategory.IsEnabled = true;
            ActivePanelCategory = ActivateCategory;
        }
        #endregion

        #region Generate
        /// <summary>
        /// Создание кнопки открывающую категорию
        /// </summary>
        /// <param name="Category">Категория на основе которой создаётся визуальный объект</param>
        /// <returns></returns>
        private IELButtonText GenerateNewButtonCategory(in CategorySettingBase Category)
        {
            IELButtonText Result = new()
            {
                Padding = new(0d, 0d, 0d, 2d),
                Margin = new(0d, 2d, 15d, 2d),
                CornerRadius = new(0d, 10d, 10d, 0d),
                Height = 35d,
                MarginViewBox = new(2d),
                Text = Category.NameCategory,
                Palette = App.CurrentApp.ActiveThemeApplication[PaletteEnum.Aquamarine],
                DataContext = Category,
                Tag = Category.KeyCategory,
            };
            Category.PropertyChanged += (sender, e) =>
            {
                CategorySettingBase Category = sender as CategorySettingBase ??
                    throw new Exception("Неудалось преобразовать объект в базовый класс категории.");
                switch (e.PropertyName)
                {
                    case nameof(CategorySettingBase.NameCategory):
                        Result.Text = Category.NameCategory;
                        break;
                }
            };
            BindingOperations.SetBinding(Result, IELButtonText.FontFamilyProperty, BindingCategoryText);
            Result.OnActivateMouseLeft += HandlerActivateCategory;
            return Result;
        }

        /// <summary>
        /// Создать визуальный элемент подключённый к параметру
        /// </summary>
        /// <remarks>
        /// Для данной функции создания визуального объекта применимы только поддерживаемые типы данных:<br/>
        /// <b>BOOL</b>
        /// </remarks>
        /// <param name="NewParameter">Параметр к которому подключается визуальный элемент</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private FrameworkElement GenerateNewVisualParameter(ParameterSettingBase NewParameter)
        {
            if (NewParameter.TypeParameterValue == typeof(bool))
            {
                OPLCheckBox ResultCheckBox = new()
                {
                    Text = NewParameter.ParameterName,
                    IsChecked = (bool)NewParameter.Value,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                    ManagerAnimation = ManagerAnimation,
                    Palette = App.CurrentApp.ActiveThemeApplication[PaletteEnum.PastelBlue],
                    CheckBoxCornerRadius = 5d,
                    CornerRadius = new(8d),
                    Margin = new(5d),
                };
                BindingOperations.SetBinding(ResultCheckBox, OPLCheckBox.FontFamilyProperty, BindingParameterText);
                ResultCheckBox.IsCheckedChanged += (sender, e) =>
                {
                    ParameterSettingBase.SetValue(NewParameter, e);
                };
                NewParameter.ValueChanged += (Old, New) =>
                {
                    ResultCheckBox.IsChecked = (bool)New;
                };
                NewParameter.PropertyChanged += (sender, e) =>
                {
                    ParameterSettingBase Parameter = sender as ParameterSettingBase ??
                        throw new Exception("Неудалось преобразовать объект в базовый класс параметра.");
                    switch (e.PropertyName)
                    {
                        case nameof(ParameterSettingBase.ParameterName):
                            ResultCheckBox.Text = Parameter.ParameterName;
                            break;
                    };
                };
                return ResultCheckBox;
            }
            else
                return new TextBlock()
                {
                    Text = "This type of parameter is not supported for display",
                    FontSize = 8d,
                };
            //else if (NewParameter.TypeParameterValue == typeof(string))
            //{

            //}
            //else if (NewParameter.GetType() == typeof(LimitedParameterIntSetting) ||
            //    NewParameter.GetType() == typeof(LimitedParameterDoubleSetting))
            //{

            //}
        }
        #endregion
    }
}
