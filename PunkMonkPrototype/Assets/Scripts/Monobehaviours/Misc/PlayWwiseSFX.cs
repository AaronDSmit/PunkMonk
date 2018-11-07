using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWwiseSFX : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event soundSFX;


    public void PlaySFX()
    {
        soundSFX.Post(gameObject);
    }

}
