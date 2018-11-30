using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SourceChord.FluentWPF.Utility
{
    static class AcrylicHelper
    {
        internal static void EnableBlur(IntPtr hwnd)
        {
            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            // ウィンドウ背景のぼかしを行うのはWindows10の場合のみ
            // OSのバージョンに従い、AccentStateを切り替える
            var currentVersion = SystemInfo.Version.Value;
            if (currentVersion >= VersionInfos.Windows10_1809)
            {
                accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND; //AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            }
            else if (currentVersion >= VersionInfos.Windows10)
            {
                accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
            }
            else
            {
                accent.AccentState = AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT;
            }

            accent.AccentFlags = 0x20 | 0x40 | 0x80 | 0x100;
            //accent.GradientColor = 0x99FFFFFF;  // 60%の透明度が基本
            accent.GradientColor = 0x00FFFFFF;  // Tint Colorはここでは設定せず、Bindingで外部から変えられるようにXAML側のレイヤーとして定義

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            AcrylicWindow.SetWindowCompositionAttribute(hwnd, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
    }
}
