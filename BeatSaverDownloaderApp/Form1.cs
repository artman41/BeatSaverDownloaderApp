using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Web;
using HTML5DecodePortable;

namespace BeatSaverDownloader {
    public partial class Form1 : Form {
        DirectoryInfo CustomSongs { get; set; }

        const string API = "https://beatsaver.com/api.php?mode=new&off={0}";
        const string DOWNLOAD_LINK = "https://beatsaver.com/files/{0}.zip";
        const string IMAGE = "https://beatsaver.com/img/{0}.{1}";

        DirectoryInfo AppDir = new DirectoryInfo($"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}/BeatSaverDownloader/");

        DirectoryInfo DownloadsDir { get; set; }

        Thread WorkerThread;

        //List<SongItem> Songs { get; set; } = new List<SongItem>();
        List<ListViewItem> Songs { get; set; } = new List<ListViewItem>();
        ImageList CoverArts { get; set; } = new ImageList { ImageSize = new Size(60, 60) };

        delegate void SongJsonHandler(object sender, Tuple<SongJsonObject, Image>[] array);
        SongJsonHandler OnDeserialize;

        List<SongJsonObject> SongObjects { get; set; } = new List<SongJsonObject>();

        const string WindowTitle = "Song Downloader - {0}/{1}";
        const string labelText = "Page {0}";

        const string GitHubLink = "artman41/BeatSaverDownloaderApp/releases/latest";

        bool IsLatestVersion { get; set; }

        public Form1() {
            //AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException; //DEBUG
            InitializeComponent();
            listView1.LargeImageList = CoverArts;
            listView1.SmallImageList = CoverArts;
            var Update = CanUpdate();
            IsLatestVersion = !Update.Item1;
            if (Update.Item1) MessageBox.Show($"Version [{Update.Item2}] is available at http://Github.com/{GitHubLink}");

            AppDir.Create();
            if (AppDir.GetDirectories().Any(o => o.Name == "Downloads")) {
                DownloadsDir = AppDir.GetDirectories().First(o => o.Name == "Downloads");
                DownloadsDir.EnumerateFiles().ForEach(o => o.Delete());
                DownloadsDir.EnumerateDirectories().ForEach(o => o.Delete());
            } else {
                DownloadsDir = AppDir.CreateSubdirectory("Downloads");
            }
            WorkerThread = new Thread(o => {
                GetObjects();
                WorkerThread.Join();
            })
            {
                IsBackground = true
            };

            OnDeserialize += onDeserialize;

            WorkerThread.Start();
        }

        Tuple<bool, Version> CanUpdate() {
            var request = (HttpWebRequest)WebRequest.Create($"https://api.github.com/repos/{GitHubLink}");
            request.Method = "GET";
            request.UserAgent = $"BeatSaverDownloaderApp-{Application.ProductVersion}";
            using (var response = request.GetResponse() as HttpWebResponse) {
                string githubJsonString = string.Empty;
                using (var stream = response.GetResponseStream()) {
                    using (var reader = new StreamReader(stream)) {
                        githubJsonString = reader.ReadToEnd();
                    }
                }
                var githubReleasePage = JsonConvert.DeserializeObject<JObject>(githubJsonString);
                var remoteVersion = Version.Parse(githubReleasePage["tag_name"].ToString());
                if (remoteVersion.CompareTo(Version.Parse(Application.ProductVersion)) > 0) {
                    return new Tuple<bool, Version>(true, remoteVersion);
                }
            }
            return new Tuple<bool, Version>(false, null);
        }

        private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e) {
            using (var f = new StreamWriter("Log.txt", true)) {
                f.WriteLine(e.Exception.ToString());
            }
        }

        delegate void UpdatePanelDelegate(SongItem[] songs);

        private void onDeserialize(object sender, Tuple<SongJsonObject, Image>[] array) {
            if (listView1.InvokeRequired) {
                var newItems = array.Where(o => !SongObjects.Contains(o.Item1)).Select(o => o.Item1).ToArray();
                SongObjects.AddRange(newItems);
                var temp = new List<SongItem>();
                newItems.ForEach(o => {
                    o.Beatname = Utility.HtmlDecode(o.Beatname);
                    o.AuthorName = Utility.HtmlDecode(o.AuthorName);
                    temp.Add(new SongItem(o.Beatname, o.AuthorName, array.First(x => x.Item1 == o).Item2, int.Parse(o.Id)));
                });
                listView1.Invoke(new UpdatePanelDelegate(UpdatePanel), new object[] { temp.ToArray() });
            }
        }

        struct ListViewItemData {
            public enum _State {
                Queued, Processing, Processed
            }

            public _State State { get; set; }
            public int ID { get; set; }
        }

        void UpdatePanel(SongItem[] songs) {
            songs.ForEach(s => {
                if (!Songs.Any(o => ((ListViewItemData)o.Tag).ID == s.ID)) {
                    CoverArts.Images.Add(s.ID.ToString(), s.CoverArt);
                    var item = new ListViewItem {
                        Text = s.SongName,
                        ImageKey = s.ID.ToString(),
                        Tag = new ListViewItemData { ID = s.ID }
                    };
                    item.SubItems.Add(s.SongAuthor);
                    Songs.Add(item);
                    listView1.Items.Add(item);
                }
            });
            this.Text = string.Format(WindowTitle, Songs.Count(o => ((ListViewItemData)o.Tag).State == ListViewItemData._State.Processed), Songs.Count, CurrentOffset);
            LabelOffset.Text = string.Format(labelText, CurrentOffset / 15);
            progressBar1.Maximum = Songs.Count;
        }

        bool isShown;
        bool run;

        ToolStripButton ButtonCustomSongs, ButtonDownloads, ButtonUpdate, ButtonGithub, ButtonWiki, ButtonCredits;

        CheckBox JumpCheck { get; set; }

        private void Form1_Load(object sender, EventArgs e) {
            this.Text = string.Format(WindowTitle, Songs.Count(o => ((ListViewItemData)o.Tag).State == ListViewItemData._State.Processed), Songs.Count, CurrentOffset);
            LabelOffset.Text = string.Format(labelText, CurrentOffset);
            isShown = true;
            CustomSongs = CustomSongsDialog.GetDirectoryInfo();

            #region ToolStripInit
            JumpCheck = new CheckBox {
                Text = "Jump to Latest Download"
            };
            ToolStripControlHost host = new ToolStripControlHost(JumpCheck);
            ToolStrip.Items.Add(host);

            ToolStripFileButton.DropDownItems.AddRange(new[] {
                ButtonCustomSongs = new ToolStripButton("CustomSongs", null, ButtonCustomSongs_Click, "ButtonCustomSongs"){
                    DisplayStyle = ToolStripItemDisplayStyle.Text
                },
                ButtonDownloads = new ToolStripButton("Downloads", null, ButtonDownloads_Click, "ButtonDownloads"){
                    DisplayStyle = ToolStripItemDisplayStyle.Text
                }
            });
            ToolStripFileButton.DropDownItems.Remove(HOLDER);
            HOLDER.Dispose();

            if (!IsLatestVersion) {
                ToolStripAboutButton.DropDownItems.Add(ButtonUpdate = new ToolStripButton("Update", null, ButtonUpdate_Click, "ButtonUpdate") {
                    DisplayStyle = ToolStripItemDisplayStyle.Text
                });
            }
            ToolStripAboutButton.DropDownItems.AddRange(new[] {
                ButtonGithub = new ToolStripButton("Github", null, ButtonGithub_Click, "ButtonGithub"){
                    DisplayStyle = ToolStripItemDisplayStyle.Text
                },
                ButtonWiki = new ToolStripButton("Wiki", null, ButtonWiki_Click, "ButtonWiki"){
                    DisplayStyle = ToolStripItemDisplayStyle.Text
                },
                ButtonCredits = new ToolStripButton("Credits", null, ButtonCredits_Click, "ButtonCredits"){
                    DisplayStyle = ToolStripItemDisplayStyle.Text
                }
            });
            #endregion

            if (CustomSongs == null) {
                MessageBox.Show("You did not set a correct path for the CustomSongs directory.");
                Application.Exit();
            }
            run = true;
        }

        int CurrentOffset { get; set; } = 0;

        List<SongJsonObject> GetObjects() {
            var x = new List<SongJsonObject>();
            //var y = new List<Tuple<SongJsonObject, Image>>();
            using (var client = new WebClient()) {
                client.Headers.Add("user-agent", $"BeatSaverDownloaderApp-{Application.ProductVersion}");
                string jsonString = client.DownloadString(string.Format(API, CurrentOffset));
                SongJsonObject[] objs;
                while (jsonString != "[]" && jsonString != string.Empty) {
                    //Log($"Current Offset: {i}");
                    objs = JsonConvert.DeserializeObject<SongJsonObject[]>(jsonString);
                    x.AddRange(objs);
                    CurrentOffset += 15;
                    jsonString = client.DownloadString(string.Format(API, CurrentOffset));
                    OnDeserialize?.Invoke(this, objs.Select(o => {
                        var imageUrl = string.Format(IMAGE, o.Id, o.Img);
                        byte[] imageBytes = null;
                        bool errored = false;
                        try {
                            imageBytes = client.DownloadData(imageUrl);
                        } catch (WebException ex) {
                            errored = true;
                        }
                        if (!errored) {
                            try {
                                using (var ms = new MemoryStream(imageBytes))
                                    return new Tuple<SongJsonObject, Image>(o, Image.FromStream(ms));
                            } catch (ArgumentException ex) {
                                Log($"[{o.Id}] Image doesn't exist {{{ex.Message}}}");
                                using (var ms = new MemoryStream(client.DownloadData("https://pbs.twimg.com/profile_images/955933299238756352/KAIUfh1q_400x400.jpg")))
                                    return new Tuple<SongJsonObject, Image>(o, Image.FromStream(ms));
                            }
                        }
                        return null;
                    }).Where(o => o != null).ToArray());
                }
            }
            return x;
        }

        Thread DownloaderThread { get; set; } = null;
        private void ButtonDownload_Click(object sender, EventArgs e) {
            if (listView1.Items.Count == 0) return;
            if (DownloaderThread == null) {
                DownloaderThread = new Thread(() => {
                    DownloadSongs();
                })
                {
                    IsBackground = true
                };
                ButtonDownload.Enabled = false;
                DownloaderThread.Start();
            }
        }

        Dictionary<int, string> CompletedIDs { get; set; } = new Dictionary<int, string>();

        delegate void genericDelegate();
        void DownloadSongs() {
            var historyPath = Path.Combine(CustomSongs.FullName, "History.json");
            try {
                if (!File.Exists(historyPath)) throw new FileNotFoundException("History doesn't exist");
                var historyString = File.ReadAllText(historyPath);
                if (historyString != string.Empty) CompletedIDs = JsonConvert.DeserializeObject<Dictionary<int, string>>(historyString);
            } catch (FileNotFoundException ex) {
                Log("History.json does not yet exist");
            }
            int i = 0;
            string zipPath = string.Empty;
            using (var client = new WebClient()) {
                while (run) {
                    if (isShown) this?.Invoke(new genericDelegate(() => this.Text = string.Format(WindowTitle, Songs.Count(o => ((ListViewItemData)o.Tag).State == ListViewItemData._State.Processed), Songs.Count)), new object[] { });
                    //if (zipPath != string.Empty && File.Exists(zipPath)) File.Delete(zipPath);
                    LabelCurrentDownloading?.Invoke(new genericDelegate(() => LabelCurrentDownloading.Text = ""), new object[] { });
                    var historyFile = new FileInfo(Path.Combine(CustomSongs.FullName, "History.json"));
                    if (i >= listView1.Items.Count) continue;
                    //var songControl = this.flowLayoutPanel1.Controls[i] as SongItem;
                    ListViewItem temp = Songs[i];
                    if (JumpCheck.Checked) {
                        listView1?.Invoke(new genericDelegate(() => {
                            int nextOne = listView1.Items.IndexOf(temp) + 3;
                            var focusItem = nextOne >= listView1.Items.Count ? temp : listView1.Items[nextOne];
                            listView1.FocusedItem = focusItem;
                        }), new object[] { });
                    }
                    var data = ((ListViewItemData)temp.Tag);
                    if (!CompletedIDs.Keys.Contains(data.ID)) {
                        string songName = string.Empty;
                        #region Downloader
                        LabelCurrentDownloading?.Invoke(new genericDelegate(() => LabelCurrentDownloading.Text = $"Downloading: {temp.Text} [{data.ID}]"), new object[] { });
                        zipPath = Path.Combine(DownloadsDir.FullName, $"{data.ID}.zip");
                        client.DownloadFile(string.Format(DOWNLOAD_LINK, data.ID), zipPath);
                        listView1?.Invoke(new genericDelegate(() =>{
                            temp.Tag = new ListViewItemData {
                                ID = data.ID,
                                State = ListViewItemData._State.Processing
                            }; temp.ForeColor = Color.Orange; }), new object[] { });
                        ZipArchive zip = null;
                        try {
                            zip = ZipFile.OpenRead(zipPath);
                        } catch (Exception ex) {
                            Log(ex.Message);
                        }
                        songName = zip?.Entries[0].FullName.Split('/')[0];
                        try {
                            zip?.ExtractToDirectory(CustomSongs.FullName);
                            try {
                                zip.Dispose();
                                File.Delete(zipPath);
                                Log($"Deleted Zip [{data.ID}]");
                            } catch (IOException ex) {
                                Log($"Failed to remove Zip [{data.ID}]");
                            }
                        } catch (IOException ex) {
                            Log($"Folder [{songName}] exists. Continuing.");
                            try {
                                zip.Dispose();
                                File.Delete(zipPath);
                                Log($"Deleted Zip [{data.ID}]");
                            } catch (IOException) {
                                Log($"Failed to remove Zip [{data.ID}]");
                            }
                            listView1?.Invoke(new genericDelegate(() => {
                                temp.Tag = new ListViewItemData {
                                    ID = data.ID,
                                    State = ListViewItemData._State.Processed
                                }; temp.ForeColor = Color.Green; }), new object[] { });
                            progressBar1?.Invoke(new genericDelegate(UpdateProgressBar), new object[] { });
                            CompletedIDs.Add(data.ID, songName);
                            using (var f = new StreamWriter(historyFile.Open(FileMode.Create, FileAccess.Write, FileShare.Read))) {
                                f.WriteLine(JsonConvert.SerializeObject(CompletedIDs, Formatting.Indented));
                            }
                            continue;
                        }
                        listView1?.Invoke(new genericDelegate(() => {
                            temp.Tag = new ListViewItemData {
                                ID = data.ID,
                                State = ListViewItemData._State.Processed
                            }; temp.ForeColor = Color.Green; }), new object[] { });
                        progressBar1?.Invoke(new genericDelegate(UpdateProgressBar), new object[] { });
                        #endregion
                        Log($"Downloaded {songName} [{data.ID}] to {CustomSongs.FullName}");
                        CompletedIDs.Add(data.ID, songName);
                        using (var f = new StreamWriter(historyFile.Open(FileMode.Create, FileAccess.Write, FileShare.Read))) {
                            f.WriteLine(JsonConvert.SerializeObject(CompletedIDs, Formatting.Indented));
                        }
                    } else {
                        try {
                            Thread.Sleep(15);
                            listView1?.Invoke(new genericDelegate(() => {
                                temp.Tag = new ListViewItemData {
                                    ID = data.ID,
                                    State = ListViewItemData._State.Processed
                                }; temp.ForeColor = Color.Green; }), new object[] { });
                            progressBar1?.Invoke(new genericDelegate(UpdateProgressBar), new object[] { });
                        } catch (InvalidOperationException ex) {
                            Log($"[{data.ID}] {ex.Message}");
                            Thread.Sleep(10);
                            listView1?.Invoke(new genericDelegate(() => {
                                temp.Tag = new ListViewItemData {
                                    ID = data.ID,
                                    State = ListViewItemData._State.Processed
                                };
                                temp.ForeColor = Color.Green;
                            }), new object[] { });
                            progressBar1?.Invoke(new genericDelegate(UpdateProgressBar), new object[] { });
                        }
                    }
                    i++;
                }
            }
            try {
                ButtonDownload.Enabled = true;
            } catch (Exception) { };
            DownloaderThread.Join();
        }

        void UpdateProgressBar() {
            var count = Songs.Count(o => {
                var state = ((ListViewItemData)o.Tag).State;
                return state == ListViewItemData._State.Processed;
            });
            this.progressBar1.Value = count;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            run = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            try {
                DownloadsDir.EnumerateFiles().ForEach(o => o.Delete());
                DownloadsDir.EnumerateDirectories().ForEach(o => o.Delete());
            } catch (IOException ex) {
                Log(ex.Message);
            }
        }

        void Log(object o) {
            Console.WriteLine($"{o}");
            //Debug.WriteLine(s);
        }

        private void ButtonCustomSongs_Click(object sender, EventArgs e) {
            Process.Start(CustomSongs.FullName);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {
            if (listView1.SelectedItems.Count < 1) return;
            //var length = listView1.SelectedItems.Count;
            //Parallel.For(0, length, i => listView1?.Invoke(new genericDelegate(() => Process.Start($"https://beatsaver.com/details.php?id={((ListViewItemData)(sender as ListView).SelectedItems[i].Tag).ID}")), new object[] { }));
            Process.Start($"https://beatsaver.com/details.php?id={((ListViewItemData)listView1.SelectedItems[0].Tag).ID}");
        }

        private void ButtonDownloads_Click(object sender, EventArgs e) {
            Process.Start(DownloadsDir.FullName);
        }

        private void ButtonUpdate_Click(object sender, EventArgs e) {
            Process.Start($"http://Github.com/{GitHubLink}");
        }

        private void ButtonGithub_Click(object sender, EventArgs e) {
            Process.Start($"http://Github.com/{GitHubLink}".Replace("/releases/latest", ""));
        }

        private void ButtonWiki_Click(object sender, EventArgs e) {
            Process.Start($"http://Github.com/{GitHubLink}".Replace("releases/latest", "Wiki"));
        }

        private void ButtonCredits_Click(object sender, EventArgs e) {
            var url = $"https://raw.githubusercontent.com/{Form1.GitHubLink.Replace("/releases/latest", " /master/Credits.md")}";
            RichMessageBox.Display("Credits", url);
        }
    }
}
