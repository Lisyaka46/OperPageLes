using IEL.UserElementsControl.Base;
using System.Windows;
using System.Windows.Controls;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace ApplicationOperPageLes.UI.UserElementsControl.Base
{
    public class OPLElementViewerBase : IELButtonBase
    {
        /// <summary>
        /// Главный контейнер объекта
        /// </summary>
        private Grid Base_MainGridObject;

        /// <summary>
        /// Объект текста отображения имени
        /// </summary>
        private TextBlock Base_TextBlockName;

        /// <summary>
        /// Бордер кругового индикатора
        /// </summary>
        private Border Base_BorderCircleIndiator;

        /// <summary>
        /// Бордер визуального элемента
        /// </summary>
        private Border Base_BorderElementView;
        #region Properties

        #region Content
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(OPLElementViewerBase),
                new(
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_BorderElementView.Child = (UIElement)e.NewValue;
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

        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLElementViewerBase),
                new("Name",
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_TextBlockName.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст отображаемый в названии
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region HorizontalAligmentCircleIndicator
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty HorizontalAligmentCircleIndicatorProperty =
            DependencyProperty.Register("HorizontalAligmentCircleIndicator", typeof(HorizontalAlignment), typeof(OPLElementViewerBase),
                new(HorizontalAlignment.Right,
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_BorderCircleIndiator.HorizontalAlignment = (HorizontalAlignment)e.NewValue;
                    }));

        /// <summary>
        /// Отображение позиционирования кругового индикатора по горизонтали
        /// </summary>
        public HorizontalAlignment HorizontalAligmentCircleIndicator
        {
            get => (HorizontalAlignment)GetValue(HorizontalAligmentCircleIndicatorProperty);
            set => SetValue(HorizontalAligmentCircleIndicatorProperty, value);
        }
        #endregion

        #region HorizontalAligmentCircleIndicator
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty VerticalAligmentCircleIndicatorProperty =
            DependencyProperty.Register("VerticalAligmentCircleIndicator", typeof(VerticalAlignment), typeof(OPLElementViewerBase),
                new(VerticalAlignment.Top,
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_BorderCircleIndiator.VerticalAlignment = (VerticalAlignment)e.NewValue;
                    }));

        /// <summary>
        /// Отображение позиционирования кругового индикатора по вертикали
        /// </summary>
        public VerticalAlignment VerticalAligmentCircleIndicator
        {
            get => (VerticalAlignment)GetValue(VerticalAligmentCircleIndicatorProperty);
            set => SetValue(VerticalAligmentCircleIndicatorProperty, value);
        }
        #endregion

        #region WidthCircleIndicator
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty WidthCircleIndicatorProperty =
            DependencyProperty.Register("WidthCircleIndicator", typeof(double), typeof(OPLElementViewerBase),
                new(14d,
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_BorderCircleIndiator.Width = (double)e.NewValue;
                    }));

        /// <summary>
        /// Размер ширины кругового индикатора
        /// </summary>
        public double WidthCircleIndicator
        {
            get => (double)GetValue(WidthCircleIndicatorProperty);
            set => SetValue(WidthCircleIndicatorProperty, value);
        }
        #endregion

        #region HeightCircleIndicator
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty HeightCircleIndicatorProperty =
            DependencyProperty.Register("HeightCircleIndicator", typeof(double), typeof(OPLElementViewerBase),
                new(14d,
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_BorderCircleIndiator.Height = (double)e.NewValue;
                    }));

        /// <summary>
        /// Размер высоты кругового индикатора
        /// </summary>
        public double HeightCircleIndicator
        {
            get => (double)GetValue(HeightCircleIndicatorProperty);
            set => SetValue(HeightCircleIndicatorProperty, value);
        }
        #endregion

        #region BorderThickness (Collection)

        #region BorderThicknessCircleIndicator
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty BorderThicknessCircleIndicatorProperty =
            DependencyProperty.Register("BorderThicknessCircleIndicator", typeof(Thickness), typeof(OPLElementViewerBase),
                new(new Thickness(2),
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_BorderCircleIndiator.BorderThickness = (Thickness)e.NewValue;
                    }));

        /// <summary>
        /// Толщина границ кругового индикатора
        /// </summary>
        public Thickness BorderThicknessCircleIndicator
        {
            get => (Thickness)GetValue(BorderThicknessCircleIndicatorProperty);
            set => SetValue(BorderThicknessCircleIndicatorProperty, value);
        }
        #endregion

        #region BorderThicknessBorderView
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty BorderThicknessBorderViewProperty =
            DependencyProperty.Register("BorderThicknessBorderView", typeof(Thickness), typeof(OPLElementViewerBase),
                new(new Thickness(2),
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_BorderElementView.BorderThickness = (Thickness)e.NewValue;
                    }));

        /// <summary>
        /// Толщина границ визуализирующего объекта
        /// </summary>
        public Thickness BorderThicknessBorderView
        {
            get => (Thickness)GetValue(BorderThicknessBorderViewProperty);
            set => SetValue(BorderThicknessBorderViewProperty, value);
        }
        #endregion

        #endregion

        #region CornerRadius (Collection)

        #region CornerRadiusCircleIndicator
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty CornerRadiusCircleIndicatorProperty =
            DependencyProperty.Register("CornerRadiusCircleIndicator", typeof(CornerRadius), typeof(OPLElementViewerBase),
                new(new CornerRadius(0),
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_BorderCircleIndiator.CornerRadius = (CornerRadius)e.NewValue;
                    }));

        /// <summary>
        /// Скругление границ кругового индикатора
        /// </summary>
        public CornerRadius CornerRadiusCircleIndicator
        {
            get => (CornerRadius)GetValue(CornerRadiusCircleIndicatorProperty);
            set => SetValue(CornerRadiusCircleIndicatorProperty, value);
        }
        #endregion

        #region CornerRadiusBorderView
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty CornerRadiusBorderViewProperty =
            DependencyProperty.Register("CornerRadiusBorderView", typeof(CornerRadius), typeof(OPLElementViewerBase),
                new(new CornerRadius(0),
                    (sender, e) =>
                    {
                        ((OPLElementViewerBase)sender).Base_BorderElementView.CornerRadius = (CornerRadius)e.NewValue;
                    }));

        /// <summary>
        /// Скругление границ визуализирующего объекта
        /// </summary>
        public CornerRadius CornerRadiusBorderView
        {
            get => (CornerRadius)GetValue(CornerRadiusBorderViewProperty);
            set => SetValue(CornerRadiusBorderViewProperty, value);
        }
        #endregion

        #region CornerRadius
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(OPLElementViewerBase),
                new(new CornerRadius(0),
                    (sender, e) =>
                    {
                        CornerRadius Value = (CornerRadius)e.NewValue;
                        ((IELContainerBase)sender).SetValue(IELContainerBase.CornerRadiusProperty, Value);
                        ((OPLElementViewerBase)sender).Base_MainGridObject.Margin =
                            new(
                                Math.Max(Value.TopLeft, Value.BottomLeft), Math.Max(Value.TopLeft, Value.TopRight),
                                Math.Max(Value.BottomLeft, Value.BottomRight), Math.Max(Value.TopRight, Value.BottomRight)
                                );
                    }));

        /// <summary>
        /// Скругление границ текущего объекта
        /// </summary>
        public new CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Инициализация базового класса визуализации объекта OPL
        /// </summary>
        protected OPLElementViewerBase()
        {
            Base_MainGridObject = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Base_MainGridObject.ColumnDefinitions.Add(new());
            Base_MainGridObject.ColumnDefinitions.Add(new() { Width = new(0d, GridUnitType.Auto) });

            Base_BorderCircleIndiator = new()
            {
                Width = 14d,
                Height = 14d,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                BorderThickness = new(2),
                CornerRadius = new(0),
                Background = SourceBackground.SourceBrush,
                BorderBrush = SourceBorderBrush.SourceBrush,
            };
            Grid.SetColumn(Base_BorderCircleIndiator, 0);
            Grid.SetColumnSpan(Base_BorderCircleIndiator, 2);
            Base_MainGridObject.Children.Add(Base_BorderCircleIndiator);

            Base_TextBlockName = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "Name",
                FontSize = 12d,
                Foreground = SourceForeground.SourceBrush,
            };
            Grid.SetColumn(Base_TextBlockName, 0);
            Base_MainGridObject.Children.Add(Base_TextBlockName);

            Base_BorderElementView = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                BorderThickness = new(2),
                CornerRadius = new(0),
                Background = SourceBackground.SourceBrush,
                BorderBrush = SourceBorderBrush.SourceBrush,
            };
            Grid.SetColumn(Base_BorderElementView, 1);
            Base_MainGridObject.Children.Add(Base_BorderElementView);

            base.SetValue(IELButtonBase.ContentProperty, Base_MainGridObject);
        }
    }
}
