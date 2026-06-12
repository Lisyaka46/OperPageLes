using OperPageLes.CORE.Enums;
using OperPageLes.CORE.Network;
using OperPageLes.CORE.Struct;
using OperPageLes.UI.UserElementsControl.Network;
using OperPageLes.UI.Windows.Dialogs;
using IEL.UserElementsControl.Base;
using Microsoft.Windows.Themes;
using OPLAPI.OIEL.CORE.Browser;
using OPLAPI.OIEL.UserElementsControl;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MouseButton = System.Windows.Input.MouseButton;
using OPRES = OperPageLes.Properties.Resources;

namespace OperPageLes.UI.Pages.Browser.BrowserPageNetwork
{
    /// <summary>
    /// Логика взаимодействия для PageNetwork.xaml
    /// </summary>
    public partial class PageNetwork : PageBrowser, IDisposable
    {
        /// <summary>
        /// Страница отображения всех чатов
        /// </summary>
        private static PageStackChats SourcePageStackChats = new();

        /// <summary>
        /// Страница отображения всех чатов
        /// </summary>
        private static PageNetworkChat SourcePageNetworkChat = new();

        public PageNetwork()
        {
            InitializeComponent();
            SourcePageStackChats.IELButtonNewChat.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.NewChat));
            SourcePageNetworkChat.IELButtonClip.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.PaperClip));
            IELButtonChats.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Chats));
            IELButtonFolderDownloads.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Download));
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Chats));

            SourcePageStackChats.SelectChat += SelectChat;

            IELButtonChats.OnActivateMouseLeft += (sender, e) =>
            {
                BackChatList();
            };

            IELButtonFolderDownloads.OnActivateMouseLeft += (sender, e) =>
            {
                Process p = new();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/c start {StructDirectoryResources.DirectoryDownloadApplication}";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            };

            MouseUp += (sender, e) =>
            {
                switch (e.ChangedButton)
                {
                    case MouseButton.XButton1:
                        if (IELButtonChats.IsEnabled)
                            BackChatList();
                        break;
                }
            };

            PageControllerNetwork.NextPage(SourcePageStackChats);
            IELButtonChats.IsEnabled = false;
            if (!PageStackChats.IsActiveListener)
                SourcePageStackChats.OpenListener();

            Disposed += (sender) =>
            {

            };
        }

        /// <summary>
        /// Открыть полный список всех чатов
        /// </summary>
        private void BackChatList()
        {
            PageControllerNetwork.NextPage(SourcePageStackChats, false);
            SourcePageNetworkChat.SelectChatClear();
            IELButtonChats.IsEnabled = false;
        }

        /// <summary>
        /// Активировать чат
        /// </summary>
        /// <param name="chat">Чат</param>
        private void SelectChat(ref Chat chat)
        {
            PageControllerNetwork.NextPage(SourcePageNetworkChat);
            SourcePageNetworkChat.SelectChat(ref chat);
            IELButtonChats.IsEnabled = true;
        }

        /// <summary>
        /// Освободить/Закрыть ресурсы объекта
        /// </summary>
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
