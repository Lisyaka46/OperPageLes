using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace OperPage_les.CORE
{
    internal class AudioPlayerControl
    {
        /// <summary>
        /// Плеер для воспроизведения аудио
        /// </summary>
        private static readonly Audio Player = new();

        /// <summary>
        /// Перечисление звуковых файлов
        /// </summary>
        internal enum AudioFiles
        {
            B6 = 0,
            C7 = 1,
            D_6 = 2,
            B5 = 3,
        }

        /// <summary>
        /// Воспроизвести звуковой файл
        /// </summary>
        /// <param name="BytesResorce">Данные звукового файла</param>
        internal static void PlayMP3(AudioFiles Audio)
        {
            string NameAudioObject = Enum.GetName(typeof(AudioFiles), Audio) ?? string.Empty;
            if (NameAudioObject.Length == 0) throw new Exception("Данное перечисление не найдено!");
            string FileMP3 = $"{Path.GetTempPath()}OPLTEMP\\{NameAudioObject}.wav";
            if (!Path.Exists($"{Path.GetTempPath()}OPLTEMP")) Directory.CreateDirectory($"{Path.GetTempPath()}OPLTEMP");
            if (!File.Exists(FileMP3))
            {
                byte[] ByteAudio = GetDataAudioInNameResorce(Audio);
                File.WriteAllBytes(FileMP3, ByteAudio);
            }
            Player.Play(FileMP3, Microsoft.VisualBasic.AudioPlayMode.Background);
        }

        private static byte[] GetDataAudioInNameResorce(AudioFiles Audio)
        {
            return Audio switch
            {
                AudioFiles.B6 => Properties.Resources.B6,
                AudioFiles.C7 => Properties.Resources.C7,
                AudioFiles.D_6 => Properties.Resources.D_6,
                AudioFiles.B5 => Properties.Resources.B5,
                _ => throw new Exception("Данное имя не имеет ресурса")
            };
        }
    }
}
