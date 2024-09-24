using AAC20.Classes.Flaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAC20.Classes
{
    public class ThreadGenericWhileProcess
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

        public ThreadGenericWhileProcess(Action ActionProcess, int MillisecondsSleep)
        {
            ParamManageThread = false;
            FlagElement = new(false);
            ThreadInternetCheckConnection = new(delegate ()
            {
                while (ParamManageThread)
                {
                    ActionProcess.Invoke();
                    Thread.Sleep(MillisecondsSleep);
                }
            });
        }

        /// <summary>
        /// Вызвать выполнение потока данных
        /// </summary>
        public void Start()
        {
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
            ThreadInternetCheckConnection.Join();
        }
    }
}
