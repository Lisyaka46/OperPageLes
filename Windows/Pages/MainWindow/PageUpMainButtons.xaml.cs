using AAC20.Interfaces;
using AAC20.Interfaces.Button;
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

namespace AAC20.Windows.Pages.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для PageUpMainButtons.xaml
    /// </summary>
    public partial class PageUpMainButtons : Page, IPageModuleButtonKeyAAC
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; }

        /// <summary>
        /// Объект данных режима клавиатуры
        /// </summary>
        private bool _KeyboardMode;

        /// <summary>
        /// Режим клавиатуры
        /// </summary>
        public bool KeyboardMode
        {
            get => _KeyboardMode;
            set
            {
                _KeyboardMode = value;
                KeyboardModeChanged.Invoke(value);
            }
        }

        /// <summary>
        /// Объект события изменения состояния Alt режима
        /// </summary>
        public IPageModuleButtonKeyAAC.Delegate_KeyboardModeChanged KeyboardModeChanged { get; private set; }

        public PageUpMainButtons()
        {
            InitializeComponent();
            PageName = nameof(PageUpMainButtons);
            KeyboardModeChanged = (Mode) =>
            {
                IELButtonLabel.CharKeyKeyboardActivate = Mode;
                IELButtonSettings.CharKeyKeyboardActivate = Mode;
            };
        }

        /// <summary>
        /// Активировать кнопку в данном элементе типа "IELButtonText" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        /// <param name="Orientation">Ориентация нажатия на кнопку</param>
        public void ActivateIELButtonTextInKey(Key key, IPageModuleButtonKeyAAC.OrientationActivate Orientation) =>
            IIELButtonKey.ActivateButtonInKey(MainGrid, key, Orientation);

        /// <summary>
        /// Активировать мерцание кнопки в данном элементе типа "IELButtonText" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        public void BlinkActivateIELButtonTextInKey(Key key, IPageModuleButtonKeyAAC.OrientationActivate Orientation) =>
            IIELButtonKey.BlinkActivateInKey(MainGrid, key, Orientation);
    }
}
