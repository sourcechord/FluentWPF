using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace SourceChord.FluentWPF
{
    public abstract class ThemeHandler
    {
        protected static ThemeHandler Instance { get; set; }

        static ThemeHandler()
        {
            // 初期化メソッドを呼ぶ
            var win = Application.Current.MainWindow;
            if (win != null)
            {
                Initialize(win);
            }
            else
            {
                EventHandler handler = null;
                handler = (e, args) =>
                {
                    if (Application.Current.MainWindow != null)
                    {
                        Initialize(Application.Current.MainWindow);
                    }
                    Application.Current.Activated -= handler;
                };
                Application.Current.Activated += handler;
            }
        }

        public ThemeHandler()
        {
            WndProcEvent += this.WndProc;
        }

        public static void Initialize(Window win)
        {
            if (win.IsLoaded)
            {
                InitializeCore(win);
            }
            else
            {
                win.Loaded += (_, __) =>
                {
                    InitializeCore(win);
                };
            }
        }

        protected static void InitializeCore(Window win)
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(win).Handle);
            source.AddHook(WndProcCore);
        }

        public delegate IntPtr WndProcEventHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
        protected static event WndProcEventHandler WndProcEvent;
        private static IntPtr WndProcCore(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            WndProcEvent?.Invoke(hwnd, msg, wParam, lParam, ref handled);
            return IntPtr.Zero;
        }


        protected abstract IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
    }
}
