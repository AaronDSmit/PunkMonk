using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int turnToSpawn;

    [SerializeField] private GameObject EnemyToSpawn;

    private TextMesh textM;

    public int TurnToSpawn
    {
        get { return turnToSpawn; }

        set
        {
            turnToSpawn = value;

            if(!textM)
            {
                textM = GetComponent<TextMesh>();
                textM.anchor = TextAnchor.MiddleCenter;
                textM.alignment = TextAlignment.Center;
            }

            textM.text = turnToSpawn.ToString();
        }
    }
}