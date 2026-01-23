using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Struct;
using IEL.CORE.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
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
        internal string DirectoryFile { get; set; }

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
        /// Инициализировать объект темы по экземпляру файла
        /// </summary>
        /// <param name="NameTheme">Имя темы</param>
        /// <param name="Source">Данные палитры создающие по экземпляру</param>
        public Theme(string DirectoryFileQData)
        {
            if (!File.Exists(DirectoryFileQData)) throw new Exception("Файл не существует!");
            if (!Path.GetExtension(DirectoryFileQData).Equals(".qd")) throw new Exception("Файл не соответствует формату!");
            DirectoryFile = DirectoryFileQData;
            SourcePalette = new Palette(App.CurrentApp.ActiveThemeApplication, File.ReadAllBytes(DirectoryFileQData));
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

        public static implicit operator Palette(Theme obj) => obj.SourcePalette;

        /// <summary>
        /// Создать новый файл байтов темы по заданной директории<br/>
        /// <b>Не подходит для сохранения!</b>
        /// </summary>
        public async Task GenerateNewFileSource()
        {
            int i;
            QData SourceBytes;
            FileStream Stream = File.OpenWrite(DirectoryFile);
            foreach (PaletteSpectrumEnum Element in Enum.GetValues(typeof(PaletteSpectrumEnum)))
            {
                for (i = 0; i < 3; i++)
                {
                    SourceBytes = i switch
                    {
                        0 => SourcePalette[Element].BG,
                        1 => SourcePalette[Element].BB,
                        2 => SourcePalette[Element].FG,
                        _ => throw new Exception("Непредвиденное значение индекса!")
                    };
                    await Stream.WriteAsync(SourceBytes.GetSourceBytes());
                }
            }
            Stream.Close();
        }
    }
}
