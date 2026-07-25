using OPLAPI.CORE.Settings.Base;
using OPLAPI.CORE.Settings.Parameters;
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
        public ParameterSetting<string> PathMenuImage { get; internal set; } = string.Empty;

        /// <summary>
        /// Размер буфера команд
        /// </summary>
        public ParameterSetting<int> BufferSize { get; internal set; } = 50;

        /// <summary>
        /// Отображение потраченного времени на ответ интернета
        /// </summary>
        public ParameterSetting<bool> MillisecondInternetConnection { get; internal set; } = false;

        /// <summary>
        /// Ссылка открытия браузера
        /// </summary>
        public ParameterSetting<string> DefaultOpenUrlWebView { get; internal set; } = string.Empty;

        /// <summary>
        /// Состояние использования подсказок к командам
        /// </summary>
        public ParameterSetting<bool> HitUse { get; internal set; } = true;

        /// <summary>
        /// Использование открытия ссылки в внутреннем браузере
        /// </summary>
        public ParameterSetting<bool> UseOpenLinkInPageBrowser { get; internal set; } = true;

        /// <summary>
        /// Использование открытия ссылки исключительно в новой станице браузера
        /// </summary>
        public ParameterSetting<bool> UseOnlyCreatePageWebBrowser { get; internal set; } = false;

        /// <summary>
        /// Использование отключения режима клавиатуры при закрытии панели дейтсвий
        /// </summary>
        public ParameterSetting<bool> ExitKeyboardModeInClosePanelAction { get; internal set; } = true;

        /// <summary>
        /// Клавиша управления режимом клавиатуры в панели дейтсвий
        /// </summary>
        public ParameterSetting<Key> KEY_KeyboardModePanelAction { get; internal set; } = Key.Z;

        /// <summary>
        /// Клавиша управления правым режимом нажатия на кнопку в режиме клавиатуры для панели дейтсвий
        /// </summary>
        public ParameterSetting<Key> KEY_PanelActionRightClick { get; internal set; } = Key.RightCtrl;

        /// <summary>
        /// Клавиша управления закрытием панели дейтсвий
        /// </summary>
        public ParameterSetting<Key> KEY_PanelActionClose { get; internal set; } = Key.Escape;

        /// <summary>
        /// Громкость звуков приложения
        /// </summary>
        public ParameterSetting<int> Volume { get; internal set; } = 50;

        /// <summary>
        /// Состояние использования границы окна для визуализации загрузки процесса
        /// </summary>
        public ParameterSetting<bool> LoadingBorderVisualizate { get; internal set; } = true;

        /// <summary>
        /// Наименование темы которая должна использоваться в программе
        /// </summary>
        public ParameterSetting<string> ThemeInstallName { get; internal set; } = string.Empty;

        /// <summary>
        /// Перемещаться на страницу в которой исполняется команда из буфера
        /// </summary>
        public ParameterSetting<bool> MovePageExecuteBufferCommand { get; internal set; } = true;

        /// <summary>
        /// Сила прокрутки визуализаторов консоли
        /// </summary>
        public ParameterSetting<int> ConsoleScrollForce { get; internal set; } = 4;

        /// <summary>
        /// Размер главного окна по ширине
        /// </summary>
        public ParameterSetting<double> MainWindowWidth { get; internal set; } = 800d;

        /// <summary>
        /// Размер главного окна по высоте
        /// </summary>
        public ParameterSetting<double> MainWindowHeight { get; internal set; } = 650d;

        /// <summary>
        /// Активный индекс устройства для воспроизведения взуков
        /// </summary>
        public ParameterSetting<int> DeviceIndexActive { get; internal set; } = 1;
    }
}
