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

namespace OperPage_les.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для Test.xaml
    /// </summary>
    public partial class Test : Page
    {
        int i = 0;
        public Test()
        {
            InitializeComponent();
            u.Text = $"Click 0";
        }

        private void ClickLeftUp(object sender, MouseButtonEventArgs e)
        {
            u.Text = $"Click {++i}";
        }
    }
}
