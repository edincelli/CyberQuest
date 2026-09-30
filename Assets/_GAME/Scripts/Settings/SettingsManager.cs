using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class SettingsManager : GameSystemComponent
{
    public static SettingsManager Instance;
    public static int SettingsFileVersion { get; } = 0;

    [SerializeField] private List<SettingsQualityPreset> qualityPresets = new List<SettingsQualityPreset>();

    private List<string> resolutionsStrings = new List<string>();
    private List<Resolution> resolutions = new List<Resolution>();
    private List<int> refreshRates = new List<int>();

    public static List<SettingsQualityPreset> QualityPresets => Instance.qualityPresets;
    public static List<string> ResolutionsStrings => Instance.resolutionsStrings;
    public static List<Resolution> Resolutions => Instance.resolutions;
    public static List<int> RefreshRates => Instance.refreshRates;
    public static List<string> RefreshRateStrings => Instance.refreshRates.Select(x => x.ToString()).ToList();
    public static List<string> FullScreenStrings => new List<string>() { "Full Screen", "Borderless Full Screen", "Maximized Window", "Windowed" };

    public static List<string> QualityStringsAll => new List<string>() { "ultra", "high", "medium", "low" };
    public static List<string> QualityStringsUltraLow => new List<string>() { "ultra", "low" };

    public static SettingsInfo SettingsInfo;

    private static string SettingsFilePath => Application.persistentDataPath + "/settings.json";

    public void SaveSettings(SettingsInfo newSettingsInfo)
    {
        newSettingsInfo.settingsFileVersion = SettingsFileVersion;
        string jsonContent = JsonUtility.ToJson(newSettingsInfo, true);
        File.WriteAllText(SettingsFilePath, jsonContent);
        SettingsInfo = newSettingsInfo;
        ApplyGameSettings();
        ApplyLevelSettings();
    }

    private void Awake()
    {
        Instance = this;

        PrepareLists();

        if (SettingsInfo == null)
        {
            LoadSettingsFile();
            ApplyGameSettings();
        }

        ApplyLevelSettings();
    }

    private void Start()
    {
        //SoundsManager.UpdateVolumeSettings();
    }

    private void PrepareLists()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            Resolution tempResolution = Screen.resolutions[i];
            string tempResolutionString = $"{tempResolution.width}x{tempResolution.height}";

            if (resolutionsStrings.Contains(tempResolutionString) == false)
            {
                resolutionsStrings.Add(tempResolutionString);
                resolutions.Add(tempResolution);
            }

            if (refreshRates.Contains(tempResolution.refreshRate) == false)
            {
                refreshRates.Add(tempResolution.refreshRate);
            }
        }

        refreshRates.OrderBy(x => x).ToList();
    }

    private void LoadSettingsFile()
    {
        if (File.Exists(SettingsFilePath) == false)
        {
            CreateNewSettingsFile();
            return;
        }

        try
        {
            string jsonContent = File.ReadAllText(SettingsFilePath);
            SettingsInfo tempSettingsInfo = JsonUtility.FromJson<SettingsInfo>(jsonContent);
            CheckSettingsVersion(tempSettingsInfo);
            SettingsInfo = tempSettingsInfo.Clone();
        }
        catch
        {
            CreateNewSettingsFile();
        }
    }

    private void CheckSettingsVersion(SettingsInfo settingsInfo)
    {
        //Examples from SCP Strategy
        /*if (gameInfo.saveFileVersion < 3)
        {
            gameInfo.askReview = false;
        }

        if (gameInfo.saveFileVersion < 4)
        {
            gameInfo.AnalyticsInfo.started_games++;
        }*/
    }

    private void CreateNewSettingsFile()
    {
        SettingsInfo = new SettingsInfo();
        SettingsInfo.SetupSettingsInfo();
        SettingsInfo.qualitySettings = new SettingsQualityInfo();
        SaveSettings(SettingsInfo);
    }

    private void ApplyGameSettings()
    {
        Screen.SetResolution(SettingsInfo.width, SettingsInfo.height,
            (FullScreenMode)SettingsInfo.fullScreen, SettingsInfo.refreshRate);

        QualitySettings.vSyncCount = SettingsInfo.vSync ? 1 : 0;
        QualitySettings.globalTextureMipmapLimit = SettingsInfo.qualitySettings.texturesQuality;
        //QualitySettings.maximumLODLevel = SettingsInfo.qualitySettings.modelsQuality;

        //switch (SettingsInfo.qualitySettings.shadowsQuality)
        //{
        //    case 0:
        //        QualitySettings.shadows = ShadowQuality.All;
        //        QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
        //        QualitySettings.shadowDistance = 150;
        //        break;
        //    case 1:
        //        QualitySettings.shadows = ShadowQuality.All;
        //        QualitySettings.shadowResolution = ShadowResolution.Medium;
        //        QualitySettings.shadowDistance = 100;
        //        break;
        //    case 2:
        //        QualitySettings.shadows = ShadowQuality.HardOnly;
        //        QualitySettings.shadowResolution = ShadowResolution.Low;
        //        QualitySettings.shadowDistance = 50;
        //        break;
        //    default:
        //        QualitySettings.shadows = ShadowQuality.Disable;
        //        break;
        //}

        AudioManager.UpdateVolumeSettings();
    }

    private void ApplyLevelSettings()
    {
        PostProcessLayer ppLayer = Camera.main.GetComponent<PostProcessLayer>();

        if (ppLayer != null)
            ppLayer.enabled = SettingsInfo.qualitySettings.postProcessing;

        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;

        //Terrain[] terrains = FindObjectsOfType<Terrain>();

        //for (int i = 0; i < terrains.Length; i++)
        //{
        //    terrains[i].detailObjectDensity = SettingsInfo.qualitySettings.GetVegetationDensity();
        //    terrains[i].detailObjectDistance = SettingsInfo.qualitySettings.GetVegetationDistance();
        //}
    }
}
