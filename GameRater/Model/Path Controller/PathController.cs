using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    static class PathController
    {
        public static string BaseDirectory()
        {
            IPathController pc = new PathControllerWPF();
            return pc.BaseDirectory();
        }

        public static string Combine(string path1, string path2)
        {
            return System.IO.Path.Combine(path1, path2);
        }
    }
}
