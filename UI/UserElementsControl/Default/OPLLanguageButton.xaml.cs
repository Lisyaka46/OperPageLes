using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OperPageLes.CORE.Enums.Language;
using OperPageLes.CORE.Struct;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.CORE.Language;
using OPLAPI.CORE.Person;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.UserElementsControl.Default
{
    /// <summary>
    /// Логика взаимодействия для OPLLanguageButton.xaml
    /// </summary>
    public partial class OPLLanguageButton : IELContainerBase, IOPLAnimate
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

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Визуальный элемент отображающий контакты автора
        /// </summary>
        private StackPanel StackVisualContacts;

        /// <summary>
        /// Активное состояние отображения списка контактов
        /// </summary>
        private bool ActiveVisualConracts = false;

        public OPLLanguageButton()
        {
            InitializeComponent();
            _DataContext = LangInfo.Inknown;
            StackVisualContacts = new()
            {
                Width = 0d,
                Background = IELButtonContacts.SourceForeground.SourceBrush,
                Margin = new(0, 0, 8, 0),
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            };
            UpdateVisualData();
            TextBlockNameLanguage.Foreground = SourceForeground.SourceBrush;
            TextBlockVersionLanguage.Foreground = SourceForeground.SourceBrush;
            TextBlockAutor.Foreground = SourceForeground.SourceBrush;
            Lang.LanguageUpdated += Lang_LanguageUpdated;
            IELButtonContacts.OnActivateMouseLeft += (sender, e) =>
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, StackVisualContacts, WidthProperty,
                    ActiveVisualConracts ? 0d : StackVisualContacts.ActualWidth, TimeSpan.FromMilliseconds(460d));
                ActiveVisualConracts = !ActiveVisualConracts;
            };
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
            UpdateVisualContacts(DataContext.LangAutor.Contacts);
        }

        /// <summary>
        /// Обновить отображение списка контактов
        /// </summary>
        /// <param name="SourceContacts">Новый список контактов</param>
        private void UpdateVisualContacts(Contact[] SourceContacts)
        {
            IELButtonImage ButtonContact;
            for (int i = 0; i < SourceContacts.Length; i++)
            {
                ButtonContact = GenerateButtonContact(SourceContacts[i]);
            }
        }

        /// <summary>
        /// Обновить отображение списка контактов
        /// </summary>
        /// <param name="SourceContacts">Новый список контактов</param>
        private static IELButtonImage GenerateButtonContact(Contact SourceContact)
        {
            IELButtonImage Result = new()
            {
                DataContext = SourceContact,
            };
            Result.OnActivateMouseLeft += (sender, e) =>
            {
                Contact Source = (Contact)((FrameworkElement)sender).DataContext;
                Process.Start(new ProcessStartInfo(Source.URL) { UseShellExecute = true });
            };
            Result.DataContextChanged += async (sender, e) => await ContactButton_DataContextChanged(sender, e);
            return Result;
        }

        /// <summary>
        /// Обработчик события изменения опорных данных для отображения контакта
        /// </summary>
        private static async Task ContactButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Contact Source = (Contact)((FrameworkElement)sender).DataContext;
            await App.CurrentApp.DownloadFavicon(new(Source.URL));
        }
    }
}
