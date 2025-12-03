using NAudio.Wave;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ApplicationOperPageLes.CORE.Struct
{
    internal readonly struct StructDirectoryResources
    {
        /// <summary>
        /// Массив всех директорий ресурсов по их именам
        /// </summary>
        private static readonly Dictionary<string, string> PathesFromNameResource = [];

        /// <summary>
        /// Массив всех ресурсных картинок по их именам
        /// </summary>
        private static readonly Dictionary<string, BitmapImage> ResourcesImages = [];

        /// <summary>
        /// Объект исключения при использовании не инициализированного массива ресурсов
        /// </summary>
        private static readonly Exception ExceptionResourceNullDictionary =
            new("Для использования ресурсов их нужно инициализировать \"CheckCreateAllResources()\"");

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
        /// Главная директория ресурсов палитры
        /// </summary>
        internal static readonly string DirectoryDictionaryApplication = DirectoryResourcesApplication + @"/Dictionary/";

        /// <summary>
        /// Директория ресурса ярлыков
        /// </summary>
        internal static readonly string DirectoryDataLabels = DirectoryResourcesApplication + "Labels.json";

        /// <summary>
        /// Директория ресурса тегов для ярлыков
        /// </summary>
        internal static readonly string DirectoryDataLabelTags = DirectoryResourcesApplication + "Label_Tags.json";
        #endregion

        /// <summary>
        /// Директория файла валидного ключа
        /// </summary>
        internal static readonly string DirectoryKeyValidFile = MainDirectoryApplication + "Key";

        /// <summary>
        /// Главная директория файлов изображений
        /// </summary>
        internal static readonly string DirectoryImagesApplication = MainDirectoryApplication + @"/Images/";

        /// <summary>
        /// Главная директория файлов видео
        /// </summary>
        internal static readonly string DirectoryMediaApplication = DirectoryImagesApplication + @"/Media/";

        /// <summary>
        /// Главная директория файлов звука
        /// </summary>
        internal static readonly string DirectoryAudioApplication = MainDirectoryApplication + @"/Audio/";

        #endregion

        /// <summary>
        /// Функция перевода файла .json в ожидаемый объект.<br/>
        /// При отсутствии файла в директории создаёт новый экземпляр и возвращает предусматривающее значение объекта по умолчанию.<br/>
        /// <b>Такая логика задействуется также при ошибке перевода.</b>
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип объекта.<br/><b>Преобразуется в массив ожидаемого типа</b></typeparam>
        /// <param name="PathJSON">Директория читаемого .json файла преобразовываемый в массив ожидаемого типа</param>
        internal static T[] DeserializeObjectJson<T>(string PathJSON)
        {
            if (!File.Exists(PathJSON))
            {
                CheckCreateDirectoryInFile(PathJSON);
                File.WriteAllText(PathJSON, JsonConvert.SerializeObject(Array.Empty<T>()));
                return [];
            }
            return JsonConvert.DeserializeObject<T[]>(File.ReadAllText(PathJSON)) ?? [];
        }

        /// <summary>
        /// Условное создание ресурсов приложения, при их отсутствии
        /// </summary>
        internal static void CheckCreateAllResources()
        {
            App.CurrentApp.LogWriteLine("Проверка ресурсных файлов");
            string Prefics;
            bool SetBitmapImages = ResourcesImages.Count == 0;
            PathesFromNameResource.Clear();
            foreach (PropertyInfo prop in typeof(ApplicationOperPageLes.Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (prop.PropertyType == typeof(byte[]))
                {
                    if (prop.Name.Contains("Audio")) Prefics = DirectoryAudioApplication + $"{prop.Name}.mp3";
                    else if (prop.Name.Contains("Media")) Prefics = DirectoryMediaApplication + $"{prop.Name}.mp4";
                    else if (prop.Name.Contains("Dictionary")) Prefics = DirectoryDictionaryApplication + $"{prop.Name}.xaml";
                    else Prefics = DirectoryImagesApplication + $"{prop.Name}.png";
                    if (!File.Exists(Prefics) || prop.Name.Contains("Dictionary"))
                        CreateResourceMedia(Prefics, (byte[]?)prop.GetValue(null) ?? throw new Exception("Ресурс является нулевым."));
                    PathesFromNameResource.Add(prop.Name, Prefics);
                    if (Prefics.Contains(".png") && SetBitmapImages) ResourcesImages.Add(prop.Name, new(new Uri(Prefics)));
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

        //
        internal static void Play(WaveOut SourceWaveOut, string NameResourceSound) // mp3
        {
            if (PathesFromNameResource.Count == 0) throw new Exception("Для использования ресурсов их нужно инициализировать \"CheckCreateAllResources()\"");
            SourceWaveOut.Init(new Mp3FileReader(PathesFromNameResource[NameResourceSound]));
            SourceWaveOut.Play();
        }

        /// <summary>
        /// Узнать директорию нахождения ресурса по его имени
        /// </summary>
        /// <param name="NameResource">Имя ресурса</param>
        /// <returns>Директория файла ресурса</returns>
        internal static string GetResourcePath(string NameResource) => PathesFromNameResource[NameResource];

        /// <summary>
        /// Получить картинку ресурса по его имени
        /// </summary>
        /// <param name="NameResource">Имя ресурса</param>
        internal static BitmapImage GetResourceBitmap(string NameResource) => ResourcesImages[NameResource];
    }
}
