using System.Windows;

namespace GitCleaner
{
    public partial class ConfirmDialog : Window
    {
        public ConfirmDialog(string message, bool showConfirmButton = true, string title = "Подтверждение")
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text = title;

            if (!showConfirmButton)
            {
                ConfirmBtn.Visibility = Visibility.Collapsed;
                CancelBtn.Content = "ОК";
            }
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