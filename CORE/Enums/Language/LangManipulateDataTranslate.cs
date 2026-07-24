using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE.Enums.Language
{
    /// <summary>
    /// Перечисление ключей языковых переводов для интерфейса манимуляции с данными программы
    /// </summary>
    public enum LangManipulateDataTranslate : ulong
    {
        /// <summary>
        /// "Сохранение ваших самых важных данных"
        /// </summary>
        SaveImportantData = 0LU,

        /// <summary>
        /// "Чтение ваших самых важных данных"
        /// </summary>
        ReadImportantData = 1LU,
    }
}
