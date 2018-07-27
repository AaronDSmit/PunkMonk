using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class Spawner : MonoBehaviour
{
    [SerializeField] private int turnToSpawn;

    [SerializeField] private Entity EntityToSpawn;

    private TextMesh textM;

    private void Awake()
    {
        StateManager.TurnEvent += TurnEvent;
    }

    private void TurnEvent(Turn_state newState, Team team, int turnNumber)
    {
        if (newState == Turn_state.start && turnNumber == turnToSpawn)
        {
            Entity entity = Instantiate(EntityToSpawn, transform.parent.position, Quaternion.identity);
        }
    }

    public int TurnToSpawn
    {
        get { return turnToSpawn; }

        set
        {
            turnToSpawn = value;

            if (!textM)
            {
                textM = GetComponent<TextMesh>();
                textM.anchor = TextAnchor.MiddleCenter;
                textM.alignment = TextAlignment.Center;
            }

            textM.text = turnToSpawn.ToString();
        }
    }
}