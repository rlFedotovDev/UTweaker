using Hardcodet.Wpf.TaskbarNotification;
using MakuTweakerNew.Properties;
using MicaWPF.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.UI.Composition.Desktop;

namespace MakuTweakerNew
{
    public partial class Perf : Page
    {
        bool isLoaded = false;
        public Perf()
        {
            InitializeComponent();
            LoadLang();
            this.Loaded += Perf_Loaded;
            var languageCode = Properties.Settings.Default.lang ?? "en";
            var perfor = MainWindow.Localization.LoadLocalization(languageCode, "perfor");
            isLoaded = true;
        }

        private (int ExitCode, string Output, string Error) RunPowerCfg(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powercfg",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return (process.ExitCode, output, error);
        }

        private void LoadLang()
        {
            var languageCode = Properties.Settings.Default.lang ?? "en";
            var perfor = MainWindow.Localization.LoadLocalization(languageCode, "perfor");
            label.Text = perfor["main"]["label"];
            apply.Content = perfor["main"]["applyb"];
            minpercent.Content = perfor["main"]["minb"];
            maxpercent.Content = perfor["main"]["maxb"];
            infolabel.Text = perfor["main"]["info"];
        }

        private void ApplyThrottle(int percent)
        {
            if (percent < 1 || percent > 100)
                return;

            string scheme = GetActiveScheme();

            var r1 = RunPowerCfg($"/setdcvalueindex {scheme} SUB_PROCESSOR PROCTHROTTLEMAX {percent}");
            var r2 = RunPowerCfg($"/setacvalueindex {scheme} SUB_PROCESSOR PROCTHROTTLEMAX {percent}");
            var r3 = RunPowerCfg($"/setactive {scheme}");

            if (r1.ExitCode == 0 && r2.ExitCode == 0 && r3.ExitCode == 0)
            {
                percentslider.Value = percent / 10.0;
                ShowThrottleNotification(percent);
            }
            else
            {
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"{r1.Error}\n{r2.Error}\n{r3.Error}", "MakuTweaker Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            int percent = (int)percentslider.Value * 10;
            ApplyThrottle(percent);
        }



        private void Perf_Loaded(object sender, RoutedEventArgs e)
        {
            RunPowerCfg("-attributes SUB_PROCESSOR PROCTHROTTLEMAX -ATTRIB_HIDE");

            int percent = GetCurrentThrottlePercent();

            if (percent >= 1 && percent <= 100)
                percentslider.Value = percent / 10.0;
            else
                percentslider.Value = 10;
        }

        private string GetActiveScheme()
        {
            var result = RunPowerCfg("/getactivescheme");

            if (result.ExitCode != 0)
                return "SCHEME_CURRENT";

            Match match = Regex.Match(result.Output, @"GUID:\s+([a-fA-F0-9\-]+)");

            return match.Success ? match.Groups[1].Value : "SCHEME_CURRENT";
        }

        private const string SUB_PROCESSOR_GUID = "54533251-82be-4824-96c1-47b60b740d00";
        private const string PROCTHROTTLEMAX_GUID = "bc5038f7-23e0-4960-96da-33abaf5935ec";
        private int GetCurrentThrottlePercent()
        {
            string scheme = GetActiveScheme();

            var result = RunPowerCfg(
                $"/query {scheme} {SUB_PROCESSOR_GUID} {PROCTHROTTLEMAX_GUID}"
            );

            if (result.ExitCode != 0)
                return -1;

            var matches = Regex.Matches(result.Output, @"0x([0-9A-Fa-f]+)");

            if (matches.Count == 0)
                return -1;

            string hex = matches[matches.Count - 1].Groups[1].Value;

            return Convert.ToInt32(hex, 16);
        }

        private void minpercent_Click(object sender, RoutedEventArgs e)
        {
            ApplyThrottle(10);
        }

        private void maxpercent_Click(object sender, RoutedEventArgs e)
        {
            ApplyThrottle(100);
        }

        private Stream GetResourceStream(string relativePath)
        {
            var uri = new Uri($"pack://application:,,,/{relativePath}", UriKind.Absolute);
            var resourceInfo = Application.GetResourceStream(uri);

            if (resourceInfo == null)
                throw new FileNotFoundException($"Ресурс {relativePath} не найден.");

            return resourceInfo.Stream;
        }

        private void ShowThrottleNotification(int percent)
        {
            var languageCode = Properties.Settings.Default.lang ?? "en";
            var perfor = MainWindow.Localization.LoadLocalization(languageCode, "perfor");

            string baseText = perfor["main"]["flyout"];
            string message = $"{baseText}{percent}%";

            Icon trayIcon = new Icon(GetResourceStream("assets/icons/MakuT.ico"));

            TaskbarIcon tray = new TaskbarIcon
            {
                ToolTipText = "MakuTweaker",
                Icon = trayIcon
            };

            tray.ShowBalloonTip("MakuTweaker", message, BalloonIcon.Info);

            Task.Delay(8000).ContinueWith(t =>
            {
                tray.Dispatcher.Invoke(() => tray.Dispose());
            });
        }
    }
}
