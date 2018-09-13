using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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