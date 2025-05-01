using Microsoft.VisualBasic.Devices;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace OperPage_les.CORE
{
    internal readonly struct AudioPlayerControl
    {
        /// <summary>
        /// Плеер для воспроизведения аудио
        /// </summary>
        private static readonly WindowsMediaPlayer Player = new();

        /// <summary>
        /// Воспроизвести звуковой файл
        /// </summary>
        /// <param name="AudioBytes">Данные звукового файла</param>
        internal static void PlayMP3(byte[] AudioBytes)
        {
            //if (!Player.IsLoadCompleted) Player.Stop();
            //Player.Stream = new MemoryStream(AudioBytes);
            //Player.LoadTimeout = 0;
            //Player.Play();
            //Player.currentMedia = Player.newMedia(AudioBytes);
        }
    }
}
