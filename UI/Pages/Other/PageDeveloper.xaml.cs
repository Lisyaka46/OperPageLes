using AAC20.Windows.Frames;
using IEL.Classes;
using IEL.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public PageDeveloper()
        {
            InitializeComponent();
            ListBoxDeveloper.Items.Add("[0] CountVisible=*");
            ListBoxDeveloper.Items.Add("[1] ActualHeight=*");
        }
    }
}
