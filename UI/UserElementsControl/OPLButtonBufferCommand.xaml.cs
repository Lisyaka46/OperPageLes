using IEL.CORE.Classes.ObjectSettings;
using System.Windows;
using FontFamily = System.Windows.Media.FontFamily;
using IEL.UserElementsControl.Base;

namespace ApplicationOperPageLes.UI.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLButtonBufferCommand.xaml
    /// </summary>
    public partial class OPLButtonBufferCommand : IELButtonBase
    {
        /// <summary>
        /// Текст кнопки
        /// </summary>
        public string Text
        {
            get => TextBlockButtonName.Text;
            set => TextBlockButtonName.Text = value;
        }

        /// <summary>
        /// Текст команды
        /// </summary>
        public string TextCommand
        {
            get => TextBlockButtonCommand.Text;
            set => TextBlockButtonCommand.Text = value;
        }

        private int _Index;
        /// <summary>
        /// Индекс элемента 
        /// </summary>
        public int Index
        {
            get => _Index;
            set
            {
                TextBlockNumberCommand.Text = $"#{value + 1}";
                _Index = value;
            }
        }

        /// <summary>
        /// Шрифт текста в кнопке
        /// </summary>
        public FontFamily TextFontFamily
        {
            get => TextBlockButtonName.FontFamily;
            set => TextBlockButtonName.FontFamily = value;
        }

        /// <summary>
        /// Размер текста в кнопке
        /// </summary>
        public double TextFontSize
        {
            get => TextBlockButtonName.FontSize;
            set => TextBlockButtonName.FontSize = value;
        }

        public OPLButtonBufferCommand(string Name, string FullTextCommand, int indexBuffer)
        {
            InitializeComponent();
            #region Foreground
            TextBlockButtonName.Foreground = SourceForeground.SourceBrush;
            TextBlockButtonCommand.Foreground = SourceForeground.SourceBrush;
            TextBlockNumberCommand.Foreground = SourceForeground.SourceBrush;
            #endregion
            TextFontFamily = new FontFamily("Arial");
            TextFontSize = 14;
            TextBlockButtonName.FontWeight = FontWeights.Bold;
            Text = Name;
            TextBlockButtonCommand.Text = FullTextCommand;
            CornerRadius = new CornerRadius(10);
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Top;
            Height = 27;
            Width = 230;
            Index = indexBuffer;
        }
    }
}
