using System.Windows.Controls;
using IEL.Interfaces.Core;
using IEL;
using IEL.Classes;

namespace AAC20.Windows.Frames
{
    /// <summary>
    /// Логика взаимодействия для PageMainActionPanel.xaml
    /// </summary>
    public partial class PageMainActionPanel : Page, IPageKey
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageMainActionPanel);

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

        public PageMainActionPanel()
        {
            InitializeComponent();
            KeyboardModeChanged = (Mode) =>
            {
                IELButtonCrearConsole.CharKeyboardActivate = Mode;
                IELButtonCommandBuffer.CharKeyboardActivate = Mode;
                IELButtonDiscriptionCommand.CharKeyboardActivate = Mode;
            };
        }
    }
}
