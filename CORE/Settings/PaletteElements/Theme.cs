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
        public Theme(string NameTheme, byte[] Source)
        {
            SourcePalette = new(Source);
            Name = NameTheme;
        }

        /// <summary>
        /// Инициализировать палитру по умолчанию
        /// </summary>
        internal Theme()
        {
            Name = "Default";
            SourcePalette = new(StructDirectoryResources.GetResourcePath(nameof(OPRES.PaletteDictionary)));
        }

        /// <summary>
        /// Изменить данные темы
        /// </summary>
        /// <param name="Source">Тема, данные которой отражаются</param>
        /// <returns></returns>
        public async Task ChangeSourceTheme(Palette Source) => await SourcePalette.ChangeSourcePalette(Source);
    }
}
