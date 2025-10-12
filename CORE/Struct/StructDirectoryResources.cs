using IEL.CORE.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OperPageLes.CORE.Struct
{
    internal readonly struct StructDirectoryResources
    {
        #region MainDirectoryApplication
        /// <summary>
        /// Главная директория ресурсов OperPageLes
        /// </summary>
        internal static readonly string MainDirectoryApplication = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/OperPageLes/";

        /// <summary>
        /// Главная директория файлов изображений
        /// </summary>
        internal static readonly string DirectoryImagesApplication = MainDirectoryApplication + @"/Images/";

        /// <summary>
        /// Директория файла валидного ключа
        /// </summary>
        internal static readonly string DirectoryKeyValidFile = MainDirectoryApplication + "Key";

        #region DirectoryResourcesApplication
        /// <summary>
        /// Главная директория ресурсов пользователя
        /// </summary>
        internal static readonly string DirectoryResourcesApplication = MainDirectoryApplication + @"/Resources/";

        /// <summary>
        /// Директория ресурса ярлыков
        /// </summary>
        internal static readonly string DirectoryDataLabels = DirectoryResourcesApplication + "Labels.json";

        /// <summary>
        /// Директория ресурса тегов для ярлыков
        /// </summary>
        internal static readonly string DirectoryDataLabelTags = DirectoryResourcesApplication + "Label_Tags.json";
        #endregion

        #region DirectoryMediaApplication
        /// <summary>
        /// Главная директория файлов видео
        /// </summary>
        internal static readonly string DirectoryMediaApplication = DirectoryImagesApplication + @"/Media/";

        /// <summary>
        /// Директория файла анимации обычной загрузки
        /// </summary>
        internal static readonly string DirectoryFileLoadingDefault = DirectoryMediaApplication + "LoadingDefault.mp4";

        /// <summary>
        /// Директория файла анимации загрузки для интернета
        /// </summary>
        internal static readonly string DirectoryFileLoadingInternet = DirectoryMediaApplication + "LoadingInternet.mp4";

        /// <summary>
        /// Директория файла праздничной анимации
        /// </summary>
        internal static readonly string DirectoryFileHappy = DirectoryMediaApplication + "Happy.mp4";
        #endregion

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
            CreateResourceMedia(DirectoryFileLoadingDefault, Properties.Resources.LoadingDefault);
            CreateResourceMedia(DirectoryFileLoadingInternet, Properties.Resources.LoadingInternet);
            CreateResourceMedia(DirectoryFileHappy, Properties.Resources.Happy);
        }

        /// <summary>
        /// Создать файл ресурса медиа-файла
        /// </summary>
        /// <param name="DirectoryMedia">Директория медиа-файла</param>
        /// <param name="ResourceSource">Данные медиа для записи</param>
        private static void CreateResourceMedia(string DirectoryMedia, byte[] ResourceSource)
        {
            if (File.Exists(DirectoryMedia)) return;
            CheckCreateDirectoryInFile(DirectoryMedia);
            FileStream stream = File.Create(DirectoryMedia);
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
    }
}
