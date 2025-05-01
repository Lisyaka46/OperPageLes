using OperPage_les.Windows.Pages.ActionPanel;
using IEL.Interfaces.Core;
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

namespace OperPage_les.UI.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageLabelMainActionPanel.xaml
    /// </summary>
    public partial class PageLabelMainActionPanel : Page, IPageKey
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageLabelMainActionPanel);

        /// <summary>
        /// Объект данных режима клавиатуры
        /// </summary>
        private bool _KeyboardMode = false;

        /// <summary>
        /// Режим клавиатуры
        /// </summary>
        public bool KeyboardMode
        {
            get => _KeyboardMode;
            set
            {
                _KeyboardMode = value;
                KeyboardModeChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Объект события изменения состояния Alt режима
        /// </summary>
        public IPageKey.Delegate_KeyboardModeChanged? KeyboardModeChanged { get; set; }

        public PageLabelMainActionPanel()
        {
            InitializeComponent();
            KeyboardModeChanged = (Mode) =>
            {
                IELButtonCreateLabel.CharKeyboardActivate = Mode;
                IELButtonImportLabel.CharKeyboardActivate = Mode;
                IELButtonExportLabel.CharKeyboardActivate = Mode;
            };
        }
    }
}
