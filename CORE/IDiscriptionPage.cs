using Interpreter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationOperPageLes.CORE
{
    internal interface IDiscriptionPageEventsHandler
    {
        /// <summary>
        /// Делегат события изменения состояния
        /// </summary>
        /// <typeparam name="T">Тип состояния</typeparam>
        /// <param name="NewValue">Новое значение состояния</param>
        internal delegate void ChangeStateHandler<T>(T NewValue);
    }

    internal interface IDiscriptionPage<T> : IDiscriptionPageEventsHandler
    {
        /// <summary>
        /// Обновить информацию описания об объекте
        /// </summary>
        /// <param name="command">Объект описания</param>
        internal void UpdateInformation(T command);
    }
}
