using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Timer = System.Timers.Timer;
using System.Windows.Input;
using System.Windows.Media.Animation;
using AAC20.Classes.Commands;

namespace AAC20
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private sealed class UpdateBackgroundData
        {
            /// <summary>
            /// Объект управляющий фоновым обновлением визуальной информации
            /// </summary>
            public readonly Timer TimerDataUpdate;

            /// <summary>
            /// Инициализировать объект управления фоновым обновлением информации в данном окне
            /// </summary>
            public UpdateBackgroundData(ElapsedEventHandler Elapsed)
            {
                TimerDataUpdate = new(1000);
                TimerDataUpdate.Elapsed += Elapsed;
            }
        }

        /// <summary>
        /// Реальное время
        /// </summary>
        private static string RealTime => DateTime.Now.ToString("HH:mm:ss");

        /// <summary>
        /// Реальное время
        /// </summary>
        private static string RealData => DateTime.Now.ToString("dd.MM.yyyy");

        /// <summary>
        /// Объект управления фоновым обновлением информации в данном окне
        /// </summary>
        private readonly UpdateBackgroundData UpdateBackgroundDataThis;

        public MainWindow()
        {
            InitializeComponent();
            App.DataConsoleCommand.AddRange([
                new ConsoleCommand("clear", "Очистка выводимых данных", (param) =>
                {
                    //ObjLog.LOGTextAppend($"Была распознана очистка консоли <tbOutput> (Командой clear)");
                    //AnimationDL.StopAnimate(AnimationDL.StyleAnimateObj.AnimText, "tbOutput");
                    RichTextBoxMainMessage.Document = new();
                    return Task.FromResult(CommandStateResult.Completed);
                }),
                new ConsoleCommand("print", [new Parameter("Text", true)], "Вывод текста на экран", (param) =>
                {
                    if (param.Length == 0) return Task.FromResult(CommandStateResult.FaledParameteres("Print"));
                    RichTextBoxMainMessage.Document.Blocks.Add(new Paragraph(new Run($">>> {param[0]}\n")));
                    return Task.FromResult(CommandStateResult.Completed);
                }),
            ]);

            UpdateBackgroundDataThis = new((sender, e) => Dispatcher.BeginInvoke(BackgroundUpdateVisualData));
            BackgroundUpdateVisualData();

            ButtonReturnCommand.MouseUp += (sender, e) =>
            {
                ActivateActionCommand();
            };
            TextBoxCommandInput.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    TextBoxCommandInput.TextBackground.BeginAnimation(SolidColorBrush.ColorProperty,
                        new ColorAnimation(Color.FromRgb(160, 245, 200), TimeSpan.FromMilliseconds(90)));
                }
            };
            TextBoxCommandInput.KeyUp += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    TextBoxCommandInput.TextBackground.BeginAnimation(SolidColorBrush.ColorProperty,
                        new ColorAnimation(Color.FromRgb(120, 204, 160), TimeSpan.FromMilliseconds(430)));
                    ActivateActionCommand();
                }
            };


            UpdateBackgroundDataThis.TimerDataUpdate.Start();
        }

        private void ActivateActionCommand()
        {
            if (TextBoxCommandInput.Text.Length == 0) return;
            string CommandString = TextBoxCommandInput.Text;
            TextBoxCommandInput.Text = string.Empty;
            CommandStateResult Result = ConsoleCommand.ReadAndExecuteCommand([.. App.DataConsoleCommand], CommandString);
            if (Result.State == ResultState.Complete)
            {
            }
            else
            {
                RichTextBoxMainMessage.Document.Blocks.Add(new Paragraph(new Run($">>> {Result.Massage}\n")));
            }
            // Paragraph myParagraph = new();
            // myParagraph.Inlines.Add(new Run(CommandString));
            // RichTextBoxMainMessage.Document.Blocks.Add(myParagraph);
        }

        /// <summary>
        /// Функция обновления визуальной информации в данном окне
        /// </summary>
        private void BackgroundUpdateVisualData()
        {
            TextBlockTime.Text = RealTime;
            TextBlockData.Text = RealData;
        }
    }
}