using ApplicationOperPageLes.CORE.Enums;
using IEL.CORE.Classes;
using System.Collections.ObjectModel;
using System.Windows;

namespace ApplicationOperPageLes.CORE.Settings.PaletteElements
{
    internal class Palette
    {
        /// <summary>
        /// Объект словаря всех данных Q-логики
        /// </summary>
        private Dictionary<PaletteSpectrumEnum, PaletteSpectrum> _SourcePalette;

        /// <summary>
        /// Словарь доступный только для чтения
        /// </summary>
        public ReadOnlyDictionary<PaletteSpectrumEnum, PaletteSpectrum> SourcePalette => _SourcePalette.AsReadOnly();

        /// <summary>
        /// Инициализировать объект палитры по настройке ресурса
        /// </summary>
        /// <param name="PathFileXAML">Директория читаемого файла</param>
        /// <exception cref="Exception"></exception>
        internal Palette(string PathFileXAML)
        {
            ResourceDictionary Resource = new() { Source = new(PathFileXAML) };
            _SourcePalette = [];
            string[] Keys = [..Resource.Keys.Cast<string>()];
            for (int i = 0; i < Keys.Length; i++)
            {
                _SourcePalette.Add((PaletteSpectrumEnum)Enum.Parse(typeof(PaletteSpectrumEnum), Keys[i]), (PaletteSpectrum)Resource[Keys[i]]);
            }
        }

        /// <summary>
        /// Инициализировать объект палитры
        /// </summary>
        /// <param name="SourceData">Данные байтов цветов ARGB по всем значениям состояний палитры</param>
        internal Palette(byte[] SourceData)
        {
            PaletteSpectrumEnum[] t = Enum.GetValues<PaletteSpectrumEnum>();
            int CountBytesValuesInPalette =
                t.Length * PaletteSpectrum.CountQDataSpectrum * QData.CountSpectrumColor * QData.CountBytesFromColor;
            if (SourceData.Length != CountBytesValuesInPalette) // 16 значений DSUN -> 3 спектра BG BB FG
                throw new Exception($"Размер массива байтов не соответствует ожидаемому размеру {CountBytesValuesInPalette} байт");
            _SourcePalette = [];
            PaletteSpectrum spectrum;
            for (int IndexPaletteElement = 0; IndexPaletteElement < t.Length; IndexPaletteElement++)
            {
                spectrum = new();
                for (int IndexQDataSpectrum = 0; IndexQDataSpectrum < PaletteSpectrum.CountQDataSpectrum; IndexQDataSpectrum++)
                {
                    byte[][] BytesFromQdata = new byte[QData.CountSpectrumColor][];
                    for (int IndexSpectrumQData = 0; IndexSpectrumQData < QData.CountSpectrumColor; IndexSpectrumQData++)
                    {
                        BytesFromQdata[IndexSpectrumQData] = new byte[QData.CountBytesFromColor];
                        for (int IndexByteColor = 0; IndexByteColor < QData.CountBytesFromColor; IndexByteColor++)
                        {
                            BytesFromQdata[IndexSpectrumQData][IndexByteColor] = SourceData[
                                (IndexPaletteElement * (QData.CountSpectrumColor * QData.CountBytesFromColor * PaletteSpectrum.CountQDataSpectrum)) +
                                (IndexQDataSpectrum * (QData.CountSpectrumColor * QData.CountBytesFromColor)) +
                                IndexSpectrumQData * QData.CountSpectrumColor +
                                IndexByteColor];
                        }
                    }
                    QData SourceElement = new(BytesFromQdata);
                    switch (IndexQDataSpectrum)
                    {
                        case 0:
                            spectrum.BG = SourceElement;
                            break;
                        case 1:
                            spectrum.BB = SourceElement;
                            break;
                        case 2:
                            spectrum.FG = SourceElement;
                            break;
                        default: throw new Exception("Непредвиденное значение издекса спектра элемента палитры!");
                    }
                }
                _SourcePalette.Add(t[IndexPaletteElement], spectrum);
            }
        }

        /// <summary>
        /// Изменить данные палитры (Не затрагивает класс)
        /// </summary>
        /// <param name="Source">Объект палитры</param>
        /// <returns></returns>
        public async Task ChangeSourcePalette(Palette Source)
        {
            foreach (var key in _SourcePalette.Keys)
            {
                await Task.Run(() =>
                {
                    _SourcePalette[key].BG.ChangeSourceQData(Source._SourcePalette[key].BG);
                    _SourcePalette[key].BB.ChangeSourceQData(Source._SourcePalette[key].BB);
                    _SourcePalette[key].FG.ChangeSourceQData(Source._SourcePalette[key].FG);
                });
            }
        }
    }
}
