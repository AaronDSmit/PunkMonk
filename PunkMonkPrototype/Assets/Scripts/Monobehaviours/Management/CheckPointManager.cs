using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    //private StateTransitionPoint firstEncounter;
    private StateTransitionPoint lastEncounter;
    private CameraController cameraRig;

    private PlayerController player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cameraRig = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();
    }

    public void Init()
    {
        //GameObject[] points = GameObject.FindGameObjectsWithTag("StateTransition");

        //foreach (GameObject point in points)
        //{
        //    if (point.GetComponent<StateTransitionPoint>().index == 0)
        //    {
        //        firstEncounter = point.GetComponent<StateTransitionPoint>();
        //        break;
        //    }
        //}
    }

    public void SetCheckPoint(StateTransitionPoint a_lastEncounter, Vector3 a_position)
    {
        lastEncounter = a_lastEncounter;
    }

    public void ResetLevel()
    {
        StartCoroutine(ResetToFirstEncounter());
    }

    public void ResetToLastCheckPoint()
    {
        StartCoroutine(WaitTillFadeOutToSpawn());
    }

    private IEnumerator ResetToFirstEncounter()
    {
        player.ResetUnitDeaths();
        yield return StartCoroutine(Spawn());

        // find all spawn triggers belonging to the last encounter and enable them again
        StateTransitionPoint[] transitionPoints = FindObjectsOfType<StateTransitionPoint>();

        foreach (StateTransitionPoint point in transitionPoints)
        {
            point.triggered = false;
        }

        // find all spawn triggers belonging to the last encounter and enable them again
        SpawnTrigger[] spawnTriggers = FindObjectsOfType<SpawnTrigger>();

        foreach (SpawnTrigger spawn in spawnTriggers)
        {
            spawn.triggered = false;
        }
    }

    private IEnumerator WaitTillFadeOutToSpawn()
    {
        yield return StartCoroutine(Spawn());

        lastEncounter.triggered = false;

        // find all spawn triggers belonging to the last encounter and enable them again
        SpawnTrigger[] spawnTriggers = FindObjectsOfType<SpawnTrigger>();

        foreach (SpawnTrigger spawn in spawnTriggers)
        {
            if (spawn.index == lastEncounter.index)
            {
                spawn.triggered = false;
            }
        }
    }

    private IEnumerator Spawn()
    {
        Manager.instance.StateController.ChangeStateAfterFade(GameState.overworld);

        yield return new WaitUntil(() => Manager.instance.StateController.MidLoad);

        player.SpawnEarthUnit(lastEncounter.CheckPoint);
        player.SpawnLightningUnit(lastEncounter.CheckPoint);

        player.GetComponent<OverworldController>().Init();
        cameraRig.Init();
    }
}