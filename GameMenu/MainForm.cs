using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameMenu
{
    public partial class MainForm : Form
    {

        static string ApplicationDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string ApplicationDir = ApplicationDataDir + "/aaron-am";
        string menuFilePath = ApplicationDir + "/menu.dat";

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public enum SHGFI
        {
            SHGFI_ICON = 0x100,
            SHGFI_LARGEICON = 0x0,
            SHGFI_USEFILEATTRIBUTES = 0x10
        }


        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public MainForm()
        {
            InitializeComponent();

            InitData();
        }

        private void SaveListToFile()
        {
            // 写入文件
            FileStream fileStream = File.Open(menuFilePath, FileMode.OpenOrCreate);
            ArrayList menu = new ArrayList();
            foreach (ListViewItem i in this.listView1.Items)
            {
                Dictionary<string, object> menuItem = new Dictionary<string, object>();
                menuItem.Add("name", i.Text);
                menuItem.Add("path", i.Tag);
                menuItem.Add("index", i.ImageIndex);
                menu.Add(menuItem);
            }

            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(fileStream, menu);
            fileStream.Close();
        }

        private void InitData()
        {
            if (!Directory.Exists(ApplicationDir))
            {
                Directory.CreateDirectory(ApplicationDir);
            }

            // 打开文件,读取listview数据
            FileStream fileStream = new FileStream(menuFilePath, FileMode.OpenOrCreate);

            if(fileStream.Length == 0)
            {
                fileStream.Close();
                return;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            ArrayList menu = (ArrayList)formatter.Deserialize(fileStream);
            fileStream.Close();

            // 初始化liestview
            foreach (Dictionary<string, object> menuItem in menu)
            {
                string name = (string)menuItem["name"];
                string path = (string)menuItem["path"];
                int index = (int)menuItem["index"];

                SHFILEINFO _SHFILEINFO = new SHFILEINFO();
                IntPtr _IconIntPtr = SHGetFileInfo(path, 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), (uint)(SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON | SHGFI.SHGFI_USEFILEATTRIBUTES));
                Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);

                this.listView1.BeginUpdate();
                this.listView1.LargeImageList.Images.Add(_Icon.ToBitmap());
                ListViewItem item = new ListViewItem();
                item.ImageIndex = index;
                item.Tag = path;
                item.Text = path;
                this.listView1.Items.Add(item);
                this.listView1.EndUpdate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SHFILEINFO _SHFILEINFO = new SHFILEINFO();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(openFileDialog.FileName);
                string filename = openFileDialog.FileName;
                IntPtr _IconIntPtr = SHGetFileInfo(filename, 0, ref _SHFILEINFO, (uint)Marshal.SizeOf(_SHFILEINFO), (uint)(SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON | SHGFI.SHGFI_USEFILEATTRIBUTES));
                Icon _Icon = System.Drawing.Icon.FromHandle(_SHFILEINFO.hIcon);

                this.listView1.BeginUpdate();
                this.listView1.LargeImageList.Images.Add(_Icon.ToBitmap());
                ListViewItem item = new ListViewItem();
                item.ImageIndex = this.listView1.LargeImageList.Images.Count - 1;
                item.Tag = filename;
                item.Text = filename;
                this.listView1.Items.Add(item);
                this.listView1.EndUpdate();

                this.SaveListToFile();
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
