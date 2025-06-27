using OperPage_les.CORE;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace OperPage_les.UI.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для WindowSaveWait.xaml
    /// </summary>
    public partial class WindowSaveWait : Window
    {
        private UpdateBackgroundData UpdateCheckCompleteSave;
        private bool Compliting = false;
        private int Count = 0;
        public WindowSaveWait()
        {
            InitializeComponent();
            UpdateCheckCompleteSave = new((sender, e) => Dispatcher.Invoke(delegate { }));
            ImageBehavior.SetAnimatedSource(ImageIndicator, new BitmapImage(new Uri(App.DirectoryImageLoading)));
        }

        /// <summary>
        /// Добавить в браузер страниц новую вкладку и активировать страницу
        /// </summary>
        /// <param name="BrowserPage">Браузер для взаимодействия</param>
        /// <returns>Успешно или нет</returns>
        internal void OpenOnToComplete()
        {
            UpdateCheckCompleteSave = new(1000d, (sender, e) => Dispatcher.BeginInvoke(() =>
            {
                TextBlockTime.Text = $"{++Count}";
                if (Compliting)
                {
                    Close();
                    UpdateCheckCompleteSave.Stop();
                }
            }));
            TextBlockTime.Text = "0";
            Opacity = 0d;
            DoubleAnimation animation = App.GetDoubleAnimate();
            animation.BeginTime = TimeSpan.FromMilliseconds(20d);
            animation.Duration = TimeSpan.FromMilliseconds(1270d);
            animation.From = 0d;
            animation.To = 1d;
            BeginAnimation(OpacityProperty, animation);
            UpdateCheckCompleteSave.Start();
            Show();
        }

        /// <summary>
        /// Отобразить текст что конкретно происходит
        /// </summary>
        /// <param name="Text">Отображаемый текст</param>
        internal void SetVisualTextSaving(string Text)
        {
            TextBlockInfoSaving.Text = Text;
        }

        /// <summary>
        /// Завершить загрузку
        /// </summary>
        internal void Complete()
        {
            Compliting = true;
        }
    }
}
