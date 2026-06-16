using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Network;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.UserElementsControl.Network;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using OPLAPI.OIEL.CORE.Browser;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Key = System.Windows.Input.Key;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Pages.Browser.BrowserPageNetwork
{
    /// <summary>
    /// Логика взаимодействия для PageNetworkChat.xaml
    /// </summary>
    public partial class PageNetworkChat : PageBrowser
    {
        /// <summary>
        /// Объект анимации линии загрузки
        /// </summary>
        private readonly DoubleAnimation AnimationDoubleLine = new()
        {
            Duration = TimeSpan.FromSeconds(5d),
            EasingFunction = null,
            RepeatBehavior = RepeatBehavior.Forever,
            From = 0d,
            To = 40d,
        };

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public override OPLAnimationManager? ManagerAnimation
        {
            get => base.ManagerAnimation;
            set
            {
                base.ManagerAnimation = value;
            }
        }

        /// <summary>
        /// Объект отображаемый историю сообщений
        /// </summary>
        private Chat? SourceChat = null;

        /// <summary>
        /// Директории файлов прикреплённых к сообщению
        /// </summary>
        private List<string> ClipPathFiles = [];

        public PageNetworkChat()
        {
            InitializeComponent();
            LineTextConnection.Opacity = 0d;
            BorderDropFile.Visibility = Visibility.Hidden;

            IELButtonClip.OnActivateMouseLeft += (sender, e) =>
            {
                if (SourceChat == null)
                    throw new Exception();
                Microsoft.Win32.OpenFileDialog Dialog = new()
                {
                    Multiselect = true,
                };
                Dialog.FileOk += (sender, e) =>
                {
                    AddClipFiles(Dialog.FileNames);
                };
                Dialog.ShowDialog();
            };

            IELTextBoxMessage.KeyUp += async (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        Keyboard.ClearFocus();
                        await GoSendData();
                        IELTextBoxMessage.Focus();
                        break;
                }
            };

            DragEnter += (sender, e) =>
            {
                OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, BorderDropFile, OpacityProperty,
                    0d, 1d, TimeSpan.FromMilliseconds(500d));
                BorderDropFile.Visibility = Visibility.Visible;
            };
            Drop += (sender, e) =>
            {
                BorderDropFile.Visibility = Visibility.Hidden;
                if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                {
                    string[] Pathes = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                    AddClipFiles(Pathes);
                }
            };
            DragLeave += (sender, e) =>
            {
                BorderDropFile.Visibility = Visibility.Hidden;
            };

            IELButtonGoSend.OnActivateMouseLeft += async (sender, e) =>
            {
                await GoSendData();
            };
        }

        /// <summary>
        /// Выделить объект чата
        /// </summary>
        /// <param name="SourceChat">Открываемый объект чата</param>
        /// <param name="Client">Клиент который соединён с чатом</param>
        internal void SelectChat(ref Chat SourceChat)
        {
            this.SourceChat = SourceChat;
            this.SourceChat.IsBusyChanged += ChatBusyChanged;
            IELTextBoxMessage.Text = SourceChat.EnteringMessage;
            IELScrollHistoryMessage.Content = SourceChat.HistoryMessages;
            IELScrollViewerClipFiles.Content = SourceChat.ClipFiles;
            IELScrollHistoryMessage.ScrollToVerticalOffset(SourceChat.SaveScrollValue);
            if (SourceChat.IsBusy)
                BusyActivateVisual();
            IELTextBoxMessage.Focus();
        }

        /// <summary>
        /// Очистить выделение объекта чата
        /// </summary>
        internal void SelectChatClear()
        {
            SourceChat?.IsBusyChanged -= ChatBusyChanged;
            SourceChat?.EnteringMessage = IELTextBoxMessage.Text;
            IELTextBoxMessage.Text = string.Empty;
            SourceChat?.SaveScrollValue = IELScrollHistoryMessage.ActualVerticalOffset;
            if (SourceChat?.IsBusy ?? false)
                BusyDiactivateVisual();
        }

        /// <summary>
        /// Прикрепить файлы к сообщению
        /// </summary>
        /// <param name="Pathes">Массив директорий прикрепляемых файлов</param>
        private void AddClipFiles(string[] Pathes)
        {
            if (SourceChat == null) return;
            ClipPathFiles.AddRange(Pathes);
            OPLNetworkClipElement ClipElement;
            foreach (string Path in Pathes)
            {
                ClipElement = new()
                {
                    TextFileName = System.IO.Path.GetFileName(Path),
                    CornerRadius = new(5),
                    Margin = new(5),
                    //ManagerAnimation = App.CurrentApp.ManagerAnimation,
                };
                ClipElement.MathSizeFile(Path);
                ClipElement.SetExtractAssociatedIcon(Path, StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication)));

                App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Tangerine].ConnectPalleteFromIELElement(ClipElement);
                SourceChat.ClipFiles.Children.Add(ClipElement);
                ClipElement.SetIndex((uint)SourceChat.ClipFiles.Children.Count);
                if (BorderClip.Height == 0d)
                    OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderClip, HeightProperty,
                        100d, TimeSpan.FromMilliseconds(400d));
            }
        }

        /// <summary>
        /// Отправить данные независимо от их содержания
        /// </summary>
        /// <returns></returns>
        private async Task GoSendData()
        {
            if ((IELTextBoxMessage.Text.Length == 0 && ClipPathFiles.Count == 0) || SourceChat == null) return;
            else
            {
                BusyActivateVisual();

                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderClip, HeightProperty,
                    0d, TimeSpan.FromMilliseconds(400d));
                string Message = IELTextBoxMessage.Text;
                string[] PathFiles = [..ClipPathFiles];
                IELTextBoxMessage.Text = string.Empty;
                ClipPathFiles.Clear();

                SourceChat.SendNetworkData(Message, PathFiles);

                BusyDiactivateVisual();
            }
        }

        /// <summary>
        /// Функция события изменения занятости
        /// </summary>
        /// <param name="NewValue">Новое значение занятости чата</param>
        private void ChatBusyChanged(bool NewValue)
        {
            if (NewValue) BusyActivateVisual();
            else BusyDiactivateVisual();
        }

        /// <summary>
        /// Активировать визуализацию занятости чата
        /// </summary>
        private void BusyActivateVisual()
        {
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, LineTextConnection,
                    HeightProperty, 0d, 10d, TimeSpan.FromMilliseconds(500d));
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, LineTextConnection,
                OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
            LineTextConnection.BeginAnimation(Line.StrokeDashOffsetProperty, AnimationDoubleLine);

            IELButtonGoSend.IsEnabled = false;
            IELButtonClip.IsEnabled = false;
            IELTextBoxMessage.IsEnabled = false;
        }

        /// <summary>
        /// Активировать визуализацию занятости чата
        /// </summary>
        private void BusyDiactivateVisual()
        {
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, LineTextConnection, OpacityProperty,
                0d, TimeSpan.FromMilliseconds(500d));
            LineTextConnection.BeginAnimation(Line.StrokeDashOffsetProperty, null);
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, LineTextConnection, Line.StrokeDashOffsetProperty,
                LineTextConnection.StrokeDashOffset, LineTextConnection.StrokeDashOffset - 5d, TimeSpan.FromMilliseconds(500d));

            IELButtonGoSend.IsEnabled = true;
            IELButtonClip.IsEnabled = true;
            IELTextBoxMessage.IsEnabled = true;
            IELTextBoxMessage.Text = string.Empty;
        }
    }
}
