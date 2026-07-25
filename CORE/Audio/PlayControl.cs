using IEL.UserElementsControl.Base;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.UserElementsControl.Default;
using OPLAPI.CORE.Animation;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Un4seen.Bass;

namespace OperPageLes.CORE.Audio
{
    internal class PlayControl
    {
        /// <summary>
        /// Директории всех аудио файлов
        /// </summary>
        private string[] AllDirectorySamples;

        /// <summary>
        /// Массив всех ресурсных аудио
        /// </summary>
        private readonly Dictionary<string, int> Samples = [];

        internal PlayControl()
        {
            if (Bass.BASS_Start())
                Bass.BASS_Free();
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero); // App.CurrentApp.SettingMainApplication.DeviceIndexActive

            string Prefics;
            List<string> AllDirectories = [];
            foreach (PropertyInfo prop in typeof(OperPageLes.Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (prop.PropertyType != typeof(byte[]) && !prop.Name.Contains("Audio")) continue;
                Prefics = StructDirectoryResources.DirectoryAudioApplication + $"{prop.Name}.mp3";
                if (!File.Exists(Prefics)) continue;
                else AllDirectories.Add(Prefics);
            }
            AllDirectorySamples = [.. AllDirectories];
            UpdateDataSamples();
        }

        /// <summary>
        /// Обновить данные с очисткой семплов на основе директорий аудио-файлов
        /// </summary>
        internal void UpdateDataSamples()
        {
            ClearSamples();
            AddDataSamples();
        }

        /// <summary>
        /// Очистить все семплы
        /// </summary>
        private void ClearSamples()
        {
            if (!Bass.BASS_Start())
                throw new Exception("BASS не инициализирован!");

            foreach (int SampleHandle in Samples.Values)
                Bass.BASS_SampleFree(SampleHandle);
            Samples.Clear();
        }

        /// <summary>
        /// Добавить данные семплов на основе директорий аудио-файлов
        /// </summary>
        private void AddDataSamples()
        {
            if (!Bass.BASS_Start())
                throw new Exception("BASS не инициализирован!");

            foreach (string Directory in AllDirectorySamples)
                Samples.Add(Path.GetFileNameWithoutExtension(Directory), Bass.BASS_SampleLoad(Directory, 0L, 0, 5, BASSFlag.BASS_DEFAULT));
        }

        /// <summary>
        /// Обновить устройство аудио вывода
        /// </summary>
        /// <param name="Handle">Идентификатор устройства</param>
        internal void ChangeDevice(int Handle)
        {
            ClearSamples();
            Bass.BASS_Free();
            Bass.BASS_Init(Handle, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            //App.CurrentApp.SettingMainApplication.DeviceIndexActive.Value = Handle;
            AddDataSamples();
        }

        /// <summary>
        /// Воспроизвести аудио-файл
        /// </summary>
        /// <param name="SourceWaveOut">Экземпляр воспроизведения аудио</param>
        /// <param name="NameResourceSound">Директория звукового файла</param>
        /// <exception cref="Exception">Исключение не инициализированного экземпляра ресурсов</exception>
        internal void Play(string NameResourceSound) // mp3
        {
            if (!Samples.TryGetValue(NameResourceSound, out int ActiveSample)) return;

            // Получить свободный канал из семпла
            int channel = Bass.BASS_SampleGetChannel(ActiveSample,
                BASSFlag.BASS_SAMCHAN_STREAM | BASSFlag.BASS_STREAM_AUTOFREE);

            // Установить громкость (опционально)
            //Bass.BASS_ChannelSetAttribute(channel, BASSAttribute.BASS_ATTRIB_VOL,
            //    (float)App.CurrentApp.SettingMainApplication.Volume / 100f);

            if (channel == 0)
            {
                BASSError error = Bass.BASS_ErrorGetCode();
                Console.WriteLine($"Ошибка: {error}");
            }

            // Воспроизвести
            Bass.BASS_ChannelPlay(channel, false);
            //int Channel = Bass.BASS_StreamCreateFile(BASSFiletype.BASS_FILE_HANDLE,
            //ResourcesAudio[NameResourceSound].SafeFileHandle.DangerousGetHandle(), 0L, 0L,
            //BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.);

            //ChannelsEndPlay.Add(NameResourceSound, Channel);
            //ActiveSample = Channel;
        }

        #region Visual Control
        /// <summary>
        /// Обновить отображение аудио устройств для контейнера
        /// </summary>
        /// <param name="ContainerVisualElements">Контейнер отображаемых элементов аудио устройств</param>
        /// <param name="Manager">Менеджер анимаций для элементов</param>
        /// <param name="ChangeDeviceHandler">Событие изменения аудио устройства</param>
        internal void UpdateVisualElementsFromStackPanel(StackPanel ContainerVisualElements, OPLAnimationManager? Manager,
            IELButtonBase.ActivateHandler ChangeDeviceHandler)
        {
            if (Manager != null)
            {
                DoubleAnimation animation = Manager.GetCloneAnimationElementFromType<DoubleAnimation>();
                animation.To = 0d;
                animation.Duration = TimeSpan.FromMilliseconds(600d);
                animation.FillBehavior = FillBehavior.Stop;
                animation.Completed += (sender, e) =>
                {
                    ContainerVisualElements.Opacity = 0d;
                    SetDataDevices(in ContainerVisualElements, in Manager, in ChangeDeviceHandler);
                    OPLAnimationManager.AnimateTakingZeroTo(Manager, ContainerVisualElements, StackPanel.OpacityProperty,
                        1d, TimeSpan.FromMilliseconds(600d));
                };
                ContainerVisualElements.BeginAnimation(StackPanel.OpacityProperty, animation);
            }
            else
            {
                ContainerVisualElements.Opacity = 0d;
                SetDataDevices(in ContainerVisualElements, in Manager, in ChangeDeviceHandler);
                ContainerVisualElements.Opacity = 1d;
            }
        }

        /// <summary>
        /// Установить данные в контейнер
        /// </summary>
        /// <param name="ContainerVisualElements">Контейнер всех элементов устройств</param>
        /// <param name="Manager">Менеджер анимаций для элементов</param>
        /// <param name="ChangeDeviceHandler">Событие левого клика по объекту</param>
        private static void SetDataDevices(in StackPanel ContainerVisualElements, in OPLAnimationManager? Manager,
            in IELButtonBase.ActivateHandler ChangeDeviceHandler)
        {
            BASS_DEVICEINFO info;
            for (int i = 1; i < Bass.BASS_GetDeviceCount(); i++)
            {
                info = Bass.BASS_GetDeviceInfo(i);
                OPLCheckAudioDevice VisualDevice = new()
                {
                    Margin = new(3d),
                    ManagerAnimation = Manager,
                    IsEnabled = info.flags.HasFlag(BASSDeviceInfo.BASS_DEVICE_ENABLED),
                    IndexCurrentDevice = i,
                    Text = info.name,
                    //Activate = i == App.CurrentApp.SettingMainApplication.DeviceIndexActive,
                };
                VisualDevice.OnActivateMouseLeft += ChangeDeviceHandler;
                ContainerVisualElements.Children.Add(VisualDevice);
            }
        }
        #endregion
    }
}
