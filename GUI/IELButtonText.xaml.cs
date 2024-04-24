using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Auxiliary_Access_Console_DL_20.GUI
{
    /// <summary>
    /// Логика взаимодействия для IELButtonText.xaml
    /// </summary>
    public partial class IELButtonText : UserControl
    {
        // Background
        private Brush _BgButton;

        /// <summary>
        /// Цвет фона при не активном состоянии
        /// </summary>
        public Brush BgButton
        {
            get { return _BgButton; }
            set
            {
                _BgButton = value;
                BorderButton.Background = value;
            }
        }

        /// <summary>
        /// Цвет фона при наведении мышью на элемент
        /// </summary>
        public Brush BgMouseActive { get; set; }

        /// <summary>
        /// Цвет фона при нажатии мышью на элемент
        /// </summary>
        public Brush BgMouseClick { get; set; }

        // BorderBrush
        private Brush _BbButton;

        /// <summary>
        /// Цвет границ при не активном состоянии
        /// </summary>
        public Brush BbButton
        {
            get { return _BbButton; }
            set
            {
                _BbButton = value;
                BorderButton.BorderBrush = value;
            }
        }

        /// <summary>
        /// Цвет границ при наведении мышью на элемент
        /// </summary>
        public Brush BbMouseActive { get; set; }

        /// <summary>
        /// Цвет границ при нажатии мышью на элемент
        /// </summary>
        public Brush BbMouseClick { get; set; }

        // Foreground
        private Brush _FgButton;

        /// <summary>
        /// Цвет текста при не активном состоянии
        /// </summary>
        public Brush FgButton
        {
            get { return _FgButton; }
            set
            {
                _FgButton = value;
                RealyButton.Foreground = value;
            }
        }

        /// <summary>
        /// Цвет текста при наведении мышью на элемент
        /// </summary>
        public Brush FgMouseActive { get; set; }

        /// <summary>
        /// Цвет текста при нажатии мышью на элемент
        /// </summary>
        public Brush FgMouseClick { get; set; }

        // Text
        /// <summary>
        /// Текст который отображается при не активном состоянии элемента
        /// </summary>
        public string Text
        {
            get { return RealyButton.Text; }
            set { RealyButton.Text = value; }
        }

        /// <summary>
        /// Текст отображаемый при наведении мышью на элемент
        /// </summary>
        public string? TextMouseActive { get; set; }

        public IELButtonText()
        {
            InitializeComponent();
            _BbButton = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            BorderButton.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            BbMouseActive = new SolidColorBrush(Color.FromRgb(20, 20, 20));
            BbMouseClick = new SolidColorBrush(Color.FromRgb(40, 40, 40));

            _BgButton = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            BorderButton.Background = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            BgMouseActive = new SolidColorBrush(Color.FromRgb(80, 80, 80));
            BgMouseClick = new SolidColorBrush(Color.FromRgb(120, 120, 120));

            _FgButton = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            RealyButton.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            FgMouseActive = new SolidColorBrush(Color.FromRgb(235, 235, 235));
            FgMouseClick = new SolidColorBrush(Color.FromRgb(215, 215, 215));

            Text = "Кнопка";

            GridButton.MouseEnter += (sender, e) =>
            {
                BorderButton.Background = BgMouseActive;
                BorderButton.BorderBrush = BbMouseActive;
                RealyButton.Foreground = FgMouseActive;
                if (TextMouseActive != null) RealyButton.Text = TextMouseActive;
            };

            GridButton.MouseLeave += (sender, e) =>
            {
                BorderButton.Background = _BgButton;
                BorderButton.BorderBrush = _BbButton;
                RealyButton.Foreground = _FgButton;
                if (TextMouseActive != null) RealyButton.Text = Text;
            };

            GridButton.MouseDown += (sender, e) =>
            {
                BorderButton.Background = BgMouseClick;
                BorderButton.BorderBrush = BbMouseClick;
                RealyButton.Foreground = FgMouseClick;
                RealyButton.Focus();
            };

            GridButton.MouseUp += (sender, e) =>
            {
                BorderButton.Background = _BgButton;
                BorderButton.BorderBrush = _BbButton;
                RealyButton.Foreground = _FgButton;
                if (TextMouseActive != null) RealyButton.Text = Text;
            };
        }
    }
}
