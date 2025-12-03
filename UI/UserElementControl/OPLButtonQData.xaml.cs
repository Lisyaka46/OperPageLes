using ApplicationOperPageLes.UI.Windows.Dialogs;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using Cursors = System.Windows.Input.Cursors;
using WnColor = System.Windows.Media.Color;

namespace ApplicationOperPageLes.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLButtonQData.xaml
    /// </summary>
    public partial class OPLButtonQData : IELButtonBase
    {
        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLButtonQData),
                new(
                    (sender, e) =>
                    {
                        ((OPLButtonQData)sender).TextBlockName.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст названия
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region CornerRadius
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(OPLButtonQData),
                new(
                    (sender, e) =>
                    {
                        OPLButtonQData Element = (OPLButtonQData)sender;
                        Element.BorderColorDefault.CornerRadius = (CornerRadius)e.NewValue;
                        Element.BorderColorSelect.CornerRadius = (CornerRadius)e.NewValue;
                        Element.BorderColorUsed.CornerRadius = (CornerRadius)e.NewValue;
                        Element.BorderColorNotEnabled.CornerRadius = (CornerRadius)e.NewValue;

                        Element.SetValue(IELButtonBase.CornerRadiusProperty, (CornerRadius)e.NewValue);
                    }));

        /// <summary>
        /// Скругление границ
        /// </summary>
        public new CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #region BorderThickness
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(OPLButtonQData),
                new(
                    (sender, e) =>
                    {
                        OPLButtonQData Element = (OPLButtonQData)sender;
                        Element.BorderColorDefault.BorderThickness = (Thickness)e.NewValue;
                        Element.BorderColorSelect.BorderThickness = (Thickness)e.NewValue;
                        Element.BorderColorUsed.BorderThickness = (Thickness)e.NewValue;
                        Element.BorderColorNotEnabled.BorderThickness = (Thickness)e.NewValue;

                        Element.SetValue(IELButtonBase.BorderThicknessProperty, (Thickness)e.NewValue);
                    }));

        /// <summary>
        /// Скругление границ
        /// </summary>
        public new Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }
        #endregion

        #region QDataBackground
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(QData), typeof(OPLButtonQData),
                new(
                    (sender, e) =>
                    {
                        OPLButtonQData Element = (OPLButtonQData)sender;
                        Element.SetValue(IELContainerBase.BackgroundProperty, e.NewValue);
                        Element.UpdateVisualQDataSpectrum();
                    }));

        /// <summary>
        /// Данные отображения фона
        /// </summary>
        public new QData Background
        {
            get => (QData)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        #endregion

        #region QDataBorderBrush
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(QData), typeof(OPLButtonQData),
                new(
                    (sender, e) =>
                    {
                        OPLButtonQData Element = (OPLButtonQData)sender;
                        Element.SetValue(IELContainerBase.BorderBrushProperty, e.NewValue);
                        Element.UpdateVisualQDataSpectrum();
                    }));

        /// <summary>
        /// Данные отображения границ
        /// </summary>
        public new QData BorderBrush
        {
            get => (QData)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }
        #endregion

        #region QDataForeground
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(QData), typeof(OPLButtonQData),
                new(
                    (sender, e) =>
                    {
                        OPLButtonQData Element = (OPLButtonQData)sender;
                        Element.SetValue(IELContainerBase.ForegroundProperty, e.NewValue);
                        Element.UpdateVisualQDataSpectrum();
                    }));

        /// <summary>
        /// Данные отображения текста
        /// </summary>
        public new QData Foreground
        {
            get => (QData)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        #endregion

        public OPLButtonQData()
        {
            InitializeComponent();
            BorderColorDefault.Background = new SolidColorBrush(Colors.Black);
            BorderColorSelect.Background = new SolidColorBrush(Colors.Black);
            BorderColorUsed.Background = new SolidColorBrush(Colors.Black);
            BorderColorNotEnabled.Background = new SolidColorBrush(Colors.Black);

            BorderColorDefault.BorderBrush = new SolidColorBrush(Colors.Black);
            BorderColorSelect.BorderBrush = new SolidColorBrush(Colors.Black);
            BorderColorUsed.BorderBrush = new SolidColorBrush(Colors.Black);
            BorderColorNotEnabled.BorderBrush = new SolidColorBrush(Colors.Black);

            TextBlockDefault.Foreground = new SolidColorBrush(Colors.Black);
            TextBlockSelect.Foreground = new SolidColorBrush(Colors.Black);
            TextBlockUsed.Foreground = new SolidColorBrush(Colors.Black);
            TextBlockNotEnabled.Foreground = new SolidColorBrush(Colors.Black);

            BorderColorDefault.Cursor = Cursors.Hand;
            BorderColorSelect.Cursor = Cursors.Hand;
            BorderColorUsed.Cursor = Cursors.Hand;
            BorderColorNotEnabled.Cursor = Cursors.Hand;

            BorderColorDefault.MouseUp += (sender, e) =>
            {
                DialogQDataSpectrum g = new();
                g.ShowDialogChangeQData(SourceBackground.Source, SourceBorderBrush.Source, SourceForeground.Source, QData.EnumDataSpectrum.Default);
                UpdateVisualQDataSpectrum();
            };
            BorderColorSelect.MouseUp += (sender, e) =>
            {
                DialogQDataSpectrum g = new();
                g.ShowDialogChangeQData(SourceBackground.Source, SourceBorderBrush.Source, SourceForeground.Source, QData.EnumDataSpectrum.Select);
                UpdateVisualQDataSpectrum();
            };
            BorderColorUsed.MouseUp += (sender, e) =>
            {
                DialogQDataSpectrum g = new();
                g.ShowDialogChangeQData(SourceBackground.Source, SourceBorderBrush.Source, SourceForeground.Source, QData.EnumDataSpectrum.Used);
                UpdateVisualQDataSpectrum();
            };
            BorderColorNotEnabled.MouseUp += (sender, e) =>
            {
                DialogQDataSpectrum g = new();
                g.ShowDialogChangeQData(SourceBackground.Source, SourceBorderBrush.Source, SourceForeground.Source, QData.EnumDataSpectrum.NotEnabled);
                UpdateVisualQDataSpectrum();
            };
        }

        internal void UpdateVisualQDataSpectrum()
        {
            ((SolidColorBrush)BorderColorDefault.Background).Color = SourceBackground.Source.Default;
            ((SolidColorBrush)BorderColorSelect.Background).Color = SourceBackground.Source.Select;
            ((SolidColorBrush)BorderColorUsed.Background).Color = SourceBackground.Source.Used;
            ((SolidColorBrush)BorderColorNotEnabled.Background).Color = SourceBackground.Source.NotEnabled;

            ((SolidColorBrush)BorderColorDefault.BorderBrush).Color = SourceBorderBrush.Source.Default;
            ((SolidColorBrush)BorderColorSelect.BorderBrush).Color = SourceBorderBrush.Source.Select;
            ((SolidColorBrush)BorderColorUsed.BorderBrush).Color = SourceBorderBrush.Source.Used;
            ((SolidColorBrush)BorderColorNotEnabled.BorderBrush).Color = SourceBorderBrush.Source.NotEnabled;

            ((SolidColorBrush)TextBlockDefault.Foreground).Color = SourceForeground.Source.Default;
            ((SolidColorBrush)TextBlockSelect.Foreground).Color = SourceForeground.Source.Select;
            ((SolidColorBrush)TextBlockUsed.Foreground).Color = SourceForeground.Source.Used;
            ((SolidColorBrush)TextBlockNotEnabled.Foreground).Color = SourceForeground.Source.NotEnabled;
        }
    }
}
