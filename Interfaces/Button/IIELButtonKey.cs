using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AAC20.Interfaces;

namespace AAC20.Interfaces.Button
{
    public interface IIELButtonKey : IIELButton
    {
        /// <summary>
        /// Активность видимости символа действия активации кнопки
        /// </summary>
        public bool CharKeyKeyboardActivate { get; set; }

        /// <summary>
        /// Клавиша отвечающая за активацию кнопки
        /// </summary>
        public Key? CharKeyKeyboard { get; set; }

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
        /// <param name="KeyboardActivate">Активировался ли объект с помощью клавиатуры</param>
        public delegate void Activate(bool KeyboardActivate);

        /// <summary>
        /// Анимация мерцания
        /// </summary>
        [MTAThread()]
        public void BlinkAnimation();

        /// <summary>
        /// Найти кнопку типа "IELButtonText" в странице
        /// </summary>
        /// <param name="VisualObject">Ссылка на объект поиска</param>
        /// <param name="key">Ключ клавиши</param>
        protected static sealed IIELButtonKey? SearchButton(Visual VisualObject, Key key)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(VisualObject); i++)
            {
                Visual ChildVisualElement = (Visual)VisualTreeHelper.GetChild(VisualObject, i);
                try
                {
                    IIELButtonKey ObjectButton = (IIELButtonKey)ChildVisualElement;
                    if (ObjectButton.CharKeyKeyboard == key && ObjectButton.IsEnabled) return ObjectButton;
                }
                catch
                {
                    if (ChildVisualElement.GetType() == typeof(IAddChild)) return SearchButton(ChildVisualElement, key);
                }
            }
            return null;
        }

        /// <summary>
        /// Активировать кнопку типа "IELButtonText" в странице
        /// </summary>
        /// <param name="VisualObject">Ссылка на объект поиска</param>
        /// <param name="key">Ключ клавиши</param>
        /// <param name="Orientation">Ориентация нажатия</param>
        internal static sealed void ActivateButtonInKey(Visual VisualObject, Key key, IPageModuleButtonKeyAAC.OrientationActivate Orientation)
        {
            IIELButtonKey? Button = SearchButton(VisualObject, key);
            if (Button == null) return;
            else
            {
                if (Orientation == IPageModuleButtonKeyAAC.OrientationActivate.LeftButton) Button.OnActivateMouseLeft?.Invoke(true);
                else if (Orientation == IPageModuleButtonKeyAAC.OrientationActivate.RightButton) Button.OnActivateMouseRight?.Invoke(true);
            }
        }

        /// <summary>
        /// Активировать мерцание кнопки типа "IELButtonText" в странице
        /// </summary>
        /// <param name="VisualObject">Ссылка на объект поиска</param>
        /// <param name="key">Ключ клавиши</param>
        internal static sealed void BlinkActivateInKey(Visual VisualObject, Key key, IPageModuleButtonKeyAAC.OrientationActivate Orientation)
        {
            IIELButtonKey? Button = SearchButton(VisualObject, key);
            if (Button == null) return;
            else
            {
                if ((Orientation == IPageModuleButtonKeyAAC.OrientationActivate.LeftButton && Button.OnActivateMouseLeft != null) ||
                    (Orientation == IPageModuleButtonKeyAAC.OrientationActivate.RightButton && Button.OnActivateMouseRight != null)) Button.BlinkAnimation();
            }
        }
    }
}
