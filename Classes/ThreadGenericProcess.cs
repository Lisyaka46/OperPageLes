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
        private Thread? ThreadInternetCheckConnection;

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
        /// Действие которое выполняет поток
        /// </summary>
        private readonly Action ThreadAction;

        //
        private readonly bool While;

        //
        private readonly int MillisecondSleep;

        /// <summary>
        /// Инициализировать <b>ПОВТОРЯЮЩИЙСЯ</b> поток
        /// </summary>
        /// <param name="ActionProcess">Действие которое выполняется в новом потоке</param>
        /// <param name="MillisecondsSleep">Тайм-аут после каждого завершения действия</param>
        public ThreadGenericProcess(Action ActionProcess, uint MillisecondsSleep)
        {
            ParamManageThread = false;
            FlagElement = new(false);
            ThreadAction = ActionProcess;
            While = true;
            MillisecondSleep = (int)MillisecondsSleep;
        }

        /// <summary>
        /// Инициализировать <b>НЕ ПОВТОРЯЮЩИЙСЯ</b> поток
        /// </summary>
        /// <param name="ActionProcess">Действие которое выполняется в новом потоке</param>
        public ThreadGenericProcess(Action ActionProcess)
        {
            ParamManageThread = false;
            FlagElement = new(false);
            ThreadAction = ActionProcess;
            While = false;
            MillisecondSleep = 0;
        }

        /// <summary>
        /// Вызвать выполнение потока данных
        /// </summary>
        public void Start()
        {
            if (ParamManageThread) return;
            if (While)
            {
                ThreadInternetCheckConnection = new(delegate ()
                {
                    while (ParamManageThread)
                    {
                        ThreadAction.Invoke();
                        Thread.Sleep(MillisecondSleep);
                    }
                });
            }
            else
            {
                ThreadInternetCheckConnection = new(delegate ()
                {
                    ThreadAction.Invoke();
                });
            }
            ParamManageThread = true;
            FlagElement.Value = true;
            ThreadInternetCheckConnection.Start();
        }

        /// <summary>
        /// Убить поток данных
        /// </summary>
        public void Kill()
        {
            ParamManageThread = false;
            FlagElement.Value = false;
            ThreadInternetCheckConnection?.Join();
        }
    }
}
