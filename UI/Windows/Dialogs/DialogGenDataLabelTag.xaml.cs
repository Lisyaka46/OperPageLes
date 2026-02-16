using ApplicationOperPageLes.CORE.Struct;
using IEL.CORE.Enums;
using OIEL.UserElementsControl.Base.LabelBase;
using System.Windows;
using Key = System.Windows.Input.Key;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowGenDataLabelTag.xaml
    /// </summary>
    public partial class DialogGenDataLabelTag : Window
    {
        private bool Cancel = true;
        public DialogGenDataLabelTag()
        {
            InitializeComponent();
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Tag));
            IELButtonClose.OnActivateMouseLeft += (sender, e) =>
            {
                Close();
            };
            IELButtonComplete.OnActivateMouseLeft += (sender, e) =>
            {
                CompleteEditTag();
            };
            IELTextBoxNameTag.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        CompleteEditTag();
                        break;
                }
            };
            Loaded += (sender, e) =>
            {
                IELTextBoxNameTag.Focus();
            };
        }

        /// <summary>
        /// Проверить текст на подход для тегов
        /// </summary>
        /// <returns>true при успешной проверке</returns>
        private bool Check()
        {
            if (IELTextBoxNameTag.Text.Length == 0) return false;
            else if (App.CurrentApp.DataLabelTags.Any(i => i.ValueTag.Equals(IELTextBoxNameTag.Text))) return false;
            return true;
        }

        /// <summary>
        /// Итоговая функция редактирования и создания тега
        /// </summary>
        private void CompleteEditTag()
        {
            if (!Check())
            {
                IELTextBoxNameTag.SetActiveSpecrum(StateSpectrum.Default, true);
                return;
            }
            Cancel = false;
            Close();
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
