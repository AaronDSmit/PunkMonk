using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    #region Unity Inspector Fields

    //// needs to be public for custom inspector
    //[SerializeField]
    //private List<Conversation> conversations;
    [SerializeField]
    private GameObject conversationPlane;

    #endregion

    #region Reference Fields

    //private GameObject conversationPlane;
    private TextMeshProUGUI tmp;
    private CameraController camController;
    private LightningUnit lightningUnit;
    private EarthUnit earthUnit;
    private GameObject earthProfile;
    private GameObject lightningProfile;
    private Conversation currentConversation;


    #endregion

    #region Local Fields

    public static Dialogue instance;
    private int currentConversationCount;
    private int currentSpeachCount;
    bool inCinematic;


    #endregion

    #region Properties
    public Conversation CurrentConversation
    {
        set
        {
            currentConversation = value;
        }
    }



    #endregion

    #region Public Methods


    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //conversationPlane = transform.GetChild(0).GetChild(4).gameObject;
        tmp = conversationPlane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        camController = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
        earthProfile = conversationPlane.transform.GetChild(1).GetChild(0).gameObject;
        lightningProfile = conversationPlane.transform.GetChild(1).GetChild(1).gameObject;


    }

    private void Update()
    {
        if (inCinematic)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentSpeachCount >= currentConversation.dialogue.Count)
                {
                    EndConversation();
                    return;
                }
                tmp.text = currentConversation.dialogue[currentSpeachCount].text;
                if(currentConversation.dialogue[currentSpeachCount].speaker == Speach.Speaker.Clade)
                {
                    camController.LookAtPosition(earthUnit.transform.position);
                    lightningProfile.SetActive(false);
                    earthProfile.SetActive(true);
                }
                else if(currentConversation.dialogue[currentSpeachCount].speaker == Speach.Speaker.Gen)
                {
                    camController.LookAtPosition(lightningUnit.transform.position);
                    lightningProfile.SetActive(true);
                    earthProfile.SetActive(false);
                }


                //camController.LookAtPosition(conversations[currentConversationCount].dialogue[currentSpeachCount].speaker == Speach.Speaker.Clade ? earthUnit.transform.position : lightningUnit.transform.position);

                currentSpeachCount++;
            }
        }
    }


    #endregion

    #region Local Methods


    private void GameStateChanged(GameState _oldstate, GameState _newstate)
    {

        if (_newstate == GameState.cinematic)
        {
            StartConversation();
        }

    }

    private void EndConversation()
    {
        tmp.text = "text is fucked";
        currentConversationCount++;
        conversationPlane.SetActive(false);
        Manager.instance.StateController.ChangeGameState(GameState.overworld);
        inCinematic = false;
    }

    private void StartConversation()
    {
        if(lightningUnit == null || earthUnit == null)
        {
            lightningUnit = GameObject.FindGameObjectWithTag("LightningUnit").GetComponent<LightningUnit>();
            earthUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<EarthUnit>();
        }

        inCinematic = true;
        conversationPlane.SetActive(true);
        currentSpeachCount = 0;

        if (currentConversation.dialogue[currentSpeachCount].speaker == Speach.Speaker.Clade)
        {
            camController.LookAtPosition(earthUnit.transform.position);
            lightningProfile.SetActive(false);
            earthProfile.SetActive(true);
        }
        else if (currentConversation.dialogue[currentSpeachCount].speaker == Speach.Speaker.Gen)
        {
            camController.LookAtPosition(lightningUnit.transform.position);
            lightningProfile.SetActive(true);
            earthProfile.SetActive(false);
        }

        tmp.text = currentConversation.dialogue[currentSpeachCount].text;
        currentSpeachCount++;
    }

    #endregion
}