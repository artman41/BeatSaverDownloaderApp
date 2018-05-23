using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeatSaverDownloader
{
    public partial class CustomSongsDialog : Form
    {
        public CustomSongsDialog()
        {
            InitializeComponent();
        }

        public static DirectoryInfo GetDirectoryInfo()
        {
            var x = new CustomSongsDialog();
            if (x.ShowDialog() == DialogResult.OK) return new DirectoryInfo(x.textBox1.Text);
            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            DialogResult = DialogResult.OK;
        }
    }
}
