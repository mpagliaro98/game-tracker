using System;
using System.Collections.Generic;
using System.Text;

namespace GameTrackerMobile
{
    public static class Extensions
    {
        public static RatableTracker.Framework.Color ToFrameworkColor(this Xamarin.Forms.Color? color)
        {
            var colorInput = color ?? new Xamarin.Forms.Color();
            return RatableTracker.Framework.Color.FromArgb(Convert.ToByte(colorInput.A), Convert.ToByte(colorInput.R),
                Convert.ToByte(colorInput.G), Convert.ToByte(colorInput.B));
        }

        public static RatableTracker.Framework.Color ToFrameworkColor(this Xamarin.Forms.Color color)
        {
            return RatableTracker.Framework.Color.FromArgb(Convert.ToByte(color.A), Convert.ToByte(color.R),
                Convert.ToByte(color.G), Convert.ToByte(color.B));
        }

        public static Xamarin.Forms.Color ToXamarinColor(this RatableTracker.Framework.Color? color)
        {
            var colorInput = color ?? new RatableTracker.Framework.Color();
            return Xamarin.Forms.Color.FromRgba(colorInput.R, colorInput.G, colorInput.B, colorInput.A);
        }

        public static Xamarin.Forms.Color ToXamarinColor(this RatableTracker.Framework.Color color)
        {
            return Xamarin.Forms.Color.FromRgba(color.R, color.G, color.B, color.A);
        }
    }
}
