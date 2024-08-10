using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AAC20.Classes
{
    internal sealed class UpdateBackgroundData
    {
        /// <summary>
        /// Объект управляющий фоновым обновлением визуальной информации
        /// </summary>
        public readonly System.Timers.Timer TimerDataUpdate;

        /// <summary>
        /// Объект выполняемого события
        /// </summary>
        private readonly ElapsedEventHandler EventElapsed;

        /// <summary>
        /// Инициализировать объект управления фоновым обновлением информации в данном окне
        /// </summary>
        /// <param name="IntervalUpdate">Промежуток времени через который будет обновляться объект</param>
        /// <param name="Elapsed">Объект действия при выполнении</param>
        public UpdateBackgroundData(double IntervalUpdate, ElapsedEventHandler Elapsed)
        {
            TimerDataUpdate = new()
            {
                Enabled = true,
                AutoReset = true,
                Interval = IntervalUpdate
            };
            EventElapsed = Elapsed;
            TimerDataUpdate.Elapsed += EventElapsed;
        }
    }
}
