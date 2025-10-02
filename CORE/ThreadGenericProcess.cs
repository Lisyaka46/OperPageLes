namespace OperPageLes.CORE
{
    public class ThreadGenericProcess
    {
        /// <summary>
        /// Поток обновляемый данные
        /// </summary>
        private Thread? ThreadUpdating;

        /// <summary>
        /// Параметр управляемый потоком
        /// </summary>
        private volatile bool ParamManageThread;

        /// <summary>
        /// Действие которое выполняет поток
        /// </summary>
        private Action ThreadAction;

        /// <summary>
        /// Цикличный поток или нет
        /// </summary>
        private readonly bool While;

        /// <summary>
        /// Количество миллисекунд ожидания
        /// </summary>
        private readonly int MillisecondSleep;

        /// <summary>
        /// Инициализировать <b>ПОВТОРЯЮЩИЙСЯ</b> поток
        /// </summary>
        /// <param name="ActionProcess">Действие которое выполняется в новом потоке</param>
        /// <param name="MillisecondsSleep">Тайм-аут после каждого завершения действия</param>
        public ThreadGenericProcess(Action ActionProcess, uint MillisecondsSleep)
        {
            ParamManageThread = false;
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
                ThreadUpdating = new(delegate ()
                {
                    while (ParamManageThread)
                    {
                        ThreadAction.Invoke();
                        Thread.Sleep(MillisecondSleep);
                    }
                })
                {
                    IsBackground = true
                };
            }
            else
            {
                ThreadUpdating = new(delegate ()
                {
                    ThreadAction.Invoke();
                })
                {
                    IsBackground = true
                };
            }
            ParamManageThread = true;
            ThreadUpdating.Start();
        }

        /// <summary>
        /// Убить поток данных
        /// </summary>
        public void Kill()
        {
            ParamManageThread = false;
            ThreadUpdating?.Join();
        }

        /// <summary>
        /// Подключить новое действие в поток
        /// </summary>
        /// <param name="ActionProcess">Действие подключаемое в поток</param>
        /// <exception cref="Exception">Исключение включнного потока данных</exception>
        public void SetAction(Action ActionProcess)
        {
            if (ParamManageThread) throw new Exception("Невозможно подключить действие в выполняющимся потоке.");
            ThreadAction = ActionProcess;
        }
    }
}
