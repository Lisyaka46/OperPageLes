using OperPage_les.CORE.Label;
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
using System.Windows.Shapes;

namespace OperPage_les.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowGenDataLabelTag.xaml
    /// </summary>
    public partial class WindowGenDataLabelTag : Window
    {
        private bool Cancel = true;
        public WindowGenDataLabelTag()
        {
            InitializeComponent();
            Icon = App.LoadImage(Properties.Resources.Tag);
            IELButtonClose.OnActivateMouseLeft += (sender, Key) =>
            {
                Close();
            };
            IELButtonComplete.OnActivateMouseLeft += (sender, Key) =>
            {
                if (IELTextBoxNameTag.Text.Length == 0)
                {
                    App.AnimateColorEffect(IELTextBoxNameTag.Background, SolidColorBrush.ColorProperty,
                        Colors.Red, IELTextBoxNameTag.IELSettingObject.BackgroundSetting.Default,
                        TimeSpan.FromMilliseconds(IELTextBoxNameTag.IELSettingObject.AnimationMillisecond));
                    return;
                }
                Cancel = false;
                Close();
            };
        }

        /// <summary>
        /// Сгенерировать тег для ярлыка
        /// </summary>
        /// <returns>Объект тега</returns>
        public LabelTag? GenereteTag()
        {
            TextBlockTitle.Text = "Создание тега для ярлыка";
            IELButtonComplete.Text = "Создать";
            ShowDialog();
            return Cancel ? null : new LabelTag(IELTextBoxNameTag.Text);
        }

        /// <summary>
        /// Сгенерировать тег для ярлыка
        /// </summary>
        /// <returns>Объект тега</returns>
        public void ChangeDataTag(LabelTag Tag)
        {
            TextBlockTitle.Text = "Изменение тега в ярлыке";
            IELButtonComplete.Text = "Изменить";
            IELTextBoxNameTag.Text = Tag.ValueTag;
            ShowDialog();
            if (Cancel) return;
            Tag.ValueTag = IELTextBoxNameTag.Text;
        }
    }
}
