using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ApplicationOperPageLes.CORE.DEV
{
#if DEBUG
    internal class TextBlockInlay(string NameInlay)
    {
        /// <summary>
        /// 
        /// </summary>
        internal TextBlock Inlay { get; private set; } = new()
        {
            Text = NameInlay,
            Padding = new(10, 5, 10, 5),
            Background = new SolidColorBrush(Colors.Goldenrod),
            Foreground = new SolidColorBrush(Colors.Black),
            FontSize = 15d,
            IsHitTestVisible = true,
            Focusable = true,
            Cursor = System.Windows.Input.Cursors.Hand,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            VerticalAlignment = System.Windows.VerticalAlignment.Top,
        };

        /// <summary>
        /// 
        /// </summary>
        private StackPanel PanelInformation { get; set; } = new();

        /// <summary>
        /// Элемент панели информации
        /// </summary>
        internal UIElement StackPanelInformation => PanelInformation;

        /// <summary>
        /// Добавить новый элемент в панель
        /// </summary>
        /// <returns>Текстовый элемент интерфейса добавленный в панель</returns>
        internal TextBlock AddNewTextElement()
        {
            TextBlock textBlock = new()
            {
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                FontSize = 10d,
            };
            PanelInformation.Children.Add(textBlock);
            return textBlock;
        }
    }
#endif
}
