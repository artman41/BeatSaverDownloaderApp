using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace BeatSaverDownloader
{
    public partial class SongItem : UserControl
    {
        string SongName { get; set; }
        string SongAuthor { get; set; }
        Image CoverArt { get; set; }
        public int ID { get; set; }

        public SongItem()
        {
            InitializeComponent();
        }

        public SongItem(string SongName, string SongAuthor, Image CoverArt, int id)
        {
            this.SongName = SongName;
            this.SongAuthor = SongAuthor;
            this.CoverArt = CoverArt;
            this.ID = id;
            InitializeComponent();
        }

        private void SongItem_Load(object sender, EventArgs e) {
            this.LabelName.Text = SongName;
            this.LabelAuthor.Text = SongAuthor;
            this.pictureBox1.Image = CoverArt;
            this.LabelID.Text = $"[{ID}]";
            this.LabelID.Links.Add(new LinkLabel.Link { LinkData = $"https://beatsaver.com/details.php?id={ID}"});
        }

        private void IsDownloaded_CheckStateChanged(object sender, EventArgs e) {
            switch (IsDownloaded.CheckState) {
                case CheckState.Unchecked:
                    this.BackColor = SystemColors.Control;
                    break;
                case CheckState.Indeterminate:
                    this.BackColor = Color.Orange;
                    break;
                case CheckState.Checked:
                    this.BackColor = Color.Green;
                    break;
            }
        }

        private void LabelID_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(e.Link.LinkData as string);
        }
    }
}
