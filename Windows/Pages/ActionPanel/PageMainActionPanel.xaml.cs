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
        /// Модуль страницы
        /// </summary>
        public ModulePageKey ModulePage { get; }

        /// <summary>
        /// Главная страница компонента
        /// </summary>
        public Grid MainGrid => GridMain;

        public PageMainActionPanel()
        {
            InitializeComponent();
            ModulePage = new(nameof(PageMainActionPanel))
            {
                KeyboardModeChanged = (Mode) =>
                {
                    IELButtonCrearConsole.CharKeyboardActivate = Mode;
                    IELButtonCommandBuffer.CharKeyboardActivate = Mode;
                    IELButtonDiscriptionCommand.CharKeyboardActivate = Mode;
                }
            };
        }
    }
}
