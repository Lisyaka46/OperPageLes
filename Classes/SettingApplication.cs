using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace AAC20.Classes
{

    /// <summary>
    /// Перечисление именованных свойств настроек
    /// </summary>
    internal enum EnumNameParameter
    {
        PathMenuImage = 0,
    }

    internal partial class SettingApplication
    {
        private readonly string[] DataPropetry =
        [
            // PathMenuImage
            $"!"
        ];

        internal SettingApplication(string? PathSettingFile)
        {
            string PathFile = PathSettingFile ?? App.NameFileApplicationSetting + ".so";
            if (!File.Exists(PathFile)) throw new FileNotFoundException($"Файл настроек не найден {(PathSettingFile != null ? "C" : "D")}:\"{PathFile}\"");
            UpdateDataPropetrySettingFile(PathFile);
        }

        /// <summary>
        /// Обновить параметры настроек через файл настроек
        /// </summary>
        /// <param name="Path">Директория файла настроек .so</param>
        internal void UpdateDataPropetrySettingFile(string Path)
        {
            string[] LinesText = File.ReadAllLines(Path);
            lock (DataPropetry)
            {
                Regex
                    regexName = RegexNameSettingParameter(),
                    regexValue = RegexValueSettingParameter();
                string[] MassEnumNameParameter = Enum.GetNames(typeof(EnumNameParameter));
                int index;
                foreach (string Line in LinesText)
                {
                    index = Array.IndexOf(MassEnumNameParameter, regexName.Match(Line).Value[..^1]);
                    if (index == -1) continue;
                    // Text:Value
                    DataPropetry[index] = regexValue.Match(Line).Value[1..];
                }
            }
        }

        /// <summary>
        /// Узнать значение параметра по его имени
        /// </summary>
        /// <param name="Parameter">Индексированное имя параметра</param>
        /// <returns>Значение параметра настроек</returns>
        internal string GetSettingValue(EnumNameParameter Parameter) => DataPropetry[(int)Parameter];

        /// <summary>
        /// Регулярное выражение имени параметра
        /// </summary>
        [GeneratedRegex("\\b[^:]+:")]
        private static partial Regex RegexNameSettingParameter();

        /// <summary>
        /// Регулярное выражение значения параметра
        /// </summary>
        [GeneratedRegex(":[^\n]+")]
        private static partial Regex RegexValueSettingParameter();
    }
}
