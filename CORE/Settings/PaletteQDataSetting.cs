using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Struct;
using IEL.CORE.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.CORE.Settings
{
    internal class PaletteQDataSetting
    {
        public PaletteQDataSetting()
        {
            Resource = new() { Source = new(StructDirectoryResources.GetResourcePath(nameof(OPRES.PaletteDictionary))) };
            DictionaryQData = Resource.Keys.Cast<string>().ToDictionary(i => i, i => (QData)Resource[i]);
        }

        /// <summary>
        /// Объект ресурса данных палитры
        /// </summary>
        private ResourceDictionary Resource;

        /// <summary>
        /// Объект словаря всех данных Q-логики
        /// </summary>
        private Dictionary<string, QData> DictionaryQData;

        /// <summary>
        /// Получить значение данных впектров цвета Q-логики
        /// </summary>
        /// <param name="PaletteValue">Значение пересичления представляющее название ресурса</param>
        /// <returns></returns>
        /// <exception cref="Exception">Исключение при отсутствующем ресурсе в словаре</exception>
        public QData GetQdataFromEnum(PaletteValuesEnum PaletteValue) => DictionaryQData[Enum.GetName(PaletteValue) ??
            throw new Exception("Ресурс под заданным именем не содержится в палитре Q-логики")];
    }
}
