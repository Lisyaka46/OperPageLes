using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE.Enums.Language
{
    /// <summary>
    /// Перечисление ключей языковых переводов для интерфейса общей категории настроек программы
    /// </summary>
    public enum LangSettingGeneralUITranslate : ulong
    {
        /// <summary>
        /// "Общие"
        /// </summary>
        GeneralTitle = 0LU,

        #region VisibleConnectInternetMillisecond
        /// <summary>
        /// "Отображать отклик интернета в миллисекундах"
        /// </summary>
        ParameterName_VisibleConnectInternetMillisecond = 1LU,

        /// <summary>
        /// "Если включён, тогда будет отображаться снизу количество потраченых миллицекунд на проверку подключения интернета"
        /// </summary>
        ParameterDescription_VisibleConnectInternetMillisecond = 2LU,
        #endregion
    }
}
