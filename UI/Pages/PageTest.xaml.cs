using System.Windows.Controls;

namespace Test.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageTest.xaml
    /// </summary>
    public partial class PageTest : Page
    {
        public PageTest()
        {
            InitializeComponent();
            Button.Click += (sender, e) =>
            {
                Label1.Content = "Realy 2";
            };
        }
    }
}
