using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;

namespace OperPageLes.CORE.Animation
{
    internal class OPLColorAnimationType<T> : OPLAnimationTypeBase<T> where T : ColorAnimation
    {
        /// <summary>
        /// Объект анимации Color
        /// </summary>
        public override T SourceAnimation { get; protected set; }

        internal OPLColorAnimationType(T SourceAnimation)
        {
            this.SourceAnimation = SourceAnimation;
        }

        /// <summary>
        /// Анимировать эффект объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="From">Значение от которого начинается анимация</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal void AnimateEffect(IAnimatable Element, DependencyProperty Property, Color From, Color To, TimeSpan Duration)
        {
            SourceAnimation.Duration = Duration;
            SourceAnimation.From = From;
            SourceAnimation.To = To;
            Element.BeginAnimation(Property, SourceAnimation, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// Анимировать эффект объекта
        /// </summary>
        /// <param name="Element">Объект анимации</param>
        /// <param name="Property">Анимируемое свойство</param>
        /// <param name="To">Значение к которому стремится анимация</param>
        /// <param name="Duration">Количество миллисекунд для анимации</param>
        internal void AnimateEffect(IAnimatable Element, DependencyProperty Property, Color To, TimeSpan Duration)
        {
            SourceAnimation.Duration = Duration;
            SourceAnimation.From = (Color?)(Element as DependencyObject)?.GetValue(Property) ??
                throw new Exception("Не удалось распознать стартовую точку анимации");
            SourceAnimation.To = To;
            Element.BeginAnimation(Property, SourceAnimation, HandoffBehavior.SnapshotAndReplace);
        }
    }
}
