using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleApplicationWPF
{
    public static class InfoExtensions
    {
        public static string SetAttributes(this System.IO.FileSystemInfo __file)
        {
            string attributes = "";

            if (__file.Attributes.HasFlag(FileAttributes.ReadOnly))
                attributes += "r";
            else
                attributes += "-";

            if (__file.Attributes.HasFlag(FileAttributes.Archive))
                attributes += "a";
            else
                attributes += "-";

            if (__file.Attributes.HasFlag(FileAttributes.Hidden))
                attributes += "h";
            else
                attributes += "-";

            if (__file.Attributes.HasFlag(FileAttributes.System))
                attributes += "s";
            else
                attributes += "-";

            return attributes;
        }

        // Finding file attribute
        public static bool FindAttribute(this System.IO.FileSystemInfo __file, FileAttributes __attribute)
        {
            // ?????????????????
            return (__file.Attributes == __attribute);
        }

        // Deleting attribute.
        public static void DeleteAttribute(this System.IO.FileSystemInfo __file, FileAttributes __attribute)
        {
            __file.Attributes ^= __attribute;
        }

    }
}
