using AAC20.Interfaces;
using System.Windows;

namespace AAC20.Classes
{
    public struct SettingsPanelActionFrameworkElement(FrameworkElement Element, IPageModuleButtonKeyAAC DefaultPage, Size size)
    {
        /// <summary>
        /// Элемент интерфейса в границах которого будет находится панель действий
        /// </summary>
        public FrameworkElement ElementInPanel { get; } = Element;

        /// <summary>
        /// Размер панели действий находясь в объекте
        /// </summary>
        public Size SizedPanel { get; set; } = size;

        /// <summary>
        /// Стартовая страница панели
        /// </summary>
        public IPageModuleButtonKeyAAC DefaultPageInPanel { get; set; } = DefaultPage;
    }
}
