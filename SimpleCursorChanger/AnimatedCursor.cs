using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCursorChanger
{
    // 이름이 AnimatedCursor지만 현재 애니메이션으로 커서 이미지를 출력하진 않는다
    // 애니메이션 커서를 출력하는 것을 목표로 잡은 네이밍ㅎㅎ
    internal class AnimatedCursor
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }
        [DllImport("user32")]
        private static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO pIconInfo);
        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(String str);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static Bitmap GetBitmapFromCursorFile(string filename)
        {
            Cursor cursor;
            // how to handle cursor.
            IntPtr HC = LoadCursorFromFile(filename);
            // Check  wheather it is succeeded or not.
            if (!IntPtr.Zero.Equals(HC))
            {
                cursor = new Cursor(HC);
            }
            else
            {
                throw new ApplicationException("cursor was not created from file " + filename);
            }
            return BitmapFromCursor(cursor);
        }

        private static Bitmap BitmapFromCursor(Cursor cur)
        {
            ICONINFO ii;
            GetIconInfo(cur.Handle, out ii);

            Bitmap bmp = Bitmap.FromHbitmap(ii.hbmColor);
            DeleteObject(ii.hbmColor);
            DeleteObject(ii.hbmMask);

            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            Bitmap dstBitmap = new Bitmap(bmData.Width, bmData.Height, bmData.Stride, PixelFormat.Format32bppArgb, bmData.Scan0);
            bmp.UnlockBits(bmData);

            //ani커서 대응은 조금 생각해보자
            return new Bitmap(dstBitmap);
        }
    }
}
