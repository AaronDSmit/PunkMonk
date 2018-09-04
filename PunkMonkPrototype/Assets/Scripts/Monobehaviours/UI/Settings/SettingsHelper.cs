﻿using UnityEngine;
using UnityEngine.UI;

public class SettingsHelper : MonoBehaviour
{
    [SerializeField]
    SettingsInitialiser settingsInitialiser = null;

    private Settings currentSettings;

    private void Awake()
    {
        currentSettings = Resources.Load<Settings>("Settings/current");
    }

    #region Settings

    public void SetSettingsToDefault()
    {
        settingsInitialiser.InitializeToDefault();
    }

    #region Gameplay

    public void SetInverseCameraRotationToggle(Toggle a_toggle)
    {
        currentSettings.InverseCameraRotation = a_toggle.isOn;
    }

    public void SetScreenEdgePanToggle(Toggle a_toggle)
    {
        currentSettings.ScreenEdgePan = a_toggle.isOn;
    }

    #endregion

    #region Quality

    public void SetQuality(int a_newQualityLevel)
    {
        currentSettings.Quality = a_newQualityLevel;
    }

    public void SetQualityDropdown(Dropdown a_dropDown)
    {
        currentSettings.Quality = a_dropDown.value;
    }

    #endregion

    #region Sound

    public void SetMuteAllToggle(Toggle a_toggle)
    {
        currentSettings.MuteAll = a_toggle.isOn;
    }

    public void SetMasterVolumeSlider(Slider a_slider)
    {
        currentSettings.Master = a_slider.value;
    }

    public void SetMusicVolumeSlider(Slider a_slider)
    {
        currentSettings.Music = a_slider.value;
    }

    public void SetEffectsVolumeSlider(Slider a_slider)
    {
        currentSettings.Effects = a_slider.value;
    }

    public void SetDialougeVolumeSlider(Slider a_slider)
    {
        currentSettings.Dialouge = a_slider.value;
    }

    #endregion

    #endregion

}