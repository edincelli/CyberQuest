using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace BuildScripts
{
    public static class BuildProfilesMenu
    {
        public static bool KeepRepositoryFiles { get; private set; } = false;

        public static void BuildWithProfile(BuildProfile profile)
        {
            if (!profile.TryGetOutputFolder(out string dir))
                return;

            string[] scenePaths = profile.scenes.Where(s => s != null)
                .Select(s => AssetDatabase.GetAssetPath(s)).ToArray();

            if (scenePaths.Length == 0)
            {
                EditorUtility.DisplayDialog("Build error", "No scenes assigned to build profile!", "OK");
                return;
            }

            if (profile.gameName.IsNullOrEmpty() == false)
                PlayerSettings.productName = profile.gameName;
            
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            buildOptions.scenes = scenePaths;
            buildOptions.target = profile.targetPlatform;
            buildOptions.options = profile.buildOptions;

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string exeName = profile.gameName + (profile.targetPlatform == BuildTarget.StandaloneWindows || profile.targetPlatform == BuildTarget.StandaloneWindows64 ? ".exe" : "");
            buildOptions.locationPathName = Path.Combine(dir, exeName);

            KeepRepositoryFiles = profile.keepRepositoryFiles;
            BuildPipeline.BuildPlayer(buildOptions);
            Debug.Log($"Build for {profile.BuildName} completed: {buildOptions.locationPathName}");
        }
    }
}
