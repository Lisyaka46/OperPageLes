#define if
using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Struct;
using IEL.CORE.BaseUserControls;
using IEL.CORE.Classes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Windows.ApplicationModel.Background;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.CORE.Settings
{
    internal class PaletteQDataSetting
    {
        public PaletteQDataSetting()
        {
            Resource = new() { Source = new(StructDirectoryResources.GetResourcePath(nameof(OPRES.PaletteDictionary))) };
            DictionaryQData = Resource.Keys.Cast<string>().ToDictionary(i => i, i => ((BrushSettingQ)Resource[i]).Clone());
#if DEBUG
            if (Enum.GetValues<PaletteSpectrumEnum>().Length * 3u != Enum.GetValues<PaletteValuesEnum>().Length)
                throw new Exception("Типы не являются индентичными в палитре");
#endif
             
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


        /// <summary>
        /// Соеденить объект IEL с спектром палитры
        /// </summary>
        /// <param name="IelObj">Объект который присоеденяется к палитре</param>
        /// <param name="PaletteSpectrum">Спектр палитры</param>
        [EnumDataType(typeof(PaletteSpectrumEnum))]
        public void ConnectPalleteFromIELElement([DisallowNull] IELObject IelObj, PaletteSpectrumEnum PaletteSpectrum)
        {
            uint ValueSpectrum = ((uint)PaletteSpectrum) * 3u;
            IelObj.QBackground.SetQData(GetQdataFromEnum((PaletteValuesEnum)ValueSpectrum));
            IelObj.QBorderBrush.SetQData(GetQdataFromEnum((PaletteValuesEnum)(ValueSpectrum + 1u)));
            IelObj.QForeground.SetQData(GetQdataFromEnum((PaletteValuesEnum)(ValueSpectrum + 2u)));
        }
    }
}
