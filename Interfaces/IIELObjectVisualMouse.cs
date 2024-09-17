using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AAC20.Interfaces
{
    public interface IIELObjectVisualMouse : IIELObject
    {
        /// <summary>
        /// Состояние активности отображения действий на кнопке
        /// </summary>
        public bool VisibleMouseImaging { get; set; }

        /// <summary>
        /// Узнать отображения действий над кнопкой
        /// </summary>
        /// <returns>Изображение мыши с действиями</returns>
        protected static sealed BitmapImage? ImageMouseButton(IIELObject ObjectElement)
        {
            if (ObjectElement.OnActivateMouseLeft != null)
            {
                if (ObjectElement.OnActivateMouseRight != null) return new(new Uri("/Windows/WindowsImages/DoubleMouseButton.png", UriKind.Relative));
                else return new(new Uri("/Windows/WindowsImages/LeftMouseButton.png", UriKind.Relative));
            }
            else if (ObjectElement.OnActivateMouseRight != null) return new(new Uri("/Windows/WindiwsImages/RightMouseButton.png", UriKind.Relative));
            else return null;
        }
    }
}
