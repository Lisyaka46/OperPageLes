using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAC20.CORE.Settings
{
    /// <summary>
    /// Перечисление именованных свойств настроек <b>приложения</b>
    /// </summary>
    internal enum EnumSettingApplication
    {
        /// <summary>
        /// Ссылка на директорию изображения главного меню
        /// </summary>
        PathMenuImage = 0,

        /// <summary>
        /// Размер буфера команд
        /// </summary>
        BufferSize = 1,

        /// <summary>
        /// Блюр фона в панели даты и времени
        /// </summary>
        BlurBackgroundDataTime = 2,
    }
}
