using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.Windows.Base;
using IEL.UserElementsControl;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media;
using OPRES = ApplicationOperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;

namespace ApplicationOperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для DialogCreateTcpListener.xaml
    /// </summary>
    public partial class DialogCreateTcpListener : OPLWindowBase
    {
        public DialogCreateTcpListener()
        {
            InitializeComponent();
            IELTextBoxIP.Text = string.Empty;
            IELTextBoxPort.Text = string.Empty;
            TextBlockIndicatorConnect.Opacity = 0d;
            IELButtonCancel.Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Cross));
            IELButtonCancel.OnActivateMouseLeft += (sender, e) =>
            {
                Close();
            };
        }

        /// <summary>
        /// Открыть сервер по индивидуальным настройкам
        /// </summary>
        /// <returns></returns>
        internal TcpListener? OpenTcpListener()
        {
            TcpListener? Result = null;
            IELTextBoxIP.Text = "[IP]";
            IELTextBoxIP.IsEnabled = false;
            IELTextBoxPort.Text = new Random().Next(1024, 49151).ToString();
            IELComplete.OnActivateMouseLeft += (sender, e) =>
            {
                try
                {
                    if (IELTextBoxPort.Text.Length > 0)
                    {
                        int port = int.Parse(IELTextBoxPort.Text);
                        if (port >= 1024 && port <= 49151)
                        {
                            Result = new(IPAddress.Any, port);
                            Close();
                        }
                    }
                } catch { }
                IELTextBoxPort.SourceBackground.SetActiveSpecrum(Colors.IndianRed);
            };
            ShowDialog();
            return Result;
        }

        /// <summary>
        /// Произвести попытку подключения по индивидуальным настройкам
        /// </summary>
        /// <returns></returns>
        internal TcpClient? OpenTcpClient()
        {
            TcpClient? Result = null;
            IELComplete.Text = "Подключиться";
            IELComplete.OnActivateMouseLeft += async (sender, e) =>
            {
                IELComplete.IsEnabled = false;
                IELTextBoxIP.IsEnabled = false;
                IELTextBoxPort.IsEnabled = false;
                IELButtonCancel.IsEnabled = false;
                try
                {
                    if (IELTextBoxPort.Text.Length > 0)
                    {
                        int port = int.Parse(IELTextBoxPort.Text);
                        if (port >= 1024 && port <= 49151)
                        {
                            Result = new();
                            TextBlockIndicatorConnect.Text = "Идёт попытка подключения...";
                            if (ManagerAnimation != null)
                                ManagerAnimation.DoubleAnimationType.AnimateEffect(TextBlockIndicatorConnect, OpacityProperty,
                                    1d, TimeSpan.FromMilliseconds(400d));
                            else
                                TextBlockIndicatorConnect.Opacity = 1d;
                            await Result.ConnectAsync(IELTextBoxIP.Text, port);
                            Close();
                            return;
                        }
                    }
                }
                catch { }
                TextBlockIndicatorConnect.Text = "Не удалось подключиться...";
                IELComplete.IsEnabled = true;
                IELTextBoxIP.IsEnabled = true;
                IELTextBoxPort.IsEnabled = true;
                IELButtonCancel.IsEnabled = true;
                IELTextBoxIP.SourceBackground.SetActiveSpecrum(Colors.IndianRed);
                IELTextBoxPort.SourceBackground.SetActiveSpecrum(Colors.IndianRed);
            };
            ShowDialog();
            return Result;
        }
    }
}
