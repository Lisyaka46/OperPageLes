using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace ApplicationOperPageLes.UI.Windows.Base
{
    public partial class OPLWindowBase : Window, IOPLAnimate, IDisposable
    {
        #region UIElements
        /// <summary>
        /// Главный объект границ окна
        /// </summary>
        private Border MainBorderWindow;

        /// <summary>
        /// Главный объект контента окна
        /// </summary>
        private Grid MainGridWindow;

        #region Title
        /// <summary>
        /// Сетка элементов заголовка окна
        /// </summary>
        private Grid GridTitleContent;

        /// <summary>
        /// Изображение инокни окна
        /// </summary>
        private System.Windows.Controls.Image ImageIcon;

        /// <summary>
        /// Объект масштабирования заголовка
        /// </summary>
        private Viewbox BoxTitle;

        /// <summary>
        /// Объект текста заголовка окна
        /// </summary>
        private TextBlock TextBlockTitle;
        #endregion

        /// <summary>
        /// Главный объект контента окна
        /// </summary>
        private Grid GridContentWindow;
        #endregion

        #region Properties

        #region Content
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(OPLWindowBase),
                new(
                    (sender, e) =>
                    {
                        OPLWindowBase WindowBase = (OPLWindowBase)sender;
                        if (WindowBase.GridContentWindow.Children.Count > 0)
                            throw new Exception("Невозможно задать более одного контента для окна!");
                        else if (e.NewValue is UIElement element)
                            WindowBase.GridContentWindow.Children.Add(element);
                        else throw new Exception($"Невозможно привести тип \"{e.NewValue.GetType().Name}\" к ожидаемому UIElement для установки контента!");
                    }));

        /// <summary>
        /// Внутренний элемент объекта
        /// </summary>
        public new UIElement Content
        {
            get => (UIElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        #endregion

        #region CornerRadius
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(OPLWindowBase),
                new(new CornerRadius(10),
                    (sender, e) =>
                    {
                        ((OPLWindowBase)sender).MainBorderWindow.CornerRadius = (CornerRadius)e.NewValue;
                    }));

        /// <summary>
        /// Скругление границ объекта
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)MainBorderWindow.GetValue(CornerRadiusProperty);
            set => MainBorderWindow.SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #region BorderThickness
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(OPLWindowBase),
                new(new Thickness(0),
                    (sender, e) =>
                    {
                        ((OPLWindowBase)sender).MainBorderWindow.BorderThickness = (Thickness)e.NewValue;
                    }));

        /// <summary>
        /// Толщина границ объекта
        /// </summary>
        public new Thickness BorderThickness
        {
            get => (Thickness)MainBorderWindow.GetValue(BorderThicknessProperty);
            set => MainBorderWindow.SetValue(BorderThicknessProperty, value);
        }
        #endregion

        #region Title
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(OPLWindowBase),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLWindowBase)sender).TextBlockTitle.Text = (string)e.NewValue;
                        ((Window)sender).Title = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст заголовка окна
        /// </summary>
        public new string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                TextBlockTitle.UpdateLayout();
                BoxTitle.MaxWidth = TextBlockTitle.MaxWidth;
            }
        }
        #endregion

        #region TitleFontFamily
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TitleFontFamilyProperty =
            DependencyProperty.Register("TitleFontFamily", typeof(System.Windows.Media.FontFamily), typeof(OPLWindowBase),
                new(new System.Windows.Media.FontFamily("Calibri"),
                    (sender, e) =>
                    {
                        ((OPLWindowBase)sender).TextBlockTitle.FontFamily = (System.Windows.Media.FontFamily)e.NewValue;
                    }));

        /// <summary>
        /// Шрифт используемый для заголовка окна
        /// </summary>
        public System.Windows.Media.FontFamily TitleFontFamily
        {
            get => (System.Windows.Media.FontFamily)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        #endregion

        #region TitleForeground
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(OPLWindowBase),
                new(new SolidColorBrush(Colors.Black),
                    (sender, e) =>
                    {
                        ((OPLWindowBase)sender).TextBlockTitle.Foreground = (Brush)e.NewValue;
                    }));

        /// <summary>
        /// Цвет текста заголовка
        /// </summary>
        public Brush TitleForeground
        {
            get => (Brush)GetValue(TitleForegroundProperty);
            set => SetValue(TitleForegroundProperty, value);
        }
        #endregion

        #region Icon
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(OPLWindowBase),
                new(null,
                    (sender, e) =>
                    {
                        ((OPLWindowBase)sender).ImageIcon.Source = (ImageSource?)e.NewValue;
                        ((Window)sender).Icon = (ImageSource?)e.NewValue;
                    }));

        /// <summary>
        /// Ресурс иконки окна
        /// </summary>
        public new ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        #endregion

        #region Background
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(OPLWindowBase),
                new(null,
                    (sender, e) =>
                    {
                        ((OPLWindowBase)sender).MainBorderWindow.Background = (Brush?)e.NewValue;
                    }));

        /// <summary>
        /// Цвет фона окна
        /// </summary>
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        public OPLWindowBase()
        {
            ImageIcon = new()
            {
                Width = 25d,
                Height = 25d,
                Margin = new(3),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
            };
            BoxTitle = new()
            {
                Margin = new(0, 0, 5, 0),
                Stretch = Stretch.Uniform,
                StretchDirection = StretchDirection.DownOnly,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment= System.Windows.VerticalAlignment.Center,
            };
            TextBlockTitle = new()
            {
                Text = string.Empty,
                FontSize = 16d,
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
            };
            System.Windows.Data.Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["Plup"]
            };
            BindingOperations.SetBinding(TextBlockTitle, TextBlock.FontFamilyProperty, binding);
            BoxTitle.Child = TextBlockTitle;

            GridTitleContent = new()
            {
                Margin = new(0, 5, 0, 0),
            };
            GridTitleContent.ColumnDefinitions.Add(new() { Width = new(50d, GridUnitType.Pixel) });
            GridTitleContent.ColumnDefinitions.Add(new() { Width = new(1d, GridUnitType.Star) });
            Grid.SetColumn(ImageIcon, 0);
            Grid.SetColumn(BoxTitle, 1);
            GridTitleContent.Children.Add(ImageIcon);
            GridTitleContent.Children.Add(BoxTitle);

            GridContentWindow = new()
            {
                Margin = new(5),
            };
            MainGridWindow = new();
            MainGridWindow.RowDefinitions.Add(new() { Height = new(0d, GridUnitType.Auto) });
            MainGridWindow.RowDefinitions.Add(new() { Height = new(1d, GridUnitType.Star) });
            Grid.SetRow(GridTitleContent, 0);
            Grid.SetRow(GridContentWindow, 1);
            MainGridWindow.Children.Add(GridTitleContent);
            MainGridWindow.Children.Add(GridContentWindow);
            MainBorderWindow = new()
            {
                BorderThickness = new(0),
                CornerRadius = new(10),
                Child = MainGridWindow,
                Background = null,
            };
            base.Content = MainBorderWindow;
            Background = new SolidColorBrush(Colors.Aqua);
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            base.Background = null;
            Opacity = 0d;
        }

        /// <summary>
        /// Очистить ресурсы объекта
        /// </summary>
        public void Dispose()
        {
            if (ShowActivated)
            ManagerAnimation = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Отобразить объект окна
        /// </summary>
        public new void Show()
        {
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(this, OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(1400d));
            else
                Opacity = 1d;
            base.Show();
        }

        /// <summary>
        /// Отобразить объект окна
        /// </summary>
        public new void ShowDialog()
        {
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(this, OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(1400d));
            else
                Opacity = 1d;
            base.ShowDialog();
        }
    }
}
