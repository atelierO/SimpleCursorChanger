using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;


namespace SimpleCursorChangerGUI
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        const int SPI_SETCURSORS = 0x0057;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;

        const string regKey = "HKEY_CURRENT_USER\\Control Panel\\Cursors";
        const string WIN_CUR_PATH = "C:\\Windows\\Cursors";

        readonly string[] CURSOR_NAMES = {
            "Arrow",        //일반 선택 Normal Select
            "Help",         //도움말 선택 Help Select
            "AppStarting",  //백그라운드 작업 Working in Background
            "Wait",         //사용 중 Busy
            "Crosshair",    //정밀도 선택 Precision Select
            "IBeam",        //텍스트 선택 Text Select
            "NWPen",        //필기 Handwriting
            "No",           //사용할 수 없음 Unavailable
            "SizeNS",       //수직 크기 조절 Vertical Resize
            "SizeWE",       //수평 크기 조절 Horizontal Resize
            "SizeNWSE",     //대각선 방향 크기 조절 1 Diagonal Resize 1
            "SizeNESW",     //대각선 방향 크기 조절 2 Diagonal Resize 2
            "SizeAll",      //이동 Move
            "UpArrow",      //대체 선택 Alternate Select
            "Hand",         //연결 선택 Link Select
            "Pin",          //위치 선택 Location Selet
            "Person"        //사용자 선택 Person Select
        };

        public MainForm()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Directory.GetCurrentDirectory() + "\\";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = fbd.SelectedPath;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            int cursorSize = Int32.Parse(cbCursorSize.Text);
            int parsedCursorSize = 0x20 + 0x10 * (cursorSize - 1);
            //0x2029 not documented windows SPI number to change cursor size
            SystemParametersInfo(0x2029, 0, (uint)parsedCursorSize, 0x01);

            if (tbPath.Text == "")
                return;


            string folder_name = new DirectoryInfo(tbPath.Text).Name;
            string cursor_shceme = "";

            foreach (string val in CURSOR_NAMES)
            {
                //C:\\등 경로일 경우 일반 경로와 다르게 \\를 포함하고 있기 때문에 \\를 삭제
                if (tbPath.Text.Substring(tbPath.Text.Length-1).Equals("\\"))
                {
                    tbPath.Text = tbPath.Text.Substring(0, tbPath.Text.Length - 2);
                }
                string cursorPath = tbPath.Text + "\\" + val;
                string ext = "";
                bool fileExist = false;
                if(File.Exists(cursorPath+".cur"))
                {
                    ext = ".cur";
                    fileExist = true;
                }
                if(File.Exists(cursorPath+".ani"))
                {
                    ext = ".ani";
                    fileExist = true;

                }
                if (fileExist)
                {
                    string currentCursorPath = WIN_CUR_PATH + '\\' + folder_name + "_" + val + ext;
                    cursor_shceme += currentCursorPath + ",";
                    File.Copy(cursorPath + ext, currentCursorPath, true);
                    Registry.SetValue(regKey, val, currentCursorPath);
                }
                else cursor_shceme += ",";
            }
            
            Registry.SetValue(regKey + "\\Schemes", folder_name, cursor_shceme.TrimEnd(),RegistryValueKind.ExpandString);
            SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach (string val in CURSOR_NAMES)
            {
                Registry.SetValue(regKey, val, "");
            }
            SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            SystemParametersInfo(0x2029, 0, 0x20, 0x01);
        }
    }
}
