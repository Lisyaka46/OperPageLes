using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System.Windows;
using System.Windows.Media;

namespace OperPageLes.UI.UserElementsControl.Theme
{
    /// <summary>
    /// Логика взаимодействия для OPLThemeFile.xaml
    /// </summary>
    public partial class OPLThemeFile : IEL.UserElementsControl.Base.IELContainerBase, IOPLAnimate
    {
        #region Properties

        #region IsActivate
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty IsActivateProperty =
            DependencyProperty.Register("IsActivate", typeof(bool), typeof(OPLThemeFile),
                new(false,
                    (sender, e) =>
                    {
                        OPLThemeFile Element = (OPLThemeFile)sender;
                        OPLAnimationManager.AnimateTakingZeroTo(Element.ManagerAnimation, Element.BorderActivateIndicator, WidthProperty,
                            (bool)e.NewValue ? 10d : 0d, TimeSpan.FromMilliseconds(400));
                    }));

        /// <summary>
        /// Состояние активации элемента
        /// </summary>
        public bool IsActivate
        {
            get => (bool)GetValue(IsActivateProperty);
            set => SetValue(IsActivateProperty, value);
        }
        #endregion

        #region TextNameFile
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextNameFileProperty =
            DependencyProperty.Register("TextNameFile", typeof(string), typeof(OPLThemeFile),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLThemeFile)sender).TextBlockNameFile.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст отображаемого имени файла
        /// </summary>
        public string TextNameFile
        {
            get => (string)GetValue(TextNameFileProperty);
            set => SetValue(TextNameFileProperty, value);
        }
        #endregion

        #region SourceElement
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty SourceElementProperty =
            DependencyProperty.Register("SourceElement", typeof(ImageSource), typeof(OPLThemeFile),
                new(
                    (sender, e) =>
                    {
                        ((OPLThemeFile)sender).ImageIcon.Source = (ImageSource)e.NewValue;
                    }));

        /// <summary>
        /// Данные отображаемой иконки
        /// </summary>
        public ImageSource SourceElement
        {
            get => (ImageSource)GetValue(SourceElementProperty);
            set => SetValue(SourceElementProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        public OPLThemeFile()
        {
            InitializeComponent();
            Base_BorderContainer.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            TextBlockNameFile.Text = String.Empty;
            BorderActivateIndicator.Width = 0d;
            BorderActivateIndicator.Background = SourceBorderBrush.SourceBrush;
            BorderIcon.BorderBrush = SourceBorderBrush.SourceBrush;

            TextBlockNameFile.Foreground = SourceForeground.SourceBrush;
        }
    }
}
