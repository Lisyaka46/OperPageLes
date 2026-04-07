using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace OperPageLes.CORE.Animation
{
    /// <summary>
    /// Базовый класс анимационного элемента программы
    /// </summary>
    /// <typeparam name="T">Тип анимируемого свойства</typeparam>
    internal abstract class OPLAnimationTypeBase<T> where T : AnimationTimeline
    {
        /// <summary>
        /// Объект Анимации
        /// </summary>
        public abstract T SourceAnimation { get; protected set; }
    }
}
