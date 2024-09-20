using AAC20.Interfaces;
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
        /// Объект данных Alt-режима
        /// </summary>
        private bool _AltMode;

        /// <summary>
        /// Объект события изменения состояния Alt режима
        /// </summary>
        public IPageModuleButtonKeyAAC.Delegate_AltModeChanged AltModeChanged { get; private set; }

        public PageUpMainButtons()
        {
            InitializeComponent();
            AltModeChanged = (Mode) =>
            {
                IELButtonLabel.CharKeyKeyboardActivate = Mode;
                IELButtonSettings.CharKeyKeyboardActivate = Mode;
            };
        }

        /// <summary>
        /// Узнать состояние Alt-режима
        /// </summary>
        /// <returns>Состояние</returns>
        public bool GetAltMode() => _AltMode;

        /// <summary>
        /// Изменить состояние Alt-режима
        /// </summary>
        /// <param name="value">Значение</param>
        public void SetAltMode(bool value) => _AltMode = value;

        /// <summary>
        /// Активировать кнопку в данном элементе типа "IELButtonText" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        /// <param name="Orientation">Ориентация нажатия на кнопку</param>
        public void ActivateIELButtonTextInKey(Key key, IPageModuleButtonKeyAAC.OrientationActivate Orientation) =>
            IIELObjectKey.ActivateButtonInKey(MainGrid, key, Orientation);

        /// <summary>
        /// Активировать мерцание кнопки в данном элементе типа "IELButtonText" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        public void BlinkActivateIELButtonTextInKey(Key key, IPageModuleButtonKeyAAC.OrientationActivate Orientation) =>
            IIELObjectKey.BlinkActivateInKey(MainGrid, key, Orientation);
    }
}
