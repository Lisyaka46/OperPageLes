using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE.Enums.Language
{
    /// <summary>
    /// Перечисление ключей языковых переводов для интерфейса ярлыков программы
    /// </summary>
    public enum LangShortcutUITranslate : ulong
    {
        /// <summary>
        /// "Название ярлыка"
        /// </summary>
        ShortcutName = 0LU,

        /// <summary>
        /// "Команда ярлыка"
        /// </summary>
        ShortcutCommand = 1LU,

        /// <summary>
        /// "Описание ярлыка"
        /// </summary>
        ShortcutDescription = 2LU,

        /// <summary>
        /// "Создать ярлык"
        /// </summary>
        ShortcutCreate = 3LU,

        /// <summary>
        /// "Создание ярлыка"
        /// </summary>
        ShortcutCreateTitle = 4LU,

        /// <summary>
        /// "Добавить ярлык на главную страницу"
        /// </summary>
        ShortcutCreateDescription = 5LU,

        /// <summary>
        /// "Измнение ярлыка"
        /// </summary>
        ShortcutChangeTitle = 6LU,

        /// <summary>
        /// "Измненить ярлыка"
        /// </summary>
        ShortcutChange = 7LU,
    }
}
