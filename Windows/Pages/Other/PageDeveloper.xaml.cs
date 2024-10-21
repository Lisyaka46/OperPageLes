using IEL.Interfaces.Core;
using System.Windows.Controls;

namespace AAC20.Windows.Pages.Other
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : Page, IPageDefault
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageDeveloper);

        /// <summary>
        /// Главная страница компонента
        /// </summary>
        public Grid MainGrid => GridMain;

        public PageDeveloper()
        {
            InitializeComponent();
            ListBoxDeveloper.Items.Add("[0] CountVisible=*");
            ListBoxDeveloper.Items.Add("[1] ActualHeight=*");
        }
    }
}
