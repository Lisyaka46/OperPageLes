using IEL.UserElementsControl;
using OperPageLes.CORE;
using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Settings.PaletteElements;
using OperPageLes.CORE.Struct;
using OPLAPI.CORE;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Pages.ActionPanel.PageLabel
{
    /// <summary>
    /// Логика взаимодействия для PageLabelActionPanel.xaml
    /// </summary>
    public partial class PageLabelElementActionPanel : Page
    {
        /// <summary>
        /// объект отображающий форматированное описание ярлыка
        /// </summary>
        private TextBlock TextBlockDescriptionHead;

        /// <summary>
        /// Форматированный текст содержащий описание ярлыка
        /// </summary>
        private TextBlock TextBlockDescriptionContent;

        //
        private StackPanel ConteinerDescription;

        public PageLabelElementActionPanel()
        {
            InitializeComponent();
            ConteinerDescription = new()
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
            };
            TextBlockDescriptionHead = new()
            {
                FontSize = 18d,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                TextAlignment = System.Windows.TextAlignment.Center,
            };
            TextBlockDescriptionHead.Inlines.Add(new Run("Описание ярлыка"));
            TextBlockDescriptionContent = new()
            {
                FontSize = 13d,
                TextWrapping = System.Windows.TextWrapping.Wrap,
                TextTrimming = System.Windows.TextTrimming.CharacterEllipsis,
                Padding = new(5, 2, 5, 2),
            };

            ConteinerDescription.Children.Add(TextBlockDescriptionHead);
            ConteinerDescription.Children.Add(TextBlockDescriptionContent);
            ScrollViewerDescription.Content = ConteinerDescription;
        }

        /// <summary>
        /// Изменить текст описания ярлыка
        /// </summary>
        /// <param name="Desctiption">Текст описания ярлыка</param>
        /// <param name="FormattedBIU">Форматировать ли текст по алгоритпу BIU</param>
        internal void ChangeTextDescription(string Desctiption, bool FormattedBIU = false)
        {
            if (!FormattedBIU)
            {
                TextBlockDescriptionContent.Text = Desctiption;
                TextBlockDescriptionHead.UpdateLayout();
                return;
            }
            else
            {
                TextBlockDescriptionContent.Inlines.Clear();
                TextBlockDescriptionContent.Inlines.Add(BIU.FormattedAllTextDetect(Desctiption));
            }
        }

        /// <summary>
        /// Установить отображение темы в объектах страницы
        /// </summary>
        /// <param name="ActiveTheme">Опорная тема</param>
        internal void SetVisualTheme(in Theme ActiveTheme)
        {
            ActiveTheme[PaletteSpectrumEnum.Green].ConnectPalleteFromIELElement(IELButtonExecuteLabel);
            ActiveTheme[PaletteSpectrumEnum.Purple].ConnectPalleteFromIELElement(IELButtonChangeLabel);
            ActiveTheme[PaletteSpectrumEnum.PastelRed].ConnectPalleteFromIELElement(IELButtonRemoveLabel);
            System.Windows.Data.Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Source = (System.Windows.Media.FontFamily)System.Windows.Application.Current.Resources["Deledda Open Regular"]
            };
            BindingOperations.SetBinding(TextBlockDescriptionHead, IELButtonText.FontFamilyProperty, binding);
            BindingOperations.SetBinding(TextBlockDescriptionContent, IELButtonText.FontFamilyProperty, binding);
        }
    }
}
