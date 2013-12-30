using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;

namespace nonowaWindowResize
{
    class EnumWindow
    {
        private delegate int EnumWindowsDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsDelegate lpEnumFunc, int lParam);
        [DllImport("user32.dll")]
        private static extern int IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("User32.Dll")]
        private static extern int GetWindowRect(
            IntPtr hWnd,      // ウィンドウのハンドル
            out RECT rect   // ウィンドウの座標値
            );

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int Length;
            public int flags;
            public int showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }


        [DllImport("user32.dll")]
        extern private static bool
          GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPlacement(IntPtr hWnd,
           [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern int MoveWindow(IntPtr hwnd, int x, int y,
            int nWidth, int nHeight, int bRepaint);

        Hashtable ht = new Hashtable();

        public void save()
        {
            EnumWindows(new EnumWindowsDelegate(delegate(IntPtr hWnd, int lParam)
            {
                StringBuilder sb = new StringBuilder(0x1024);
                if (IsWindowVisible(hWnd) != 0 && GetWindowText(hWnd, sb, sb.Capacity) != 0)
                {
                    try
                    {
                        string title = sb.ToString();
                        int pid;
                        GetWindowThreadProcessId(hWnd, out pid);
                        Process p = Process.GetProcessById(pid);

                        WINDOWPLACEMENT wndpl = new WINDOWPLACEMENT();
                        wndpl.Length = Marshal.SizeOf(wndpl);

                        GetWindowPlacement(hWnd, ref wndpl);

                        ht[hWnd] = wndpl;
                    }
                    catch
                    {
                    }
                }
                return 1;
            }), 0);
        }

        public void load()
        {
            Debug.Print("" + ht.Values.Count);
            foreach (DictionaryEntry de in ht)
            {
                try
                {
                    IntPtr hWnd = (IntPtr)de.Key;
                    if (IsWindowVisible(hWnd) == 0) continue;
                    WINDOWPLACEMENT wp = (WINDOWPLACEMENT)de.Value;
                    SetWindowPlacement(hWnd, ref wp);

                    Debug.Print("" + wp.showCmd);
                    if (wp.showCmd == 1)
                    {
                        WINDOWPLACEMENT wp2 = new WINDOWPLACEMENT();
                        GetWindowPlacement(hWnd, ref wp2);
                        if (wp2.rcNormalPosition.left == wp.rcNormalPosition.left)
                        {
                            RECT r = wp.rcNormalPosition;
                            MoveWindow(hWnd, r.left, r.top, r.right - r.left, r.bottom - r.top, 1);
                        }
                    }
                }
                catch
                {
                }

            }
        }
    }
}
