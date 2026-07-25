using OPLAPI.CORE.Language;
using System.IO;
using System.Net.Http.Json;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace OperPageLes.CORE.Struct
{
    internal readonly struct StructDirectoryResources
    {
        /// <summary>
        /// Массив всех директорий ресурсов по их именам
        /// </summary>
        private static readonly Dictionary<string, string> PathesFromNameResource = [];

        /// <summary>
        /// Массив всех директорий медиа ресурсов по их именам
        /// </summary>
        private static readonly Dictionary<string, Uri> ResourcesMedia = [];

        /// <summary>
        /// Массив всех ресурсных картинок по их именам
        /// </summary>
        private static readonly Dictionary<string, BitmapImage> ResourcesImages = [];

        #region MainDirectoryApplication
        /// <summary>
        /// Главная директория ресурсов OperPageLes
        /// </summary>
        internal static readonly string MainDirectoryApplication = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/OperPageLes/";
        
        #region DirectoryResourcesApplication
        /// <summary>
        /// Главная директория ресурсов пользователя
        /// </summary>
        internal static readonly string DirectoryResourcesApplication = MainDirectoryApplication + @"/Resources/";

        /// <summary>
        /// Главная директория ресурсов словарь
        /// </summary>
        internal static readonly string DirectoryDictionaryApplication = DirectoryResourcesApplication + @"/Dictionary/";

        /// <summary>
        /// Главная директория ресурсов тем приложения
        /// </summary>
        internal static readonly string DirectoryThemeApplication = MainDirectoryApplication + @"/Theme/";

        /// <summary>
        /// Директория ресурса ярлыков
        /// </summary>
        internal static readonly string DirectoryDataLabels = DirectoryResourcesApplication + "Labels.json";

        /// <summary>
        /// Файл настроек <b>процесса</b>
        /// </summary>
        internal static readonly string PathSettingProcess = StructDirectoryResources.MainDirectoryApplication + "/CurrentSettings.json";

        /// <summary>
        /// Имя файла настроек <b>приложения</b>
        /// </summary>
        internal static readonly string PathSettingApplication = StructDirectoryResources.MainDirectoryApplication + "/ApplicationSettings.json";
        #endregion

        /// <summary>
        /// Директория файла валидного ключа
        /// </summary>
        internal static readonly string DirectoryKeyValidFile = MainDirectoryApplication + "Key";

        /// <summary>
        /// Главная директория файлов изображений
        /// </summary>
        internal static readonly string DirectoryImagesApplication = MainDirectoryApplication + @"Images/";

        /// <summary>
        /// Главная директория файлов видео
        /// </summary>
        internal static readonly string DirectoryMediaApplication = DirectoryImagesApplication + @"Media/";

        /// <summary>
        /// Главная директория файлов звука
        /// </summary>
        internal static readonly string DirectoryAudioApplication = MainDirectoryApplication + @"Audio/";

        /// <summary>
        /// Главная директория скачаных файлов
        /// </summary>
        internal static readonly string DirectoryDownloadApplication = MainDirectoryApplication + @"Download/";

        #endregion

        /// <summary>
        /// Условное создание ресурсов приложения, при их отсутствии
        /// </summary>
        internal static void CheckCreateAllResources()
        {
            App.LogWriteLine("Проверка ресурсных файлов");
            string Prefics;
            PathesFromNameResource.Clear();
            CheckCreateDirectoryInFile(DirectoryImagesApplication);
            CheckCreateDirectoryInFile(DirectoryAudioApplication);
            CheckCreateDirectoryInFile(DirectoryMediaApplication);
            CheckCreateDirectoryInFile(DirectoryDictionaryApplication);
            foreach (PropertyInfo prop in typeof(OperPageLes.Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (prop.PropertyType == typeof(byte[]))
                {
                    if (prop.Name.Contains("Audio")) Prefics = DirectoryAudioApplication + $"{prop.Name}.mp3";
                    else if (prop.Name.Contains("Media")) Prefics = DirectoryMediaApplication + $"{prop.Name}.mp4";
                    else if (prop.Name.Contains("Dictionary")) Prefics = DirectoryDictionaryApplication + $"{prop.Name}.xaml";
                    else if (prop.Name.Contains("Lang"))
                    {
                        byte[] BytesJSONData = (byte[]?)prop.GetValue(null) ?? throw new Exception("Ресурс является нулевым.");
                        string JSONContent = System.Text.Encoding.UTF8.GetString(BytesJSONData);
                        Lang.AppendNewLanguage(JSONContent);
                        continue;
                    }
                    else if (prop.Name.Contains("Flag")) Prefics = Lang.DirectoryImageFlagsApplication + $"{prop.Name}.png";
                    else Prefics = DirectoryImagesApplication + $"{prop.Name}.png";
                    if (!File.Exists(Prefics) || prop.Name.Contains("Dictionary"))
                        CreateResourceMedia(Prefics, (byte[]?)prop.GetValue(null) ?? throw new Exception("Ресурс является нулевым."));
                        PathesFromNameResource.Add(prop.Name, Prefics);
                    if (Prefics.Contains(".png"))
                    {
                        BitmapImage bitmap = new(new Uri(Prefics));
                        bitmap.Freeze();
                        ResourcesImages.Add(prop.Name, bitmap);
                    }
                    else if (Prefics.Contains(".mp4")) ResourcesMedia.Add(prop.Name, new Uri(Prefics));
                    //else if (Prefics.Contains(".mp3"))
                    //{
                    //    int h = Bass.BASS_SampleLoad(Prefics, 0L, 0, 5, BASSFlag.BASS_DEFAULT);

                    //    if (h == 0)
                    //    {
                    //        BASSError errorCode = Bass.BASS_ErrorGetCode();
                    //        Console.WriteLine($"Ошибка загрузки: {errorCode}");
                    //    }
                    //    else
                    //        Samples.Add(prop.Name, h);
                    //}
                }
            }
        }

        /// <summary>
        /// Создать файл ресурса медиа-файла
        /// </summary>
        /// <param name="DirectoryMedia">Директория медиа-файла</param>
        /// <param name="ResourceSource">Данные медиа для записи</param>
        private static void CreateResourceMedia(string DirectoryMedia, byte[] ResourceSource)
        {
            CheckCreateDirectoryInFile(DirectoryMedia);
            FileStream stream = File.OpenWrite(DirectoryMedia);
            stream.Position = 0;
            stream.Write(ResourceSource);
            stream.Close();
        }

        /// <summary>
        /// Открыть запись в файл .log<br/>
        /// При отсутствии данного файла создаёт новый.
        /// </summary>
        /// <param name="NameFile">Имя открываемого файла</param>
        internal static StreamWriter CreateLogStreamWriter(string NameFile)
        {
            if (!File.Exists(MainDirectoryApplication + @$"/Log/")) Directory.CreateDirectory(MainDirectoryApplication + @$"/Log/");
            return File.AppendText(MainDirectoryApplication + @$"/Log/{NameFile}.log");
        }

        /// <summary>
        /// Проверка наличия директории, при её отсутствии создаёт директорию которая привязана к файлу.<br/>
        /// При передаче исключительно директории проверяет её и не масштабирует.
        /// </summary>
        /// <param name="path">Директория поверки</param>
        internal static void CheckCreateDirectoryInFile(string path)
        {
            string DirectoryFolder = Path.GetDirectoryName(path) ?? string.Empty;
            if (!Directory.Exists(DirectoryFolder)) Directory.CreateDirectory(DirectoryFolder);
        }

        /// <summary>
        /// Узнать директорию нахождения ресурса по его имени
        /// </summary>
        /// <param name="NameResource">Имя ресурса</param>
        /// <returns>Директория файла ресурса</returns>
        internal static string GetResourcePath(string NameResource) => PathesFromNameResource[NameResource];

        /// <summary>
        /// Получить ссылку на файл ресурса по его имени
        /// </summary>
        /// <param name="NameResource">Имя ресурса</param>
        internal static Uri GetResourceUri(string NameResource) => ResourcesMedia[NameResource];

        /// <summary>
        /// Получить картинку ресурса по его имени
        /// </summary>
        /// <param name="NameResource">Имя ресурса</param>
        internal static BitmapImage GetResourceBitmap(string NameResource) => ResourcesImages[NameResource];
    }
}
