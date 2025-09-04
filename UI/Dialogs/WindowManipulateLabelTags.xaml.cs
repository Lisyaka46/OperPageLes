using OperPage_les.CORE.Label;
using OperPage_les.UI.UserElementControl;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Devices.PointOfService;

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

        /// <summary>
        /// Состояние отмены
        /// </summary>
        private bool Cancel = true;

        public WindowManipulateLabelTags()
        {
            InitializeComponent();
            Icon = App.LoadImage(Properties.Resources.Tag);
            IELButtonChangeTag.IsEnabled = false;
            IELButtonRemoveTag.IsEnabled = false;
            IELButtonComplete.OnActivateMouseLeft += (sender, e, Key) =>
            {
                Cancel = false;
                Close();
            };
            KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        Close();
                        break;
                }
            };
        }

        /// <summary>
        /// Отобразить окно с возможностью манипуляции над тегами
        /// </summary>
        internal void ShowManipulateTags()
        {
            Title = "Окно манипуляции над тегами";
            IELButtonAddTag.IsEnabled = true;
            IELButtonAddTag.OnActivateMouseLeft += (sender, e, Key) =>
            {
                LabelTag? Tag = new WindowGenDataLabelTag().GenereteTag();
                if (Tag == null) return;
                App.CurrentApp.DataLabelTags.Add(Tag);
                OPLLabelTag VisualTag = AddVisualTag(Tag);
                VisualTag.MouseLeftButtonUp += (sender, e) =>
                {
                    MouseLeftButtonUpOnManipulate(VisualTag);
                };
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
            foreach (LabelTag Element in App.CurrentApp.DataLabelTags)
            {
                OPLLabelTag VisualTag = AddVisualTag(Element);
                VisualTag.MouseLeftButtonUp += (sender, e) =>
                {
                    MouseLeftButtonUpOnManipulate(VisualTag);
                };
            }
            ShowDialog();
        }

        /// <summary>
        /// Отобразить окно с исключительной возможностью выбора одного тега
        /// </summary>
        internal LabelTag? ShowSelectOneTag()
        {
            Title = "Окно выделения тега устанавливаемого для ярлыка";
            IELButtonAddTag.IsEnabled = false;
            IELButtonAddTag.Visibility = Visibility.Hidden;
            IELButtonChangeTag.Visibility = Visibility.Hidden;
            IELButtonRemoveTag.Visibility = Visibility.Hidden;
            foreach (LabelTag Element in App.CurrentApp.DataLabelTags)
            {
                OPLLabelTag VisualTag = AddVisualTag(Element);
                VisualTag.MouseLeftButtonUp += (sender, e) =>
                {
                    MouseLeftButtonUpNotManipulate(VisualTag);
                };
            }
            ShowDialog();
            return !Cancel ? SelectedTag?.Tag : null;
        }

        /// <summary>
        /// Взаимодействие с тегом без возможности манипуляции
        /// </summary>
        /// <param name="OPLTag">Объект тега</param>
        private void MouseLeftButtonUpNotManipulate(OPLLabelTag OPLTag)
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
        }

        /// <summary>
        /// Взаимодействие с тегом с возможностью манипуляции
        /// </summary>
        /// <param name="OPLTag">Объект тега</param>
        private void MouseLeftButtonUpOnManipulate(OPLLabelTag OPLTag)
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
        }

        /// <summary>
        /// Добавить отображение тега
        /// </summary>
        /// <param name="NewTag">Отображаемый объект тега</param>
        private OPLLabelTag AddVisualTag(LabelTag NewTag)
        {
            OPLLabelTag OPLTag = OPLLabelCommand.CreateVisualTag(NewTag);
            OPLTag.IELTag.Cursor = System.Windows.Input.Cursors.Hand;
            OPLTag.Opacity = 0d;
            App.AnimateDoubleEffect(OPLTag, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
            StackPanelTags.Children.Add(OPLTag);
            return OPLTag;
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
