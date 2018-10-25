using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransformPosition : MonoBehaviour
{
    [SerializeField]
    Transform target = null;

    [SerializeField]
    bool realitivePosition = false;

    Vector3 offset;

    private void Awake()
    {
        if (realitivePosition)
        {
            offset = transform.position - target.position;
        }
    }

    private void Update()
    {
        if (realitivePosition)
        {
            transform.position = target.transform.position + offset;
        }
        else
        {
            transform.position = target.transform.position;
        }
    }

}
