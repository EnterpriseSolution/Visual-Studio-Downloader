using VisualStudioDownloader.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace VisualStudioDownloader
{
    public partial class MainForm : Form
    {
        private string baseFromPath = "https://aka.ms/vs/17/release/";
/*
        https://aka.ms/vs/17/release/vs_enterprise.exe

        https://aka.ms/vs/17/release/vs_professional.exe

        https://aka.ms/vs/17/release/vs_community.exe

        */
        private const string vs_enterprise =
            "https://download.visualstudio.microsoft.com/download/pr/33081bfc-10f1-42d4-8f5a-df6709b8b105/4f370342df52079eafa623d16b50b1349a080722852ace1b98411e73ec8718b2/vs_Enterprise.exe";

       private const string vs_professional =
           " https://download.visualstudio.microsoft.com/download/pr/33081bfc-10f1-42d4-8f5a-df6709b8b105/70c749298e15efdf8a3543b74c8dcc4cd30a64cb2187953df43d05e29a5bb183/vs_Professional.exe";

       private const string vs_community =
           " https://download.visualstudio.microsoft.com/download/pr/33081bfc-10f1-42d4-8f5a-df6709b8b105/0b11ab6a0bc941b3968f666ac9ae26e257758ae8ae408bbe3118a341ac8816f3/vs_Community.exe";


        private string baseDownPath = Environment.CurrentDirectory;
      
        public event DelReadErrOutput ReadErrOutput;

        public event DelReadStdOutput ReadStdOutput;

        public MainForm()
        {
            InitTreeComponent();
            this.InitializeComponent();
            this.ReadStdOutput += new DelReadStdOutput(this.ReadStdOutputAction);
            this.ReadErrOutput += new DelReadErrOutput(this.ReadErrOutputAction);
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (cbxVersion.SelectedIndex == -1)
            {
                MessageBoxEx.Show(this, "请选择程序版本", "提示");
                return;
            }

            if (txtSaveDirectory.Text.ToString() == "")
            {
                MessageBoxEx.Show(this, "请选择缓存目录", "提示");
                return;
            }

            if (!Directory.Exists(txtSaveDirectory.Text.Trim()))
            {
                MessageBoxEx.Show(this, "缓存目录不存在，请检查后重试", "提示");
                return;
            }
            string remoteFile = string.Empty;

            if (cbxVersion.SelectedIndex == 0)
            {
                remoteFile = vs_community;
            }

            if (cbxVersion.SelectedIndex == 1)
            {
                remoteFile = vs_professional;
            }

            if (cbxVersion.SelectedIndex == 2)
            {
                remoteFile = vs_enterprise;
            }

            string localFile = this.baseDownPath + @"\" + this.GetFileNameByVersionType();

            if (!File.Exists(localFile))
                this.DownloadFile(remoteFile, localFile, new Action<int>(this.DownloadProgressChanged),
                    new Action(this.DownloadFileCompleted));

            List<DicNode> list = (from a in workloadList
                where a.Ischecked
                select a).ToList();
            List<DicNode> list2 = (from a in componentList
                where a.Ischecked
                select a).ToList();
            if (list.Count != 1 || list2.Count != 0 ||
                MessageBoxEx.Show("是否确认只安装核心组件?", "提示", MessageBoxButtons.YesNo) != DialogResult.No)
            {
                string text = "";
                foreach (DicNode item in list)
                {
                    if (item.Name == "Others")
                    {
                        List<DicNode> second = (from a in componentList
                            where a.Parentname == "Others"
                            select a).ToList();
                        list2 = list2.Concat(second).ToList();
                    }
                    else
                    {
                        text = text + "--add Microsoft.VisualStudio.Workload." + item.Name + " ";
                    }
                }

                foreach (DicNode item2 in list2)
                {
                    text = text + "--add " + item2.Name + " ";
                }

                if (text != "")
                {
                    text += "--includeRecommended ";
                }

                var language = cbxLanguage.SelectedItem as LanguageItem;
                string text2 = GetFileNameByVersionType() + " --layout " + txtSaveDirectory.Text.Trim() + " " + text +
                               $" --lang {language.locale}";
                txtcmdLogTextArea.Text = text2;
            }
        }

        private void DownloadFileCompleted()
        {
            Cursor = Cursors.Default;
        }

        private void DownloadProgressChanged(int val)
        {
            Cursor = Cursors.WaitCursor;
        }
        
        private void btnDown_Click(object sender, EventArgs e)
        {
            this.RealAction(this.txtcmdLogTextArea.Text);
        }

        private void btnSave_ConfigClick()
        {
            string filename = Application.ExecutablePath + ".config";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            string xpath = "configuration/applicationSettings/VisualStudioDownloader.Properties.Settings/setting[@name='Version']/value";
            XmlNode xmlNode = xmlDocument.SelectSingleNode(xpath);
            if (xmlNode != null && cbxVersion.SelectedIndex > -1)
            {
                xmlNode.InnerText = cbxVersion.SelectedItem.ToString();
                xmlDocument.Save(filename);
            }
            xpath = "configuration/applicationSettings/VisualStudioDownloader.Properties.Settings/setting[@name='Directory']/value";
            xmlNode = xmlDocument.SelectSingleNode(xpath);
            if (xmlNode != null)
            {
                xmlNode.InnerText = txtSaveDirectory.Text.Trim();
                xmlDocument.Save(filename);
                Settings.Default.Reload();
            }
            xpath = "configuration/applicationSettings/VisualStudioDownloader.Properties.Settings/setting[@name='Workloads']/value";
            xmlNode = xmlDocument.SelectSingleNode(xpath);
            if (xmlNode != null)
            {
                string text = "";
                foreach (DicNode item in (from a in workloadList
                             where a.Ischecked
                             select a).ToList())
                {
                    if (text != "")
                    {
                        text += "|";
                    }
                    text += item.Name;
                }
                xmlNode.InnerText = text;
                xmlDocument.Save(filename);
                Settings.Default.Reload();
            }
            xpath = "configuration/applicationSettings/VisualStudioDownloader.Properties.Settings/setting[@name='Components']/value";
            xmlNode = xmlDocument.SelectSingleNode(xpath);
            if (xmlNode != null)
            {
                string text = "";
                foreach (DicNode item2 in (from a in componentList
                             where a.Ischecked
                             select a).ToList())
                {
                    if (text != "")
                    {
                        text += "|";
                    }
                    text += item2.Name;
                }
                xmlNode.InnerText = text;
                xmlDocument.Save(filename);
                Settings.Default.Reload();
            }
        }

        private void btnSelectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog {
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.Desktop,
                SelectedPath = AppDomain.CurrentDomain.BaseDirectory
           };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.txtSaveDirectory.Text = dialog.SelectedPath;
            }
        }

        private void CmdProcess_Exited(object sender, EventArgs e)
        {
        }

        private string GetFileNameByVersionType()
        {
            string str = "";
            string str2 = this.cbxVersion.SelectedItem.ToString();
            if (str2 == "Visual Studio Community 2022")
            {
                str = "vs_community.exe";
            }
            else if (str2 == "Visual Studio Professional 2022")
            {
                str = "vs_professional.exe";
            }
            else if (str2 == "Visual Studio Enterprise 2022")
            {
                str = "vs_enterprise.exe";
            }
            return str;
        }

        private void DownloadFile(string url, string saveFile, Action<int> downloadProgressChanged, Action downloadFileCompleted)
        {
            WebClient client = new WebClient
            {
                Proxy = null
            };
            if (downloadProgressChanged != null)
            {
                client.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    object[] args = new object[] { e.ProgressPercentage };
                    this.Invoke(downloadProgressChanged, args);
                };
            }
            if (downloadFileCompleted != null)
            {
                client.DownloadFileCompleted += (sender, e) => this.Invoke(downloadFileCompleted);
            }
            client.DownloadFileAsync(new Uri(url), saveFile);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            InitializeLanguage();

            if (Settings.Default["Version"].ToString() != "")
            {
                this.cbxVersion.SelectedItem = Settings.Default["Version"].ToString();
            }
            if (Settings.Default["Directory"].ToString() != "")
            {
                this.txtSaveDirectory.Text = Settings.Default["Directory"].ToString();
            }
            string str = "";
            string str2 = "";
            if (Settings.Default["Workloads"].ToString() != "")
            {
                str = Settings.Default["Workloads"].ToString();
            }
            if (Settings.Default["Components"].ToString() != "")
            {
                str2 = Settings.Default["Components"].ToString();
            }

            this.treeViewWorkload.Nodes.Clear();
            foreach (DicNode node in this.workloadList)
            {
                TreeNode node2 = new TreeNode {
                    Name = node.Name,
                    Text = node.Text,
                    ToolTipText = node.Tooltip
                };
                if (str.IndexOf(node.Name) > -1)
                {
                    node.Ischecked = true;
                    node2.Checked = true;
                }
                if (node.Name == "CoreEditor")
                {
                    node2.Checked = true;
                }
                if (node.Name == "Others")
                {
                    int num = 0;
                    foreach (DicNode node3 in this.componentList)
                    {
                        TreeNode node4 = new TreeNode {
                            Name = node3.Name,
                            Text = node3.Text,
                            ToolTipText = node3.Tooltip
                        };
                        node2.Nodes.Add(node4);
                        if (str2.IndexOf(node3.Name) > -1)
                        {
                            node3.Ischecked = true;
                            node4.Checked = true;
                            num++;
                        }
                    }
                    if (node2.Nodes.Count != num)
                    {
                        node2.Checked = false;
                        node.Ischecked = false;
                    }
                }
                this.treeViewWorkload.Nodes.Add(node2);
            }
        }

        private void InitializeLanguage()
        {
            cbxLanguage.Items.Add(new LanguageItem("Cs-cz", "Czech"));
            cbxLanguage.Items.Add(new LanguageItem("De-de", "German"));
            cbxLanguage.Items.Add(new LanguageItem("En-us", "English"));
            cbxLanguage.Items.Add(new LanguageItem("Es-es", "Spanish"));
            cbxLanguage.Items.Add(new LanguageItem("Fr-fr", "French"));
            cbxLanguage.Items.Add(new LanguageItem("It-it", "Italian"));
            cbxLanguage.Items.Add(new LanguageItem("Ja-jp", "Japanese"));
            cbxLanguage.Items.Add(new LanguageItem("Ko-kr", "Korean"));
            cbxLanguage.Items.Add(new LanguageItem("Pl-pl", "Polish"));
            cbxLanguage.Items.Add(new LanguageItem("Pt-br", "Portuguese-Brazil"));
            cbxLanguage.Items.Add(new LanguageItem("Ru-ru", "Russian"));
            cbxLanguage.Items.Add(new LanguageItem("Tr-tr", "Turkish"));
            cbxLanguage.Items.Add(new LanguageItem("Zh-cn", "Chinese-Simplified"));
            cbxLanguage.Items.Add(new LanguageItem("Zh-tw", "Chinese-Traditional"));

            cbxLanguage.SelectedIndex = cbxLanguage.Items.Count - 2;
        }

        class LanguageItem
        {
            public LanguageItem(string locale, string description)
            {
                this.locale = locale;
                this.description = description;
            }

            public string locale;
            public string description;

            public override string ToString()
            {
                return description;
            }
        }

        private void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                object[] args = new object[] { e.Data };
                base.Invoke(this.ReadErrOutput, args);
            }
        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                object[] args = new object[] { e.Data };
                base.Invoke(this.ReadStdOutput, args);
            }
        }

        private void ReadErrOutputAction(string result)
        {
        }

        private void ReadStdOutputAction(string result)
        {
            this.txtcmdLogTextArea.AppendText(result + "\r\n");
        }

        private void RealAction(string StartCmd)
        {
            Process process1 = new Process();
            process1.StartInfo.FileName = "cmd.exe";
            process1.StartInfo.WorkingDirectory = ".";
            process1.StartInfo.CreateNoWindow = true;
            process1.StartInfo.UseShellExecute = false;
            process1.StartInfo.RedirectStandardInput = true;
            process1.StartInfo.RedirectStandardOutput = true;
            process1.StartInfo.RedirectStandardError = true;
            process1.OutputDataReceived += new DataReceivedEventHandler(this.p_OutputDataReceived);
            process1.ErrorDataReceived += new DataReceivedEventHandler(this.p_ErrorDataReceived);
            process1.EnableRaisingEvents = true;
            process1.Exited += new EventHandler(this.CmdProcess_Exited);
            process1.Start();
            process1.StandardInput.WriteLine(StartCmd);
            process1.BeginOutputReadLine();
            process1.BeginErrorReadLine();
        }

        private void treeViewWorkload_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Name == "CoreEditor")
            {
                if (!e.Node.Checked)
                {
                    e.Node.Checked = true;
                }
            }
            else
            {
                IEnumerator enumerator;
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode nodeNode in e.Node.Nodes)
                    {
                        ((TreeNode)nodeNode).Checked = e.Node.Checked;
                    }
                    using (List<DicNode>.Enumerator enumerator2 = (from a in this.componentList
                               where a.Parentname == e.Node.Name
                               select a).ToList<DicNode>().GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            enumerator2.Current.Ischecked = e.Node.Checked;
                        }
                    }
                }
                if (e.Node.Parent != null)
                {
                    int count = e.Node.Parent.Nodes.Count;
                    int num2 = 0;
                    foreach (TreeNode nodeNode in e.Node.Parent.Nodes)
                    {
                        if (!((TreeNode)nodeNode).Checked)
                        {
                            continue;
                        }
                        num2++;
                    }
                    e.Node.Parent.Checked = count == num2;
                }
                DicNode node = this.workloadList.Find(a => a.Name == e.Node.Name);
                if (node != null)
                {
                    node.Ischecked = e.Node.Checked;
                }
                node = this.componentList.Find(a => a.Name == e.Node.Name);
                if (node != null)
                {
                    node.Ischecked = e.Node.Checked;
                }
            }
        }

        private class DicNode
        {
            private string parentname;
            private string name;
            private string text;
            private string tooltip;
            private bool ischecked;

            public string Parentname
            {
                get => 
                    this.parentname;
                set => 
                    this.parentname = value;
            }

            public string Name
            {
                get => 
                    this.name;
                set => 
                    this.name = value;
            }

            public string Text
            {
                get => 
                    this.text;
                set => 
                    this.text = value;
            }

            public string Tooltip
            {
                get => 
                    this.tooltip;
                set => 
                    this.tooltip = value;
            }

            public bool Ischecked
            {
                get => 
                    this.ischecked;
                set => 
                    this.ischecked = value;
            }
        }

        public delegate string MyDelegate();
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnSave_ConfigClick();
        }

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // MainForm
        //    // 
        //    this.ClientSize = new System.Drawing.Size(402, 314);
        //    this.Name = "MainForm";
        //    this.ResumeLayout(false);

        //}
    }
}

