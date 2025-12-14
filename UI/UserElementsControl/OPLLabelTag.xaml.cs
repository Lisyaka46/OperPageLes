using ApplicationOperPageLes.CORE.Label;

namespace ApplicationOperPageLes.UI.UserElementsControl
{
    /// <summary>
    /// Логика взаимодействия для OPLLabelTag.xaml
    /// </summary>
    public partial class OPLLabelTag : System.Windows.Controls.UserControl
    {
        internal delegate void ChangedValueHandler<T>(T OldValue, T NewValue);

        /// <summary>
        /// Шрифт текста
        /// </summary>
        public new System.Windows.Media.FontFamily FontFamily
        {
            get => IELTag.FontFamily;
            set => IELTag.FontFamily = value;
        }

        /// <summary>
        /// Размер шрифта текста
        /// </summary>
        public new double FontSize
        {
            get => IELTag.FontSize;
            set => IELTag.FontSize = value;
        }

        /// <summary>
        /// Размер шрифта текста
        /// </summary>
        public string Text
        {
            get => IELTag.Text;
            set => IELTag.Text = value;
        }

        private LabelTag _Tag;
        /// <summary>
        /// Тег объекта
        /// </summary>
        internal new LabelTag Tag
        {
            get => _Tag;
            set
            {
                TagChanged.Invoke(_Tag, value);
            }
        }

        /// <summary>
        /// Событие изменения тега
        /// </summary>
        internal event ChangedValueHandler<LabelTag> TagChanged;

        public OPLLabelTag()
        {
            InitializeComponent();
            App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Lime].ConnectPalleteFromIELElement(IELTag);
            _Tag = new(string.Empty);
            TagChanged += (Old, New) =>
            {
                _Tag = New;
                _Tag.TagValueChanged += (OldV, NewV) =>
                {
                    IELTag.Text = NewV ?? string.Empty;
                };
                IELTag.Text = _Tag.ValueTag;
            };
        }
    }
}
