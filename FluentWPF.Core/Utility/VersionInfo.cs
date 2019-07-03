using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceChord.FluentWPF.Utility
{
    struct VersionInfo : IEquatable<VersionInfo>, IComparable<VersionInfo>, IComparable
    {
        public int Major;
        public int Minor;
        public int Build;

        public VersionInfo(int major, int minor, int build)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
        }

        public bool Equals(VersionInfo other)
        {
            return this.Major == other.Major && this.Minor == other.Minor && this.Build == other.Build;
        }

        public override bool Equals(object obj)
        {
            return (obj is VersionInfo other) && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Major.GetHashCode() ^ this.Minor.GetHashCode() ^ this.Build.GetHashCode();
        }

        public static bool operator ==(VersionInfo left, VersionInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VersionInfo left, VersionInfo right)
        {
            return !(left == right);
        }


        public int CompareTo(VersionInfo other)
        {
            if (this.Major != other.Major)
            {
                return this.Major.CompareTo(other.Major);
            }
            else if (this.Minor != other.Minor)
            {
                return this.Minor.CompareTo(other.Minor);
            }
            else if (this.Build != other.Build)
            {
                return this.Build.CompareTo(other.Build);
            }
            else
            {
                return 0;
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is VersionInfo other))
            {
                throw new ArgumentException();
            }

            return this.CompareTo(other);
        }

        public static bool operator <(VersionInfo left, VersionInfo right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(VersionInfo left, VersionInfo right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(VersionInfo left, VersionInfo right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(VersionInfo left, VersionInfo right)
        {
            return left.CompareTo(right) >= 0;
        }

        public override string ToString()
        {
            return $"{this.Major}.{this.Minor}.{this.Build}";
        }
    }
}
