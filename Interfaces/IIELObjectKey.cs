using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AAC20.Interfaces
{
    public interface IIELObjectKey : IIELObject
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
        /// Анимация мерцания
        /// </summary>
        [MTAThread()]
        public void BlinkAnimation();

        /// <summary>
        /// Узнать символ клавиши по коду клавиши
        /// </summary>
        /// <param name="key">Код клавиши</param>
        /// <returns>Символ клавиши</returns>
        protected static sealed char KeyName(Key? key)
        {
            return (key switch
            {
                Key.Oem3 => '~',
                Key.OemMinus => '-',
                Key.OemPlus => '+',
                Key.OemComma => '<',
                Key.OemPeriod => '>',
                Key.Oem2 => '/',
                Key.Oem4 => '[',
                Key.Oem6 => ']',
                Key.OemPipe => '\\',
                _ => key?.ToString()[^1]
            }) ?? '\0';
        }

        /// <summary>
        /// Найти кнопку типа "IELButtonText" в странице
        /// </summary>
        /// <param name="VisualObject">Ссылка на объект поиска</param>
        /// <param name="key">Ключ клавиши</param>
        protected static sealed IIELObjectKey? SearchButton(Visual VisualObject, Key key)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(VisualObject); i++)
            {
                Visual ChildVisualElement = (Visual)VisualTreeHelper.GetChild(VisualObject, i);
                try
                {
                    IIELObjectKey ObjectButton = (IIELObjectKey)ChildVisualElement;
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
        internal static sealed void ActivateButtonInKey(Visual VisualObject, Key key, IPageActionPanelAAC.OrientationActivate Orientation)
        {
            IIELObjectKey? Button = SearchButton(VisualObject, key);
            if (Button == null) return;
            else
            {
                if (Orientation == IPageActionPanelAAC.OrientationActivate.LeftButton) Button.OnActivateMouseLeft?.Invoke(true);
                else if (Orientation == IPageActionPanelAAC.OrientationActivate.RightButton) Button.OnActivateMouseRight?.Invoke(true);
            }
        }

        /// <summary>
        /// Активировать мерцание кнопки типа "IELButtonText" в странице
        /// </summary>
        /// <param name="VisualObject">Ссылка на объект поиска</param>
        /// <param name="key">Ключ клавиши</param>
        internal static sealed void BlinkActivateInKey(Visual VisualObject, Key key)
        {
            IIELObjectKey? Button = SearchButton(VisualObject, key);
            if (Button == null) return;
            else Button.BlinkAnimation();
        }
    }
}
