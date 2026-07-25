using IEL.UserElementsControl;
using IEL.UserElementsControl.Base;
using OperPageLes.CORE.Enums.Language;
using OperPageLes.CORE.Struct;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.CORE.Language;
using OPLAPI.CORE.Person;
using OPLAPI.OIEL.UserElementsControl;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
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
                Dispatcher.Invoke(UpdateVisualData);
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
            _DataContext = LangInfo.Unknown;
            StackVisualContacts = new()
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                ClipToBounds = false,
            };
            BorderContacts.Width = 0d;
            BorderContacts.Child = StackVisualContacts;
            TextBlockNameLanguage.Foreground = SourceForeground.SourceBrush;
            TextBlockVersionLanguage.Foreground = SourceForeground.SourceBrush;
            TextBlockPercentTranslate.Foreground = SourceForeground.SourceBrush;
            TextBlockAutor.Foreground = SourceForeground.SourceBrush;
            Lang.LanguageUpdated += Lang_LanguageUpdated;
            IELButtonContacts.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.ContactInfo));
            IELButtonContacts.OnActivateMouseLeft += (sender, e) =>
            {
                ChangeVisibleContacts();
                e.Handled = true;
            };
            MouseLeave += (sender, e) =>
            {
                if (ActiveVisualConracts) ChangeVisibleContacts();
            };
        }

        /// <summary>
        /// Изменить вид отображения контактов автора
        /// </summary>
        internal void ChangeVisibleContacts()
        {
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderContacts, WidthProperty,
                    ActiveVisualConracts ? 0d : StackVisualContacts.ActualWidth, TimeSpan.FromMilliseconds(460d));
            ActiveVisualConracts = !ActiveVisualConracts;
        }

        /// <summary>
        /// Обработчик события изменения языкового перевода
        /// </summary>
        private void Lang_LanguageUpdated(object? sender, EventArgs e)
        {
            RunTextAutorTitle.Text = Lang.GetValue(LangUITranslate.Autor);
            RunPercentTranslateName.Text = Lang.GetValue(LangUITranslate.Translated);
        }

        /// <summary>
        /// Обновить отображение данных в объекте от DataContext
        /// </summary>
        private async Task UpdateVisualData()
        {
            try { ImageBrushImageLanguage.ImageSource = StructDirectoryResources.GetResourceBitmap($"Flag{DataContext.Config.Locate}"); }
            catch { ImageBrushImageLanguage.ImageSource = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.World)); }

            TextBlockNameLanguage.Text = DataContext.Name;
            TextBlockVersionLanguage.Text = DataContext.Config.Version;
            RunPercentTranslateValue.Text = $"{(double)Math.Round(DataContext.PercentTranslate, 2) * 100d}%";
            RunTextAutorName.Text = DataContext.LangAutor.Name;
            IELButtonContacts.IsEnabled = DataContext.LangAutor.Contacts.Length > 0;
            await UpdateVisualContacts(DataContext.LangAutor.Contacts);
        }

        /// <summary>
        /// Обновить отображение списка контактов
        /// </summary>
        /// <param name="SourceContacts">Новый список контактов</param>
        private async Task UpdateVisualContacts(Contact[] SourceContacts)
        {
            IELButtonContacts.IsEnabled = false;
            if (SourceContacts.Length == 0) return;
            if (ActiveVisualConracts)
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderContacts, WidthProperty,
                    0d, TimeSpan.FromMilliseconds(460d));
                await Task.Delay(600);
            }
            OPLVisualElementIM ButtonContact;
            int i;
            for (i = 0; i < SourceContacts.Length; i++)
            {
                if (i < StackVisualContacts.Children.Count)
                {
                    ButtonContact = (OPLVisualElementIM)StackVisualContacts.Children[i];
                    ButtonContact.DataContext = SourceContacts[i];
                }
                else
                {
                    ButtonContact = GenerateButtonContact(SourceContacts[i]);
                    StackVisualContacts.Children.Add(ButtonContact);
                }
            }
            if (i < StackVisualContacts.Children.Count)
                StackVisualContacts.Children.RemoveRange(i + 1, StackVisualContacts.Children.Count - i);
            if (ActiveVisualConracts)
            {
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderContacts, WidthProperty,
                    StackVisualContacts.ActualWidth, TimeSpan.FromMilliseconds(1460d));
            }
            IELButtonContacts.IsEnabled = true;
        }

        /// <summary>
        /// Обновить отображение списка контактов
        /// </summary>
        /// <param name="SourceContacts">Новый список контактов</param>
        private OPLVisualElementIM GenerateButtonContact(Contact SourceContact)
        {
            OPLVisualElementIM Result = new()
            {
                ManagerAnimation = ManagerAnimation,
                PaletteElement = App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Terracotta],
                Margin = new(-8d, -5d, -8d, -5d),
                CornerRadius = new(15d),
                BorderThickness = new(2d),
                VisualOrientationName = OrientationName.Up,
                Text = SourceContact.Mask,
                FontSize = 9d,
                OffsetBorderNaming = 24d,
            };
            Result.SetSizeIconApp(new(20d, 20d));
            Result.OnActivateMouseLeft += (sender, e) =>
            {
                Contact Source = (Contact)((FrameworkElement)sender).DataContext;
                Process.Start(new ProcessStartInfo(Source.URL) { UseShellExecute = true });
                e.Handled = true;
            };
            Result.DataContextChanged += async (sender, e) => await ContactButton_DataContextChanged(sender, e);
            Result.DataContext = SourceContact;
            return Result;
        }

        /// <summary>
        /// Обработчик события изменения опорных данных для отображения контакта
        /// </summary>
        private async Task ContactButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OPLVisualElementIM Sender = (OPLVisualElementIM)sender;
            Contact Source = (Contact)Sender.DataContext;
            Sender.IsEnabled = false;
            Sender.Text = Source.Mask;
            Sender.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.World));
            try { Sender.Source = await App.CurrentApp.DownloadFavicon(new(Source.URL)); } catch { }
            Sender.IsEnabled = true;
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Sender, OPLVisualElementIM.OpacityProperty,
                1d, TimeSpan.FromMilliseconds(600d));
        }
    }
}
