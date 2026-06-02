using OIEL.CORE.Browser;
using OIEL.UserElementsControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperPageLes.CORE.Objects
{
    internal class ApplicationPage
    {
        /// <summary>
        /// Визуальный элемент представления объекта
        /// </summary>
        internal readonly OPLVisualElementIM VisualELement;

        /// <summary>
        /// Тип приложения страницы
        /// </summary>
        public readonly Type TypeBrowserAppPage;

        /// <summary>
        /// Имя страничного приложения
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Событие активации страничного приложения
        /// </summary>
        internal event EventHandler<ApplicationPage>? ApplicationPageActivate;

        /// <summary>
        /// Инициализировать объект представления приложения страницы
        /// </summary>
        /// <param name="SourceTypeAppPage">Тип приложения страницы</param>
        /// <param name="NameAppPage">Имя приложения страницы</param>
        /// <exception cref="ArgumentException">Исключение при несоответствии базового типа</exception>
        internal ApplicationPage(Type SourceTypeAppPage, string NameAppPage, System.Windows.Size SourceSize)
        {
            if (SourceTypeAppPage.BaseType != typeof(PageBrowser)) throw new ArgumentException("Недопустимый тип приложения страницы");
            TypeBrowserAppPage = SourceTypeAppPage;
            Name = NameAppPage;
            VisualELement = new()
            {
                ManagerAnimation = App.ManagerAnimation,
                Text = NameAppPage,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Padding = new(0),
            };
            VisualELement.SetSizeIconApp(SourceSize);
            VisualELement.OnActivateMouseLeft += (sender, e) => ApplicationPageActivate?.Invoke((OPLVisualElementIM)sender, this);
        }
    }
}
