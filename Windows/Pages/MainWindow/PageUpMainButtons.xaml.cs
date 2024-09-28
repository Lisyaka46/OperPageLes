using System.Windows.Controls;
using IEL.Interfaces.Core;
using IEL;
using IEL.Classes;

namespace AAC20.Windows.Pages.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для PageUpMainButtons.xaml
    /// </summary>
    public partial class PageUpMainButtons : Page, IPageKey
    {
        /// <summary>
        /// Модуль страницы
        /// </summary>
        public ModulePageKey ModulePage { get; }

        /// <summary>
        /// Главная страница компонента
        /// </summary>
        public Grid MainGrid => GridMain;

        public PageUpMainButtons()
        {
            InitializeComponent();
            ModulePage = new(nameof(PageUpMainButtons))
            {
                KeyboardModeChanged = (Mode) =>
                {
                    IELButtonLabel.CharKeyKeyboardActivate = Mode;
                    IELButtonSettings.CharKeyKeyboardActivate = Mode;
                }
            };
        }
    }
}
