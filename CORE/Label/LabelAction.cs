using IEL.Interfaces.Core;
using OperPage_les.CORE.Label;
using static OperPage_les.CORE.Label.LabelTag;

namespace IEL.CORE.Classes
{
    public class LabelAction(string name, string description, string command)
    {
        /// <summary>
        /// Событие добавления ярлыка
        /// </summary>
        internal event ValueChangedHandler<LabelTag>? AddTag;

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

        private readonly List<LabelTag> _Tags = [];
        /// <summary>
        /// Теги ярлыка
        /// </summary>
        public List<LabelTag> Tags
        {
            get => _Tags;
        }

        /// <summary>
        /// Добавить тег в ярлык
        /// </summary>
        /// <param name="index">Индекс тега</param>
        internal void AppendTag(LabelTag NewTag)
        {
            Tags.Add(NewTag);
            AddTag?.Invoke(null, Tags[^1]);
        }

        /// <summary>
        /// Удалить тег по индексу из ярлыка
        /// </summary>
        /// <param name="index">Индекс тега</param>
        internal void RemoveAtTag(int index)
        {
            DeleteTag?.Invoke(Tags[index], null);
            Tags.RemoveAt(index);
        }

        /// <summary>
        /// Удалить тег из ярлыка
        /// </summary>
        /// <param name="tag">Тег</param>
        internal void RemoveTag(LabelTag tag)
        {
            DeleteTag?.Invoke(tag, null);
            Tags.Remove(tag);
        }
    }
}
