using System.Windows;

namespace OperPageLes.UI.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для DialogGenerateTheme.xaml
    /// </summary>
    public partial class DialogGenerateTheme : Window
    {
        //public DialogGenerateTheme()
        //{
        //    InitializeComponent();
        //    //VisualLoading.ManagerAnimation = App.CurrentApp.ManagerAnimation;
        //    VisualLoading.Opacity = 0d;
        //    ComboBoxThemeSourceCreating.SelectionChanged += (sender, e) =>
        //    {
        //        if (IELButtonDefaultTheme.SourceBackground.GetUsedState())
        //            IELButtonDefaultTheme.SourceBackground.SetUsedState(false);
        //    };
        //    IELTextBoxNameTheme.KeyUp += (sender, e) =>
        //    {
        //        switch (e.Key)
        //        {
        //            case Key.Escape:
        //                ComboBoxThemeSourceCreating.Focus();
        //                break;
        //        }
        //    };
        //    IELButtonDefaultTheme.OnActivateMouseLeft += (sender, e) =>
        //    {
        //        ComboBoxThemeSourceCreating.SelectedIndex = -1;
        //        IELButtonDefaultTheme.SourceBackground.SetUsedState(true);
        //    };
        //}

        /// <summary>
        /// Активировать окно создания новой темы приложения
        /// </summary>
        /// <param name="PathesTheme">Массив директорий тем которые загружены в приложение</param>
        /// <returns>Новая создаваемая тема</returns>
        //internal Theme? ShowDialogCreateNewTheme(string[] PathesTheme)
        //{
        //    Theme? Result = null;
        //    ComboBoxThemeSourceCreating.Items.Clear();
        //    if (PathesTheme.Length > 0)
        //    {
        //        for (int i = 0; i < PathesTheme.Length; i++)
        //            ComboBoxThemeSourceCreating.Items.Add(Path.GetFileNameWithoutExtension(PathesTheme[i]));
        //    }
        //    else
        //        ComboBoxThemeSourceCreating.IsEnabled = false;
        //    ComboBoxThemeSourceCreating.SelectedIndex = -1;
        //    IELButtonDefaultTheme.SourceBackground.SetUsedState(true);
        //    IELButtonCreateTheme.OnActivateMouseLeft += async (sender, e) =>
        //    {
        //        string Path = StructDirectoryResources.DirectoryThemeApplication + $"/{IELTextBoxNameTheme.Text}.qd";
        //        if (IELTextBoxNameTheme.Text.Length == 0 || File.Exists(Path))
        //        {
        //            IELTextBoxNameTheme.SourceBackground.SetActiveSpecrum(WnColor.FromRgb(255, 100, 100));
        //            return;
        //        }
        //        IELButtonCancel.IsEnabled = false;
        //        VisualLoading.OpenLoading();
        //        Result = ComboBoxThemeSourceCreating.SelectedIndex != -1 ?
        //            new(PathesTheme[ComboBoxThemeSourceCreating.SelectedIndex]) : new();
        //        Result.Name = IELTextBoxNameTheme.Text;
        //        Result.DirectoryFile = Path;
        //        await Result.GenerateNewFileSource();
        //        await Task.Delay(1000);
        //        VisualLoading.CloseLoading();
        //        Close();
        //    };
        //    IELButtonCancel.OnActivateMouseLeft += (sender, e) =>
        //    {
        //        Close();
        //    };
        //    ShowDialog();
        //    return Result;
        //}
    }
}
