using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SourceChord.FluentWPF.Utility
{
    class IconHelper
    {
        private static ImageSource appIcon;
        public static ImageSource AppIcon
        {
            get
            {
                if (appIcon == null)
                {
                    var path = System.Reflection.Assembly.GetEntryAssembly().Location;
                    appIcon = GetIcon(path);
                }
                return appIcon;
            }
            protected set { appIcon = value; }
        }

        public static ImageSource GetIcon(string path)
        {
            var flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES | SHGFI_SMALLICON;

            var ret = SHGetFileInfo(path,
                                    FILE_ATTRIBUTE_NORMAL,
                                    out var shfi,
                                    (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                                    flags);

            if (ret != 0)
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(shfi.hIcon,
                                                                                  Int32Rect.Empty,
                                                                                  BitmapSizeOptions.FromEmptyOptions());
            }
            return null;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_LARGEICON = 0x000000000;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
    }
}
