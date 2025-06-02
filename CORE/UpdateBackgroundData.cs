using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OperPage_les.CORE
{
    internal sealed class UpdateBackgroundData
    {
        /// <summary>
        /// Объект управляющий фоновым обновлением визуальной информации
        /// </summary>
        private readonly System.Timers.Timer TimerDataUpdate;

        private ElapsedEventHandler _EventElapsed;
        /// <summary>
        /// Объект выполняемого события
        /// </summary>
        public ElapsedEventHandler EventElapsed
        {
            get => _EventElapsed;
            set
            {
                TimerDataUpdate.Elapsed -= _EventElapsed;
                _EventElapsed = value;
                TimerDataUpdate.Elapsed += value;
            }
        }

        /// <summary>
        /// Инициализировать объект управления <b>ПОВТОРЯЮЩИМСЯ</b> фоновым обновлением информации
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
            _EventElapsed = Elapsed;
            TimerDataUpdate.Elapsed += _EventElapsed;
        }

        /// <summary>
        /// Инициализировать объект управления <b>НЕ ПОВТОРЯЮЩИМСЯ</b> фоновым обновлением информации
        /// </summary>
        /// <param name="Elapsed">Объект действия при выполнении</param>
        public UpdateBackgroundData(ElapsedEventHandler Elapsed)
        {
            TimerDataUpdate = new()
            {
                Enabled = true,
                AutoReset = false,
            };
            _EventElapsed = Elapsed;
            TimerDataUpdate.Elapsed += _EventElapsed;
        }

        /// <summary>
        /// Запустить таймер обновления фоновых данных
        /// </summary>
        public void Start() => TimerDataUpdate.Start();

        /// <summary>
        /// Остановить таймер обновления фоновых данных
        /// </summary>
        public void Stop() => TimerDataUpdate.Stop();
    }
}
