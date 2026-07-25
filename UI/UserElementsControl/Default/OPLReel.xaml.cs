using IEL.CORE.Classes;
using OPLAPI.CORE.Animation;
using OPLAPI.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Media.Playback;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OperPageLes.UI.UserElementsControl.Default
{
    /// <summary>
    /// Логика взаимодействия для OPLReel.xaml
    /// </summary>
    public partial class OPLReel : System.Windows.Controls.UserControl, IOPLAnimate
    {
        #region Parameters

        //#region Symbols
        ///// <summary>
        ///// Данные конкретного свойства
        ///// </summary>
        //public static readonly new DependencyProperty SymbolsProperty =
        //    DependencyProperty.Register("PaletteElement", typeof(PaletteSpectrum), typeof(OPLLangParameter),
        //        new(new PaletteSpectrum(),
        //            (sender, e) =>
        //            {
        //                PaletteSpectrum palette = (PaletteSpectrum)e.NewValue;
        //                ((OPLLangParameter)sender).PaletteElement = palette;
        //            }));

        ///// <summary>
        ///// Объект палитры
        ///// </summary>
        //public new PaletteSpectrum PaletteElement
        //{
        //    get => (PaletteSpectrum)GetValue(PaletteElementProperty);
        //    set
        //    {
        //        IELTextBoxLangValueTranslate.PaletteElement = value;
        //        SetValue(PaletteElementProperty, value);
        //    }
        //}
        //#endregion

        #endregion

        /// <summary>
        /// Объект менеджера анимаций настроек OPL
        /// </summary>
        public OPLAnimationManager? ManagerAnimation { get; set; }

        /// <summary>
        /// Символы в столбце барабана
        /// </summary>
        private List<Border> Symbols = [];

        /// <summary>
        /// Количество символов в столбце барабана
        /// </summary>
        public int SymbolsCount => Symbols.Count;

        //
        private const int UpHideIndex = -29;

        //
        private int ActiveIndex = -1;

        public OPLReel()
        {
            InitializeComponent();
            DataContext = this;
        }

        //
        public TextBlock AddSymbol(string SourceSymbol)
        {
            Border VisualSymbol = CreateVisualSymbol(SourceSymbol);
            Symbols.Add(VisualSymbol);
            CanvasReel.Children.Add(VisualSymbol);
            if (ActiveIndex == -1) SelectSymbolIndex(0);
            return (TextBlock)VisualSymbol.Child;
        }

        /// <summary>
        /// Создать визуальный элемент символа в барабане
        /// </summary>
        /// <param name="Symbol"></param>
        /// <returns></returns>
        private static Border CreateVisualSymbol(string Symbol)
        {
            TextBlock TextBlockElement = new()
            {
                Text = Symbol,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Padding = new(0d, 0d, 0d, 1d),
                FontSize = 23d,
            };
            Border Result = new()
            {
                Background = new SolidColorBrush(Colors.White),
                CornerRadius = new(4d),
                Height = 28,
                Width = 28,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Child = TextBlockElement,
            };
            Canvas.SetLeft(Result, 3d);
            Canvas.SetTop(Result, 35d);
            return Result;
        }

        /// <summary>
        /// Выделить определённый индекс символа
        /// </summary>
        /// <param name="Index">Индекс символа</param>
        public void SelectSymbolIndex(int Index)
        {
            if (ActiveIndex != -1) Canvas.SetTop(Symbols[ActiveIndex], 35d);
            Canvas.SetTop(Symbols[Index], 3d);
            ActiveIndex = Index;
        }

        /// <summary>
        /// Переместить следующий элемент барабана вверх
        /// </summary>
        /// <param name="MillisecondTime">Количество миллисекунд на анимацию</param>
        private void MoveNext(double MillisecondTime)
        {
            OPLAnimationManager.AnimateTakingZeroTo(ManagerAnimation, Symbols[ActiveIndex], Canvas.TopProperty,
                -29d, TimeSpan.FromMilliseconds(MillisecondTime));
            ActiveIndex = ++ActiveIndex % Symbols.Count;
            OPLAnimationManager.AnimateTakingZeroFromTo(ManagerAnimation, Symbols[ActiveIndex], Canvas.TopProperty,
               35d, 3d, TimeSpan.FromMilliseconds(MillisecondTime));
        }

        public async Task SpinAsync(int FinalIndex, int CountRepeat = 6)
        {
            int CountMove = CountRepeat * Symbols.Count + FinalIndex - ActiveIndex, CurrentMove = 0;
            int MiddleIndexMove = (int)Math.Ceiling((double)CountMove / 2d), CurrentCountMillisecondMove = 0;
            double CurrentMillisecond = 600d;
            while (CurrentMove != CountMove)
            {
                if (CurrentMove < MiddleIndexMove && CurrentMillisecond >= 100d)
                {
                    CurrentMillisecond /= 1.2d;
                }
                else if (CurrentMove >= MiddleIndexMove)
                {
                    if (CurrentCountMillisecondMove > 0) CurrentCountMillisecondMove--;
                    else CurrentMillisecond *= 1.2d;
                }
                else if (CurrentMove < MiddleIndexMove)
                    CurrentCountMillisecondMove++;
                MoveNext(CurrentMillisecond);
                await Task.Delay((int)(CurrentMillisecond / 2));
                if (CurrentMove - 1 == CountMove)
                {
                    break;
                }
                CurrentMove++;
            }
            //int totalItems = Symbols.Count;
            //int offset = spinCount * totalItems + finalIndex;
            //double itemHeight = 100; // Высота одного элемента

            //double targetOffset = offset * itemHeight;

            //await Task.Delay(3000);
        }
    }
}
