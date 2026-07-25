using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE.Enums.Language
{
    /// <summary>
    /// Перечисления языкового перевода для создания перевода
    /// </summary>
    public enum LangGenDictionaryTranslate : ulong
    {
        /// <summary>
        /// "Опорный язык перевода"
        /// </summary>
        SourceLanguage = 0LU,

        /// <summary>
        /// "Ошибка установки директории сохранения"
        /// </summary>
        ErrorDirectoryPath = 1LU,
    }
}
