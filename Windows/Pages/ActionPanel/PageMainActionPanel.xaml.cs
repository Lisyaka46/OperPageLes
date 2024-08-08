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
using AAC20.Interfaces;

namespace AAC20.Windows.Frames
{
    /// <summary>
    /// Логика взаимодействия для PageMainActionPanel.xaml
    /// </summary>
    public partial class PageMainActionPanel : Page, IPageActionPanelAAC
    {
        private bool PAltMode = false;
        /// <summary>
        /// Alt режим для переключения кнопок с помощью клавиш клавиатуры
        /// </summary>
        public bool AltMode { get => PAltMode; set => AltModeChanged.Invoke(value); }

        /// <summary>
        /// Объект события изменения состояния Alt режима
        /// </summary>
        public IPageActionPanelAAC.Delegate_AltModeChanged AltModeChanged { get; private set; }

        public PageMainActionPanel()
        {
            InitializeComponent();
            AltModeChanged = (Mode) =>
            {
                IELButtonCrearConsole.CharKeyKeyboardActivate = Mode;
                IELButtonCommandBuffer.CharKeyKeyboardActivate = Mode;
                PAltMode = Mode;
            };
        }

        /// <summary>
        /// Активировать кнопку по ключу
        /// </summary>
        /// <param name="key">Ключ активации</param>
        /// <param name="KeyDownEvent">Начало нажатия на кнопку</param>
        public void ActivateInKey(Key key, bool KeyDownEvent) => GUI.IELButtonText.ActivateButtonInKey(this, key, KeyDownEvent);
    }
}
