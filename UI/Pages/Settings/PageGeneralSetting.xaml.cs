using AAC20.Windows.Pages.Other;
using IEL.Interfaces.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AAC20.UI.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для PageGeneral.xaml
    /// </summary>
    public partial class PageGeneralSetting : Page, IPageDefault
    {
        /// <summary>
        /// Имя страницы
        /// </summary>
        public string PageName { get; } = nameof(PageGeneralSetting);

        /// <summary>
        /// Делегат события изменения настроек
        /// </summary>
        internal delegate void ChangeValue(string Name, string NewValue);

        /// <summary>
        /// Событие изщменения значения настроек
        /// </summary>
        internal event ChangeValue? EventChangeValue;

        internal PageGeneralSetting()
        {
            InitializeComponent();
            IElButtonDialogDirectoryFile.OnActivateMouseLeft += () =>
            {
                OpenFileDialog dialog = new()
                {
                    //FileName = "Обзор файла изображения", // Default file name
                    DefaultExt = ".png", // Default file extension
                    Filter =
                    "Растровое изображение|*.png|" +
                    "Сжатое изображение|*.jpeg;*.jpg|" +
                    "Карта растрового изображения|*.bmp" // Filter files by extension
                };
                dialog.FileOk += (sender, e) =>
                {
                    TextBoxPathMenuImage.Text = dialog.FileName;
                    EventChangeValue?.Invoke("PathMenuImage", dialog.FileName);
                };
                dialog.ShowDialog();
            };
        }
    }
}
