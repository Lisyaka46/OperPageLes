using OperPage_les.CORE;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace OperPage_les.Windows.Pages.License
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
        private readonly ThicknessAnimation AnimMargin = new(new(0), TimeSpan.FromMilliseconds(MillisecondsHide))
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
        /// <param name="assistent">Данные помошника разработки</param>
        internal void NextUser(AssistentThanks assistent)
        {
            AnimMargin.To = new(0, 14, 0, 0);
            AnimOpacity.Completed += (sender, e) => UpdateInfo(ref assistent);
            BeginAnimation(OpacityProperty, AnimOpacity);
            BeginAnimation(MarginProperty, AnimMargin);
        }

        /// <summary>
        /// Обновить данные
        /// </summary>
        /// <param name="assistent">Объект данных помошника</param>
        private void UpdateInfo(ref AssistentThanks assistent)
        {
            Opacity = 0d;
            TextBlockNickName.Foreground = new SolidColorBrush(assistent.ColorNickName);
            TextBlockNickName.Text = assistent.NickName;
            TextBlockPhrase.Text = $"\"{assistent.Phrase}\"";
            TextBlockPhrase.Foreground = new SolidColorBrush(assistent.ColorPhrase);
            TextBlockMessage.Text = assistent.Message;
            if (assistent.UriImage != null || assistent.MapImage != null)
            {
                BitmapImage? bitmap;
                if (assistent.UriImage != null && App.Flags.InternetPinging) bitmap = new(assistent.UriImage);
                else bitmap = assistent.MapImage;
                ImageIconNickName.Source = bitmap;
                ImageIconNickName.Opacity = 0.4d;
                AnimMargin.To = new(0, -50, 0, 0);
                AnimMargin.Duration = TimeSpan.FromMilliseconds(12000d);
                AnimMargin.EasingFunction = null;
                ImageIconNickName.BeginAnimation(MarginProperty, AnimMargin);
                AnimMargin.Duration = TimeSpan.FromMilliseconds(MillisecondsHide);
                AnimMargin.EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut };
            }
            else
            {
                ImageIconNickName.Opacity = 0d;
                ImageIconNickName.BeginAnimation(MarginProperty, null);
                ImageIconNickName.Margin = new(0);
            }
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
