using UnityEngine;
using UnityEngine.UI;

public class MenuHelper : MonoBehaviour
{
    [SerializeField] ConfirmationPopup confirmationPopup = null;

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
        //OpenConfirmationPopup(a_message, SetSettingsToDefault);
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
