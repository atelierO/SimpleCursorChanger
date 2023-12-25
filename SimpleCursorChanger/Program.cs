// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;

[DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);


const int SPI_SETCURSORS = 0x0057;
const int SPIF_UPDATEINIFILE = 0x01;
const int SPIF_SENDCHANGE = 0x02;


//형식: cursorsize or empty
string[] cmdArgs = Environment.GetCommandLineArgs();

if (cmdArgs.Length > 3)
{
    Environment.Exit(-1);
}

if(cmdArgs.Length == 3)
{
    if (int.TryParse(cmdArgs[1], out int cursorSize) == false)
    {
        Environment.Exit(-2);
    }

    if (cursorSize < 0)
    {
        Environment.Exit(-3);
    }
    int parsedCursorSize = 0x20 + 0x10 * (cursorSize - 1);
    //0x2029 not documented windows SPI number to change cursor size
    SystemParametersInfo(0x2029, 0, (uint)parsedCursorSize, 0x01);
    SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
}
if (cmdArgs.Length == 2)
{
    if (int.TryParse(cmdArgs[1], out int cursorSize) == false)
    {
        Environment.Exit(-2);
    }

    if (cursorSize < 0)
    {
        Environment.Exit(-3);
    }
    int parsedCursorSize = 0x20 + 0x10 * (cursorSize - 1);
    //0x2029 not documented windows SPI number to change cursor size
    SystemParametersInfo(0x2029, 0, (uint)parsedCursorSize, 0x01);
}
else if (cmdArgs.Length == 1)
{
    //레지스트리에 등록한 
    SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
}

Environment.Exit(0);


