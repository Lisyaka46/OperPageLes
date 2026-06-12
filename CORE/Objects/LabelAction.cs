using InterpreterCommand.Classes;
using OPLAPI.OIEL.UserElementsControl;
using OperPageLes.CORE.Struct;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Storage;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.CORE.Objects
{
    public class LabelAction
    {
        /// <summary>
        /// Имя ярлыка
        /// </summary>
        public readonly SourceLabelAction Label;

        /// <summary>
        /// Визуальный элемент представления объекта
        /// </summary>
        internal readonly OPLVisualElementIM VisualELement;

        /// <summary>
        /// Сохранённая прошлая команда ярлыка для проверки её изменения
        /// </summary>
        private string SaveOldCommand = string.Empty;

        /// <summary>
        /// Инициализировать объект ярлыка
        /// </summary>
        /// <param name="Source">Объект данных ярлыка</param>
        /// <param name="SourceSize">Размер визуального элемента</param>
        internal LabelAction(SourceLabelAction Source, System.Windows.Size SourceSize)
        {
            Label = Source;
            VisualELement = new()
            {
                //ManagerAnimation = App.CurrentApp.ManagerAnimation,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Margin = new(3d),
            };
            VisualELement.SetSizeIconApp(SourceSize);
            UpdateVisualLabel();
        }

        /// <summary>
        /// Установить иконку сайта на ярлык по событию
        /// </summary>
        private void LoadFaviconIcon(object? sender, ObjectConnectEventArgs e)
        {
            if (!e.Connect) return;
            App.CurrentApp.ConnectionPingChanged -= LoadFaviconIcon;
            LoadFaviconIcon();
        }

        /// <summary>
        /// Установить иконку сайта на ярлык
        /// </summary>
        internal void LoadFaviconIcon()
        {
            string[] Params = COMInterpreterBase.ReadParametersCommand(Label.Command);
            VisualELement.Dispatcher.Invoke(async () =>
            {
                VisualELement.ActivateVisualMedia();
                await Task.Delay(300);
                try
                {
                    VisualELement.ChangeSourceImage(await App.CurrentApp.DownloadFavicon(new(Params[0])));
                }
                catch
                {
                    VisualELement.ChangeSourceImage(StructDirectoryResources.GetResourceBitmap(nameof(OPRES.World)));
                }
                finally
                {
                    VisualELement.DiactivateVisualMedia();
                }
            });
        }

        /// <summary>
        /// Активировать 
        /// </summary>
        /// <returns></returns>
        internal async Task Activate()
        {
            await App.CurrentApp.Interpreter.ReadAndExecuteCommand(null, Label.Command, null, Interpreter.Classes.CommandLevel.LowLevel);
        }

        /// <summary>
        /// Обновить отображение данных ярлыка
        /// </summary>
        internal void UpdateVisualLabel()
        {
            if (Label.Name.Length > 64)
                Label.Name = Label.Name[..64];
            VisualELement.Text = Label.Name;
            try
            {
                VisualELement.PaletteElement = App.CurrentApp.ActiveThemeApplication[(Enums.PaletteSpectrumEnum)(uint)Label.IndexSpectrumTheme];
            }
            catch
            {
                VisualELement.PaletteElement = App.CurrentApp.ActiveThemeApplication[Enums.PaletteSpectrumEnum.Aquamarine];
                App.CurrentApp.AddNewNotification($"Не удалось присвоить спектр темы \"{Label.IndexSpectrumTheme}\" ярлыку \"{Label.Name}\"", Enums.EnumNotificationStyle.System,
                    null, "Ошибка ярлыка");
            }
            if (SaveOldCommand.Equals(Label.Command)) return;
            else if (Label.Command.Contains("open_link"))
            {
                VisualELement.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Link));
                App.CurrentApp.ConnectionPingChanged += LoadFaviconIcon;
            }
            else if (Label.Command.Contains("open_directory"))
                VisualELement.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Folder));
            else if (Label.Command.Contains("open_file"))
            {
                string[] Param = COMInterpreterBase.ReadParametersCommand(Label.Command);
                if (Param.Length > 0)
                {
                    try
                    {
                        Icon? icon = System.Drawing.Icon.ExtractAssociatedIcon(Param[0]);
                        VisualELement.Source = icon != null ?
                            Imaging.CreateBitmapSourceFromHBitmap(icon.ToBitmap().GetHbitmap(),
                            IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) :
                            StructDirectoryResources.GetResourceBitmap(nameof(OPRES.File));
                    }
                    catch
                    {
                        VisualELement.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.File));
                    }
                }
                else
                    VisualELement.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.File));
            }
            else if (Label.Command.Length > 0)
                VisualELement.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Command));
            SaveOldCommand = Label.Command;
        }
    }

    /// <summary>
    /// Ярлык содержащий данные
    /// </summary>
    /// <param name="name">Имя</param>
    /// <param name="command">Команда</param>
    /// <param name="description">Описание</param>
    public class SourceLabelAction(string name, string command, string description, int Index = 17)
    {
        /// <summary>
        /// Пустой ярлык
        /// </summary>
        public static SourceLabelAction Empty => new(string.Empty, string.Empty, string.Empty);
        /// <summary>
        /// Имя ярлыка
        /// </summary>
        public string Name { get; internal set; } = name;

        /// <summary>
        /// Команда реализуемая ярлыком
        /// </summary>
        public string Command { get; internal set; } = command;

        /// <summary>
        /// Описание ярлыка
        /// </summary>
        public string? Description { get; internal set; } = description;

        /// <summary>
        /// Индекс спектра темы для присвоения определённого цветового отображения
        /// </summary>
        public int IndexSpectrumTheme { get; set; } = Index; // Aquamarine

        /// <summary>
        /// Свойство форматирования описания ярлыка
        /// </summary>
        public bool DescriptionFormattedBIU { get; set; }

        /// <summary>
        /// Сворачивать приложение при выполнении ярлыка
        /// </summary>
        public bool ExecuteMinimized { get; set; }
    }
}
