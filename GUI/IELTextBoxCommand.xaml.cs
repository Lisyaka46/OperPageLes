using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AAC20.GUI
{
    /// <summary>
    /// Логика взаимодействия для IELTextBoxCommand.xaml
    /// </summary>
    public partial class IELTextBoxCommand : UserControl
    {
        /// <summary>
        /// Скруглённость границ объекта
        /// </summary>
        public CornerRadius CornerRadius
        {
            get
            {
                return TextBoxBorder.CornerRadius;
            }
            set
            {
                TextBoxBorder.CornerRadius = value;
            }
        }

        /// <summary>
        /// Толщина границ объекта
        /// </summary>
        public Thickness TextBorderThickness
        {
            get
            {
                return TextBoxBorder.BorderThickness;
            }
            set
            {
                TextBoxBorder.BorderThickness = value;
            }
        }

        /// <summary>
        /// Размер текста в элементе
        /// </summary>
        public double TextFontSize
        {
            get
            {
                return TextBoxMain.FontSize;
            }
            set
            {
                TextBoxMain.FontSize = value;
            }
        }

        /// <summary>
        /// Фон элемента
        /// </summary>
        public Brush TextBackground
        {
            get
            {
                return TextBoxBorder.Background;
            }
            set
            {
                TextBoxBorder.Background = value;
            }
        }

        private Color _Foreground;
        /// <summary>
        /// Фон текста в элементе
        /// </summary>
        public new Color Foreground
        {
            get => _Foreground;
            set
            {
                TextBoxMain.Foreground = new SolidColorBrush(value);
                _Foreground = value;
            }
        }

        private Color _SelectionBrush;
        /// <summary>
        /// Фон выделенного текста в элементе
        /// </summary>
        public Color SelectionBrush
        {
            get => _SelectionBrush;
            set
            {
                TextBoxMain.SelectionBrush = new SolidColorBrush(value);
                _SelectionBrush = value;
            }
        }

        private Color _TextBorderBrush;
        /// <summary>
        /// Фон элемента
        /// </summary>
        public Color TextBorderBrush
        {
            get => _TextBorderBrush;
            set
            {
                TextBoxBorder.BorderBrush = new SolidColorBrush(value);
                _TextBorderBrush = value;
            }
        }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text
        {
            get => TextBoxMain.Text;
            set => TextBoxMain.Text = value;
        }

        public IELTextBoxCommand()
        {
            InitializeComponent();

            TextBoxBorder.BorderBrush = new SolidColorBrush(Colors.Black);

            TextBackground = new SolidColorBrush(Colors.White);
            Foreground = Colors.Black;
            SelectionBrush = Colors.Gray;
            TextBorderBrush = Colors.Black;
            BorderThickness = new(1, 1, 1, 1);
            CornerRadius = new(1, 1, 1, 1);
            TextFontSize = 14;

            GotKeyboardFocus += (sender, e) =>
            {
                TextBoxBorder.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                    new ColorAnimation(Color.FromRgb(4, 68, 101), TimeSpan.FromMilliseconds(150)));
            };
            LostKeyboardFocus += (sender, e) =>
            {
                TextBoxBorder.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                    new ColorAnimation(Colors.Black, TimeSpan.FromMilliseconds(150)));
            };
        }
    }
}
