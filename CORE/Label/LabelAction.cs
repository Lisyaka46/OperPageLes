using OperPage_les.CORE.Label;
using static OperPage_les.CORE.Label.LabelTag;

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
        /// Тег ярлыка
        /// </summary>
        public LabelTag? Tag { get; private set; }

        /// <summary>
        /// Добавить тег в ярлык
        /// </summary>
        /// <param name="NewTag">Добавляемый объект тега</param>
        internal void AppendTag(LabelTag NewTag)
        {
            Tag = NewTag;
            SetTag?.Invoke(null, NewTag);
        }

        /// <summary>
        /// Удалить тег из ярлыка
        /// </summary>
        internal void RemoveTag()
        {
            DeleteTag?.Invoke(Tag, null);
            Tag = null;
        }
    }
}
