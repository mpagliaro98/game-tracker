using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public struct Color
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

        public int ToArgb()
        {
            byte[] bytes = new byte[4];
            bytes[0] = A;
            bytes[1] = R;
            bytes[2] = G;
            bytes[3] = B;
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static Color FromArgb(byte A, byte R, byte G, byte B)
        {
            var color = new Color();
            color.A = A;
            color.R = R;
            color.G = G;
            color.B = B;
            return color;
        }

        public static Color FromArgb(int argb)
        {
            byte[] bytes = BitConverter.GetBytes(argb);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            var color = new Color();
            color.A = bytes[0];
            color.R = bytes[1];
            color.G = bytes[2];
            color.B = bytes[3];
            return color;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Color)) return false;
            Color color2 = (Color)obj;
            return A == color2.A && R == color2.R && G == color2.G && B == color2.B;
        }

        public override int GetHashCode()
        {
            return ToArgb();
        }
    }
}
