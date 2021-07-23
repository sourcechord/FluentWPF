using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SourceChord.FluentWPF.Utility
{
    internal enum AccentFlagsType
    {
        Window = 0,
        Popup,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public uint GradientColor;
        public int AnimationId;
    }

    internal static class AcrylicHelper
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        internal static void SetBlur(IntPtr hwnd, AccentFlagsType style = AccentFlagsType.Window, AccentState? state = null)
        {
            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            accent.AccentState = state ?? SelectAccentState();

            if (style == AccentFlagsType.Window)
            {
                accent.AccentFlags = 2;
            }
            else
            {
                accent.AccentFlags = 0x20 | 0x40 | 0x80 | 0x100;
            }
            
            //accent.GradientColor = 0x99FFFFFF;  // 60%の透明度が基本
            accent.GradientColor = 0x00FFFFFF;  // Tint Colorはここでは設定せず、Bindingで外部から変えられるようにXAML側のレイヤーとして定義

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(hwnd, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        internal static AccentState SelectAccentState(AcrylicAccentState state = AcrylicAccentState.Default)
        {
            // ウィンドウのアクリル効果を設定する
            AccentState value = state switch
            {
                // ウィンドウ背景のぼかしを行うのはWindows10の場合のみ
                // OSのバージョンに従い、AccentStateを切り替える
                AcrylicAccentState.Default => SystemInfo.Version.Value switch
                {
                    // Windows11環境ではアクリル効果を無効にする
                    var version when version >= VersionInfos.Windows11_Preview => AccentState.ACCENT_ENABLE_GRADIENT,
                    // Windows10 1903以降では、ACCENT_ENABLE_ACRYLICBLURBEHINDを用いると、ウィンドウのドラッグ移動などでマウス操作に追従しなくなる。
                    // ウィンドウの移動/リサイズ中だけ、ACCENT_ENABLE_ACRYLICBLURBEHINDを無効にして、この問題を回避する
                    //var version when version >= VersionInfos.Windows10_1903 => AccentState.ACCENT_ENABLE_BLURBEHIND,
                    var version when version >= VersionInfos.Windows10_1809 => AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                    var version when version >= VersionInfos.Windows10 => AccentState.ACCENT_ENABLE_BLURBEHIND,
                    _ => AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT,
                },
                AcrylicAccentState.Disabled => AccentState.ACCENT_DISABLED,
                AcrylicAccentState.Gradient => AccentState.ACCENT_ENABLE_GRADIENT,
                AcrylicAccentState.TransparentGradient => AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT,
                AcrylicAccentState.BlurBehind => AccentState.ACCENT_ENABLE_BLURBEHIND,
                AcrylicAccentState.AcrylicBlurBehind => AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                _ => throw new InvalidOperationException(),
            };

            return value;
        }
    }
}
