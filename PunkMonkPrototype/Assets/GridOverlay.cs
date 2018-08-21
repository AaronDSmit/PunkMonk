using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    [SerializeField]
    Transform earthPlayer;

    Renderer myRenderer;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (earthPlayer)
        {
            myRenderer.material.SetVector("_PlayerPos", earthPlayer.position);
        }
    }
}
