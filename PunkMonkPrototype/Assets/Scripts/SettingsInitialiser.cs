using UnityEngine;
using UnityEngine.UI;

public class SettingsInitialiser : MonoBehaviour
{

    #region Inspector Fields

    [HideInInspector] private Settings defaultSettings = null;
    [HideInInspector] private Settings settings = null;

    [Header("Gameplay")]
    [SerializeField]
    private Toggle inverseCameraRotationToggle = null;
    [SerializeField]
    private Toggle screenEdgePanToggle = null;

    [Header("Graphics")]
    [HideInInspector]
    private Dropdown qualityValue = null;

    [Header("Sounds")]
    [SerializeField]
    private Toggle muteAll = null;
    [SerializeField]
    private Slider master = null;
    [SerializeField]
    private Slider music = null;
    [SerializeField]
    private Slider effects = null;
    [SerializeField]
    private Slider dialouge = null;

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
            if (qualityValue)
                qualityValue.value = defaultSettings.quality;
            // Sounds
            if (muteAll)
                muteAll.isOn = defaultSettings.muteAll;
            if (master)
                master.value = defaultSettings.master;
            if (music)
                music.value = defaultSettings.music;
            if (effects)
                effects.value = defaultSettings.effects;
            if (dialouge)
                dialouge.value = defaultSettings.dialouge;

            hasInitialised = true;
        }
    }

    #endregion
}
