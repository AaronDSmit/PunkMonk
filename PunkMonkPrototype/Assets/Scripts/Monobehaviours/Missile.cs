using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private GameObject landParticlePrefab = null;

    private Animator animator;

    private bool done = false;

    private bool playedLandParticles = false;

    public bool Done { get { return done; } }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerDown()
    {
        animator.SetTrigger("Down");
    }

    public void TriggerUp()
    {
        animator.SetTrigger("Up");
    }

    public void PlayLandParticles()
    {
        Instantiate(landParticlePrefab, transform.parent.position, landParticlePrefab.transform.rotation);
        playedLandParticles = true;
    }

    public void StartedAnimation()
    {
        done = false;
    }

    public void FinishedAnimation()
    {
        if (playedLandParticles)
        {
            Invoke("Finished", 1);
            playedLandParticles = false;
        }
        else
        {
            done = true;
        }
    }

    private void Finished()
    {
        done = true;
    }
}
