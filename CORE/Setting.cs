using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Runtime.CompilerServices;

namespace AAC20.CORE
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
        /// <exception cref="ArgumentException">Искючение при меньшем количестве значений нежели в перечислении параметров настроек</exception>
        private void CopyDefaultDataPropertyEnumerator(string[] Parameters)
        {
            if (LengthEnumerator > Parameters.Length)
                throw new ArgumentException("Размер значений по умолчанию ниже чем ожидалось в перечислении свойств настроек: " +
                    $"(ENUM:{LengthEnumerator} > DEF:{Parameters.Length})");
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
                    DataPropetry[index] = regexValue.Match(Line).Value[1..];
                }
            }
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
