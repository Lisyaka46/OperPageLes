using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ApplicationOperPageLes.CORE.Network
{
    /// <summary>
    /// Класс информации об передоваемом объекте файла
    /// </summary>
    internal class FileNetworkInfo
    {
        /// <summary>
        /// Длинна расширения файла (в байтах)
        /// </summary>
        internal readonly byte LengthFileExpansion;

        /// <summary>
        /// Длинна имени файла (в байтах)
        /// </summary>
        internal readonly ushort LengthFileName;

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
        /// <b>[FF - FF_FF - FF_FF_FF_FF]</b><br/>
        /// <b>FF</b> : <i>Длинна расширения</i><br/>
        /// <b>FF_FF</b> : <i>Длинна имени</i><br/>
        /// <b>FF_FF_FF_FF</b> : <i>Длинна данных файла</i><br/>
        /// </remarks>
        internal FileNetworkInfo(ArraySegment<byte> Data)
        {
            byte[] BytesInfo = [.. Data];
            LengthFileExpansion = BytesInfo[0];
            LengthFileName = BitConverter.ToUInt16(new ArraySegment<byte>(BytesInfo, 1, 2));
            LengthFileData = BitConverter.ToUInt32(new ArraySegment<byte>(BytesInfo, 3, 4));
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
            byte[] BytesFileInfo = Encoding.UTF8.GetBytes(info.Extension[1..]);
            Data.AddRange(BytesFileInfo);
            LengthFileExpansion = (byte)BytesFileInfo.Length;
            BytesFileInfo = Encoding.UTF8.GetBytes(info.Name[..^info.Extension.Length]);
            Data.AddRange(BytesFileInfo);
            LengthFileName = (ushort)BytesFileInfo.Length;
            LengthFileData = (uint)info.Length;
            SourceBytes = Data.AsReadOnly();
        }
    }
}
