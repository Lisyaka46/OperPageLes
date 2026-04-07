using OperPageLes.CORE.DEV;
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

namespace OperPageLes.UI.Windows.DEV
{
#if DEBUG
    /// <summary>
    /// Логика взаимодействия для WindowDeveloper.xaml
    /// </summary>
    public partial class WindowDeveloper : Window
    {
        private TextBlockInlay? FocusInlay = null;
        private bool KeyDownIniting = false;
        private bool CTRL_Mode = false;

        internal TextBlockInlay[] BlockInlays;

        public WindowDeveloper()
        {
            InitializeComponent();
            BlockInlays = [
            CreateTextInlayDev("Главное окно"),
            CreateTextInlayDev("Фоновое обновление"),
            CreateTextInlayDev("Хранимые данные")
            ];
            Activated += (sender, e) =>
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(this, OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
            };
            Deactivated += (sender, e) =>
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(this, OpacityProperty, 0.7d, TimeSpan.FromMilliseconds(500d));
            };
            BorderMain.MouseLeftButtonDown += (sender, e) =>
            {
                if (CTRL_Mode) DragMove();
            };
            KeyDown += (sender, e) =>
            {
                if (KeyDownIniting) return;
                KeyDownIniting = true;
                switch (e.Key)
                {
                    case Key.LeftCtrl:
                        CTRL_Mode = true;
                        break;
                };
            };
            KeyUp += (sender, e) =>
            {
                KeyDownIniting = false;
                switch (e.Key)
                {
                    case Key.LeftCtrl:
                        CTRL_Mode = false;
                        break;
                };
            };
            foreach (TextBlockInlay item in BlockInlays) StackPanelInlays.Children.Add(item.Inlay);
        }

        //
        private TextBlockInlay CreateTextInlayDev(string NameInlay)
        {
            TextBlockInlay BlockInlay = new(NameInlay);
            BlockInlay.Inlay.MouseUp += (sender, e) =>
            {
                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BlockInlay.Inlay, FontSizeProperty, 20d, TimeSpan.FromMilliseconds(500d));
                if (FocusInlay != null)
                {
                    App.ManagerAnimation.DoubleAnimationType.AnimateEffect(FocusInlay.Inlay, FontSizeProperty, 15d, TimeSpan.FromMilliseconds(500d));
                    GridVisual.Children.Remove(FocusInlay.StackPanelInformation);
                }
                if (BlockInlay.Inlay.Equals(FocusInlay))
                {
                    FocusInlay = null;
                    return;
                }
                FocusInlay = BlockInlay;
                GridVisual.Children.Add(FocusInlay.StackPanelInformation);
                Grid.SetColumn(FocusInlay.StackPanelInformation, 1);
            };
            return BlockInlay;
        }
    }
#endif
}
