using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransitionPoint : MonoBehaviour
{
    [SerializeField] private Game_state targetState;
    [SerializeField] private Game_state fromState;

    [HideInInspector]
    [SerializeField] public bool drawText;

    [HideInInspector]
    [SerializeField] public int index;

    [HideInInspector]
    [SerializeField] public int numberToKill;

    [HideInInspector]
    [SerializeField] private Hex earthHex;

    [HideInInspector]
    [SerializeField] private Hex lightningHex;

    [HideInInspector]
    [SerializeField] private Transform earthHexT;

    [HideInInspector]
    [SerializeField] private Transform lightningHexT;

    public Hex LightningHex
    {
        get { return lightningHex; }
    }

    public Hex EarthHex
    {
        get { return earthHex; }
    }

    public void SetEarthHex(Hex a_earthHex)
    {
        earthHex = a_earthHex;

        if (earthHexT)
        {
            DestroyImmediate(earthHexT.gameObject);
        }

        earthHexT = new GameObject("Earth Hex").transform;
        earthHexT.parent = transform;
        earthHexT.position = earthHex.transform.position;
    }

    public void SetLightninghHex(Hex a_lightningHex)
    {
        lightningHex = a_lightningHex;

        if (lightningHexT)
        {
            DestroyImmediate(lightningHexT.gameObject);
        }

        lightningHexT = new GameObject("Earth Hex").transform;
        lightningHexT.parent = transform;
        lightningHexT.position = lightningHexT.transform.position;
    }

    public Game_state TargetState
    {
        get { return targetState; }

        set { targetState = value; }
    }

    public Game_state CurrentState
    {
        get { return fromState; }

        set { fromState = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EarthUnit") || other.CompareTag("LightningUnit"))
        {
            if (Manager.instance.StateController.CurrentGameState == fromState)
            {
                PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                player.EncounterKillLimit = numberToKill;
                player.SetUnitSnapHexes(earthHex, lightningHex);

                Manager.instance.StateController.ChangeStateAfterFade(targetState);

                Destroy(this);
            }
        }
    }
}