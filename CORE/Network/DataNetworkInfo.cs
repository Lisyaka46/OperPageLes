using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OperPageLes.CORE.Network
{
    internal class DataNetworkInfo
    {
        /// <summary>
        /// Длинна сообщения (в байтах)
        /// </summary>
        internal readonly ushort LengthMessage;

        /// <summary>
        /// Массив данных о передаваемых файлах
        /// </summary>
        internal ReadOnlyCollection<FileNetworkInfo> FilesInfo => SourceFilesInfo.AsReadOnly();

        /// <summary>
        /// Данные о передаваемых файлах
        /// </summary>
        private List<FileNetworkInfo> SourceFilesInfo = [];

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
            if (Data[2] > 0)
                SourceFilesInfo.AddRange(FileNetworkInfo.InicializeMainInfoFiles(Data[3..]));
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
            Data.Add((byte)PathFiles.Length);
            if (PathFiles.Length > 0)
            {
                for (int i = 0; i < PathFiles.Length; i++)
                {
                    SourceFilesInfo.Add(new(PathFiles[i]));
                    Data.AddRange(SourceFilesInfo[^1].SourceBytes);
                }
            }
            SourceBytes = Data.AsReadOnly();
        }
    }
}