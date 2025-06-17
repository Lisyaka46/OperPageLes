using OperPage_les.CORE.Label;
using OperPage_les.UI.UserElementControl;
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
using Windows.ApplicationModel.UserDataTasks.DataProvider;

namespace OperPage_les.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowManipulateLabelTags.xaml
    /// </summary>
    public partial class WindowManipulateLabelTags : Window
    {
        /// <summary>
        /// Ярлык подлежащий манипуляции
        /// </summary>
        private readonly OPLLabelCommand LabelManipulate;

        /// <summary>
        /// Выделенный тег
        /// </summary>
        private OPLLabelTag? SelectedTag;

        public WindowManipulateLabelTags(OPLLabelCommand OPLLabel)
        {
            InitializeComponent();
            LabelManipulate = OPLLabel;
            for (int i = 0; i < LabelManipulate.SourceLabel.Tags.Count; i++)
            {
                AddVisualTag(LabelManipulate.SourceLabel.Tags[i]);
            }
            ImageElementLabel.Source = LabelManipulate.ImageElementLabel.Source;
            TextBlockNameLabel.Text = LabelManipulate.TextBlockNameLabel.Text;
            Icon = App.LoadImage(Properties.Resources.Tag);
            IELButtonChangeTag.IsEnabled = false;
            IELButtonRemoveTag.IsEnabled = false;
            IELButtonAddTag.OnActivateMouseLeft += (sender, e, Key) =>
            {
                LabelTag? Tag = new WindowGenDataLabelTag().GenereteTag();
                if (Tag == null) return;
                LabelManipulate.SourceLabel.AppendTag(Tag);
                AddVisualTag(Tag);
            };
            IELButtonChangeTag.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectedTag == null) return;
                new WindowGenDataLabelTag().ChangeDataTag(SelectedTag.Tag);
                ClearSelectTag();
            };
            IELButtonRemoveTag.OnActivateMouseLeft += (sender, e, Key) =>
            {
                if (SelectedTag == null) return;
                LabelManipulate.SourceLabel.RemoveTag(SelectedTag.Tag);
                StackPanelTags.Children.Remove(SelectedTag);
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
                SelectedTag?.IELSettingObject.BackgroundSetting.SetUsedState(false);
                OPLTag.IELSettingObject.BackgroundSetting.SetUsedState(true);
                SelectedTag = OPLTag;
                IELButtonChangeTag.IsEnabled = true;
                IELButtonRemoveTag.IsEnabled = true;
            };
            OPLTag.MouseRightButtonUp += (sender, e) =>
            {
                SelectedTag?.IELSettingObject.BackgroundSetting.SetUsedState(false);
                ClearSelectTag();
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
            SelectedTag = null;
            IELButtonChangeTag.IsEnabled = false;
            IELButtonRemoveTag.IsEnabled = false;
        }
    }
}
