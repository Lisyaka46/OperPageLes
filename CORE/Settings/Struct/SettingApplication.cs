using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public ObjSetting<string> PathMenuImage { get; internal set; } = string.Empty;

        /// <summary>
        /// Размер буфера команд
        /// </summary>
        public ObjSetting<int> BufferSize { get; internal set; } = 50;

        /// <summary>
        /// Размытие изображения на панели даты и времени
        /// </summary>
        public ObjSetting<bool> BlurBackgroundDataTime { get; internal set; } = false;

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
        /// Сколлапсировать пустые значения настроек в значения по умолчанию
        /// </summary>
        /// <param name="setting">Изменяемая ссылка настроек</param>
        internal readonly SettingApplication CollapseNullSetting()
        {
            SettingApplication Def = new();
            PropertyInfo[] Info = typeof(SettingApplication).GetProperties();
            for (int i = 0; i < Info.Length; i++)
            {
                if (Info[i].GetValue(this) != null)
                {
                    object? Value = Info[i].GetValue(this);
                    //object? OriginValue = Info[i].GetValue(this);
                    Info[i].SetValue(Def, Value);
                }
            }
            return Def;
        }
    }
}
