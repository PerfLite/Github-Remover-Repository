using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        public static bool IsEnglish { get; private set; } = true;
        private ObservableCollection<string> Repositories { get; } = new();
        private bool isEnglish = true;

        private readonly Dictionary<string, Dictionary<string, string>> translations = new()
        {
            ["ClearBtn"] = new() { ["ru"] = "Отменить выбор", ["en"] = "Clear selection" },
            ["RefreshBtn"] = new() { ["ru"] = "🔄 Обновить", ["en"] = "🔄 Refresh" },
            ["DeleteBtn"] = new() { ["ru"] = "❌ Удалить", ["en"] = "❌ Delete" },
            ["SelectRepo"] = new() { ["ru"] = "Выберите репозиторий.", ["en"] = "Select a repository." },
            ["ConfirmDelete"] = new() { ["ru"] = "Удалить репозиторий {0} безвозвратно?", ["en"] = "Delete repository {0} permanently?" },
            ["ErrorLoad"] = new() { ["ru"] = "Не удалось загрузить репозитории. Убедитесь, что GitHub CLI установлен и вы вошли в аккаунт (gh auth login).", ["en"] = "Failed to load repositories. Make sure GitHub CLI is installed and you are logged in (gh auth login)." },
            ["ErrorParse"] = new() { ["ru"] = "Ошибка разбора данных: {0}", ["en"] = "Data parsing error: {0}" }
        };

        public MainWindow()
        {
            InitializeComponent();
            IconImage.Source = LoadImageFromResources("github-64.png");
            
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyLanguage(isEnglish);
            var image = FindChildByName(CloseBtn, "CloseIcon") as System.Windows.Controls.Image;
            if (image != null) image.Source = LoadImageFromResources("shutdown_def.png");
        }

        private BitmapImage? LoadImageFromResources(string name)
        {
            try {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var names = assembly.GetManifestResourceNames();
                foreach (var n in names) System.Diagnostics.Debug.WriteLine(n);
                
                var stream = assembly.GetManifestResourceStream(name);
                if (stream == null) return null;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            } catch { 
                System.Diagnostics.Debug.WriteLine($"Error loading {name}");
                return null; 
            }
        }

        private System.Windows.DependencyObject FindChildByName(System.Windows.DependencyObject parent, string name)
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is System.Windows.FrameworkElement fe && fe.Name == name)
                    return child;
                var result = FindChildByName(child, name);
                if (result != null) return result;
            }
            return null;
        }

        private void ApplyLanguage(bool english)
        {
            var lang = english ? "en" : "ru";
            
            var langBtn = LangBtn;
            if (langBtn != null)
            {
                var langText = langBtn.Template.FindName("LangText", langBtn) as System.Windows.Controls.TextBlock;
                if (langText != null) langText.Text = english ? "EN" : "RU";
            }
            
            if (ClearBtn != null) ClearBtn.Content = translations["ClearBtn"][lang];
            
            if (RefreshBtn != null)
            {
                var refreshTemplate = RefreshBtn.Template;
                var refreshText = refreshTemplate.FindName("RefreshText", RefreshBtn) as System.Windows.Controls.TextBlock;
                if (refreshText != null) refreshText.Text = translations["RefreshBtn"][lang];
            }
            
            if (DeleteBtn != null) DeleteBtn.Content = translations["DeleteBtn"][lang];
        }

private async void LoadRepos()
        {
            Repositories.Clear();
            var output = await RunGhCommand("repo list --limit 100 --json nameWithOwner");
            var lang = isEnglish ? "en" : "ru";

            if (string.IsNullOrWhiteSpace(output))
            {
                MessageBox.Show(translations["ErrorLoad"][lang],
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(string.Format(translations["ErrorParse"][lang], ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var lang = isEnglish ? "en" : "ru";
            
            if (RepoList.SelectedItem is not string repoName)
            {
                var warnDialog = new ConfirmDialog(translations["SelectRepo"][lang], showConfirmButton: false);
                warnDialog.Owner = this;
                warnDialog.ShowDialog();
                return;
            }

            var dialog = new ConfirmDialog(string.Format(translations["ConfirmDelete"][lang], repoName));
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

        private void LangBtn_Click(object sender, RoutedEventArgs e)
        {
            isEnglish = !isEnglish;
            IsEnglish = isEnglish;
            var lang = isEnglish ? "en" : "ru";
            
            var langBtn = sender as System.Windows.Controls.Button;
            if (langBtn != null)
            {
                var template = langBtn.Template;
                var langText = template.FindName("LangText", langBtn) as System.Windows.Controls.TextBlock;
                if (langText != null) langText.Text = isEnglish ? "EN" : "RU";
            }
            
            ClearBtn.Content = translations["ClearBtn"][lang];
            
            var refreshTemplate = RefreshBtn.Template;
            var refreshText = refreshTemplate.FindName("RefreshText", RefreshBtn) as System.Windows.Controls.TextBlock;
            if (refreshText != null) refreshText.Text = translations["RefreshBtn"][lang];
            
            DeleteBtn.Content = translations["DeleteBtn"][lang];
        }
    }
}
