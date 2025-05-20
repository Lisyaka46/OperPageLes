using CefSharp.DevTools.CSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperPage_les.CORE.Settings.Struct
{
    /// <summary>
    /// Настройки процесса приложения
    /// </summary>
    internal struct SettingProcess()
    {
        /// <summary>
        /// Директория к файлу настроек приложения
        /// </summary>
        public string PathFileApplicationSetting { get; set; } = string.Empty;
    }
}
