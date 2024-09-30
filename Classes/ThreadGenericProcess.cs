using AAC20.Classes.Flaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAC20.Classes
{
    public class ThreadGenericProcess
    {
        /// <summary>
        /// Поток обновляемый данные
        /// </summary>
        private readonly Thread ThreadInternetCheckConnection;

        /// <summary>
        /// Флаг подключённый к потоку данных
        /// </summary>
        private readonly Flag FlagElement;

        /// <summary>
        /// Узнать состояние флага
        /// </summary>
        public bool StateFlag => FlagElement;

        /// <summary>
        /// Параметр управляемый потоком
        /// </summary>
        private volatile bool ParamManageThread;

        /// <summary>
        /// Инициализировать <b>ПОВТОРЯЮЩИЙСЯ</b> поток
        /// </summary>
        /// <param name="ActionProcess">Действие которое выполняется в новом потоке</param>
        /// <param name="MillisecondsSleep">Тайм-аут после каждого завершения действия</param>
        public ThreadGenericProcess(Action ActionProcess, uint MillisecondsSleep)
        {
            ParamManageThread = false;
            FlagElement = new(false);
            ThreadInternetCheckConnection = new(delegate ()
            {
                while (ParamManageThread)
                {
                    ActionProcess.Invoke();
                    Thread.Sleep((int)MillisecondsSleep);
                }
            });
        }

        /// <summary>
        /// Инициализировать <b>НЕ ПОВТОРЯЮЩИЙСЯ</b> поток
        /// </summary>
        /// <param name="ActionProcess">Действие которое выполняется в новом потоке</param>
        public ThreadGenericProcess(Action ActionProcess)
        {
            ParamManageThread = false;
            FlagElement = new(false);
            ThreadInternetCheckConnection = new(delegate ()
            {
                ActionProcess.Invoke();
            });
        }

        /// <summary>
        /// Вызвать выполнение потока данных
        /// </summary>
        public void Start()
        {
            if (ParamManageThread) return;
            ParamManageThread = true;
            FlagElement.Value = true;
            ThreadInternetCheckConnection.Start();
        }

        /// <summary>
        /// Убить поток данных
        /// </summary>
        public void Kill()
        {
            Paused();
            ThreadInternetCheckConnection.Join();
        }

        /// <summary>
        /// Остановить поток данных
        /// </summary>
        public void Paused()
        {
            ParamManageThread = false;
            FlagElement.Value = false;
        }
    }
}
