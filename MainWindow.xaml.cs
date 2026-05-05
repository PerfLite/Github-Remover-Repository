using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;

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
            ["RefreshBtn"] = new() { ["ru"] = "Обновить", ["en"] = "Refresh" },
            ["DeleteBtn"] = new() { ["ru"] = "Удалить", ["en"] = "Delete" },
            ["SelectRepo"] = new() { ["ru"] = "Выберите репозиторий.", ["en"] = "Select a repository." },
            ["ConfirmDelete"] = new() { ["ru"] = "Удалить репозиторий {0} безвозвратно?", ["en"] = "Delete repository {0} permanently?" },
            ["ErrorLoad"] = new() { ["ru"] = "Не удалось загрузить репозитории. Убедитесь, что GitHub CLI установлен и вы вошли в аккаунт (gh auth login).", ["en"] = "Failed to load repositories. Make sure GitHub CLI is installed and you are logged in (gh auth login)." },
            ["ErrorParse"] = new() { ["ru"] = "Ошибка разбора данных: {0}", ["en"] = "Data parsing error: {0}" }
        };

        public MainWindow()
        {
            InitializeComponent();
            IconImage.Source = LoadImage("github-64.png");
            
            Loaded += (s, e) => {
                ApplyLanguage(isEnglish);
                CloseIcon.Source = LoadImage("shutdown_def.png");
            };
            
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(7);
            dispatcherTimer.Tick += async (s, e) => {
                TitleText.Opacity = 0;
                await Task.Delay(300);
                TitleText.Text = TitleText.Text == "Cleaner" ? "hub" : "Cleaner";
                TitleText.Opacity = 1;
            };
            dispatcherTimer.Start();
            
            RepoList.ItemsSource = Repositories;
            LoadRepos();
        }

        private Bitmap? LoadImage(string name)
        {
            try {
                var uri = new Uri($"avares://GithubRemover/Assets/{name}");
                using var stream = AssetLoader.Open(uri);
                return new Bitmap(stream);
            } catch { 
                Debug.WriteLine($"Error loading {name}");
                return null; 
            }
        }

        private void ApplyLanguage(bool english)
        {
            var lang = english ? "en" : "ru";

            LangText.Text = english ? "EN" : "RU";
            ClearBtnText.Text = translations["ClearBtn"][lang];
            RefreshBtnText.Text = translations["RefreshBtn"][lang];
            DeleteBtnText.Text = translations["DeleteBtn"][lang];
        }

        private async void LoadRepos()
        {
            Repositories.Clear();
            var output = await RunGhCommand("repo list --limit 100 --json nameWithOwner");
            var lang = isEnglish ? "en" : "ru";

            if (string.IsNullOrWhiteSpace(output))
            {
                var warnDialog = new ConfirmDialog(translations["ErrorLoad"][lang], showConfirmButton: false);
                await warnDialog.ShowDialog<bool>(this);
                return;
            }

            try
            {
                using var doc = JsonDocument.Parse(output);
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    if (item.TryGetProperty("nameWithOwner", out var nameProp))
                        Repositories.Add(nameProp.GetString()!);
                }
            }
            catch (Exception ex)
            {
                var errDialog = new ConfirmDialog(string.Format(translations["ErrorParse"][lang], ex.Message), showConfirmButton: false);
                await errDialog.ShowDialog<bool>(this);
            }
        }

        private async void DeleteBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var lang = isEnglish ? "en" : "ru";
            
            if (RepoList.SelectedItem is not string repoName)
            {
                var warnDialog = new ConfirmDialog(translations["SelectRepo"][lang], showConfirmButton: false);
                await warnDialog.ShowDialog<bool>(this);
                return;
            }

            var dialog = new ConfirmDialog(string.Format(translations["ConfirmDelete"][lang], repoName));
            var result = await dialog.ShowDialog<bool>(this);
            if (!result)
                return;

            await RunGhCommand($"repo delete {repoName} --yes");
            LoadRepos();
        }

        private void RefreshBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => LoadRepos();

        private void ClearBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            RepoList.SelectedItem = null;
        }

        private void RepoList_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var element = e.Source as Control;
            while (element != null)
            {
                if (element is ListBoxItem)
                    return;
                element = element.GetVisualParent() as Control;
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

        private void DragWindow(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                BeginMoveDrag(e);
        }

        private void CloseBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();

        private void LangBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            isEnglish = !isEnglish;
            IsEnglish = isEnglish;
            ApplyLanguage(isEnglish);
        }
    }
}
