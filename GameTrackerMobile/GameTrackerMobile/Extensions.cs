using System;
using System.Collections.Generic;
using System.Text;

namespace GameTrackerMobile
{
    public static class Extensions
    {
        public static RatableTracker.Framework.Color ToFrameworkColor(this Color? color)
        {
            var colorInput = color ?? new Color();
            return RatableTracker.Framework.Color.FromArgb(Convert.ToByte(colorInput.A * 255), Convert.ToByte(colorInput.R * 255),
                Convert.ToByte(colorInput.G * 255), Convert.ToByte(colorInput.B * 255));
        }

        public static RatableTracker.Framework.Color ToFrameworkColor(this Color color)
        {
            return RatableTracker.Framework.Color.FromArgb(Convert.ToByte(color.A * 255), Convert.ToByte(color.R * 255),
                Convert.ToByte(color.G * 255), Convert.ToByte(color.B * 255));
        }

        public static Color ToXamarinColor(this RatableTracker.Framework.Color? color)
        {
            var colorInput = color ?? new RatableTracker.Framework.Color();
            return Xamarin.Forms.Color.FromRgba(colorInput.R, colorInput.G, colorInput.B, colorInput.A);
        }

        public static Color ToXamarinColor(this RatableTracker.Framework.Color color)
        {
            return Xamarin.Forms.Color.FromRgba(color.R, color.G, color.B, color.A);
        }
    }
}
