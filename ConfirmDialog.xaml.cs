using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace GitCleaner
{
    public partial class ConfirmDialog : Window
    {
        public ConfirmDialog() : this("") { }

        public ConfirmDialog(string message, bool showConfirmButton = true, string? title = null)
        {
            InitializeComponent();
            
            Loaded += (s, e) => {
                try {
                    var uri = new Uri("avares://GithubRemover/Assets/shutdown_def.png");
                    using var stream = AssetLoader.Open(uri);
                    CloseIcon.Source = new Bitmap(stream);
                } catch { }
            };
            
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
                ConfirmBtn.IsVisible = false;
                CancelBtn.Content = okTexts[lang];
            }
        }

        private void ConfirmBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(true);
        }

        private void CancelBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }

        private void CloseBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
