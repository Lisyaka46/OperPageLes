using OperPage_les.CORE.Label;
using OperPage_les.UI.UserElementControl;
using System.Windows;

namespace OperPage_les.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowManipulateLabelTags.xaml
    /// </summary>
    public partial class WindowManipulateLabelTags : Window
    {
        /// <summary>
        /// Выделенный тег
        /// </summary>
        private OPLLabelTag? SelectedTag;

        public WindowManipulateLabelTags()
        {
            InitializeComponent();
            Icon = App.LoadImage(Properties.Resources.Tag);
            IELButtonChangeTag.IsEnabled = false;
            IELButtonRemoveTag.IsEnabled = false;
            foreach (LabelTag Element in App.CurrentApp.DataLabelTags)
            {
                AddVisualTag(Element);
            }
            IELButtonAddTag.OnActivateMouseLeft += (sender, e, Key) =>
            {
                LabelTag? Tag = new WindowGenDataLabelTag().GenereteTag();
                if (Tag == null) return;
                App.CurrentApp.DataLabelTags.Add(Tag);
                AddVisualTag(Tag);
            };
            IELButtonChangeTag.OnActivateMouseLeft += (sender, e, Key) =>
            {
                //if (SelectedTag == null) return;
                //new WindowGenDataLabelTag().ChangeDataTag(SelectedTag.Tag);
                //ClearSelectTag();
            };
            IELButtonRemoveTag.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectedTag == null) return;
                StackPanelTags.Children.Remove(SelectedTag);
                App.CurrentApp.DataLabelTags.Remove(SelectedTag.Tag);
                ClearSelectTag();
            };
            IELButtonComplete.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Close();
            };
        }

        /// <summary>
        /// Добавить отображение тега
        /// </summary>
        /// <param name="NewTag">Отображаемый объект тега</param>
        private void AddVisualTag(LabelTag NewTag)
        {
            OPLLabelTag OPLTag = OPLLabelCommand.CreateVisualTag(NewTag);
            OPLTag.IELTag.Cursor = System.Windows.Input.Cursors.Hand;

            OPLTag.MouseLeftButtonUp += (sender, e) =>
            {
                if (SelectedTag != null)
                {
                    ClearSelectTag();
                    if (SelectedTag.GetHashCode().Equals(OPLTag.GetHashCode()))
                    {
                        SelectedTag = null;
                        return;
                    }
                }
                SelectedTag = OPLTag;
                bool UsedState = SelectedTag.IELSettingObject.BackgroundSetting.GetUsedState();
                SelectedTag.IELSettingObject.BackgroundSetting.SetUsedState(!UsedState);
                IELButtonChangeTag.IsEnabled = !UsedState;
                IELButtonRemoveTag.IsEnabled = !UsedState;
            };
            OPLTag.Opacity = 0d;
            App.AnimateDoubleEffect(OPLTag, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            StackPanelTags.Children.Add(OPLTag);
        }

        /// <summary>
        /// Очистить выделение тега
        /// </summary>
        private void ClearSelectTag()
        {
            SelectedTag?.IELSettingObject.BackgroundSetting.SetUsedState(false);
            IELButtonChangeTag.IsEnabled = false;
            IELButtonRemoveTag.IsEnabled = false;
        }
    }
}
