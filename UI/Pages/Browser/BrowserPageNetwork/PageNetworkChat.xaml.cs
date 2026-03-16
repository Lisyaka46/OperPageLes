using ApplicationOperPageLes.CORE.Enums;
using ApplicationOperPageLes.CORE.Network;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.UserElementsControl.Network;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Key = System.Windows.Input.Key;
using OPRES = ApplicationOperPageLes.Properties.Resources;

namespace ApplicationOperPageLes.UI.Pages.Browser.BrowserPageNetwork
{
    /// <summary>
    /// Логика взаимодействия для PageNetworkChat.xaml
    /// </summary>
    public partial class PageNetworkChat : Page
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
            BorderInfoSendFiles.Margin = new(0, 0, -155, 0);
            LineTextConnection.Opacity = 0d;

            IELButtonClip.OnActivateMouseLeft += (sender, e) =>
            {
                if (SourceChat == null)
                    throw new Exception();
                OPLNetworkClipElement ClipElement;
                Microsoft.Win32.OpenFileDialog Dialog = new()
                {
                    Multiselect = true,
                };
                Dialog.FileOk += (sender, e) =>
                {
                    ClipPathFiles.AddRange(Dialog.FileNames);
                    foreach (string Path in Dialog.FileNames)
                    {
                        ClipElement = new()
                        {
                            Text = System.IO.Path.GetFileName(Path),
                            CornerRadius = new(5),
                            Margin = new(5),
                            ManagerAnimation = App.ManagerAnimation,
                        };
                        ClipElement.MathSizeFile(Path);
                        ClipElement.SetExtractAssociatedIcon(Path, StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication)));

                        App.CurrentApp.ActiveThemeApplication[PaletteSpectrumEnum.Aquamarine].ConnectPalleteFromIELElement(ClipElement);
                        SourceChat.ClipFiles.Children.Add(ClipElement);
                        ClipElement.SetIndex((uint)SourceChat.ClipFiles.Children.Count);
                        if (BorderClip.Height == 0d)
                            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderClip, HeightProperty, 100d, TimeSpan.FromMilliseconds(400d));
                    }
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
            if (SourceChat?.IsBusy ?? false)
                BusyDiactivateVisual();
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

                App.ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderClip, HeightProperty, 0d, TimeSpan.FromMilliseconds(400d));
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
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(LineTextConnection,
                    HeightProperty, 0d, 10d, TimeSpan.FromMilliseconds(500d));
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(LineTextConnection,
                OpacityProperty, 1d, TimeSpan.FromMilliseconds(500d));
            LineTextConnection.BeginAnimation(Line.StrokeDashOffsetProperty, AnimationDoubleLine);
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(BorderInfoSendFiles, MarginProperty, new(0),
                TimeSpan.FromMilliseconds(800d));

            IELButtonGoSend.IsEnabled = false;
            IELButtonClip.IsEnabled = false;
            IELTextBoxMessage.IsEnabled = false;
        }

        /// <summary>
        /// Активировать визуализацию занятости чата
        /// </summary>
        private void BusyDiactivateVisual()
        {
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(LineTextConnection, OpacityProperty, 0d, TimeSpan.FromMilliseconds(500d));
            LineTextConnection.BeginAnimation(Line.StrokeDashOffsetProperty, null);
            App.ManagerAnimation.DoubleAnimationType.AnimateEffect(LineTextConnection, Line.StrokeDashOffsetProperty,
                LineTextConnection.StrokeDashOffset, LineTextConnection.StrokeDashOffset - 5d, TimeSpan.FromMilliseconds(500d));
            App.ManagerAnimation.ThicknessAnimationType.AnimateEffect(BorderInfoSendFiles, MarginProperty, new(0, 0, -155, 0),
                TimeSpan.FromMilliseconds(800d));

            IELButtonGoSend.IsEnabled = true;
            IELButtonClip.IsEnabled = true;
            IELTextBoxMessage.IsEnabled = true;
            IELTextBoxMessage.Text = string.Empty;
        }
    }
}
