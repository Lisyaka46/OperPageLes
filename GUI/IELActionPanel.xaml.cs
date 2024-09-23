using AAC20.Classes;
using AAC20.Classes.Flaging;
using AAC20.Interfaces;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static AAC20.MainWindow;

namespace AAC20.GUI
{
    /// <summary>
    /// Логика взаимодействия для IELActionPanel.xaml
    /// </summary>
    public partial class IELActionPanel : UserControl
    {
        /// <summary>
        /// Флаг состояния активности панели действий
        /// </summary>
        private readonly Flag FlagPanelActionActivate = new(false);

        /// <summary>
        /// Узнать состояние активности панели действий
        /// </summary>
        public bool PanelActionActivate => FlagPanelActionActivate;

        /// <summary>
        /// Флаг состояния режима клавиатуры
        /// </summary>
        private readonly Flag FlagActivateKeyboardMode = new(false);

        /// <summary>
        /// Узнать состояние активности режима клавиатуры
        /// </summary>
        public bool ActivateKeyboardMode => FlagActivateKeyboardMode;

        /// <summary>
        /// Код клавиши активирующий режим клавиатуры в панели действий
        /// </summary>
        public Key KeyActivateKeyboardMode { get; set; }

        /// <summary>
        /// Объект анимации для управления размерами панели действий
        /// </summary>
        private static readonly DoubleAnimation DoubleAnimateObj = new(0, TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        /// <summary>
        /// Объект анимации для управления позицией
        /// </summary>
        private static readonly ThicknessAnimation ThicknessAnimate = new(new Thickness(0), TimeSpan.FromMilliseconds(300d))
        {
            DecelerationRatio = 0.6d,
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
        };

        List<(IPageModuleButtonKeyAAC, string)> BufferPages = [];

        /// <summary>
        /// Объект предыдущей страницы
        /// </summary>
        private IPageModuleButtonKeyAAC RefBackPage => (IPageModuleButtonKeyAAC)BackFrame.Content;

        /// <summary>
        /// Объект актуального окна страницы
        /// </summary>
        private Frame ActualFrame => PanelVerschachtelung % 2 == 0 ? ref FrameActionPanelLeft : ref FrameActionPanelRight;

        /// <summary>
        /// Объект предыдущего окна страницы
        /// </summary>
        private Frame BackFrame => !(PanelVerschachtelung % 2 == 0) ? ref FrameActionPanelLeft : ref FrameActionPanelRight;

        /// <summary>
        /// Объект настроек панели для активного объекта реализации
        /// </summary>
        private SettingsPanelActionFrameworkElement ActiveObject;

        /// <summary>
        /// Индекс смены окна страницы
        /// </summary>
        private int PanelVerschachtelung = 0;

        /// <summary>
        /// Делегат события закрытия панели действий
        /// </summary>
        /// <param name="FrameworkElementName">Имя активного объекта для палени действий</param>
        public delegate void ClosingPanelAction(string FrameworkElementName);

        /// <summary>
        /// Событие закрытия панели действий
        /// </summary>
        public event ClosingPanelAction? EventClosingPanelAction;

        public IELActionPanel()
        {
            InitializeComponent();
            KeyActivateKeyboardMode = Key.Z;
        }

        /// <summary>
        /// Метод использования панели действий независимо на её состояние
        /// </summary>
        /// <param name="Settings">Объект настроек для взаимодействия с панелью действий</param>
        public void UsingPanelAction(SettingsPanelActionFrameworkElement Settings)
        {
            if (!FlagPanelActionActivate) OpenPanelAction(Settings);
            else
            {
                if (!ActiveObject.ElementInPanel.Name.Equals(Settings.ElementInPanel.Name))
                {
                    if (Settings.ElementInPanel.ActualWidth < Settings.SizedPanel.Width)
                        Settings.SizedPanel = new(Settings.ElementInPanel.ActualWidth, Settings.SizedPanel.Height);
                    if (Settings.ElementInPanel.ActualHeight < Settings.SizedPanel.Height)
                        Settings.SizedPanel = new(Settings.SizedPanel.Width, Settings.ElementInPanel.ActualHeight);
                    AddBufferElementPageAction(ActiveObject);
                    NextPageInActtionPanel(BufferSearchDefaultPage(Settings.ElementInPanel.Name) ?? Settings.DefaultPageInPanel);
                    AnimateSizePanelAction(Settings.SizedPanel);
                    ActiveObject = Settings;
                }
                AnimationMovePanelAction(PositionAnimActionPanel.Default, Settings.SizedPanel, Settings.ElementInPanel);
            }
        }

        /// <summary>
        /// Метод открытия панели действий
        /// </summary>
        /// <param name="Settings">Объект настроек для открытия панели действий</param>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private void OpenPanelAction(SettingsPanelActionFrameworkElement Settings)
        {
            if (FlagPanelActionActivate) return;
            Focus();
            ActualFrame.Navigate(BufferSearchDefaultPage(Settings.ElementInPanel.Name) ?? Settings.DefaultPageInPanel);

            DoubleAnimateObj.To = 1d;
            BeginAnimation(OpacityProperty, DoubleAnimateObj);
            AnimationMovePanelAction(PositionAnimActionPanel.Default, Settings.SizedPanel, Settings.ElementInPanel);
            AnimateSizePanelAction(Settings.SizedPanel);
            ActiveObject = Settings;
            FlagPanelActionActivate.Value = true;
        }

        /// <summary>
        /// Метод закрытия панели действий
        /// </summary>
        /// <param name="PositionAnim">Состояние анимирования позиции</param>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void ClosePanelAction(PositionAnimActionPanel PositionAnim = PositionAnimActionPanel.Default)
        {
            if (!FlagPanelActionActivate) return;

            DoubleAnimateObj.To = 0d;
            BeginAnimation(OpacityProperty, DoubleAnimateObj);
            AnimationMovePanelAction(PositionAnim, new(0, 0), ActiveObject.ElementInPanel);
            AnimateSizePanelAction(new(0, 0));
            AddBufferElementPageAction(ActiveObject);
            
            FlagPanelActionActivate.Value = false;
            EventClosingPanelAction?.Invoke(ActiveObject.ElementInPanel.Name);
            ClearInformation();
        }

        /// <summary>
        /// Перенаправить страницу панели
        /// </summary>
        /// <param name="Content">Новая страница панели</param>
        /// <param name="RightAlign">Правая ориентация движения</param>
        public void NextPageInActtionPanel([NotNull()] IPageModuleButtonKeyAAC Content, bool RightAlign = true)
        {
            if (!FlagPanelActionActivate) return;
            PanelVerschachtelung = (PanelVerschachtelung + 1) % 2;

            ActualFrame.Opacity = 0d;
            Canvas.SetZIndex(BackFrame, 0);
            Canvas.SetZIndex(ActualFrame, 1);
            BackFrame.IsEnabled = false;
            ActualFrame.IsEnabled = true;
            ActualFrame.BeginAnimation(MarginProperty, null);
            ActualFrame.Margin = !RightAlign ? new(-20, -20, 40, -3) : new(40, -10, -20, -3);
            Content.KeyboardMode = RefBackPage.KeyboardMode;
            ActualFrame.Navigate(Content);

            DoubleAnimateObj.To = 0d;
            BackFrame.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            ThicknessAnimate.To = !RightAlign ? new(40, -20, -20, -3) : new(-20, -20, 40, -3);
            BackFrame.BeginAnimation(MarginProperty, ThicknessAnimate);

            DoubleAnimateObj.To = 1;
            ActualFrame.BeginAnimation(OpacityProperty, DoubleAnimateObj);
            ThicknessAnimate.To = new(0);
            ActualFrame.BeginAnimation(MarginProperty, ThicknessAnimate);
        }

        /// <summary>
        /// Поиск страницы сохранённой в буфере
        /// </summary>
        /// <param name="FrameworkElement_Name">Имя объекта в котором была сохранена страница</param>
        /// <returns>Возможно найденная страница</returns>
        private IPageModuleButtonKeyAAC? BufferSearchDefaultPage(string FrameworkElement_Name)
        {
            string[] BufferNames = [.. BufferPages.Select((i) => i.Item2)];
            if (BufferNames.Any((i) => i.Equals(FrameworkElement_Name)))
            {
                int Index = Array.IndexOf(BufferNames, FrameworkElement_Name);
                IPageModuleButtonKeyAAC Page = BufferPages[Index].Item1;
                BufferPages.RemoveAt(Index);
                return Page;
            }
            return null;
        }

        /// <summary>
        /// Метод добавления объекта в буфер
        /// </summary>
        /// <param name="SettingsElement">Объект настроек для добавления в буфер</param>
        private void AddBufferElementPageAction(SettingsPanelActionFrameworkElement SettingsElement)
        {
            if (!((IPageModuleButtonKeyAAC)ActualFrame.Content).PageName.Equals(SettingsElement.DefaultPageInPanel.PageName))
                BufferPages.Add(((IPageModuleButtonKeyAAC)ActualFrame.Content, SettingsElement.ElementInPanel.Name));
        }

        /// <summary>
        /// Метод очистки информации при закрытой панели действий
        /// </summary>
        private void ClearInformation()
        {
            ActiveObject = default;
            BackFrame.Navigate(null);
            ActualFrame.Navigate(null);
        }

        /// <summary>
        /// Метод аниммирования размера панели действий
        /// </summary>
        /// <param name="size">Ожидаемый размер панели действий</param>
        private void AnimateSizePanelAction(Size size)
        {
            DoubleAnimateObj.From = ActualWidth;
            DoubleAnimateObj.To = size.Width;
            BeginAnimation(WidthProperty, DoubleAnimateObj);
            DoubleAnimateObj.From = ActualHeight;
            DoubleAnimateObj.To = size.Height;
            BeginAnimation(HeightProperty, DoubleAnimateObj);
            DoubleAnimateObj.From = null;
        }

        /// <summary>
        /// Анимировать передвижение панели действий константно
        /// </summary>
        /// <param name="StylePositionToAnimate">Вид вычисления позиции позиции анимации</param>
        /// <param name="ActionPanelSize">Размер панели действий при взаимодействии</param>
        /// <param name="Element">Элемент в котором будет находиться панель</param>
        private void AnimationMovePanelAction(PositionAnimActionPanel StylePositionToAnimate, Size ActionPanelSize, FrameworkElement Element)
        {
            if (StylePositionToAnimate == PositionAnimActionPanel.Default)
            {
                Point MousePoint = Mouse.GetPosition((IInputElement)VisualParent);
                Point OffsetPosElement = Element.TransformToAncestor((Visual)VisualParent).Transform(new Point(0, 0));
                if (MousePoint.X + ActionPanelSize.Width > Element.ActualWidth + OffsetPosElement.X)
                    MousePoint.X = Element.ActualWidth + OffsetPosElement.X - ActionPanelSize.Width - 1;
                if (MousePoint.Y + ActionPanelSize.Height > Element.ActualHeight + OffsetPosElement.Y)
                    MousePoint.Y = Element.ActualHeight + OffsetPosElement.Y - ActionPanelSize.Height - 1;
                ThicknessAnimate.To = new Thickness(MousePoint.X, MousePoint.Y, 0, 0);
            }
            else if (StylePositionToAnimate == PositionAnimActionPanel.CenterObject)
            {
                ThicknessAnimate.To =
                    new Thickness(
                        Margin.Left + Width / 2,
                        Margin.Top + Height / 2,
                        0, 0);
            }
            BeginAnimation(MarginProperty, ThicknessAnimate);
        }
    }
}
