using System;
using System.IO;

namespace BuildScripts
{
    internal static class BuildOutputPath
    {
        public static bool TryResolve(string projectRoot, string relativePath, out string fullPath)
        {
            fullPath = null;
            if (string.IsNullOrWhiteSpace(relativePath))
                return false;

            string normalized = relativePath.Replace('\\', '/');
            if (Path.IsPathRooted(normalized) || normalized.Contains(":"))
                return false;

            try
            {
                string root = Path.GetFullPath(projectRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                string candidate = Path.GetFullPath(Path.Combine(root, normalized));
                candidate = candidate.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                StringComparison comparison = Path.DirectorySeparatorChar == '\\'
                    ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (!candidate.StartsWith(root, comparison))
                    return false;

                fullPath = candidate;
                return true;
            }
            catch (ArgumentException) { return false; }
            catch (NotSupportedException) { return false; }
            catch (IOException) { return false; }
        }
    }
}
