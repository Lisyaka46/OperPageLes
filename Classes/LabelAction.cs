using AAC20.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AAC20.Classes.Labels
{
    internal class LabelAction(string name, string description, string command) : ILabelAction
    {
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
    }

    internal class ListLabel<T>() where T : ILabelAction
    {
        /// <summary>
        /// Динамический массив объектов ярлыка
        /// </summary>
        private readonly List<T> ObjList = [];
        
        internal delegate void AddLabelAction(T label);
        internal delegate void RemoveLabelAction(int index);
        internal delegate void ClearLabelAction();

        internal event AddLabelAction? EventAddLabelAction;
        internal event RemoveLabelAction? EventRemoveLabelAction;
        internal event ClearLabelAction? EventClearLabelAction;

        internal int Count => ObjList.Count;

        public T this[Index key]
        {
            get
            {
                if (key.Value < ObjList.Count) return ObjList[key];
                else throw new IndexOutOfRangeException($"Индекс ({key}) вышел за рамки");
            }
        }

        /// <summary>
        /// Добавить ярлык команды
        /// </summary>
        /// <param name="Element">Добавляемый элемент ярлыка команды</param>
        public void Add(T Element)
        {
            ObjList.Add(Element);
            EventAddLabelAction?.Invoke(Element);
        }

        /// <summary>
        /// Удалить ярлык команды
        /// </summary>
        /// <param name="Index">Индекс удаляемого элемента ярлыка</param>
        public void Remove(int Index)
        {
            ObjList.RemoveAt(Index);
            EventRemoveLabelAction?.Invoke(Index);
        }

        /// <summary>
        /// Удалить все ярлыки команды
        /// </summary>
        public void Clear()
        {
            ObjList.Clear();
            EventClearLabelAction?.Invoke();
        }
    } 
}
