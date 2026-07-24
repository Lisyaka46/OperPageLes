using OperPageLes.CORE.Struct;
using OperPageLes.UI.Windows.DEV;
using OPLAPI.CORE.Animation;
using OPLAPI.OIEL.CORE.Browser;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using DrColor = System.Drawing.Color;
using OPRES = OperPageLes.Properties.Resources;
using Point = System.Windows.Point;

namespace OperPageLes.UI.Pages.Browser.InlayPages
{
    /// <summary>
    /// Логика взаимодействия для PageDeveloper.xaml
    /// </summary>
    public partial class PageDeveloper : PageBrowser
    {
        private double _sagOffset = 20;

        private bool _isDragging = false;
        private FrameworkElement _draggedElement;
        private Point _dragStartPoint;
        private Point _elementStartPosition;
        private Point _lastPosition1;
        private Point _lastPosition2;
        private DispatcherTimer _moveTimer;
        private Point _currentControlPoint;
        private Point _lastPos1, _lastPos2;


        private Point StartPositionMouse;
        private bool Activate = false;
        Storyboard myStoryboard = new();
        private Vector3DAnimation Vector3DAnim = new()
        {
            From = null,
            To = null,
            Duration = TimeSpan.FromMilliseconds(500d),
            EasingFunction = new CircleEase()
            {
                EasingMode = EasingMode.EaseInOut,
            }
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
                Check.ManagerAnimation = value;
            }
        }

        public PageDeveloper()
        {
            InitializeComponent();
            Icon = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.ValidKeyIcon));
            //Loaded += async (sender, e) =>
            //{
            //    const int CountParticles = 90, DelayOneParticle = 100;
            //    TimeSpan TimeParticle = TimeSpan.FromMilliseconds(DelayOneParticle * 8);
            //    Random randomColor = new();
            //    //IELObjectBase Particle;
            //    FrameworkElement Particle;
            //    int i = 0;
            //    for (int K = 0; K < CountParticles; K++)
            //    {
            //        DoubleAnimation animationD = new()
            //        {
            //            Duration = TimeParticle,
            //            From = 1d,
            //            To = 0d
            //        };
            //        DoubleAnimation animationW = new()
            //        {
            //            Duration = TimeParticle,
            //            From = 15d,
            //            To = 60d
            //        };
            //        DoubleAnimation animationH = new()
            //        {
            //            Duration = TimeParticle,
            //            From = 15d,
            //            To = 60d
            //        };
            //        Particle = new System.Windows.Shapes.Rectangle()
            //        {
            //            Height = 4,
            //            Width = 4,
            //            Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
            //                (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255))),
            //            Opacity = 0d,
            //            RadiusX = 1,
            //            RadiusY = 1,
            //        };
            //        //Particle = new IELBlockInfoText()
            //        //{
            //        //    Width = 20,
            //        //    Height = 20,
            //        //    Opacity = 0d,
            //        //    CornerRadius = new(3),
            //        //    Text = "F",
            //        //};
            //        //Particle = new System.Windows.Controls.Image()
            //        //{
            //        //    Height = 15,
            //        //    Width = 15,
            //        //    //Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(
            //        //    //    (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255), (byte)randomColor.Next(0, 255))),
            //        //    Source = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.IconMainApplication)),
            //        //    Opacity = 0d
            //        //};
            //        //App.CurrentApp.ActiveThemeApplication[CORE.Enums.PaletteSpectrumEnum.Gray].ConnectPalleteFromIELElement(Particle);
            //        Thickness Start = new(
            //            Particles.ActualWidth / 2 - Particle.Width / 2,
            //            Particles.ActualHeight / 2 - Particle.Height / 2, 0, 0);

            //        Particle.Margin = Start;
            //        Particles.Children.Add(Particle);

            //        ThicknessAnimation animationT = App.ThicknessAnimationType.SourceAnimation.Clone();
            //        animationT.Duration = TimeParticle;
            //        animationT.From = Start;
            //        RepeatBoard(animationT, randomColor, Start);

            //        animationT.FillBehavior = FillBehavior.Stop;
            //        animationT.Completed += (sender, e) =>
            //        {
            //            Particles.Children[i].BeginAnimation(MarginProperty, null);
            //            Particles.Children[i].BeginAnimation(OpacityProperty, null);
            //            //Particles.Children[i].BeginAnimation(WidthProperty, null);
            //            //Particles.Children[i].BeginAnimation(HeightProperty, null);

            //            RepeatBoard(animationT, randomColor, Start);

            //            Particles.Children[i].BeginAnimation(MarginProperty, animationT);
            //            Particles.Children[i].BeginAnimation(OpacityProperty, animationD);
            //            //Particles.Children[i].BeginAnimation(WidthProperty, animationW);
            //            //Particles.Children[i].BeginAnimation(HeightProperty, animationH);
            //            i = ++i % Particles.Children.Count;
            //        };

            //        Particle.BeginAnimation(MarginProperty, animationT);
            //        Particle.BeginAnimation(OpacityProperty, animationD);
            //        //Particle.BeginAnimation(WidthProperty, animationW);
            //        //Particle.BeginAnimation(HeightProperty, animationH);
            //        await Task.Delay(DelayOneParticle);
            //    }
            //};
            //ManagerAnimation.GetCloneAnimationElementFromType<DoubleAnimation>();
            //anim.From = null;
            //anim.Duration = TimeSpan.FromMilliseconds(3000d);
            //myStoryboard.Children.Add(anim);
            //Check.ImageOpacityTexture = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.Check));
            ImageBrushModel.ImageSource = StructDirectoryResources.GetResourceBitmap(nameof(OPRES.VECTOR));
            MyAnimatedObject.MouseEnter += (sender, e) =>
            {
                myAngleRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
                Point CurrentPosModel = Model.TransformToAncestor((Visual)App.Current.MainWindow).TransformBounds(Model.Content.Bounds).Location;
                StartPositionMouse = new(
                    CurrentPosModel.X + Model.Content.Bounds.SizeX * myPerspectiveCamera.FieldOfView,
                    CurrentPosModel.Y + Model.Content.Bounds.SizeY * myPerspectiveCamera.FieldOfView);
                //StartPositionMouse = Mouse.GetPosition(App.Current.MainWindow);
                Activate = true;
                GetAngleModelRotate();
            };
            MyAnimatedObject.MouseLeave += (sender, e) =>
            {
                if (!Activate) return;
                Activate = false;
                OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, myAngleRotation, AxisAngleRotation3D.AngleProperty,
                    0d, TimeSpan.FromMilliseconds(500d));
            };
            MyAnimatedObject.MouseMove += (sender, e) =>
            {
                if (!Activate) return;
                GetAngleModelRotate();
                //myAngleRotation.Angle = GetAngleModelRotate();
            };
            WindowGenerateQdata.Click += (sender, e) =>
            {
                WindowQDataViewer window = new();
                window.Show();
            };

            WindowCheckIcons.Click += (sender, e) =>
            {
                WindowTestIcons window = new();
                window.Show();
            };

            //IAddChild? ChildrenControl = null;
            //UIElement? Child = null;
            //BrushConverter SourceBrushConverter = new();
            //ThicknessConverter SourceThicknessConverter = new();
            //CornerRadiusConverter SourceCornerRadiusConverter = new();
            //DoubleConverter SourceDoubleConverter = new();

            //using (XmlReader reader = XmlReader.Create(@"C:\Users\killm\Рабочий стол\Page1.xaml"))
            //{
            //    while (reader.Read())
            //    {
            //        switch (reader.NodeType)
            //        {
            //            case XmlNodeType.Element:
            //                Type type = ByName(reader.Name) ??
            //                    throw new Exception($"Данный тип \"{reader.Name}\" не поддерживается");
            //                object instance = Activator.CreateInstance(type) ??
            //                    throw new Exception($"Данный тип \"{reader.Name}\" не удалось создать через инициализаторы");
            //                if (type.BaseType == typeof(System.Windows.Controls.Panel))
            //                    ChildrenControl = (IAddChild)Convert.ChangeType(instance, type);
            //                else if (ChildrenControl != null)
            //                {
            //                    Child = (UIElement)Convert.ChangeType(instance, type);
            //                    ChildrenControl.AddChild(Child);
            //                    for (int i = 0; i < reader.AttributeCount; i++)
            //                    {
            //                        reader.MoveToAttribute(i);
            //                        PropertyInfo P_Info = type.GetProperty(reader.Name) ??
            //                            throw new Exception($"Свойство \"{reader.Name}\" не поддерживается в данном объекте \"{type.Name}\"");
            //                        if (P_Info.PropertyType != reader.ValueType)
            //                        {
            //                            if (reader.ValueType == typeof(string))
            //                            {
            //                                if (P_Info.PropertyType == typeof(System.Windows.Media.Brush))
            //                                {
            //                                    P_Info.SetValue(Child, SourceBrushConverter.ConvertFromString(reader.Value));
            //                                }
            //                                else if (P_Info.PropertyType == typeof(Thickness))
            //                                {
            //                                    P_Info.SetValue(Child, SourceThicknessConverter.ConvertFromInvariantString(reader.Value));
            //                                }
            //                                else if (P_Info.PropertyType == typeof(CornerRadius))
            //                                {
            //                                    P_Info.SetValue(Child, SourceCornerRadiusConverter.ConvertFromInvariantString(reader.Value));
            //                                }
            //                                else if (P_Info.PropertyType == typeof(double))
            //                                {
            //                                    P_Info.SetValue(Child, SourceDoubleConverter.ConvertFromInvariantString(reader.Value));
            //                                }
            //                                else throw new Exception(
            //                                    $"Не найдено поддерживаемая конвертация строкового значения в ожидаемый тип \"{P_Info.PropertyType.Name}\"");
            //                                continue;
            //                            }
            //                        }
            //                        P_Info.SetValue(Child, Convert.ChangeType(reader.Value, reader.ValueType));
            //                    }
            //                }
            //                else throw new Exception("Невозможно создать элемент который находится не в контейнере и не является контейнером");
            //                Console.WriteLine("Start Element {0}", reader.Name);
            //                break;
            //            case XmlNodeType.Text:
            //                Console.WriteLine("Text Node: {0}",
            //                         reader.GetValueAsync());
            //                break;
            //            case XmlNodeType.EndElement:
            //                Console.WriteLine("End Element {0}", reader.Name);
            //                break;
            //            default:
            //                Console.WriteLine("Other node {0} with value {1}",
            //                                reader.NodeType, reader.Value);
            //                break;
            //        }
            //    }
            //}
            //if (ChildrenControl != null)
            //    SourceGridContent.Children.Add((UIElement)ChildrenControl);

            // Таймер для отслеживания движения
            _moveTimer = new DispatcherTimer();
            _moveTimer.Interval = TimeSpan.FromMilliseconds(5);
            _moveTimer.Tick += CheckMovement;

            Element1.MouseLeftButtonDown += OnMouseDown;
            Element2.MouseLeftButtonDown += OnMouseDown;
            Element1.MouseMove += OnMouseMove;
            Element2.MouseMove += OnMouseMove;
            Element1.MouseLeftButtonUp += OnMouseUp;
            Element2.MouseLeftButtonUp += OnMouseUp;

            int _sagOffset = 30;
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += (s, e) =>
            {
                double x1 = Canvas.GetLeft(Element1);
                double y1 = Canvas.GetTop(Element1);
                double x2 = Canvas.GetLeft(Element2);
                double y2 = Canvas.GetTop(Element2);

                x1 += Element1.Width / 2;
                y1 += Element1.Height / 2;
                x2 += Element2.Width / 2;
                y2 += Element2.Height / 2;

                Point currentPos1 = new Point(x1, y1);
                Point currentPos2 = new Point(x2, y2);

                double speed1 = Math.Sqrt(Math.Pow(currentPos1.X - _lastPos1.X, 2) + Math.Pow(currentPos1.Y - _lastPos1.Y, 2));
                double speed2 = Math.Sqrt(Math.Pow(currentPos2.X - _lastPos2.X, 2) + Math.Pow(currentPos2.Y - _lastPos2.Y, 2));
                double maxSpeed = Math.Max(speed1, speed2);

                // Устанавливаем точки
                PathFigureSegment.StartPoint = new Point(x1, y1);
                SourceSegment.Point2 = new Point(x2, y2);

                // Расчёт центра и длины
                double midX = (x1 + x2) / 2;
                double midY = (y1 + y2) / 2;
                double dx = x2 - x1;
                double dy = y2 - y1;
                double length = Math.Sqrt(dx * dx + dy * dy);

                // Базовое провисание
                double sag = _sagOffset + length * 0.42;

                // Угол наклона верёвки (для эффекта маятника)
                double angle = Math.Atan2(dy, dx);

                double targetX = midX;
                double targetY = midY + sag;

                if (_isDragging || maxSpeed > 1)
                {
                    double time = DateTime.Now.Millisecond / 1000.0;

                    // Амплитуда зависит от скорости
                    double amplitude = Math.Min(20, maxSpeed * 2);

                    // Частота колебаний
                    double frequency = 30 + (maxSpeed * 0.8);

                    // Затухание
                    double decay = Math.Min(1, maxSpeed / 2);

                    // Основные колебания вверх-вниз
                    double swingY = Math.Sin(time * frequency) * amplitude * decay;

                    // Покачивание влево-вправо (маятниковое движение)
                    // Чем быстрее движение, тем сильнее раскачивание
                    double pendulumAmplitude = amplitude * 2.8;
                    double pendulumSwing = Math.Sin(time * (frequency - 2)) * pendulumAmplitude * decay;

                    // Смещение контрольной точки влево-вправо (перпендикулярно верёвке)
                    double perpX = -Math.Sin(angle) * pendulumSwing;
                    double perpY = Math.Cos(angle) * pendulumSwing;

                    // Эффект "волны" при движении (дополнительное покачивание)
                    double waveX = Math.Sin(time * 12) * (amplitude * 0.8) * decay;

                    targetX = midX + perpX + waveX;
                    targetY = midY + sag + swingY + perpY * 0.5;

                    // Эффект "хлыста" при резком дёрганье
                    if (maxSpeed > 6)
                    {
                        double whip = Math.Sin(time * 12) * (amplitude * 0.5) * decay;
                        targetX += Math.Cos(angle) * whip;
                        targetY += Math.Sin(angle) * whip * 0.5;
                    }
                }

                // Плавное движение контрольной точки
                if (_currentControlPoint == default)
                    _currentControlPoint = new Point(targetX, targetY);
                else
                {
                    double smooth = 0.12d;
                    double newX = _currentControlPoint.X + (targetX - _currentControlPoint.X) * smooth;
                    double newY = _currentControlPoint.Y + (targetY - _currentControlPoint.Y) * smooth;
                    _currentControlPoint = new Point(newX, newY);
                }

                SourceSegment.Point1 = _currentControlPoint;

                // Сохраняем позиции
                _lastPos1 = currentPos1;
                _lastPos2 = currentPos2;
            };
            _moveTimer.Start();
            timer.Start();

            //DispatcherTimer Dashtimer = new DispatcherTimer();
            //Dashtimer.Interval = TimeSpan.FromMilliseconds(1);
            //Dashtimer.Tick += (s, e) =>
            //{
            //    SourcePath.StrokeDashOffset =
            //        (SourcePath.StrokeDashOffset + SourcePath.StrokeDashArray[0] / 10) % (SourcePath.StrokeDashArray[0] * 2);
            //};
            //Dashtimer.Start();

            DoubleAnimation anim = new()
            {
                Duration = TimeSpan.FromMilliseconds(400d),
                From = 0d,
                To = -(SourcePath.StrokeDashArray[0] * 2),
                RepeatBehavior = RepeatBehavior.Forever,
            };
            SourcePath.BeginAnimation(System.Windows.Shapes.Path.StrokeDashOffsetProperty, anim);
        }

        private void CheckMovement(object sender, EventArgs e)
        {
            if (Element1 == null || Element2 == null) return;

            Point currentPos1 = new Point(Canvas.GetLeft(Element1), Canvas.GetTop(Element1));
            Point currentPos2 = new Point(Canvas.GetLeft(Element2), Canvas.GetTop(Element2));

            double dist1 = Math.Abs(currentPos1.X - _lastPosition1.X) + Math.Abs(currentPos1.Y - _lastPosition1.Y);
            double dist2 = Math.Abs(currentPos2.X - _lastPosition2.X) + Math.Abs(currentPos2.Y - _lastPosition2.Y);

            //// Если объект переместился больше чем на 2 пикселя - считаем что двигается
            //_isMoving = (dist1 > 2 || dist2 > 2);

            _lastPosition1 = currentPos1;
            _lastPosition2 = currentPos2;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _draggedElement = sender as FrameworkElement;
            if (_draggedElement != null)
            {
                _isDragging = true;
                _dragStartPoint = e.GetPosition(SourceContainer);
                _elementStartPosition = new Point(
                    Canvas.GetLeft(_draggedElement),
                    Canvas.GetTop(_draggedElement)
                );
                _draggedElement.CaptureMouse();
                e.Handled = true;
            }
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDragging && _draggedElement != null)
            {
                Point currentPoint = e.GetPosition(SourceContainer);
                double deltaX = currentPoint.X - _dragStartPoint.X;
                double deltaY = currentPoint.Y - _dragStartPoint.Y;

                double newLeft = _elementStartPosition.X + deltaX;
                double newTop = _elementStartPosition.Y + deltaY;

                // Ограничиваем перемещение в пределах Canvas
                newLeft = Math.Max(0, Math.Min(newLeft, SourceContainer.ActualWidth - _draggedElement.Width));
                newTop = Math.Max(0, Math.Min(newTop, SourceContainer.ActualHeight - _draggedElement.Height));

                Canvas.SetLeft(_draggedElement, newLeft);
                Canvas.SetTop(_draggedElement, newTop);
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedElement != null)
            {
                _draggedElement.ReleaseMouseCapture();
            }
            _isDragging = false;
            _draggedElement = null;
        }

        private static Type? ByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }

            return null;
        }

        //
        private void GetAngleModelRotate()
        {
            Point CurrentPos = Mouse.GetPosition(App.Current.MainWindow);
            double X = StartPositionMouse.X - CurrentPos.X, Y = StartPositionMouse.Y - CurrentPos.Y;
            X /= Model.Content.Bounds.SizeX * myPerspectiveCamera.FieldOfView;
            Y /= Model.Content.Bounds.SizeY * myPerspectiveCamera.FieldOfView;
            myAngleRotation.Axis = new(-Y, -X, 0);
            myAngleRotation.Angle = 15 * (Math.Abs(X) + Math.Abs(Y)) / 2;
            Element.Text = $"Axis: ({Math.Round(-X, 2)}~2 | {Math.Round(-Y, 2)}~2 | 0~)   Angle: {Math.Round(myAngleRotation.Angle, 2)}~2/2";
        }

        /// <summary>
        /// Добавить новый элемент текста в стек визуализации
        /// </summary>
        /// <param name="TextElement">Отображаемый текст в элементе</param>
        /// <returns>Элемент цвета визуализации</returns>
        internal TextBlock AddNewStackTextBlock(string TextElement)
        {
            TextBlock Element = new()
            {
                Text = TextElement,
                FontSize = 16d,
                Foreground = new SolidColorBrush(Colors.Black),
            };
            StackPanelElementsVisual.Children.Add(Element);
            return Element;
        }

        //
        private void RepeatBoard(ThicknessAnimation Board, Random random, Thickness Start)
        {
            int W, H;
            W = random.Next(-((int)Particles.ActualWidth / 3), (int)Particles.ActualWidth / 3);
            H = random.Next(-Math.Abs(W), Math.Abs(W));
            Board.To = new(Start.Left + W, Start.Top + H, 0, 0);
        }

        /// <summary>
        /// Сгенерировать изображение по формуле
        /// </summary>
        /// <param name="Width">Ширина изображения</param>
        /// <param name="Height">Высота изображения</param>
        /// <returns>Объект карты цвета изображения</returns>
        internal async Task<BitmapSource> GenImage(int Width, int Height)
        {
            int X, Y;
            Bitmap bitmap = new(Width, Height);
            Func<int, int, DrColor> d = new((X, Y) =>
            {
                byte R = (byte)(Math.Cos(Width / 20d - X) * 255);
                byte G = (byte)(Math.Cos(Height / 20d - Y) * 255);
                byte B = (byte)(0);
                return DrColor.FromArgb(R, G, B);
            });
            for (Y = 0; Y < Height; Y++)
            {
                for (X = 0; X < Width; X++)
                {
                    await Task.Run(() => 
                    bitmap.SetPixel(X, Y, d.Invoke(X, Y)));
                    //Dispatcher.Invoke(() => TextblockInformation.Text = $"X:{X} || Y:{Y}");
                }
            }
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                   IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
