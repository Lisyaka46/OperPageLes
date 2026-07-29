using OperPageLes.CORE.Enums.Theme;
using OperPageLes.UI.UserElementsControl.Default;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.CORE.Language;
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

namespace OperPageLes.UI.Pages.ActionPanel
{
    /// <summary>
    /// Логика взаимодействия для PageLanguageController.xaml
    /// </summary>
    public partial class PageLanguageController : Page, IOPLAnimate
    {
        private OPLAnimationManager? _ManagerAnimation;
        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation
        {
            get => _ManagerAnimation;
            set
            {
                _ManagerAnimation = value;
            }
        }

        /// <summary>
        /// Визуальный массив данных языковых переводов
        /// </summary>
        private StackPanel StackVisualLanguages;

        /// <summary>
        /// Визуальный объект активного языкового перевода
        /// </summary>
        private OPLLanguageButton? ActiveLanguageButton;

        public PageLanguageController()
        {
            InitializeComponent();
            StackVisualLanguages = new()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new(3d),
            };
            LanguagesScrollController.Content = StackVisualLanguages;
            TextBlockUpdateInfo.Opacity = 0d;
        }

        /// <summary>
        /// Обновить отображение списка языковых переводов
        /// </summary>
        /// <remarks>
        /// Обновляет и список установленных языковых переводов, и отображение списка
        /// </remarks>
        internal async Task UpdateListLanguages()
        {
            LanguagesScrollController.IsEnabled = false;
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockUpdateInfo, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(300d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, StackVisualLanguages, OpacityProperty,
                0d, TimeSpan.FromMilliseconds(500d));
            Lang.UpdateListLanguages();
            await Task.Delay(500);
            int i;
            OPLLanguageButton Button;
            for (i = 0; i < Lang.InstalledLanguages.Length; i++)
            {
                if (i >= StackVisualLanguages.Children.Count)
                {
                    Button = InicializeButtonLanguage(Lang.InstalledLanguages[i]);
                    StackVisualLanguages.Children.Add(Button);
                }
                else
                {
                    Button = (OPLLanguageButton)StackVisualLanguages.Children[i];
                    Button.DataContext = Lang.InstalledLanguages[i];
                }
                if (Lang.InstalledLanguages[i].Config.Locate.Equals(Lang.ActiveLang.Config.Locate))
                    ChangeVisualActiveLanguage(Button);
            }
            if (i < StackVisualLanguages.Children.Count)
                StackVisualLanguages.Children.RemoveRange(i + 1, StackVisualLanguages.Children.Count - i);
            LanguagesScrollController.IsEnabled = true;
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, TextBlockUpdateInfo, OpacityProperty,
                0d, TimeSpan.FromMilliseconds(300d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, StackVisualLanguages, OpacityProperty,
                1d, TimeSpan.FromMilliseconds(1100d));
        }

        /// <summary>
        /// Создать новый объект кнопки языкового перевода
        /// </summary>
        /// <param name="SourceInfo">Данные о языковом переводе</param>
        private OPLLanguageButton InicializeButtonLanguage(LangInfo SourceInfo)
        {
            OPLLanguageButton NewButton = new()
            {
                ManagerAnimation = ManagerAnimation,
                DataContext = SourceInfo,
                Palette = App.CurrentApp.ActiveThemeApplication[PaletteEnum.PlumCrayola],
                CornerRadius = new(6d),
                Margin = new(5d),
            };
            NewButton.MouseLeftButtonUp += (sender, e) =>
            {
                OPLLanguageButton Button = (OPLLanguageButton)sender;
                Lang.UpdateLang(Button.DataContext);
                ChangeVisualActiveLanguage(Button);
            };
            return NewButton;
        }

        /// <summary>
        /// Изменить активную кнопку языкового перевода
        /// </summary>
        /// <param name="NewActiveLangButton">Новая активная кнопка</param>
        private void ChangeVisualActiveLanguage(in OPLLanguageButton NewActiveLangButton)
        {
            ActiveLanguageButton?.SourceBackground.SetUsedState(false);
            NewActiveLangButton.SourceBackground.SetUsedState(true);
            ActiveLanguageButton = NewActiveLangButton;
        }
    }
}
