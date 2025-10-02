namespace OperPageLes.CORE.Label
{
    public class LabelTag
    {
        internal delegate void VoidHandler();
        internal delegate void ValueChangedHandler<T>(T? OldValue, T? NewValue);

        /// <summary>
        /// Событие изменения значения тега
        /// </summary>
        internal event ValueChangedHandler<string> TagValueChanged;


        private string _ValueTag;
        /// <summary>
        /// Значение тега
        /// </summary>
        public string ValueTag
        {
            get => _ValueTag;
            set
            {
                //if (TagElement != null) TagElement.Text = $"{(value < 10 ? "0" : string.Empty)}{value}";
                TagValueChanged.Invoke(_ValueTag, value);
            }
        }

        public LabelTag(string value)
        {
            _ValueTag = value;
            TagValueChanged += (OldValue, NewValue) =>
            {
                _ValueTag = NewValue ?? string.Empty;
            };
        }
    }
}
