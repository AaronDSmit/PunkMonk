using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("Missile")]
    [SerializeField]
    private ParticleSystem landParticleSystem = null;

    private Animator animator;

    private bool done = false;

    private bool landParticles = false;

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
        landParticleSystem.Play();
        landParticles = true;
    }

    public void StartedAnimation()
    {
        done = false;
    }

    public void FinishedAnimation()
    {
        //Invoke("Finished", landParticles ? landParticleSystem.main.duration : 0);
        Invoke("Finished", landParticles ? 2 : 0);
        landParticles = false;
    }

    private void Finished()
    {
        done = true;
    }
}
