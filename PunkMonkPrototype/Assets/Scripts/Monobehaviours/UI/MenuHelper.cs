using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuHelper : MonoBehaviour
{
    [SerializeField] private ConfirmationPopup confirmationPopup = null;

    [SerializeField] private float cinematicBarDistance = 300;
    //[SerializeField] private float cinematicBarSpeed = 1;

    private bool cinematic = false;

    private bool startBlackBars = false;

    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform botBar;

    private float blackBarTimer = 0;

    private float blackBarTime = 0;

    private GameSettings currentSettings = null;
    private GameSettings defaultSettings = null;


    private void Start()
    {
        if (SettingsLoader.Instance != null)
        {
            currentSettings = SettingsLoader.Instance.CurrentSettings;
            defaultSettings = SettingsLoader.Instance.DefaultSettings;
        }

    }

    void Update()
    {
        if (startBlackBars)
        {
            if (cinematic)
            {
                blackBarTimer += Time.deltaTime;
                float currentPercentage = blackBarTimer / blackBarTime;
                if (topBar.sizeDelta.y < cinematicBarDistance)
                {
                    topBar.sizeDelta = Vector2.Lerp(new Vector2(0, 0), new Vector2(0, cinematicBarDistance), currentPercentage);
                    botBar.sizeDelta = Vector2.Lerp(new Vector2(0, 0), new Vector2(0, cinematicBarDistance), currentPercentage);
                }
            }
            else

            {
                if (topBar == null)
                {
                    return;
                }
                if (topBar.sizeDelta.y >= 0)
                {
                    blackBarTimer -= Time.deltaTime;
                    float currentPercentage = blackBarTimer / blackBarTime;

                    topBar.sizeDelta = Vector2.Lerp(new Vector2(0, 0), new Vector2(0, cinematicBarDistance), currentPercentage);
                    botBar.sizeDelta = Vector2.Lerp(new Vector2(0, 0), new Vector2(0, cinematicBarDistance), currentPercentage);
                }
                else
                {
                    startBlackBars = false;
                }

            }
        }
    }

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

    public void OpenConfirmationPopup(string a_message, System.Action a_action)
    {
        if (confirmationPopup)
        {
            confirmationPopup.gameObject.SetActive(true);
            confirmationPopup.Setup(a_message, a_action);
        }
        else
            Debug.LogError("No Confirmation Popup attached.", gameObject);
    }

    public void PlayBlackBars(float a_time)
    {
        if (!topBar || !botBar)
        {
            botBar = transform.GetChild(5).transform.GetChild(1).GetComponent<RectTransform>();
            topBar = transform.GetChild(5).transform.GetChild(0).GetComponent<RectTransform>();
        }
        blackBarTimer = 0;
        cinematic = true;
        blackBarTime = a_time;
        startBlackBars = true;
    }

    public void StopBlackBars(bool instante = false)
    {
        if (instante)
        {
            blackBarTimer = 0;
        }

        cinematic = false;
    }

    public void StopIntroCutscene(float a_speed = 1)
    {
        StartCoroutine(StopCutscene(a_speed));
    }

    public void PlayCinematicBars(float a_speed, float a_distance = 0)
    {
        StartCoroutine(StartCinematicBars(a_speed, a_distance));
    }

    private IEnumerator StopCutscene(float a_speed = 1)
    {
        while (topBar.sizeDelta.y >= 0)
        {
            topBar.sizeDelta -= new Vector2(0, Time.deltaTime * 100 * a_speed);
            botBar.sizeDelta -= new Vector2(0, Time.deltaTime * 100 * a_speed);
            yield return null;
        }
    }

    private IEnumerator StartCinematicBars(float a_speed, float a_distance = 0, bool stop = false)
    {
        if(a_distance == 0)
        {
            a_distance = cinematicBarDistance;
        }

        while (topBar.sizeDelta.y <= a_distance)
        {
            topBar.sizeDelta += new Vector2(0, Time.deltaTime * 100 * a_speed);
            botBar.sizeDelta += new Vector2(0, Time.deltaTime * 100 * a_speed);
            yield return null;
        }
        if(stop == true)
        {
            StopIntroCutscene(a_speed);
        }
    }


    public void SaveSettings()
    {
        currentSettings.Save();
    }

    private void SetSettingsToDefault()
    {
        currentSettings.SetAllTo(defaultSettings);
    }
}
