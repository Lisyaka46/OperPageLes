using AAC20.GUI;
using AAC20.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace AAC20.Windows.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageBufferActionPanel.xaml
    /// </summary>
    public partial class PageBufferActionPanel : Page, IPageActionPanelAAC
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

        public PageBufferActionPanel()
        {
            InitializeComponent();
            AltModeChanged = (Mode) =>
            {
                IELButtonBackMainMenu.CharKeyKeyboardActivate = Mode;
                PAltMode = Mode;
            };
        }

        /// <summary>
        /// Активировать кнопку по ключу
        /// </summary>
        /// <param name="key">Ключ активации</param>
        public void ActivateInKey(Key key) => App.ActivateButtonInKey(this, key);
    }
}
