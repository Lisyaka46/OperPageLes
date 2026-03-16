using IEL.UserElementsControl.Base;
using OPLAnimation.CORE.Animation;
using OPLAnimation.CORE.Interfaces;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ApplicationOperPageLes.UI.UserElementsControl.Network
{
    /// <summary>
    /// Логика взаимодействия для OPLNetworkClipFile.xaml
    /// </summary>
    public partial class OPLNetworkClipElement : IELContainerBase, IOPLAnimate
    {
        #region Properties

        #region Text
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OPLNetworkClipElement),
                new("Name",
                    (sender, e) =>
                    {
                        ((OPLNetworkClipElement)sender).TextBlockNameFile.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст отображаемый в имени файла
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        #region TextFileLoading
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty TextFileLoadingProperty =
            DependencyProperty.Register("TextFileLoading", typeof(string), typeof(OPLNetworkClipElement),
                new(string.Empty,
                    (sender, e) =>
                    {
                        ((OPLNetworkClipElement)sender).TextBlockSizeFile.Text = (string)e.NewValue;
                    }));

        /// <summary>
        /// Текст отображаемый в имени файла
        /// </summary>
        public string TextFileLoading
        {
            get => (string)GetValue(TextFileLoadingProperty);
            set => SetValue(TextFileLoadingProperty, value);
        }
        #endregion

        #region FontFamily
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly new DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(System.Windows.Media.FontFamily), typeof(OPLNetworkClipElement),
                new(new System.Windows.Media.FontFamily("Calibri"),
                    (sender, e) =>
                    {
                        ((OPLNetworkClipElement)sender).TextBlockNameFile.FontFamily = (System.Windows.Media.FontFamily)e.NewValue;
                        ((OPLNetworkClipElement)sender).TextBlockSizeFile.FontFamily = (System.Windows.Media.FontFamily)e.NewValue;
                        ((OPLNetworkClipElement)sender).TextBlockIndex.FontFamily = (System.Windows.Media.FontFamily)e.NewValue;
                    }));

        /// <summary>
        /// Шрифт отображаемый в элементе
        /// </summary>
        public new System.Windows.Media.FontFamily FontFamily
        {
            get => (System.Windows.Media.FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        #endregion

        #region StrokeDashLength
        /// <summary>
        /// Данные конкретного свойства
        /// </summary>
        public static readonly DependencyProperty StrokeDashLengthProperty =
            DependencyProperty.Register("StrokeDashLength", typeof(double), typeof(OPLNetworkClipElement),
                new(28d,
                    (sender, e) =>
                    {
                        ((OPLNetworkClipElement)sender).RectangleLoading.StrokeDashArray[0] = (double)e.NewValue;
                    }));

        /// <summary>
        /// Длинна прирывистой линии
        /// </summary>
        public double StrokeDashLength
        {
            get => (double)GetValue(StrokeDashLengthProperty);
            set => SetValue(StrokeDashLengthProperty, value);
        }
        #endregion

        #endregion

        /// <summary>
        /// Состояние активности взаимодействия с файлом
        /// </summary>
        public bool IsManipulate { get; private set; } = false;

        /// <summary>
        /// Объект менеджера анимационных настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        public OPLNetworkClipElement()
        {
            InitializeComponent();
            BorderIndex.Width = 0;
            RotateGradientLoading.Angle = 0d;
            RadialGradientLoading.Center = new(0.5d, 0.5d);
            RectangleLoading.StrokeDashOffset = 28d;
            RectangleLoading.StrokeDashArray[0] = 28d;
            TextBlockSizeFile.Text = string.Empty;
            TextBlockNameFile.Text = string.Empty;
            TextBlockIndex.Text = string.Empty;

            BorderIndex.BorderBrush = SourceBorderBrush.SourceBrush;
            TextBlockSizeFile.Foreground = SourceForeground.SourceBrush;
            TextBlockNameFile.Foreground = SourceForeground.SourceBrush;
            TextBlockIndex.Foreground = SourceForeground.SourceBrush;
            RectangleLoading.Fill = SourceForeground.SourceBrush;
        }

        /// <summary>
        /// Начать визуализировать взаимодействие
        /// </summary>
        public void StartManipulate()
        {
            if (IsManipulate)
                throw new Exception("Невозможно визуализировать взаимодействие при уже активном взаимодействии");
            IsManipulate = true;

            RectangleLoading.StrokeDashOffset = 28d;
            if (ManagerAnimation != null)
            {
                RadialGradientLoading.BeginAnimation(RadialGradientBrush.OpacityProperty, null);
                RadialGradientLoading.Opacity = 0d;
                ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleLoading, System.Windows.Shapes.Rectangle.StrokeThicknessProperty,
                    4d, TimeSpan.FromSeconds(1d));
                StrokeDashLength = 28d;
                DoubleAnimation animation = ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
                animation.From = 0d;
                animation.To = 1d;
                animation.Duration = TimeSpan.FromSeconds(2d);
                animation.BeginTime = TimeSpan.FromSeconds(0.8d);
                RadialGradientLoading.BeginAnimation(RadialGradientBrush.OpacityProperty, animation);
                ManagerAnimation.PointAnimationType.AnimateEffect(RadialGradientLoading, RadialGradientBrush.CenterProperty,
                    new(0.35d, 0.5d), TimeSpan.FromMilliseconds(1500d));
                DoubleAnimation animationAngle = ManagerAnimation.DoubleAnimationType.SourceAnimation.Clone();
                animationAngle.EasingFunction = null;
                animationAngle.From = 0d;
                animationAngle.To = 360d;
                animationAngle.RepeatBehavior = RepeatBehavior.Forever;
                animationAngle.Duration = TimeSpan.FromSeconds(4d);
                RotateGradientLoading.BeginAnimation(RotateTransform.AngleProperty, animationAngle);
            }
            else
            {
                StrokeDashLength = 28d;
                RectangleLoading.StrokeThickness = 4d;
                RotateGradientLoading.Angle = 0d;
                RadialGradientLoading.Center = new(0.5d, 0.5d);
            }
            //while (OpenStreamFile.Length < DataCount)
            //{
            //    await Task.Delay(500);
            //    if (ManagerAnimation != null)
            //        ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleLoading, System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty,
            //            26 - (26 * (DataCount / OpenStreamFile.Length)), TimeSpan.FromMilliseconds(400d));
            //    else
            //        RectangleLoading.StrokeDashOffset =
            //            26 - (26 * (DataCount / OpenStreamFile.Length));
            //}
            
        }

        /// <summary>
        /// Установить текущее значение для манипуляции
        /// </summary>
        /// <param name="CurrentValue">Текущее значение</param>
        /// <param name="MaxValue">Mаксимальное значение</param>
        [LoaderOptimization(LoaderOptimization.NotSpecified)]
        public void SetValueManipulate(double Value)
        {
            if (!IsManipulate)
                throw new Exception("Невозможно взаимодействовать с объектом, предварительно не включив режим взаимодействия!");
            if (Value > 1d) Value = 1d;
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleLoading, System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty,
                        StrokeDashLength - (Value * StrokeDashLength), TimeSpan.FromMilliseconds(200d));
            else
                RectangleLoading.StrokeDashOffset = StrokeDashLength - (Value * StrokeDashLength);
        }

        /// <summary>
        /// Закончить визуализировать взаимодействие
        /// </summary>
        public void EndManipulate()
        {
            if (!IsManipulate)
                throw new Exception("Невозможно закончить визуализировать взаимодействие при не активном взаимодействии");
            IsManipulate = false;
            if (ManagerAnimation != null)
            {
                RectangleLoading.BeginAnimation(System.Windows.Shapes.Rectangle.StrokeDashOffsetProperty, null);
                BeginAnimation(StrokeDashLengthProperty, null);
                ManagerAnimation.PointAnimationType.AnimateEffect(RadialGradientLoading, RadialGradientBrush.CenterProperty,
                    new(0.5d, 0.5d), TimeSpan.FromSeconds(2d));
                ManagerAnimation.DoubleAnimationType.AnimateEffect(RotateGradientLoading, RotateTransform.AngleProperty,
                    0d, TimeSpan.FromSeconds(2d));
                ManagerAnimation.DoubleAnimationType.AnimateEffect(RectangleLoading, System.Windows.Shapes.Rectangle.StrokeThicknessProperty,
                    0d, TimeSpan.FromSeconds(2d));
                StrokeDashLength = 448d;
            }
            else
            {
                RectangleLoading.StrokeThickness = 0d;
            }
            RectangleLoading.StrokeDashOffset = 28d;
        }

        /// <summary>
        /// Установить иконку по расширению файла
        /// </summary>
        /// <param name="FilePath">Путь к файлу</param>
        /// <param name="DefaultIconFile">Значение по умолчанию при неудачной установке иконки</param>
        public void SetExtractAssociatedIcon(string FilePath, ImageSource? DefaultIconFile = null)
        {
            try
            {
                Icon? FileIcon = System.Drawing.Icon.ExtractAssociatedIcon(FilePath);
                Dispatcher.Invoke(() => IconLoadingFile.Source =
                    FileIcon != null ? Imaging.CreateBitmapSourceFromHBitmap(FileIcon.ToBitmap().GetHbitmap(),
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()) : DefaultIconFile);
            }
            catch
            {
                Dispatcher.Invoke(() => IconLoadingFile.Source = DefaultIconFile);
            }
        }

        /// <summary>
        /// Установить визуализационный индекс
        /// </summary>
        /// <param name="IndexView">Индекс</param>
        public void SetIndex(uint IndexView)
        {
            TextBlockIndex.Text = IndexView.ToString();
            if (BorderIndex.Width == 0)
            {
                if (ManagerAnimation != null)
                    ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderIndex, WidthProperty, 20d, TimeSpan.FromMilliseconds(500d));
                else
                    BorderIndex.Width = 20d;
            }
                
        }

        /// <summary>
        /// Отключить визуализационный индекс
        /// </summary>
        public void ClearIndex()
        {
            if (ManagerAnimation != null)
                ManagerAnimation.DoubleAnimationType.AnimateEffect(BorderIndex, WidthProperty, 0d, TimeSpan.FromMilliseconds(500d));
            else
                BorderIndex.Width = 0d;
        }

        /// <summary>
        /// Расчитать и отобразить размер файла
        /// </summary>
        /// <param name="Path">Директория файла</param>
        /// <exception cref="Exception"></exception>
        public void MathSizeFile(string Path) => MathSizeFile(new FileInfo(Path).Length);

        /// <summary>
        /// Расчитать и отобразить размер файла
        /// </summary>
        /// <param name="CountBytes">Количетсво байт хранящееся в файле</param>
        /// <exception cref="Exception"></exception>
        public void MathSizeFile(long CountBytes)
        {
            long LengthFile;
            byte CountR;
            double MainLengthFile;
            string R;
            MainLengthFile = (short)(CountBytes % 1024); // Установка смещения от 1024
            LengthFile = CountBytes - (int)MainLengthFile; // Число кратное 1024
            CountR = 0;
            while (LengthFile >= 1024)
            {
                LengthFile /= 1024;
                if (MainLengthFile > 0.01f)
                    MainLengthFile /= 1024; // Расчёт смещения относительно единицы измерения кол-ва информ.
                CountR++;
            }
            R = CountR switch
            {
                0 => "B",
                1 => "KB",
                2 => "MB",
                3 => "TB",
                4 => "PB",
                _ => throw new Exception("Слишком большой размер файла."),
            };
            MainLengthFile += LengthFile;
            Dispatcher.Invoke(() => TextFileLoading = $"{Math.Round(MainLengthFile, 2)} {R}");
        }
    }
}
