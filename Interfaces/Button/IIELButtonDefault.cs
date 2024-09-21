using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AAC20.Interfaces.Button
{
    public interface IIELButtonDefault : IIELButton
    {
        /// <summary>
        /// Объект события активации кнопки левым щелчком мыши
        /// </summary>
        public Activate? OnActivateMouseLeft { get; }

        /// <summary>
        /// Объект события активации кнопки правым щелчком мыши
        /// </summary>
        public Activate? OnActivateMouseRight { get; }

        /// <summary>
        /// Делегат события активации
        /// </summary>
        public delegate void Activate();
    }
}
