using ApplicationOperPageLes.UI.Windows.Base;
using OIEL.UserElementsControl.Base;
using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ApplicationOperPageLes.UI.UserElementsControl.Network
{
    /// <summary>
    /// Логика взаимодействия для OPLNetworkChat.xaml
    /// </summary>
    public partial class OPLNetworkChat : OPLNetworkElementViewerBase, IOPLAnimate
    {

        #region Properties

        #region EndMessage
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty EndMessageProperty =
            DependencyProperty.Register("EndMessage", typeof(string), typeof(OPLNetworkChat),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLNetworkChat)sender).TextBlockEndMessage.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Отображаемое последнее сообщение
        /// </summary>
        public string EndMessage
        {
            get => (string)GetValue(EndMessageProperty);
            set => SetValue(EndMessageProperty, value);
        }
        #endregion

        #region Icon
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(OPLNetworkChat),
                new(null,
                    (sender, e) =>
                    {
                        
                        ((OPLNetworkChat)sender).ImageIconMessage.Source = (ImageSource)e.NewValue;
                    }));

        /// <summary>
        /// Отображаемое иконка сообщения
        /// </summary>
        public ImageSource? Icon
        {
            get => (ImageSource?)GetValue(IconProperty);
            set
            {
                if (ManagerAnimation == null)
                {
                    ImageIconMessage.Opacity = value == null ? 0d : 1d;
                    ImageIconMessage.Width = value == null ? 0d : TextBlockEndMessage.ActualHeight;
                    ImageIconMessage.Margin = value == null ? new(0) : new(1, 0, 4, 0);
                }
                else
                {
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageIconMessage, OpacityProperty,
                        value == null ? 0d : 1d, TimeSpan.FromMilliseconds(400d));
                    ManagerAnimation.ThicknessAnimationType.AnimateEffect(ImageIconMessage, MarginProperty,
                        value == null ? new(0) : new(1, 0, 4, 0), TimeSpan.FromMilliseconds(400d));

                    ManagerAnimation.DoubleAnimationType.AnimateEffect(ImageIconMessage, OpacityProperty,
                        value == null ? 0d : TextBlockEndMessage.ActualHeight, TimeSpan.FromMilliseconds(400d));
                }
                SetValue(IconProperty, value);
            }
        }
        #endregion

        #region TextCount
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextCountProperty =
            DependencyProperty.Register("TextCount", typeof(int), typeof(OPLNetworkChat),
                new(0,
                    (sender, e) =>
                    {
                    }));

        /// <summary>
        /// Отображаемое иконка сообщения
        /// </summary>
        public int TextCount
        {
            get => (int)GetValue(TextCountProperty);
            set
            {
                TextBlockCountFiles.Text = value > 0 ? value.ToString() : string.Empty;
                SetValue(TextCountProperty, value);
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        public OPLNetworkChat()
        {
            InitializeComponent();
            TextBlockHead.Text = string.Empty;
            TextBlockEndMessage.Text = string.Empty;
            ImageIconMessage.Width = 0d;
            ImageIconMessage.Opacity = 0d;
            TextBlockCountFiles.Text = string.Empty;

            TextBlockHead.Foreground = SourceForeground.SourceBrush;
            TextBlockEndMessage.Foreground = SourceForeground.SourceBrush;
            TextBlockCountFiles.Foreground = SourceForeground.SourceBrush;
        }
    }
}
