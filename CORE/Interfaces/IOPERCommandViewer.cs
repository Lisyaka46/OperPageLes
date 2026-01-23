using InterpreterCommand.Interfaices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ApplicationOperPageLes.CORE.Interfaces
{
    /// <summary>
    /// Интерфейс объекта визуализации вывода команды в системе
    /// </summary>
    public interface IOPERCommandViewer : ICommandViewer
    {
        /// <summary>
        /// Добавить новый <b>форматированный</b> текст
        /// </summary>
        /// <param name="Source">Добавляемый текст</param>
        public void AddFormattedString(string Source);

        /// <summary>
        /// Добавить новый элемент управления в объект визуализации команды
        /// </summary>
        /// <param name="Source">Добавляемый элемент</param>
        public void AddNewUIElement(UIElement Source);

        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки без ожидаемого значения
        /// </summary>
        /// <param name="Method">Исполняемый асинхронный процесс</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        public Task ExecuteVisualizateTask(Task Method);

        /// <summary>
        /// Осуществить выполнение процесса через визуализацию асинхронной загрузки с ожидаемым значением
        /// </summary>
        /// <param name="Method">Исполняемый асинхронный процесс</param>
        /// <returns>Исполненный асинхронный процесс</returns>
        public Task<T> ExecuteVisualizateTask<T>(Task<T> Method);
    }
}
