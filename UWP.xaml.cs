using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Media;
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
using Microsoft.Win32;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Windows.UI.Composition.Desktop;

namespace MakuTweakerNew
{
    public partial class UWP : System.Windows.Controls.Page
    {
        //Эту вкладку я тоже планирую обновить внешне, и изнутри.
        //I plan to update this tab as well—both visually and internally.

        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        private int mode;
        private bool _isChecking;
        private bool _progressShown;

        public  UWP()
        {
            InitializeComponent();
            LoadLang();
            Loaded += async (_, __) =>
            {
                _isChecking = true;

                var delayTask = Task.Delay(2000);
                var checkTask = CheckInstalledUWPAppsAsync(true);

                var finishedTask = await Task.WhenAny(delayTask, checkTask);

                if (finishedTask == delayTask && _isChecking)
                {
                    ShowProgressSmooth();
                }

                await checkTask;
                if (_progressShown)
                    HideProgressSmooth();
            };
        }

        private void ShowProgressSmooth()
        {
            _progressShown = true;

            p.Visibility = Visibility.Visible;
            t.Visibility = Visibility.Visible;

            p.Opacity = 0;
            t.Opacity = 0;

            FadeIn(p, 400);
            FadeIn(t, 400);

            p.IsIndeterminate = true;
        }

        private void HideProgressSmooth()
        {
            FadeOut(p, 300);
            FadeOut(t, 300);

            _progressShown = false;
        }

        private async Task CheckInstalledUWPAppsAsync(bool anim)
        {
            var languageCode = Properties.Settings.Default.lang ?? "en";
            var uwp = MainWindow.Localization.LoadLocalization(languageCode, "uwp");

            t.Text = uwp["main"]["chk"];
            p.Value = 0;

            string[] appIds =
            {
        "Microsoft.ZuneVideo",
        "Microsoft.ZuneMusic",
        "Microsoft.MicrosoftStickyNotes",
        "Microsoft.MixedReality.Portal",
        "Microsoft.MicrosoftSolitaireCollection",
        "Microsoft.Messaging",
        "Microsoft.WindowsFeedbackHub",
        "microsoft.windowscommunicationsapps",
        "Microsoft.BingNews",
        "Microsoft.Microsoft3DViewer",
        "Microsoft.BingWeather",
        "Microsoft.549981C3F5F10",
        "Microsoft.XboxApp",
        "Microsoft.GetHelp",
        "Microsoft.WindowsCamera",
        "Microsoft.WindowsMaps",
        "Microsoft.Office.OneNote",
        "Microsoft.YourPhone",
        "Microsoft.Windows.DevHome",
        "Clipchamp.Clipchamp",
        "Microsoft.PowerAutomateDesktop",
        "Microsoft.Getstarted",
        "Microsoft.WindowsSoundRecorder",
        "Microsoft.WindowsStore",
        "Microsoft.People",
        "Microsoft.SkypeApp",
        "Microsoft.WindowsAlarms",
        "Microsoft.OutlookForWindows"
    };

            var installedApps = await GetInstalledUWPAppsAsync();
            foreach (var appId in appIds)
            {
                bool isInstalled = installedApps.Contains(appId);

                if (ToggleMap.TryGetValue(appId, out var mappedToggle))
                {
                    mappedToggle.IsEnabled = isInstalled;

                    if (mappedToggle != u9 && mappedToggle != u15 && mappedToggle != u16 && mappedToggle != u17)
                        mappedToggle.IsOn = isInstalled;
                }

                t.Text = isInstalled
                    ? $"{uwp["main"]["chk"]}{appId} {uwp["main"]["is"]}"
                    : $"{uwp["main"]["chk"]}{appId} {uwp["main"]["isnt"]}";

                p.Value++;
                u23.IsEnabled = true;
                u24.IsEnabled = true;
            }

            t.Text = uwp["main"]["comp"];

            if (anim)
            {
                FadeInBloat();
                FadeOutAll1();
            }

            b.IsEnabled = true;
            _isChecking = false;
            p.IsIndeterminate = false;
        }

        private List<ToggleSwitch> AllToggles => new()
        {
            u1,u2,u3,u4,u5,u6,u7,u8,u9,u10,
            u11,u12,u13,u14,u15,u16,u17,u18,
            u19,u20,u21,u22,u23,u24,u25,u26,u27,u28
        };

        private Dictionary<string, ToggleSwitch> ToggleMap => new()
        {
            ["Microsoft.MixedReality.Portal"] = u1,
            ["Microsoft.MicrosoftSolitaireCollection"] = u2,
            ["Microsoft.Messaging"] = u3,
            ["Microsoft.549981C3F5F10"] = u4,
            ["Microsoft.GetHelp"] = u5,
            ["Microsoft.WindowsFeedbackHub"] = u6,
            ["Microsoft.Windows.DevHome"] = u7,
            ["Microsoft.Microsoft3DViewer"] = u8,
            ["Microsoft.YourPhone"] = u9,
            ["Microsoft.WindowsMaps"] = u10,
            ["Microsoft.PowerAutomateDesktop"] = u11,
            ["Clipchamp.Clipchamp"] = u12,
            ["microsoft.windowscommunicationsapps"] = u13,
            ["Microsoft.Office.OneNote"] = u14,
            ["Microsoft.ZuneMusic"] = u15,
            ["Microsoft.ZuneVideo"] = u16,
            ["Microsoft.WindowsCamera"] = u17,
            ["Microsoft.BingNews"] = u18,
            ["Microsoft.BingWeather"] = u19,
            ["Microsoft.MicrosoftStickyNotes"] = u20,
            ["Microsoft.Getstarted"] = u21,
            ["Microsoft.WindowsSoundRecorder"] = u22,
            ["Microsoft.People"] = u25,
            ["Microsoft.SkypeApp"] = u26,
            ["Microsoft.WindowsAlarms"] = u27,
            ["Microsoft.OutlookForWindows"] = u28
        };

        private async Task<HashSet<string>> GetInstalledUWPAppsAsync()
        {
            return await Task.Run(() =>
            {
                var result = new HashSet<string>();

                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = "-Command \"Get-AppxPackage | Select -ExpandProperty Name\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(psi);
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                        result.Add(line.Trim());
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"PowerShell error: {ex.Message}")
                    );
                }

                return result;
            });
        }

        private async Task RemovePackageAsync(string packageName)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"Get-AppxPackage -Name '{packageName}' | Remove-AppxPackage\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            await process.WaitForExitAsync();
        }

        private void FadeIn(UIElement element, double durationSeconds)
        {
            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(durationSeconds),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            element.BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        private void FadeOut(UIElement element, double durationSeconds)
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(durationSeconds),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            element.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private List<UIElement> PopularElements => new() { u15, u16, u17, u9 };
        private void FadeInPopular()
        {
            foreach (var el in PopularElements)
            {
                el.Visibility = Visibility.Visible;
                FadeIn(el, 300);
            }
        }

        private async Task FadeOutPopularAsync()
        {
            foreach (var el in PopularElements)
                FadeOut(el, 300);

            await Task.Delay(300);

            foreach (var el in PopularElements)
                el.Visibility = Visibility.Collapsed;
        }

        private void FadeInAll1()
        {
            foreach (var toggle in AllToggles)
                FadeIn(toggle, 300);

            FadeIn(b, 300);
            FadeIn(view, 300);
        }

        private async void FadeOutAll1()
        {
            FadeOut(p, 300);
            FadeOut(t, 300);
            await Task.Delay(300);
        }
        private void FadeInAll2()
        {
            FadeIn(p, 300);
            FadeIn(t, 300);
        }

        private void FadeOutAll2()
        {
            foreach (var toggle in AllToggles)
            {
                FadeOut(toggle, 300);
                toggle.IsEnabled = false;
            }

            FadeOut(b, 300);
            FadeOut(view, 300);
            b.IsEnabled = false;
            view.IsEnabled = false;
        }

        private async void b_Click(object sender, RoutedEventArgs e)
        {
            var languageCode = Properties.Settings.Default.lang ?? "en";
            var uwp = MainWindow.Localization.LoadLocalization(languageCode, "uwp");

            int count = 0;

            foreach (var toggle in AllToggles)
            {
                if (toggle.IsOn)
                {
                    if (toggle == u8)       
                        count += 3;
                    else if(toggle == u23)
                    {
                        ILOVEMAKUTWEAKERDialog dialog = new ILOVEMAKUTWEAKERDialog("Microsoft Store");
                        var result = await dialog.ShowAsync();
                        int resulty = await dialog.TaskCompletionSource.Task;
                        if(resulty == 0)
                        {
                            return;
                        }
                        else
                        {
                            count++;
                        }
                    }
                    else if (toggle == u24)
                    {
                        ILOVEMAKUTWEAKERDialog dialog = new ILOVEMAKUTWEAKERDialog("Xbox");
                        var result = await dialog.ShowAsync();
                        int resulty = await dialog.TaskCompletionSource.Task;
                        if (resulty == 0)
                        {
                            return;
                        }
                        else
                        {
                            count += 5;
                        }
                    }
                    else if (toggle == u25)       
                        count += 2;
                    else
                        count++;       
                }
            }

            if (count > 0)
            {
                if (!_progressShown)
                {
                    ShowProgressSmooth();
                }
                p.IsIndeterminate = false;
                t.Text = $"{uwp["status"]["started"]} 0/{count}";
                mw.NavigationView_Root.IsEnabled = false;
                mw.ABCB.IsEnabled = false;
                p.Maximum = count;
                p.Value = 0;
                FadeInAll2();
                switch (mode)
                {
                    case 0:
                        FadeOutBloat();
                        break;
                    case 1:
                        FadeOutBloat();
                        await FadeOutPopularAsync();
                        break;
                    case 2:
                        FadeInAll1();
                        FadeOutAll2();
                        break;
                        
                }
                await Task.Delay(300);
                b.Visibility = Visibility.Collapsed;

                var appPackages = new (ToggleSwitch toggle, string packageName)[]
                {
            (u1, "Microsoft.MixedReality.Portal"),
            (u2, "Microsoft.MicrosoftSolitaireCollection"),
            (u3, "Microsoft.Messaging"),
            (u4, "Microsoft.549981C3F5F10"),
            (u5, "Microsoft.GetHelp"),
            (u6, "Microsoft.WindowsFeedbackHub"),
            (u7, "Microsoft.Windows.DevHome"),
            (u8, "Microsoft.MSPaint"),
            (u8, "Microsoft.3DBuilder"),
            (u8, "Microsoft.Microsoft3DViewer"),
            (u9, "Microsoft.YourPhone"),
            (u10, "Microsoft.WindowsMaps"),
            (u11, "Microsoft.PowerAutomateDesktop"),
            (u12, "Clipchamp.Clipchamp"),
            (u13, "microsoft.windowscommunicationsapps"),
            (u14, "Microsoft.Office.OneNote"),
            (u15, "Microsoft.ZuneMusic"),
            (u16, "Microsoft.ZuneVideo"),
            (u17, "Microsoft.WindowsCamera"),
            (u18, "Microsoft.BingNews"),
            (u19, "Microsoft.BingWeather"),
            (u20, "Microsoft.MicrosoftStickyNotes"),
            (u21, "Microsoft.Getstarted"),
            (u22, "Microsoft.WindowsSoundRecorder"),
            (u23, "Microsoft.WindowsStore"),
            (u24, "Microsoft.XboxApp"),
            (u24, "Microsoft.GamingApp"),
            (u24, "Microsoft.Xbox.TCUI"),
            (u24, "Microsoft.XboxSpeechToTextOverlay"),
            (u24, "Microsoft.XboxGameCallableUI"),
            (u25, "Microsoft.People"),
            (u25, "Microsoft.Windows.PeopleExperienceHost"),
            (u26, "Microsoft.SkypeApp"),
            (u27, "Microsoft.WindowsAlarms"),
            (u28, "Microsoft.OutlookForWindows"),
                };

                foreach (var (toggle, packageName) in appPackages)
                {
                    if (toggle.IsOn)
                    {
                        await RemovePackageAsync(packageName);
                        p.Value++;
                        t.Text = $"{uwp["status"]["started"]} {p.Value}/{count}";
                    }
                }
                p.Value = 0;
                p.Maximum = 27;
                view.SelectedIndex = 0;
                FadeInBloat();
                FadeOutAll1();
                SystemSounds.Asterisk.Play();
                mw.ChSt(uwp["status"]["complete"]);
                mw.NavigationView_Root.IsEnabled = true;
                mw.ABCB.IsEnabled = true;
                foreach (var toggle in AllToggles)
                {
                    toggle.IsOn = false;
                    toggle.IsEnabled = false;
                }
                b.IsEnabled = false;
                b.Visibility = Visibility.Visible;
                CheckInstalledUWPAppsAsync(false);
            }
            else
            {
                mw.ChSt(uwp["status"]["noapps"]);
            }
        }

        private void FadeInNecessary()
        {
            u23.Visibility = Visibility.Visible;
            u24.Visibility = Visibility.Visible;
            FadeIn(u23, 300);
            FadeIn(u24, 300);
        }

        private async Task FadeOutNecessary()
        {
            FadeOut(u23, 300);
            FadeOut(u24, 300);
            await Task.Delay(300);
            u23.Visibility = Visibility.Collapsed;
            u24.Visibility = Visibility.Collapsed;
        }

        private void FadeInBloat()
        {
            FadeIn(u1, 300);
            FadeIn(u2, 300);
            FadeIn(u3, 300);
            FadeIn(u4, 300);
            FadeIn(u5, 300);
            FadeIn(u6, 300);
            FadeIn(u7, 300);
            FadeIn(u8, 300);
            FadeIn(u10, 300);
            FadeIn(u11, 300);
            FadeIn(u12, 300);
            FadeIn(u13, 300);
            FadeIn(u14, 300);
            FadeIn(u18, 300);
            FadeIn(u19, 300);
            FadeIn(u20, 300);
            FadeIn(u21, 300);
            FadeIn(u22, 300);
            FadeIn(u25, 300);
            FadeIn(u26, 300);
            FadeIn(u27, 300);
            FadeIn(u28, 300);
            FadeIn(b, 300);
            FadeIn(view, 300);
        }
        private void FadeOutBloat()
        {
            FadeOut(u1, 300);
            FadeOut(u2, 300);
            FadeOut(u3, 300);
            FadeOut(u4, 300);
            FadeOut(u5, 300);
            FadeOut(u6, 300);
            FadeOut(u7, 300);
            FadeOut(u8, 300);
            FadeOut(u10, 300);
            FadeOut(u11, 300);
            FadeOut(u12, 300);
            FadeOut(u13, 300);
            FadeOut(u14, 300);
            FadeOut(u18, 300);
            FadeOut(u19, 300);
            FadeOut(u20, 300);
            FadeOut(u21, 300);
            FadeOut(u22, 300);
            FadeOut(u25, 300);
            FadeOut(u26, 300);
            FadeOut(u27, 300);
            FadeOut(u28, 300);
            FadeOut(b, 300);
            FadeOut(view, 300);
        }

        private async void view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (view.SelectedIndex)
            {
                case 0:
                    if(mode == 1)
                    {
                        await FadeOutPopularAsync();
                    }
                    if (mode == 2)
                    {
                        await FadeOutPopularAsync();
                        FadeOutNecessary();
                    }
                    mode = 0;
                    view.IsEnabled = false;
                    await Task.Delay(300);
                    view.IsEnabled = true;
                    break;
                case 1:
                    if(mode == 0)
                    {
                        FadeInPopular();
                    }
                    else if(mode == 2)
                    {
                        FadeOutNecessary();
                    }
                    mode = 1;
                    view.IsEnabled = false;
                    await Task.Delay(300);
                    view.IsEnabled = true;
                    break;
                case 2:
                    if(mode == 0)
                    {
                        FadeInNecessary();
                        FadeInPopular();
                    }
                    else if(mode == 1)
                    {
                        FadeInNecessary();
                    }
                    mode = 2;
                    view.IsEnabled = false;
                    await Task.Delay(300);
                    view.IsEnabled = true;
                    break;
            }
        }
        private void LoadLang()
        {
            var languageCode = Properties.Settings.Default.lang ?? "en";
            var uwp = MainWindow.Localization.LoadLocalization(languageCode, "uwp");
            label.Text = uwp["main"]["label"];
            info1.Text = uwp["main"]["info1"];
            info2.Text = uwp["main"]["info2"];

            u3.OffContent = uwp["main"]["u3"];
            u5.OffContent = uwp["main"]["u5"];
            u6.OffContent = uwp["main"]["u6"];
            u9.OffContent = uwp["main"]["u9"];
            u10.OffContent = uwp["main"]["u10"];
            u13.OffContent = uwp["main"]["u13"];
            u15.OffContent = uwp["main"]["u15"];
            u16.OffContent = uwp["main"]["u16"];
            u17.OffContent = uwp["main"]["u17"];
            u18.OffContent = uwp["main"]["u18"];
            u19.OffContent = uwp["main"]["u19"];
            u20.OffContent = uwp["main"]["u20"];
            u22.OffContent = uwp["main"]["u22"];
            u27.OffContent = uwp["main"]["u27"];

            u3.OnContent = uwp["main"]["u3"];
            u5.OnContent = uwp["main"]["u5"];
            u6.OnContent = uwp["main"]["u6"];
            u9.OnContent = uwp["main"]["u9"];
            u10.OnContent = uwp["main"]["u10"];
            u13.OnContent = uwp["main"]["u13"];
            u15.OnContent = uwp["main"]["u15"];
            u16.OnContent = uwp["main"]["u16"];
            u17.OnContent = uwp["main"]["u17"];
            u18.OnContent = uwp["main"]["u18"];
            u19.OnContent = uwp["main"]["u19"];
            u20.OnContent = uwp["main"]["u20"];
            u22.OnContent = uwp["main"]["u22"];
            u27.OnContent = uwp["main"]["u27"];

            mode1.Content = uwp["main"]["mode1"];
            mode2.Content = uwp["main"]["mode2"];
            mode3.Content = uwp["main"]["mode3"];

            b.Content = uwp["main"]["b"];
            t.Text = uwp["main"]["chk"];
        }
    }
}
