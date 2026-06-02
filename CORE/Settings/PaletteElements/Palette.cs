using OperPageLes.CORE.Enums;
using IEL.CORE.Classes;
using System.Collections.ObjectModel;
using System.Windows;

namespace OperPageLes.CORE.Settings.PaletteElements
{
    internal class Palette
    {
        /// <summary>
        /// Объект словаря всех данных Q-логики
        /// </summary>
        private Dictionary<PaletteSpectrumEnum, PaletteSpectrum> _SourcePalette;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PaletteSpectrum this[PaletteSpectrumEnum index]
        {
            get => _SourcePalette[index];
        }

        /// <summary>
        /// Инициализировать объект палитры по настройке ресурса
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal Palette(ResourceDictionary Resource)
        {
            _SourcePalette = [];
            string[] Keys = [.. Resource.Keys.Cast<string>()];
            for (int i = 0; i < Keys.Length; i++)
            {
                _SourcePalette.Add((PaletteSpectrumEnum)Enum.Parse(typeof(PaletteSpectrumEnum), Keys[i]),
                    (PaletteSpectrum)((PaletteSpectrum)Resource[Keys[i]]).Clone());
            }
        }

        /// <summary>
        /// Инициализировать объект палитры
        /// </summary>
        /// <param name="Source">Палитра значений которые являются для создаваемой палитры опорными</param>
        /// <param name="SourceData">Данные байтов цветов ARGB по всем значениям состояний палитры</param>
        internal Palette(Palette Source, byte[] SourceData)
        {
            if (SourceData.Length % QData.CountSpectrumColor * QData.CountBytesFromColor != 0) // 16 значений DSUN -> 3 спектра BG BB FG
                throw new Exception("Размер массива байтов не эквивалентен ожидаемому размеру " +
                    $"{SourceData.Length}%{QData.CountSpectrumColor * QData.CountBytesFromColor} байт");

            _SourcePalette = Source._SourcePalette.ToDictionary(
                entry => entry.Key, entry => (PaletteSpectrum)entry.Value.Clone());
            ChangePaletteFromBytes(ref SourceData);
        }

        /// <summary>
        /// Изменить данные текущей палитры по массиву байтов из файла .qd
        /// </summary>
        /// <param name="SourceData">Массив байтов представляющий значения QData для всех спектров палитры</param>
        internal void ChangePaletteFromBytes(ref byte[] SourceData)
        {
            if (SourceData.Length % QData.CountSpectrumColor * QData.CountBytesFromColor != 0) // 16 значений DSUN -> 3 спектра BG BB FG
                throw new Exception("Размер массива байтов не эквивалентен ожидаемому размеру " +
                    $"{SourceData.Length}%{QData.CountSpectrumColor * QData.CountBytesFromColor} байт");

            PaletteSpectrum spectrum;
            int CountBytesFromOneSpectrum = QData.CountSpectrumColor * QData.CountBytesFromColor * PaletteSpectrum.CountQDataSpectrum;
            foreach (PaletteSpectrumEnum Element in Enum.GetValues<PaletteSpectrumEnum>())
            {
                spectrum = _SourcePalette[Element];
                SetDataFromBytesInSpectrum(ref spectrum,
                    [..SourceData.Skip(CountBytesFromOneSpectrum * (int)Element).Take(CountBytesFromOneSpectrum)]);
            }
        }

        /// <summary>
        /// Изменить данные текущей палитры по экземпляру палитры
        /// </summary>
        /// <param name="SourcePalette">Палитра темы откуда берутся значения</param>
        internal void ChangePaletteFromBytes(Palette SourcePalette)
        {
            foreach (PaletteSpectrumEnum Element in Enum.GetValues<PaletteSpectrumEnum>())
            {
                _SourcePalette[Element].BG.ChangeSourceQData(SourcePalette._SourcePalette[Element].BG);
                _SourcePalette[Element].BB.ChangeSourceQData(SourcePalette._SourcePalette[Element].BB);
                _SourcePalette[Element].FG.ChangeSourceQData(SourcePalette._SourcePalette[Element].FG);
            }
        }

        /// <summary>
        /// Установить значение спектру палитры по данным байтам данных
        /// </summary>
        /// <param name="Spectrum">Спектр которому задаётся новое значение</param>
        /// <param name="SourceData">Байты которые представляют собой данные для нового значения спектра палитры</param>
        /// <exception cref="Exception">Исключение несоответствующего количества байтов для установки значения спектра палитры</exception>
        private static void SetDataFromBytesInSpectrum(ref PaletteSpectrum Spectrum, byte[] SourceData)
        {
            if (SourceData.Length == 0) return;
            else if (SourceData.Length != QData.CountSpectrumColor * QData.CountBytesFromColor * PaletteSpectrum.CountQDataSpectrum)
                // 16 значений DSUN -> 3 спектра BG BB FG (ДЛЯ 1 ЭЛЕМЕНТА 4*4*3)
                throw new Exception("Размер массива байтов не эквивалентен ожидаемому размеру " +
                    $"{SourceData.Length}%{QData.CountSpectrumColor * QData.CountBytesFromColor} байт");
            for (int IndexQDataSpectrum = 0; IndexQDataSpectrum < PaletteSpectrum.CountQDataSpectrum;
                    //SourceData.Length > IndexPaletteElement * (QData.CountSpectrumColor * QData.CountBytesFromColor * PaletteSpectrum.CountQDataSpectrum);
                    IndexQDataSpectrum++)
            {
                byte[][] BytesFromQdata = new byte[QData.CountSpectrumColor][];
                for (int IndexSpectrumQData = 0; IndexSpectrumQData < QData.CountSpectrumColor; IndexSpectrumQData++)
                {
                    BytesFromQdata[IndexSpectrumQData] = new byte[QData.CountBytesFromColor];
                    for (int IndexByteColor = 0; IndexByteColor < QData.CountBytesFromColor; IndexByteColor++)
                    {
                        BytesFromQdata[IndexSpectrumQData][IndexByteColor] = SourceData[
                            (IndexQDataSpectrum * (QData.CountSpectrumColor * QData.CountBytesFromColor)) +
                            IndexSpectrumQData * QData.CountSpectrumColor +
                            IndexByteColor];
                    }
                }
                QData ManipulateQdata = IndexQDataSpectrum switch
                {
                    0 => Spectrum.BG,
                    1 => Spectrum.BB,
                    2 => Spectrum.FG,
                    _ => throw new Exception("Непредвиденное значение издекса спектра элемента палитры!")
                };
                ManipulateQdata.ChangeSourceQData(new(BytesFromQdata));
            }
        }
    }
}
