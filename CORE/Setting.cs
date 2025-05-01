using System.IO;
using System.Text.RegularExpressions;

namespace OperPage_les.CORE
{
    internal partial class Setting<T> where T : Enum
    {
        /// <summary>
        /// Данные параметров настроек
        /// </summary>
        private readonly string[] DataPropetry;

        /// <summary>
        /// Объект перечисления имён параметров
        /// </summary>
        private readonly Type TypeEnumerator = typeof(T);

        /// <summary>
        /// Количество перечисляемых свойств настроек
        /// </summary>
        private int LengthEnumerator => Enum.GetValues(TypeEnumerator).Length;

        internal Setting(string PathSettingFile, string[]? DefaultParameters)
        {
            DataPropetry = new string[LengthEnumerator];

            if (DefaultParameters != null)
                CopyDefaultDataPropertyEnumerator(DefaultParameters);

            string PathFile = PathSettingFile;
            if (!File.Exists(PathFile))
            {
                File.WriteAllText(PathFile, null);
                return;
            }
            UpdateDataPropetrySettingFile(PathFile);
        }

        internal Setting(string[] DefaultParameters)
        {
            DataPropetry = new string[LengthEnumerator];

            CopyDefaultDataPropertyEnumerator(DefaultParameters);
        }

        /// <summary>
        /// Скопировать данные в значения по умолчанию учитывая тип перечисления
        /// </summary>
        /// <param name="Parameters">Массив значений по умолчанию</param>
        private void CopyDefaultDataPropertyEnumerator(string[] Parameters)
        {
            if (LengthEnumerator > Parameters.Length)
            {
                List<string> ParametersList = [.. Parameters];
                for (int i = ParametersList.Count; i < LengthEnumerator; i++) ParametersList.Add(string.Empty);
                Parameters = [.. ParametersList];
            }
            Array.Copy(Parameters, 0, DataPropetry, 0, LengthEnumerator);
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
                string[] MassEnumNameParameter = Enum.GetNames(TypeEnumerator);
                int index;
                foreach (string Line in LinesText)
                {
                    index = Array.IndexOf(MassEnumNameParameter, regexName.Match(Line).Value[..^1]);
                    if (index == -1) continue;
                    // Text:Value
                    try
                    {
                        DataPropetry[index] = regexValue.Match(Line).Value[1..];
                    }
                    catch
                    {
                        DataPropetry[index] = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Обновить файл настроек текущими значениями
        /// </summary>
        /// <param name="PathFileSetting">Директория файла настроек</param>
        internal void UpdateFileSetting(string PathFileSetting)
        {
            string[] ArrayNameValueSetting = Enum.GetNames(TypeEnumerator);
            List<string> NewLines = [];
            for (int i = 0; i < ArrayNameValueSetting.Length; i++)
            {
                NewLines.Add($"{ArrayNameValueSetting[i]}:{DataPropetry[i]}");
            }
            File.WriteAllLines(PathFileSetting, NewLines);
        }

        /// <summary>
        /// Узнать значение параметра по его имени
        /// </summary>
        /// <param name="Parameter">Индексированное имя параметра</param>
        /// <returns>Значение параметра настроек</returns>
        internal string GetSettingValue(T Parameter)
        {
            int index = Array.IndexOf(Enum.GetValues(TypeEnumerator), Parameter);
            if (index > -1)
            {
                string Result = DataPropetry[(int)Convert.ChangeType(Parameter, TypeEnumerator)];
                if (Result.Equals("!")) return string.Empty;
                return Result;
            }
            throw new NotImplementedException($"Значение не соответствует предполагаемому типу \"{TypeEnumerator.Name}\": \"{Parameter.GetType().Name}\"");
        }

        /// <summary>
        /// Узнать значение параметра по его имени
        /// </summary>
        /// <param name="Parameter">Индексированное имя параметра</param>
        /// <param name="Value">Новое значение параметра</param>
        internal void SetSettingValue(T Parameter, string Value)
        {
            int index = Array.IndexOf(Enum.GetValues(TypeEnumerator), Parameter);
            if (index > -1)
            {
                DataPropetry[(int)Convert.ChangeType(Parameter, TypeEnumerator)] = Value;
                return;
            }
            throw new NotImplementedException($"Значение не соответствует предполагаемому типу \"{TypeEnumerator.Name}\": \"{Parameter.GetType().Name}\"");
        }

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
