using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE
{
    /// <summary>
    /// Инициализация события сохранения данных
    /// </summary>
    /// <param name="SourceName">Название события</param>
    /// <param name="SourceSleepMilliseconds">Время ожидания в миллисекундах после сохранения</param>
    internal class ActionManipulateData(string SourceName, double SourceSleepMilliseconds)
    {
        /// <summary>
        /// Название события
        /// </summary>
        internal readonly string Name = SourceName;

        /// <summary>
        /// Время ожидания в миллисекундах после сохранения
        /// </summary>
        internal readonly double SleepMilliseconds = SourceSleepMilliseconds;

        /// <summary>
        /// Действие сохранения данных
        /// </summary>
        internal Action? OriginAction = null;

        /// <summary>
        /// Установить действие сохранения данных
        /// </summary>
        /// <param name="SourceAction">Действие сохранения данных</param>
        internal void SetActionSave(in Action SourceAction) => OriginAction = SourceAction;

        /// <summary>
        /// Активировать событие сохранения данных
        /// </summary>
        internal async Task InvokeActionSave()
        {
            OriginAction?.Invoke();
            await Task.Delay((int)SleepMilliseconds);
        }
    }
}
