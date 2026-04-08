using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OIEL.UserElementsControl;
using OIEL.UserElementsControl.Base.LabelBase;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.Windows.Base;
using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Cursors = System.Windows.Input.Cursors;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowManipulateLabelTags.xaml
    /// </summary>
    public partial class DialogManipulateLabelTags : OPLWindowBase
    {
        /// <summary>
        /// Состояние отмены
        /// </summary>
        private bool Cancel = true;

        /// <summary>
        /// Состояние активации добавления тега
        /// </summary>
        private bool StateActivateAddTag = false;

        /// <summary>
        /// Выделенный индекс тега
        /// </summary>
        private int SelectIndexTag = -1;

        /// <summary>
        /// Представление объектов массива тегов
        /// </summary>
        private StackPanel StackPanelTags;

        public DialogManipulateLabelTags()
        {
            InitializeComponent();
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Tag));
            IELButtonChangeTag.IsEnabled = false;
            IELButtonRemoveTag.IsEnabled = false;
            Opacity = 0d;
            BorderAddTag.Width = 10d;
            BorderAddTag.Cursor = Cursors.Hand;
            StackPanelTags = new()
            {
                VerticalAlignment = VerticalAlignment.Top,
            };
            ScrollViewerTags.AutoUpdateVisibleHorizontalScroll = false;
            ScrollViewerTags.Content = StackPanelTags;
            IELBlockInfoAddTag.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.LightBulb));
            BlockMessage.Opacity = 0d;
            IELButtonComplete.OnActivateMouseLeft += (sender, e) =>
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
            Activated += (sender, e) =>
            {
                if (ManagerAnimation != null)
                {
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(BackGroundRadialBrush, RadialGradientBrush.OpacityProperty, 1d, TimeSpan.FromMilliseconds(1500d));
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(this, OpacityProperty, 1d, TimeSpan.FromMilliseconds(400d));
                }
                else
                {
                    BackGroundRadialBrush.Opacity = 1d;
                    Opacity = 1d;
                }
            };
            BorderAddTag.MouseLeftButtonUp += (sender, e) =>
            {
                if (!StateActivateAddTag)
                    ActivateAddTag();
            };
            IELButtonCancelAddTag.OnActivateMouseLeft += (sender, e) =>
            {
                DiactivateAddTag();
                e.Handled = true;
            };
            IELBlockInfoAddTag.MouseHover += (sender, e) =>
            {
                BlockMessage.UsingBorderInformation(IELBlockInfoAddTag,
                    "Escape - \"Выход из добавления тега\"\n" +
                    "Enter - \"Добавить новый тег\"",
                    IEL.CORE.Enums.OrientationPositionCursor.RightUp);
            };
            IELBlockInfoAddTag.MouseLeave += (sender, e) =>
            {
                BlockMessage.CloseBorderInformation();
            };
            IELTextBoxAddTag.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        if (IELTextBoxAddTag.Text.Length > 0 &&
                        (!App.CurrentApp.DataLabelTags.Any((i) => i.ValueTag.Equals(IELTextBoxAddTag.Text)) || App.CurrentApp.DataLabelTags.Count == 0))
                        {
                            App.CurrentApp.DataLabelTags.Add(new(IELTextBoxAddTag.Text));
                            IELBlockInfoText VisualTag = AddVisualTag(IELTextBoxAddTag.Text);
                            IELTextBoxAddTag.Text = string.Empty;
                            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(VisualTag, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                        }
                        break;
                    case Key.Escape:
                        DiactivateAddTag();
                        break;
                }
                e.Handled = true;
            };
        }

        /// <summary>
        /// Активировать состояние добавления нового тега
        /// </summary>
        private void ActivateAddTag()
        {
            BorderAddTag.Cursor = Cursors.Arrow;
            IELTextBoxAddTag.Text = string.Empty;
            Keyboard.Focus(IELTextBoxAddTag);
            IELTextBoxAddTag.Focus();
            if (ManagerAnimation != null)
            {
                ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderAddTag, WidthProperty, 400d, TimeSpan.FromMilliseconds(500d));
            }
            else
            {
                BorderAddTag.Width = 400d;
            }
            StateActivateAddTag = true;
        }

        /// <summary>
        /// Диактивировать сотсояние добавление тега
        /// </summary>
        private void DiactivateAddTag()
        {
            BorderAddTag.Cursor = Cursors.Hand;
            Keyboard.ClearFocus();
            if (ManagerAnimation != null)
            {
                ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderAddTag, WidthProperty, 10d, TimeSpan.FromMilliseconds(500d));
            }
            else
            {
                BorderAddTag.Width = 10d;
            }
            StateActivateAddTag = false;
        }

        /// <summary>
        /// Отобразить окно с возможностью манипуляции над тегами
        /// </summary>
        internal void ShowManipulateTags()
        {
            BackGroundRadialBrush.Opacity = 0d;
            IELButtonChangeTag.OnActivateMouseLeft += (sender, e) =>
            {
                //if (SelectedTag == null) return;
                //new WindowGenDataLabelTag().ChangeDataTag(SelectedTag.Tag);
                //ClearSelectTag();
            };
            IELButtonRemoveTag.OnActivateMouseLeft += (sender, e) =>
            {
                if (SelectIndexTag == -1) return;
                StackPanelTags.Children.RemoveAt(SelectIndexTag);
                App.CurrentApp.DataLabelTags.RemoveAt(SelectIndexTag);
                SelectIndexTag = -1;
            };
            foreach (LabelTag Element in App.CurrentApp.DataLabelTags)
            {
                IELBlockInfoText VisualTag = AddVisualTag(Element.ValueTag);
                if (ManagerAnimation != null)
                {
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(VisualTag, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                }
                else
                {
                    VisualTag.Opacity = 1d;
                }
            }
            ShowDialog();
        }

        /// <summary>
        /// Отобразить окно с исключительной возможностью выбора одного тега
        /// </summary>
        internal LabelTag? ShowSelectOneTag()
        {
            BackGroundRadialBrush.Opacity = 0d;
            BorderAddTag.IsEnabled = false;
            BorderAddTag.Opacity = 0d;
            IELButtonChangeTag.Visibility = Visibility.Hidden;
            IELButtonRemoveTag.Visibility = Visibility.Hidden;
            foreach (LabelTag Element in App.CurrentApp.DataLabelTags)
            {
                IELBlockInfoText VisualTag = AddVisualTag(Element.ValueTag);
                if (ManagerAnimation != null)
                {
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(VisualTag, OpacityProperty, 1d, TimeSpan.FromMilliseconds(300d));
                }
                else
                {
                    VisualTag.Opacity = 1d;
                }
            }
            ShowDialog();
            return (!Cancel && SelectIndexTag > -1) ? App.CurrentApp.DataLabelTags[SelectIndexTag] : null;
        }

        /// <summary>
        /// Добавить отображение тега
        /// </summary>
        /// <param name="NewTag">Отображаемый объект тега</param>
        private IELBlockInfoText AddVisualTag(string TextTag)
        {
            IELBlockInfoText IELTag = new()
            {
                BorderThickness = new(1),
                Text = TextTag,
                CornerRadius = new(5),
                Margin = new(2),
                Padding = new(0, 2, 0, 2),
                FontSize = 17d,
                Cursor = System.Windows.Input.Cursors.Hand,
                Opacity = 0d,
                PaletteElement = App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Aquamarine],
            };
            IELTag.MouseLeftButtonUp += (sender, e) =>
            {
                IELObjectBase Obj = (IELObjectBase)sender;
                if (SelectIndexTag > -1)
                {
                    IELObjectBase BackObj = (IELObjectBase)StackPanelTags.Children[SelectIndexTag];
                    BackObj.SourceBackground.SetUsedState(false);
                    if (SelectIndexTag == StackPanelTags.Children.IndexOf(Obj)) return;
                }
                SelectIndexTag = StackPanelTags.Children.IndexOf(Obj);
                Obj.SourceBackground.SetUsedState(true);
            };
            System.Windows.Data.Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["Alphasano"]
            };
            BindingOperations.SetBinding(IELTag, IELBlockInfoText.FontFamilyProperty, binding);
            StackPanelTags.Children.Add(IELTag);
            return IELTag;
        }
    }
}
