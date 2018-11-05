using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmationPopup : MonoBehaviour
{
    //[SerializeField] private Button yesButton = null;
    [SerializeField] private Text text = null;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI = null;

    private System.Action yesAction;

    public void Setup(string a_message, System.Action a_yesAction)
    {
        if (text)
        {
            text.text = a_message;
        }
        else if (textMeshProUGUI)
        {
            textMeshProUGUI.text = a_message;
        }
        else
        {
            Debug.LogError("No Text or textMeshProUGUI.", gameObject);
        }
        yesAction = a_yesAction;
    }


    private void OnDisable()
    {
        Manager.instance.UIController.UnlockUI();
    }

    public void Yes()
    {
        if (yesAction != null)
        {
            yesAction();
        }
    }

}
