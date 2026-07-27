using OperPageLes.CORE.Enums.Theme;
using OperPageLes.CORE.Network;
using OperPageLes.CORE.Struct;
using OPLAPI.CORE.Animation;
using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.UserElementsControl.Network;
using System.Windows;
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
            IELTextBoxMessage.Text = SourceChat.EnteringMessage;
            IELScrollHistoryMessage.Content = SourceChat.HistoryMessages;
            IELScrollViewerClipFiles.Content = SourceChat.ClipFiles;
            IELScrollHistoryMessage.ScrollToVerticalOffset(SourceChat.SaveScrollValue);
            IELTextBoxMessage.Focus();
        }

        /// <summary>
        /// Очистить выделение объекта чата
        /// </summary>
        internal void SelectChatClear()
        {
            SourceChat?.EnteringMessage = IELTextBoxMessage.Text;
            IELTextBoxMessage.Text = string.Empty;
            SourceChat?.SaveScrollValue = IELScrollHistoryMessage.ActualVerticalOffset;
        }

        /// <summary>
        /// Прикрепить файлы к сообщению
        /// </summary>
        /// <param name="Pathes">Массив директорий прикрепляемых файлов</param>
        private void AddClipFiles(string[] Pathes)
        {
            if (SourceChat == null) return;
            ClipPathFiles.AddRange(Pathes);
            OPLVisualNetworkClipFile ClipElement;
            foreach (string Path in Pathes)
            {
                ClipElement = new()
                {
                    TextFileName = System.IO.Path.GetFileName(Path),
                    CornerRadius = new(5),
                    Margin = new(5),
                    PaletteElement = App.CurrentApp.ActiveThemeApplication[PaletteEnum.Tangerine],
                    ManagerAnimation = ManagerAnimation,
                };
                ClipElement.MathSizeFile(Path);
                ClipElement.SetExtractAssociatedIcon(Path, StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication)));
                ClipElement.UnClipElement += (sender, e) =>
                {
                    ClipPathFiles.Remove(Path);
                    SourceChat.ClipFiles.Children.Remove(ClipElement);
                    if (SourceChat.ClipFiles.Children.Count == 0)
                        OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderClip, HeightProperty,
                            0d, TimeSpan.FromMilliseconds(400d));
                };

                SourceChat.ClipFiles.Children.Add(ClipElement);
                ClipElement.NumberIndex = (uint)SourceChat.ClipFiles.Children.Count;
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
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, BorderClip, HeightProperty,
                    0d, TimeSpan.FromMilliseconds(400d));
                string Message = IELTextBoxMessage.Text;
                string[] PathFiles = [..ClipPathFiles];
                IELTextBoxMessage.Text = string.Empty;
                ClipPathFiles.Clear();

                SourceChat.SendNetworkData(Message, PathFiles);
            }
        }
    }
}
