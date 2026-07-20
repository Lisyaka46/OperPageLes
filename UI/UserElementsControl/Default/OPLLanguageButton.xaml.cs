using IEL.UserElementsControl.Base;
using OperPageLes.CORE.Enums.Language;
using OperPageLes.CORE.Struct;
using OPLAPI.CORE.Language;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.UserElementsControl.Default
{
    /// <summary>
    /// Логика взаимодействия для OPLLanguageButton.xaml
    /// </summary>
    public partial class OPLLanguageButton : IELContainerBase
    {
        private LangInfo _DataContext;
        /// <summary>
        /// Данные на которые опирается отображение
        /// </summary>
        public new LangInfo DataContext
        {
            get => _DataContext;
            set
            {
                _DataContext = value;
                UpdateVisualData();
            }
        }

        public OPLLanguageButton()
        {
            InitializeComponent();
            _DataContext = LangInfo.Inknown;
            UpdateVisualData();
            TextBlockNameLanguage.Foreground = SourceForeground.SourceBrush;
            TextBlockVersionLanguage.Foreground = SourceForeground.SourceBrush;
            TextBlockAutor.Foreground = SourceForeground.SourceBrush;
            Lang.LanguageUpdated += Lang_LanguageUpdated;
        }

        /// <summary>
        /// Обработчик события изменения языкового перевода
        /// </summary>
        private void Lang_LanguageUpdated(object? sender, EventArgs e)
        {
            RunTextAutorTitle.Text = Lang.GetValue(LangUITranslate.Autor);
        }

        /// <summary>
        /// Обновить отображение данных в объекте от DataContext
        /// </summary>
        private void UpdateVisualData()
        {
            try { ImageBrushImageLanguage.ImageSource = StructDirectoryResources.GetResourceBitmap($"Flag{DataContext.Config.Locate}"); }
            catch { ImageBrushImageLanguage.ImageSource = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.World)); }

            TextBlockNameLanguage.Text = DataContext.Name;
            TextBlockVersionLanguage.Text = DataContext.Config.Version;
            RunTextAutorName.Text = DataContext.LangAutor.Name;
            IELButtonContacts.IsEnabled = DataContext.LangAutor.Contacts.Length > 0;
        }
    }
}
