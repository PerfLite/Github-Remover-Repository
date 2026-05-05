using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GitCleaner
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> Repositories { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            try {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("github-64.png");
                if (stream != null) {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    IconImage.Source = bitmap;
                }
            } catch { }
            
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(7);
            dispatcherTimer.Tick += (s, e) => {
                var fadeOut = new System.Windows.Media.Animation.DoubleAnimation { From = 1, To = 0, Duration = TimeSpan.FromSeconds(0.3) };
                fadeOut.Completed += (s2, e2) => {
                    TitleText.Text = TitleText.Text == "Cleaner" ? "hub" : "Cleaner";
                    var fadeIn = new System.Windows.Media.Animation.DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromSeconds(0.3) };
                    TitleText.BeginAnimation(OpacityProperty, fadeIn);
                };
                
                TitleText.BeginAnimation(OpacityProperty, fadeOut);
            };
            dispatcherTimer.Start();
            
            RepoList.ItemsSource = Repositories;
            LoadRepos();
        }

        private async void LoadRepos()
        {
            Repositories.Clear();
            var output = await RunGhCommand("repo list --limit 100 --json nameWithOwner");

            if (string.IsNullOrWhiteSpace(output))
            {
                MessageBox.Show("Не удалось загрузить репозитории. Убедитесь, что GitHub CLI установлен и вы вошли в аккаунт (gh auth login).",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using var doc = JsonDocument.Parse(output);
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    if (item.TryGetProperty("nameWithOwner", out var nameProp))
                        Repositories.Add(nameProp.GetString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка разбора данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RepoList.SelectedItem is not string repoName)
            {
                var warnDialog = new ConfirmDialog("Выберите репозиторий.", showConfirmButton: false);
                warnDialog.Owner = this;
                warnDialog.ShowDialog();
                return;
            }

            var dialog = new ConfirmDialog($"Удалить репозиторий {repoName} безвозвратно?");
            dialog.Owner = this;
            if (dialog.ShowDialog() != true)
                return;

            await RunGhCommand($"repo delete {repoName} --yes");
            LoadRepos();
        }

        private async void RefreshBtn_Click(object sender, RoutedEventArgs e) => LoadRepos();

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            RepoList.SelectedItem = null;
        }

        private void RepoList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var hit = e.OriginalSource as System.Windows.DependencyObject;
            while (hit != null)
            {
                if (hit is System.Windows.Controls.ListBoxItem)
                    return;
                hit = System.Windows.Media.VisualTreeHelper.GetParent(hit);
            }
            RepoList.SelectedItem = null;
        }

        private Task<string> RunGhCommand(string arguments)
        {
            return Task.Run(() =>
            {
                try
                {
                    using var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "gh",
                            Arguments = arguments,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            StandardOutputEncoding = System.Text.Encoding.UTF8
                        }
                    };
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output.Trim();
                }
                catch (Exception ex)
                {
                    return $"error: {ex.Message}";
                }
            });
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();
    }
}
