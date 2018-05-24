using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Markdig;

namespace BeatSaverDownloader {
    public partial class RichMessageBox : Form {
        RichMessageBox() {
            InitializeComponent();
        }

        public static void Display(string title, string markdownUrl) {
            var x = new RichMessageBox();
            x.Text = title;
            using (var client = new WebClient()) {
                client.Headers.Add("user-agent", $"BeatSaverDownloaderApp-{Application.ProductVersion}");
                var markdown = client.DownloadString(markdownUrl);
                var html = Markdown.ToHtml(markdown);
                    
                var style = "style=\"font-size:14px; font-family: Arial; \" ";
                html = $"<!DOCTYPE html><html><body {style}><ul style=\"margin-left: 0px;margin-top:0px;padding-left: 15px;padding-top:0px;\">{html.Replace("p>", "li style=\"margin: 5px 0;\">")}</ul></body></html>";
                Console.WriteLine(html);
                x.webBrowser1.DocumentText = html;
            }
            x.ShowDialog();
        }

        private void AcceptButton_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
