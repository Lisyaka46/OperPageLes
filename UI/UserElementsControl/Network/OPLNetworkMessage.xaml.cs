using ApplicationOperPageLes.CORE.Network;
using ApplicationOperPageLes.UI.UserElementsControl.Network;
using OIEL.UserElementsControl.Base;
using System.Windows;
using System.Windows.Controls;

namespace ApplicationOperPageLes.UI.UserElementsControl.Network
{
    /// <summary>
    /// Логика взаимодействия для OPLNetworkMessage.xaml
    /// </summary>
    public partial class OPLNetworkMessage : OPLNetworkElementViewerBase
    {
        #region Properties

        #region Message
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(OPLNetworkMessage),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLNetworkMessage)sender).TextBlockMessage.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Сообщение хранящееся в объекте
        /// </summary>
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        #endregion

        #region BorderThicknessClipContent
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty BorderThicknessClipContentProperty =
            DependencyProperty.Register("BorderThicknessClipContentProperty", typeof(Thickness), typeof(OPLNetworkMessage),
                new(new Thickness(0),
                    (sender, e) =>
                    {
                        ((OPLNetworkMessage)sender).BorderClipElements.BorderThickness = (Thickness)e.NewValue;
                    }));

        /// <summary>
        /// Толщина границы прикреплённого контента к сообщению
        /// </summary>
        public int BorderThicknessClipContent
        {
            get => (int)GetValue(BorderThicknessClipContentProperty);
            set
            {
                SetValue(BorderThicknessClipContentProperty, value);
            }
        }
        #endregion

        #endregion

        public OPLNetworkMessage()
        {
            InitializeComponent();
            BorderClipElements.BorderThickness = new(0);
            TextBlockMessage.Text = string.Empty;
        }

        /// <summary>
        /// Установить визуализацию объекта сообветственно данным
        /// </summary>
        /// <param name="NetworkInfo">Данные о передаваемых данных</param>
        internal void SetVisualFromNetworkInfo(DataNetworkInfo NetworkInfo, UIElementCollection? ClipCollection = null)
        {
            if (NetworkInfo.LengthMessage > 0)
            {
                TextBlockMessage.Height = double.NaN;
                TextBlockMessage.Margin = new(5);
            }
            else
            {
                TextBlockMessage.Height = 0d;
                TextBlockMessage.Margin = new(0);
                TextBlockMessage.Text = string.Empty;
            }
            if (NetworkInfo.CountFilesData > 0)
            {
                OPLNetworkClipElement Element;
                StackPanelClip.Children.Clear();
                if (ClipCollection != null)
                {
                    for (int i = 0; i < NetworkInfo.CountFilesData; i++)
                    {
                        Element = (OPLNetworkClipElement)ClipCollection[0];
                        ClipCollection.RemoveAt(0);
                        Element.Margin = new(0);
                        Element.Opacity = 0d;
                        StackPanelClip.Children.Add(Element);
                        App.ManagerAnimation.DoubleAnimationType.AnimateEffect(Element, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
                        App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(Element, MarginProperty, new(3), TimeSpan.FromMilliseconds(500d));
                    }
                }
                else
                {
                    for (int i = 0; i < NetworkInfo.CountFilesData; i++)
                    {
                        Element = new()
                        {
                            PaletteElement = App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Aquamarine],
                            Opacity = 0d,
                            CornerRadius = new(5),
                            Margin = new(0),
                        };
                        StackPanelClip.Children.Add(Element);
                    }
                }
            }
        }

        /// <summary>
        /// Прикрепить объект к сообщению
        /// </summary>
        /// <param name="Source">Прикрепляемый элемент к сообщению</param>
        /// <returns></returns>
        public void ClipObjectFromMessage(ref OPLNetworkClipElement Source)
        {
            StackPanelClip.Children.Add(Source);
        }
    }
}
