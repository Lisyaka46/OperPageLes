using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AAC20.Windows.Pages.License
{
    /// <summary>
    /// Логика взаимодействия для PageUser.xaml
    /// </summary>
    public partial class PageUserThanks : Page
    {
        /// <summary>
        /// Константа времени изчезновения страницы
        /// </summary>
        const double MillisecondsHide = 1250d;

        /// <summary>
        /// Константа времени появления страницы
        /// </summary>
        const double MillisecondsShow = 1200d;

        /// <summary>
        /// Фнимация прозрачности страницы
        /// </summary>
        private readonly DoubleAnimation AnimOpacity = new(0d, TimeSpan.FromMilliseconds(MillisecondsHide))
        {
            EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop,
        };

        /// <summary>
        /// Анимация позиции страницы
        /// </summary>
        private readonly ThicknessAnimation AnimMargin = new(new(0, -14, 0, 0), TimeSpan.FromMilliseconds(MillisecondsHide))
        {
            EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.HoldEnd,
        };

        public PageUserThanks()
        {
            InitializeComponent();
            Opacity = 0d;
        }

        /// <summary>
        /// Переключить страцину на другого пользователя
        /// </summary>
        /// <param name="NickName">Ник</param>
        /// <param name="Phrase">Фраза</param>
        /// <param name="Message">Сообщение</param>
        /// <param name="UriIcon">Путь к иконке</param>
        /// <param name="ColorNickName">Цвет текста ника</param>
        /// <param name="ColorPhrase">Цвет текста фразы</param>
        public void NextUser(string NickName, string Phrase, string Message, Uri? UriIcon, Color ColorNickName, Color ColorPhrase)
        {
            AnimOpacity.Completed += (sender, e) => UpdateInfo(NickName, Phrase, Message, UriIcon, ColorNickName, ColorPhrase);
            BeginAnimation(OpacityProperty, AnimOpacity);
            BeginAnimation(MarginProperty, AnimMargin);
        }

        /// <summary>
        /// Обновить данные
        /// </summary>
        /// <param name="NickName">Ник</param>
        /// <param name="Phrase">Фраза</param>
        /// <param name="Message">Сообщение</param>
        /// <param name="UriIcon">Путь к иконке</param>
        /// <param name="ColorNickName">Цвет текста ника</param>
        /// <param name="ColorPhrase">Цвет текста фразы</param>
        private void UpdateInfo(string NickName, string Phrase, string Message, Uri? UriIcon, Color ColorNickName, Color ColorPhrase)
        {
            Opacity = 0d;
            TextBlockNickName.Foreground = new SolidColorBrush(ColorNickName);
            TextBlockNickName.Text = NickName;
            TextBlockPhrase.Text = Phrase;
            TextBlockPhrase.Foreground = new SolidColorBrush(ColorPhrase);
            TextBlockMessage.Text = Message;
            if (UriIcon != null)
            {
                ImageIconNickName.Source = new BitmapImage(UriIcon);
                ImageIconNickName.Opacity = 1d;
            }
            else ImageIconNickName.Opacity = 0d;
            VisiblePage();
        }

        /// <summary>
        /// Сделать видимым данную страницу
        /// </summary>
        private void VisiblePage()
        {
            ThicknessAnimation AnimMargin = new(new(0), TimeSpan.FromMilliseconds(MillisecondsShow))
            {
                From = new Thickness(0, 14, 0, 0),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.HoldEnd,
            };
            DoubleAnimation AnimOpacity = new(1d, TimeSpan.FromMilliseconds(MillisecondsShow))
            {
                EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.HoldEnd,
            };
            BeginAnimation(OpacityProperty, AnimOpacity);
            BeginAnimation(MarginProperty, AnimMargin);
        }
    }
}
