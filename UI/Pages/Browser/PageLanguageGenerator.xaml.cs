using OperPageLes.CORE.Enums.Language;
using OPLAPI.CORE.Language;
using OPLAPI.OIEL.CORE.Browser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace OperPageLes.UI.Pages.Browser
{
    /// <summary>
    /// Логика взаимодействия для PageLanguageGenerator.xaml
    /// </summary>
    public partial class PageLanguageGeneratorApp : PageBrowser
    {
        public PageLanguageGeneratorApp()
        {
            InitializeComponent();
            Lang.LanguageUpdated += Lang_LanguageUpdated;
        }

        /// <summary>
        /// Обработчик обновления языкового перевода
        /// </summary>
        private void Lang_LanguageUpdated(object? sender, EventArgs e)
        {
            TextBlockLanguageLocate.Text = Lang.GetValue(LangUITranslate.Localization);
            TextBlockLanguageName.Text = Lang.GetValue(LangUITranslate.Naming);
            TextBlockLanguageVersion.Text = Lang.GetValue(LangUITranslate.Naming);
            TextBlockSourceLanguage.Text = Lang.GetValue(LangGenDictionaryTranslate.SourceLanguage);
            TextBlockLanguagePathSave.Text = Lang.GetValue(LangUITranslate.DirectorySave);
            IELButtonAutors.Text = Lang.GetValue(LangUITranslate.Autors);
            IELButtonOverview.Text = Lang.GetValue(LangUITranslate.Overview);
            //IELButtonManipulation.Text = Lang.GetValue(LangUITranslate.Autors);
            IELButtonCancel.Text = Lang.GetValue(LangUITranslate.Cancel);
        }
    }
}
