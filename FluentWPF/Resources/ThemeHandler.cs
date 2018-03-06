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
        }

        public ThemeHandler()
        {
            // 初期化メソッドを呼ぶ
            var win = Application.Current.MainWindow;
            if (win != null)
            {
                this.Initialize(win);
            }
            else
            {
                EventHandler handler = null;
                handler = (e, args) =>
                {
                    this.Initialize(Application.Current.MainWindow);
                    Application.Current.Activated -= handler;
                };
                Application.Current.Activated += handler;
            }
        }

        private void Initialize(Window win)
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

        protected virtual void InitializeCore(Window win)
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(win).Handle);
            source.AddHook(this.WndProc);
        }

        protected abstract IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
    }
}
