using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BuildScripts
{
    public class CreateContentFolder : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platformGroup == BuildTargetGroup.WebGL)
            {
                Debug.LogWarning("Skipping content folder creation for WebGL builds.");
                return;
            }

            if (report.summary.result == BuildResult.Cancelled)
                return;

            if (report.summary.result == BuildResult.Failed)
                return;

            CopyContentFolder(report.summary.outputPath, report.summary.platform, BuildProfilesMenu.KeepRepositoryFiles);
        }

        public static void CopyContentFolder(string outputPath, BuildTarget platform, bool keepRepositoryFiles)
        {
            string productName = Path.GetFileName(outputPath);
            string buildPath = Path.GetDirectoryName(outputPath);
            string contentFolder = ContentConstValues.FOLDER_CONTENT;
            string buildContentFolderPath;

            string projectPath = Application.dataPath;
            string projectContentPath = Path.Combine(projectPath, contentFolder);

            if (platform == BuildTarget.StandaloneOSX)
            {
                string macAppFolder = productName;

                if (macAppFolder.EndsWith(".app") == false)
                    macAppFolder += ".app";

                string macContentsFolder = "Contents";
                buildContentFolderPath = Path.Combine(buildPath, macAppFolder, macContentsFolder, contentFolder);
                //Debug.Log($"buildContentFolderPath: {buildContentFolderPath}\n" +
                //    $"buildPath: {buildPath}\n" +
                //    $"macAppFolder: {macAppFolder}\n" +
                //    $"macContentsFolder: {macContentsFolder}\n" +
                //    $"contentFolder: {contentFolder}");
            }
            else
            {
                string windowsDataFolder = productName.Replace(".exe", "") + "_Data";
                buildContentFolderPath = Path.Combine(buildPath, windowsDataFolder, contentFolder);
            }

            if (Directory.Exists(buildContentFolderPath))
            {
                try
                {
                    Directory.Delete(buildContentFolderPath, true);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Cannot delete directory: {buildContentFolderPath}\n{ex.Message}");
                    UtilityMethods.ForceDeleteDirectory(buildContentFolderPath);
                }
            }

            Directory.CreateDirectory(buildContentFolderPath);

            UtilityMethods.CopyDirecoryRecirsively(projectContentPath, buildContentFolderPath);

            if (keepRepositoryFiles == false)
            {
                DeleteGitDirectory(buildContentFolderPath);
                DeleteFilesByExtensions(buildContentFolderPath, new[] { ".meta", ".gitignore", ".gitattributes" });
                DeleteEmptyDirectories(buildContentFolderPath);
            }
        }

        private static void DeleteGitDirectory(string rootFolderPath)
        {
            if (string.IsNullOrWhiteSpace(rootFolderPath))
                return;

            if (Directory.Exists(rootFolderPath) == false)
                return;

            string gitDirPath = Path.Combine(rootFolderPath, ".git");
            if (Directory.Exists(gitDirPath) == false)
                return;

            try
            {
                Directory.Delete(gitDirPath, true);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Cannot delete directory: {gitDirPath}\n{ex.Message}");

                try
                {
                    UtilityMethods.ForceDeleteDirectory(gitDirPath);
                }
                catch (Exception forceEx)
                {
                    Debug.LogError($"Cannot force delete directory: {gitDirPath}\n{forceEx.Message}");
                }
            }
        }

        private static void DeleteFilesByExtensions(string rootFolderPath, IEnumerable<string> extensions)
        {
            if (string.IsNullOrWhiteSpace(rootFolderPath))
                return;

            if (Directory.Exists(rootFolderPath) == false)
                return;

            HashSet<string> extSet = new HashSet<string>(
                extensions
                    .Where(e => string.IsNullOrWhiteSpace(e) == false)
                    .Select(e => e.StartsWith(".") ? e : "." + e),
                StringComparer.OrdinalIgnoreCase);

            if (extSet.Count == 0)
                return;

            DirectoryInfo rootDI = new DirectoryInfo(rootFolderPath);

            FileInfo[] files;
            try
            {
                files = rootDI.GetFiles("*", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Cannot enumerate files in: {rootFolderPath}\n{ex.Message}");
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i];

                if (extSet.Contains(file.Extension) == false)
                    continue;

                try
                {
                    file.Attributes = FileAttributes.Normal;
                    File.Delete(file.FullName);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Cannot delete file: {file.FullName}\n{ex.Message}");
                }
            }
        }

        private static void DeleteEmptyDirectories(string rootFolderPath)
        {
            if (string.IsNullOrWhiteSpace(rootFolderPath))
                return;

            if (Directory.Exists(rootFolderPath) == false)
                return;

            string[] allDirs;
            try
            {
                allDirs = Directory.GetDirectories(rootFolderPath, "*", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Cannot enumerate directories in: {rootFolderPath}\n{ex.Message}");
                return;
            }

            // Deleting bottom-up to ensure children are removed before parents.
            Array.Sort(allDirs, (a, b) => b.Length.CompareTo(a.Length));

            for (int i = 0; i < allDirs.Length; i++)
            {
                string dir = allDirs[i];

                try
                {
                    bool hasAnyFiles = Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Any();
                    if (hasAnyFiles)
                        continue;

                    bool hasAnyDirs = Directory.EnumerateDirectories(dir, "*", SearchOption.TopDirectoryOnly).Any();
                    if (hasAnyDirs)
                        continue;

                    Directory.Delete(dir, false);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Cannot delete directory: {dir}\n{ex.Message}");
                }
            }
        }
    }
}
