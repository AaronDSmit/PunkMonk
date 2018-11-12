using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour {


    [SerializeField] private Animation creditsRollAnim;
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private Transform creditsPanel;
    [SerializeField] private float endPositionY;
    [SerializeField] private bool isPlaying;


	
	// Update is called once per frame
	void Update () {
        if (creditsRollAnim.isPlaying)
        {
            isPlaying = true;
            if (creditsPanel.position.y >= endPositionY)
            {
                isPlaying = false;
            }
        }

        if (!isPlaying)
        {
            this.gameObject.SetActive(false);
            buttonsPanel.SetActive(true);
        }

	}



}
