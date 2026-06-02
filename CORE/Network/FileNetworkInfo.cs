using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace OperPageLes.CORE.Network
{
    /// <summary>
    /// Класс информации об передоваемом объекте файла
    /// </summary>
    internal class FileNetworkInfo
    {
        /// <summary>
        /// Длинна расширения файла (в байтах)
        /// </summary>
        internal readonly byte LengthFileExtension;

        /// <summary>
        /// Длинна имени файла (в байтах)
        /// </summary>
        internal readonly ushort LengthFileName;

        /// <summary>
        /// Строка имени файла
        /// </summary>
        internal readonly string FileName;

        /// <summary>
        /// Строка разширения файла
        /// </summary>
        internal readonly string FileExtension;

        /// <summary>
        /// Длинна данных файла (в байтах)
        /// </summary>
        internal readonly uint LengthFileData;

        /// <summary>
        /// Количество байт которое содержится в объекте
        /// </summary>
        internal const byte LengthDataOneObject = 0x07;

        /// <summary>
        /// Байты отражающие объект передаваемых данных
        /// </summary>
        internal readonly ReadOnlyCollection<byte> SourceBytes;

        /// <summary>
        /// Создать информационный объект об передаваемых файлах
        /// </summary>
        /// <remarks>
        /// <b>[FF_FF {} - FF {} - FF_FF_FF_FF]</b><br/>
        /// <b>FF_FF</b> : <i>Длинна имени</i><br/>
        /// <b>FF</b> : <i>Длинна расширения</i><br/>
        /// <b>FF_FF_FF_FF</b> : <i>Длинна данных файла</i><br/>
        /// </remarks>
        internal FileNetworkInfo(ArraySegment<byte> Data)
        {
            byte[] BytesInfo = [.. Data];
            LengthFileName = BitConverter.ToUInt16(new ArraySegment<byte>(BytesInfo, 0, 2));
            FileName = Encoding.UTF8.GetString(BytesInfo[2..(LengthFileName + 2)]);

            LengthFileExtension = BytesInfo[LengthFileName + 2];
            FileExtension = Encoding.UTF8.GetString(BytesInfo[(LengthFileName + 3)..(LengthFileName + LengthFileExtension + 3)]);
            LengthFileData = BitConverter.ToUInt32(new ArraySegment<byte>(BytesInfo, LengthFileName + LengthFileExtension + 3, 4));

            SourceBytes = Data.AsReadOnly();
        }

        /// <summary>
        /// Создать информационный объект об передаваемом файле
        /// </summary>
        /// <param name="PathFile">Директория файла</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal FileNetworkInfo(string PathFile)
        {
            FileInfo info = new(PathFile);
            if (!info.Exists)
                throw new FileNotFoundException("Указанный файл не существует");
            else if (info.Length > uint.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(PathFile), "Файл слишком большой для передачи");
            else if (info.Name.Length > ushort.MaxValue / 2)
                throw new ArgumentOutOfRangeException(nameof(PathFile), "Имя файла слишком большое для передачи");
            List<byte> Data = [];

            FileName = info.Name[..^info.Extension.Length];
            byte[] BytesFileInfo = Encoding.UTF8.GetBytes(FileName);
            LengthFileName = (ushort)BytesFileInfo.Length;
            Data.AddRange(BitConverter.GetBytes(LengthFileName));
            Data.AddRange(BytesFileInfo);

            FileExtension = info.Extension[1..];
            BytesFileInfo = Encoding.UTF8.GetBytes(FileExtension);
            LengthFileExtension = (byte)BytesFileInfo.Length;
            Data.Add(LengthFileExtension);
            Data.AddRange(BytesFileInfo);

            LengthFileData = (uint)info.Length;
            Data.AddRange(BitConverter.GetBytes(LengthFileData));
            SourceBytes = Data.AsReadOnly();
        }

        //
        internal static FileNetworkInfo[] InicializeMainInfoFiles(byte[] SourceBytes)
        {
            List<FileNetworkInfo> FilesInfo = [];
            ushort LengthNameFile;
            byte LengthExtensionFile;
            while (SourceBytes.Length > 0)
            {
                LengthNameFile = BitConverter.ToUInt16(SourceBytes[..2], 0);
                LengthExtensionFile = SourceBytes[LengthNameFile + 2];
                FilesInfo.Add(new(SourceBytes[..(LengthNameFile + LengthExtensionFile + LengthDataOneObject)]));
                SourceBytes = SourceBytes[(LengthNameFile + LengthExtensionFile + LengthDataOneObject)..];
            }

            return [.. FilesInfo];
        }
    }
}
