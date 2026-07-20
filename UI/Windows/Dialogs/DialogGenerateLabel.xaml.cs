using OperPageLes.CORE.Enums.Language;
using OperPageLes.CORE.Objects;
using OperPageLes.UI.UserElementsControl.Default;
using OPLAPI.CORE.Language;
using System.Windows;
using System.Windows.Input;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowGenLabel.xaml
    /// </summary>
    public partial class DialogGenLabel : Window
    {
        /// <summary>
        /// Состояние отмены создания ярлыка
        /// </summary>
        private bool Cancel = true;

        /// <summary>
        /// Выделенный индекс используемого спектра в отображении ярлыка Aquamarine
        /// </summary>
        private uint SelectIndexSpectrum = 17u;

        public DialogGenLabel()
        {
            InitializeComponent();
            RunNameShortcut.Text = Lang.GetValue(LangUITranslate.ShortcutName);
            RunCommandShortcut.Text = Lang.GetValue(LangUITranslate.ShortcutCommand);
            RunDescriptionShortcut.Text = Lang.GetValue(LangUITranslate.ShortcutDescription);
            RunNameRequiredSymbol.Text = Lang.GetValue(LangUITranslate.RequiredMarker);
            RunCommandRequiredSymbol.Text = Lang.GetValue(LangUITranslate.RequiredMarker);
            IELButtonCancel.Text = Lang.GetValue(LangUITranslate.Cancel);

            ButtonSelectSpectrumTheme.PaletteElement = App.CurrentApp.ActiveThemeApplication[(CORE.Enums.PaletteSpectrumEnum)SelectIndexSpectrum];
            IELTextBoxNameLabel.Text = string.Empty;
            IELTextBoxCommand.Text = string.Empty;
            IELTextBoxDescription.Text = string.Empty;

            //HitInterpreter.ManagerAnimation = App.CurrentApp.ManagerAnimation;
            HitInterpreter.Connect(App.CurrentApp.Interpreter, in IELTextBoxCommand.TextBoxMain);
            HitInterpreter.Height = 0d;
            HitInterpreter.Width = 0d;

            IELButtonCancel.OnActivateMouseLeft += (sender, e) =>
            {
                Cancel = true;
                Close();
            };
            IELButtonCreateLabel.OnActivateMouseLeft += (sender, e) =>
            {
                Create();
            };
            IELTextBoxNameLabel.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Down:
                    case Key.Enter:
                        IELTextBoxCommand.Focus();
                        //if (App.CurrentApp.SettingMainApplication.HitUse)
                        //    HitInterpreter.UpdateState(IELTextBoxCommand.Text);
                        break;
                    case Key.Escape:
                        Close();
                        break;
                }
            };
            IELTextBoxCommand.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Down:
                    case Key.Enter:
                        IELTextBoxDescription.Focus();
                        HitInterpreter.ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);
                        break;
                    case Key.Up:
                        IELTextBoxNameLabel.Focus();
                        HitInterpreter.ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);
                        break;
                    case Key.Escape:
                        Close();
                        break;
                    default:
                        //if (App.CurrentApp.SettingMainApplication.HitUse)
                        //{
                        //    HitInterpreter.UpdateState(IELTextBoxCommand.Text);
                        //}
                        break;
                }
                e.Handled = true;
            };
            IELTextBoxCommand.LostFocus += (sender, e) =>
            {
                HitInterpreter.ChangeVisualHintCommand(OPLHitInterpreter.HitStateEnum.Hidden);
            };
            IELTextBoxCommand.GotFocus += (sender, e) =>
            {
                //if (App.CurrentApp.SettingMainApplication.HitUse)
                //{
                //    HitInterpreter.UpdateState(IELTextBoxCommand.Text);
                //}
            };
            IELTextBoxDescription.KeyUp += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        Create();
                        break;
                    case Key.Up:
                        IELTextBoxCommand.Focus();
                        //if (App.CurrentApp.SettingMainApplication.HitUse)
                        //    HitInterpreter.UpdateState(IELTextBoxCommand.Text);
                        break;
                    case Key.Escape:
                        Close();
                        break;
                }
            };
            ButtonSelectSpectrumTheme.MouseLeftButtonUp += (sender, e) =>
            {
                try
                {
                    ButtonSelectSpectrumTheme.PaletteElement = App.CurrentApp.ActiveThemeApplication[(CORE.Enums.PaletteSpectrumEnum)(++SelectIndexSpectrum)];
                }
                catch
                {
                    SelectIndexSpectrum = 0u;
                    ButtonSelectSpectrumTheme.PaletteElement = App.CurrentApp.ActiveThemeApplication[(CORE.Enums.PaletteSpectrumEnum)0u];
                }
            };
            ButtonSelectSpectrumTheme.MouseRightButtonUp += (sender, e) =>
            {
                try
                {
                    ButtonSelectSpectrumTheme.PaletteElement = App.CurrentApp.ActiveThemeApplication[(CORE.Enums.PaletteSpectrumEnum)(--SelectIndexSpectrum)];
                }
                catch
                {
                    CORE.Enums.PaletteSpectrumEnum[] Array = Enum.GetValues<CORE.Enums.PaletteSpectrumEnum>();
                    SelectIndexSpectrum = (uint)(Array.Length - 1);
                    ButtonSelectSpectrumTheme.PaletteElement = App.CurrentApp.ActiveThemeApplication[Array[^1]];
                }
            };
            Loaded += (sender, e) =>
            {
                IELTextBoxNameLabel.Focus();
            };
        }

        /// <summary>
        /// Сгенерировать итоговое исполнение создания ярлыка
        /// </summary>
        private void Create()
        {
            if (IELTextBoxNameLabel.Text.Length == 0)
            {
                //IELTextBoxNameLabel.SetActiveSpecrum(Colors.Red, false);
                return;
            }
            if (IELTextBoxCommand.Text.Length == 0)
            {
                //IELTextBoxCommand.SetActiveSpecrum(Colors.Red, false);
                return;
            }
            Cancel = false;
            Close();
        }

        /// <summary>
        /// Создать ярлык с помощью диалогового окна
        /// </summary>
        /// <returns>Созданный объект ярлыка</returns>
        internal SourceLabelAction? CreateLabel()
        {
            Title = Lang.GetValue(LangUITranslate.CreateShortcutTitle);
            IELButtonCreateLabel.Text = Lang.GetValue(LangUITranslate.ShortcutCreate);
            IELTextBoxNameLabel.Focus();
            ShowDialog();
            if (Cancel) return null;
            return new(IELTextBoxNameLabel.Text, IELTextBoxCommand.Text, IELTextBoxDescription.Text)
            {
                IndexSpectrumTheme = (int)SelectIndexSpectrum
            };
        }

        /// <summary>
        /// Изменить ярлык с помощью диалогового окна
        /// </summary>
        /// <param name="Source">Изменяемый объект ярлыка</param>
        /// <returns>Созданный объект ярлыка</returns>
        internal void ChangeLabel(in SourceLabelAction Source)
        {
            Title = Lang.GetValue(LangUITranslate.ChangeShortcutTitle);
            IELButtonCreateLabel.Text = Lang.GetValue(LangUITranslate.ShortcutChange);
            IELTextBoxNameLabel.Focus();

            IELTextBoxNameLabel.Text = Source.Name;
            IELTextBoxCommand.Text = Source.Command;
            IELTextBoxDescription.Text = Source.Description ?? string.Empty;
            ButtonSelectSpectrumTheme.PaletteElement = App.CurrentApp.ActiveThemeApplication[(CORE.Enums.PaletteSpectrumEnum)Source.IndexSpectrumTheme];
            SelectIndexSpectrum = (uint)Source.IndexSpectrumTheme;

            ShowDialog();
            if (Cancel) return;
            Source.Name = IELTextBoxNameLabel.Text;
            Source.Command = IELTextBoxCommand.Text;
            Source.Description = IELTextBoxDescription.Text;
            Source.IndexSpectrumTheme = (int)SelectIndexSpectrum;
        }
    }
}
