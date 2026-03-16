using ApplicationOperPageLes.CORE.Network;
using ApplicationOperPageLes.CORE.Struct;
using ApplicationOperPageLes.UI.Windows.Base;
using IEL.UserElementsControl;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media;
using ApplicationOperPageLes.UI.Pages.Browser.BrowserPageNetwork;
using OPRES = ApplicationOperPageLes.Properties.Resources;
using WnColor = System.Windows.Media.Color;

namespace ApplicationOperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для DialogCreateTcpListener.xaml
    /// </summary>
    public partial class DialogConnect : OPLWindowBase
    {
        public DialogConnect()
        {
            InitializeComponent();
            Title = "Подключение по IP";
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Wifi));
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
        /// Произвести попытку подключения по индивидуальным настройкам
        /// </summary>
        /// <returns></returns>
        internal TcpClientChat? ConnectClient()
        {
            TcpClientChat? Result = null;
            IELComplete.Text = "Подключиться";
            IELTextBoxPort.Text = "[PORT]";
            IELTextBoxPort.IsEnabled = false;
            IELComplete.OnActivateMouseLeft += async (sender, e) =>
            {
                IELComplete.IsEnabled = false;
                IELTextBoxIP.IsEnabled = false;
                IELButtonCancel.IsEnabled = false;
                try
                {
                    if (IELTextBoxPort.Text.Length > 0)
                    {
                        Result = new();
                        TextBlockIndicatorConnect.Text = "Идёт попытка подключения...";
                        if (ManagerAnimation != null)
                            ManagerAnimation.DoubleAnimationType.AnimateEffect(TextBlockIndicatorConnect, OpacityProperty,
                                1d, TimeSpan.FromMilliseconds(400d));
                        else
                            TextBlockIndicatorConnect.Opacity = 1d;
                        await Result.ConnectAsync(IELTextBoxIP.Text);
                        Close();
                    }
                }
                catch
                {
                    Result = null;
                    TextBlockIndicatorConnect.Text = "Не удалось подключиться...";
                    IELComplete.IsEnabled = true;
                    IELTextBoxIP.IsEnabled = true;
                    IELButtonCancel.IsEnabled = true;
                    IELTextBoxIP.SourceBackground.SetActiveSpecrum(Colors.IndianRed);
                }
                return;
            };
            ShowDialog();
            return Result;
        }
    }
}
