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
    public class MainForm : Form
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
        private List<DicNode> workloadList;
        private List<DicNode> componentList;
        private IContainer components;
        private Label label1;
        private Label label2;
        private TextBox txtSaveDirectory;
        private Button btnSelectDir;
        private ComboBox cbxVersion;
        private Button btnDown;
        private TextBox txtcmdLogTextArea;
        private TreeView treeViewWorkload;
        private Button btnBuild;

        public event DelReadErrOutput ReadErrOutput;

        public event DelReadStdOutput ReadStdOutput;

        public MainForm()
        {
            DicNode item = new DicNode();
            item.Name = "CoreEditor";
            item.Text = "Visual Studio 核心编辑器";
            item.Tooltip = "Visual Studio 核心 shell 体验，包括语法感知代码编辑、源代码管理和工作项管理。";
            item.Ischecked = true;
            List<DicNode> list1 = new List<DicNode>();
            list1.Add(item);
            DicNode node2 = new DicNode();
            node2.Name = "Azure";
            node2.Text = "Azure 开发";
            node2.Tooltip = "用于开发云应用、创建资源以及生成包括 Docker 支持的容器的 Azure SDK、工具和项目。";
            list1.Add(node2);
            DicNode node3 = new DicNode();
            node3.Name = "Data";
            node3.Text = "数据存储和处理";
            node3.Tooltip = "使用 SQL Server、Azure Data Lake 或 Hadoop 连接、开发和测试数据解决方案。";
            list1.Add(node3);
            DicNode node4 = new DicNode();
            node4.Name = "DataScience";
            node4.Text = "数据科学和分析应用程序";
            node4.Tooltip = "用于创建数据科学应用程序的语言和工具（包括 Python、R 和 F#）。";
            list1.Add(node4);
            DicNode node5 = new DicNode();
            node5.Name = "ManagedDesktop";
            node5.Text = ".NET 桌面开发";
            node5.Tooltip = "使用 C#、Visual Basic 和 F# 生成 WPF、Windows 窗体和控制台应用程序。";
            list1.Add(node5);
            DicNode node6 = new DicNode();
            node6.Name = "ManagedGame";
            node6.Text = "使用 Unity 的游戏开发";
            node6.Tooltip = "使用 Unity（功能强大的跨平台开发环境）创建 2D 和 3D 游戏。";
            list1.Add(node6);
            DicNode node7 = new DicNode();
            node7.Name = "NativeCrossPlat";
            node7.Text = "使用 C++ 的 Linux 开发";
            node7.Tooltip = "创建和调试在 Linux 环境中运行的应用程序。";
            list1.Add(node7);
            DicNode node8 = new DicNode();
            node8.Name = "NativeDesktop";
            node8.Text = "使用 C++ 的桌面开发";
            node8.Tooltip = "使用 Microsoft C++ 工具集、ATL 或 MFC 生成 Windows 桌面应用程序。";
            list1.Add(node8);
            DicNode node9 = new DicNode();
            node9.Name = "NativeGame";
            node9.Text = "使用 C++ 的游戏开发";
            node9.Tooltip = "以 DirectX、Unreal 或 Cocos2d 为后盾，利用 C++ 的强大功能生成专业游戏。";
            list1.Add(node9);
            DicNode node10 = new DicNode();
            node10.Name = "NativeMobile";
            node10.Text = "使用 C++ 的移动开发";
            node10.Tooltip = "使用 C++ 生成适用于 iOS、Android 或 Windows 的跨平台应用程序。";
            list1.Add(node10);
            DicNode node11 = new DicNode();
            node11.Name = "NetCoreTools";
            node11.Text = ".NET Core 跨平台开发";
            node11.Tooltip = "使用 .NET Core、ASP.NET Core、HTML/JavaScript 和包括 Docker 支持的容器生成跨平台应用程序。";
            list1.Add(node11);
            DicNode node12 = new DicNode();
            node12.Name = "NetCrossPlat";
            node12.Text = "使用 .NET 的移动开发";
            node12.Tooltip = "使用 Xmarin 生成适用于 iOS、Android 或 Windows 的跨平台应用程序。";
            list1.Add(node12);
            DicNode node13 = new DicNode();
            node13.Name = "NetWeb";
            node13.Text = "ASP.NET 和 Web 开发";
            node13.Tooltip = "使用 ASP.NET、ASP.NET Core、HTML/JavaScript 和包括 Docker 支持的容器生成 Web 应用程序。";
            list1.Add(node13);
            DicNode node14 = new DicNode();
            node14.Name = "Node";
            node14.Text = "Node.js 开发";
            node14.Tooltip = "使用 Node.js（事件驱动的异步 JavaScript 运行时）生成可扩展的网络应用程序。";
            list1.Add(node14);
            DicNode node15 = new DicNode();
            node15.Name = "Office";
            node15.Text = "Office/SharePoint 开发";
            node15.Tooltip = "使用 C#、VB 和 JavaScript 创建 Office 和 SharePoint 外接程序、SharePoint 解决方案和 VSTO 外接程序。";
            list1.Add(node15);
            DicNode node16 = new DicNode();
            node16.Name = "Python";
            node16.Text = "Python 开发";
            node16.Tooltip = "适用于 Python 的编辑、调试、交互式开发和源代码管理。";
            list1.Add(node16);
            DicNode node17 = new DicNode();
            node17.Name = "Universal";
            node17.Text = "通用 Windows 平台开发";
            node17.Tooltip = "使用 C#、VB 和 JavaScript 或 C++（可选）创建适用于通用 Windows 平台的应用程序。";
            list1.Add(node17);
            DicNode node18 = new DicNode();
            node18.Name = "VisualStudioExtension";
            node18.Text = "Visual Studio 扩展开发";
            node18.Tooltip = "创建适用于 Visual Studio 的加载项和扩展，包括新命令、代码分析器和工具窗口。";
            list1.Add(node18);
            DicNode node19 = new DicNode();
            node19.Name = "WebCrossPlat";
            node19.Text = "使用 JavaScript 的移动开发";
            node19.Tooltip = "使用用于 Apache Cordova 的工具生成 Android、iOS 和 UWP 应用。";
            list1.Add(node19);
            DicNode node20 = new DicNode();
            node20.Name = "Others";
            node20.Text = "独立组件";
            node20.Tooltip = "这些组件不随附于任何工作负载，但可选择作为单个组件。";
            list1.Add(node20);
            this.workloadList = list1;
            DicNode node21 = new DicNode();
            node21.Parentname = "Others";
            node21.Name = "Component.Android.Emulator";
            node21.Text = "适用于 Android 的 Visual Studio 仿真程序";
            List<DicNode> list2 = new List<DicNode>();
            list2.Add(node21);
            DicNode node22 = new DicNode();
            node22.Parentname = "Others";
            node22.Name = "Component.Android.NDK.R11C";
            node22.Text = "Android NDK (R11C)";
            list2.Add(node22);
            DicNode node23 = new DicNode();
            node23.Parentname = "Others";
            node23.Name = "Component.Android.NDK.R11C_3264";
            node23.Text = "Android NDK (R11C)（32 位）";
            list2.Add(node23);
            DicNode node24 = new DicNode();
            node24.Parentname = "Others";
            node24.Name = "Component.Android.SDK23";
            node24.Text = "Android SDK 安装程序（API 级别 23）（全局安装）";
            list2.Add(node24);
            DicNode node25 = new DicNode();
            node25.Parentname = "Others";
            node25.Name = "Component.Android.SDK25";
            node25.Text = "Android SDK 安装程序（API 级别 25）";
            list2.Add(node25);
            DicNode node26 = new DicNode();
            node26.Parentname = "Others";
            node26.Name = "Component.GitHub.VisualStudio";
            node26.Text = "适用于 Visual Studio 的 GitHub 扩展";
            list2.Add(node26);
            DicNode node27 = new DicNode();
            node27.Parentname = "Others";
            node27.Name = "Component.Google.Android.Emulator.API23.V2";
            node27.Text = "Google Android Emulator（API 级别 23）（全局安装）";
            list2.Add(node27);
            DicNode node28 = new DicNode();
            node28.Parentname = "Others";
            node28.Name = "Component.Google.Android.Emulator.API25";
            node28.Text = "Google Android Emulator（API 级别 25）";
            list2.Add(node28);
            DicNode node29 = new DicNode();
            node29.Parentname = "Others";
            node29.Name = "Microsoft.Component.Blend.SDK.WPF";
            node29.Text = "用于 .NET 的 Blend for Visual Studio SDK ";
            list2.Add(node29);
            DicNode node30 = new DicNode();
            node30.Parentname = "Others";
            node30.Name = "Microsoft.Component.HelpViewer";
            node30.Text = "帮助查看器";
            list2.Add(node30);
            DicNode node31 = new DicNode();
            node31.Parentname = "Others";
            node31.Name = "Microsoft.VisualStudio.Component.LinqToSql";
            node31.Text = "LINQ to SQL 工具";
            list2.Add(node31);
            DicNode node32 = new DicNode();
            node32.Parentname = "Others";
            node32.Name = "Microsoft.VisualStudio.Component.Phone.Emulator Windows";
            node32.Text = "10 移动版仿真程序（周年纪念版）";
            list2.Add(node32);
            DicNode node33 = new DicNode();
            node33.Parentname = "Others";
            node33.Name = "Microsoft.VisualStudio.Component.Phone.Emulator.15063";
            node33.Text = "Windows 10 Mobile 仿真器（创意者更新）";
            list2.Add(node33);
            DicNode node34 = new DicNode();
            node34.Parentname = "Others";
            node34.Name = "Microsoft.VisualStudio.Component.Runtime.Node.x86.6.4.0";
            node34.Text = "基于 Node.js v6.4.0 (x86) 的组件运行时";
            list2.Add(node34);
            DicNode node35 = new DicNode();
            node35.Parentname = "Others";
            node35.Name = "Microsoft.VisualStudio.Component.Runtime.Node.x86.7.4.0";
            node35.Text = "基于 Node.js v7.4.0 (x86) 的组件运行时";
            list2.Add(node35);
            DicNode node36 = new DicNode();
            node36.Parentname = "Others";
            node36.Name = "Microsoft.VisualStudio.Component.TestTools.CodedUITest";
            node36.Text = "编码的 UI 测试";
            list2.Add(node36);
            DicNode node37 = new DicNode();
            node37.Parentname = "Others";
            node37.Name = "Microsoft.VisualStudio.Component.TestTools.FeedbackClient";
            node37.Text = "Microsoft Feedback Client";
            list2.Add(node37);
            DicNode node38 = new DicNode();
            node38.Parentname = "Others";
            node38.Name = "Microsoft.VisualStudio.Component.TestTools.MicrosoftTestManager Microsoft";
            node38.Text = "测试管理器";
            list2.Add(node38);
            DicNode node39 = new DicNode();
            node39.Parentname = "Others";
            node39.Name = "Microsoft.VisualStudio.Component.TypeScript.2.0";
            node39.Text = "TypeScript 2.0 SDK";
            list2.Add(node39);
            DicNode node40 = new DicNode();
            node40.Parentname = "Others";
            node40.Name = "Microsoft.VisualStudio.Component.TypeScript.2.1";
            node40.Text = "TypeScript 2.1 SDK";
            list2.Add(node40);
            DicNode node41 = new DicNode();
            node41.Parentname = "Others";
            node41.Name = "Microsoft.VisualStudio.Component.TypeScript.2.2";
            node41.Text = "TypeScript 2.2 SDK";
            list2.Add(node41);
            DicNode node42 = new DicNode();
            node42.Parentname = "Others";
            node42.Name = "Microsoft.VisualStudio.Component.TypeScript.2.5";
            node42.Text = "TypeScript 2.5 SDK";
            list2.Add(node42);
            DicNode node43 = new DicNode();
            node43.Parentname = "Others";
            node43.Name = "Microsoft.VisualStudio.Component.TypeScript.2.6";
            node43.Text = "TypeScript 2.6 SDK";
            list2.Add(node43);
            DicNode node44 = new DicNode();
            node44.Parentname = "Others";
            node44.Name = "Microsoft.VisualStudio.Component.TypeScript.2.7";
            node44.Text = "TypeScript 2.7 SDK";
            list2.Add(node44);
            DicNode node45 = new DicNode();
            node45.Parentname = "Others";
            node45.Name = "Microsoft.VisualStudio.Component.UWP.VC.ARM64";
            node45.Text = "适用于 ARM64 的 C++ 通用 Windows 平台工具";
            list2.Add(node45);
            DicNode node46 = new DicNode();
            node46.Parentname = "Others";
            node46.Name = "Microsoft.VisualStudio.Component.VC.ATL.ARM.Spectre";
            node46.Text = "带有 Spectre 缓解措施的 Visual C++ ATL for ARM";
            list2.Add(node46);
            DicNode node47 = new DicNode();
            node47.Parentname = "Others";
            node47.Name = "Microsoft.VisualStudio.Component.VC.ATL.ARM64.Spectre";
            node47.Text = "带有 Spectre 缓解措施的 Visual C++ ATL for ARM64";
            list2.Add(node47);
            DicNode node48 = new DicNode();
            node48.Parentname = "Others";
            node48.Name = "Microsoft.VisualStudio.Component.VC.ATL.Spectre";
            node48.Text = "带有 Spectre 缓解措施的 Visual C++ ATL (x86/x64)";
            list2.Add(node48);
            DicNode node49 = new DicNode();
            node49.Parentname = "Others";
            node49.Name = "Microsoft.VisualStudio.Component.VC.ATLMFC.Spectre";
            node49.Text = "带有 Spectre 缓解措施的 Visual C++ MFC for x86/x64";
            list2.Add(node49);
            DicNode node50 = new DicNode();
            node50.Parentname = "Others";
            node50.Name = "Microsoft.VisualStudio.Component.VC.ClangC2";
            node50.Text = "Clang/C2（实验）";
            list2.Add(node50);
            DicNode node51 = new DicNode();
            node51.Parentname = "Others";
            node51.Name = "Microsoft.VisualStudio.Component.VC.MFC.ARM";
            node51.Text = "Visual C++ MFC for ARM";
            list2.Add(node51);
            DicNode node52 = new DicNode();
            node52.Parentname = "Others";
            node52.Name = "Microsoft.VisualStudio.Component.VC.MFC.ARM.Spectre";
            node52.Text = "带有 Spectre 缓解措施的 Visual C++ MFC for ARM";
            list2.Add(node52);
            DicNode node53 = new DicNode();
            node53.Parentname = "Others";
            node53.Name = "Microsoft.VisualStudio.Component.VC.MFC.ARM64";
            node53.Text = "Visual C++ MFC for ARM64";
            list2.Add(node53);
            DicNode node54 = new DicNode();
            node54.Parentname = "Others";
            node54.Name = "Microsoft.VisualStudio.Component.VC.MFC.ARM64.Spectre";
            node54.Text = "带有 Spectre 缓解措施的针对 ARM64 的 Visual C++ MFC 支持";
            list2.Add(node54);
            DicNode node55 = new DicNode();
            node55.Parentname = "Others";
            node55.Name = "Microsoft.VisualStudio.Component.VC.Runtimes.ARM.Spectre";
            node55.Text = "面向 Spectre 的 VC++ 2022 版本 15.7 v14.14 库 (ARM)";
            list2.Add(node55);
            DicNode node56 = new DicNode();
            node56.Parentname = "Others";
            node56.Name = "Microsoft.VisualStudio.Component.VC.Runtimes.ARM64.Spectre";
            node56.Text = "面向 Spectre 的 VC++ 2022 版本 15.7 v14.14 库 (ARM64)";
            list2.Add(node56);
            DicNode node57 = new DicNode();
            node57.Parentname = "Others";
            node57.Name = "Microsoft.VisualStudio.Component.VC.Runtimes.x86.x64.Spectre";
            node57.Text = "面向 Spectre 的 VC++ 2022 版本 15.7 v14.14 库 (x86 和 x64)";
            list2.Add(node57);
            DicNode node58 = new DicNode();
            node58.Parentname = "Others";
            node58.Name = "Microsoft.VisualStudio.Component.VC.Tools.14.11";
            node58.Text = "VC++ 2022 版本 15.4 v14.11 工具集";
            list2.Add(node58);
            DicNode node59 = new DicNode();
            node59.Parentname = "Others";
            node59.Name = "Microsoft.VisualStudio.Component.VC.Tools.14.12";
            node59.Text = "VC++ 2022 版本 15.5 v14.12 工具集";
            list2.Add(node59);
            DicNode node60 = new DicNode();
            node60.Parentname = "Others";
            node60.Name = "Microsoft.VisualStudio.Component.VC.Tools.14.13";
            node60.Text = "VC++ 2022 版本 15.6 v14.13 工具集";
            list2.Add(node60);
            DicNode node61 = new DicNode();
            node61.Parentname = "Others";
            node61.Name = "Microsoft.VisualStudio.Component.VC.Tools.ARM64";
            node61.Text = "用于 ARM64 的 Visual C++ 编译器和库";
            list2.Add(node61);
            DicNode node62 = new DicNode();
            node62.Parentname = "Others";
            node62.Name = "Microsoft.VisualStudio.Component.Windows10SDK.16299.Desktop.arm";
            node62.Text = "适用于桌面 C++ [ARM 和 ARM64] 的 Windows 10 SDK";
            list2.Add(node62);
            this.componentList = list2;
            this.InitializeComponent();
            this.Init();
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

            string localFile = this.baseDownPath + @"\" + this.getFileNameByTypeCombox();

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

                string text2 = getFileNameByTypeCombox() + " --layout " + txtSaveDirectory.Text.Trim() + " " + text +
                               " --lang Zh-cn";
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

        //private void DownloadFile(string url, string saveFile, Action<int> downloadProgressChanged, Action downloadFileCompleted)
        //{
        //    WebClient client = new WebClient
        //    {
        //        Proxy = null
        //    };
        //    if (downloadProgressChanged != null)
        //    {
        //        client.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
        //        {
        //            object[] args = new object[] { e.ProgressPercentage };
        //            this.Invoke(downloadProgressChanged, args);
        //        };
        //    }
        //    if (downloadFileCompleted != null)
        //    {
        //        client.DownloadFileCompleted += (sender, e) => this.Invoke(downloadFileCompleted);
        //    }
        //    client.DownloadFileAsync(new Uri(url), saveFile);
        //}

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            new DirectoryInfo(Environment.CurrentDirectory).GetFiles(this.getFileNameByTypeCombox());
            //if (this.comboBox1.Text == "Visual Studio Community 2022")
            //{
            //    this.label3.Text = "社区版无需序列号";
            //}
            //else if (this.comboBox1.Text == "Visual Studio Professional 2022")
            //{
            //    this.label3.Text = "TD244-P4NB7-YQ6XK-Y8MMM-YWV2J";
            //}
            //else if (this.comboBox1.Text == "Visual Studio Enterprise 2022")
            //{
            //    this.label3.Text = "VHF9H-NXBBB-638P6-6JHCY-88JWH";
            //}
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private string getFileNameByTypeCombox()
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

        private void Init()
        {
            this.ReadStdOutput += new DelReadStdOutput(this.ReadStdOutputAction);
            this.ReadErrOutput += new DelReadErrOutput(this.ReadErrOutputAction);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode121 = new System.Windows.Forms.TreeNode("Visual Studio 核心编辑器");
            System.Windows.Forms.TreeNode treeNode122 = new System.Windows.Forms.TreeNode("Azure 开发");
            System.Windows.Forms.TreeNode treeNode123 = new System.Windows.Forms.TreeNode("数据存储和处理");
            System.Windows.Forms.TreeNode treeNode124 = new System.Windows.Forms.TreeNode("数据科学和分析应用程序");
            System.Windows.Forms.TreeNode treeNode125 = new System.Windows.Forms.TreeNode(".NET 桌面开发");
            System.Windows.Forms.TreeNode treeNode126 = new System.Windows.Forms.TreeNode("使用 Unity 的游戏开发");
            System.Windows.Forms.TreeNode treeNode127 = new System.Windows.Forms.TreeNode("使用 C++ 的 Linux 开发");
            System.Windows.Forms.TreeNode treeNode128 = new System.Windows.Forms.TreeNode("使用 C++ 的桌面开发");
            System.Windows.Forms.TreeNode treeNode129 = new System.Windows.Forms.TreeNode("使用 C++ 的游戏开发");
            System.Windows.Forms.TreeNode treeNode130 = new System.Windows.Forms.TreeNode("使用 C++ 的移动开发");
            System.Windows.Forms.TreeNode treeNode131 = new System.Windows.Forms.TreeNode(".NET Core 跨平台开发");
            System.Windows.Forms.TreeNode treeNode132 = new System.Windows.Forms.TreeNode("使用 .NET 的移动开发");
            System.Windows.Forms.TreeNode treeNode133 = new System.Windows.Forms.TreeNode("ASP.NET 和 Web 开发");
            System.Windows.Forms.TreeNode treeNode134 = new System.Windows.Forms.TreeNode("Node.js 开发");
            System.Windows.Forms.TreeNode treeNode135 = new System.Windows.Forms.TreeNode("Office/SharePoint 开发");
            System.Windows.Forms.TreeNode treeNode136 = new System.Windows.Forms.TreeNode("Python 开发");
            System.Windows.Forms.TreeNode treeNode137 = new System.Windows.Forms.TreeNode("通用 Windows 平台开发");
            System.Windows.Forms.TreeNode treeNode138 = new System.Windows.Forms.TreeNode("Visual Studio 扩展开发");
            System.Windows.Forms.TreeNode treeNode139 = new System.Windows.Forms.TreeNode("使用 JavaScript 的移动开发");
            System.Windows.Forms.TreeNode treeNode140 = new System.Windows.Forms.TreeNode("独立组件");
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSaveDirectory = new System.Windows.Forms.TextBox();
            this.btnSelectDir = new System.Windows.Forms.Button();
            this.cbxVersion = new System.Windows.Forms.ComboBox();
            this.btnDown = new System.Windows.Forms.Button();
            this.txtcmdLogTextArea = new System.Windows.Forms.TextBox();
            this.treeViewWorkload = new System.Windows.Forms.TreeView();
            this.btnBuild = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Directory：";
            // 
            // txtSaveDirectory
            // 
            this.txtSaveDirectory.Enabled = false;
            this.txtSaveDirectory.Location = new System.Drawing.Point(83, 37);
            this.txtSaveDirectory.Name = "txtSaveDirectory";
            this.txtSaveDirectory.Size = new System.Drawing.Size(330, 20);
            this.txtSaveDirectory.TabIndex = 2;
            // 
            // btnSelectDir
            // 
            this.btnSelectDir.Location = new System.Drawing.Point(419, 35);
            this.btnSelectDir.Name = "btnSelectDir";
            this.btnSelectDir.Size = new System.Drawing.Size(31, 25);
            this.btnSelectDir.TabIndex = 3;
            this.btnSelectDir.Text = "...";
            this.btnSelectDir.UseVisualStyleBackColor = true;
            this.btnSelectDir.Click += new System.EventHandler(this.btnSelectDir_Click);
            // 
            // cbxVersion
            // 
            this.cbxVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxVersion.FormattingEnabled = true;
            this.cbxVersion.Items.AddRange(new object[] {
            "Visual Studio Community 2022",
            "Visual Studio Professional 2022",
            "Visual Studio Enterprise 2022"});
            this.cbxVersion.Location = new System.Drawing.Point(83, 10);
            this.cbxVersion.Name = "cbxVersion";
            this.cbxVersion.Size = new System.Drawing.Size(330, 21);
            this.cbxVersion.TabIndex = 4;
            this.cbxVersion.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(422, 104);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(80, 25);
            this.btnDown.TabIndex = 6;
            this.btnDown.Text = "Start";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // txtcmdLogTextArea
            // 
            this.txtcmdLogTextArea.Location = new System.Drawing.Point(14, 72);
            this.txtcmdLogTextArea.Multiline = true;
            this.txtcmdLogTextArea.Name = "txtcmdLogTextArea";
            this.txtcmdLogTextArea.Size = new System.Drawing.Size(399, 110);
            this.txtcmdLogTextArea.TabIndex = 8;
            // 
            // treeViewWorkload
            // 
            this.treeViewWorkload.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewWorkload.CheckBoxes = true;
            this.treeViewWorkload.Location = new System.Drawing.Point(14, 194);
            this.treeViewWorkload.Name = "treeViewWorkload";
            treeNode121.Name = "CoreEditor";
            treeNode121.Text = "Visual Studio 核心编辑器";
            treeNode121.ToolTipText = "Visual Studio 核心 shell 体验，包括语法感知代码编辑、源代码管理和工作项管理。";
            treeNode122.Name = "Azure";
            treeNode122.Text = "Azure 开发";
            treeNode122.ToolTipText = "用于开发云应用、创建资源以及生成包括 Docker 支持的容器的 Azure SDK、工具和项目。";
            treeNode123.Name = "Data";
            treeNode123.Text = "数据存储和处理";
            treeNode123.ToolTipText = "使用 SQL Server、Azure Data Lake 或 Hadoop 连接、开发和测试数据解决方案。";
            treeNode124.Name = "DataScience";
            treeNode124.Text = "数据科学和分析应用程序";
            treeNode124.ToolTipText = "用于创建数据科学应用程序的语言和工具（包括 Python、R 和 F#）。";
            treeNode125.Name = "ManagedDesktop";
            treeNode125.Text = ".NET 桌面开发";
            treeNode125.ToolTipText = "使用 C#、Visual Basic 和 F# 生成 WPF、Windows 窗体和控制台应用程序。";
            treeNode126.Name = "ManagedGame";
            treeNode126.Text = "使用 Unity 的游戏开发";
            treeNode126.ToolTipText = "使用 Unity（功能强大的跨平台开发环境）创建 2D 和 3D 游戏。";
            treeNode127.Name = "NativeCrossPlat";
            treeNode127.Text = "使用 C++ 的 Linux 开发";
            treeNode127.ToolTipText = "创建和调试在 Linux 环境中运行的应用程序。";
            treeNode128.Name = "NativeDesktop";
            treeNode128.Text = "使用 C++ 的桌面开发";
            treeNode128.ToolTipText = "使用 Microsoft C++ 工具集、ATL 或 MFC 生成 Windows 桌面应用程序。";
            treeNode129.Name = "NativeGame";
            treeNode129.Text = "使用 C++ 的游戏开发";
            treeNode129.ToolTipText = "以 DirectX、Unreal 或 Cocos2d 为后盾，利用 C++ 的强大功能生成专业游戏。";
            treeNode130.Name = "NativeMobile";
            treeNode130.Text = "使用 C++ 的移动开发";
            treeNode130.ToolTipText = "使用 C++ 生成适用于 iOS、Android 或 Windows 的跨平台应用程序。";
            treeNode131.Name = "NetCoreTools";
            treeNode131.Text = ".NET Core 跨平台开发";
            treeNode131.ToolTipText = "使用 .NET Core、ASP.NET Core、HTML/JavaScript 和包括 Docker 支持的容器生成跨平台应用程序。";
            treeNode132.Name = "NetCrossPlat";
            treeNode132.Text = "使用 .NET 的移动开发";
            treeNode132.ToolTipText = "使用 Xmarin 生成适用于 iOS、Android 或 Windows 的跨平台应用程序。";
            treeNode133.Name = "NetWeb";
            treeNode133.Text = "ASP.NET 和 Web 开发";
            treeNode133.ToolTipText = "使用 ASP.NET、ASP.NET Core、HTML/JavaScript 和包括 Docker 支持的容器生成 Web 应用程序。";
            treeNode134.Name = "Node";
            treeNode134.Text = "Node.js 开发";
            treeNode134.ToolTipText = "使用 Node.js（事件驱动的异步 JavaScript 运行时）生成可扩展的网络应用程序。";
            treeNode135.Name = "Office";
            treeNode135.Text = "Office/SharePoint 开发";
            treeNode135.ToolTipText = "使用 C#、VB 和 JavaScript 创建 Office 和 SharePoint 外接程序、SharePoint 解决方案和 VSTO 外接程序。";
            treeNode136.Name = "Python";
            treeNode136.Text = "Python 开发";
            treeNode136.ToolTipText = "适用于 Python 的编辑、调试、交互式开发和源代码管理。";
            treeNode137.Name = "Universal";
            treeNode137.Text = "通用 Windows 平台开发";
            treeNode137.ToolTipText = "使用 C#、VB 和 JavaScript 或 C++（可选）创建适用于通用 Windows 平台的应用程序。";
            treeNode138.Name = "VisualStudioExtension";
            treeNode138.Text = "Visual Studio 扩展开发";
            treeNode138.ToolTipText = "创建适用于 Visual Studio 的加载项和扩展，包括新命令、代码分析器和工具窗口。";
            treeNode139.Name = "WebCrossPlat";
            treeNode139.Text = "使用 JavaScript 的移动开发";
            treeNode139.ToolTipText = "使用用于 Apache Cordova 的工具生成 Android、iOS 和 UWP 应用。";
            treeNode140.Name = "Others";
            treeNode140.Text = "独立组件";
            treeNode140.ToolTipText = "这些组件不随附于任何工作负载，但可选择作为单个组件。";
            this.treeViewWorkload.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode121,
            treeNode122,
            treeNode123,
            treeNode124,
            treeNode125,
            treeNode126,
            treeNode127,
            treeNode128,
            treeNode129,
            treeNode130,
            treeNode131,
            treeNode132,
            treeNode133,
            treeNode134,
            treeNode135,
            treeNode136,
            treeNode137,
            treeNode138,
            treeNode139,
            treeNode140});
            this.treeViewWorkload.ShowNodeToolTips = true;
            this.treeViewWorkload.Size = new System.Drawing.Size(488, 387);
            this.treeViewWorkload.TabIndex = 9;
            this.treeViewWorkload.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewWorkload_NodeMouseClick);
            // 
            // btnBuild
            // 
            this.btnBuild.Location = new System.Drawing.Point(422, 72);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(80, 25);
            this.btnBuild.TabIndex = 6;
            this.btnBuild.Text = "Initiate";
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 593);
            this.Controls.Add(this.treeViewWorkload);
            this.Controls.Add(this.txtcmdLogTextArea);
            this.Controls.Add(this.btnBuild);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.cbxVersion);
            this.Controls.Add(this.btnSelectDir);
            this.Controls.Add(this.txtSaveDirectory);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visual Studio Downloader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        private void linkClean_Click(object sender, EventArgs e)
        {
            if (this.cbxVersion.SelectedIndex == -1)
            {
                MessageBoxEx.Show(this, "请选择程序版本", "提示");
            }
            else if (this.txtSaveDirectory.Text.ToString() == "")
            {
                MessageBoxEx.Show(this, "请选择缓存目录", "提示");
            }
            else if (!Directory.Exists(this.txtSaveDirectory.Text.Trim()))
            {
                MessageBoxEx.Show(this, "缓存目录不存在，请检查后重试", "提示");
            }
            else
            {
                string fileName = "";
                DirectoryInfo[] directories = new DirectoryInfo(this.txtSaveDirectory.Text.ToString()).GetDirectories("Archive");
                if (directories.Length == 0)
                {
                    MessageBoxEx.Show(this, "当前缓存目录下文件无需清理", "提示");
                }
                else
                {
                    directories = directories[0].GetDirectories();
                    if (directories.Length == 0)
                    {
                        MessageBoxEx.Show(this, "当前缓存目录下文件无需清理", "提示");
                    }
                    else
                    {
                        if (directories.Length <= 1)
                        {
                            fileName = this.txtSaveDirectory.Text.Trim() + @"\Archive\" + directories[0].Name + @"\Catalog.json";
                        }
                        else
                        {
                            MessageBoxEx.Show("当前目录存在多个旧文件记录，请选择指定文件夹下Catalog.json文件，清理旧文件", "提示");
                            OpenFileDialog dialog = new OpenFileDialog {
                                InitialDirectory = this.txtSaveDirectory.Text + @"\Archive",
                                Multiselect = false
                            };
                            if (dialog.ShowDialog() != DialogResult.OK)
                            {
                                return;
                            }
                            fileName = dialog.FileName;
                        }
                        string[] textArray1 = new string[] { this.getFileNameByTypeCombox(), " --layout ", this.txtSaveDirectory.Text.Trim(), " --clean ", fileName };
                        string startCmd = string.Concat(textArray1);
                        this.RealAction(startCmd);
                    }
                }
            }
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
    }
}

