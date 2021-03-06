using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GameTrackerWPF
{
    public static partial class Extensions
    {
        public static T FindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (!(child is T t))
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        foundChild = t;
                        break;
                    }
                }
                else
                {
                    foundChild = t;
                    break;
                }
            }

            return foundChild;
        }

        public static RatableTracker.Framework.Color ToDrawingColor(this Color? color)
        {
            var colorInput = color ?? new Color();
            return RatableTracker.Framework.Color.FromArgb(colorInput.A, colorInput.R, colorInput.G, colorInput.B);
        }

        public static RatableTracker.Framework.Color ToDrawingColor(this Color color)
        {
            return RatableTracker.Framework.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color ToMediaColor(this RatableTracker.Framework.Color? color)
        {
            var colorInput = color ?? new RatableTracker.Framework.Color();
            return Color.FromArgb(colorInput.A, colorInput.R, colorInput.G, colorInput.B);
        }

        public static Color ToMediaColor(this RatableTracker.Framework.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
