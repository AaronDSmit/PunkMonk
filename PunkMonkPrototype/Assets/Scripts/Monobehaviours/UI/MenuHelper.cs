using UnityEngine;
using UnityEngine.UI;

public class MenuHelper : MonoBehaviour
{
    [SerializeField] ConfirmationPopup confirmationPopup = null;
    [SerializeField] SettingsInitialiser settingsInitialiser = null;

    private Settings currentSettings;
    //private Settings defaultSettings;

    private void Awake()
    {
        currentSettings = Resources.Load<Settings>("Settings/current");
        //defaultSettings = Resources.Load<Settings>("Settings/default");
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

    public void SetButtonInteractableTrue(Button a_button)
    {
        a_button.interactable = true;
    }

    public void SetButtonInteractableFalse(Button a_button)
    {
        a_button.interactable = false;
    }

    public void SetChildrenButtonsInteractableTrue(GameObject a_go)
    {
        Button[] buttons = a_go.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    public void SetChildrenButtonsInteractableFalse(GameObject a_go)
    {
        Button[] buttons = a_go.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    public void Quit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetGameObjectActive(GameObject a_go)
    {
        a_go.SetActive(true);
    }

    public void SetGameObjectInactive(GameObject a_go)
    {
        a_go.SetActive(false);
    }

    public void SetChildrenActive(GameObject a_go)
    {
        for (int i = 0; i < a_go.transform.childCount; i++)
        {
            a_go.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SetChildrenInactive(GameObject a_go)
    {
        for (int i = 0; i < a_go.transform.childCount; i++)
        {
            a_go.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ResetAllSettingsPopup(string a_message)
    {
        OpenConfirmationPopup(a_message, SetSettingsToDefault);
    }

    private void OpenConfirmationPopup(string a_message, System.Action a_action)
    {
        if (confirmationPopup)
        {
            confirmationPopup.gameObject.SetActive(true);
            confirmationPopup.Setup(a_message, a_action);
        }
        else
            Debug.LogError("No Confirmation Popup attached.", gameObject);
    }
}
