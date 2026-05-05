using System.Windows;
using System.Windows.Media.Imaging;

namespace GitCleaner
{
    public partial class ConfirmDialog : Window
    {
        public ConfirmDialog(string message, bool showConfirmButton = true, string title = null)
        {
            InitializeComponent();
            var lang = MainWindow.IsEnglish ? "en" : "ru";
            var titles = new Dictionary<string, string> { ["ru"] = "Подтверждение", ["en"] = "Confirmation" };
            var okTexts = new Dictionary<string, string> { ["ru"] = "ОК", ["en"] = "OK" };
            var cancelTexts = new Dictionary<string, string> { ["ru"] = "Отмена", ["en"] = "Cancel" };
            var deleteTexts = new Dictionary<string, string> { ["ru"] = "Удалить", ["en"] = "Delete" };
            
            TitleText.Text = title ?? titles[lang];
            MessageText.Text = message;
            CancelBtn.Content = cancelTexts[lang];
            ConfirmBtn.Content = deleteTexts[lang];

            if (!showConfirmButton)
            {
                ConfirmBtn.Visibility = Visibility.Collapsed;
                CancelBtn.Content = okTexts[lang];
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("shutdown_def.png");
                if (stream != null)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    var icon = CloseBtn.Template.FindName("CloseIcon", CloseBtn) as System.Windows.Controls.Image;
                    if (icon != null) icon.Source = bitmap;
                }
            } catch { }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}