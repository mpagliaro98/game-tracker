using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Util
{
    public struct VersionNumber : IComparable
    {
        private byte majorVersion;
        private byte minorVersion;
        private byte buildVersion;
        private byte revisionVersion;

        public VersionNumber(byte majorVersion, byte minorVersion, byte buildVersion, byte revisionVersion)
        {
            this.majorVersion = majorVersion;
            this.minorVersion = minorVersion;
            this.buildVersion = buildVersion;
            this.revisionVersion = revisionVersion;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            VersionNumber other = (VersionNumber)obj;
            if (majorVersion.CompareTo(other.majorVersion) == 0)
            {
                if (minorVersion.CompareTo(other.minorVersion) == 0)
                {
                    if (buildVersion.CompareTo(other.buildVersion) == 0)
                    {
                        return revisionVersion.CompareTo(other.revisionVersion);
                    }
                    else
                    {
                        return buildVersion.CompareTo(other.buildVersion);
                    }
                }
                else
                {
                    return minorVersion.CompareTo(other.minorVersion);
                }
            }
            else
            {
                return majorVersion.CompareTo(other.majorVersion);
            }
        }

        public static bool operator ==(VersionNumber obj1, VersionNumber obj2)
        {
            return obj1.CompareTo(obj2) == 0;
        }

        public static bool operator !=(VersionNumber obj1, VersionNumber obj2)
        {
            return !(obj1 == obj2);
        }

        public static bool operator <(VersionNumber obj1, VersionNumber obj2)
        {
            return obj1.CompareTo(obj2) < 0;
        }

        public static bool operator <=(VersionNumber obj1, VersionNumber obj2)
        {
            return obj1 < obj2 || obj1 == obj2;
        }

        public static bool operator >(VersionNumber obj1, VersionNumber obj2)
        {
            return !(obj1 <= obj2);
        }

        public static bool operator >=(VersionNumber obj1, VersionNumber obj2)
        {
            return !(obj1 < obj2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is VersionNumber)) return false;
            VersionNumber other = (VersionNumber)obj;
            return majorVersion.Equals(other.majorVersion) && minorVersion.Equals(other.minorVersion) &&
                buildVersion.Equals(other.buildVersion) && revisionVersion.Equals(other.revisionVersion);
        }

        public override int GetHashCode()
        {
            byte[] bytes = {majorVersion, minorVersion, buildVersion, revisionVersion};
            return BitConverter.ToInt32(bytes, 0);
        }

        public override string ToString()
        {
            return "v. " + majorVersion.ToString() + "." + minorVersion.ToString() + "." + buildVersion.ToString("00") + "." + revisionVersion.ToString("00");
        }
    }
}
