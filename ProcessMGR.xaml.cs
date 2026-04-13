using MakuTweakerNew.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows.Threading;
using Windows.UI.Composition.Desktop;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace MakuTweakerNew
{
    public class ProcessItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MemoryUsage { get; set; }
        public override string ToString()
        {
            return $"{Name} ({MemoryUsage})";
        }
    }
    public partial class ProcessMGR : Page
    {
        private DispatcherTimer _timer;
        private long _dynamicMemoryThreshold = 524288000;
        bool isLoaded = false;
        private bool helpVisible = false;
        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        public ProcessMGR()
        {
            InitializeComponent();
            LoadLang();
            isLoaded = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            RefreshProcessList();
            _timer.Start();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= Timer_Tick;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void RefreshProcessList()
        {
            try
            {
                var excludedNames = new[] { "dwm", "msedgewebview2", "startmenuexperiencehost", "taskmgr", "explorer", "system", "idle", "dllhost", "smss", "csrss", "wininit", "services", "lsass", "winlogon", "svchost", "fontdrvhost", "sihost", "shellexperiencehost", "ctfmon", "runtimebroker", "searchindexer", "searchapp", "wpfsurface", "searchhost", "phoneexperiencehost", "textinputhost", "nvidia overlay", "vscodium", "lockapp", "shellhost", "systemsettings", "crossdeviceresume", "applicationframehost", "searchui", "gamebar", "xboxgamebarwidgets", "xboxpcappft", "icloudservices","nvdisplay.container", "widgets", "xboxgamebarspotify", "backgroundtaskhost", "perfwatson2", "msbuild", "crossdeviceservice", "bioenrollmenthost", "acergaicameraw", "vmtoolsd", "onedrive", "onedrive.sync.service", "igcctray", "igcc", "microsoft.cmdpal.ui", "wwahost", "onedrive.setup", "rtkuwp",
"makutweaker", "msedge", "nvcontainer", "sharex", "everything", "firefox", "chrome", "discord"};

                int? selectedId = null;
                Dispatcher.Invoke(() =>
                {
                    if (ProcessListView.SelectedItem is ProcessItem selected)
                    {
                        selectedId = selected.Id;
                    }

                    if (MemoryLimitCombo?.SelectedItem is ComboBoxItem comboItem)
                    {
                        _dynamicMemoryThreshold = long.Parse(comboItem.Tag.ToString());
                    }
                });

                bool showOnlyHung = false;
                Dispatcher.Invoke(() => showOnlyHung = OnlyNotRespondingCheck.IsChecked ?? false);

                var allProcesses = Process.GetProcesses();

                var heavyProcesses = allProcesses
                    .Where(p =>
                    {
                        try
                        {
                            if (p.Id <= 4 || p.SessionId == 0) return false;
                            if (excludedNames.Contains(p.ProcessName.ToLower())) return false;
                            if (p.WorkingSet64 <= _dynamicMemoryThreshold) return false;
                            if (showOnlyHung && p.Responding) return false;

                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    })
                    .OrderByDescending(p => p.WorkingSet64)
                    .Select(p => new ProcessItem
                    {
                        Id = p.Id,
                        Name = p.ProcessName,
                        MemoryUsage = $"{Math.Round(p.WorkingSet64 / 1024.0 / 1024.0, 2)} MB"
                    })
                    .ToList();

                Dispatcher.Invoke(() =>
                {
                    ProcessListView.ItemsSource = heavyProcesses;

                    if (selectedId.HasValue)
                    {
                        var itemToSelect = heavyProcesses.FirstOrDefault(x => x.Id == selectedId.Value);
                        if (itemToSelect != null)
                        {
                            ProcessListView.SelectedItem = itemToSelect;
                            ProcessListView.Focus();
                            Keyboard.Focus(ProcessListView);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        private void KillProcess_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessListView.SelectedItem is ProcessItem selected)
            {
                try
                {
                    var processesToKill = Process.GetProcessesByName(selected.Name);
                    foreach (var proc in processesToKill)
                    {
                        try { proc.Kill(); } catch { }
                    }
                    Task.Delay(150).ContinueWith(_ => Dispatcher.Invoke(() => RefreshProcessList()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "MakuTweaker Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            if (isLoaded) RefreshProcessList();
        }

        private void ProcessListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && ProcessListView.SelectedItem != null)
            {
                KillProcess_Click(sender, e);
            }
        }

        private void LoadLang()
        {
            var languageCode = Properties.Settings.Default.lang ?? "en";
            var pmgr = MainWindow.Localization.LoadLocalization(languageCode, "pmgr");
            var tooltips = MainWindow.Localization.LoadLocalization(languageCode, "tooltips");

            mgr_tooltip.Content = tooltips["main"]["MakuTweakerProcessMGR1"];
            label.Text = pmgr["main"]["label"];
            if (KillBtn != null) KillBtn.Content = pmgr["main"]["endprocess"];
            if (OnlyNotRespondingCheck != null) OnlyNotRespondingCheck.Content = pmgr["main"]["onlyfrozen"];

            if (MemoryLimitCombo != null && MemoryLimitCombo.Items.Count >= 5)
            {
                string[] keys = { "from50mb", "from100mb", "from300mb", "from500mb", "from1000mb", "from2000mb" };
                for (int i = 0; i < keys.Length; i++)
                {
                    if (MemoryLimitCombo.Items[i] is ComboBoxItem item)
                    {
                        item.Content = pmgr["main"][keys[i]];
                    }
                }
            }

            var contextMenu = ProcessListView.Resources["ItemContextMenu"] as System.Windows.Controls.ContextMenu;
            if (contextMenu != null)
            {
                var items = (contextMenu as System.Windows.Controls.ItemsControl).Items;

                if (items.Count >= 2)
                {
                    if (items[0] is System.Windows.Controls.MenuItem itemKill)
                        itemKill.Header = pmgr["main"]["endprocess"];

                    if (items[1] is System.Windows.Controls.MenuItem itemLoc)
                        itemLoc.Header = pmgr["main"]["location"];
                }
            }
        }

        private void OpenLocation_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessListView.SelectedItem is ProcessItem selected)
            {
                try
                {
                    var proc = Process.GetProcessById(selected.Id);
                    string filePath = proc.MainModule.FileName;
                    Process.Start("explorer.exe", $"/select, \"{filePath}\"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "MakuTweaker Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            buttontooltip.IsEnabled = false;

            helpVisible = !helpVisible;

            if (helpVisible)
            {
                var languageCode = Properties.Settings.Default.lang ?? "en";
                var tooltips = MainWindow.Localization.LoadLocalization(languageCode, "tooltips");

                HelpText.Text = tooltips["main"]["MakuTweakerProcessMGR"];

                AnimatePages(true);

                buttontooltip.Content = "←";
            }
            else
            {
                AnimatePages(false);

                buttontooltip.Content = "?";
            }

            await Task.Delay(200);
            buttontooltip.IsEnabled = true;
        }

        private void AnimatePages(bool showHelp)
        {
            double offset = ContentHost.ActualHeight;
            double duration = 0.25;

            var ease = new System.Windows.Media.Animation.CubicEase
            {
                EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut
            };

            if (showHelp)
            {
                HelpContent.Visibility = Visibility.Visible;
                MainContent.IsHitTestVisible = false;

                ControlPanel.Visibility = Visibility.Collapsed;

                var mainAnim = new DoubleAnimation
                {
                    To = -offset,
                    Duration = TimeSpan.FromSeconds(duration),
                    EasingFunction = ease
                };

                var helpAnim = new DoubleAnimation
                {
                    From = offset,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(duration),
                    EasingFunction = ease
                };

                var fadeOut = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromSeconds(duration * 0.8),
                    EasingFunction = ease
                };

                MainTransform.BeginAnimation(TranslateTransform.YProperty, mainAnim);
                MainContent.BeginAnimation(UIElement.OpacityProperty, fadeOut);

                HelpTransform.BeginAnimation(TranslateTransform.YProperty, helpAnim);
            }
            else
            {
                MainContent.IsHitTestVisible = true;
                ControlPanel.Visibility = Visibility.Visible;

                MainContent.BeginAnimation(UIElement.OpacityProperty, null);
                MainContent.Opacity = 0;

                var mainAnim = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromSeconds(duration),
                    EasingFunction = ease
                };

                var helpAnim = new DoubleAnimation
                {
                    To = offset,
                    Duration = TimeSpan.FromSeconds(duration),
                    EasingFunction = ease
                };

                helpAnim.Completed += (s, e) =>
                {
                    HelpContent.Visibility = Visibility.Collapsed;

                    var fadeIn = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.25),
                        EasingFunction = ease
                    };

                    MainContent.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                MainTransform.BeginAnimation(TranslateTransform.YProperty, mainAnim);
                HelpTransform.BeginAnimation(TranslateTransform.YProperty, helpAnim);
            }
        }
    }
}
