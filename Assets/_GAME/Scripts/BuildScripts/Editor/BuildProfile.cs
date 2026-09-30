using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using System.IO;

namespace BuildScripts
{
    [CreateAssetMenu(fileName = "BuildProfile", menuName = "Build Profile", order = 1)]
    public class BuildProfile : ScriptableObject
    {
        [EnumToggleButtons] public BuildProfileType buildProfileType;
        public string gameName;
        [Tooltip("Output folder relative to the Unity project root, for example Builds/CQ_Game_Win.")]
        public string outputFolder;
        public BuildTarget targetPlatform;
        public BuildOptions buildOptions;
        public bool keepRepositoryFiles = false;
        public List<SceneAsset> scenes;

        public string BuildName 
        { 
            get => 
                $"{buildProfileType.ToString()}" +
                $" - " +
                $"{targetPlatform.ToString().Replace("Standalone", "")}"; 
        }

        [Button("Select Output Folder"), PropertySpace(10)]
        private void SelectOutputFolder()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string initialFolder;
            if (!BuildOutputPath.TryResolve(projectRoot, outputFolder, out initialFolder) || !Directory.Exists(initialFolder))
                initialFolder = projectRoot;
            string path = EditorUtility.OpenFolderPanel("Select Output Folder", initialFolder, "");

            if (!string.IsNullOrEmpty(path))
            {
                string relativePath = Path.GetRelativePath(projectRoot, path).Replace('\\', '/');
                if (!BuildOutputPath.TryResolve(projectRoot, relativePath, out _))
                {
                    EditorUtility.DisplayDialog("Invalid output folder", "Select a folder inside the Unity project.", "OK");
                    return;
                }
                Undo.RecordObject(this, "Change Build Output Folder");
                outputFolder = relativePath;
                EditorUtility.SetDirty(this);
            }
        }

        [Button("Open Output Folder in Explorer"), PropertySpace(10)]
        private void OpenOutputFolderInExplorer()
        {
            if (!TryGetOutputFolder(out string folderPath))
                return;

            if (!System.IO.Directory.Exists(folderPath))
            {
                EditorUtility.DisplayDialog("Error", $"Folder does not exist:\n{folderPath}", "OK");
                return;
            }

            EditorUtility.RevealInFinder(folderPath);
        }

        public bool TryGetOutputFolder(out string folderPath)
        {
            if (BuildOutputPath.TryResolve(Path.GetDirectoryName(Application.dataPath), outputFolder, out folderPath))
                return true;

            EditorUtility.DisplayDialog("Invalid output folder",
                "Set a project-relative folder inside the Unity project, for example Builds/CQ_Game_Win.", "OK");
            return false;
        }

        [Button("Copy Content Folder")]
        public void CopyContentFolder()
        {
            if (!TryGetOutputFolder(out string folderPath))
                return;
            string path;

            if (targetPlatform == BuildTarget.StandaloneWindows || targetPlatform == BuildTarget.StandaloneWindows64)
            {
                path = Path.Combine(folderPath, gameName + ".exe");
            }
            else if (targetPlatform == BuildTarget.StandaloneOSX)
            {
                path = Path.Combine(folderPath, gameName + ".app");
            }
            else
            {
                Debug.LogError($"[Build Copy CONTENT] {BuildName} - {BuildTarget.StandaloneWindows}" +
                    $"\nUnsupported platform: {targetPlatform}");
                return;
            }

            CreateContentFolder.CopyContentFolder(path, targetPlatform, keepRepositoryFiles);
        }

        [Flags]
        public enum BuildProfileType
        {
            None = 0,
            Game = 1,
            Editor = 2,
            Demo = 4
        }
    }
}
