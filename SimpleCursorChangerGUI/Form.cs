using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SimpleCursorChangerGUI
{
    public partial class MainForm : Form
    {

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        const int SPI_SETCURSORS = 0x0057;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;

        //0x2029 not documented windows SPI number to change cursor size
        const int SPI_SETCURSORSIZE = 0x2029;


        const string REG_CURSOR_PATH = "HKEY_CURRENT_USER\\Control Panel\\Cursors";
        const string REG_CURSOR_SCHEME_PATH = $"{REG_CURSOR_PATH}\\Schemes";

        readonly string PROGRAM_IMAGE_DIRECTORY = Environment.GetEnvironmentVariable("USERPROFILE")! + "\\.simplecursorchanger";
        readonly string PROGRAM_IMAGE_WORKING_DIRECTORY = Environment.GetEnvironmentVariable("USERPROFILE")! + "\\.simplecursorchanger\\current";
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

        struct CursorElement
        {
            public string Name { get; }
            public string Path { get; }
            public CursorElement(string name, string path)
            {
                Name = name;
                Path = path;
            }
        }

        struct CursorScheme
        {
            public string Name { get; }
            public CursorElement[] Elements { get; }
            public CursorScheme(string name, CursorElement[] elements)
            {
                Name = name;
                Elements = elements;
            }
        }

        List<CursorScheme> globalCursorScheme = new List<CursorScheme>();
        int globalCursorSize = 0;

        public MainForm()
        {
            InitializeComponent();
            CenterToScreen();
            Initialize();
        }

        #region UserMethods
        void Initialize()
        {
            // 드래그 드롭 이벤트 등록
            foreach (var name in CURSOR_NAMES)
            {
                PictureBox inputPicBox = GetInputPicBox(name)!;
                inputPicBox.DragEnter += inputPicBox_DragEnter;
                inputPicBox.DragDrop += inputPicBox_DragDrop;
            }
            // 현재 커서의 사이즈 읽기
            if (TryGetCursorSize(out globalCursorSize) == false)
            {
                MessageBox.Show("커서 사이즈 레지스트리를 읽을 수 없습니다");
            }
            cbCursorSize.SelectedIndex = globalCursorSize - 1;

            // 현재 커서의 이미지 로드해서 추가
            LoadCurrentCursorImagePath();

            // 유저 폴더에 커서 이미지 저장용 디렉토리 생성
            if (Directory.Exists(PROGRAM_IMAGE_DIRECTORY) == false)
            {
                Directory.CreateDirectory(PROGRAM_IMAGE_DIRECTORY);
            }

            if (Directory.Exists(PROGRAM_IMAGE_WORKING_DIRECTORY) == false)
            {
                Directory.CreateDirectory(PROGRAM_IMAGE_WORKING_DIRECTORY);
            }

            // 커서 스키마 로딩하기
            var hiveKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            var subKey = hiveKey.OpenSubKey("Control Panel")!
                .OpenSubKey("Cursors")!
                .OpenSubKey("Schemes")!;

            var keyValues = subKey.GetValueNames();

            foreach (var keyVal in keyValues)
            {
                var keyData = subKey.GetValue(keyVal) as string;
                if (keyData == null)
                {
                    continue;
                }

                var paths = keyData.Split(",");
                CursorElement[] elements = new CursorElement[CURSOR_NAMES.Length];
                for (int i = 0; i < CURSOR_NAMES.Length; i++)
                {
                    elements[i] = new CursorElement(CURSOR_NAMES[i], paths[i]);
                }

                // 이렇게도 할 수 있음 
                //CURSOR_NAMES.Select((name, index) => new CursorElement(name, paths[index])).ToArray();

                globalCursorScheme.Add(new CursorScheme(keyVal, elements));
                cbRegisterSystem.Items.Add(keyVal);
            }
        }

        private void AddCursorScheme(CursorScheme scheme)
        {
            globalCursorScheme.Add(scheme);
            cbRegisterSystem.Items.Add(scheme.Name);
        }

        private void RemoveCursorScheme(string name)
        {
            //TODO: 이 코드가 잘 작동하는지 디버그 모드로 확인해야함
            globalCursorScheme.Remove(globalCursorScheme.Find(scheme => scheme.Name == name));
            cbRegisterSystem.Items.Remove(name);
        }

        private void LoadCurrentCursorImagePath()
        {
            int currCursorSize = globalCursorSize;
            UpdateCursorSize(1);

            foreach (var name in CURSOR_NAMES)
            {
                string? cursorPath = Registry.GetValue(REG_CURSOR_PATH, name, null) as string;
                if (cursorPath == null)
                {
                    continue;
                }
                UpdateInputPicBox(name, cursorPath);
            }

            UpdateCursorSize(currCursorSize);
        }

        private string GetNameFromInputPicBox(PictureBox pictureBox)
        {
            string[] splits = pictureBox.Name.Split("_");
            if (splits.Length != 2)
            {
                return "";
            }
            return splits[1];
        }

        private bool TryGetCursorSize(out int cursorSize)
        {
            cursorSize = 0;
            object? regValue = Registry.GetValue(REG_CURSOR_PATH, "CursorBaseSize", 32);
            if (regValue == null)
            {
                return false;
            }

            cursorSize = (int)regValue;

            cursorSize = (cursorSize - 0x20) / 0x10 + 1;
            if (cursorSize <= 0)
            {
                return false;
            }
            return true;
        }

        private PictureBox? GetInputPicBox(string name)
        {
            Control[] control = this.Controls.Find("inputPicBox_" + name, true);
            return control[0] as PictureBox;
        }

        private void UpdateInputPicBox(string name, string filePath)
        {
            PictureBox? pictureBox = GetInputPicBox(name);
            if (pictureBox == null)
                return;

            try
            {
                pictureBox.Image = AnimatedCursor.GetBitmapFromCursorFile(filePath);
            }
            // Exception 무시하기
            catch (ApplicationException)
            {
                return;
            }
        }

        private void InitInputPicBox(string name)
        {

            PictureBox? pictureBox = GetInputPicBox(name);
            if (pictureBox == null)
                return;
            pictureBox.Image = Properties.Resources.add;
        }

        private void UpdateCursorSize(int cursorSize)
        {
            int parsedCursorSize = 0x20 + 0x10 * (cursorSize - 1);
            if (parsedCursorSize <= 0)
            {
                MessageBox.Show("잘못된 커서 사이즈!");
                return;
            }

            globalCursorSize = cursorSize;
            SystemParametersInfo(SPI_SETCURSORSIZE, 0, (uint)parsedCursorSize, 0x01);
        }

        private void UpdateCursorImageRegistry(string name, string filePath, bool updateSystem = true)
        {
            if (CURSOR_NAMES.Contains(name) == false)
            {
                return;
            }
            //cursorInfos[name] = filePath;
            Registry.SetValue(REG_CURSOR_PATH, name, filePath);

            if (updateSystem == true)
            {
                SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
        }

        private string CopyCursorImageToUserFolder(string name, string filePath, string folderName = "current")
        {
            if (File.Exists(filePath) == false)
            {
                MessageBox.Show($"CopyCursorImageToUserFolder - File Not Exist : {filePath}");
                return "";
            }
            string copyingPath = $"{PROGRAM_IMAGE_DIRECTORY}\\{folderName}\\{name}{Path.GetExtension(filePath)}";
            File.Copy(filePath, copyingPath, true);
            return copyingPath;
        }

        #endregion

        #region EventMethods
        private void btnCursorSizeApply_Click(object sender, EventArgs e)
        {
            if (int.TryParse(cbCursorSize.Text, out int cursorSize) == false)
            {
                MessageBox.Show("잘못된 커서 사이즈입니다!");
                return;
            }
            UpdateCursorSize(cursorSize);
        }

        private void btnRegisterSystem_Click(object sender, EventArgs e)
        {
            if (tbRegisterSystem.Text == "")
                return;

            if (cbRegisterSystem.Items.Contains(tbRegisterSystem.Text))
            {
                MessageBox.Show("같은 이름의 구성목록이 이미 존재합니다!");
                return;
            }

            // \ / : * ? " < > |  
            char[] folderRestrictCharacter = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            bool isRestrictName = false;
            foreach (char c in folderRestrictCharacter)
            {
                isRestrictName |= tbRegisterSystem.Text.IndexOf(c) > 0;
            }

            if (isRestrictName)
            {
                MessageBox.Show("폴더이름으로 사용할 수 없는 문자가 있습니다!");
                return;
            }


            Directory.CreateDirectory($"{PROGRAM_IMAGE_DIRECTORY}\\{tbRegisterSystem.Text}");

            string cursor_shceme = "";

            //TODO: capacity넣어서 잘 작동하는지 확인하기 + 뭔가 이상하다 filePath로 넘어가는 인수가 맞는가?
            List<CursorElement> cursorElements = new List<CursorElement>();
            foreach (string name in CURSOR_NAMES)
            {
                string cursorPath = $"{PROGRAM_IMAGE_WORKING_DIRECTORY}\\{name}";
                if (File.Exists($"{cursorPath}.cur"))
                    cursorPath += ".cur";
                if (File.Exists($"{cursorPath}.ani"))
                    cursorPath += ".ani";

                if (File.Exists(cursorPath))
                {
                    string copiedPath = CopyCursorImageToUserFolder(name, cursorPath, tbRegisterSystem.Text);
                    Registry.SetValue(REG_CURSOR_PATH, name, copiedPath);
                    cursor_shceme += $"{copiedPath},";
                    cursorElements.Add(new CursorElement(name, copiedPath));
                }
                else
                {
                    cursor_shceme += ",";
                    cursorElements.Add(new CursorElement(name, ""));
                }
            }

            Registry.SetValue(REG_CURSOR_SCHEME_PATH, tbRegisterSystem.Text, cursor_shceme.TrimEnd(), RegistryValueKind.ExpandString);



            AddCursorScheme(new CursorScheme(tbRegisterSystem.Text, cursorElements.ToArray()));
            cbRegisterSystem.SelectedItem = tbRegisterSystem.Text;

            tbRegisterSystem.Text = "";

            MessageBox.Show($"{tbRegisterSystem.Text} 등록완료!");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach (string val in CURSOR_NAMES)
            {
                Registry.SetValue(REG_CURSOR_PATH, val, "");
                Control[] control = this.Controls.Find("inputPicBox_" + val, true);
                PictureBox pictureBox = (PictureBox)control[0];
                pictureBox.Image = Properties.Resources.add;
            }
            tbFolderPath.Text = "";
            SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        private void inputPicBox_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void inputPicBox_DragDrop(object? sender, DragEventArgs e)
        {
            string[]? files = (string[]?)e.Data!.GetData(DataFormats.FileDrop);
            if (files == null)
                return;
            if (files.Length == 0)
            {
                MessageBox.Show("파일이 없어요!");
                return;
            }
            else if (files.Length > 1)
            {
                MessageBox.Show("파일이 너무 많아요!");
                return;
            }

            string filePath = files[0];

            if (File.Exists(filePath) == false)
            {
                MessageBox.Show("존재하지 않는 파일이에요!");
                return;
            }

            if (Path.GetExtension(filePath).ToLower() != ".cur" &&
                Path.GetExtension(filePath).ToLower() != ".ani")
            {
                MessageBox.Show("잘못된 파일 형식이에요! .cur과 .ani파일만 넣어주세요");
                return;
            }

            if (sender == null)
            {
                MessageBox.Show("Drop Target Not Exists");
                return;
            }

            PictureBox pictureBox = (PictureBox)sender;
            string name = GetNameFromInputPicBox(pictureBox);

            //커서 사이즈가 크면 이미지가 깨지는데 이유를 찾을 수 없어서
            //일단 꼼수로 해결해둠 
            //커서 사이즈를 1로 바꾸면 해결되서 1로 바꾼후 다시 크게 바꿔야한다
            int currCursorSize = globalCursorSize;
            UpdateCursorSize(1);
            string copiedPath = CopyCursorImageToUserFolder(name, filePath);
            UpdateInputPicBox(name, copiedPath);
            UpdateCursorSize(currCursorSize);
            UpdateCursorImageRegistry(name, copiedPath);
        }

        private void lbFolderExample_Click(object sender, EventArgs e)
        {
            FormLoadFolderExample form = new FormLoadFolderExample();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void lbRegExample_Click(object sender, EventArgs e)
        {
            FormRegExample form = new FormRegExample();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void btnFolderPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "모든 커서 파일 (*.cur,*ani) |*.cur;*.ani|커서 파일 (*.cur)|*.cur|애니메이션 커서 파일(*.ani)|*.ani";
            ofd.Multiselect = true;
            ofd.Title = "커서 파일 선택하기";
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            //커서 사이즈가 크면 이미지가 깨지는데 이유를 찾을 수 없어서
            //일단 꼼수로 해결해둠 
            //커서 사이즈를 1로 바꾸면 해결되서 1로 바꾼후 다시 크게 바꿔야한다
            int currCursorSize = globalCursorSize;
            UpdateCursorSize(1);
            foreach (string filePath in ofd.FileNames)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (CURSOR_NAMES.Contains(fileName))
                {
                    string copiedPath = CopyCursorImageToUserFolder(fileName, filePath);
                    UpdateInputPicBox(fileName, copiedPath);
                    UpdateCursorImageRegistry(fileName, copiedPath, updateSystem: false);
                }
            }
            UpdateCursorSize(currCursorSize);
            SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        #endregion

        private void btnDeleteRegSystem_Click(object sender, EventArgs e)
        {
            if (cbRegisterSystem.SelectedItem.ToString() == "")
                return;

            var key = Registry.CurrentUser.OpenSubKey("Control Panel")!
                .OpenSubKey("Cursors")!
                .OpenSubKey("Schemes", writable: true)!;

            var valueName = cbRegisterSystem.SelectedItem.ToString()!;
            key.DeleteValue(valueName);

            //cbRegisterSystem.Items.Remove(cbRegisterSystem.SelectedItem);
            RemoveCursorScheme(cbRegisterSystem.SelectedItem.ToString()!);
        }

        private void cbRegisterSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedComp = cbRegisterSystem.SelectedItem.ToString();
            var selectedScheme = globalCursorScheme.Find(scheme => scheme.Name == selectedComp);

            int currCursorSize = globalCursorSize;
            UpdateCursorSize(1);
            foreach (var element in selectedScheme.Elements)
            {
                if (element.Path != "")
                {
                    string copiedPath = CopyCursorImageToUserFolder(element.Name, element.Path);
                    UpdateInputPicBox(element.Name, copiedPath);
                    UpdateCursorImageRegistry(element.Name, copiedPath, updateSystem: false);
                }
                else
                {
                    InitInputPicBox(element.Name);
                    UpdateCursorImageRegistry(element.Name, "", updateSystem: false);
                }
            }
            UpdateCursorSize(currCursorSize);
            SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }
    }
}
