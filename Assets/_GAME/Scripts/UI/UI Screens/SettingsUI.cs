using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsUI : UI_Screen
{
    [Header("Panels")]
    [SerializeField] private Transform generalParent;
    [SerializeField] private Transform visualsParent;
    [SerializeField] private Transform audioParent;

    [Header("Element Prefabs")]
    [SerializeField] private TextMeshProUGUI categoryLabelPrefab;
    [SerializeField] private SettingsUI_Element elementSliderPrefab;
    [SerializeField] private SettingsUI_Element elementSwitchPrefab;
    [SerializeField] private SettingsUI_Element elementHorizontalSelectorPrefab;

    [Header("Elements Content")]
    [SerializeField] private SettingsUI_Element.Config languageConfig;
    [SerializeField] private SettingsUI_Element.Config rotationSensConfig;
    [SerializeField] private SettingsUI_Element.Config zoomSensConfig;
    [Space]
    [SerializeField] private SettingsUI_Element.Config fullScreenConfig;
    [SerializeField] private SettingsUI_Element.Config resolutionConfig;
    [SerializeField] private SettingsUI_Element.Config refreshRateConfig;
    [SerializeField] private SettingsUI_Element.Config vsyncConfig;
    [Space]
    [SerializeField] private SettingsUI_Element.Config texturesConfig;
    [SerializeField] private SettingsUI_Element.Config mapConfig;
    //[SerializeField] private SettingsUI_Element.Config modelsConfig;
    //[SerializeField] private SettingsUI_Element.Config shadowsConfig;
    //[SerializeField] private SettingsUI_Element.Config vegetationConfig;
    [SerializeField] private SettingsUI_Element.Config postProcessingConfig;
    [Space]
    [SerializeField] private SettingsUI_Element.Config volumeGeneralConfig;
    [SerializeField] private SettingsUI_Element.Config volumeMusicConfig;
    [SerializeField] private SettingsUI_Element.Config volumeEffectsConfig;
    [SerializeField] private SettingsUI_Element.Config volumeUiConfig;


    private SettingsUI_Element languageElement;
    private SettingsUI_Element rotationSensElement;
    private SettingsUI_Element zoomSensElement;

    private SettingsUI_Element fullScreenElement;
    private SettingsUI_Element resolutionElement;
    private SettingsUI_Element refreshRateElement;
    private SettingsUI_Element vsyncElement;

    private SettingsUI_Element texturesElement;
    private SettingsUI_Element mapElement;
    //privat SettingsUI_Elements modelsElement;
    //privat SettingsUI_Elements shadowsElement;
    //privat SettingsUI_Elements vegetationElement;
    private SettingsUI_Element postProcessingElement;

    private SettingsUI_Element volumeGeneralElement;
    private SettingsUI_Element volumeMusicElement;
    private SettingsUI_Element volumeEffectsElement;
    private SettingsUI_Element volumeUiElement;

    private SettingsInfo tempSettings;

    private bool elementsSpawned = false;

    public override void ShowScreen()
    {
        base.ShowScreen();
        //TimeController.PauseGame();

        if (elementsSpawned == false)
            SpawnElements();

        tempSettings = SettingsManager.SettingsInfo.Clone();
        LoadSettingsValues();
    }

    public override void HideScreen()
    {
        base.HideScreen();
        //TimeController.UnpauseGame();
    }

    public override void Back()
    {
        base.Back();
    }

    public void ClickCancel()
    {
        Back();
    }

    public void ClickSave()
    {
        PrepareTempSettings();
        SettingsManager.Instance.SaveSettings(tempSettings);
        Back();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            LoadSettingsValues();
    }

    public void SetQualityPreset(int qualityPresetIndex)
    {
        qualityPresetIndex.ClampInt(0, SettingsManager.QualityPresets.Count - 1);
        SettingsQualityPreset preset = SettingsManager.QualityPresets[qualityPresetIndex];
        tempSettings.qualitySettings = preset.qualitySettings.Clone();
        LoadSettingsValues();
    }

    private void LoadSettingsValues()
    {
        languageElement.SetValue_HorizontalSelector(tempSettings.language);
        rotationSensElement.SetValue_Slider(tempSettings.rotationSpeed);
        zoomSensElement.SetValue_Slider(tempSettings.zoomSpeed);

        fullScreenElement.SetValue_HorizontalSelector(tempSettings.fullScreen);
        resolutionElement.SetValue_HorizontalSelector(tempSettings.GetResolutionIndex());
        refreshRateElement.SetValue_HorizontalSelector(tempSettings.GetRefreshRateIndex());
        vsyncElement.SetValue_Switch(tempSettings.vSync);

        texturesElement.SetValue_HorizontalSelector(tempSettings.qualitySettings.texturesQuality);
        mapElement.SetValue_HorizontalSelector(tempSettings.qualitySettings.mapQuality);
        postProcessingElement.SetValue_Switch(tempSettings.qualitySettings.postProcessing);

        volumeGeneralElement.SetValue_Slider(tempSettings.volumeGeneral);
        volumeMusicElement.SetValue_Slider(tempSettings.volumeMusic);
        volumeEffectsElement.SetValue_Slider(tempSettings.volumeEffects);
        volumeUiElement.SetValue_Slider(tempSettings.volumeUI);
    }

    private void PrepareTempSettings()
    {
        tempSettings.language = languageElement.GetValue_HorizontalSelector();
        tempSettings.rotationSpeed = rotationSensElement.GetValue_Slider();
        tempSettings.zoomSpeed = zoomSensElement.GetValue_Slider();

        tempSettings.fullScreen = fullScreenElement.GetValue_HorizontalSelector();
        tempSettings.SetResoultion(resolutionElement.GetValue_HorizontalSelector());
        tempSettings.SetRefreshRate(refreshRateElement.GetValue_HorizontalSelector());
        tempSettings.vSync = vsyncElement.GetValue_Switch();

        tempSettings.qualitySettings.texturesQuality = texturesElement.GetValue_HorizontalSelector();
        tempSettings.qualitySettings.mapQuality = mapElement.GetValue_HorizontalSelector();
        tempSettings.qualitySettings.postProcessing = postProcessingElement.GetValue_Switch();

        tempSettings.volumeGeneral = volumeGeneralElement.GetValue_Slider();
        tempSettings.volumeMusic = volumeMusicElement.GetValue_Slider();
        tempSettings.volumeEffects = volumeEffectsElement.GetValue_Slider();
        tempSettings.volumeUI = volumeUiElement.GetValue_Slider();
    }


    private void SpawnElements()
    {
        //General

        Instantiate(categoryLabelPrefab, generalParent).SetText("General");

        languageElement = Instantiate(elementHorizontalSelectorPrefab, generalParent);
        languageElement.SetupElement_HorizontalSelector(languageConfig, new List<string>() { "English" });

        Instantiate(categoryLabelPrefab, generalParent).SetText("Camera");

        rotationSensElement = Instantiate(elementSliderPrefab, generalParent);
        rotationSensElement.SetupElement_Slider(rotationSensConfig, 0.1f, 2f);

        zoomSensElement = Instantiate(elementSliderPrefab, generalParent);
        zoomSensElement.SetupElement_Slider(zoomSensConfig, 0.1f, 2f);

        Instantiate(categoryLabelPrefab, generalParent).SetText("Display");

        fullScreenElement = Instantiate(elementHorizontalSelectorPrefab, generalParent);
        fullScreenElement.SetupElement_HorizontalSelector(fullScreenConfig, SettingsManager.FullScreenStrings);

        resolutionElement = Instantiate(elementHorizontalSelectorPrefab, generalParent);
        resolutionElement.SetupElement_HorizontalSelector(resolutionConfig, SettingsManager.ResolutionsStrings);

        refreshRateElement = Instantiate(elementHorizontalSelectorPrefab, generalParent);
        refreshRateElement.SetupElement_HorizontalSelector(refreshRateConfig, SettingsManager.RefreshRateStrings);

        vsyncElement = Instantiate(elementSwitchPrefab, generalParent);
        vsyncElement.SetupElement_Switch(vsyncConfig);

        //Visuals

        Instantiate(categoryLabelPrefab, visualsParent).SetText("Visuals");

        texturesElement = Instantiate(elementHorizontalSelectorPrefab, visualsParent);
        texturesElement.SetupElement_HorizontalSelector(texturesConfig, SettingsManager.QualityStringsAll);

        mapElement = Instantiate(elementHorizontalSelectorPrefab, visualsParent);
        mapElement.SetupElement_HorizontalSelector(mapConfig, SettingsManager.QualityStringsUltraLow);

        postProcessingElement = Instantiate(elementSwitchPrefab, visualsParent);
        postProcessingElement.SetupElement_Switch(postProcessingConfig);

        //Audio
        Instantiate(categoryLabelPrefab, audioParent).SetText("Audio");

        volumeGeneralElement = Instantiate(elementSliderPrefab, audioParent);
        volumeGeneralElement.SetupElement_Slider(volumeGeneralConfig, 0, 1);

        volumeMusicElement = Instantiate(elementSliderPrefab, audioParent);
        volumeMusicElement.SetupElement_Slider(volumeMusicConfig, 0, 1);

        volumeEffectsElement = Instantiate(elementSliderPrefab, audioParent);
        volumeEffectsElement.SetupElement_Slider(volumeEffectsConfig, 0, 1);

        volumeUiElement = Instantiate(elementSliderPrefab, audioParent);
        volumeUiElement.SetupElement_Slider(volumeUiConfig, 0, 1);

        elementsSpawned = true;
    }
}
