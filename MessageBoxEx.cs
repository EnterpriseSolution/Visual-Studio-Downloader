namespace VisualStudioDownloader
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    public class MessageBoxEx
    {
        private static IWin32Window _owner;
        private static HookProc _hookProc = new HookProc(MessageBoxEx.MessageBoxHookProc);
        private static IntPtr _hHook = IntPtr.Zero;
        public const int WH_CALLWNDPROCRET = 12;

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
        private static void CenterWindow(IntPtr hChildWnd)
        {
            Rectangle lpRect = new Rectangle(0, 0, 0, 0);
            GetWindowRect(hChildWnd, ref lpRect);
            int nWidth = lpRect.Width - lpRect.X;
            int nHeight = lpRect.Height - lpRect.Y;
            Rectangle rectangle2 = new Rectangle(0, 0, 0, 0);
            GetWindowRect(_owner.Handle, ref rectangle2);
            Point point = new Point(0, 0) {
                X = rectangle2.X + ((rectangle2.Width - rectangle2.X) / 2),
                Y = rectangle2.Y + ((rectangle2.Height - rectangle2.Y) / 2)
            };
            Point point2 = new Point(0, 0) {
                X = point.X - (nWidth / 2),
                Y = point.Y - (nHeight / 2)
            };
            point2.X = (point2.X < 0) ? 0 : point2.X;
            point2.Y = (point2.Y < 0) ? 0 : point2.Y;
            MoveWindow(hChildWnd, point2.X, point2.Y, nWidth, nHeight, false);
        }

        [DllImport("user32.dll")]
        public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        private static void Initialize()
        {
            if (_hHook != IntPtr.Zero)
            {
                throw new NotSupportedException("multiple calls are not supported");
            }
            if (_owner != null)
            {
                _hHook = SetWindowsHookEx(12, _hookProc, IntPtr.Zero, AppDomain.GetCurrentThreadId());
            }
        }

        private static IntPtr MessageBoxHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(_hHook, nCode, wParam, lParam);
            }
            CWPRETSTRUCT cwpretstruct = (CWPRETSTRUCT) Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
            IntPtr idHook = _hHook;
            if (cwpretstruct.message == 5)
            {
                try
                {
                    CenterWindow(cwpretstruct.hwnd);
                }
                finally
                {
                    UnhookWindowsHookEx(_hHook);
                    _hHook = IntPtr.Zero;
                }
            }
            return CallNextHookEx(idHook, nCode, wParam, lParam);
        }

        [DllImport("user32.dll")]
        private static extern int MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("User32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("User32.dll")]
        public static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        public static DialogResult Show(string text)
        {
            Initialize();
            return MessageBox.Show(text);
        }

        public static DialogResult Show(string text, string caption)
        {
            Initialize();
            return MessageBox.Show(text, caption);
        }

        public static DialogResult Show(IWin32Window owner, string text)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            Initialize();
            return MessageBox.Show(text, caption, buttons);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Initialize();
            return MessageBox.Show(text, caption, buttons, icon);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption, buttons);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton)
        {
            Initialize();
            return MessageBox.Show(text, caption, buttons, icon, defButton);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption, buttons, icon);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, MessageBoxOptions options)
        {
            Initialize();
            return MessageBox.Show(text, caption, buttons, icon, defButton, options);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption, buttons, icon, defButton);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, MessageBoxOptions options)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption, buttons, icon, defButton, options);
        }

        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr idHook);

        public enum CbtHookAction
        {
            HCBT_MOVESIZE,
            HCBT_MINMAX,
            HCBT_QS,
            HCBT_CREATEWND,
            HCBT_DESTROYWND,
            HCBT_ACTIVATE,
            HCBT_CLICKSKIPPED,
            HCBT_KEYSKIPPED,
            HCBT_SYSCOMMAND,
            HCBT_SETFOCUS
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate void TimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);
    }
}

