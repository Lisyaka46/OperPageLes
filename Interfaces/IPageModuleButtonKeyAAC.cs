using System.Windows.Input;

namespace AAC20.Interfaces
{
    public interface IPageModuleButtonKeyAAC
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
        /// Имя страницы
        /// </summary>
        public string PageName { get; }

        /// <summary>
        /// Объект состояния режима клавиатуры <b>БЕЗ СОБЫТИЯ ИЗМЕНЕНИЯ</b>
        /// </summary>
        public bool KeyboardMode { get; set; }

        /// <summary>
        /// Делегат события изменения состояния режима клавиатуры
        /// </summary>
        /// <param name="ModeChanged">Новое значение Alt режима</param>
        public delegate void Delegate_KeyboardModeChanged(bool ModeChanged);

        /// <summary>
        /// Объект события режима клавиатуры
        /// </summary>
        internal Delegate_KeyboardModeChanged? KeyboardModeChanged { get; }

        /// <summary>
        /// Активировать кнопку в данном элементе типа "IIELButtonKey" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        /// <param name="Orientation">Ориентация нажатия на кнопку</param>
        internal void ActivateIELButtonTextInKey(Key key, OrientationActivate Orientation);

        /// <summary>
        /// Активировать мерцание кнопки в данном элементе типа "IIELButtonKey" с помощью клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        public void BlinkActivateIELButtonTextInKey(Key key, OrientationActivate Orientation);
    }
}
