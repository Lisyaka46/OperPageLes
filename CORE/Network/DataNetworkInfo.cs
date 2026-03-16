using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ApplicationOperPageLes.CORE.Network
{
    internal class DataNetworkInfo
    {
        /// <summary>
        /// Длинна сообщения (в байтах)
        /// </summary>
        internal readonly ushort LengthMessage;

        /// <summary>
        /// Количество передоваемых файлов
        /// </summary>
        internal readonly byte CountFilesData;

        /// <summary>
        /// Массив данных о передаваемых файлах
        /// </summary>
        internal ReadOnlyCollection<FileNetworkInfo>? FilesInfo => SourceFilesInfo?.AsReadOnly() ?? null;

        /// <summary>
        /// Данные о передаваемых файлах
        /// </summary>
        private FileNetworkInfo[]? SourceFilesInfo = null;

        /// <summary>
        /// Байты отражающие объект передаваемых данных
        /// </summary>
        internal readonly ReadOnlyCollection<byte> SourceBytes;

        /// <summary>
        /// Создать информационный объект об передаваемых данных
        /// </summary>
        /// <remarks>
        /// <b>[FF_FF - FF - {FF - FF_FF - FF_FF_FF_FF}]</b><br/>
        /// <b>FF_FF</b> : <i>Длинна сообщения</i><br/><br/>
        /// <b>FF{}</b> : <i>Количество файлов</i><br/>
        /// <b>FF</b> : <i>Длинна расширения</i><br/>
        /// <b>FF_FF</b> : <i>Длинна имени</i><br/>
        /// <b>FF_FF_FF_FF</b> : <i>Длинна данных файла</i><br/>
        /// </remarks>
        /// <param name="Data">Данные в байтах, которые содержат в себе первоначальную настройку</param>
        internal DataNetworkInfo(byte[] Data)
        {
            if (Data.Length < 3)
                throw new ArgumentException("Недостаточно данных для создания информации о передаваемых данных.");
            LengthMessage = BitConverter.ToUInt16(new ArraySegment<byte>(Data, 0, 2));
            CountFilesData = Data[2];
            if (CountFilesData > 0)
            {
                if (FileNetworkInfo.LengthDataOneObject * CountFilesData == Data.Length - 3)
                {
                    SourceFilesInfo = new FileNetworkInfo[CountFilesData];
                    for (int i = 0; i < CountFilesData; i++)
                        SourceFilesInfo[i] = new FileNetworkInfo(new ArraySegment<byte>(Data,
                        3 + i * FileNetworkInfo.LengthDataOneObject, FileNetworkInfo.LengthDataOneObject));
                }
                else throw new ArgumentException(
                    "Невозможно преобразовать байты в данные файлов, так как данных недостаточно для создания информации о файлах");
            }
            SourceBytes = Data.AsReadOnly();
        }

        /// <summary>
        /// Создать информационный объект об передаваемых данных
        /// </summary>
        /// <param name="Message">Передаваемое сообщение</param>
        /// <param name="PathFiles">Директории прикреплённых файлов</param>
        internal DataNetworkInfo(ref string Message, ref string[] PathFiles)
        {
            if (Message.Length > ushort.MaxValue / 2 || PathFiles.Length > byte.MaxValue)
                throw new ArgumentException("Длинна сообщения или количество файлов превышают лимит");
            List<byte> Data = [];
            LengthMessage = (ushort)Encoding.UTF8.GetBytes(Message).Length;
            Data.AddRange(BitConverter.GetBytes(LengthMessage));
            CountFilesData = (byte)PathFiles.Length;
            Data.Add(CountFilesData);
            if (CountFilesData > 0)
            {
                if (PathFiles.Length > 0)
                {
                    SourceFilesInfo = new FileNetworkInfo[CountFilesData];
                    for (int i = 0; i < CountFilesData; i++)
                    {
                        SourceFilesInfo[i] = new FileNetworkInfo(PathFiles[i]);
                        Data.Add(SourceFilesInfo[i].LengthFileExpansion);
                        Data.AddRange(BitConverter.GetBytes(SourceFilesInfo[i].LengthFileName));
                        Data.AddRange(BitConverter.GetBytes(SourceFilesInfo[i].LengthFileData));
                    }
                }
                else throw new Exception("Невозможно создать информацию о файлах не имея их директорию");
            }
            SourceBytes = Data.AsReadOnly();
        }
    }
}