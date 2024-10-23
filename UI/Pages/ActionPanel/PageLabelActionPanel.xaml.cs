using System.Windows.Controls;
using AAC20.Windows.Frames;
using IEL.Classes;
using IEL.Interfaces.Core;

namespace AAC20.Windows.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageLabelActionPanel.xaml
    /// </summary>
    public partial class PageLabelActionPanel : Page, IPageKey
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageLabelActionPanel);

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

        public PageLabelActionPanel()
        {
            InitializeComponent();
            KeyboardModeChanged = (Mode) =>
            {
                IELButtonExecuteLabel.CharKeyboardActivate = Mode;
                IELButtonChangeLabel.CharKeyboardActivate = Mode;
                IELButtonMovingLabel.CharKeyboardActivate = Mode;
            };
        }
    }
}
