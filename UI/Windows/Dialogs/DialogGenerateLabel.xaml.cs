using OperPageLes.CORE.Objects;
using System.Windows;
using System.Windows.Controls;
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
            ButtonSelectSpectrumTheme.PaletteElement = App.CurrentApp.ActiveThemeApplication[(CORE.Enums.PaletteSpectrumEnum)SelectIndexSpectrum];
            IELTextBoxNameLabel.Text = string.Empty;
            IELTextBoxCommand.Text = string.Empty;
            IELTextBoxDescription.Text = string.Empty;
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
                        break;
                    case Key.Up:
                        IELTextBoxNameLabel.Focus();
                        break;
                    case Key.Escape:
                        Close();
                        break;
                }
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
            Title = "Создание ярлыка";
            IELButtonCreateLabel.Text = "Создать ярлык";
            Focus();
            ShowDialog();
            if (Cancel) return null;
            return new(IELTextBoxNameLabel.Text, IELTextBoxCommand.Text, IELTextBoxDescription.Text)
            {
                IndexSpectrumTheme = (int)SelectIndexSpectrum
            };
        }

        ///// <summary>
        ///// Изменить ярлык с помощью диалогового окна
        ///// </summary>
        ///// <param name="Source">Изменяемый объект ярлыка</param>
        ///// <returns>Изменённый объект ярлыка</returns>
        //internal void ChangeLabel(OPLLabelAction Source)
        //{
        //    LabelAction Label = Source.SourceLabel;
        //    IELTextBoxNameLabel.Text = Label.Name;
        //    IELTextBoxDescription.Text = Label.Description ?? string.Empty;
        //    IELTextBoxCommand.Text = Label.Command;
        //    IELButtonCreateLabel.Text = "Изменить ярлык";
        //    Title = "Изменение ярлыка";
        //    ShowDialog();
        //    if (Cancel ||
        //        (
        //            IELTextBoxNameLabel.Text.Equals(Label.Name) &&
        //            IELTextBoxDescription.Text.Equals(Label.Description) &&
        //            IELTextBoxCommand.Text.Equals(Label.Command)
        //        )) return;
        //    Label.Name = IELTextBoxNameLabel.Text;
        //    Label.Description = IELTextBoxDescription.Text.Length > 0 ? IELTextBoxDescription.Text : string.Empty;
        //    Label.Command = IELTextBoxCommand.Text;
        //    Source.UpdateLayout();
        //    return;
        //}
    }
}
