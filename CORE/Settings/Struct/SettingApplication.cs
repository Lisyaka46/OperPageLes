using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OperPage_les.CORE.Settings.ISetting;

namespace OperPage_les.CORE.Settings.Struct
{
    /// <summary>
    /// Класс настроек приложения
    /// </summary>
    internal struct SettingApplication()
    {
        /// <summary>
        /// Параметр настроек фоновой картинки
        /// </summary>
        public ObjSetting<string> PathMenuImage { get; set; } = string.Empty;

        /// <summary>
        /// Размер буфера команд
        /// </summary>
        public ObjSetting<int> BufferSize { get; set; } = 50;

        /// <summary>
        /// Размытие изображения на панели даты и времени
        /// </summary>
        public ObjSetting<bool> BlurBackgroundDataTime { get; set; } = false;

        /// <summary>
        /// Отображение потраченного времени на ответ интернета
        /// </summary>
        public ObjSetting<bool> MillisecondInternetConnection { get; set; } = false;

        /// <summary>
        /// Ссылка открытия браузера
        /// </summary>
        public ObjSetting<string> DefaultOpenUrlWebView { get; set; } = string.Empty;
    }
}
