using AAC20.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AAC20.Interfaces
{
    public interface IPageActionPanelAAC
    {
        /// <summary>
        /// Перечисление вариаций активации кнопки по клавише
        /// </summary>
        public enum OrientationActivate
        {
            /// <summary>
            /// Левая активация кнопки
            /// </summary>
            LeftButton = 0,

            /// <summary>
            /// Правая активация кнопки
            /// </summary>
            RightButton = 1,
        }

        /// <summary>
        /// Узнать состояние Alt-режима
        /// </summary>
        /// <returns>Состояние</returns>
        bool GetAltMode();

        /// <summary>
        /// Изменить состояние Alt-режима
        /// </summary>
        /// <param name="value">Значение</param>
        void SetAltMode(bool value);

        /// <summary>
        /// Объект состояния Alt-режима
        /// </summary>
        internal sealed bool AltMode
        {
            get => GetAltMode();
            set
            {
                SetAltMode(value);
                AltModeChanged.Invoke(value);
            }
        }

        /// <summary>
        /// Делегат события изменения состояния Alt режима
        /// </summary>
        /// <param name="ModeChanged">Новое значение Alt режима</param>
        public delegate void Delegate_AltModeChanged(bool ModeChanged);

        /// <summary>
        /// Объект события Alt режима
        /// </summary>
        internal Delegate_AltModeChanged AltModeChanged { get; }

        /// <summary>
        /// Активировать кнопку по ключу
        /// </summary>
        /// <param name="PageIELButtons">Страница содержащая кнопки типа "T"</param>
        /// <param name="key">Ключ активации</param>
        /// <param name="Orienation">Ориентация активации кнопки</param>
        public sealed static void ActivateInKey<T>(Page PageIELButtons, System.Windows.Input.Key key, OrientationActivate Orienation) where T : IELButtonText
        {
            IELButtonText.ActivateButtonInKey<T>(PageIELButtons, key, Orienation);
        }

        /// <summary>
        /// Активировать мерцание кнопки по ключу
        /// </summary>
        /// <param name="PageIELButtons">Страница содержащая кнопки типа "T"</param>
        /// <param name="key">Ключ активации</param>
        public sealed static void BlinkActivateInKey<T>(Page PageIELButtons, System.Windows.Input.Key key) where T : IELButtonText
        {
            IELButtonText.BlinkActivateInKey<T>(PageIELButtons, key);
        }

        /// <summary>
        /// Активировать кнопку в данном элементе типа "IELButtonText" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        /// <param name="Orientation">Ориентация нажатия на кнопку</param>
        internal void ActivateIELButtonTextInKey(Key key, OrientationActivate Orientation);

        /// <summary>
        /// Активировать мерцание кнопки в данном элементе типа "IELButtonText" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        public void BlinkActivateIELButtonTextInKey(Key key);
    }
}
