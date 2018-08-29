using UnityEngine;
using UnityEngine.UI;

public class SettingsInitialiser : MonoBehaviour
{

    #region Inspector Fields

    [SerializeField] private Settings defaultSettings = null;
    [SerializeField] private Settings currentSettings = null;

    [Header("Gameplay")]
    [SerializeField]
    private Toggle inverseCameraRotationToggle = null;
    [SerializeField]
    private Toggle screenEdgePanToggle = null;

    [Header("Graphics")]
    [SerializeField]
    private Dropdown qualityDropdown = null;

    [Header("Sounds")]
    [SerializeField]
    private Toggle muteAllToggle = null;
    [SerializeField]
    private Slider masterSlider = null;
    [SerializeField]
    private Slider musicSlider = null;
    [SerializeField]
    private Slider effectsSlider = null;
    [SerializeField]
    private Slider dialougeSlider = null;

    #endregion

    #region Private Fields

    private bool hasInitialised = false;

    #endregion

    #region Unity life-cycle methods

    private void OnEnable()
    {
        if (hasInitialised == false)
        {
            // Gameplay
            if (inverseCameraRotationToggle)
                inverseCameraRotationToggle.isOn = defaultSettings.inverseCameraRotation;
            if (screenEdgePanToggle)
                screenEdgePanToggle.isOn = defaultSettings.screenEdgePan;
            // Graphics
            if (qualityDropdown)
                qualityDropdown.value = defaultSettings.quality;
            // Sounds
            if (muteAllToggle)
                muteAllToggle.isOn = defaultSettings.muteAll;
            if (masterSlider)
                masterSlider.value = defaultSettings.master;
            if (musicSlider)
                musicSlider.value = defaultSettings.music;
            if (effectsSlider)
                effectsSlider.value = defaultSettings.effects;
            if (dialougeSlider)
                dialougeSlider.value = defaultSettings.dialouge;

            // Set the current settings to the default
            currentSettings.SetAllTo(defaultSettings);

            hasInitialised = true;
        }
    }

    #endregion
}
