using OperPageLes.CORE.Label;
using static OperPageLes.CORE.Label.LabelTag;

namespace IEL.CORE.Classes
{
    public class LabelAction(string name, string description, string command)
    {
        /// <summary>
        /// Событие добавления ярлыка
        /// </summary>
        internal event ValueChangedHandler<LabelTag>? SetTag;

        /// <summary>
        /// Событие удаления ярлыка
        /// </summary>
        internal event ValueChangedHandler<LabelTag>? DeleteTag;

        /// <summary>
        /// Имя ярлыка
        /// </summary>
        public string Name { set; get; } = name;

        /// <summary>
        /// Описчание ярлыка
        /// </summary>
        public string? Description { set; get; } = description;

        /// <summary>
        /// Команда реализуемая ярлыком
        /// </summary>
        public string Command { get; set; } = command;

        /// <summary>
        /// Имя тега ярлыка
        /// </summary>
        public string? Tag
        {
            get => _Tag?.ValueTag;
            set
            {
                if (value == null)
                {
                    RemoveTag();
                    return;
                }
                if (_Tag != null) _Tag.ValueTag = value;
                else _Tag = new(value);
            }
        }

        /// <summary>
        /// Тег ярлыка
        /// </summary>
        private LabelTag? _Tag;

        /// <summary>
        /// Добавить тег в ярлык
        /// </summary>
        /// <param name="NewTag">Добавляемый объект тега</param>
        internal void AppendTag(LabelTag NewTag)
        {
            _Tag = NewTag;
            SetTag?.Invoke(null, NewTag);
        }

        /// <summary>
        /// Удалить тег из ярлыка
        /// </summary>
        internal void RemoveTag()
        {
            DeleteTag?.Invoke(_Tag, null);
            _Tag = null;
        }
    }
}
