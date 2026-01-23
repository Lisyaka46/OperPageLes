using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Point = System.Windows.Point;

namespace ApplicationOperPageLes.CORE.Animation
{
    internal class OPLPointAnimationType<T> : OPLAnimationTypeBase<T> where T : PointAnimation
    {
        /// <summary>
        /// Объект анимации Rect
        /// </summary>
        public override T SourceAnimation { get; protected set; }

        internal OPLPointAnimationType(T SourceAnimation)
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
        internal void AnimateEffect(IAnimatable Element, DependencyProperty Property, Point From, Point To, TimeSpan Duration)
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
        internal void AnimateEffect(IAnimatable Element, DependencyProperty Property, Point To, TimeSpan Duration)
        {
            SourceAnimation.Duration = Duration;
            SourceAnimation.From = (Point?)(Element as DependencyObject)?.GetValue(Property) ??
                throw new Exception("Не удалось распознать стартовую точку анимации");
            SourceAnimation.To = To;
            Element.BeginAnimation(Property, SourceAnimation, HandoffBehavior.SnapshotAndReplace);
        }
    }
}
