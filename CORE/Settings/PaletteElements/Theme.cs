using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Struct;
using IEL.CORE.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.CORE.Settings.PaletteElements
{
    internal class Theme
    {
        /// <summary>
        /// Имя объекта темы
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Директория файла прочитанного для создания экземпляра темы
        /// </summary>
        internal string DirectoryFile { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PaletteSpectrum this[PaletteSpectrumEnum index]
        {
            get => SourcePalette[index];
        }

        /// <summary>
        /// Палитра соответствующая теме
        /// </summary>
        private Palette SourcePalette { get; set; }

        /// <summary>
        /// Инициализировать объект темы
        /// </summary>
        /// <param name="NameTheme">Имя темы</param>
        /// <param name="Source">Данные палитры создающие по экземпляру</param>
        public Theme(string DirectoryFileQData)
        {
            if (!File.Exists(DirectoryFileQData)) throw new Exception("Файл не существует!");
            if (!Path.GetExtension(DirectoryFileQData).Equals(".qd")) throw new Exception("Файл не соответствует формату!");
            DirectoryFile = DirectoryFileQData;
            SourcePalette = new Palette(File.ReadAllBytes(DirectoryFileQData));
            Name = Path.GetFileNameWithoutExtension(DirectoryFileQData);
        }

        /// <summary>
        /// Инициализировать палитру по умолчанию
        /// </summary>
        internal Theme()
        {
            Name = "Default";
            DirectoryFile = String.Empty;
            SourcePalette = new Palette(StructDirectoryResources.GetResourcePath(nameof(OPRES.PaletteDictionary)));
        }

        /// <summary>
        /// Изменить данные текущей темы
        /// </summary>
        /// <param name="Source">Тема из которой берутся данные</param>
        public void ChangeSourceData(ref Theme Source)
        {
            Name = Source.Name;
            DirectoryFile = Source.DirectoryFile;
            foreach (PaletteSpectrumEnum Element in Enum.GetValues<PaletteSpectrumEnum>())
            {
                SourcePalette[Element].BG.ChangeSourceQData(Source[Element].BG);
                SourcePalette[Element].BB.ChangeSourceQData(Source[Element].BB);
                SourcePalette[Element].FG.ChangeSourceQData(Source[Element].FG);
            }
        }

        public static implicit operator Palette(Theme obj) => obj.SourcePalette;
    }
}
