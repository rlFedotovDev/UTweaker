using MakuTweakerNew.Properties;
using MicaWPF.Core.Enums;
using MicaWPF.Core.Services;
using Microsoft.Win32;
using ModernWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MakuTweakerNew
{
    public partial class SettingsAbout : Page
    {
        private MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;
        bool isLoaded = false;

        private readonly Dictionary<string, (string Label, string Name)> _translators = new()
        {
            ["cs"] = ("Pomohl s lokalizací:", "qCLairvoyant"),
            ["de"] = ("Hilfe bei der Lokalisierung:", "Scorazio"),
            ["pl"] = ("Pomoc w lokalizacji:", "dfa_jk"),
            ["et"] = ("Aitas lokaliseerimisega:", "KirTeanEesti")
        };

        public SettingsAbout()
        {
            InitializeComponent();
            credN.Text = "Mark Adderly\nNikitori\nNikitori, Massgrave";
            if (string.IsNullOrEmpty(Settings.Default.lang))
            {
                string systemLang = CultureInfo.CurrentUICulture.Name.ToLower();
                string isoLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
                Settings.Default.lang = systemLang switch
                {
                    "zh-tw" or "zh-hk" or "zh-mo" => "tw",
                    "zh-cn" or "zh-sg" or "zh-chs" => "zh",

                    _ => isoLang switch
                    {
                        "uk" => "uk",   // Украинский
                        "cs" => "cs",   // Чешский
                        "ru" => "ru",   // Русский
                        "az" => "az",   // Азербайджанский
                        "es" => "es",   // Испанский
                        "tl" => "tl",   // Тагальский
                        "fil" => "tl",  // Филиппинский
                        "tr" => "tr",   // Турецкий
                        "ko" => "ko",   // Корейский
                        "zh" => "zh",   // Китайский
                        "it" => "it",   // Итальянский
                        "de" => "de",   // Немецкий
                        "fr" => "fr",   // Французский
                        "be" => "be",   // Белорусский
                        "vi" => "vi",   // Вьетнамский
                        "id" => "id",   // Индонезийский
                        "hi" => "hi",   // Хинди
                        "ja" => "ja",   // Японский
                        "kk" => "kk",   // Казахский
                        "pt" => "pt",   // Португальский
                        "lv" => "lv",   // Латвийский
                        "fi" => "fi",   // Финский
                        "et" => "et",   // Эстонский
                        "pl" => "pl",   // Польский
                        "th" => "th",   // Тайский
                        _ => "en"       // Стандартный (Английский)
                    }
                };

                Settings.Default.Save();
            }

            foreach (ComboBoxItem item in lang.Items)
            {
                if (item.Tag?.ToString() == Settings.Default.lang)
                {
                    lang.SelectedItem = item;
                    break;
                }
            }

            if (lang.SelectedIndex == -1)
            {
                lang.SelectedIndex = 0;
            }

            Settings.Default.langSI = lang.SelectedIndex;

            if (checkWinVer() < 22000)
            {
                style.Visibility = Visibility.Collapsed;
                styleL.Visibility = Visibility.Collapsed;
            }

            var currentTheme = MicaWPFServiceUtility.ThemeService.CurrentTheme;
            theme.SelectedIndex = currentTheme == WindowsTheme.Dark ? 1 : 0;

            style.SelectedIndex = Settings.Default.style switch
            {
                "Mica" => 0,
                "Tabbed" => 1,
                "Acrylic" => 2,
                "None" => 3,
                _ => 0
            };

            relang();
            UpdateLocalizationCredits();
            isLoaded = true;
        }

        private int checkWinVer()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                var value = key?.GetValue("CurrentBuild");
                return (value != null && int.TryParse(value.ToString(), out int build)) ? build : 19045;
            }
            catch { return 19045; }
        }

        private void OpenUrl(string url) => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

        #region Обработчики событий

        private void Button_Click(object sender, RoutedEventArgs e) => OpenUrl("https://adderly.top");

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => OpenUrl("https://boosty.to/adderly");

        private void Image_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e) => OpenUrl("https://t.me/adderly324");

        private void Image_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e) => OpenUrl("https://youtube.com/@MakuAdarii");

        private void theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded) return;

            bool isDark = theme.SelectedIndex == 1;
            Settings.Default.theme = isDark ? "Dark" : "Light";

            MicaWPFServiceUtility.ThemeService.ChangeTheme(isDark ? WindowsTheme.Dark : WindowsTheme.Light);
            ThemeManager.Current.ApplicationTheme = isDark ? ApplicationTheme.Dark : ApplicationTheme.Light;

            Brush color = isDark ? Brushes.White : Brushes.Black;
            mw.Foreground = color;
            mw.Separator.Stroke = color;

            Settings.Default.Save();
        }

        private void lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded) return;
            if (lang.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                Settings.Default.lang = selectedItem.Tag.ToString();
            }

            Settings.Default.langSI = lang.SelectedIndex;
            Settings.Default.Save();

            mw.LoadLang(Settings.Default.lang);
            relang();
            UpdateLocalizationCredits();
        }

        private void style_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded) return;
            var styleData = style.SelectedIndex switch
            {
                0 => (Type: BackdropType.Mica, Name: "Mica"),
                1 => (Type: BackdropType.Tabbed, Name: "Tabbed"),
                2 => (Type: BackdropType.Acrylic, Name: "Acrylic"),
                _ => (Type: BackdropType.None, Name: "None")
            };

            MicaWPFServiceUtility.ThemeService.EnableBackdrop(mw, styleData.Type);
            Settings.Default.style = styleData.Name;
            Settings.Default.Save();
        }

        #endregion

        private void relang()
        {
            var languageCode = Settings.Default.lang ?? "en";
            var ab = MainWindow.Localization.LoadLocalization(languageCode, "ab");
            var b = MainWindow.Localization.LoadLocalization(languageCode, "base");

            credL.Text = ab["main"]["credL"];
            label.Text = ab["main"]["label"];
            web.Content = ab["main"]["atop"];
            langL.Text = ab["main"]["lang"];
            styleL.Text = ab["main"]["st"];
            themeL.Text = ab["main"]["th"];
            l.Content = " " + ab["main"]["l"];
            d.Content = " " + ab["main"]["d"];
            off.Content = " " + b["def"]["off"];
        }

        private void UpdateLocalizationCredits()
        {
            string currentLang = Settings.Default.lang ?? "en";

            if (_translators.TryGetValue(currentLang, out var credits))
            {
                credLang.Visibility = Visibility.Visible;
                credLangtext.Visibility = Visibility.Visible;
                credLang.Text = credits.Label;
                credLangtext.Text = credits.Name;
            }
            else
            {
                credLang.Visibility = Visibility.Collapsed;
                credLangtext.Visibility = Visibility.Collapsed;
            }
        }
    }
}