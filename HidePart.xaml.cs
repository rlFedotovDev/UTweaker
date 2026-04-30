using MakuTweakerNew.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using iNKORE.UI.WPF.Modern.Controls;

namespace MakuTweakerNew
{
    public partial class HidePart : ContentDialog
    {
        public TaskCompletionSource<decimal> TaskCompletionSource { get; private set; }
        public HidePart()
        {
            InitializeComponent();
            var languageCode = Properties.Settings.Default.lang ?? "en";
            var expl = MainWindow.Localization.LoadLocalization(languageCode, "expl");
            Run run1 = new Run
            {
                Text = expl["status"]["hdInfo1"],
                FontSize = 18,
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI Semilight")
            };
            Run run2 = new Run
            {
                Text = expl["status"]["hdInfo2"],
                FontSize = 18,
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI Semibold")
            };
            this.CloseButtonText = expl["status"]["hide"];
            this.PrimaryButtonText = expl["status"]["cc"];
            textBlock.Inlines.Add(run1);
            textBlock.Inlines.Add(new LineBreak());
            textBlock.Inlines.Add(run2);
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.MaxWidth = 500;
            this.TaskCompletionSource = new TaskCompletionSource<decimal>();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void CloseDialog(decimal result)
        {
            TaskCompletionSource.SetResult(result);
            this.Hide();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            CloseDialog(-1);
        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            decimal i = 0;    

            if (a.IsChecked == true) i += 1;            
            if (d.IsChecked == true) i += 8;        
            if (e.IsChecked == true) i += 16;       
            if (f.IsChecked == true) i += 32;       
            if (g.IsChecked == true) i += 64;       
            if (h.IsChecked == true) i += 128;      
            if (this.i.IsChecked == true) i += 256;
            if (j.IsChecked == true) i += 512;      
            if (k.IsChecked == true) i += 1024;     
            if (l.IsChecked == true) i += 2048;     
            if (m.IsChecked == true) i += 4096;     
            if (n.IsChecked == true) i += 8192;     
            if (o.IsChecked == true) i += 16384;    
            if (p.IsChecked == true) i += 32768;    
            if (q.IsChecked == true) i += 65536;    
            if (r.IsChecked == true) i += 131072;   
            if (s.IsChecked == true) i += 262144;   
            if (t.IsChecked == true) i += 524288;   
            if (u.IsChecked == true) i += 1048576;  
            if (v.IsChecked == true) i += 2097152;  
            if (w.IsChecked == true) i += 4194304;  
            if (x.IsChecked == true) i += 8388608;  
            if (y.IsChecked == true) i += 16777216;  
            if (z.IsChecked == true) i += 33554432;  

            StringBuilder drives = new StringBuilder();
            foreach (var child in checkboxpanel.Children)
            {
                if (child is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    drives.Append(checkBox.Content.ToString());
                }
            }
            CloseDialog(i);
        }
    }
}
