using UnityEngine;
using UnityEngine.UI;

public class SettingsInitialiser : MonoBehaviour
{

    #region Inspector Fields

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

    #region References

    private Settings defaultSettings = null;
    private Settings currentSettings = null;

    #endregion

    #region Private Fields

    private bool hasInitialised = false;

    #endregion

    #region Unity life-cycle methods

    private void Awake()
    {
        defaultSettings = Resources.Load<Settings>("Settings/default");
        currentSettings = Resources.Load<Settings>("Settings/current");
    }

    private void OnEnable()
    {
        if (hasInitialised == false)
        {
            InitializeToDefault();

            hasInitialised = true;
        }
    }

    public void InitializeToDefault()
    {
        Initialize(defaultSettings);
    }

    public void Initialize(Settings a_settings)
    {
        // Gameplay
        if (inverseCameraRotationToggle)
            inverseCameraRotationToggle.isOn = a_settings.InverseCameraRotation;
        if (screenEdgePanToggle)
            screenEdgePanToggle.isOn = a_settings.ScreenEdgePan;
        // Graphics
        if (qualityDropdown)
            qualityDropdown.value = a_settings.Quality;
        // Sounds
        if (muteAllToggle)
            muteAllToggle.isOn = a_settings.MuteAll;
        if (masterSlider)
            masterSlider.value = a_settings.Master;
        if (musicSlider)
            musicSlider.value = a_settings.Music;
        if (effectsSlider)
            effectsSlider.value = a_settings.Effects;
        if (dialougeSlider)
            dialougeSlider.value = a_settings.Dialouge;

        // Set the current settings to the default
        currentSettings.SetAllTo(a_settings);
    }

    #endregion
}
