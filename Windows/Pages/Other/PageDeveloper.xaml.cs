using IEL.Classes;
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

namespace AAC20.Windows.Pages.Other
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : Page, IPageDefault
    {
        /// <summary>
        /// Модуль страницы
        /// </summary>
        public ModulePage ModulePage { get; }

        /// <summary>
        /// Главная страница компонента
        /// </summary>
        public Grid MainGrid => GridMain;

        public PageDeveloper()
        {
            InitializeComponent();
            ModulePage = new(nameof(PageDeveloper));
            ListBoxDeveloper.Items.Add("[0] CountVisible=*");
            ListBoxDeveloper.Items.Add("[1] ActualHeight=*");
        }
    }
}
