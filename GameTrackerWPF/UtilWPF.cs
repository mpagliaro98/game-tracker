using RatableTracker.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GameTrackerWPF
{
    public static class UtilWPF
    {
        public const string SCORE_FORMAT = "0.##";

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

        public static RatableTracker.Util.Color ToDrawingColor(this Color? color)
        {
            var colorInput = color ?? new Color();
            return RatableTracker.Util.Color.FromArgb(colorInput.A, colorInput.R, colorInput.G, colorInput.B);
        }

        public static RatableTracker.Util.Color ToDrawingColor(this Color color)
        {
            return RatableTracker.Util.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color ToMediaColor(this RatableTracker.Util.Color? color)
        {
            var colorInput = color ?? new RatableTracker.Util.Color();
            return Color.FromArgb(colorInput.A, colorInput.R, colorInput.G, colorInput.B);
        }

        public static Color ToMediaColor(this RatableTracker.Util.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static void DisplayUIExceptionMessage(this Exception e)
        {
            if (e is ValidationException val)
                MessageBox.Show(val.Message, "Invalid Fields");
            else
                MessageBox.Show("Unexpected error - " + e.GetType().Name + ": " + e.Message + "\n\nSee the logs for more information.", "Unexpected Error");
        }

        public static void GoToURL(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        public static Version GetVersionNumber()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
