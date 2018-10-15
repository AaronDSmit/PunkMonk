using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem landParticleSystem = null;

    private Animator animator;

    private bool done = false;

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
    }

    public void StartedAnimation()
    {
        done = false;
    }

    public void FinishedAnimation()
    {
        done = true;
    }
}
