using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using MakuTweakerNew.Properties;
using MicaWPF.Controls;
using Microsoft.Win32;

namespace MakuTweakerNew
{
    //Это старейшая страница в коде MakuTweaker. Я давно планирую отказаться от вкладки "Контекстное меню"...
    //потому что это самая невостребованная вкладка.
    //Здесь очень страшный код, потому что он со времён моих начинаний в C#.
    //Скорее всего - в MakuTweaker 5.4, как раз эта вкладка и весь этот код будет удалён, и заменён на что то новое.

    //This is the oldest page in the MakuTweaker source code.
    //I have long planned to retire the "Context Menu" tab,
    //as it is the least-used tab. The code here is truly dreadful,
    //dating back to my very beginnings with C#.
    //Most likely, in MakuTweaker 5.4, this specific tab—along with all this code—will be removed and replaced with something new.

    public partial class ContextMenu : Page
    {
        private MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;
        bool isLoaded = false;
        public ContextMenu()
        {
            InitializeComponent();
            checkReg();
            LoadLang();
            if(checkWinVer() < 22000)
            {
                t15.Visibility = Visibility.Collapsed;
                t13.Visibility = Visibility.Collapsed;
            }
            isLoaded = true;
        }
        private int checkWinVer()
        {
            string keyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            string valueName = "CurrentBuild";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
            {
                if (key != null)
                {
                    object value = key.GetValue(valueName);

                    if (value != null && int.TryParse(value.ToString(), out int build))
                    {
                        return build;
                    }
                }
            }
            return 19045;
        }

        private void t1_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch (t1.IsOn)
                {
                    case true:
                        Registry.CurrentUser.CreateSubKey(@"Control Panel\Desktop").SetValue("MenuShowDelay", "50");
                        break;
                    case false:
                        Registry.CurrentUser.CreateSubKey(@"Control Panel\Desktop").SetValue("MenuShowDelay", "400");
                        break;
                }
                mw.RebootNotify(2);
            }
        }

        private void t3_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch(t3.IsOn)
                {
                    case true:
                        try
                        {
                            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked").SetValue("{9F156763-7844-4DC4-B2B1-901F640F5155}", "");
                        }
                        catch
                        {

                        }
                        break;
                    case false:
                        {
                            try
                            {
                                Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked");
                            }
                            catch
                            {

                            }
                        }
                        break;
                }
                mw.RebootNotify(2);
            }
        }

        private void t5_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch (t5.IsOn)
                {
                    case true:
                        try
                        {
                            Registry.ClassesRoot.DeleteSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers\ModernSharing");
                        }
                        catch
                        {

                        }
                    break;
                    case false:
                        Registry.ClassesRoot.CreateSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers\ModernSharing").SetValue("", "{e2bf9676-5f8f-435c-97eb-11607a5bedf7}");
                        break;
                }
            }
        }

        private void t6_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch (t6.IsOn)
                {
                    case true:
                        try
                        {
                            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked").SetValue("{596AB062-B4D2-4215-9F74-E9109B0A8153}", "");
                        }
                        catch
                        {

                        }
                        break;
                    case false:
                        try
                        {
                            Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked");
                        }
                        catch
                        {

                        }
                        break;
                }
                mw.RebootNotify(2);
            }
        }

        private void t8_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch (t8.IsOn)
                {
                    case true:
                        try
                        {
                            Registry.ClassesRoot.CreateSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers\SendTo").SetValue("", "");
                        }
                        catch
                        {

                        }
                        break;
                    case false:
                        {
                            Registry.ClassesRoot.CreateSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers\SendTo").SetValue("", "{7BA4C740-9E81-11CF-99D3-00AA004AE837}");
                            break;
                        }
                }
                mw.RebootNotify(2);
            }
        }

        private void t10_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch (t10.IsOn)
                {
                    case true:
                        try
                        {
                            Registry.ClassesRoot.DeleteSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers\CopyAsPathMenu");
                        }
                        catch
                        {

                        }
                        break;
                    case false:
                        {
                            Registry.ClassesRoot.CreateSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers\CopyAsPathMenu").SetValue("", "{f3d06e7c-1e45-4a26-847e-f9fcdee59be0}");
                            break;
                        }
                }
            }
        }

        private void t11_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch (t11.IsOn)
                {
                    case true:
                        try
                        {
                            Registry.LocalMachine.DeleteSubKey(@"SOFTWARE\Classes\Folder\shellex\ContextMenuHandlers\PintoStartScreen");
                            Registry.ClassesRoot.CreateSubKey(@"exefile\shellex\ContextMenuHandlers\PintoStartScreen").SetValue("", "");
                        }
                        catch
                        {

                        }
                        break;
                    case false:
                        Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\Folder\shellex\ContextMenuHandlers\PintoStartScreen").SetValue("", "{470C0EBD-5D73-4d58-9CED-E91E22E23282}");
                        Registry.ClassesRoot.CreateSubKey(@"exefile\shellex\ContextMenuHandlers\PintoStartScreen").SetValue("", "{470C0EBD-5D73-4d58-9CED-E91E22E23282}");
                        break;
                }
            }
        }

        private void t12_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch(t12.IsOn)
                {
                    case true:
                        try
                        {
                            Registry.ClassesRoot.DeleteSubKey(@"*\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}");
                        }
                        catch
                        {

                        }
                        break;
                    case false:
                        {
                            Registry.ClassesRoot.CreateSubKey(@"*\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}").SetValue("", "Taskband Pin");
                        }
                        break;
                }
            }
        }

        private void t13_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch(t13.IsOn)
                {
                    case true:
                        try
                        {
                            Registry.ClassesRoot.DeleteSubKeyTree(@"Folder\shell\opennewtab");
                        }
                        catch
                        {

                        }
                        break;
                    case false:
                        {
                            Registry.ClassesRoot.CreateSubKey(@"Folder\shell\opennewtab").SetValue("CommandStateHandler", "{11dbb47c-a525-400b-9e80-a54615a090c0}");
                            Registry.ClassesRoot.CreateSubKey(@"Folder\shell\opennewtab").SetValue("CommandStateSync", "");
                            Registry.ClassesRoot.CreateSubKey(@"Folder\shell\opennewtab").SetValue("LaunchExplorerFlags", 32);
                            Registry.ClassesRoot.CreateSubKey(@"Folder\shell\opennewtab").SetValue("MUIVerb", "@windows.storage.dll,-8519");
                            Registry.ClassesRoot.CreateSubKey(@"Folder\shell\opennewtab").SetValue("MultiSelectModel", "Document");
                            Registry.ClassesRoot.CreateSubKey(@"Folder\shell\opennewtab").SetValue("OnlyInBrowserWindow", "");
                            Registry.ClassesRoot.CreateSubKey(@"Folder\shell\opennewtab\command").SetValue("DelegateExecute", "{11dbb47c-a525-400b-9e80-a54615a090c0}");
                            break;
                        }
                }
            }
        }

        private void t14_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch (t14.IsOn)
                {
                    case true:
                        try
                        {
                            Registry.CurrentUser.CreateSubKey(@"Software\NVIDIA Corporation\Global\NvCplApi\Policies").SetValue("ContextUIPolicy", 0);
                        }
                        catch
                        {

                        }
                        break;
                    case false:
                        {
                            try
                            {
                                Registry.CurrentUser.CreateSubKey(@"Software\NVIDIA Corporation\Global\NvCplApi\Policies").SetValue("ContextUIPolicy", 2);
                            }
                            catch
                            {

                            }
                            break;
                        }
                }
            }
        }

        private void t15_Toggled(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                switch (t15.IsOn)
                {
                    case true:
                        Process.Start("cmd.exe", "/c \"reg.exe add \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\" /f /ve\"");
                        Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel").SetValue("{20D04FE0-3AEA-1069-A2D8-08002B30309D}", 0);
                        break;
                    case false:
                        {
                            Process.Start("cmd.exe", "/c \"reg delete \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\" /f\"");
                            Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel").SetValue("{20D04FE0-3AEA-1069-A2D8-08002B30309D}", 0);
                            break;
                        }
                }
                mw.RebootNotify(2);
            }
        }

        private void LoadLang()
        {
            var languageCode = Properties.Settings.Default.lang ?? "en";
            var cm = MainWindow.Localization.LoadLocalization(languageCode, "cm");
            var basel = MainWindow.Localization.LoadLocalization(languageCode, "base");

            label.Text = cm["main"]["label"];
            t15.Header = cm["main"]["t1"];
            t1.Header = cm["main"]["t2"];
            t3.Header = cm["main"]["t3"];
            t5.Header = cm["main"]["t5"];
            t6.Header = cm["main"]["t6"];
            t8.Header = cm["main"]["t8"];
            t10.Header = cm["main"]["t10"];
            t11.Header = cm["main"]["t11"];
            t12.Header = cm["main"]["t12"];
            t13.Header = cm["main"]["t13"];
            t14.Header = cm["main"]["t14"];

            t1.OffContent = basel["def"]["off"];
            t3.OffContent = basel["def"]["off"];
            t5.OffContent = basel["def"]["off"];
            t6.OffContent = basel["def"]["off"];
            t8.OffContent = basel["def"]["off"];
            t10.OffContent = basel["def"]["off"];
            t11.OffContent = basel["def"]["off"];
            t12.OffContent = basel["def"]["off"];
            t13.OffContent = basel["def"]["off"];
            t14.OffContent = basel["def"]["off"];
            t15.OffContent = basel["def"]["off"];

            t1.OnContent = basel["def"]["on"];
            t3.OnContent = basel["def"]["on"];
            t5.OnContent = basel["def"]["on"];
            t6.OnContent = basel["def"]["on"];
            t8.OnContent = basel["def"]["on"];
            t10.OnContent = basel["def"]["on"];
            t11.OnContent = basel["def"]["on"];
            t12.OnContent = basel["def"]["on"];
            t13.OnContent = basel["def"]["on"];
            t14.OnContent = basel["def"]["on"];
            t15.OnContent = basel["def"]["on"];
        }
        private void checkReg()
        {
            t1.IsOn = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop")?
                           .GetValue("MenuShowDelay")?.Equals("50") ?? false;

            t3.IsOn = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked")?
                           .GetValue("{9F156763-7844-4DC4-B2B1-901F640F5155}")?.Equals("") ?? false;

            t5.IsOn = Registry.ClassesRoot.OpenSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers\ModernSharing") == null;

            t6.IsOn = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked")?
                           .GetValue("{596AB062-B4D2-4215-9F74-E9109B0A8153}")?.Equals("") ?? false;

            t8.IsOn = Registry.ClassesRoot.OpenSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers\SendTo")?
                           .GetValue("")?.Equals("") ?? false;

            t10.IsOn = Registry.ClassesRoot.OpenSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers\CopyAsPathMenu") == null;

            t11.IsOn = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Folder\shellex\ContextMenuHandlers\PintoStartScreen") == null ||
                       (Registry.ClassesRoot.CreateSubKey(@"exefile\shellex\ContextMenuHandlers\PintoStartScreen")?
                           .GetValue("")?.Equals("") ?? false);

            t12.IsOn = Registry.ClassesRoot.OpenSubKey(@"*\shellex\ContextMenuHandlers\{90AA3A4E-1CBA-4233-B8BB-535773D48449}") == null;
            t13.IsOn = Registry.ClassesRoot.OpenSubKey(@"Folder\shell\opennewtab") == null;

            t14.IsOn = Registry.CurrentUser.OpenSubKey(@"Software\NVIDIA Corporation\Global\NvCplApi\Policies")?
                           .GetValue("ContextUIPolicy")?.Equals(0) ?? false;

            t15.IsOn = Registry.CurrentUser.OpenSubKey(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32")?
                           .GetValue("")?.Equals("") ?? false;
        }
    }
}
