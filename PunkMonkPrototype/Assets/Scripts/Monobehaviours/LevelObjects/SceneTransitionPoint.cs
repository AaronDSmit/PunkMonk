using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionPoint : MonoBehaviour
{
    [SerializeField]
    private int nextLevelIndex;

    [HideInInspector]
    [SerializeField]
    public bool drawText;

    [SerializeField]
    private float fadeOutTime = 1;

    [SerializeField]
    private Hex targetHex = null;

    [SerializeField]
    private AK.Wwise.Event sceneTransitionEvent = null;

    public int NextLevelIndex
    {
        get { return nextLevelIndex; }
        set { nextLevelIndex = value; }
    }

    public float FadeOutTime
    {
        get { return fadeOutTime; }
        set { fadeOutTime = value; }
    }

    public Hex TargetHex
    {
        get { return targetHex; }
        set { targetHex = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Manager.instance.StateController.CurrentGameState == GameState.overworld)
        {
            StartCoroutine(TransitionAfterFade());
        }
    }

    private IEnumerator TransitionAfterFade()
    {
        Manager.instance.UIController.FadeOut(fadeOutTime);
        if (targetHex)
            Manager.instance.PlayerController.GetComponent<OverworldController>().RunToHex(targetHex);
        else
            Manager.instance.PlayerController.GetComponent<OverworldController>().CanMove = false;
        yield return new WaitForSeconds(fadeOutTime);
        Manager.instance.TransitionController.Transition(nextLevelIndex);
    }
}