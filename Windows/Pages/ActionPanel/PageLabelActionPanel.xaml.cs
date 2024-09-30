using System.Windows.Controls;
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
        /// Модуль страницы
        /// </summary>
        public ModulePageKey ModulePage { get; }

        /// <summary>
        /// Главная страница компонента
        /// </summary>
        public Grid MainGrid => GridMain;

        public PageLabelActionPanel()
        {
            InitializeComponent();
            ModulePage = new(nameof(PageLabelActionPanel))
            {
                KeyboardModeChanged = (Mode) =>
                {
                    IELButtonExecuteLabel.CharKeyboardActivate = Mode;
                    IELButtonChangeLabel.CharKeyboardActivate = Mode;
                    IELButtonMovingLabel.CharKeyboardActivate = Mode;
                }
            };
        }
    }
}
