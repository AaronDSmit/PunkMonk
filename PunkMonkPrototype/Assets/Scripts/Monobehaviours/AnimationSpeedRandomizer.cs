using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeedRandomizer : MonoBehaviour
{
    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private float min = 0.01f;
    [SerializeField]
    private float max = 100f;

    void Start()
    {
        animator.speed = Random.Range(min, max);

        Destroy(this);
    }
}
