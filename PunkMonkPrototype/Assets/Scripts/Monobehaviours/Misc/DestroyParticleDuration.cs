using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleDuration : MonoBehaviour
{
    private ParticleSystem particle = null;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        Destroy(gameObject, particle.main.duration);
    }

}
