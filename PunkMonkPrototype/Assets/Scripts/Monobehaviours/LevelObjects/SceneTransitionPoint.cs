using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionPoint : MonoBehaviour
{
    [SerializeField] private int nextLevelIndex;

    [HideInInspector]
    [SerializeField]
    public bool drawText;

    [SerializeField]
    private float fadeOutTime = 1;

    public int NextLevelIndex
    {
        get { return nextLevelIndex; }
        set { nextLevelIndex = value; }
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
        Manager.instance.PlayerController.GetComponent<OverworldController>().enabled = false;
        yield return new WaitForSeconds(fadeOutTime);
        Manager.instance.TransitionController.Transition(nextLevelIndex);
    }
}