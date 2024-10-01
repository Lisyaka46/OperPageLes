using IEL.Classes;
using System.Windows;

namespace AAC20.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowGenLabel.xaml
    /// </summary>
    public partial class WindowGenLabel : Window
    {
        /// <summary>
        /// Состояние отмены создания ярлыка
        /// </summary>
        private bool Cancel = false;

        public WindowGenLabel()
        {
            InitializeComponent();
            Width = 230;
            Height = 230;
            IELButtonCancel.OnActivateMouseLeft += delegate ()
            {
                Cancel = true;
                Close();
            };
            Closed += (sender, e) =>
            {

            };
        }

        //
        internal LabelAction? CreateLabel()
        {
            ShowDialog();
            if (Cancel) return null;
            return new("", "", "");
        }
    }
}
