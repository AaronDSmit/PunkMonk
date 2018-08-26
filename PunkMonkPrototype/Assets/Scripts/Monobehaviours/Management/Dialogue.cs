using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    #region Unity Inspector Fields



    #endregion

    #region Reference Fields



    #endregion

    #region Local Fields

    public static Dialogue instance;

    // needs to be public for custom inspector
    [SerializeField]
    private List<Conversation> conversations;

    #endregion

    #region Properties



    #endregion

    #region Public Methods

    public void TriggerDialogue(int a_DialogueIndex)
    {

    }

    #endregion

    #region Unity Life-cycle Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    #region Local Methods



    #endregion
}

public class Conversation : ScriptableObject
{
    public new string name = "Conversation";
    public List<Speach> dialogue = new List<Speach>();
}

[System.Serializable]
public class Speach
{
    public enum Speaker { Clade, Gen };

    public Speaker speaker = Speaker.Clade;

    public string text;

    public Speach()
    {
        speaker = Speaker.Clade;
        text = "";
    }

    public Speach(Speaker a_speaker)
    {
        speaker = a_speaker;
        text = "";
    }
}