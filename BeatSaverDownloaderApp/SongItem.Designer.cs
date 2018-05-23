namespace BeatSaverDownloader
{
    partial class SongItem
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.IsDownloaded = new System.Windows.Forms.CheckBox();
            this.LabelName = new System.Windows.Forms.Label();
            this.LabelAuthor = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.LabelID = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // IsDownloaded
            // 
            this.IsDownloaded.AutoSize = true;
            this.IsDownloaded.BackColor = System.Drawing.SystemColors.Control;
            this.IsDownloaded.Enabled = false;
            this.IsDownloaded.Location = new System.Drawing.Point(6, 16);
            this.IsDownloaded.Name = "IsDownloaded";
            this.IsDownloaded.Size = new System.Drawing.Size(15, 14);
            this.IsDownloaded.TabIndex = 0;
            this.ToolTip.SetToolTip(this.IsDownloaded, "Shows whether the current item is downloaded");
            this.IsDownloaded.UseVisualStyleBackColor = false;
            this.IsDownloaded.CheckStateChanged += new System.EventHandler(this.IsDownloaded_CheckStateChanged);
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelName.Location = new System.Drawing.Point(27, 12);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(191, 13);
            this.LabelName.TabIndex = 1;
            this.LabelName.Text = "Through the Fire and the Flames";
            // 
            // LabelAuthor
            // 
            this.LabelAuthor.AutoSize = true;
            this.LabelAuthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelAuthor.Location = new System.Drawing.Point(27, 25);
            this.LabelAuthor.Name = "LabelAuthor";
            this.LabelAuthor.Size = new System.Drawing.Size(69, 13);
            this.LabelAuthor.TabIndex = 2;
            this.LabelAuthor.Text = "DragonForce";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(224, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(54, 41);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // ToolTip
            // 
            this.ToolTip.Tag = "";
            // 
            // LabelID
            // 
            this.LabelID.AutoSize = true;
            this.LabelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelID.Location = new System.Drawing.Point(163, 32);
            this.LabelID.Name = "LabelID";
            this.LabelID.Size = new System.Drawing.Size(55, 13);
            this.LabelID.TabIndex = 4;
            this.LabelID.TabStop = true;
            this.LabelID.Text = "linkLabel1";
            this.ToolTip.SetToolTip(this.LabelID, "Link to the beat saver page");
            this.LabelID.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LabelID_LinkClicked);
            // 
            // SongItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LabelID);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.LabelAuthor);
            this.Controls.Add(this.LabelName);
            this.Controls.Add(this.IsDownloaded);
            this.Name = "SongItem";
            this.Size = new System.Drawing.Size(281, 48);
            this.Load += new System.EventHandler(this.SongItem_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label LabelName;
        private System.Windows.Forms.Label LabelAuthor;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.CheckBox IsDownloaded;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.LinkLabel LabelID;
    }
}
