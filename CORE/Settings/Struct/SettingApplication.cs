using System.Reflection;
using System.Windows.Input;

namespace OperPageLes.CORE.Settings.Struct
{
    /// <summary>
    /// Класс настроек приложения
    /// </summary>
    internal struct SettingApplication()
    {
        /// <summary>
        /// Параметр настроек фоновой картинки
        /// </summary>
        public ObjSetting<string> PathMenuImage { get; internal set; } = string.Empty;

        /// <summary>
        /// Размер буфера команд
        /// </summary>
        public ObjSetting<int> BufferSize { get; internal set; } = 50;

        /// <summary>
        /// Отображение потраченного времени на ответ интернета
        /// </summary>
        public ObjSetting<bool> MillisecondInternetConnection { get; internal set; } = false;

        /// <summary>
        /// Ссылка открытия браузера
        /// </summary>
        public ObjSetting<string> DefaultOpenUrlWebView { get; internal set; } = string.Empty;

        /// <summary>
        /// Состояние использования подсказок к командам
        /// </summary>
        public ObjSetting<bool> HitUse { get; internal set; } = true;

        /// <summary>
        /// Использование открытия ссылки в внутреннем браузере
        /// </summary>
        public ObjSetting<bool> UseOpenLinkInPageBrowser { get; internal set; } = true;

        /// <summary>
        /// Использование открытия ссылки исключительно в новой станице браузера
        /// </summary>
        public ObjSetting<bool> UseOnlyCreatePageWebBrowser { get; internal set; } = false;

        /// <summary>
        /// Использование отключения режима клавиатуры при закрытии панели дейтсвий
        /// </summary>
        public ObjSetting<bool> ExitKeyboardModeInClosePanelAction { get; internal set; } = true;

        /// <summary>
        /// Клавиша управления режимом клавиатуры в панели дейтсвий
        /// </summary>
        public ObjSetting<Key> KEY_KeyboardModePanelAction { get; internal set; } = Key.Z;

        /// <summary>
        /// Клавиша управления правым режимом нажатия на кнопку в режиме клавиатуры для панели дейтсвий
        /// </summary>
        public ObjSetting<Key> KEY_PanelActionRightClick { get; internal set; } = Key.RightCtrl;

        /// <summary>
        /// Клавиша управления закрытием панели дейтсвий
        /// </summary>
        public ObjSetting<Key> KEY_PanelActionClose { get; internal set; } = Key.Escape;

        /// <summary>
        /// Громкость звуков приложения
        /// </summary>
        public ObjSetting<float> Volume { get; internal set; } = 0.5f;

        /// <summary>
        /// Состояние использования границы окна для визуализации загрузки процесса
        /// </summary>
        public ObjSetting<bool> LoadingBorderVisualizate { get; internal set; } = true;

        /// <summary>
        /// Наименование темы которая должна использоваться в программе
        /// </summary>
        public ObjSetting<string> ThemeInstallName { get; internal set; } = string.Empty;

        /// <summary>
        /// Перемещаться на страницу в которой исполняется команда из буфера
        /// </summary>
        public ObjSetting<bool> MovePageExecuteBufferCommand { get; internal set; } = true;

        /// <summary>
        /// Сила прокрутки визуализаторов консоли
        /// </summary>
        public ObjSetting<int> ConsoleScrollForce { get; internal set; } = 30;
    }
}
