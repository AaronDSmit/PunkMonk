using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private GameObject landParticlePrefab = null;

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
        GameObject particleGO = Instantiate(landParticlePrefab, transform.parent.position, landParticlePrefab.transform.rotation);
    }

    public void StartedAnimation()
    {
        done = false;
    }

    public void FinishedAnimation()
    {
        //Invoke("Finished", landParticles ? landParticleSystem.main.duration : 0);
        done = true;
    }
}
