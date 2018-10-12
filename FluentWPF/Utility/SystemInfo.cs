using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceChord.FluentWPF.Utility
{
    class SystemInfo
    {
        public static Lazy<VersionInfo> Version { get; private set; } = new Lazy<VersionInfo>(() => GetVersionInfo());


        internal static VersionInfo GetVersionInfo()
        {
            using (var mc = new System.Management.ManagementClass("Win32_OperatingSystem"))
            using (var moc = mc.GetInstances())
            {
                foreach (System.Management.ManagementObject mo in moc)
                {
                    // majar/minor/buildの番号を取得
                    var version = mo["Version"] as string;
                    var versionNumbers = version.Split('.')
                                                .Select(o => int.Parse(o))
                                                .ToList();

                    var info = new VersionInfo()
                    {
                        Major = versionNumbers[0],
                        Minor = versionNumbers[1],
                        Build = versionNumbers[2],
                    };
                    return info;
                }
            }
            return default(VersionInfo);
        }

        /// <summary>
        /// 実行環境のOSがWindows10か否かを判定
        /// </summary>
        /// <returns></returns>
        internal static bool IsWin10()
        {
            return Version.Value.Major == 10;
        }


        internal static bool IsWin7()
        {
            return Version.Value.Major == 6 && Version.Value.Minor == 1;
        }

        internal static bool IsWin8x()
        {
            return Version.Value.Major == 6 && (Version.Value.Minor == 2 || Version.Value.Minor == 3);
        }
    }
}
