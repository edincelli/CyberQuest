using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsInfo
{
    public int settingsFileVersion = 0;

    public int language = 0;

    [Range(0.5f, 1.5f)]
    public float rotationSpeed = 1;
    [Range(0.5f, 1.5f)]
    public float zoomSpeed = 1;

    public int fullScreen = 0;
    public int width = 1920;
    public int height = 1080;
    public int refreshRate = 60;
    public bool vSync = true;

    public SettingsQualityInfo qualitySettings;

    public float volumeGeneral = 1;
    public float volumeMusic = 1;
    public float volumeEffects = 1;
    public float volumeUI = 1;

    public Resolution GetResolution()
    {
        Resolution resolution = new Resolution();
        resolution.width = width;
        resolution.height = height;
        resolution.refreshRate = refreshRate;

        return resolution;
    }

    public void SetResoultion(Resolution resolution)
    {
        width = resolution.width;
        height = resolution.height;
        resolution.refreshRate = refreshRate;
    }

    public void SetupSettingsInfo()
    {
        settingsFileVersion = SettingsManager.SettingsFileVersion;
        fullScreen = (int)Screen.fullScreenMode;
        width = Screen.currentResolution.width;
        height = Screen.currentResolution.height;
        refreshRate = Screen.currentResolution.refreshRate;
    }

    public string GetLanguageString()
    {
        //in future it will be taken from language system
        string[] languageStrings = new string[] { "English" };

        if (languageStrings.Length <= language)
            return languageStrings[languageStrings.Length - 1];

        return languageStrings[language];
    }
    public string GetFullScreenString()
    {
        if (SettingsManager.FullScreenStrings.Count <= fullScreen)
            return SettingsManager.FullScreenStrings[SettingsManager.FullScreenStrings.Count - 1];

        return SettingsManager.FullScreenStrings[fullScreen];
    }

    public int GetResolutionIndex()
    {
        int tempResolutionIndex = SettingsManager.ResolutionsStrings.IndexOf(GetResolutionString());
        tempResolutionIndex.ClampInt(0, SettingsManager.ResolutionsStrings.Count - 1);
        return tempResolutionIndex;
    }

    public int GetRefreshRateIndex()
    {
        int tempRefreshIndex = SettingsManager.RefreshRates.IndexOf(refreshRate);
        tempRefreshIndex.ClampInt(0, SettingsManager.RefreshRates.Count - 1);
        return tempRefreshIndex;
    }

    public string GetResolutionString()
    {
        return GetResolutionString(width, height);
    }

    public void SetResoultion(int resolutionIndex)
    {
        resolutionIndex.ClampInt(0, SettingsManager.Resolutions.Count - 1);
        width = SettingsManager.Resolutions[resolutionIndex].width;
        height = SettingsManager.Resolutions[resolutionIndex].height;
    }

    public void SetRefreshRate(int refreshRateIndex)
    {
        refreshRateIndex.ClampInt(0, SettingsManager.RefreshRates.Count - 1);
        refreshRate = SettingsManager.RefreshRates[refreshRateIndex];
    }

    public static string GetResolutionString(int width, int height)
    {
        return $"{width}x{height}";
    }
}