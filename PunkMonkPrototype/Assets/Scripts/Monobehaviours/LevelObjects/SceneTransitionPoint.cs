﻿using System.Collections;
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

    bool transitioning = false;

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
        // Make sure this coroutine isn't going twice
        if (transitioning == true)
            yield break;

        transitioning = true;

        Manager.instance.UIController.FadeOut(fadeOutTime);

        if (targetHex)
            Manager.instance.PlayerController.GetComponent<OverworldController>().RunToHex(targetHex);
        else
            Manager.instance.PlayerController.GetComponent<OverworldController>().CanMove = false;
        yield return new WaitForSeconds(fadeOutTime);

        //sceneTransitionEvent.Post(gameObject);

        Manager.instance.TransitionController.Transition(nextLevelIndex);

        transitioning = false;
    }
}