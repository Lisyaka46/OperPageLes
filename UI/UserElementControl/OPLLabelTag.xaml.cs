using IEL.CORE.Classes.ObjectSettings;
using OperPage_les.CORE.Label;
using System.Windows;

namespace OperPage_les.UI.UserElementControl
{
    /// <summary>
    /// Логика взаимодействия для OPLLabelTag.xaml
    /// </summary>
    public partial class OPLLabelTag : System.Windows.Controls.UserControl
    {
        internal delegate void ChangedValueHandler<T>(T OldValue, T NewValue);
        /// <summary>
        /// Настройка использования объекта
        /// </summary>
        public IELUsingObjectSetting IELSettingObject
        {
            get => IELTag.IELSettingObject;
            set => IELTag.IELSettingObject = value;
        }

        /// <summary>
        /// скругление границ объекта
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => IELTag.CornerRadius;
            set => IELTag.CornerRadius = value;
        }

        /// <summary>
        /// Толщина границ объекта
        /// </summary>
        public Thickness BorderThicknessBlock
        {
            get => IELTag.BorderThicknessBlock;
            set => IELTag.BorderThicknessBlock = value;
        }

        /// <summary>
        /// Смещение контента в объекте
        /// </summary>
        public Thickness PaddingContent
        {
            get => IELTag.PaddingContent;
            set => IELTag.PaddingContent = value;
        }

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
