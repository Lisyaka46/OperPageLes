using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAC20.Interfaces
{
    public interface IPageActionPanelAAC
    {
        bool @AltMode { get; set; }

        /// <summary>
        /// Делегат события изменения состояния Alt режима
        /// </summary>
        /// <param name="ModeChanged">Новое значение Alt режима</param>
        public delegate void Delegate_AltModeChanged(bool ModeChanged);

        /// <summary>
        /// Объект события Alt режима
        /// </summary>
        Delegate_AltModeChanged AltModeChanged { get; }

        /// <summary>
        /// Активация кнопки по ключу
        /// </summary>
        /// <param name="key">Ключ активации</param>
        public void ActivateInKey(System.Windows.Input.Key key);
    }
}
