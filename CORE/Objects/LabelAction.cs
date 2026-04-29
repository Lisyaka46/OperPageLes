using InterpreterCommand.Classes;
using OIEL.UserElementsControl;
using OperPageLes.CORE.Struct;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        /// Инициализировать объект ярлыка
        /// </summary>
        /// <param name="Source">Объект данных ярлыка</param>
        /// <param name="SourceSize">Размер визуального элемента</param>
        internal LabelAction(SourceLabelAction Source, System.Windows.Size SourceSize)
        {
            if (Source.Name.Length > 64)
                Source.Name = Source.Name[..64];
            Label = Source;
            VisualELement = new()
            {
                ManagerAnimation = App.ManagerAnimation,
                Text = Label.Name,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Margin = new(3d),
            };
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

            VisualELement.SetSizeIconApp(SourceSize);

            if (Source.Command.Contains("open_link"))
            {
                VisualELement.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Link));
                App.ConnectionPingChanged += LoadFaviconIcon;
            }
            else if (Source.Command.Contains("open_directory"))
                VisualELement.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Folder));
            else if (Source.Command.Length > 0)
                VisualELement.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Command));
        }

        /// <summary>
        /// Установить иконку сайта на ярлык по событию
        /// </summary>
        private void LoadFaviconIcon(object? sender, ObjectConnectEventArgs e)
        {
            if (!e.Connect) return;
            App.ConnectionPingChanged -= LoadFaviconIcon;
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
            await App.CurrentApp.Interpreter.ReadAndExecuteCommand(null, Label.Command, null);
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
    }
}
